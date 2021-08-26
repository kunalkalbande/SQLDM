using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WMIPhysicalMemoryMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WMIPhysicalMemoryMetrics");
        private List<WMIPhysicalMemory> _mem = new List<WMIPhysicalMemory>();

        public UInt64 TotalCapacity
        {
            get
            {
                UInt64 r = 0;
                foreach (var m in _mem) { if (null != m) r += m.Capacity; }
                return (r);
            }
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WMIPhysicalMemoryMetrics not added : " + snapshot.Error); return; }
            if (snapshot.WmiPhysicalMemorySnapshotValueShutdown == null) { return; }
            if (snapshot.WmiPhysicalMemorySnapshotValueShutdown.WmiPhysicalMemory != null && snapshot.WmiPhysicalMemorySnapshotValueShutdown.WmiPhysicalMemory.Rows.Count > 0)
            {
                for (int index = 0; index < snapshot.WmiPhysicalMemorySnapshotValueShutdown.WmiPhysicalMemory.Rows.Count; index++)
                {
                    WMIPhysicalMemory obj = new WMIPhysicalMemory();

                    try
                    {
                        obj.Caption = (string)snapshot.WmiPhysicalMemorySnapshotValueShutdown.WmiPhysicalMemory.Rows[index]["Caption"];
                        obj.Capacity = (ulong)snapshot.WmiPhysicalMemorySnapshotValueShutdown.WmiPhysicalMemory.Rows[index]["Capacity"];
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }

                    _mem.Add(obj);
                }
            }
        }
    }

    internal class WMIPhysicalMemory
    {
        public string Caption { get;  set; }
        public UInt64 Capacity { get;  set; }

        public WMIPhysicalMemory() { }

        public override string ToString()
        {
            return (string.Format("WMIPhysicalMemory - Caption: {0}  Capacity: {1}", Caption, Capacity));
        }
    }
}
