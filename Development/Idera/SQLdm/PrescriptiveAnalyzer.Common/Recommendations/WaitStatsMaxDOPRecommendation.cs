using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class WaitStatsMaxDOPRecommendation : WaitStatsRecommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider
    {
        public UInt32 RecommendedMaxDOP { get; private set; }
        public UInt32 CurrentMaxDOP { get; private set; }
        public WaitStatsMaxDOPRecommendation(AffectedBatches ab, UInt32 currentMaxDOP, UInt32 recommendedMaxDOP)
            : base(ab, RecommendationType.WaitStatsMaxDOP)
        {
            RecommendedMaxDOP = recommendedMaxDOP;
            CurrentMaxDOP = currentMaxDOP;
        }

        public WaitStatsMaxDOPRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.WaitStatsMaxDOP, recProp)
        {
            RecommendedMaxDOP = recProp.GetUInt32("RecommendedMaxDOP");
            CurrentMaxDOP = recProp.GetUInt32("CurrentMaxDOP");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("RecommendedMaxDOP", RecommendedMaxDOP.ToString());
            prop.Add("CurrentMaxDOP", CurrentMaxDOP.ToString());
            return prop;
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new ConfigureMaxDOPScriptGenerator((int)RecommendedMaxDOP, CurrentMaxDOP);
        }

        public bool IsScriptRunnable { get { return true; } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new ConfigureMaxDOPScriptGenerator((int)RecommendedMaxDOP, CurrentMaxDOP);
        }

        public bool IsUndoScriptRunnable { get { return true; } }
    }
}
