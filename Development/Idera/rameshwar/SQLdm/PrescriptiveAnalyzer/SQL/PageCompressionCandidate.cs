using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Idera.SQLdoctor.Common.Helpers;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.SQL
{
    internal class PageCompressionCandidate
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        public string SchemaName { get; set; }
        public string IndexName { get; set; }
        public int CurrentSizeKB { get; set; }
        public int CompressedSizeKB { get; set; }

        internal PageCompressionCandidate(DataRow row)
        {
            DatabaseName = DataHelper.ToString(row, 0);
            TableName = DataHelper.ToString(row, 1);
            SchemaName = DataHelper.ToString(row, 2);
            IndexName = DataHelper.ToString(row, 3);
            CurrentSizeKB = DataHelper.ToInt32(row, "current");
            CompressedSizeKB = DataHelper.ToInt32(row, "compressed");
        }
    }
}
