using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Values
{
    [Serializable]
    public class SnapshotMetrics
    {

        //Typically the metrics I would look at are:
        //  •	CPU high for 30 seconds 
        //      o	Happy if its ave over 30 secs < 50%
        //      o	Serious if its ave over 30 secs > 75%
        //      o	Critical if its ave over 30 secs > 90%
        //  •	Network congested for 30 seconds
        //      o	Retransmits > 4% serious
        //      o	Retransmits > 10% critical
        //  •	IO congested for 30 seconds
        //      o	Disk queue length ave > 15 over 30 secs is a serious problem
        //      o	Disk queue length ave > 25 over 30 secs is a critical problem 
        //  •	Memory squeeze
        //      o	SQL Server been up for more than 1 hour and page-life-expectancy < 300 secs – serious
        //      o	SQL Server been up for more than 1 hour and page-life-expectancy < 100 secs – critical 
        //  •	Substantial blocking for many seconds
        //      o	1 process blocked < 15 secs fine
        //      o	1 process blocked > 30 secs serious
        //      o	Multiple processes blocked > 30 secs critical
        //  •	Open / long running transaction
        //      o	> 1 hour – Serious
        //      o	> 6 hours - critical

        #region CPU Metrics

        public double ProcessorUsagePercent { get; set; }
        public double ProcessorUsagePercentHigh { get; set; }

        #endregion

        #region Network Metrics

        public double RetransmitPercent { get; set; }

        #endregion

        #region IO Metrics

        public double AverageDiskQueueLength { get; set; }
        public double AverageDiskQueueLengthHigh { get; set; }
        public string AverageDiskQueueLengthHighName;

        #endregion

        #region Memory Metrics

        public UInt64 PageLifeExpectancy { get; set; }

        #endregion

        #region Blocking Metrics

        public UInt64 LongestBlockingTimeInSeconds { get; set; }
        public int BlockingProcessCount { get; set; }

        #endregion

        #region Transaction Metrics

        public UInt64 LongestRunningTransactionInSeconds { get; set; }

        #endregion

    }
}
