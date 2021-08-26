using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WMINetworkRedirectorMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WMINetworkRedirectorMetrics");
        private WMINetworkRedirectorSnapshots _snapshots = new WMINetworkRedirectorSnapshots();

        public UInt32 NetworkErrorsPerSec { get { return (_snapshots.NetworkErrorsPerSec); } }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            AddSnapshot(snapshot.WmiNetworkRedirectorSnapshotValueStartup);
            AddSnapshot(snapshot.WmiNetworkRedirectorSnapshotValueShutdown);
        }

        private void AddSnapshot(Idera.SQLdm.Common.Snapshots.WmiNetworkRedirectorSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WMINetworkRedirectorMetrics not added : " + snapshot.Error); return; }
            
            if (snapshot.WmiNetworkRedirector != null && snapshot.WmiNetworkRedirector.Rows.Count > 0)
            {
                for (int index = 0; index < snapshot.WmiNetworkRedirector.Rows.Count; index++)
                {
                    WMINetworkRedirector obj = new WMINetworkRedirector();
                    try
                    {
                        obj.NetworkErrorsPerSec = (uint)snapshot.WmiNetworkRedirector.Rows[index]["NetworkErrorsPerSec"];
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                    _snapshots.Add(obj);
                }
            }
        }
    }

    internal class WMINetworkRedirectorSnapshots : List<WMINetworkRedirector>
    {
        public UInt32 NetworkErrorsPerSec
        {
            get
            {
                if (this.Count <= 1) return (0);
                //return (this.Last().NetworkErrorsPerSec - this[0].NetworkErrorsPerSec);
                return (this[this.Count - 1].NetworkErrorsPerSec - this[0].NetworkErrorsPerSec);
            }
        }
    }

    internal class WMINetworkRedirector
    {
        public UInt32 NetworkErrorsPerSec { get; set; }

        public WMINetworkRedirector() { }

    }
}
