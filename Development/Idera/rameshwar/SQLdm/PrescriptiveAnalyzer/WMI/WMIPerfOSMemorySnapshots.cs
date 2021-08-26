using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.WMI
{
    internal class WMIPerfOSMemorySnapshots : List<WMIPerfOSMemory>
    {
        internal double AvgPagesPerSecond
        {
            get
            {
                if (Count <= 1) return (0);
                try
                {
                    WMIPerfOSMemory last = this.Last();
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
}
