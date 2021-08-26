using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{/// <summary>
    /// //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-I30 Adding new analyzer 
    /// </summary>
    internal class ColumnStoreIndexAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 23;
        private static Logger _logX = Logger.GetLogger("ColumnStoreIndexAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public ColumnStoreIndexAnalyzer()
        {
            _id = id;
        }

        public override string GetDescription() { return ("ColumnStoreIndex analysis"); }

        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            if (sm == null) return;
            if (sm.ColumnStoreIndexMetrics == null) return;
            foreach (ColumnStoreIndexForDB metrics in sm.ColumnStoreIndexMetrics.ColumnStoreIndexForDBs)
            {
                string db;
                foreach (ColumnStoreIndex snap in metrics.ColumnStoreIndexList)
                {
                    db = snap.Database;
                    if (!(string.IsNullOrEmpty(db)))
                    {
                        AddRecommendation(new ColumnStoreIndexMissingOnLargeTablesRecommendation(
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
