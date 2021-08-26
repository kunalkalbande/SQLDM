using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.Configuration.ServerActions
{
     [Serializable]
    public class PrescriptiveScriptConfiguration : OnDemandConfiguration, IServerActionConfiguration
    {
        private PrescriptiveScriptType scriptType;

        public PrescriptiveScriptType ScriptType
        {
            get { return scriptType; }
            set { scriptType = value; }
        }
        private List<Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation> recommendations;

        public List<Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation> Recommendation
        {
            get { return recommendations; }
            set { recommendations = value; }
        }
       
         public PrescriptiveScriptConfiguration(int monitoredServerId, PrescriptiveScriptType scriptType, List<Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.IRecommendation> recommendations)
             : base(monitoredServerId)
         {
             this.scriptType = scriptType;
             this.recommendations = recommendations;
         }
    }

    [Serializable]
     public enum PrescriptiveScriptType
     {
         Optimize,
         Undo
     }
}
