using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data.SqlClient;
using BBS.TracerX;
//using Idera.SQLdoctor.AnalysisEngine.Batches;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    class OutOfDateStatsAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 11;
        private static Logger _logX = Logger.GetLogger("OutOfDateStatsAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public OutOfDateStatsAnalyzer()
        {
            _id = id;
        }

        public override string GetDescription() { return ("OutOfDateStats analysis"); }

        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            if (sm == null) return;
            if (sm.OutOfDateStatsMetrics == null) return;
            foreach (OutOfDateStatsForDB metrics in sm.OutOfDateStatsMetrics.OutOfDateStatsForDBs)
            {
                string db;
                foreach (OutOfDateStats snap in metrics.OutOfDateStatsList)
                {
                    db = snap.Database;
                    if (!(string.IsNullOrEmpty(db)))
                    {
                        AddRecommendation(new OutOfDateStatsRecommendation(
                                                db,
                                                snap.schema,
                                                snap.Table,
                                                snap.Name,
                                                snap.RowCount,
                                                snap.ModCount,
                                                snap.StatsDate,
                                                snap.HoursSinceUpdated
                                            ));
                    }
                }
            }
        }
    }
}
