using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Values;
using Idera.SQLdm.Common.Snapshots;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class QueryStoreMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("QueryStoreMetrics");
        public string Edition { get; set; }
        public List<QueryStoreForDB> QueryStoreForDBs { get; set; }

        public QueryStoreMetrics()
        {
            QueryStoreForDBs = new List<QueryStoreForDB>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            if (snapshot.QueryStoreSnapshotList == null || snapshot.QueryStoreSnapshotList.Count == 0) { return; }
            foreach (QueryStoreSnapshot snap in snapshot.QueryStoreSnapshotList)
            {
                //Check for error free snap shot
                if (snap != null && snap.Error == null)
                {
                    if (snap.QueryStoreInfo != null && snap.QueryStoreInfo.Rows.Count > 0)
                    {
                        QueryStoreForDB objDB = new QueryStoreForDB();
                        for (int index = 0; index < snap.QueryStoreInfo.Rows.Count; index++)
                        {
                            QueryStore obj = new QueryStore();
                            try
                            {
                                obj.dbname = snap.DbName;
                                obj.actual_state = Convert.ToInt32(snap.QueryStoreInfo.Rows[index]["actual_state"]);
                                obj.readonly_reason = Convert.ToInt32(snap.QueryStoreInfo.Rows[index]["readonly_reason"]);
                                obj.remaining_space = Convert.ToDouble(snap.QueryStoreInfo.Rows[index]["remaining_space"]);
                                obj.enabledPlanGuideNames = snap.PlanName;
                            }
                            catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                            objDB.QueryStoreList.Add(obj);
                        }
                        QueryStoreForDBs.Add(objDB);
                    }
                    else
                    {
                        _logX.Error("QueryStoreMetrics not added : " + snap.Error);
                        continue;
                    }
                }
            }
        }
    }

    public class QueryStoreForDB
    {
        public List<QueryStore> QueryStoreList { get; set; }

        public QueryStoreForDB()
        {
            QueryStoreList = new List<QueryStore>();
        }
    }

    public class QueryStore
    {
        public string dbname { get; set; }
        public int actual_state { get; set; }
        public int readonly_reason { get; set; }
        public double remaining_space { get; set; }
        public List<string> enabledPlanGuideNames { get; set; }
    }
}
