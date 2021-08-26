using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    public class ConfigureUserConnectionsScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        public UInt64 ObservedUserConnections { get; private set; }
        public ConfigureUserConnectionsScriptGenerator(UInt64 observedUserConnections)
        {
            ObservedUserConnections = observedUserConnections;
        }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            return (FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.ConfigureUserConnections.sql"), this));
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            return (FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.ConfigureUserConnectionsUndo.sql"), this));
        }
    }
}
