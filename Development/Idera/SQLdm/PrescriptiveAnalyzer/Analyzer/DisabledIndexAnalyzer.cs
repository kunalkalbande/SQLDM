using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data.SqlClient;
using BBS.TracerX;
//using Idera.SQLdoctor.AnalysisEngine.Batches;

using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
     

    class DisabledIndexAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 4;
        private static Logger _logX = Logger.GetLogger("DisabledIndexAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public DisabledIndexAnalyzer()
        {
            _id = id;
        }

        public override string GetDescription() { return ("DisabledIndex analysis"); }

        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            if (sm == null) return;
            if (sm.DisabledIndexMetrics == null) return;
            foreach (DisabledIndexesForDB metrics in sm.DisabledIndexMetrics.DisabledIndexesForDBs)
            {
                string db = string.Empty;
                foreach (DisabledIndex index in metrics.DisabledIndexes)
                {
                    db = index.DatabaseName;
                    if (string.IsNullOrEmpty(db)) continue;
                    if (index.IsHypothetical)
                    {
                        AddRecommendation(new HypotheticalIndexRecommendation(db,
                                                index.schema,
                                                index.TableName,
                                                index.IndexName
                                            ));
                    }
                    else if (Convert.ToBoolean(index.IsDisabled))
                    {
                        AddRecommendation(new DisabledIndexRecommendation(
                                                conn,
                                                db,
                                                index.schema,
                                                index.TableName,
                                                index.IndexName
                                            ));
                    }
                }
            }
        }
    }
}

