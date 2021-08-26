using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.WMI
{
    internal class WMITCPSnapshots : List<WMITCP>
    {
        public UInt32 SegmentsPersec 
        {
            get
            {
                if (this.Count <= 1) return (0);
                return (this.Last().SegmentsPersec - this[0].SegmentsPersec);
            }
        }
        public UInt32 SegmentsRetransmittedPerSec 
        {
            get
            {
                if (this.Count <= 1) return (0);
                return (this.Last().SegmentsRetransmittedPerSec - this[0].SegmentsRetransmittedPerSec);
            }
        }
    }
}
