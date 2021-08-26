using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WMIPerfDiskPhysicalDiskMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WMIPerfDiskPhysicalDiskMetrics");
        private Dictionary<string, WMIPerfDiskPhysicalDiskSnapshots> _disks = new Dictionary<string, WMIPerfDiskPhysicalDiskSnapshots>();

        internal UInt32 GetAvgDiskTransfersPerSec(string disk)
        {
            WMIPerfDiskPhysicalDiskSnapshots snapshot;
            if (!_disks.TryGetValue(disk, out snapshot)) return (0);
            if (null == snapshot) return (0);
            return (snapshot.AvgDiskTransfersPerSec());
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            AddSnapshot(snapshot.WmiPerfDiskPhysicalDiskSnapshotValueStartup);
            AddSnapshot(snapshot.WmiPerfDiskPhysicalDiskSnapshotValueShutdown);
        }

        private void AddSnapshot(Idera.SQLdm.Common.Snapshots.WmiPerfDiskPhysicalDiskSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WMIPerfDiskPhysicalDiskMetrics not added : " + snapshot.Error); return; }
            
            if (snapshot.WmiPerfDiskPhysicalDisk != null && snapshot.WmiPerfDiskPhysicalDisk.Rows.Count > 0)
            {
                WMIPerfDiskPhysicalDiskSnapshots pcs = null;
                for (int index = 0; index < snapshot.WmiPerfDiskPhysicalDisk.Rows.Count; index++)
                {
                    WMIPerfDiskPhysicalDisk obj = new WMIPerfDiskPhysicalDisk();

                    try
                    {
                        obj.Name = (string)snapshot.WmiPerfDiskPhysicalDisk.Rows[index]["Name"];
                        obj.DiskTransfersPerSec = (uint)snapshot.WmiPerfDiskPhysicalDisk.Rows[index]["DiskTransfersPerSec"];
                        obj.Frequency_Sys100NS = (ulong)snapshot.WmiPerfDiskPhysicalDisk.Rows[index]["Frequency_Sys100NS"];
                        obj.Timestamp_Sys100NS = (ulong)snapshot.WmiPerfDiskPhysicalDisk.Rows[index]["Timestamp_Sys100NS"];
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                    if (!_disks.TryGetValue(obj.Name, out pcs))
                    {
                        _disks.Add(obj.Name, pcs = new WMIPerfDiskPhysicalDiskSnapshots());
                    }
                    pcs.Add(obj);
                }
            }
        }
    }

    internal class WMIPerfDiskPhysicalDiskSnapshots : List<WMIPerfDiskPhysicalDisk>
    {
        internal UInt32 AvgDiskTransfersPerSec()
        {
            if (this.Count <= 1) return (0);
            //var last = this.Last();
            var last = this[this.Count - 1];
            if (null == last) return (0);
            var Nx = last.DiskTransfersPerSec;
            var N0 = this[0].DiskTransfersPerSec;
            var Dx = last.Timestamp_Sys100NS;
            var D0 = this[0].Timestamp_Sys100NS;
            var F = last.Frequency_Sys100NS;
            if (0 == F) F = 100;
            var D = Dx - D0;
            var N = Nx - N0;
            if (0 == D) return (N);
            // Average: (Nx - N0) / ((Dx - D0) / F)
            return (UInt32)Math.Round(((double)N / ((double)D / F)));
        }
    }

    internal class WMIPerfDiskPhysicalDisk
    {
        public string Name { get;  set; }
        public UInt32 DiskTransfersPerSec { get;  set; }
        public UInt64 Frequency_Sys100NS { get;  set; }
        public UInt64 Timestamp_Sys100NS { get;  set; }

        public WMIPerfDiskPhysicalDisk() { }
    }
}
