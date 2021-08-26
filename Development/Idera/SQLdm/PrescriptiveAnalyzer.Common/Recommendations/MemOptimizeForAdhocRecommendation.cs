using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.Helpers;
//using Idera.SQLdoctor.Common.ScriptGenerator;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class MemOptimizeForAdhocRecommendation : MemRecommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider
    {
        public UInt64 AdhocCacheSize { get; private set; }
        public UInt64 HighBytes { get; private set; }
        public string AdhocCacheSizeFormatted { get { return (FormatHelper.FormatBytes(AdhocCacheSize)); } }

        public MemOptimizeForAdhocRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.MemOptimizeForAdhoc, recProp)
        {
            AdhocCacheSize = recProp.GetUInt64("AdhocCacheSize");
            HighBytes = recProp.GetUInt64("HighBytes");
        }
        
        public MemOptimizeForAdhocRecommendation(UInt64 adhocCacheSize, UInt64 highBytes)
            : base(RecommendationType.MemOptimizeForAdhoc)
        {
            AdhocCacheSize = adhocCacheSize;
            HighBytes = highBytes;
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("AdhocCacheSize", AdhocCacheSize.ToString());
            prop.Add("HighBytes", HighBytes.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if (AdhocCacheSize > 200000000) return (HIGH_IMPACT);
            return (base.AdjustImpactFactor(i));
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new ConfigureOptimizeAdhocScriptGenerator();
        }

        public bool IsScriptRunnable { get { return true; } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new ConfigureOptimizeAdhocScriptGenerator();
        }

        public bool IsUndoScriptRunnable { get { return true; } }
    }
}
