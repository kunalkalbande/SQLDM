using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// SQLDm 10.0 Srishti Purohit- New Recommendations (SDR-M33)
    /// </summary>
    [Serializable]
    public class BufferPoolExtensionHighIORecommendation : Recommendation
    {
        public BufferPoolExtensionHighIORecommendation()
            : base(RecommendationType.BufferPoolExtensionHighIO)
        {
        }

    }
}
