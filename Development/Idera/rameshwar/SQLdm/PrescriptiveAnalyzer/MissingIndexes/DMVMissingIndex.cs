using System;
using System.Data;
using System.Data.SqlClient;
//using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.State;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Batches;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.MissingIndexes
{
    internal class DMVMissingIndex
    {
        public string Database { get; private set; }
        public string Schema { get; private set; }
        public string Table { get; private set; }
        public string FullTableName { get { return (SQLHelper.Bracket(Database, Schema, Table)); } }
        public double TotalCost { get; private set; }
        public double Impact { get; private set; }
        public double AvgUserCost { get; private set; }
        public double AvgUserImpact { get; private set; }
        public UInt64 UserScans { get; private set; }
        public UInt64 UserSeeks { get; private set; }
        public string EqualityColumns { get; private set; }
        public string InequalityColumns { get; private set; }
        public string IncludedColumns { get; private set; }
        private List<string> _clusteredIndexKeyColumns = null;

        public ICollection<string> AllColumns
        {
            get
            {
                List<string> cols = new List<string>();
                Append(cols, GetEqualityColumnNames());
                Append(cols, GetInequalityColumnNames());
                Append(cols, GetIncludeColumnNames());
                for (int i = 0; i < cols.Count; i++)
                    cols[i] = SQLHelper.RemoveBrackets(cols[i]);
                return (cols);
            }
        }

        public DMVMissingIndex(DataRow dr, SQLSchemaNameHelper ssnh)
        {
            Database = DataHelper.ToString(dr, "Database");
            TotalCost = DataHelper.ToDouble(dr, "TotalCost");
            Impact = DataHelper.ToDouble(dr, "Impact");
            AvgUserCost = DataHelper.ToDouble(dr, "avg_total_user_cost");
            AvgUserImpact = DataHelper.ToDouble(dr, "avg_user_impact");
            UserScans = DataHelper.ToUInt64(dr, "user_scans");
            UserSeeks = DataHelper.ToUInt64(dr, "user_seeks");
            EqualityColumns = DataHelper.ToString(dr, "equality_columns");
            InequalityColumns = DataHelper.ToString(dr, "inequality_columns");
            IncludedColumns = DataHelper.ToString(dr, "included_columns");

            UInt32 id = DataHelper.ToUInt32(dr, "ObjectID");
            Schema = ssnh.GetSchemaName(id, Database);
            Table = ssnh.GetObjectName(id, Database);
        }

        private void Append(List<string> l, IEnumerable<string> c)
        {
            if (null == l) return;
            if (null == c) return;
            l.AddRange(c);
        }

        private IEnumerable<string> ParseColumnNames(string p)
        {
            List<string> names = new List<string>();
            if (string.IsNullOrEmpty(p)) return (names);
            string[] cols = p.Split(new string[] {"], "}, StringSplitOptions.None);
            if (null != cols)
            {
                for (int n = 0; n < cols.Length; ++n)
                {
                    if (string.IsNullOrEmpty(cols[n].Trim())) continue;
                    names.Add((cols[n].StartsWith("[") && !cols[n].EndsWith("]")) ? cols[n] + "]" : cols[n]);
                }
            }
            return (names);
        }

        internal IEnumerable<string> GetEqualityColumnNames()
        {
            return (ParseColumnNames(EqualityColumns));
        }

        internal IEnumerable<string> GetInequalityColumnNames()
        {
            return (ParseColumnNames(InequalityColumns));
        }

        internal IEnumerable<string> GetIncludeColumnNames()
        {
            IEnumerable<string> temp = ParseColumnNames(IncludedColumns);
            if (null == _clusteredIndexKeyColumns) return (temp);
            if (_clusteredIndexKeyColumns.Count <= 0) return (temp);
            if (null == temp) return (temp);
            List<string> includes = new List<string>(temp);
            //includes.RemoveAll(delegate(string col) { return (_clusteredIndexKeyColumns.Contains(col, StringComparer.InvariantCultureIgnoreCase) || _clusteredIndexKeyColumns.Contains(col.TrimStart('[').TrimEnd(']'), StringComparer.InvariantCultureIgnoreCase)); });
            includes.RemoveAll(delegate(string col) 
            {
                bool res1 = false;
                bool res2 = false;
                foreach (string c in _clusteredIndexKeyColumns)
                {
                    if (StringComparer.InvariantCultureIgnoreCase.Equals(col, c))
                    {
                        res1 = true;
                    }
                }
                foreach (string c in _clusteredIndexKeyColumns)
                {
                    if (StringComparer.InvariantCultureIgnoreCase.Equals(col.TrimStart('[').TrimEnd(']'), c))
                    {
                        res2 = true;
                    }
                }
                return (res1 || res2);
            });
            return (includes);
        }
        internal void SetClusteredKeyColumns(List<string> clusterColumns)
        {
            _clusteredIndexKeyColumns = new List<string>(clusterColumns);
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DMVMissingIndex on table " + FullTableName + " ");
            if (!string.IsNullOrEmpty(EqualityColumns)) sb.Append(string.Format("Equal ({0}) ", EqualityColumns));
            if (!string.IsNullOrEmpty(InequalityColumns)) sb.Append(string.Format("NotEqual ({0}) ", InequalityColumns));
            if (!string.IsNullOrEmpty(IncludedColumns)) sb.Append(string.Format("Include ({0}) ", IncludedColumns));
            return (sb.ToString());
        }

    }

    public class DMVMissingIndexes
    {

        private static Logger _logX = Logger.GetLogger("DMVMissingIndexes");

        private Dictionary<string, List<DMVMissingIndex>> _missingIndexes = new Dictionary<string, List<DMVMissingIndex>>();
        private List<IRecommendation> _recommendations = new List<IRecommendation>();
        private BaseOptions _ops;

        private DMVMissingIndexes() { }
        public DMVMissingIndexes(BaseOptions ops)
        {
            using (_logX.InfoCall("DMVMissingIndexes Construction"))
            {
                try
                {
                    _ops = ops;
                    using (SqlConnection conn = SQLHelper.GetConnection(_ops.ConnectionInfo))
                    {
                        Load(conn, new ServerVersion(conn.ServerVersion));
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "DMVMissingIndexes.Load() Exception: ", ex);
                }
            }
        }
        private void Load(SqlConnection conn, ServerVersion ver)
        {
            using (DataSet ds = new DataSet())
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(BatchFinder.GetBatch("DMVMissingIndexes", ver), conn))
                {
                    adapter.SelectCommand.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                    try
                    {
                        adapter.Fill(ds);
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogger.Log(_logX, "DMVMissingIndexes.SqlDataAdapter.Fill() Exception: ", ex);
                    }
                }
                if (null == ds) return;
                if (null == ds.Tables) return;
                SQLSchemaNameHelper ssnh = new SQLSchemaNameHelper(conn);
                foreach (DataTable dt in ds.Tables)
                {
                    if (null == dt) continue;
                    if (null == dt.Rows) continue;
                    if (0 >= dt.Rows.Count) continue;
                    List<DMVMissingIndex> indexes = null;
                    DMVMissingIndex i = null;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (null == dr) continue;
                        i = new DMVMissingIndex(dr, ssnh);
                        //----------------------------------------------------------------------------
                        // Per PR DR294 - no not include the missing index recommendation if the computed
                        // impact is less than 30000.
                        // 
                        if (i.Impact < Settings.Default.Min_DMVMissingIndexImpact) 
                        {
                            _logX.InfoFormat("Missing index {0} skipped due to impact {1} being less than {2}", i, i.Impact, Settings.Default.Min_DMVMissingIndexImpact); 
                            continue; 
                        }
                        if (!_missingIndexes.TryGetValue(i.FullTableName, out indexes)) _missingIndexes.Add(i.FullTableName, indexes = new List<DMVMissingIndex>());
                        indexes.Add(i);
                    }
                }
            }
        }
        public IEnumerable<IRecommendation> GetRecommendations()
        {
            return (_recommendations);
        }

        internal void RemoveMatches(MissingIndexesOnTable miot)
        {
            if (null == miot) return;
            if (null == _missingIndexes) return;
            using (_logX.InfoCall(string.Format("DMVMissingIndexes.RemoveMatches({0})", miot.FullTableName)))
            {
                List<DMVMissingIndex> indexes = null;
                if (!_missingIndexes.TryGetValue(miot.FullTableName, out indexes)) return;
                if (null == indexes) return;
                indexes.RemoveAll(delegate(DMVMissingIndex i) 
                {
                    if (null == i) return (true);
                    if (miot.HasPartialMatch(i))
                    {
                        _logX.Info("Removing DMV Missing Index: " + i.ToString());
                        return (true);
                    }
                    return (false);
                });
            }
        }


        internal void Analyze(SqlConnectionInfo connInfo)
        {
            using (_logX.InfoCall("DMVMissingIndexes.Analyze"))
            {
                try
                {
                    using (SqlConnection conn = SQLHelper.GetConnection(connInfo))
                    {
                        Analyze(conn, new ServerVersion(conn.ServerVersion));
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "DMVMissingIndexes.Analyze() Exception: ", ex);
                }
            }
        }

        private void Analyze(SqlConnection conn, ServerVersion ver)
        {
            ServerOverview so = new ServerOverview(conn, ver);
            if (_missingIndexes.Count <= 0) return;
            _ops.UpdateState(AnalysisStateType.DMVMissingIndexAnalysis, "Starting analysis of the missing index dmv...", 0, 100);
            int count = 0;
            foreach (KeyValuePair<string, List<DMVMissingIndex>> table in _missingIndexes)
            {
                _ops.UpdateState(AnalysisStateType.DMVMissingIndexAnalysis, string.Format("Analyzing table {0}.", table.Key), count++, _missingIndexes.Count);
                if (AllowIndexRecommendation(conn, ver, table.Value))
                {
                    AddRecommendations(conn, ver, table.Value, so);
                }
            }
            _ops.UpdateState(AnalysisStateType.DMVMissingIndexAnalysis, "Completed analysis of the missing index dmv.", 100, 100);
        }

        private void AddRecommendations(SqlConnection conn, ServerVersion ver, List<DMVMissingIndex> indexes, ServerOverview so)
        {
            if (null == indexes) return;
            if (indexes.Count <= 0) return;
            IEnumerable<RecommendedIndex> recs = GetRecommendedIndexes(conn, ver, indexes);
            if (null == recs) return;
            double totalImpact = 0;
            double avgUserCost = 0;
            double avgUserImpact = 0;
            double daysSinceServerStarted = 0;
            UInt64 userScans = 0;
            UInt64 userSeeks = 0;
            if (null != so) daysSinceServerStarted = so.UpTime.TotalDays;
            
            indexes.ForEach(delegate(DMVMissingIndex i) 
            { 
                totalImpact += i.Impact;
                avgUserCost += i.AvgUserCost;
                avgUserImpact += i.AvgUserImpact;
                userScans += i.UserScans;
                userSeeks += i.UserSeeks;
            });
            double updatesPerMin = GetTableUpdatesPerMin(conn, ver, indexes[0]);
            double updatesPerSec = updatesPerMin / 60;
            if (updatesPerSec > Settings.Default.Max_TableUpdatesPerSecForMissingIndexRecommendation)
            {
                _logX.InfoFormat("Recommendation for {0} is being skipped due to {1} table updates per second.  The max per second is {2}.", indexes[0], updatesPerSec, Settings.Default.Max_TableUpdatesPerSecForMissingIndexRecommendation);
                return;
            }
            var r = new DMVMissingIndexRecommendation(RemoveDups(recs), 
                                                        updatesPerSec, 
                                                        updatesPerMin,
                                                        totalImpact / indexes.Count,
                                                        avgUserCost,
                                                        avgUserImpact,
                                                        userScans,
                                                        userSeeks,
                                                        daysSinceServerStarted
                                                        );
            if (null != _ops)
            {
                if (_ops.IsDatabaseBlocked(r.Database))
                {
                    _logX.InfoFormat("DMV Missing Index recommendation ignored due to blocked database {0}", r.Database);
                }
                if (_ops.IsRecommendationTypeBlocked(r.RecommendationType))
                {
                    _logX.Info("DMV Missing Index recommendation ignored due to recommendation type being blocked");
                }
            }
            //----------------------------------------------------------------------------
            // If we already have too many recommendations, ignore the new recommendations
            // being added.  This is added as a fail safe limiting mechanism and is not intended
            // to select the best recommendations.
            // 
            if (Settings.Default.Max_RecommendationsPerAnalyzer > 0)
            {
                if (_recommendations.Count >= Settings.Default.Max_RecommendationsPerAnalyzer)
                {
                    using (_logX.InfoCall(string.Format("AddRecommendation() limit of {0} encountered", Settings.Default.Max_RecommendationsPerAnalyzer)))
                    {
                        _logX.InfoFormat("Recommendation being thrown away due to limit of {0} recommendations", Settings.Default.Max_RecommendationsPerAnalyzer);
                        _logX.InfoFormat("Recommendation:{0} - {1}", r.ID, r.FindingText);
                    }
                    return;
                }
            }
            _recommendations.Add(r);
        }

        private IEnumerable<RecommendedIndex> RemoveDups(IEnumerable<RecommendedIndex> recs)
        {
            if (null == recs) return (null);
            //int count = recs.Count();
            int recsCount = 0;
            foreach (var item in recs)
            { recsCount++; }
            if (recsCount <= 1) return (recs);
            List<RecommendedIndex> l = new List<RecommendedIndex>(recsCount);
            RecommendedIndexComparer c = new RecommendedIndexComparer();
            foreach (var ri in recs)
            {
                if (null == ri) continue;
                //if (!l.Contains(ri, c))
                bool contains = false;
                foreach (RecommendedIndex index in l)
                {
                    if (c.Equals(index, ri))
                    { contains = true; }
                }
                if (!contains)
                {
                    l.Add(ri);
                }
                else
                {
                    _logX.InfoFormat("Duplicate DMV index being skipped: {0}", ri.IndexName);
                }
            }
            return (l);
        }

        private double GetTableUpdatesPerSec(SqlConnection conn, ServerVersion ver, DMVMissingIndex i)
        {
            if (null == i) return (0);
            using (_logX.InfoCall("GetTableUpdatesPerSec for " + i.ToString()))
            {
                try
                {
                    string sql = BatchFinder.GetTableUpdatesPerSec(ver, i.Database, i.Schema, i.Table);
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                        object o = command.ExecuteScalar();
                        if (null != o)
                        {
                            _logX.Info("GetTableUpdatesPerSec result: " + o.ToString());
                            return (Convert.ToDouble(o));
                        }
                        else
                        {
                            _logX.Info("GetTableUpdatesPerSec result is null");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "GetTableUpdatesPerSec Exception: ", ex);
                }
                return (0);
            }
        }

        private double GetTableUpdatesPerMin(SqlConnection conn, ServerVersion ver, DMVMissingIndex i)
        {
            if (null == i) return (0);
            using (_logX.InfoCall("GetTableUpdatesPerMin for " + i.ToString()))
            {
                try
                {
                    string sql = BatchFinder.GetTableUpdatesPerMin(ver, i.Database, i.Schema, i.Table);
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                        object o = command.ExecuteScalar();
                        if (null != o)
                        {
                            _logX.Info("GetTableUpdatesPerMin result: " + o.ToString());
                            return (Convert.ToDouble(o));
                        }
                        else
                        {
                            _logX.Info("GetTableUpdatesPerMin result is null");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "GetTableUpdatesPerMin Exception: ", ex);
                }
                return (0);
            }
        }

        private IEnumerable<RecommendedIndex> GetRecommendedIndexes(SqlConnection conn, ServerVersion ver, List<DMVMissingIndex> indexes)
        {
            List<RecommendedIndex> result = new List<RecommendedIndex>();
            long estSize = 0;
            foreach (DMVMissingIndex mi in indexes)
            {
                estSize = IndexHelper.GetNonclusteredIndexSize(ver, conn, mi.Database, mi.Schema, mi.Table, mi.GetEqualityColumnNames(), mi.GetInequalityColumnNames(), mi.GetIncludeColumnNames());
                result.Add(new RecommendedIndex(estSize, mi.Database, mi.Schema, mi.Table, mi.GetEqualityColumnNames(), mi.GetInequalityColumnNames(), mi.GetIncludeColumnNames(), null));
            }
            return result;
        }

        private bool AllowIndexRecommendation(SqlConnection conn, ServerVersion ver, DMVMissingIndex i)
        {
            if (null == i) return (false);
            using (_logX.InfoCall("AllowIndexRecommendation for " + i.ToString()))
            {
                try
                {
                    string sql = BatchFinder.GetAllowIndexRecommendation(ver, i.Database, i.Schema, i.Table, i.AllColumns);
                    
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                        object o = command.ExecuteScalar();
                        if (null != o)
                        {
                            _logX.Info("AllowIndexRecommendation result: " + o.ToString());
                            if (o is string) return (false);
                            return (Convert.ToBoolean(o));
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "AllowIndexRecommendation Exception: ", ex);
                }
                return (false);
            }
        }
        private bool AllowIndexRecommendation(SqlConnection conn, ServerVersion ver, List<DMVMissingIndex> indexes)
        {
            RemoveExistingIndexes(conn, ver, indexes);
            if (null == indexes) return (false);
            if (0 >= indexes.Count) return (false);
            return (AllowIndexRecommendation(conn, ver, indexes[0]));
        }

        private void RemoveExistingIndexes(SqlConnection conn, ServerVersion ver, List<DMVMissingIndex> indexes)
        {
            if (null == indexes) return;
            if (0 >= indexes.Count) return;
            var ti = new TableIndexes(conn, indexes[0].Database, indexes[0].Schema, indexes[0].Table);
            RemoveClusteredKeyColumnsFromIncluded(ti, indexes);
            using (_logX.InfoCall(string.Format("DMVMissingIndexes.RemoveExistingIndexes({0})", SQLHelper.Bracket(ti.Database, ti.Schema, ti.Table))))
            {
                indexes.RemoveAll(delegate(DMVMissingIndex i)
                {
                    if (null == i) return (true);
                    if (ti.HasMatch(i))
                    {
                        _logX.Info("Index exists on table.  Removing DMV Missing Index: " + i.ToString());
                        return (true);
                    }
                    return (false);
                });
            }
        }

        private void RemoveClusteredKeyColumnsFromIncluded(TableIndexes ti, List<DMVMissingIndex> indexes)
        {
            if (null == indexes) return;
            if (null == ti) return;
            List<string> clusterColumns = ti.GetClusteredIndexKeyColumns();
            if (null == clusterColumns) return;
            if (clusterColumns.Count <= 0) return;
            foreach (DMVMissingIndex index in indexes)
            {
                if (null == index) continue;
                IEnumerable<string> includes = index.GetIncludeColumnNames();
                if (null == includes) continue;
                //if (includes.Count() <= 0) continue;
                int includesCount = 0;
                foreach (var item in includes)
                { includesCount++; }
                if (includesCount <= 0) continue;
                index.SetClusteredKeyColumns(clusterColumns);
            }
        }
    }
}
