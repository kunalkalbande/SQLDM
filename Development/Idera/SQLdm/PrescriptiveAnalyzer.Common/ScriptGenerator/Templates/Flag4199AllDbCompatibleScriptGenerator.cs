using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates
{
    /// <summary>
    /// //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-Q38
    /// </summary>
    public class Flag4199AllDbCompatibleScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        public Flag4199AllDbCompatibleScriptGenerator() { }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            return (ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.DisableTrace4199.sql"));
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            return (ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.EnableTrace4199.sql"));
        }
    }
}
