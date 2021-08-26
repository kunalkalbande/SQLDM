using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.Services;

namespace Idera.SQLdoctor.Common.Recommendations
{ 
    [Serializable]
    public class CpuThemesRecommendation : CpuRecommendation
    {
        public CpuThemesRecommendation()
            : base(RecommendationType.CpuThemes)
        {
        }
    }
}
