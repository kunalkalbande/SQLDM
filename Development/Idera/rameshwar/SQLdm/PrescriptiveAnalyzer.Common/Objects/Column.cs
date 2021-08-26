using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Objects
{
    [Serializable]
    public class Column
    {
        public int KeyOrdinal;
        public int PartitionOrdinal;
        public bool IsDescendingKey;
        public bool IsIncludedColumn;
        public string ColumnName;

        public Column()
        { }

        public Column(string name, int keyOrdinal, int partitionOrdinal, bool isDescendingKey, bool isIncludedColumn)
        {
            ColumnName = name;
            KeyOrdinal = keyOrdinal;
            PartitionOrdinal = partitionOrdinal;
            IsDescendingKey = isDescendingKey;
            IsIncludedColumn = isIncludedColumn;
        }

        public string SortOrder
        {
            get { return (IsDescendingKey) ? "DESC" : "ASC"; }
            set {}
        }

        public bool IsPartitionColumn
        {
            get { return (0 < PartitionOrdinal); }
            set { }
        }
    }
}
