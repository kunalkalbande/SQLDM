using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// SQLDm 10.0 Srishti Purohit- New Recommendations (SDR-Q45)
    /// </summary>
    [Serializable]
    public class BothNewAndOldCardinalityEstimatorInUseRecommendation : Recommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, ITransactionLessScript
    {
        public List<DbWithCompatibility> DbWithCompatibilityLowerThan120;

        public BothNewAndOldCardinalityEstimatorInUseRecommendation(Dictionary<string, int> dbWithCompatibilityLowerThan120)
            : base(RecommendationType.BothNewAndOldCardinalityEstimatorInUse)
        {
            DbWithCompatibilityLowerThan120 = new List<DbWithCompatibility>();
            DbWithCompatibility compatibilityObj;
            foreach (KeyValuePair<string, int> item in dbWithCompatibilityLowerThan120)
            {
                compatibilityObj = new DbWithCompatibility();
                compatibilityObj.DbName = item.Key;
                compatibilityObj.Compatibility = item.Value;
                DbWithCompatibilityLowerThan120.Add(compatibilityObj);
            }
        }
        public BothNewAndOldCardinalityEstimatorInUseRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.BothNewAndOldCardinalityEstimatorInUse, recProp)
        {
            DbWithCompatibilityLowerThan120 = recProp.GetDbWithCompatibility("DbWithCompatibility");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("DbWithCompatibility", RecommendationProperties.GetXml<List<DbWithCompatibility>>(DbWithCompatibilityLowerThan120));
            return prop;
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new BothNewAndOldCardinalityEstimatorInUseScriptGenerator(DbWithCompatibilityLowerThan120);
        }

        public bool IsScriptRunnable { get { return (true); } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new BothNewAndOldCardinalityEstimatorInUseScriptGenerator(DbWithCompatibilityLowerThan120);
        }

        public bool IsUndoScriptRunnable { get { return (true); } }

        /// <summary>
        /// Implemented ITransactionLessScript to support opti/undo scripts to run without transactions
        /// </summary>
        public bool IsScriptTransactionLess
        {
            get { return true; }
        }

        public bool IsUndoScriptTransactionLess
        {
            get { return true; }
        }

    }
}
