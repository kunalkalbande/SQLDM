using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WMIComputerSystemMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WMIComputerSystemMetrics");
        WMIComputerSystem _cs = null;
        public UInt32 DomainRole { get { return (null == _cs ? 0 : _cs.DomainRole); } }
        public bool IsBackupDomainController { get { return (null == _cs ? false : _cs.IsBackupDomainController); } }
        public bool IsPrimaryDomainController { get { return (null == _cs ? false : _cs.IsPrimaryDomainController); } }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WMIComputerSystemMetrics not added : " + snapshot.Error); return; }
            
            if (snapshot.WmiComputerSystemSnapshotValueShutdown == null) { return; }
            if (snapshot.WmiComputerSystemSnapshotValueShutdown.WmiComputerSystem != null && snapshot.WmiComputerSystemSnapshotValueShutdown.WmiComputerSystem.Rows.Count > 0)
            {
                for (int index = 0; index < snapshot.WmiComputerSystemSnapshotValueShutdown.WmiComputerSystem.Rows.Count; index++)
                {
                    WMIComputerSystem obj = new WMIComputerSystem();
                    try
                    {
                        obj.DomainRole = (uint)snapshot.WmiComputerSystemSnapshotValueShutdown.WmiComputerSystem.Rows[index]["DomainRole"];
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }

                    _cs = obj;
                }
            }
        }
    }

    internal class WMIComputerSystem
    {
        public UInt32 DomainRole { get;  set; }
        public bool IsBackupDomainController { get { return (4 == DomainRole); } }
        public bool IsPrimaryDomainController { get { return (5 == DomainRole); } }

        public WMIComputerSystem() { }

    }
}
