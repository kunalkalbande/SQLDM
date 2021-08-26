using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.ScriptGenerator;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DeadlockTraceFlagsRecommendation : Recommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider
    {
        public DeadlockTraceFlagsRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DeadlockTraceFlags, recProp)
        {
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new CreateStartupProcedureScriptGenerator();
        }

        public bool IsScriptRunnable { get { return true; } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new CreateStartupProcedureScriptGenerator();
        }

        public bool IsUndoScriptRunnable { get { return true; } }
    }
}
