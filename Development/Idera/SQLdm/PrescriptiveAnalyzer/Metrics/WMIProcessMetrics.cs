using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WMIProcessMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WMIProcessMetrics");
        private List<WMIProcess> _p = new List<WMIProcess>();

        public IEnumerable<WMIProcess> Processes { get { return (_p); } }

        public WMIProcess GetProcessById(UInt32 id)
        {
            if (null == _p) return (null);
            foreach (WMIProcess p in _p)
            {
                if (null != p) if (id == p.ProcessId) return (p);
            }
            return (null);
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WMIProcessMetrics not added : " + snapshot.Error); return; }  
            if (snapshot.WmiProcessSnapshotValueShutdown == null) { return; }
            if (snapshot.WmiProcessSnapshotValueShutdown.WmiProcess != null && snapshot.WmiProcessSnapshotValueShutdown.WmiProcess.Rows.Count > 0)
            {
                for (int index = 0; index < snapshot.WmiProcessSnapshotValueShutdown.WmiProcess.Rows.Count; index++)
                {
                    WMIProcess obj = new WMIProcess();

                    try
                    {
                        DataRow r = snapshot.WmiProcessSnapshotValueShutdown.WmiProcess.Rows[index];
                        obj.Name =  DataHelper.ToString(r,"Name");
                        obj.CommandLine = DataHelper.ToString(r,"CommandLine");
                        obj.Priority = DataHelper.ToUInt32(r, "Priority");
                        obj.ProcessId = DataHelper.ToUInt32(r, "ProcessId");
                        obj.ThreadCount = DataHelper.ToUInt32(r, "ThreadCount");
                        obj.WorkingSetSize = DataHelper.ToUInt64(r, "WorkingSetSize");
                    }
                    catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                    _p.Add(obj);
                }
            }
        }
    }

    public class WMIProcess
    {
        public string Name { get;  set; }
        public string CommandLine { get;  set; }
        public UInt32 Priority { get;  set; }
        public UInt32 ThreadCount { get;  set; }
        public UInt32 ProcessId { get;  set; }
        public UInt64 WorkingSetSize { get;  set; }
        public string SvcHostArgs
        {
            get
            {
                if (0 == string.Compare(Name, "svchost.exe", true))
                {
                    int offset = CommandLine.IndexOf(" -k ");
                    if (offset >= 0)
                    {
                        return (CommandLine.Substring(offset + 4));
                    }
                }
                return (string.Empty);
            }
        }
        public WMIProcess() { }

        public override string ToString()
        {
            return (string.Format("{0}  WorkingSetSize:{1}", Name, WorkingSetSize));
        }
    }
}
