using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class CpuDisableParallelismRecommendation : CpuRecommendation, IProvideAffectedBatches
    {
        public UInt32 MaxDOP { get; private set; }
        public int Cores { get; private set; }
        private AffectedBatches _affectedBatches = null;

        public CpuDisableParallelismRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.CpuDisableParallelism, recProp)
        {
            MaxDOP = recProp.GetUInt32("MaxDOP");
            Cores = recProp.GetInt("Cores");
            _affectedBatches = recProp.GetAffectedBatches("_affectedBatches");
        }
        public CpuDisableParallelismRecommendation(UInt32 maxDop, int cpu, AffectedBatches batches)
            : base(RecommendationType.CpuDisableParallelism)
        {
            MaxDOP = maxDop;
            Cores = cpu;
            _affectedBatches = batches;
        }


        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("MaxDOP", MaxDOP.ToString());
            prop.Add("Cores", Cores.ToString());
            prop.Add("_affectedBatches", RecommendationProperties.GetXml<AffectedBatches>(_affectedBatches));
            return prop;
        }

        public AffectedBatches GetAffectedBatches()
        {
            return (_affectedBatches);
        }

    }
}
