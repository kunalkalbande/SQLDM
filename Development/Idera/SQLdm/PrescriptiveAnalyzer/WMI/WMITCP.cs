using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Idera.SQLdoctor.Common.Helpers;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.WMI
{
    internal class WMITCP
    {
        public UInt32 SegmentsPersec { get; set; }
        public UInt32 SegmentsRetransmittedPerSec { get; set; }

        private WMITCP() { }
        public WMITCP(DataTable dt)
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
                    case ("segmentspersec"): { SegmentsPersec = DataHelper.ToUInt32(dr, 1); break; }
                    case ("segmentsretransmittedpersec"): { SegmentsRetransmittedPerSec = DataHelper.ToUInt32(dr, 1); break; }
                    default: { System.Diagnostics.Debug.WriteLine("Unknown WMITCP Property: " + dr[0].ToString()); break; }

                }
            }
        }

        internal static string[] GetPropNames()
        {
            return (new string[] { 
                                    "SegmentsPersec", 
                                    "SegmentsRetransmittedPerSec", 
                                    });
        }
    }
}
