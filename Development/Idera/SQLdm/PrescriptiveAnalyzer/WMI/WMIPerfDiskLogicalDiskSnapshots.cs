using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TracerX;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.WMI
{
    internal class WMIPerfDiskLogicalDiskSnapshots : List<WMIPerfDiskLogicalDisk>
    {
        private static Logger _logX = Logger.GetLogger("WMIPerfDiskLogicalDiskSnapshots");
        internal double AverageDiskQueueLength()
        {
            if (this.Count > 1)
            {
                WMIPerfDiskLogicalDisk first = this.First();
                WMIPerfDiskLogicalDisk last = this.Last();

                double countervalue = last.AvgDiskQueueLength - first.AvgDiskQueueLength;
                double timerbase = last.Timestamp_Sys100NS - first.Timestamp_Sys100NS;
                _logX.VerboseFormat("Avg Disk Queue Length counter={0} base={1}", countervalue, timerbase);
                if (timerbase != 0)
                    return (countervalue / timerbase);
            }
            else
                _logX.DebugFormat("Avg. Disk Queue Length skipped due to lack of data (samples={0})", this.Count);

            return 0.0d;
        }

        internal uint MaxDiskQueueLength()
        {
            uint value = 0;
            if (this.Count > 0)
            {
                value = this.Max(num => num.CurrentDiskQueueLength);
                _logX.VerboseFormat("Max disk queue length is {0}", value);
            }
            else
                _logX.Debug("Max disk queue length is 0 because count < 1");

            return value;
        }

        internal double AveragePercentDiskTime()
        {
            if (this.Count > 1)
            {
                WMIPerfDiskLogicalDisk first = this.First();
                WMIPerfDiskLogicalDisk last = this.Last();

                double countervalue = last.PercentDiskTime - first.PercentDiskTime;
                _logX.VerboseFormat("Avg Percent Disk Time last.counter={0} first.counter={1}", last.PercentDiskTime, first.PercentDiskTime);
                ulong counterbase = last.PercentDiskTimeBase - first.PercentDiskTimeBase;
                _logX.VerboseFormat("Avg Percent Disk Time last.base={0} first.base={1}", last.PercentDiskTimeBase, first.PercentDiskTimeBase);
                _logX.VerboseFormat("Avg Percent Disk Time counter={0} base={1}", countervalue, counterbase);
                if (counterbase != 0)
                    return (countervalue / counterbase);
            }
            else
                _logX.DebugFormat("Avg. Pct. Disk Time skipped due to lack of data (samples={0})", this.Count);
 
            return 0.0d;
        }

        internal double AverageDiskSecondsPerTransfer()
        {
            if (this.Count > 1)
            {
                WMIPerfDiskLogicalDisk first = this.First();
                WMIPerfDiskLogicalDisk last = this.Last();

                double countervalue = last.AvgDisksecPerTransfer - first.AvgDisksecPerTransfer;
                long counterbase = last.AvgDisksecPerTransfer_Base - first.AvgDisksecPerTransfer_Base;
                _logX.VerboseFormat("Avg. Disk Sec/transfer counter={0} base={1}", countervalue, counterbase);
                if (counterbase > 0)
                    return Math.Round((countervalue / 10000000.0) / counterbase, 4);
            }
            else 
                _logX.DebugFormat("Avg. Disk Sec/transfer skipped due to lack of data (samples={0})", this.Count);
            
            return 0.0d;
        }

        internal double AverageSplitIoPerSecond()
        {
            if (this.Count > 1)
            {
                WMIPerfDiskLogicalDisk first = this.First();
                WMIPerfDiskLogicalDisk last = this.Last();

                double countervalue = last.SplitIOPerSec - first.SplitIOPerSec;
                double timerbase = last.Timestamp_Sys100NS - first.Timestamp_Sys100NS;
                _logX.VerboseFormat("Avg. Split IO/sec counter={0} timerbase={1}", countervalue, timerbase);
                if (timerbase > 0)
                    return Math.Round(countervalue / (timerbase / 10000000.0), 2);
            }
            else
                _logX.DebugFormat("Avg. Split IO/sec skipped due to lack of data (samples={0})", this.Count);

            return 0.0d;
        }

        internal double AverageDiskTransfersPerSecond()
        {
            if (this.Count > 1)
            {
                WMIPerfDiskLogicalDisk first = this.First();
                WMIPerfDiskLogicalDisk last = this.Last();

                double countervalue = last.DiskTransfersPerSecond - first.DiskTransfersPerSecond;
                double timerbase = last.Timestamp_Sys100NS - first.Timestamp_Sys100NS;
                _logX.VerboseFormat("Avg. Disk Transfers/sec counter={0} timerbase={1}", countervalue, timerbase);
                if (timerbase > 0)
                    return Math.Round(countervalue / (timerbase / 10000000.0), 2);
            }
            else
                _logX.DebugFormat("Avg. Disk Transfers/sec skipped due to lack of data (samples={0})", this.Count);

            return 0.0d;
        }
    }
}
