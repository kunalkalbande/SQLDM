using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class MemPerfOfServicesRecommendation : MemRecommendation
    {
        public bool ProductionServer { get; private set; }
        public MemPerfOfServicesRecommendation(bool productionServer)
            : base(RecommendationType.MemPerfOfServices)
        {
            ProductionServer = productionServer;
        }

        public MemPerfOfServicesRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.MemPerfOfServices, recProp)
        {
            ProductionServer = recProp.GetBool("ProductionServer");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("ProductionServer", ProductionServer.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if (ProductionServer) return (LOW_IMPACT + 1);
            return (base.AdjustImpactFactor(i));
        }
    }
}
