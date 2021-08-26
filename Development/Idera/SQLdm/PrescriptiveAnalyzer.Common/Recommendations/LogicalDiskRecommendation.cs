using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class LogicalDiskRecommendation : Recommendation
    {
        public string DiskName { get; internal set; }
        public LogicalDiskRecommendation(RecommendationType type)
            : base(type)
        {
        }
        public LogicalDiskRecommendation(RecommendationType type, RecommendationProperties recProp)
            : base(type, recProp)
        {
            DiskName = recProp.GetString("DiskName");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("DiskName", DiskName.ToString());
            return prop;
        }
    }
}
