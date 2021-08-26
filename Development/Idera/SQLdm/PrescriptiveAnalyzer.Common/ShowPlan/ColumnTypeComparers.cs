using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan
{
    public class ColumnTypeComparer : IComparer<ColumnType>
    {
        public int Compare(ColumnType x, ColumnType y)
        {
            return (x.ColumnId.CompareTo(y.ColumnId));
        }
    }
    public class ColumnTypeEqualityComparer : IEqualityComparer<ColumnType>
    {
        public bool Equals(ColumnType x, ColumnType y)
        {
            return (x.ColumnId.Equals(y.ColumnId));
        }

        public int GetHashCode(ColumnType obj)
        {
            return (obj.ColumnId);
        }
    }

}
