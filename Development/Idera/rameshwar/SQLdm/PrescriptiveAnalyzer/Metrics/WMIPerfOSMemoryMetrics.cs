using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WMIPerfOSMemoryMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WMIPerfOSMemoryMetrics");
        private WMIPerfOSMemorySnapshots _snapshots = new WMIPerfOSMemorySnapshots();

        public double AvgPagesPerSecond { get { return _snapshots.AvgPagesPerSecond; } }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WMIPerfOSMemoryMetrics not added : " + snapshot.Error); return; }
            
            if (snapshot.WmiPerfOSMemorySnapshotValueShutdown == null) { return; }
            if (snapshot.WmiPerfOSMemorySnapshotValueShutdown.WmiPerfOSMemory != null && snapshot.WmiPerfOSMemorySnapshotValueShutdown.WmiPerfOSMemory.Rows.Count > 0)
            {
                for (int index = 0; index < snapshot.WmiPerfOSMemorySnapshotValueShutdown.WmiPerfOSMemory.Rows.Count; index++)
                {
                    WMIPerfOSMemory obj = new WMIPerfOSMemory();
                    try
                    {
                        obj.PagesPerSecond = (uint)snapshot.WmiPerfOSMemorySnapshotValueShutdown.WmiPerfOSMemory.Rows[index]["PagesPersec"];
                        obj.Timestamp_Sys100NS = (ulong)snapshot.WmiPerfOSMemorySnapshotValueShutdown.WmiPerfOSMemory.Rows[index]["Timestamp_Sys100NS"];
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }

                    _snapshots.Add(obj);
                }
            }
        }
    }

    internal class WMIPerfOSMemorySnapshots : List<WMIPerfOSMemory>
    {
        internal double AvgPagesPerSecond
        {
            get
            {
                if (Count <= 1) return (0);
                try
                {
                    //WMIPerfOSMemory last = this.Last();
                    WMIPerfOSMemory last = this[this.Count - 1];
                    double d = (last.Timestamp_Sys100NS - this[0].Timestamp_Sys100NS) / 10000000;
                    if (0 != d)
                    {
                        //Average formula: (Nx - N0) / ((Dx - D0) / F)
                        return (last.PagesPerSecond - this[0].PagesPerSecond) / d;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log("WMIPerfOSSystemSnapshots.ContextSwitchesPerSec Exception:", ex);
                }

                return 0.0d;
            }
        }

    }

    internal class WMIPerfOSMemory
    {
        internal UInt32 PagesPerSecond { get;  set; }
        internal UInt64 Timestamp_Sys100NS { get;  set; }
    }
}
