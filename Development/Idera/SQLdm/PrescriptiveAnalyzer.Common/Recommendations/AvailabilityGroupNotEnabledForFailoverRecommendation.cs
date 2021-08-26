using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// SQLDm 10.0 Srishti Purohit- New Recommendations (SDR-R8)
    /// </summary>
    [Serializable]
    public class AvailabilityGroupNotEnabledForFailoverRecommendation : Recommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, ITransactionLessScript
    {
        public string AvailabilityGroupName { get; set; }
        public AvailabilityGroupNotEnabledForFailoverRecommendation(string name)
            : base(RecommendationType.AvailabilityGroupNotEnabledForFailover)
        {
            AvailabilityGroupName = name;
        }
        public AvailabilityGroupNotEnabledForFailoverRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.AvailabilityGroupNotEnabledForFailover, recProp)
        {
            AvailabilityGroupName = recProp.GetString("AvailabilityGroupName");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("AvailabilityGroupName", AvailabilityGroupName.ToString());
            return prop;
        }
        #region OptimizeUndo
        public IScriptGenerator GetScriptGenerator()
        {
            return new AvailabilityGroupDBFailoverScriptGenerator(AvailabilityGroupName);
        }

        public bool IsScriptRunnable { get { return (true); } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new AvailabilityGroupDBFailoverScriptGenerator(AvailabilityGroupName);
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
        #endregion

    }
}
