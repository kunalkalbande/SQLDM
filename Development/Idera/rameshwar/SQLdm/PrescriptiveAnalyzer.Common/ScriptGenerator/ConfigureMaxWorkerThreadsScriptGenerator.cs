using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    public class ConfigureMaxWorkerThreadsScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        public UInt64 CurrentMaxWorkerThreads { get; private set; }
        public UInt64 SuggestedMaxWorkerThreads { get; private set; }

        public ConfigureMaxWorkerThreadsScriptGenerator(UInt64 current, UInt64 suggested)
        {
            CurrentMaxWorkerThreads = current;
            SuggestedMaxWorkerThreads = suggested;
        }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            return (FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.ConfigureMaxWorkerThreads.sql"), this));
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            return (FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.ConfigureMaxWorkerThreadsUndo.sql"), this));
        }
    }
}
