using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.SQL
{
    public class WorstDatabaseFillFactorSnapshot
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public string SchemaName { get; set; }
        public int    FillFactor { get; set; }
        public int   DataSizeMB { get; set; }
        public int   IndexsizeMB { get; set; }

        internal WorstDatabaseFillFactorSnapshot(DataRow row)
        {
            DatabaseName = DataHelper.ToString(row, 0);
            TableName   = DataHelper.ToString(row, 1);
            IndexName = DataHelper.ToString(row, 2);
            SchemaName  = DataHelper.ToString(row, 3);
            FillFactor  = DataHelper.ToInt32(row, "fillfactor");
            DataSizeMB  = DataHelper.ToInt32(row, "datasizeinmb");
            IndexsizeMB = DataHelper.ToInt32(row, "indexsizeinmb");
        }
    }
}
