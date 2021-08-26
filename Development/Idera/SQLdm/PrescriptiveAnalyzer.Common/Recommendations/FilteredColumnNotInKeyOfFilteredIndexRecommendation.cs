using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// // SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-I31 -Adding new recommendation class
    /// </summary>
    [Serializable]
    public class FilteredColumnNotInKeyOfFilteredIndexRecommendation : IndexRecommendation
    {
        public string DbName;
        public string FilteredTableName;
        public string SchemaName;

        public string FilteredIndexName;

        public FilteredColumnNotInKeyOfFilteredIndexRecommendation(string dbName, string schema, string tableName, string indexName)
            : base(RecommendationType.FilteredColumnNotInKeyOfFilteredIndex, dbName, schema, tableName, indexName)
        {
            DbName = dbName;
            FilteredTableName = tableName;
            SchemaName = schema;
            FilteredIndexName = indexName;
        }
        public FilteredColumnNotInKeyOfFilteredIndexRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.FilteredColumnNotInKeyOfFilteredIndex, recProp)
        {
            DbName = recProp.GetString("dbname");
            FilteredTableName = recProp.GetString("TableName");
            SchemaName = recProp.GetString("SchemaName");
            FilteredIndexName = recProp.GetString("IndexName");
        }
        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("dbname", DbName.ToString());
            prop.Add("TableName", FilteredTableName.ToString());
            prop.Add("SchemaName", SchemaName.ToString());
            prop.Add("IndexName", FilteredIndexName.ToString());
            return prop;
        }
    }
}
