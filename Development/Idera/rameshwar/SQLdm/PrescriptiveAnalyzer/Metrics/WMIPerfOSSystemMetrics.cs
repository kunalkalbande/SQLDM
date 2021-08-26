using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using System.Data;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WMIPerfOSSystemMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WMIPerfOSSystemMetrics");
        private WMIPerfOSSystemSnapshots _snapshots = new WMIPerfOSSystemSnapshots();
        public double AvgProcessorQueueLength { get { return (_snapshots.AvgProcessorQueueLength); } }
        public UInt32 ContextSwitchesPerSec { get { return (_snapshots.ContextSwitchesPerSec); } }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WMIPerfOSSystemMetrics not added : " + snapshot.Error); return; }
            if (snapshot.WmiPerfOSSystemSnapshotValueStartup == null) { return; }
            AddSnapshot(snapshot.WmiPerfOSSystemSnapshotValueStartup.WmiPerfOSSystem);
            if (snapshot.WmiPerfOSSystemSnapshotValueInterval != null && snapshot.WmiPerfOSSystemSnapshotValueInterval.ListWmiPerfOSSystem != null)
            {
                foreach (var dt in snapshot.WmiPerfOSSystemSnapshotValueInterval.ListWmiPerfOSSystem)
                {
                    AddSnapshot(dt);
                }
            }
            //This is older code as per older approach of having multiple snapshots. Now we are having one snapshot with multiple datatables 
            //foreach (Idera.SQLdm.Common.Snapshots.WmiPerfOSSystemSnapshot snp in snapshot.WmiPerfOSSystemSnapshotValueInterval)
            //{
            //    AddSnapshot(snp);
            //}
        }

        private void AddSnapshot(DataTable dt)
        {
            if (dt == null) { return; }
            if (dt.Rows.Count > 0)
            {
                for (int index = 0; index < dt.Rows.Count; index++)
                {
                    WMIPerfOSSystem obj = new WMIPerfOSSystem();

                    try
                    {
                        obj.ProcessorQueueLength = (uint)dt.Rows[index]["ProcessorQueueLength"];
                        obj.ContextSwitchesPerSec = (uint)dt.Rows[index]["ContextSwitchesPerSec"];
                        obj.Frequency_Sys100NS = (ulong)dt.Rows[index]["Frequency_Sys100NS"];
                        obj.Timestamp_Sys100NS = (ulong)dt.Rows[index]["Timestamp_Sys100NS"];
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                    _snapshots.Add(obj);
                }
            }
        }
    }

    internal class WMIPerfOSSystemSnapshots : List<WMIPerfOSSystem>
    {
        public UInt32 ContextSwitchesPerSec
        {
            get
            {
                if (Count <= 1) return (0);
                try
                {
                    //WMIPerfOSSystem last = this.Last();
                    WMIPerfOSSystem last = this[this.Count - 1];
                    UInt64 x0 = this[0].ContextSwitchesPerSec;
                    UInt64 y0 = this[0].Timestamp_Sys100NS;
                    UInt64 x1 = last.ContextSwitchesPerSec;
                    UInt64 y1 = last.Timestamp_Sys100NS;
                    UInt64 d = (y1 - y0);
                    if (0 == d) return (0);
                    if (0 != this[0].Frequency_Sys100NS) d /= this[0].Frequency_Sys100NS;
                    if (0 == d) return (0);
                    //Average formula: (Nx - N0) / ((Dx - D0) / F)
                    return ((UInt32)((x1 - x0) / d));
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log("WMIPerfOSSystemSnapshots.ContextSwitchesPerSec Exception:", ex);
                }
                return (0);
            }
        }

        public double AvgProcessorQueueLength
        {
            get
            {
                if (Count <= 0) return (0);
                UInt32 l = 0;
                foreach (WMIPerfOSSystem s in this)
                {
                    if (null != s) l += s.ProcessorQueueLength;
                }
                return ((double)l / (double)Count);
            }
        }
    }

    internal class WMIPerfOSSystem
    {
        public UInt32 ProcessorQueueLength { get;  set; }
        public UInt32 ContextSwitchesPerSec { get;  set; }
        public UInt64 Frequency_Sys100NS { get;  set; }
        public UInt64 Timestamp_Sys100NS { get;  set; }

        public WMIPerfOSSystem() { }

    }

}
