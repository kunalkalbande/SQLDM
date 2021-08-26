using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.Services;
using Idera.SQLdoctor.Common.ScriptGenerator;
using Idera.SQLdoctor.Common.Configuration;

namespace Idera.SQLdoctor.Common.Recommendations
{ 
    [Serializable]
    public class CpuC2AuditRecommendation : CpuRecommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, IMessageGenerator, IUndoMessageGenerator
    {
        public CpuC2AuditRecommendation()
            : base(RecommendationType.CpuC2Audit)
        {
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new ConfigureC2AuditModeScriptGenerator();
        }

        public bool IsScriptRunnable { get { return true; } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new ConfigureC2AuditModeScriptGenerator();
        }

        public bool IsUndoScriptRunnable { get { return true; } }

        new public List<string> GetUndoMessages(RecommendationExecutionStatus res, SqlConnectionInfo connectionInfo)
        {
            List<string> messages = new List<string>();
            messages.AddRange(base.GetUndoMessages(res, connectionInfo));
            messages.Add(Properties.Resources.RecommendationRequiresRestartMessageUndo);
            return messages;
        }

        new public List<string> GetMessages(RecommendationExecutionStatus res, SqlConnectionInfo connectionInfo)
        {
            List<string> messages = new List<string>();
            messages.AddRange(base.GetMessages(res, connectionInfo));
            messages.Add(Properties.Resources.RecommendationRequiresRestartMessage);
            return messages;
        }
    }
}
