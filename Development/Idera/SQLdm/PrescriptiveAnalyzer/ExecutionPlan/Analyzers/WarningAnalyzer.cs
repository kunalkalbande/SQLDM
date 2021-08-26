using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Stats;

namespace Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan.Analyzers
{
    internal class WarningAnalyzer : ExecutionPlanWarnings, IAnalyzePlan
    {
        private const Int32 id = 205;
        private static TracerX.Logger _logX = TracerX.Logger.GetLogger("WarningAnalyzer");

        class StatsWarning
        {
            public TraceEventStats Stats { get; set; }
            public List<StatementWarning> Warnings { get; set; }
        }
        class NoColumnStats
        {
            public string Database { get; private set; }
            public string Schema { get; private set; }
            public string Table { get; private set; }
            public string Column { get; private set; }
            public Dictionary<string, List<string>> Batches { get; set; }
            public UInt64 Count { get; set; }
            public NoColumnStats(string database, string schema, string table, string column) 
            {
                Database = database;
                Table = table;
                Schema = schema;
                Column = column;
                Batches = new Dictionary<string, List<string>>();
                Count = 0;
            }
        }
        private List<StatsWarning> _warnings = null;
        private BaseOptions _ops = null;
        private int _traceDurationMinutes = 0;
        private RecommendationCountHelper _counter = new RecommendationCountHelper(Properties.Settings.Default.Max_RecommendationsPerType);

        public WarningAnalyzer(BaseOptions ops, SqlSystemObjectManager ssom) : base(ssom) { _ops = ops; }

        public Int32 ID { get { return id; } }

        public void Analyze(TraceEventStatsCollection tesc)
        {
            if (null == tesc) return;
            _traceDurationMinutes = tesc.TraceDurationMinutes;
            foreach (TraceEventStats tes in tesc)
            {
                if (null == tes) continue;
                IEnumerable<StatementWarning> warnings = GetWarnings(tes.Plan, tes.High.DBID);
                if (null == warnings) continue;
                //if (warnings.Count() <= 0) continue;
                int warningsCount = 0;
                foreach (var item in warnings)
                { warningsCount++; }
                if (warningsCount <= 0) continue;
                if (null == _warnings) _warnings = new List<StatsWarning>();
                //_warnings.Add(new StatsWarning() { Stats = tes, Warnings = warnings.ToList() });
                List<StatementWarning> listWarnings = new List<StatementWarning>();
                foreach (StatementWarning sw in warnings)
                {
                    listWarnings.Add(sw);
                }
                _warnings.Add(new StatsWarning() { Stats = tes, Warnings = listWarnings });
            }
        }

        public void Clear()
        {
            _warnings.Clear();
        }

        public IEnumerable<Exception> GetExceptions()
        {
            return (null);
        }
        public IEnumerable<IRecommendation> GetRecommendations()
        {
            if (null != _warnings)
            {
                List<NoJoinPredicateStatement> noJoin = new List<NoJoinPredicateStatement>();
                Dictionary<string, NoColumnStats> noStats = new Dictionary<string, NoColumnStats>();
                IRecommendation r = null;
                foreach (StatsWarning sw in _warnings)
                {
                    noJoin.Clear();
                    foreach (StatementWarning warning in sw.Warnings)
                    {
                        if (warning.Warning.NoJoinPredicateSpecified && warning.Warning.NoJoinPredicate)
                        {
                            noJoin.Add(new NoJoinPredicateStatement
                                { statement = warning.Statement.StatementText,
                                  EstimatedTotalSubTreeCost = warning.RelOp.EstimatedTotalSubtreeCost,
                                  StatementSubTreeCost = warning.Statement.StatementSubTreeCost
                                }
                            );
                        }
                        AddNoStatsWarning(noStats, warning, sw.Stats.High.TextData, sw.Stats.TotalCount);
                    }
                    if (noJoin.Count > 0) 
                    {
                        Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents.TEBase teb = sw.Stats.High;
                        string db = string.Empty;
                        if ((null != _ops) && (null != teb)) { db = _ops.GetDatabaseName(teb.DBID); }
                        r = new NoJoinPredicateRecommendation(db, teb.ApplicationName, teb.LoginName, teb.HostName, teb.TextData, noJoin, sw.Stats.XmlPlan);
                        _counter.Add(r);
                        yield return r;
                        if (!_counter.Allow(r)) { break; }
                    }
                }
                if (noStats.Count > 0)
                {
                    var tp = new SQLTablePropHelper(_ops.ConnectionInfo);
                    string db;
                    string schema;
                    string table;
                    string col;
                    foreach (NoColumnStats ncs in noStats.Values)
                    {
                        db = SQLHelper.RemoveBrackets(ncs.Database);
                        schema = SQLHelper.RemoveBrackets(ncs.Schema);
                        table = SQLHelper.RemoveBrackets(ncs.Table);
                        col = SQLHelper.RemoveBrackets(ncs.Column);
                        if (string.IsNullOrEmpty(db)) 
                        { 
                            _logX.DebugFormat("Skipped no column stats due to no database name!"); 
                            continue; 
                        }
                        if (!tp.IsSystemTable(db, schema, table))
                        {
                            _logX.DebugFormat("No column stats recommendation added for database:{0} schema:{1} table:{2} column:{3}", db, schema, table, col);
                            r = new NoColumnStatsRecommendation(db, schema, table, col, ncs.Batches, ncs.Count, _traceDurationMinutes);
                            _counter.Add(r);
                            yield return r;
                            if (!_counter.Allow(r)) { break; }
                        }
                        else
                        {
                            _logX.DebugFormat("Skipped no column stats recommendation on system table of database:{0} schema:{1} table:{2} column:{3}", db, schema, table, col);
                        }
                    }
                }
            }
        }

        private void AddNoStatsWarning(Dictionary<string, NoColumnStats> noStats, StatementWarning warning, string batch, UInt64 count)
        {
            if (null == warning.Warning.ColumnsWithNoStatistics) return;
            //if (warning.Warning.ColumnsWithNoStatistics.Count() <= 0) return;
            if (warning.Warning.ColumnsWithNoStatistics.Length <= 0) return;
            string colName;
            NoColumnStats noColStats;
            List<string> noColStatsBatchStatements;
            foreach (ColumnReferenceType col in warning.Warning.ColumnsWithNoStatistics)
            {
                if (string.IsNullOrEmpty(col.Column)) continue;
                colName = string.Format("{0}.{1}.{2}.{3}", col.Database, col.Schema, col.Table, col.Column);
                if (!noStats.TryGetValue(colName, out noColStats)) noStats.Add(colName, noColStats = new NoColumnStats(col.Database, col.Schema, col.Table, col.Column));
                noColStats.Count += count;
                if (!noColStats.Batches.TryGetValue(batch, out noColStatsBatchStatements)) noColStats.Batches.Add(batch, (noColStatsBatchStatements = new List<string>()));
                noColStatsBatchStatements.Add(warning.Statement.StatementText);
            }
        }
    }
}
