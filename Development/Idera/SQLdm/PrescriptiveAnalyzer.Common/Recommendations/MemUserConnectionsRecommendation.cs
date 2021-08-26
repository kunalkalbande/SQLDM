using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Idera.SQLdoctor.Common.Helpers;
//using Idera.SQLdoctor.Common.Services;
//using Idera.SQLdoctor.Common.ScriptGenerator;
//using Idera.SQLdoctor.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using System.Data.SqlClient;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class MemUserConnectionsRecommendation : MemRecommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, IMessageGenerator, IUndoMessageGenerator
    {
        public UInt64 UserConnections { get; private set; }
        public string MemoryEstimate { get { return (FormatHelper.FormatBytes((100 * 1024) * UserConnections)); } }
        public MemUserConnectionsRecommendation(UInt64 userConnections)
            : base(RecommendationType.MemUserConnections)
        {
            UserConnections = userConnections;
        }

        public MemUserConnectionsRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.MemUserConnections, recProp)
        {
            UserConnections = recProp.GetUInt64("UserConnections");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("UserConnections", UserConnections.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            if (UserConnections > 300) { return (LOW_IMPACT + 1); }
            return (base.AdjustImpactFactor(i));
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new ConfigureUserConnectionsScriptGenerator(UserConnections);
        }

        public bool IsScriptRunnable { get { return true; } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new ConfigureUserConnectionsScriptGenerator(UserConnections);
        }

        public bool IsUndoScriptRunnable { get { return true; } }

        new public List<string> GetUndoMessages(RecommendationOptimizationStatus res, SqlConnection connection)
        {
            List<string> messages = new List<string>();
            messages.AddRange(base.GetUndoMessages(res, connection));
            messages.Add(Properties.Resources.RecommendationRequiresRestartMessageUndo);
            return messages;
        }

        new public List<string> GetMessages(RecommendationOptimizationStatus res, SqlConnection connection)
        {
            List<string> messages = new List<string>();
            messages.AddRange(base.GetMessages(res, connection));
            messages.Add(Properties.Resources.RecommendationRequiresRestartMessage);
            return messages;
        }
    }
}
