using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// //SQLDm 10.0 Srishti Purohit - New Recommendations
    /// </summary>
    [Serializable]
    public class QueryRecommendation : Recommendation, IProvideQueryBatches, IProvideDatabase
    {
        private AffectedBatches _ab = null;
        public string Database { get; private set; }

        public QueryRecommendation(RecommendationType type, AffectedBatches ab, string db)
            : base(type)
        {
            _ab = ab;
            Database = db;
        }
        public QueryRecommendation(RecommendationType rt, RecommendationProperties recProp)
            : base(rt, recProp)
        {
            Database = recProp.GetString("dbname");
            _ab = recProp.GetAffectedBatches("_affectedBatches");
        }

        public AffectedBatches GetAffectedBatches()
        {
            return (_ab);
        }
    }
}
