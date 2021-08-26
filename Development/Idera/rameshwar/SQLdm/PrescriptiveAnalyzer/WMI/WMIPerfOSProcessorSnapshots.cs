using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WMI
{
    internal class WMIPerfOSProcessorSnapshots : List<WMIPerfOSProcessor>
    {
        internal string Name
        {
            get
            {
                if (this.Count <= 0) return (string.Empty);
                return (this[0].Name);
            }
        }
        internal UInt32 AvgInterruptsPerSec
        {
            get
            {
                if (this.Count <= 1) return (0);
                //var last = this.Last();
                var last = this[this.Count - 1];
                if (null == last) return (0);
                var Nx = last.InterruptsPerSec;
                var N0 = this[0].InterruptsPerSec;
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

        internal int AvgPrivilegedTime
        {
            get
            {
                if (this.Count <= 1) return (0);
                try
                {
                    //WMIPerfOSProcessor last = this.Last();
                    WMIPerfOSProcessor last = this[this.Count - 1];
                    UInt64 x0 = this[0].PercentPrivilegedTime;
                    UInt64 y0 = this[0].Timestamp_Sys100NS;
                    UInt64 x1 = last.PercentPrivilegedTime;
                    UInt64 y1 = last.Timestamp_Sys100NS;
                    //Average: (Nx - N0) / (Dx - D0) x 100
                    return (int)Math.Round((((double)(x1 - x0) / (double)(y1 - y0)) * 100));
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log("WMIPerfOSProcessorSnapshots.AvgPrivilegedTime Exception:", ex);
                    return (0);
                }
            }
        }

        internal double AvgPercentInterruptTime
        {
            get
            {
                if (this.Count <= 1) return (0);
                try
                {
                    //WMIPerfOSProcessor last = this.Last();
                    WMIPerfOSProcessor last = this[this.Count - 1];
                    UInt64 x0 = this[0].PercentInterruptTime;
                    UInt64 y0 = this[0].Timestamp_Sys100NS;
                    UInt64 x1 = last.PercentInterruptTime;
                    UInt64 y1 = last.Timestamp_Sys100NS;
                    //Average: (Nx - N0) / (Dx - D0) x 100
                    return (((double)(x1 - x0) / (double)(y1 - y0)) * 100);
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log("WMIPerfOSProcessorSnapshots.AvgPercentInterruptTime Exception:", ex);
                    return (0);
                }
            }
        }

        internal int AvgCpu
        {
            get
            {
                if (this.Count <= 1) return (0);
                try
                {
                    //WMIPerfOSProcessor last = this.Last();
                    WMIPerfOSProcessor last = this[this.Count - 1];
                    UInt64 x0 = this[0].PercentProcessorTime;
                    UInt64 y0 = this[0].Timestamp_Sys100NS;
                    UInt64 x1 = last.PercentProcessorTime;
                    UInt64 y1 = last.Timestamp_Sys100NS;
                    return (int)Math.Round((100 * (1 - (double)(x1 - x0) / (double)(y1 - y0))));
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log("WMIPerfOSProcessorSnapshots.AvgCpu Exception:", ex);
                    return (0);
                }
            }
        }
    }
}
