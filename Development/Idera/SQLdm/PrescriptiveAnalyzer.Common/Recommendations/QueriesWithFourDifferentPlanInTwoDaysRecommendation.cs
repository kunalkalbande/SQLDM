using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// // SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-Q50 -Adding new recommendation class
    /// </summary>
    [Serializable]
    public class QueriesWithFourDifferentPlanInTwoDaysRecommendation : QueryRecommendation
    {
        public string Database { get; private set; }
        private AffectedBatches _ab = null;
        public QueriesWithFourDifferentPlanInTwoDaysRecommendation(AffectedBatches ab, string db)
            : base(RecommendationType.QueriesWithFourDifferentPlanInTwoDays, ab, db)
        {
            Database = db;
            _ab = ab;
        }

        public QueriesWithFourDifferentPlanInTwoDaysRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.QueriesWithFourDifferentPlanInTwoDays, recProp)
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
