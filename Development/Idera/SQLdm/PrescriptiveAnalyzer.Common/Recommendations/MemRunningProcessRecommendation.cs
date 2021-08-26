using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class MemRunningProcessRecommendation : MemRecommendation
    {
        public ICollection<string> RunningProcesses { get; private set; }
        public MemRunningProcessRecommendation(IEnumerable<string> processes)
            : base(RecommendationType.MemRunningProcess)
        {
            RunningProcesses = new List<string>(processes);
        }

        public MemRunningProcessRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.MemRunningProcess, recProp)
        {
            RunningProcesses = recProp.GetListOfStrings("RunningProcesses");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("RunningProcesses", RecommendationProperties.GetXml<List<string>>((List<string>)RunningProcesses));        
            return prop;
        }
    }
}
