using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
//using Idera.SQLdoctor.Common.Helpers;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class MemIndexCreationRecommendation : MemRecommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider
    {
        public UInt64 MemoryInKB { get; private set; }
        public MemIndexCreationRecommendation(UInt64 memoryInKB)
            : base(RecommendationType.MemIndexCreation)
        {
            MemoryInKB = memoryInKB;
        }

        public MemIndexCreationRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.MemIndexCreation, recProp)
        {
            MemoryInKB = recProp.GetUInt64("MemoryInKB");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("MemoryInKB", MemoryInKB.ToString());
            return prop;
        }


        public override int AdjustImpactFactor(int i)
        {
            if (MemoryInKB > (UInt16.MaxValue + 1)) { return (LOW_IMPACT + 1); }
            return (base.AdjustImpactFactor(i));
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new ConfigureIndexCreateMemoryScriptGenerator(MemoryInKB);
        }

        public bool IsScriptRunnable { get { return true; } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new ConfigureIndexCreateMemoryScriptGenerator(MemoryInKB);
        }

        public bool IsUndoScriptRunnable { get { return true; } }
    }
}
