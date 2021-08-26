using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    public class ServerConfigurationScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        public readonly string Configuration;
        public readonly long DefaultValue;

        public ServerConfigurationScriptGenerator(string configuration, long defaultValue)
        {
            Configuration = configuration;
            DefaultValue = defaultValue;
        }

        #region IScriptGenerator Members

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            create.AppendLine(
                FormatHelper.Format(
                    ApplicationHelper.GetEmbededResource(GetType().Assembly,
                                                         "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.ServerConfiguration.sql"),
                    this));
            return (create.ToString());
        }

        #endregion

        #region IUndoScriptGenerator Members

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            return GetTSqlFix(connectionInfo);
        }

        #endregion

    }
}
