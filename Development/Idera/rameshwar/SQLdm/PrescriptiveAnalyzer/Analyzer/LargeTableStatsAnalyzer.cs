using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{/// <summary>
    /// //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-I24 Adding new analyzer 
    /// </summary>
    internal class LargeTableStatsAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 26;
        private static Logger _logX = Logger.GetLogger("LargeTableStatsAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public LargeTableStatsAnalyzer()
        {
            _id = id;
        }

        public override string GetDescription() { return ("LargeTableStats analysis"); }

        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            if (sm == null) return;
            if (sm.LargeTableStatsMetrics == null) return;
            foreach (LargeTableStatsForDB metrics in sm.LargeTableStatsMetrics.LargeTableStatsForDBs)
            {
                string db;
                foreach (LargeTableStats snap in metrics.LargeTableStatsList)
                {
                    db = snap.Database;
                    if (!(string.IsNullOrEmpty(db)))
                    {
                        AddRecommendation(new HighModificationsSinceLastStatUpdateRecommendation(
                                                db,
                                                snap.schema,
                                                snap.Table
                                            ));
                    }
                }
            }
        }
    
    }
}
