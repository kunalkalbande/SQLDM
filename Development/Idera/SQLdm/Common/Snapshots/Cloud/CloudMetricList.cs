using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Common.Snapshots.Cloud
{
    public class CloudMetricList
    {
        public enum AWSMetric
        {
            CPUUtilization,
            CPUCreditBalance,
            CPUCreditUsage,
            DiskQueueDepth,
            ReadLatency,
            ReadThroughput,
            SwapUsage,
            WriteLatency,
            
            FreeableMemory,
            WriteThroughput,
            CPUCreditBalanceHigh,
            ReadLatencyLow,
            WriteLatencyLow
        }

        public enum AzureMetric
        {
            AverageDataIOPercent,
            AverageLogWritePercent,
            MaxWorkerPercent,
            MaxSessionPercent,
            ServiceTierChanges,
            DatabaseAverageMemoryUsagePercent,
            InMemoryStorageUsagePercent,

            //START 5.4.2
            AverageDataIOPercentLow,
            AverageLogWritePercentLow,
            MaxWorkerPercentLow,
            MaxSessionPercentLow,
            DatabaseAverageMemoryUsagePercentLow,
            InMemoryStorageUsagePercentLow
            //END 5.4.2
        }
    }
}
