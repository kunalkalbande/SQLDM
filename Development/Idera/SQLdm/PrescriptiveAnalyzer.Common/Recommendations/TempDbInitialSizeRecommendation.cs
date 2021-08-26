using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class TempDbInitialSizeRecommendation : Recommendation, IProvideDatabase
    {
        public long InitialSizeMB { get;  set; }
        public long CurrentSizeMB { get;  set; }
        public string Database { get { return "tempdb"; } }

        public TempDbInitialSizeRecommendation() : base(RecommendationType.DiskTempDbPresizeSettings)
        {
        }

        public TempDbInitialSizeRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DiskTempDbPresizeSettings, recProp)
        {
            InitialSizeMB = recProp.GetLong("InitialSizeMB");
            CurrentSizeMB = recProp.GetLong("CurrentSizeMB");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("InitialSizeMB", InitialSizeMB.ToString());
            prop.Add("CurrentSizeMB", CurrentSizeMB.ToString());
            return prop;
        }
    }
}
