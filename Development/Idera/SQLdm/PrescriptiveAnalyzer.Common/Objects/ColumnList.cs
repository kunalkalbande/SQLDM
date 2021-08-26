using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Objects
{
    [Serializable]
    public class ColumnList
    {
        public List<Column> Columns;

        public ColumnList()
        {
            Columns = new List<Column>();
        }

        public void Add(Column x)
        {
            Columns.Add(x);
        }

        public void Add(string name, int keyOrdinal, int partitionOrdinal, bool isDescendingKey, bool isIncludedColumn)
        {
            Columns.Add(new Column(name, keyOrdinal, partitionOrdinal, isDescendingKey, isIncludedColumn));
        }

        public string KeyColumnsString
        {
            get
            {

                if (1 == Columns.Count) return string.Format("{0}", SQLHelper.Bracket(Columns[0].ColumnName));

                StringBuilder keyColumnsString = new StringBuilder();
                Columns.Sort(CompareColumnsByKeyOrdinal);

                foreach (Column col in Columns)
                {
                    if (col.KeyOrdinal > 0)
                        if (0 == keyColumnsString.Length)
                            keyColumnsString.Append(string.Format("{0} {1}", SQLHelper.Bracket(col.ColumnName), col.SortOrder));
                        else
                            keyColumnsString.Append(string.Format(", {0} {1}", SQLHelper.Bracket(col.ColumnName), col.SortOrder));
                }
                return keyColumnsString.ToString();
            }

            set
            {
            }
        }

        public string IncludeColumnsString
        {
            get
            {
                StringBuilder includeColumnsString = new StringBuilder();

                foreach (Column col in Columns)
                {
                    if (col.IsIncludedColumn)
                        if (0 == includeColumnsString.Length)
                            includeColumnsString.Append(string.Format("{0}", SQLHelper.Bracket(col.ColumnName)));
                        else
                            includeColumnsString.Append(string.Format(", {0}", SQLHelper.Bracket(col.ColumnName)));
                }
                return includeColumnsString.ToString();
            }

            set { }
        }

        private static int CompareColumnsByKeyOrdinal(Column x, Column y)
        {
            return x.KeyOrdinal.CompareTo(y.KeyOrdinal);
        }

        public Column PartitionColumn
        {
            get
            {
                return Columns.Find(delegate(Column c) { return c.IsPartitionColumn; });
            }
            set { }
        }
    }
}
