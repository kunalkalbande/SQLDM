using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-I29 -Adding new recommendation class
    /// </summary>
    [Serializable]
    public class RarelyUsedIndexOnInMemoryTableRecommendation : IndexRecommendation
    {
        public string DbName;
        public string TableName;
        public string SchemaName;

        public string IndexName;
        //public long Rows_returned;
        //public long Scans_started;
        public RarelyUsedIndexOnInMemoryTableRecommendation(string dbName, string schema, string tableName, string indexName)
            : base(RecommendationType.RarelyUsedIndexOnInMemoryTable, dbName, schema, tableName, indexName)
        {
            DbName = dbName;
            TableName = tableName;
            SchemaName = schema;
            IndexName = indexName;
        }
        public RarelyUsedIndexOnInMemoryTableRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.RarelyUsedIndexOnInMemoryTable, recProp)
        {
            DbName = recProp.GetString("dbname");
            TableName = recProp.GetString("TableName");
            SchemaName = recProp.GetString("SchemaName");
            IndexName = recProp.GetString("IndexName");
        }
        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("dbname", DbName.ToString());
            prop.Add("TableName", TableName.ToString());
            prop.Add("SchemaName", SchemaName.ToString());
            prop.Add("IndexName", IndexName.ToString());
            return prop;
        }
    }
}
