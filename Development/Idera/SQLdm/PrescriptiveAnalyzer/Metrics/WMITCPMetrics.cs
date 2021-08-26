using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WMITCPMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WMITCPMetrics");
        private WMITCPSnapshots _snapshots = new WMITCPSnapshots();

        public UInt32 SegmentsPersec { get { return (_snapshots.SegmentsPersec); } }
        public UInt32 SegmentsRetransmittedPerSec { get { return (_snapshots.SegmentsRetransmittedPerSec); } }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            AddSnapshot(snapshot.WmiTCPSnapshotValueStartup);
            AddSnapshot(snapshot.WmiTCPSnapshotValueShutdown);
        }

        private void AddSnapshot(Idera.SQLdm.Common.Snapshots.WmiTCPSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WMITCPMetrics not added : " + snapshot.Error); return; }
            
            if (snapshot.WmiTCP != null && snapshot.WmiTCP.Rows.Count > 0)
            {
                for (int index = 0; index < snapshot.WmiTCP.Rows.Count; index++)
                {
                    WMITCP obj = new WMITCP();
                    try
                    {
                        obj.SegmentsPersec = (uint)snapshot.WmiTCP.Rows[index]["SegmentsPersec"];
                        obj.SegmentsRetransmittedPerSec = (uint)snapshot.WmiTCP.Rows[index]["SegmentsRetransmittedPerSec"];
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                    _snapshots.Add(obj);
                }
            }
        }

    }

    internal class WMITCPSnapshots : List<WMITCP>
    {
        public UInt32 SegmentsPersec
        {
            get
            {
                if (this.Count <= 1) return (0);
                //return (this.Last().SegmentsPersec - this[0].SegmentsPersec);
                return (this[this.Count - 1].SegmentsPersec - this[0].SegmentsPersec);
            }
        }
        public UInt32 SegmentsRetransmittedPerSec
        {
            get
            {
                if (this.Count <= 1) return (0);
                //return (this.Last().SegmentsRetransmittedPerSec - this[0].SegmentsRetransmittedPerSec);
                return (this[this.Count - 1].SegmentsRetransmittedPerSec - this[0].SegmentsRetransmittedPerSec);
            }
        }
    }

    internal class WMITCP
    {
        public UInt32 SegmentsPersec { get; set; }
        public UInt32 SegmentsRetransmittedPerSec { get; set; }

        public WMITCP() { }
    }
}
