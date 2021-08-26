using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Services;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class WaitStatsPageLatchIndexRecommendation : WaitStatsRecommendation, IProvideTableName, IProvideAffectedBatches
    {
        public string Database { get; private set; }
        public string Schema { get; private set; }
        public string Table { get; private set; }
        public string IndexName { get; private set; }
        public UInt32 IndexId { get; private set; }

        public WaitStatsPageLatchIndexRecommendation(AffectedBatches ab, string db, string schema, string table, UInt32 indexId, string index)
            : base(ab, RecommendationType.WaitStatsPageLatchIndex)
        {
            Database = db;
            Schema = schema;
            Table = table;
            IndexName = index;
            IndexId = indexId;
        }

        public WaitStatsPageLatchIndexRecommendation(RecommendationProperties recProp)
            : base( RecommendationType.WaitStatsPageLatchIndex, recProp)
        {
            Database = recProp.GetString("Database");
            Schema = recProp.GetString("Schema");
            Table = recProp.GetString("Table");
            IndexName = recProp.GetString("IndexName");
            IndexId = recProp.GetUInt32("IndexId");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Database", Database.ToString());
            prop.Add("Schema", Schema.ToString());
            prop.Add("Table", Table.ToString());
            prop.Add("IndexName", IndexName.ToString());
            prop.Add("IndexId", IndexId.ToString());
            return prop;
        }
    }
}
