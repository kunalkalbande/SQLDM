using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    /// <summary>
    /// //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-I23 Adding new analyzer 
    /// </summary>
    internal class InMemoryTableIndexAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 21;
        private static Logger _logX = Logger.GetLogger("InMemoryTableIndexAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public InMemoryTableIndexAnalyzer()
        {
            _id = id;
        }

        public override string GetDescription() { return ("InMemoryTableIndex analysis"); }

        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            if (sm == null) return;
            if (sm.RarelyUsedIndexOnInMemoryTableMetrics == null || sm.RarelyUsedIndexOnInMemoryTableMetrics.RarelyUsedIndexOnInMemoryTableForDBs == null) return;
            foreach (RarelyUsedIndexOnInMemoryTableForDB metrics in sm.RarelyUsedIndexOnInMemoryTableMetrics.RarelyUsedIndexOnInMemoryTableForDBs)
            {
                string db;
                foreach (RarelyUsedIndexOnInMemoryTable snap in metrics.RarelyUsedIndexOnInMemoryTableList)
                {
                    db = snap.Database;
                    if (!(string.IsNullOrEmpty(db)))
                    {
                        AddRecommendation(new RarelyUsedIndexOnInMemoryTableRecommendation(
                            snap.Database, snap.TableName,snap.SchemaName, snap.IndexName
                                            ));
                    }
                }
            }
        }
    }
}
