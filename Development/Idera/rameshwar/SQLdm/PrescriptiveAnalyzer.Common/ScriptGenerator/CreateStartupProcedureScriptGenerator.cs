using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    class CreateStartupProcedureScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        public CreateStartupProcedureScriptGenerator()
        {
        }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            return ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.CreateStartupProcedure.sql");
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            return ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.CreateStartupProcedureUndo.sql");
        }
    }
}
