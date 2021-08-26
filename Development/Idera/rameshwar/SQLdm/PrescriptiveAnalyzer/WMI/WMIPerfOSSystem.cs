using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Idera.SQLdoctor.Common.Helpers;
using Idera.SQLdoctor.AnalysisEngine.Batches;
using TracerX;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.WMI
{
    internal class WMIPerfOSSystem
    {
        private static Logger _logX = Logger.GetLogger("WMIPerfOSSystem");

        public UInt32 ProcessorQueueLength { get; private set; }
        public UInt32 ContextSwitchesPerSec { get; private set; }
        public UInt64 Frequency_Sys100NS { get; private set; }
        public UInt64 Timestamp_Sys100NS { get; private set; }

        private WMIPerfOSSystem() { }
        public WMIPerfOSSystem(DataTable dt)
        {
            if (null == dt) return;
            if (null == dt.Rows) return;
            if (null == dt.Columns) return;
            if (dt.Columns.Count < 2) return;
            foreach (DataRow dr in dt.Rows)
            {
                if (null == dr[0]) continue;
                switch (dr[0].ToString().ToLower())
                {
                    case ("processorqueuelength"): { ProcessorQueueLength = DataHelper.ToUInt32(dr, 1); break; }
                    case ("contextswitchespersec"): { ContextSwitchesPerSec = DataHelper.ToUInt32(dr, 1); break; }
                    case ("frequency_sys100ns"): { Frequency_Sys100NS = DataHelper.ToUInt64(dr, 1); break; }
                    case ("timestamp_sys100ns"): { Timestamp_Sys100NS = DataHelper.ToUInt64(dr, 1); break; }
                    default: { _logX.Debug("Unknown WMIPerfOSSystem Property: " + dr[0].ToString()); break; }

                }
            }
        }

        internal static BatchFinder.WmiObjectNameType[] GetPropNames()
        {
            return (new BatchFinder.WmiObjectNameType[] { 
                                    new BatchFinder.WmiObjectNameType(){Name="ProcessorQueueLength", Type=BatchFinder.WmiObjectType.Int}, 
                                    new BatchFinder.WmiObjectNameType(){Name="ContextSwitchesPerSec", Type=BatchFinder.WmiObjectType.Int}, 
                                    new BatchFinder.WmiObjectNameType(){Name="Frequency_Sys100NS", Type=BatchFinder.WmiObjectType.BigInt}, 
                                    new BatchFinder.WmiObjectNameType(){Name="Timestamp_Sys100NS", Type=BatchFinder.WmiObjectType.BigInt}
                                    });
        }
    }
}
