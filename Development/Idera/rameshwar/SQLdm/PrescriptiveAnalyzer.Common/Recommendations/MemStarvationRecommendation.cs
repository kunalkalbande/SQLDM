using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class MemStarvationRecommendation : MemRecommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider
    {
        public UInt64 SuggestedMaxServerMemory { get; private set; }
        public UInt64 MaxServerMemory { get; private set; }

        public MemStarvationRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.MemStarvation, recProp)
        {
            SuggestedMaxServerMemory = recProp.GetUInt64("SuggestedMaxServerMemory");
            MaxServerMemory = recProp.GetUInt64("MaxServerMemory");
        }

        public MemStarvationRecommendation(double suggestedMaxMemGB, double maxServerMem)
            : base(RecommendationType.MemStarvation)
        {
            SuggestedMaxServerMemory = (UInt64)(suggestedMaxMemGB * 1024);
            MaxServerMemory = (UInt64)(maxServerMem * 1024);
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("SuggestedMaxServerMemory", SuggestedMaxServerMemory.ToString());
            prop.Add("MaxServerMemory", MaxServerMemory.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if ((SuggestedMaxServerMemory - MaxServerMemory) > 1024) { return (HIGH_IMPACT); }
            return (base.AdjustImpactFactor(i));
        }

        public IScriptGenerator GetScriptGenerator()
        {
            ConfigureMaxServerMemoryGenerator generator = new ConfigureMaxServerMemoryGenerator();
            generator.RecommendedSizeMB = SuggestedMaxServerMemory;
            return generator;
        }

        public bool IsScriptRunnable
        {
            get { return true; }
        }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            ConfigureMaxServerMemoryGenerator generator = new ConfigureMaxServerMemoryGenerator();
            generator.ObservedSizeMB = MaxServerMemory;
            return generator;
        }

        public bool IsUndoScriptRunnable
        {
            get { return true; }
        }
    }
}
