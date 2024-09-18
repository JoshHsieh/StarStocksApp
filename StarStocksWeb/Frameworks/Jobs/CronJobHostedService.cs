// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Spi;

namespace StarStocksWeb.Frameworks.Jobs
{
    public class CronJobHostedService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;

        private readonly IJobFactory _jobFactory;

        private readonly ILogger<CronJobHostedService> _logger;

        private readonly IEnumerable<JobMetadata> _injectJobMetadatas;

        private List<JobMetadata> _allJobMetadatas;

        //private readonly IJobListener _jobListener;

        //private readonly ISchedulerListener _schedulerListener;
        public IScheduler Scheduler { get; set; }

        public CancellationToken CancellationToken { get; private set; }

        public CronJobHostedService(ILogger<CronJobHostedService> logger, ISchedulerFactory schedulerFactory, IJobFactory jobFactory, IEnumerable<JobMetadata> jobMetadatas)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
            _jobFactory = jobFactory ?? throw new ArgumentNullException(nameof(jobFactory));
            //_jobListener = jobListener ?? throw new ArgumentNullException(nameof(jobListener));
            //_schedulerListener = schedulerListener ?? throw new ArgumentNullException(nameof(schedulerListener));
            _injectJobMetadatas = jobMetadatas ?? throw new ArgumentNullException(nameof(jobMetadatas));
        }

        /// <summary>
        /// start cron scheduler
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (Scheduler == null || Scheduler.IsShutdown)
            {
                // 存下 cancellation token 
                CancellationToken = cancellationToken;

                // 先加入在 startup 註冊注入的 Job 工作
                _allJobMetadatas = new List<JobMetadata>();
                _allJobMetadatas.AddRange(_injectJobMetadatas);

                // 再模擬動態加入新 Job 項目 (e.g. 從 DB 來的，針對不同報表能動態決定產出時機)
                //_allJobSchedules.Add(new JobSchedule(jobName: "333", jobType: typeof(ReportJob), cronExpression: "0/13 * * * * ?"));
                //_allJobSchedules.Add(new JobSchedule(jobName: "444", jobType: typeof(ReportJob), cronExpression: "0/20 * * * * ?"));

                // 初始排程器 Scheduler
                Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
                Scheduler.JobFactory = _jobFactory;
                //Scheduler.ListenerManager.AddJobListener(_jobListener);
                //Scheduler.ListenerManager.AddSchedulerListener(_schedulerListener);

                // 逐一將工作項目加入排程器中 
                foreach (var jobSchedule in _allJobMetadatas)
                {
                    var jobDetail = CreateJobDetail(jobSchedule);
                    var trigger = CreateTrigger(jobSchedule);
                    await Scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);
                    jobSchedule.JobStatus = JobStatus.Scheduled;
                }

                // 啟動排程
                await Scheduler.Start(cancellationToken);
            }
        }

        /// <summary>
        /// 停止排程器
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (Scheduler != null && !Scheduler.IsShutdown)
            {
                _logger.LogInformation($"@{DateTime.Now:HH:mm:ss} - Scheduler StopAsync");

                await Scheduler.Shutdown(cancellationToken);
            }
        }

        /// <summary>
        /// 取得所有作業的最新狀態
        /// </summary>
        public async Task<IEnumerable<JobMetadata>> GetJobMetadatas()
        {
            if (Scheduler.IsShutdown)
            {
                // 排程器停止時更新各工作狀態為停止
                foreach (var jobMeta in _allJobMetadatas)
                {
                    jobMeta.JobStatus = JobStatus.Stopped;
                }
            }
            else
            {
                // 取得目前正在執行的 Job 來更新各 Job 狀態
                var executingJobs = await Scheduler.GetCurrentlyExecutingJobs();

                foreach (var jobMeta in _allJobMetadatas)
                {
                    var isRunning = executingJobs.FirstOrDefault(j => j.JobDetail.Key.Name == jobMeta.JobName) != null;
                    jobMeta.JobStatus = isRunning ? JobStatus.Running : JobStatus.Scheduled;
                }
            }

            return _allJobMetadatas;
        }

        /// <summary>
        /// 手動觸發作業
        /// </summary>
        public async Task TriggerJobAsync(string jobName)
        {
            if (Scheduler != null && !Scheduler.IsShutdown)
            {
                _logger.LogInformation($"@{DateTime.Now:HH:mm:ss} - job{jobName} - TriggerJobAsync");

                await Scheduler.TriggerJob(new JobKey(jobName), CancellationToken);
            }
        }

        /// <summary>
        /// 手動中斷作業
        /// </summary>
        public async Task InterruptJobAsync(string jobName)
        {
            if (Scheduler != null && !Scheduler.IsShutdown)
            {
                var targetExecutingJob = await GetExecutingJob(jobName);

                if (targetExecutingJob != null)
                {
                    _logger.LogInformation($"@{DateTime.Now:HH:mm:ss} - job{jobName} - InterruptJobAsync");
                    await Scheduler.Interrupt(new JobKey(jobName));
                }

            }
        }

        /// <summary>
        /// 取得特定執行中的作業
        /// </summary>
        private async Task<IJobExecutionContext> GetExecutingJob(string jobName)
        {
            if (Scheduler != null)
            {
                var executingJobs = await Scheduler.GetCurrentlyExecutingJobs();
                return executingJobs.FirstOrDefault(j => j.JobDetail.Key.Name == jobName);
            }

            return null;
        }

        /// <summary>
        /// 建立作業細節 (後續會透過 JobFactory 依此資訊從 DI 容器取出 Job 實體)
        /// </summary>
        private IJobDetail CreateJobDetail(JobMetadata jobMeta)
        {
            var jobType = jobMeta.JobType;
            var jobDetail = JobBuilder
                .Create(jobType)
                .WithIdentity(jobMeta.JobId.ToString())
                .WithDescription($"{jobMeta.JobName}")
                .Build();

            // 可以在建立 job 時傳入資料給 job 使用
            jobDetail.JobDataMap.Put("OptionChainJob", jobMeta);

            return jobDetail;
        }

        /// <summary>
        /// 產生觸發器
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        private ITrigger CreateTrigger(JobMetadata jobMeta)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity($"{jobMeta.JobName}.trigger")
                .WithCronSchedule(jobMeta.CronExpression)
                .WithDescription(jobMeta.CronExpression)
                .Build();
        }
    }
}
