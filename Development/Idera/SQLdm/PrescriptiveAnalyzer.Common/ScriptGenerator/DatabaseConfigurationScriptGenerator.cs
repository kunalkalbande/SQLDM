using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    public class DatabaseConfigurationScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        private string databaseName;
        public readonly string ConfigurationName;
        public readonly string ConfigurationValue;

        public string Database
        {
            get
            {
                if (String.IsNullOrEmpty(databaseName))
                    return null;
                return SQLHelper.CreateBracketedString(databaseName);
            }
            private set { databaseName = value; }
        }

        public DatabaseConfigurationScriptGenerator(string databaseName, string configurationName, string configurationValue)
        {
            this.databaseName = databaseName;
            ConfigurationName = configurationName;
            ConfigurationValue = configurationValue;
        }


        #region IScriptGenerator Members

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            create.AppendLine(
                FormatHelper.Format(
                    ApplicationHelper.GetEmbededResource(GetType().Assembly,
                                                         "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.DatabaseConfiguration.sql"),
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
