using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    /// <summary>
    /// //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-I24
    /// </summary>
    public class HighModificationsSinceLastStatUpdateScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        public HighModificationsSinceLastStatUpdateScriptGenerator() { }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            return (ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.EnableTrace2371.sql"));
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            return (ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.DisableTrace2371.sql"));
        }
    }
}
