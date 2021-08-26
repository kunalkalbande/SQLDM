using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class SeManageVolumeNameRecommendation : Recommendation
    {
        public string SQLServerServiceAccount { get;  set; }
        public SeManageVolumeNameRecommendation() : base(RecommendationType.DiskNeedSeManageVolumeName)
        {
        }

        public SeManageVolumeNameRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DiskNeedSeManageVolumeName, recProp)
        {
            SQLServerServiceAccount = recProp.GetString("SQLServerServiceAccount");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            if (prop != null)
            {
                if (SQLServerServiceAccount == null) SQLServerServiceAccount = string.Empty;
                prop.Add("SQLServerServiceAccount", SQLServerServiceAccount);
            }
            return prop;
        }
    }
}
