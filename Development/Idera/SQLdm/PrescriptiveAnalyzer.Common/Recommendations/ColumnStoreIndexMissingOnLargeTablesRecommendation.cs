using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-I30 
    /// </summary>
    [Serializable]
    public class ColumnStoreIndexMissingOnLargeTablesRecommendation : Recommendation, IProvideTableName
    {
        public string Database { get; private set; }
        public string Schema { get; private set; }
        public string Table { get; private set; }


        public ColumnStoreIndexMissingOnLargeTablesRecommendation(string db, string schema, string table)
            : base(RecommendationType.ColumnStoreIndexMissingOnLargeTables)
        {
            Database = db;
            Schema = schema;
            Table = table;
        }
        public ColumnStoreIndexMissingOnLargeTablesRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.ColumnStoreIndexMissingOnLargeTables, recProp)
        {
            Database = recProp.GetString("dbname");
            Table = recProp.GetString("TableName");
            Schema = recProp.GetString("SchemaName");
        }
        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("dbname", Database.ToString());
            prop.Add("TableName", Table.ToString());
            prop.Add("SchemaName", Schema.ToString());
            return prop;
        }
    }
}
