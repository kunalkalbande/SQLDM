using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    /// <summary>
    /// //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-Q46,Q47,Q48,Q49,Q50 Adding new analyzer 
    /// </summary>
    internal class QueryAnalyzer : AbstractAnalyzer
    {

        private const Int32 id = 22;
        private static Logger _logX = Logger.GetLogger("HashIndexAnalyzer");
        protected override Logger GetLogger() { return (_logX); }
        public QueryAnalyzer()
            : base()
        {
            _id = id;
        }
        public override string GetDescription() { return ("OutOfDateStats analysis"); }

        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            if (sm == null) return;
            if (sm.OutOfDateStatsMetrics == null) return;
            foreach (QueryAnalyzerForDB metrics in sm.QueryAnalyzerMetrics.QueryAnalyzerForDBs)
            {
                string dbname;
                foreach (QueryAnalyzerData snap in metrics.QueryAnalyzerList)
                {
                    dbname = snap.Database;
                    if (!(string.IsNullOrEmpty(dbname)))
                    {
                        // Add recomm Q46
                        if (snap.AffectedBatchesQ46 != null && snap.AffectedBatchesQ46.Count > 0)
                        {
                            AddRecommendation(new Top10QueriesWithLongestAverageExecutionTimeRecommendation(snap.AffectedBatchesQ46, dbname));
                        }
                        //Add recomm Q47
                        if (snap.AffectedBatchesQ47 != null && snap.AffectedBatchesQ47.Count > 0)
                        {
                            AddRecommendation(new Top10QueriesConsumingMostIORecommendation(snap.AffectedBatchesQ47, dbname));
                        }
                        //Add recomm Q48
                        if (snap.AffectedBatchesQ48 != null && snap.AffectedBatchesQ48.Count > 0)
                        {
                            AddRecommendation(new QueriesWithDoubleIncreaseInExecutionTimeRecommendation(snap.AffectedBatchesQ48, dbname));
                        }
                        //Add recomm Q49
                        if (snap.AffectedBatchesQ49 != null && snap.AffectedBatchesQ49.Count > 0)
                        {
                            AddRecommendation(new Top10QueriesHavingLongerDurationInLastHourRecommendation(snap.AffectedBatchesQ49, dbname));
                        }
                        //Add recomm Q50
                        if (snap.AffectedBatchesQ50 != null && snap.AffectedBatchesQ50.Count > 0)
                        {
                            AddRecommendation(new QueriesWithFourDifferentPlanInTwoDaysRecommendation(snap.AffectedBatchesQ50, dbname));
                        }
                    }
                }
            }
        }
    }
}
