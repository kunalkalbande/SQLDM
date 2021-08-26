using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// SQLdm10.0 -Srishti Purohit - New Recommendations (SDR-Q44)
    /// </summary>
    [Serializable]
    public class NewCardinalityEstimatorNotBeingUsedRecommendation : Recommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, ITransactionLessScript
    {
        public List<DbWithCompatibility> DbWithCompatibilityLowerThan120;

        public NewCardinalityEstimatorNotBeingUsedRecommendation(Dictionary<string, int> dbWithCompatibilityLowerThan120)
            : base(RecommendationType.NewCardinalityEstimatorNotBeingUsed)
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
        public NewCardinalityEstimatorNotBeingUsedRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.NewCardinalityEstimatorNotBeingUsed, recProp)
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
            return new NewCardinalityEstimatorNotBeingUsedScriptGenerator(DbWithCompatibilityLowerThan120);
        }

        public bool IsScriptRunnable { get { return (true); } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new NewCardinalityEstimatorNotBeingUsedScriptGenerator(DbWithCompatibilityLowerThan120);
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
