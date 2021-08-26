using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.Services;
using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdoctor.Common.Recommendations
{ 
    [Serializable]
    public class MemAdjustDefaultFillFactorRecommendation : MemRecommendation
    {
        public UInt32 FillFactor { get; private set; }
        public UInt64 PageSplits { get; private set; }
        public MemAdjustDefaultFillFactorRecommendation(UInt32 fillFactor, UInt64 pageSplits)
            : base(RecommendationType.MemAdjustDefaultFillFactor)
        {
            PageSplits = pageSplits;
            FillFactor = fillFactor;
        }
        public override int AdjustImpactFactor(int i)
        {
            if (FillFactor <= 50) { return (LOW_IMPACT + 1); }
            return (base.AdjustImpactFactor(i));
        }
    }
}
