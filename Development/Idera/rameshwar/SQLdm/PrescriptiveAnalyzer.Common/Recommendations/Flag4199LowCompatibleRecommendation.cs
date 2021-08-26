using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// SQLdm10.0 -Srishti Purohit -  New Recommendations
    /// Trace flag 4199 is enabled globally for SQL Server 2016 and there are databases running in compatibility level 120 or lower
    /// </summary>
    [Serializable]
    public class Flag4199LowCompatibleRecommendation : Recommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, ITransactionLessScript
    {
        public List<DbWithCompatibility> DbWithCompatibilityLowerThan130;

        public Flag4199LowCompatibleRecommendation(Dictionary<string, int> dbWithCompatibilityLowerThan130)
            : base(RecommendationType.Flag4199LowCompatible)
        {
            DbWithCompatibilityLowerThan130 = new List<DbWithCompatibility>();
            DbWithCompatibility compatibilityObj;
            foreach (KeyValuePair<string, int> item in dbWithCompatibilityLowerThan130)
            {
                compatibilityObj = new DbWithCompatibility();
                compatibilityObj.DbName = item.Key;
                compatibilityObj.Compatibility = item.Value;
                DbWithCompatibilityLowerThan130.Add(compatibilityObj);
            }
        }
        public Flag4199LowCompatibleRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.Flag4199LowCompatible, recProp)
        {
            DbWithCompatibilityLowerThan130 = recProp.GetDbWithCompatibility("DbWithCompatibility");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("DbWithCompatibility", RecommendationProperties.GetXml<List<DbWithCompatibility>>(DbWithCompatibilityLowerThan130));
           
            return prop;
        }
        public IScriptGenerator GetScriptGenerator()
        {
            return new Flag4199LowCompatibleScriptGenerator(DbWithCompatibilityLowerThan130);
        }

        public bool IsScriptRunnable { get { return (true); } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new Flag4199LowCompatibleScriptGenerator(DbWithCompatibilityLowerThan130);
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
