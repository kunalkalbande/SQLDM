using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class RecommendedIndex : IProvideTableName
    {
        private const int MAX_INDEX_NAME_LENGTH = 128;

        private readonly string _db;
        private readonly string _schema;
        private readonly string _table;
        private readonly long _estSize;
        private readonly List<string> _equalColumns = null;
        private readonly List<string> _notEqualColumns = null;
        private readonly List<string> _includeColumns = null;
        private readonly List<RedundantIndex> _redundantIndexes = null;
        private string _indexName = null;
        private RecommendedIndex() { }
        public RecommendedIndex(long estSize, string db, string schema, string table, IEnumerable<string> equalColumns, IEnumerable<string> notequalColumns, IEnumerable<string> includeColumns, IEnumerable<RedundantIndex> redundantIndexes) 
        {
            _estSize = estSize;
            _db = db;
            _schema = schema;
            _table = table;
            if (null != equalColumns) { _equalColumns = new List<string>(); _equalColumns.AddRange(equalColumns); }
            if (null != notequalColumns) { _notEqualColumns = new List<string>(); _notEqualColumns.AddRange(notequalColumns); }
            if (null != includeColumns) { _includeColumns = new List<string>(); _includeColumns.AddRange(includeColumns); }
            if (null != redundantIndexes) { _redundantIndexes = new List<RedundantIndex>(); _redundantIndexes.AddRange(redundantIndexes); }
        }
        public string BracketDatabase { get { return SQLHelper.Bracket(_db); } }
        public string BracketSchemaTable { get { return SQLHelper.Bracket(_schema, _table); } }
        public string BracketIndexName { get { return SQLHelper.Bracket(IndexName); } }
        public string SafeSchemaTable { get { return SQLHelper.CreateSafeString(BracketSchemaTable); } }
        public string SafeIndexName { get { return SQLHelper.CreateSafeString(IndexName); } }

        public string Database { get { return _db; } }
        public string Schema { get { return _schema; } }
        public string Table { get { return _table; } }
        public long EstSize { get { return _estSize; } }
        public ICollection<string> EqualColumns { get { return _equalColumns; } }
        public ICollection<string> NotEqualColumns { get { return _notEqualColumns; } }
        public ICollection<string> IncludeColumns { get { return _includeColumns; } }
        public ICollection<RedundantIndex> RedundantIndexes { get { return _redundantIndexes; } }

        public ICollection<string> KeyColumns 
        { 
            get 
            {
                List<string> cols = new List<string>();
                Append(cols, EqualColumns);
                Append(cols, NotEqualColumns);
                return (cols); 
            } 
        }

        public bool IsMatch(RecommendedIndex r)
        {
            if (0 != string.Compare(this.Database, r.Database)) return (false);
            if (0 != string.Compare(this.Schema, r.Schema)) return (false);
            if (0 != string.Compare(this.Table, r.Table)) return (false);
            if (!IsMatch(this.EqualColumns, r.EqualColumns)) return (false);
            if (!IsMatch(this.NotEqualColumns, r.NotEqualColumns)) return (false);
            if (!IsMatch(this.IncludeColumns, r.IncludeColumns)) return (false);
            return (true);
        }

        private bool IsMatch(ICollection<string> i1, ICollection<string> i2)
        {
            if ((null == i1) && (null == i2)) return (true);
            if ((null == i1) && (i2.Count <= 0)) return (true);
            if ((null == i2) && (i1.Count <= 0)) return (true);
            if ((null == i1) || (null == i2)) return (false);
            if (i1.Count != i2.Count) return (false);
            for (int n = 0; n < i1.Count; ++n)
            {
                //if (0 != string.Compare(i1.ElementAt(n), i2.ElementAt(n))) return (false);
                string[] array1 = new string[i1.Count];
                i1.CopyTo(array1, 0);
                string[] array2 = new string[i2.Count];
                i2.CopyTo(array2, 0);
                if (0 != string.Compare(array1[n], array2[n])) return (false);
            }
            return (true);
        }

        public string IndexName
        {
            get
            {
                if (!string.IsNullOrEmpty(_indexName)) return (_indexName);
                StringBuilder sb = new StringBuilder(string.Format("IX_{0}", _table));
                Append(sb, EqualColumns);
                Append(sb, NotEqualColumns);
                Append(sb, IncludeColumns);

                if (sb.Length > MAX_INDEX_NAME_LENGTH)
                {
                    sb.Length = MAX_INDEX_NAME_LENGTH;
                }
                _indexName = sb.ToString();
                return (_indexName);
            }
        }

        private void Append(List<string> l, ICollection<string> c)
        {
            if (null == l) return;
            if (null == c) return;
            l.AddRange(c);
        }

        private void Append(StringBuilder sb, ICollection<string> cols)
        {
            if (null == cols) return;
            if (null == sb) return;
            if (cols.Count <= 0) return;
            foreach (string col in cols)
            {
                if (!string.IsNullOrEmpty(col)) sb.Append("_" + SQLHelper.RemoveBrackets(col));
            }
        }

    }

    public class RecommendedIndexComparer : IEqualityComparer<RecommendedIndex>
    {
        public bool Equals(RecommendedIndex x, RecommendedIndex y)
        {
            return (x.IsMatch(y));
        }

        public int GetHashCode(RecommendedIndex obj)
        {
            return (obj.GetHashCode());
        }
    }
}
