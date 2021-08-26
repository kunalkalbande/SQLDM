using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TracerX;
using Idera.SQLdoctor.Common.Helpers;
using Idera.SQLdoctor.AnalysisEngine.Batches;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.WMI
{
    internal class WMIPerfOSMemory
    {
        private static Logger _logX = Logger.GetLogger("WMIPerfOSMemory");

        internal UInt32 PagesPerSecond { get; private set; }
        internal UInt64 Timestamp_Sys100NS { get; private set; }

        internal WMIPerfOSMemory(DataTable dt)
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
                    case ("pagespersec"): { PagesPerSecond = DataHelper.ToUInt32(dr, 1); break; }
                    case ("timestamp_sys100ns"): { Timestamp_Sys100NS = DataHelper.ToUInt64(dr, 1); break; }
                    default: { _logX.Debug("Unknown WMIPerfOSSystem Property: " + dr[0].ToString()); break; }

                }
            }
        }


        internal static BatchFinder.WmiObjectNameType[] GetPropNames()
        {
            return (new BatchFinder.WmiObjectNameType[] { 
                                    new BatchFinder.WmiObjectNameType(){Name="PagesPersec", Type=BatchFinder.WmiObjectType.Int}, 
                                    new BatchFinder.WmiObjectNameType(){Name="Timestamp_Sys100NS", Type=BatchFinder.WmiObjectType.BigInt}
                                    });
        }
    }
}
