using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data.SqlClient;
//using Idera.SQLdoctor.AnalysisEngine.Batches;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    class FragmentedIndexesAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 5;
        private static Logger _logX = Logger.GetLogger("FragmentedIndexesAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public FragmentedIndexesAnalyzer()
        {
            _id = id;
        }

        public override string GetDescription() { return ("FragmentedIndexes analysis"); }

        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            if (sm == null) return;
            if (sm.FragmentedIndexesMetrics == null) return;
            foreach (FragmentedIndexesForDB metrics in sm.FragmentedIndexesMetrics.FragmentedIndexesForDBs)
            {
                string db;
                foreach (FragmentedIndex index in metrics.FragmentedIndexes)
                {
                    db = index.DatabaseName;
                    if (string.IsNullOrEmpty(db)) continue;
                    AddRecommendation(new FragmentedIndexRecommendation(
                                            conn,
                                            db,
                                            index.schema,
                                            index.TableName,
                                            index.IndexName,
                                            index.Partition,
                                            (float)index.FragPercent,
                                            Convert.ToUInt64(index.PartitionPages),
                                            Convert.ToUInt64(index.TablePages),
                                            Convert.ToUInt64(index.TotalServerBufferPages)
                                        ));
                }
            }
        }
        
    }
}
