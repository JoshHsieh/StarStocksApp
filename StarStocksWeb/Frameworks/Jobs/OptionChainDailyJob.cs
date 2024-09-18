// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using StarStocks.Core.Interfaces;
using StarStocks.Core.Extensions;
using StarStocks.Core.Models;
using StarStocks.Core.Services;
using StarStocks.Core.DbWrapper;

namespace StarStocksWeb.Frameworks.Jobs
{
    [DisallowConcurrentExecution]
    public class OptionChainDailyJob : IJob
    {
        private readonly ILogger<OptionChainDailyJob> _logger;

        private readonly IServiceProvider _provider;

        private readonly IConfiguration _config;

        public OptionChainDailyJob(ILogger<OptionChainDailyJob> logger, IServiceProvider provider, IConfiguration config)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            // 可取得自定義的 JobSchedule 資料, 可根據 JobSchedule 提供的內容建立不同 report 資料
            var jobMeta = context.JobDetail.JobDataMap.Get("OptionChainJob") as JobMetadata;
            var jobName = jobMeta.JobName;

            _logger.LogInformation($"@{DateTime.Now:HH:mm:ss} - job{jobName} - start");

            // job parameter(we can retrive these from db)
            var d = new Dictionary<string, string>();
            d.Add("symbol", "$SPX.X");
            d.Add("strikeCount", "60");
            d.Add("toDate", DateTime.UtcNow.Date.ToEstNy().AddDays(1).ToString("yyyy-MM-dd"));

            var spyD = new Dictionary<string, string>();
            spyD.Add("symbol", "SPY");
            spyD.Add("strikeCount", "30");
            spyD.Add("toDate", DateTime.UtcNow.Date.ToEstNy().AddDays(1).ToString("yyyy-MM-dd"));

            using (var scope = _provider.CreateScope())
            {
                // 如果要使用到 DI 容器中定義為 Scope 的物件實體時，由於 Job 定義為 singleton
                // 因此無法直接取得 Scope 的實體，此時就需要於 CreateScope 在 scope 中產生該實體
                // ex. var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
                var fetchTdApi = scope.ServiceProvider.GetRequiredService<IService<OptionChainDaily>>();

                IResult parserSpxResult = await fetchTdApi.DoService(d);

                IResult parserSpyResult = await fetchTdApi.DoService(spyD);
            }

            //for (int i = 0; i < 2; i++)
            //{

            //    // 自己定義當 job 要被迫被被中斷時，哪邊適合結束
            //    // 如果沒有設定，當作業被中斷時，並不會真的中斷，而會整個跑完
            //    if (context.CancellationToken.IsCancellationRequested)
            //    {
            //        break;
            //    }

            //    System.Threading.Thread.Sleep(1000);

            //    string connStr = _config.GetSection("ConnectionStrings")["AlgoDataDbConnection"];

            //    _logger.LogInformation($"@{DateTime.Now:HH:mm:ss} - job{jobName} - working{i} : Print ConntionStr : {connStr}");

            //}

            _logger.LogInformation($"@{DateTime.Now:HH:mm:ss} - job{jobName} - done");

            //return Task.CompletedTask;
        }
    }
}
