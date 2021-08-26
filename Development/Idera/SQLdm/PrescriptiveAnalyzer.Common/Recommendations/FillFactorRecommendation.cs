using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class FillFactorRecommendation : Recommendation, IProvideTableName
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        public string IndexName { get; set; }
        public string SchemaName { get; set; }
        public int    FillFactor { get; set; }
        public int    DataSizeMB { get; set; }
        public int    IndexSizeMB { get; set; }

        public FillFactorRecommendation() : base(RecommendationType.FillFactor) { }

        public FillFactorRecommendation(RecommendationProperties recProp) : base(RecommendationType.FillFactor, recProp) 
        {
            DatabaseName = recProp.GetString("DatabaseName");
            TableName = recProp.GetString("TableName");
            IndexName = recProp.GetString("IndexName");
            SchemaName = recProp.GetString("SchemaName");
            FillFactor = recProp.GetInt("FillFactor");
            DataSizeMB = recProp.GetInt("DataSizeMB");
            IndexSizeMB = recProp.GetInt("IndexSizeMB");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("DatabaseName", DatabaseName.ToString());
            prop.Add("TableName", TableName.ToString());
            prop.Add("IndexName", IndexName.ToString());
            prop.Add("SchemaName", SchemaName.ToString());
            prop.Add("FillFactor", FillFactor.ToString());
            prop.Add("DataSizeMB", DataSizeMB.ToString());
            prop.Add("IndexSizeMB", IndexSizeMB.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if (FillFactor < 65 && IndexSizeMB > 50)
                return HIGH_IMPACT;

            return base.AdjustImpactFactor(i);
        }

        public string Schema
        {
            get { return SchemaName; }
        }

        public string Table
        {
            get { return TableName; }
        }

        public string Database
        {
            get { return DatabaseName; }
        }

    }
}
