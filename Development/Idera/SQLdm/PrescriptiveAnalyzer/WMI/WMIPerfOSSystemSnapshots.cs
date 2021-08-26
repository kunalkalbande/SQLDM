using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.WMI
{
    internal class WMIPerfOSSystemSnapshots : List<WMIPerfOSSystem>
    {
        public UInt32 ContextSwitchesPerSec
        {
            get
            {
                if (Count <= 1) return (0);
                try
                {
                    WMIPerfOSSystem last = this.Last();
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
}
