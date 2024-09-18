// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace StarStocksWeb.Frameworks.Jobs
{
    public class JobMetadata
    {
        public Guid JobId { get; set; }
        public Type JobType { get; }
        public string JobName { get; }
        public string CronExpression { get; }

        public JobStatus JobStatus { get; set; } = JobStatus.Init;

        public JobMetadata(Guid Id, Type jobType, string jobName, string cronExpression)
        {
            JobId = Id;
            JobType = jobType ?? throw new ArgumentNullException(nameof(jobType));
            CronExpression = cronExpression ?? throw new ArgumentNullException(nameof(cronExpression));
            JobName = jobName ?? throw new ArgumentNullException(nameof(jobName));

        }
    }

    public enum JobStatus : byte
    {
        [Description("初始化")]
        Init = 0,
        [Description("已排程")]
        Scheduled = 1,
        [Description("執行中")]
        Running = 2,
        [Description("已停止")]
        Stopped = 3,
    }
}
