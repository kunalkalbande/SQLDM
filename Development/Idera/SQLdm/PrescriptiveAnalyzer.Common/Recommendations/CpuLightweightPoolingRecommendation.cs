using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.Services;

namespace Idera.SQLdoctor.Common.Recommendations
{ 
    [Serializable]
    public class CpuLightweightPoolingRecommendation : CpuRecommendation
    {
        public UInt32 ContextSwitches { get; private set; }
        public int PrivilegedTime { get; private set; }
        public UInt32 ThreadCount { get; private set; }

        public CpuLightweightPoolingRecommendation(UInt32 contextSwitches, int privilegedTime, UInt32 threadCount)
            : base(RecommendationType.CpuLightweightPooling)
        {
            ContextSwitches = contextSwitches;
            PrivilegedTime = privilegedTime;
            ThreadCount = threadCount;
        }
        public override int AdjustImpactFactor(int i)
        {
            if (ThreadCount > 500)
            {
                return (LOW_IMPACT + 1);
            }
            return (base.AdjustImpactFactor(i));
        }
    }
}
