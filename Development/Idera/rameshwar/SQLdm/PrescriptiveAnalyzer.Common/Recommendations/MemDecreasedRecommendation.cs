using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
//using Idera.SQLdoctor.Common.Helpers;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class MemDecreasedRecommendation : MemRecommendation
    {
        public UInt64 Current { get; private set; }
        public UInt64 Previous { get; private set; }
        public string DecreasedBy { get { return (FormatHelper.FormatBytes(Previous - Current)); } }
        public MemDecreasedRecommendation(UInt64 current, UInt64 previous)
            : base(RecommendationType.MemDecreased)
        {
            Current = current;
            Previous = previous;
        }

        public MemDecreasedRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.MemDecreased, recProp)
        {
            Current = recProp.GetUInt64("Current");
            Previous = recProp.GetUInt64("Previous");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Current", Current.ToString());
            prop.Add("Previous", Previous.ToString());
            return prop;
        }
    }
}
