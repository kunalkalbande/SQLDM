using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
//using Idera.SQLdoctor.Common.Helpers;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.ScriptGenerator;
//using Idera.SQLdoctor.Common.ScriptGenerator.Templates;


namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class MemMaxServerMemoryRecommendation : MemRecommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider
    {
        private UInt64 _maxServerMemoryBytes = 0;
        private UInt64 _suggestedMaxServerMemoryBytes = 0;
        public double MaxServerMemory { get; private set; }
        public double SuggestedMaxServerMemory { get; private set; }
        public string MaxServerMemoryFormatted { get { return (string.Format("{0} MB", Math.Round(_maxServerMemoryBytes / (1024.0 * 1024.0), 0))); } }
        public string SuggestedMaxServerMemoryFormatted { get { return (string.Format("{0} MB", Math.Round(_suggestedMaxServerMemoryBytes / (1024.0 * 1024.0), 0))); } }

        public MemMaxServerMemoryRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.MemMaxServerMemory, recProp)
        {
            MaxServerMemory = recProp.GetDouble("MaxServerMemory");
            SuggestedMaxServerMemory = recProp.GetDouble("SuggestedMaxServerMemory");
            try
            {
                _maxServerMemoryBytes = Convert.ToUInt64(MaxServerMemory * 1024 * 1024 * 1024);
                _suggestedMaxServerMemoryBytes = Convert.ToUInt64(SuggestedMaxServerMemory * 1024 * 1024 * 1024);
            }
            catch { }
        }
        
        public MemMaxServerMemoryRecommendation(double suggestedMax, double currentMax)
            : base(RecommendationType.MemMaxServerMemory)
        {
            MaxServerMemory = currentMax;
            SuggestedMaxServerMemory = suggestedMax;
            try
            {
                _maxServerMemoryBytes = Convert.ToUInt64(currentMax * 1024 * 1024 * 1024);
                _suggestedMaxServerMemoryBytes = Convert.ToUInt64(suggestedMax * 1024 * 1024 * 1024);
            }
            catch { }
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("MaxServerMemory", MaxServerMemory.ToString());
            prop.Add("SuggestedMaxServerMemory", SuggestedMaxServerMemory.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if ((SuggestedMaxServerMemory - MaxServerMemory) > 1) { return (HIGH_IMPACT); }
            return (base.AdjustImpactFactor(i));
        }

        public IScriptGenerator GetScriptGenerator()
        {
            ConfigureMaxServerMemoryGenerator generator = new ConfigureMaxServerMemoryGenerator();
            generator.RecommendedSizeMB = (ulong)(SuggestedMaxServerMemory * 1024);     // convert SuggestedMaxServerMemory(GB) to MB
            return generator;
        }

        public bool IsScriptRunnable
        {
            get { return true; }
        }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            ConfigureMaxServerMemoryGenerator generator = new ConfigureMaxServerMemoryGenerator();
            generator.ObservedSizeMB = (ulong)(MaxServerMemory * 1024);     // convert MaxServerMemory(GB) to MB
            return generator;
        }

        public bool IsUndoScriptRunnable
        {
            get { return true; }
        }
    }
}
