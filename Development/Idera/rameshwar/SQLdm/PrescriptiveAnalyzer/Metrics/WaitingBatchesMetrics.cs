using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics.WaitingObjects;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using System.Data;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WaitingBatchesMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("WaitingBatchesMetrics");
        private Dictionary<string, WaitingBatches> _waiting = new Dictionary<string, WaitingBatches>();
        private Dictionary<string, WaitingDatabases> _waitingDatabases = new Dictionary<string, WaitingDatabases>();

        public WaitingBatches GetWaitingBatches(string wt)
        {
            return (GetWaitingBatches(new string[] { wt }, false));
        }
        public WaitingBatches GetWaitingBatches(string[] wt)
        {
            return (GetWaitingBatches(wt, false));
        }
        public WaitingBatches GetWaitingBatches(string wt, bool startsWith)
        {
            return (GetWaitingBatches(new string[] { wt }, startsWith));
        }
        public WaitingBatches GetWaitingBatches(string[] wt, bool startsWith)
        {
            WaitingBatches wb = null;
            if (!startsWith && (wt.Length == 1))
            {
                _waiting.TryGetValue(wt[0], out wb);
                return (wb);
            }
            wb = new WaitingBatches();
            foreach (KeyValuePair<string, WaitingBatches> p in _waiting)
            {
                if (startsWith)
                {
                    if (WaitStats.StartsWith(p.Key, wt))
                    {
                        wb.Add(p.Value);
                    }
                }
                else if (WaitStats.Compare(p.Key, wt))
                {
                    wb.Add(p.Value);
                }
            }
            return (wb);
        }

        public WaitingDatabases GetWaitingDatabases(string wt)
        {
            return (GetWaitingDatabases(new string[] { wt }, false));
        }
        public WaitingDatabases GetWaitingDatabases(string[] wt)
        {
            return (GetWaitingDatabases(wt, false));
        }
        public WaitingDatabases GetWaitingDatabases(string wt, bool startsWith)
        {
            return (GetWaitingDatabases(new string[] { wt }, startsWith));
        }
        public WaitingDatabases GetWaitingDatabases(string[] wt, bool startsWith)
        {
            WaitingDatabases wd = null;
            if (!startsWith && (wt.Length == 1))
            {
                _waitingDatabases.TryGetValue(wt[0], out wd);
                return (wd);
            }
            wd = new WaitingDatabases();
            foreach (KeyValuePair<string, WaitingDatabases> p in _waitingDatabases)
            {
                if (startsWith)
                {
                    if (WaitStats.StartsWith(p.Key, wt))
                    {
                        wd.Add(p.Value);
                    }
                }
                else if (WaitStats.Compare(p.Key, wt))
                {
                    wd.Add(p.Value);
                }
            }
            return (wd);
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            //Check for error in snapshot
            if (snapshot.Error != null) { _logX.Error("WaitingBatchesMetrics not added : " + snapshot.Error); return; }
            
            if (snapshot.WaitingBatchesSnapshotValueStartup != null)
                AddSnapshot(snapshot.WaitingBatchesSnapshotValueStartup.WaitingBatches);
            if (snapshot.WaitingBatchesSnapshotValueInterval != null && snapshot.WaitingBatchesSnapshotValueInterval.ListWaitingBatches != null)
            {
                foreach (DataTable dt in snapshot.WaitingBatchesSnapshotValueInterval.ListWaitingBatches)
                {
                    AddSnapshot(dt);
                }
            }
        }

        public void AddSnapshot(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                string wt;
                string batch;
                string prg;
                string resource;
                WaitingBatches wb;
                WaitingDatabases wd;
                for (int index = 0; index < dt.Rows.Count; index++)
                {
                    DataRow r = dt.Rows[index];
                    prg = DataHelper.ToString(r, "program_name");
                    
                    if (!string.IsNullOrEmpty(prg))
                    {
//#if !DEBUG
//                    if (prg.StartsWith(Constants.ConnectionStringApplicationNamePrefix, StringComparison.InvariantCultureIgnoreCase)) continue;
//                    if (prg.StartsWith(Constants.ConnectionStringSQLdmApplicationNamePrefix, StringComparison.InvariantCultureIgnoreCase)) continue;
//#endif
                    }

                    wt = DataHelper.ToString(r, "wait_type");
                    batch = DataHelper.ToString(r, "text");
                    resource = DataHelper.ToString(r, "resource_description");
                    if (!string.IsNullOrEmpty(batch) && !string.IsNullOrEmpty(wt))
                    {
                        if (!_waiting.TryGetValue(wt, out wb)) { _waiting.Add(wt, wb = new WaitingBatches()); }
                        wb.Add(batch, prg);
                    }
                    WaitingResource wr = GetWaitingResource(resource);
                    if (null != wr)
                    {
                        if (!_waitingDatabases.TryGetValue(wt, out wd)) { _waitingDatabases.Add(wt, wd = new WaitingDatabases()); }
                        wd.Add(wr);
                    }
                }
            }
        }

        private WaitingResource GetWaitingResource(string resource)
        {
            if (string.IsNullOrEmpty(resource)) return (null);
            string[] p = resource.Split(':');
            if (null == p) return (null);
            if (3 != p.Length) return (null);
            try
            {
                return (new WaitingResource(Convert.ToUInt32(p[0]), Convert.ToUInt32(p[1]), Convert.ToUInt64(p[2])));
            }
            catch { }
            return (null);
        }
    }
}
