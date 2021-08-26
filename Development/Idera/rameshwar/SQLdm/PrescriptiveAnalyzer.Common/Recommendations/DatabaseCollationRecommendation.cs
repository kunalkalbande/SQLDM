using System;
using System.Collections.Generic;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DatabaseCollationRecommendation : Recommendation, IProvideDatabase
    {
        public string Database { get; private set; }
        public readonly string CurrentCollation;
        public readonly string ModelCollation;

        public DatabaseCollationRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DatabaseCollation,recProp)
        {
            Database = recProp.GetString("Database");
            CurrentCollation = recProp.GetString("CurrentCollation");
            ModelCollation = recProp.GetString("ModelCollation");
        }

        public DatabaseCollationRecommendation(string database, string currentCollation, string modelCollation) : base(RecommendationType.DatabaseCollation)
        {
            Database = database;
            CurrentCollation = currentCollation;
            ModelCollation = modelCollation;
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Database", Database.ToString());
            prop.Add("CurrentCollation", CurrentCollation.ToString());
            prop.Add("ModelCollation", ModelCollation.ToString());
            return prop;
        }
    }
}