using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Batches;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan;

namespace Idera.SQLdm.PrescriptiveAnalyzer.MissingIndexes
{
    internal class HypotheticalIndexes
    {
        private static int _staticCount = 0;

        public readonly int ID;

        private List<MissingIndex> _indexesToAdd = new List<MissingIndex>();
        private List<AddedIndex> _indexesAdded = new List<AddedIndex>();
        private int _dbid = 0;
        private string _db = string.Empty;
        private string _schema = string.Empty;
        private string _table = string.Empty;
        public HypotheticalIndexes(string db, string schema, string table)
        {
            _db = db;
            _schema = schema;
            _table = table;
            ID = System.Threading.Interlocked.Increment(ref _staticCount);
        }
        public HypotheticalIndexes(string db, string schema, string table, MissingIndex mi, bool permutate)
            : this(db, schema, table)
        {
            if (permutate) AddPermutations(mi); else _indexesToAdd.Add(mi);
        }

        public HypotheticalIndexes(string db, string schema, string table, IEnumerable<MissingIndex> mis)
            : this(db, schema, table)
        {
            foreach (MissingIndex mi in mis) AddPermutations(mi);
        }
        public int NeededIndexCount {get { return(_indexesToAdd.Count);}}
        private void AddPermutations(MissingIndex mi)
        {
            foreach (MissingIndex permutation in mi.GetPermutations()) _indexesToAdd.Add(permutation);
        }
        public void ResetIndexes(SqlConnection conn)
        {
            AutoPilotHelpers.ResetHypotheticalIndexes(conn, _dbid);
        }

        public void DeleteIndexes(SqlConnection conn)
        {
            foreach (AddedIndex ai in _indexesAdded)
            {
                DisableIndex(conn, ai);
                DropIndex(conn, ai);
            }
        }

        public IEnumerable<string> GetDropAddedIndexes(SqlConnection conn)
        {
            string dbSchemaTable = SQLHelper.Bracket(_db, _schema, _table);
            string tsql = string.Empty;
            foreach (AddedIndex ai in _indexesAdded)
            {
                try
                {
                    tsql = BatchFinder.GetDropIndex(new ServerVersion(conn.ServerVersion), ai.Name, dbSchemaTable, true);
                }
                catch (Exception ex)
                {
                    tsql = string.Empty;
                    ExceptionLogger.Log(string.Format("HypotheticalIndexes.GetDropAddedIndexes(Table:{0}, Index:{1}) Exception: ", dbSchemaTable, ai.Name), ex);
                }
                yield return (tsql);
            }
        }

        private void DropIndex(SqlConnection conn, AddedIndex ai)
        {
            string dbSchemaTable = SQLHelper.Bracket(_db, _schema, _table);
            try
            {
                string sql = BatchFinder.GetDropIndex(new ServerVersion(conn.ServerVersion), ai.Name, dbSchemaTable, false);
                ExecNonQuery(sql, conn);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(string.Format("HypotheticalIndexes.DropIndex(Table:{0}, Index:{1}) Exception: ", dbSchemaTable, ai.Name), ex);
            }
        }

        private void DisableIndex(SqlConnection conn, AddedIndex ai)
        {
            string dbSchemaTable = SQLHelper.Bracket(_db, _schema, _table);
            try
            {
                string sql = BatchFinder.GetDisableIndex(new ServerVersion(conn.ServerVersion), ai.Name, dbSchemaTable);
                ExecNonQuery(sql, conn);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(string.Format("HypotheticalIndexes.DisableIndex(Table:{0}, Index:{1}) Exception: ", dbSchemaTable, ai.Name), ex);
            }
        }

        private void ExecNonQuery(string sql, SqlConnection conn)
        {
            using (SqlCommand command = new SqlCommand(sql, conn))
            {
                command.CommandType = CommandType.Text;
                command.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                command.ExecuteNonQuery();
            }
        }

        public void AddIndexes(SqlConnection conn)
        {
            string indexName;
            string sql;
            string dbSchemaTable = SQLHelper.Bracket(_db, _schema, _table);
            int n = 0;
            int id;
            int tableid = SQLHelper.GetObjectId(conn, dbSchemaTable);
            ServerVersion ver = new ServerVersion(conn.ServerVersion);
            _dbid = SQLHelper.GetDatabaseId(conn, _db);
            foreach (MissingIndex mi in _indexesToAdd)
            {
                indexName = string.Format("IX_SQLdoctor_{0}_{1}_{2}", ID, ++n, Guid.NewGuid().ToString("N"));
                sql = BatchFinder.GetCreateNonClusteredIndex(ver, indexName, dbSchemaTable, mi.GetColumnListForCreate());                                    
                try
                {
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                        id = Convert.ToInt32(command.ExecuteScalar());
                        _indexesAdded.Add(new AddedIndex(id, indexName, mi.ID));
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log("HypotheticalIndexes.AddIndexes Exception: ", ex);
                }
            }
            if (_indexesAdded.Count <= 0) return;
            List<AutoPilotHelpers.IndexStats> stats = new List<AutoPilotHelpers.IndexStats>(_indexesAdded.Count);
            foreach (AddedIndex ai in _indexesAdded)
            {
                stats.Add(new AutoPilotHelpers.IndexStats(ai.ID, 0, 0));
            }
            AutoPilotHelpers.SetHypotheticalIndexes(conn, _dbid, tableid, stats);
        }
        public IEnumerable<HypotheticalIndexes> GetConsolidatedIndexes()
        {
            foreach (MissingIndex mi in MissingIndex.CreateConsolidatedIndexes(_indexesToAdd))
            {
                yield return (new HypotheticalIndexes(_db, _schema, _table, mi, false));
            }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("Table: {0}", SQLHelper.Bracket(_db, _schema, _table)));
            foreach (MissingIndex i in _indexesToAdd)
            {
                sb.Append(" Index: " + i.ToString());
            }
            return sb.ToString();
        }
        public IEnumerable<RecommendedIndex> GetRecommendedIndexes(SqlConnection conn, TableIndexes indexes)
        {
            List<RecommendedIndex> result = new List<RecommendedIndex>();
            long estSize = 0;
            ServerVersion ver = new ServerVersion(conn.ServerVersion);
            foreach (MissingIndex mi in _indexesToAdd)
            {
                estSize = IndexHelper.GetNonclusteredIndexSize(ver, conn, _db, _schema, _table, mi.GetColumnNames(MissingIndexColumnType.Equality), mi.GetColumnNames(MissingIndexColumnType.Inequality), mi.GetColumnNames(MissingIndexColumnType.Include));
                result.Add(new RecommendedIndex(estSize, _db, _schema, _table, mi.GetColumnNames(MissingIndexColumnType.Equality), mi.GetColumnNames(MissingIndexColumnType.Inequality), mi.GetColumnNames(MissingIndexColumnType.Include), mi.GetRedundantIndexes(indexes)));
            }
            return result;
        }

        internal void PurgeIndexes(IEnumerable<ShowPlanXML> plans, SqlSystemObjectManager ssom)
        {
            List<Int64> indexesUsed = new List<Int64>();
            AddedIndex addedIndex;
            foreach (string index in GetIndexes(plans, ssom))
            {
                addedIndex = _indexesAdded.Find(delegate(AddedIndex ai) { return (0 == string.Compare(ai.Name, index, true)); });
                if (null != addedIndex) { if (!indexesUsed.Contains(addedIndex.MissingIndexID)) indexesUsed.Add(addedIndex.MissingIndexID); }
            }
            _indexesToAdd.RemoveAll(delegate(MissingIndex mi) { return (!indexesUsed.Contains(mi.ID)); });
        }

        private IEnumerable<string> GetIndexes(IEnumerable<ShowPlanXML> plans, SqlSystemObjectManager ssom)
        {
            ExecutionPlanTables ept = new ExecutionPlanTables(ssom);
            string prefix = string.Format("{0}.[", SQLHelper.Bracket(_db, _schema, _table));
            foreach (ShowPlanXML plan in plans)
            {
                foreach (string index in ept.GetIndexes(plan, Convert.ToUInt32(_dbid)))
                {
                    if (index.StartsWith(prefix))
                    {
                        yield return (index.Substring(prefix.Length).TrimEnd(']'));
                    }
                }
            }
        }

        internal bool ContainsFewerColumns(HypotheticalIndexes hi)
        {
            if (null == hi) return (false);
            //return (_indexesToAdd.Sum(x => x.GetColumnCount()) < hi._indexesToAdd.Sum(x => x.GetColumnCount()));
            int sum1 = 0;
            int sum2 = 0;
            foreach (MissingIndex i in _indexesToAdd)
            {
                sum1 += i.GetColumnCount();
            }
            foreach (MissingIndex i in hi._indexesToAdd)
            {
                sum2 += i.GetColumnCount();
            }
            return (sum1 < sum2);
        }
    }

}
