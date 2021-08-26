using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// // SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-Q46 -Adding new recommendation class
    /// </summary>
    [Serializable]
    public class Top10QueriesWithLongestAverageExecutionTimeRecommendation : QueryRecommendation
    {
        public string Database { get; private set; }
        private AffectedBatches _ab = null;
        public Top10QueriesWithLongestAverageExecutionTimeRecommendation(AffectedBatches ab, string db)
            : base(RecommendationType.Top10QueriesWithLongestAverageExecutionTime, ab, db)
        {
            Database = db;
            _ab = ab;
        }
        public Top10QueriesWithLongestAverageExecutionTimeRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.Top10QueriesWithLongestAverageExecutionTime, recProp)
        {
            Database = recProp.GetString("dbname");
            _ab = recProp.GetAffectedBatches("_affectedBatches");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("dbname", Database.ToString());
            prop.Add("_affectedBatches", RecommendationProperties.GetXml<AffectedBatches>(_ab));
            return prop;
        }

    }
}
