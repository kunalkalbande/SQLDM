using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Services;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class WaitStatsPageLatchTableRecommendation : WaitStatsRecommendation, IProvideTableName, IProvideAffectedBatches
    {
        public string Database { get; private set; }
        public string Schema { get; private set; }
        public string Table { get; private set; }

        public WaitStatsPageLatchTableRecommendation(AffectedBatches ab, string db, string schema, string table)
            : base(ab, RecommendationType.WaitStatsPageLatchTable)
        {
            Database = db;
            Schema = schema;
            Table = table;
        }

        public WaitStatsPageLatchTableRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.WaitStatsPageLatchTable, recProp)
        {
            Database = recProp.GetString("Database");
            Schema = recProp.GetString("Schema");
            Table = recProp.GetString("Table");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Database", Database.ToString());
            prop.Add("Schema", Schema.ToString());
            prop.Add("Table", Table.ToString());
            return prop;
        }
    }
}
