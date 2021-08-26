using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class WaitStatsParallelismThresholdRecommendation : WaitStatsRecommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider
    {
        public UInt32 RecommendedThreshold { get; private set; }
        public UInt32 CurrentThreshold { get; private set; }
        public WaitStatsParallelismThresholdRecommendation(AffectedBatches ab, UInt32 currentThreshold, UInt32 recommendedThreshold)
            : base(ab, RecommendationType.WaitStatsParallelismThreshold)
        {
            RecommendedThreshold = recommendedThreshold;
            CurrentThreshold = currentThreshold;
        }
        public WaitStatsParallelismThresholdRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.WaitStatsParallelismThreshold, recProp)
        {
            RecommendedThreshold = recProp.GetUInt32("RecommendedThreshold");
            CurrentThreshold = recProp.GetUInt32("CurrentThreshold");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("RecommendedThreshold", RecommendedThreshold.ToString());
            prop.Add("CurrentThreshold", CurrentThreshold.ToString());
            return prop;
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new ConfigureParallelismThresholdScriptGenerator(RecommendedThreshold, CurrentThreshold);
        }

        public bool IsScriptRunnable { get { return true; } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new ConfigureParallelismThresholdScriptGenerator(RecommendedThreshold, CurrentThreshold);
        }

        public bool IsUndoScriptRunnable { get { return true; } }
    }
}
