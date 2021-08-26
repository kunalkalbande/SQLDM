using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// // SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-I27 -Adding new recommendation class
    /// </summary>
    [Serializable]
    public class ScannedHashIndexRecommendation : IndexRecommendation
    {
        public string ScannedDbName;
        public string ScannedTableName;
        public string ScannedSchemaName;
        public string ScannedIndexName;

        public ScannedHashIndexRecommendation(string dbName, string schema, string tableName, string indexName)
            : base(RecommendationType.ScannedHashIndex, dbName, schema, tableName, indexName)
        {
            ScannedDbName = dbName;
            ScannedTableName = tableName;
            ScannedSchemaName = schema;
            ScannedIndexName = indexName;
        }
        public ScannedHashIndexRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.ScannedHashIndex, recProp)
        {
            ScannedDbName = recProp.GetString("dbname");
            ScannedTableName = recProp.GetString("TableName");
            ScannedSchemaName = recProp.GetString("SchemaName");
            ScannedIndexName = recProp.GetString("IndexName");
        }
        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("dbname", ScannedDbName.ToString());
            prop.Add("TableName", ScannedTableName.ToString());
            prop.Add("SchemaName", ScannedSchemaName.ToString());
            prop.Add("IndexName", ScannedIndexName.ToString());
            return prop;
        }

    }
}
