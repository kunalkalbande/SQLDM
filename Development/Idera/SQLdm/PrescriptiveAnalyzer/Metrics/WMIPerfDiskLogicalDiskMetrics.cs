using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WMIPerfDiskLogicalDiskMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WMIPerfDiskLogicalDiskMetrics");
        private Dictionary<string, WMIPerfDiskLogicalDiskSnapshots> _disks = new Dictionary<string, WMIPerfDiskLogicalDiskSnapshots>();

        public string[] LogicalDiskNames 
        { 
            get 
            {
                string[] array = new string[_disks.Count];
                int index = 0;
                foreach (string str in _disks.Keys)
                {
                    array[index] = str;
                    index++;
                }
                //return _disks.Keys.ToArray(); 
                return array;
            } 
        }

        public double GetAverageDiskQueueLength()
        {
            int count = 0;
            double queue = 0;
            foreach (var s in _disks.Values)
            {
                if (null != s)
                {
                    ++count;
                    queue += s.AverageDiskQueueLength();
                }
            }
            return (count <= 1 ? queue : queue / count);
        }

        public double GetHighAverageDiskQueueLength()
        {
            double high = 0;
            foreach (var s in _disks.Values)
            {
                if (null != s)
                {
                    high = Math.Max(s.AverageDiskQueueLength(), high);
                }
            }
            return (high);
        }

        public double GetHighAverageDiskQueueLength(out string name)
        {
            double high = 0;
            name = "";
            foreach (WMIPerfDiskLogicalDiskSnapshots s in _disks.Values)
            {
                if (null != s)
                {
                    double temp = s.AverageDiskQueueLength();
                    if (temp > high)
                    {
                        high = temp;
                        name = s[0].Name;
                    }
                }
            }
            return (high);
        }

        public double? GetAverageDiskQueueLength(string disk)
        {
            WMIPerfDiskLogicalDiskSnapshots snapshot;
            if (_disks.TryGetValue(disk, out snapshot))
                return snapshot.AverageDiskQueueLength();

            return null;
        }

        public double? GetAveragePercentDiskTime(string disk)
        {
            WMIPerfDiskLogicalDiskSnapshots snapshot;
            if (_disks.TryGetValue(disk, out snapshot))
                return snapshot.AveragePercentDiskTime();

            return null;
        }

        public double? GetAverageDiskSecondsPerTransfer(string disk)
        {
            WMIPerfDiskLogicalDiskSnapshots snapshot;
            if (_disks.TryGetValue(disk, out snapshot))
                return snapshot.AverageDiskSecondsPerTransfer();

            return null;
        }

        public double? GetAverageSplitIoPerSecond(string disk)
        {
            WMIPerfDiskLogicalDiskSnapshots snapshot;
            if (_disks.TryGetValue(disk, out snapshot))
                return snapshot.AverageSplitIoPerSecond();

            return null;
        }

        public double? GetAverageDiskTransfersPerSecond(string disk)
        {
            WMIPerfDiskLogicalDiskSnapshots snapshot;
            if (_disks.TryGetValue(disk, out snapshot))
                return snapshot.AverageDiskTransfersPerSecond();

            return null;
        }

        //public IEnumerable<string> FindDiskNames(Func<WMIPerfDiskLogicalDiskSnapshots, bool> predicate)
        //{
        //    return _disks.Where(kvp => predicate.Invoke(kvp.Value)).Select(kvp => kvp.Key);
        //}

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            AddSnapshot(snapshot.WmiPerfDiskLogicalDiskSnapshotValueStartup);
            AddSnapshot(snapshot.WmiPerfDiskLogicalDiskSnapshotValueShutdown);
        }

        private void AddSnapshot(Idera.SQLdm.Common.Snapshots.WmiPerfDiskLogicalDiskSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WMIPerfDiskLogicalDiskMetrics not added : " + snapshot.Error); return; }
            if (snapshot.WmiPerfDiskLogicalDisk != null && snapshot.WmiPerfDiskLogicalDisk.Rows.Count > 0)
            {
                WMIPerfDiskLogicalDiskSnapshots pcs = null;
                for (int index = 0; index < snapshot.WmiPerfDiskLogicalDisk.Rows.Count; index++)
                {
                    WMIPerfDiskLogicalDisk obj = new WMIPerfDiskLogicalDisk();

                    try
                    {
                        obj.Name = (string)snapshot.WmiPerfDiskLogicalDisk.Rows[index]["Name"];
                        obj.AvgDiskQueueLength = (ulong)snapshot.WmiPerfDiskLogicalDisk.Rows[index]["AvgDiskQueueLength"];
                        obj.CurrentDiskQueueLength = (uint)snapshot.WmiPerfDiskLogicalDisk.Rows[index]["CurrentDiskQueueLength"];
                        obj.DiskTransfersPerSecond = (uint)snapshot.WmiPerfDiskLogicalDisk.Rows[index]["DiskTransfersPerSec"];
                        obj.SplitIOPerSec = (uint)snapshot.WmiPerfDiskLogicalDisk.Rows[index]["SplitIOPerSec"];
                        obj.PercentDiskTime = (ulong)snapshot.WmiPerfDiskLogicalDisk.Rows[index]["PercentDiskTime"];
                        obj.PercentDiskTimeBase = (ulong)snapshot.WmiPerfDiskLogicalDisk.Rows[index]["PercentDiskTime_Base"];
                        obj.AvgDisksecPerTransfer = (uint)snapshot.WmiPerfDiskLogicalDisk.Rows[index]["AvgDisksecPerTransfer"];
                        obj.AvgDisksecPerTransfer_Base = (uint)snapshot.WmiPerfDiskLogicalDisk.Rows[index]["AvgDisksecPerTransfer_Base"];
                        obj.Timestamp_Sys100NS = (ulong)snapshot.WmiPerfDiskLogicalDisk.Rows[index]["Timestamp_Sys100NS"];
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                    if (!_disks.TryGetValue(obj.Name, out pcs))
                    {
                        _disks.Add(obj.Name, pcs = new WMIPerfDiskLogicalDiskSnapshots());
                    }
                    pcs.Add(obj);
                }
            }
        }

    }

    internal class WMIPerfDiskLogicalDiskSnapshots : List<WMIPerfDiskLogicalDisk>
    {
        private static Logger _logX = Logger.GetLogger("WMIPerfDiskLogicalDiskSnapshots");
        internal double AverageDiskQueueLength()
        {
            if (this.Count > 1)
            {
                //WMIPerfDiskLogicalDisk first = this.First();
                WMIPerfDiskLogicalDisk first = this[0];
                //WMIPerfDiskLogicalDisk last = this.Last();
                WMIPerfDiskLogicalDisk last = this[this.Count - 1];

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
                //value = this.Max(num => num.CurrentDiskQueueLength);
                foreach (WMIPerfDiskLogicalDisk item in this)
                {
                    if (item.CurrentDiskQueueLength > value)
                    {
                        value = item.CurrentDiskQueueLength;
                    }
                }
                _logX.DebugFormat("Avg. Disk Queue Length skipped due to lack of data (samples={0})", this.Count);
            }
            else
                _logX.Debug("Max disk queue length is 0 because count < 1");

            return value;
        }

        internal double AveragePercentDiskTime()
        {
            if (this.Count > 1)
            {
                //WMIPerfDiskLogicalDisk first = this.First();
                //WMIPerfDiskLogicalDisk last = this.Last();
                WMIPerfDiskLogicalDisk first = this[0];
                WMIPerfDiskLogicalDisk last = this[this.Count - 1];

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
                //WMIPerfDiskLogicalDisk first = this.First();
                //WMIPerfDiskLogicalDisk last = this.Last();
                WMIPerfDiskLogicalDisk first = this[0];
                WMIPerfDiskLogicalDisk last = this[this.Count - 1];

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
                //WMIPerfDiskLogicalDisk first = this.First();
                //WMIPerfDiskLogicalDisk last = this.Last();
                WMIPerfDiskLogicalDisk first = this[0];
                WMIPerfDiskLogicalDisk last = this[this.Count - 1];

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
                //WMIPerfDiskLogicalDisk first = this.First();
                //WMIPerfDiskLogicalDisk last = this.Last();
                WMIPerfDiskLogicalDisk first = this[0];
                WMIPerfDiskLogicalDisk last = this[this.Count - 1];

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

    internal class WMIPerfDiskLogicalDisk
    {
        public string Name { get;  set; }
        public UInt64 AvgDiskQueueLength { get;  set; }
        public UInt32 CurrentDiskQueueLength { get;  set; }
        public UInt32 DiskTransfersPerSecond { get;  set; }
        public UInt32 SplitIOPerSec { get;  set; }
        public UInt64 PercentDiskTime { get;  set; }
        public UInt64 PercentDiskTimeBase { get;  set; }
        public UInt32 AvgDisksecPerTransfer { get;  set; }
        public UInt32 AvgDisksecPerTransfer_Base { get;  set; }
        public UInt64 Timestamp_Sys100NS { get;  set; }

        public WMIPerfDiskLogicalDisk() { }

    }
}
