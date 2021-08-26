using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class MemIncreaseDefaultFillFactorRecommendation : MemRecommendation
    {
        public UInt32 FillFactor { get; private set; }
        public UInt64 PageSplits { get; private set; }
        public MemIncreaseDefaultFillFactorRecommendation(UInt32 fillFactor, UInt64 pageSplits)
            : base(RecommendationType.MemIncreaseDefaultFillFactor)
        {
            PageSplits = pageSplits;
            FillFactor = fillFactor;
        }

        public MemIncreaseDefaultFillFactorRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.MemIncreaseDefaultFillFactor, recProp)
        {
            FillFactor = recProp.GetUInt32("FillFactor");
            PageSplits = recProp.GetUInt64("PageSplits");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("FillFactor", FillFactor.ToString());
            prop.Add("PageSplits", PageSplits.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if (FillFactor <= 75) { return (HIGH_IMPACT); }
            return (base.AdjustImpactFactor(i));
        }
    }
}
