using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.Services;
using Idera.SQLdoctor.Common.ScriptGenerator;

namespace Idera.SQLdoctor.Common.Recommendations
{ 
    [Serializable]
    public class MemPerfOfCacheRecommendation : MemRecommendation
    {
        public bool ProductionServer { get; private set; }
        public MemPerfOfCacheRecommendation(bool productionServer)
            : base(RecommendationType.MemPerfOfCache)
        {
            ProductionServer = productionServer;
        }
        public override int AdjustImpactFactor(int i)
        {
            if (ProductionServer) return (LOW_IMPACT + 1);
            return (base.AdjustImpactFactor(i));
        }
    }
}
