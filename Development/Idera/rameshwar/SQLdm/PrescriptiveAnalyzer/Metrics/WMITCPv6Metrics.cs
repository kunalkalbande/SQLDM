using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WMITCPv6Metrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WMITCPv6Metrics");
        private WMITCPSnapshots _snapshots = new WMITCPSnapshots();

        public UInt32 SegmentsPersec { get { return (_snapshots.SegmentsPersec); } }
        public UInt32 SegmentsRetransmittedPerSec { get { return (_snapshots.SegmentsRetransmittedPerSec); } }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            AddSnapshot(snapshot.WmiTCPv6SnapshotValueStartup);
            AddSnapshot(snapshot.WmiTCPv6SnapshotValueShutdown);
        }

        private void AddSnapshot(Idera.SQLdm.Common.Snapshots.WmiTCPv6Snapshot snapshot)
        {
            if (snapshot== null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WMITCPv6Metrics not added : " + snapshot.Error); return; }
            
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
}
