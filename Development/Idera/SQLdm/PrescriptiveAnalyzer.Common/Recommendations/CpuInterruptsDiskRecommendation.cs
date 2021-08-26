using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class CpuInterruptsDiskRecommendation : CpuInterruptsRecommendation
    {
        public ICollection<string> Disks { get; private set; }
        public CpuInterruptsDiskRecommendation(UInt64 activity, UInt32 interruptsPerSec, double interruptsTime, IEnumerable<string> disks)
            : base(RecommendationType.CpuInterruptsDisk, activity, interruptsPerSec, interruptsTime)
        {
            if (null != disks) Disks = new List<string>(disks);
        }

        public CpuInterruptsDiskRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.CpuInterruptsDisk, recProp)
        {
            Disks = recProp.GetListOfStrings("Disks");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Disks", RecommendationProperties.GetXml<List<string>>((List<string>)Disks));
            return prop;
        }
    }
}
