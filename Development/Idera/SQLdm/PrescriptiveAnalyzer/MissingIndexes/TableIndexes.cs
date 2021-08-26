using System;
using System.Collections.Generic;
//using System.Linq;
using System.IO;
using System.Xml;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Batches;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.MissingIndexes
{
    internal class TableIndex 
    {
        private const int colIndexName = 0;
        private const int colIndexId = 1;
        private const int colColumns = 2;
        private const int colIncluded = 3;
        private const int colIndexSize = 4;

        private string _indexName;
        private int _indexId;
        private long _indexSize;
        private List<string> _columns;
        private List<string> _includeColumns;

        public string IndexName { get { return (_indexName); } }
        public long IndexSize { get { return (_indexSize); } }
        public bool IsClustered { get { return (1 == _indexId); } }
        public bool IsHeap { get { return (0 == _indexId); } }

        private TableIndex() { }
        public TableIndex(SqlDataReader r) 
        {
            _indexName = DataHelper.ToString(r, colIndexName);
            _indexId = DataHelper.ToInt32(r, colIndexId);
            _indexSize = DataHelper.ToLong(r, colIndexSize);
            _columns = ParseColumns(DataHelper.ToString(r, colColumns));
            _includeColumns = ParseColumns(DataHelper.ToString(r, colIncluded));
        }
        internal List<string> GetKeyColumns()
        {
            return (new List<string>(_columns));
        }
        private List<string> ParseColumns(string cols)
        {
            List<string> result = new List<string>();
            if (string.IsNullOrEmpty(cols)) return(result);
            try
            {
                using (StringReader sr = new StringReader("<Columns>" + cols + "</Columns>"))
                using (XmlReader x = XmlReader.Create(sr))
                {
                    while (x.Read()) { if (XmlNodeType.Text == x.NodeType)if (!string.IsNullOrEmpty(x.Value)) result.Add(x.Value); }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(string.Format("TableIndex.ParseColumns({0}) Exception: ", cols), ex);
            }
            return (result);
        }

        internal bool IsPartialMatch(List<string> columns, List<string> includeColumns)
        {
            int n = 0;
            if (null != _columns)
            {
                if (null == columns) return (false);
                n = 0;
                foreach (string col in _columns)
                {
                    if (columns.Count <= n) return (false);
                    if (!ColumnsAreEqual(columns[n], col)) return (false);
                    ++n;
                }
            }
            if (null != _includeColumns)
            {
                if (null == includeColumns) return (false);
                n = 0;
                foreach (string col in _includeColumns)
                {
                    if (includeColumns.Count <= n) return (false);
                    if (!ColumnsAreEqual(includeColumns[n], col)) return (false);
                    ++n;
                }
            }
            return (true);
        }

        private bool ColumnsAreEqual(string p, string col)
        {
            return (0 == string.Compare(p, col, true));
        }


        internal bool IsMatch(DMVMissingIndex i)
        {
            int n = 0;
            if (IsHeap) return (false);
            List<string> ic = AppendColumns(i.GetEqualityColumnNames(), i.GetInequalityColumnNames());
            if (IsEmpty(ic)) return (true);
            if (IsEmpty(_columns)) return (false);
            if (ic.Count > _columns.Count) return (false);
            for (n = 0; n < ic.Count; ++n) if (!ColumnsAreEqual(ic[n], _columns[n])) return (false);
            if (IsClustered) return (true);
            IEnumerable<string> include = i.GetIncludeColumnNames();
            if (IsEmpty(include)) return (true);
            if (IsEmpty(_includeColumns)) return (false);
            int includeCount =0;
            foreach (var item in include)
            { includeCount++; }
            //if (include.Count() > _includeColumns.Count) return (false);
            if (includeCount > _includeColumns.Count) return (false);
            n = 0;
            foreach (string c in include) if (!ColumnsAreEqual(c, _includeColumns[n++])) return (false);
            return (true);
        }

        private bool IsEmpty(IEnumerable<string> i)
        {
            if (null == i) return (true);
            int iCount = 0;
            foreach (var item in i)
            { iCount++; }
            //return (i.Count() <= 0);
            return (iCount <= 0);
        }

        private List<string> AppendColumns(IEnumerable<string> i1, IEnumerable<string> i2)
        {
            List<string> result = new List<string>();
            if (null != i1) result.AddRange(i1);
            if (null != i2) result.AddRange(i2);
            return (result);
        }
    }
    internal class TableIndexes 
    {
        private List<TableIndex> _indexes = new List<TableIndex>();
        public string Database { get; private set; }
        public string Schema { get; private set; }
        public string Table { get; private set; }
        private TableIndexes() { }
        public TableIndexes(SqlConnection conn, string database, string schema, string table) 
        {
            Database = database;
            Schema = schema;
            Table = table;
            try
            {
                string sql = BatchFinder.GetIndexColumnsForTable(new ServerVersion(conn.ServerVersion), database, schema, table);
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                    cmd.CommandType = System.Data.CommandType.Text;
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            _indexes.Add(new TableIndex(r));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(string.Format("TableIndexes({0}) Exception: ", table), ex);
            }
        }

        internal IEnumerable<RedundantIndex> GetRedundantIndexes(List<string> columns, List<string> includeColumns)
        {
            foreach (TableIndex i in _indexes)
            {
                if (i.IsClustered || i.IsHeap) continue;
                if (i.IsPartialMatch(columns, includeColumns)) yield return (new RedundantIndex(i.IndexName, i.IndexSize));
            }
        }

        internal List<string> GetClusteredIndexKeyColumns()
        {
            foreach (TableIndex i in _indexes)
            {
                if (i.IsClustered)
                {
                    return (i.GetKeyColumns());
                }
            }
            return (null);
        }

        internal bool HasMatch(DMVMissingIndex i)
        {
            foreach (TableIndex ti in _indexes)
            {
                if (ti.IsMatch(i)) return (true);
            }
            return (false);
        }
    }
}
