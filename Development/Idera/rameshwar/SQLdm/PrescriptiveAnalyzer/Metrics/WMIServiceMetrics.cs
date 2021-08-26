using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WMIServiceMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WMIServiceMetrics");
        private Dictionary<string, WMIServiceSnapshots> _services = new Dictionary<string, WMIServiceSnapshots>();

        public IEnumerable<WMIService> ServicesThatStartWith(string name)
        {
            foreach (WMIServiceSnapshots svc in _services.Values)
            {
                if (null == svc) continue;
                if (svc.Count <= 0) continue;
                if (svc.Name.ToLower().StartsWith(name.ToLower()))
                {
                    yield return (svc[0]);
                }
            }
        }

        public bool IsRunning(string name)
        {
            foreach (WMIServiceSnapshots svc in _services.Values)
            {
                if (0 == string.Compare(svc.Name, name, true))
                {
                    if (svc.IsRunning) return (true);
                }
            }
            return (false);
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WMIServiceMetrics not added : " + snapshot.Error); return; }
            if (snapshot.WmiServiceSnapshotValueShutdown == null) { return; }
            if (snapshot.WmiServiceSnapshotValueShutdown.WmiService != null && snapshot.WmiServiceSnapshotValueShutdown.WmiService.Rows.Count > 0)
            {
                WMIServiceSnapshots snapshots = null;
                string id = string.Empty;
                for (int index = 0; index < snapshot.WmiServiceSnapshotValueShutdown.WmiService.Rows.Count; index++)
                {
                    WMIService obj = new WMIService();

                    try
                    {
                        obj.Name = (string)snapshot.WmiServiceSnapshotValueShutdown.WmiService.Rows[index]["Name"];
                        obj.State = (string)snapshot.WmiServiceSnapshotValueShutdown.WmiService.Rows[index]["State"];
                        obj.ProcessId = (uint)snapshot.WmiServiceSnapshotValueShutdown.WmiService.Rows[index]["ProcessId"];
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }

                    id = string.Format("{0} ({1})", obj.Name, obj.ProcessId);
                    if (!_services.TryGetValue(id, out snapshots)) _services.Add(id, snapshots = new WMIServiceSnapshots());
                    snapshots.Add(obj);
                }
            }
        }
    }

    internal class WMIServiceSnapshots : List<WMIService>
    {
        internal bool IsRunning
        {
            get
            {
                if (this.Count <= 0) return (false);
                //return (this.last().IsRunning);
                return (this[this.Count-1].IsRunning);
            }
        }

        internal string Name
        {
            get
            {
                if (this.Count <= 0) return (string.Empty);
                return (this[0].Name);
            }
        }
    }

    public class WMIService
    {
        public string Name { get;  set; }
        public UInt32 ProcessId { get;  set; }
        public string State { get;  set; }

        public bool IsRunning { get { return (0 == string.Compare(State, "Running", true)); } }

        public WMIService() { }

        public override string ToString()
        {
            return (string.Format("Name:{0}  PID:{1}  State:{2}", Name, ProcessId, State));
        }
    }
}
