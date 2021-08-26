using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Batches;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    /// <summary>
    /// //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I23 Adding new analyzer 
    /// </summary>
    internal class QueryStoreAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 20;
        private static Logger _logX = Logger.GetLogger("QueryStoreAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        //const to check recomm conditions
        private const int queryStoreReadOnlyState = 1;
        private const int queryStoreReadOnlyReason = 65536;
        private const int queryStoreRemainingSpace = 10;

        public QueryStoreAnalyzer()
        {
            _id = id;
        }

        public override string GetDescription() { return ("QueryStore analysis"); }

        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            if (sm == null) return;
            if (sm.QueryStoreMetrics == null || sm.QueryStoreMetrics.QueryStoreForDBs == null) return;
            foreach (QueryStoreForDB metrics in sm.QueryStoreMetrics.QueryStoreForDBs)
            {
                string db;
                foreach (QueryStore snap in metrics.QueryStoreList)
                {
                    db = snap.dbname;
                    if (!(string.IsNullOrEmpty(db)))
                    {
                        if (snap.actual_state == 0 && snap.dbname != "master" && snap.dbname != "model" && snap.dbname != "msdb" && snap.dbname != "tempdb")
                        {
                            AddRecommendation(new QueryStoreDisabledRecommendation(snap.dbname));
                        }

                        if (snap.actual_state == queryStoreReadOnlyState && snap.readonly_reason == queryStoreReadOnlyReason)
                        {
                            AddRecommendation(new QueryStoreOutOfSpaceRecommendation(snap.dbname));
                        }

                        if (snap.remaining_space < queryStoreRemainingSpace && snap.remaining_space > 0)
                        {
                            AddRecommendation(new QueryStoreAlmostFullRecommendation(snap.dbname));
                        }
                        if (snap.enabledPlanGuideNames.Count > 0)
                        {
                            AddRecommendation(new PlanGuidesUsedOverQueryStoreRecommendation(snap.dbname, snap.enabledPlanGuideNames));
                        }
                    }
                }
            }
        }
    }
}
