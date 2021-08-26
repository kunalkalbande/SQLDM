using System;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    public class DatabaseCompatibilityScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        private string _database;

        public DatabaseCompatibilityScriptGenerator(string database, string targetCompatibility, string observedCompatibility)
        {
            Database = database;
            TargetCompatibility = targetCompatibility;
            ObservedCompatibility = observedCompatibility;
        }

        public string Database
        {
            get
            {
                if (String.IsNullOrEmpty(_database))
                    return null;
                return _database.Replace("'", "''");
            }
            private set { _database = value; }
        }
        
        public string TargetCompatibility { get; set; }
        public string ObservedCompatibility { get; set; }


        #region IScriptGenerator Members

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            create.AppendLine(
                FormatHelper.Format(
                    ApplicationHelper.GetEmbededResource(GetType().Assembly,
                                                         "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.SetDatabaseCompatibility.sql"),
                    this));
            return (create.ToString());
        }

        #endregion

        #region IUndoScriptGenerator Members

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            create.AppendLine(
                FormatHelper.Format(
                    ApplicationHelper.GetEmbededResource(GetType().Assembly,
                                                         "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.SetDatabaseCompatibilityUndo.sql"),
                    this));
            return (create.ToString());
        }

        #endregion
    }
}