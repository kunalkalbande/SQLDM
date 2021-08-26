using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    /// <summary>
    /// //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-I31 Adding new analyzer 
    /// </summary>
    internal class FilteredIndexAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 24;
        private static Logger _logX = Logger.GetLogger("FilteredIndexAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public FilteredIndexAnalyzer()
        {
            _id = id;
        }

        public override string GetDescription() { return ("FilteredIndexAnalyzer analysis"); }

        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            if (sm == null) return;
            if (sm.FilteredColumnNotInKeyOfFilteredIndexMetrics == null || sm.FilteredColumnNotInKeyOfFilteredIndexMetrics.FilteredColumnNotInKeyOfFilteredIndexForDBs == null) return;
            foreach (FilteredColumnNotInKeyOfFilteredIndexForDB metrics in sm.FilteredColumnNotInKeyOfFilteredIndexMetrics.FilteredColumnNotInKeyOfFilteredIndexForDBs)
            {
                string db;
                foreach (FilteredColumnNotInKeyOfFilteredIndex snap in metrics.FilteredColumnNotInKeyOfFilteredIndexList)
                {
                    db = snap.Database;
                    if (!(string.IsNullOrEmpty(db)))
                    {
                        AddRecommendation(new FilteredColumnNotInKeyOfFilteredIndexRecommendation(
                            snap.Database, snap.TableName,snap.SchemaName, snap.IndexName
                                            ));
                    }
                }
            }
        }
    }
}
