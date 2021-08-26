using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    public class SetNoCountOnScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        public SetNoCountOnScriptGenerator() { }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            return (ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.SetNoCountOnServerProperty.sql"));
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            return (ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.SetNoCountOnServerPropertyUndo.sql"));
        }
    }
}
