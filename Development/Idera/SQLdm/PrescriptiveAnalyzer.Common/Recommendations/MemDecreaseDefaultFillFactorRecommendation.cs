using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class MemDecreaseDefaultFillFactorRecommendation : MemRecommendation
    {
        public UInt32 FillFactor { get; private set; }
        public UInt64 PageSplits { get; private set; }
        public UInt64 PagesAllocated { get; private set; }
        public string PageSplitAllocationPercentage { get { return (string.Format("{0:0.#}", PagesAllocated > 0 ? (PageSplits * 100.0) / PagesAllocated : 100)); } }
        public MemDecreaseDefaultFillFactorRecommendation(UInt32 fillFactor, UInt64 pageSplits, UInt64 pagesAllocated)
            : base(RecommendationType.MemDecreaseDefaultFillFactor)
        {
            PageSplits = pageSplits;
            FillFactor = fillFactor;
            PagesAllocated = pagesAllocated;
        }

        public MemDecreaseDefaultFillFactorRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.MemDecreaseDefaultFillFactor, recProp)
        {
            FillFactor = recProp.GetUInt32("FillFactor");
            PageSplits = recProp.GetUInt64("PageSplits");
            PagesAllocated = recProp.GetUInt64("PagesAllocated");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("FillFactor", FillFactor.ToString());
            prop.Add("PageSplits", PageSplits.ToString());
            prop.Add("PagesAllocated", PagesAllocated.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if (PageSplits > (PagesAllocated * .02)) { return (HIGH_IMPACT); }
            return (base.AdjustImpactFactor(i));
        }
    }
}
