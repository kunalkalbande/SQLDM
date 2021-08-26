using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class MemPagingRecommendation : MemRecommendation
    {
        public double PagesPerSec { get; private set; }
        public MemPagingRecommendation(double pagesPerSec)
            : base(RecommendationType.MemPaging)
        {
            PagesPerSec = Math.Round(pagesPerSec);
        }

        public MemPagingRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.MemPaging, recProp)
        {
            PagesPerSec = recProp.GetDouble("PagesPerSec");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("PagesPerSec", PagesPerSec.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if (PagesPerSec > 50) { return (HIGH_IMPACT); }
            return (base.AdjustImpactFactor(i));
        }
    }
}
