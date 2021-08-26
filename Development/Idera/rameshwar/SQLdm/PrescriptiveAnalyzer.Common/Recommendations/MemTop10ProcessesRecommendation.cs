using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class MemTop10ProcessesRecommendation : MemRecommendation
    {
        public ICollection<string> Processes { get; private set; }
        public double PagesPerSec { get; private set; }
        public MemTop10ProcessesRecommendation(double pagesPerSec, IEnumerable<string> processes)
            : base(RecommendationType.MemTop10Processes)
        {
            PagesPerSec = Math.Round(pagesPerSec, 2);
            Processes = new List<string>(processes);
        }


        public MemTop10ProcessesRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.MemTop10Processes, recProp)
        {
            Processes = recProp.GetListOfStrings("Processes");
            PagesPerSec = recProp.GetDouble("PagesPerSec");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("PagesPerSec", PagesPerSec.ToString());
            prop.Add("Processes", RecommendationProperties.GetXml<List<string>>((List<string>)Processes));        
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if (PagesPerSec > 35) return (HIGH_IMPACT);
            return (base.AdjustImpactFactor(i));
        }
    }
}
