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
    internal class NonIncrementalColumnStatAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 18;
        private static Logger _logX = Logger.GetLogger("NonIncrementalColumnStatAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public NonIncrementalColumnStatAnalyzer()
        {
            _id = id;
        }

        public override string GetDescription() { return ("NonIncrementalColumnStat analysis"); }

        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            if (sm == null) return;
            if (sm.NonIncrementalColStatsMetrics == null || sm.NonIncrementalColStatsMetrics.NonIncrementalColumnStatsForDBs == null) return;
            foreach (NonIncrementalColumnStatForDB metrics in sm.NonIncrementalColStatsMetrics.NonIncrementalColumnStatsForDBs)
            {
                string db;
                foreach (NonIncrementalColumnStats snap in metrics.NonIncrementalStatsList)
                {
                    db = snap.Database;
                    if (!(string.IsNullOrEmpty(db)))
                    {
                        AddRecommendation(new NonIncrementalColumnStatRecommendation(
                                                snap.Database, snap.TableName,snap.StateName, snap.OptiScript, snap.UndoScript
                                            ));
                    }
                }
            }
        }
    }
}
