using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.WMI
{
    internal class WMIPerfDiskPhysicalDiskSnapshots : List<WMIPerfDiskPhysicalDisk>
    {
        internal UInt32 AvgDiskTransfersPerSec()
        {
            if (this.Count <= 1) return (0);
            var last = this.Last();
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
}
