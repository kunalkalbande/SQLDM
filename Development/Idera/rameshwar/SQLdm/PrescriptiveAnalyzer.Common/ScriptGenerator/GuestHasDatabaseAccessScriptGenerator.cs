using System;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    public class GuestHasDatabaseAccessScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        private string _database;
        public string Database
        {
            get
            {
                if (String.IsNullOrEmpty(_database))
                    return null;
                return SQLHelper.CreateBracketedString(_database);
            }
            private set { _database = value; }
        }

        public GuestHasDatabaseAccessScriptGenerator(string database)
        {
            Database = database;
        }


        #region IScriptGenerator Members

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            create.AppendLine(
                FormatHelper.Format(
                    ApplicationHelper.GetEmbededResource(GetType().Assembly,
                                                         "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.RemoveGuestAccess.sql"),
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
                                                         "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.RemoveGuestAccessUndo.sql"),
                    this));
            return (create.ToString());
        }

        #endregion
    }
}