using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Values;
using Idera.SQLdm.Common.Snapshots;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class QueryAnalyzerMetrics : BaseMetrics
    {
        private static Logger _logX = Logger.GetLogger("QueryAnalyzerMetrics");
        public List<QueryAnalyzerForDB> QueryAnalyzerForDBs { get; set; }

        public QueryAnalyzerMetrics()
        {
            QueryAnalyzerForDBs = new List<QueryAnalyzerForDB>();
        }

        public override void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
            if (snapshot == null) { return; }
            if (snapshot.QueryAnalyzerSnapshotList == null || snapshot.QueryAnalyzerSnapshotList.Count == 0) { return; }
            foreach (QueryAnalyzerSnapshot snap in snapshot.QueryAnalyzerSnapshotList)
            {
                //Check for error free snap shot
                if (snap != null && snap.Error == null)
                {
                    if (!string.IsNullOrEmpty(snap.Dbname))
                    {
                        QueryAnalyzerForDB objDB = new QueryAnalyzerForDB();
                        //for (int index = 0; index < snap.QueryAnalyzerInfo.Rows.Count; index++)
                        //{
                            QueryAnalyzerData obj = new QueryAnalyzerData();
                            try
                            {
                                obj.Database = (string)snap.Dbname;
                              //  obj.AffectedBatchesQ46 = snap.AffectedBatchesQ46;
                               // obj.AffectedBatchesQ47 = snap.AffectedBatchesQ47;
                               // obj.AffectedBatchesQ48 = snap.AffectedBatchesQ48;
                               // obj.AffectedBatchesQ49 = snap.AffectedBatchesQ49;
                               // obj.AffectedBatchesQ50 = snap.AffectedBatchesQ50;
                            }
                            catch (Exception e) { _logX.Error(e); IsDataValid = false; return; }
                            objDB.QueryAnalyzerList.Add(obj);
                        //}
                        QueryAnalyzerForDBs.Add(objDB);
                    }
                    else
                    {
                        _logX.Error("QueryAnalyzerMetrics not added : " + snap.Error);
                        continue;
                    }
                }
            }
        }
    }

    public class QueryAnalyzerForDB
    {
        public List<QueryAnalyzerData> QueryAnalyzerList { get; set; }

        public QueryAnalyzerForDB()
        {
            QueryAnalyzerList = new List<QueryAnalyzerData>();
        }
    }

    public class QueryAnalyzerData
    {
        public string Database { get; set; }

        public AffectedBatches AffectedBatchesQ46 { get; set; }

        public AffectedBatches AffectedBatchesQ47 { get; set; }

        public AffectedBatches AffectedBatchesQ48 { get; set; }

        public AffectedBatches AffectedBatchesQ49 { get; set; }

        public AffectedBatches AffectedBatchesQ50 { get; set; }
    }
}
