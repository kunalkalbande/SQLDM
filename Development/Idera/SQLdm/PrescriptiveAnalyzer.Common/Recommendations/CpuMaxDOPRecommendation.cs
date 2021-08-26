using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class CpuMaxDOPRecommendation : CpuRecommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider
    {
        public int SuggestedMaxDOP { get; private set; }
        public UInt32 MaxDOP { get; private set; }
        public int Cores { get; private set; }
        public CpuMaxDOPRecommendation(int suggestedMaxDOP, UInt32 maxDop, int cpu)
            : base(RecommendationType.CpuMaxDOP)
        {
            SuggestedMaxDOP = suggestedMaxDOP;
            MaxDOP = maxDop;
            Cores = cpu;
        }

        public CpuMaxDOPRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.CpuMaxDOP, recProp)
        {
            SuggestedMaxDOP = recProp.GetInt("SuggestedMaxDOP");
            MaxDOP = recProp.GetUInt32("MaxDOP");
            Cores = recProp.GetInt("Cores");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("SuggestedMaxDOP", SuggestedMaxDOP.ToString());
            prop.Add("MaxDOP", MaxDOP.ToString());
            prop.Add("Cores", Cores.ToString());
            return prop;
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new ConfigureMaxDOPScriptGenerator(SuggestedMaxDOP, MaxDOP);
        }

        public bool IsScriptRunnable { get { return true; } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new ConfigureMaxDOPScriptGenerator(SuggestedMaxDOP, MaxDOP);
        }

        public bool IsUndoScriptRunnable { get { return true; } }

        //-----------------------------------------------------------------------------------
        //  Per Brett (PR DR376):
        //
        //  Low: if MaxDOP < 9 & MaxDOP< ¾ of CPU cores available to SQL Server
        //  Medium: if MaxDOP = 0 or MaxDOP > 8 or MaxDOP> ¾ of CPU cores available to SQL Server
        //
        public override int AdjustImpactFactor(int i)
        {
            if ((MaxDOP < 9) && (MaxDOP < (Cores * .75))) return (LOW_IMPACT);
            return (LOW_IMPACT + 1);
        }

    }
}
