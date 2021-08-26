using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.Services;

namespace Idera.SQLdoctor.Common.Recommendations
{ 
    [Serializable]
    public class CpuMultipleInstancesRecommendation : CpuRecommendation
    {
        public int Count { get; private set; }
        public CpuMultipleInstancesRecommendation(int sqlServerCount)
            : base(RecommendationType.CpuMultipleInstances)
        {
            Count = sqlServerCount;
        }
    }
}
