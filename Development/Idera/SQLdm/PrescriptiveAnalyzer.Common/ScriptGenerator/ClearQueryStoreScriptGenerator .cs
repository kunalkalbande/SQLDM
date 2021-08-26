using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    /// <summary>
    /// //SQLDm 10.0  - Srishti Purohit- New Recommendations - SDR- Q41
    /// </summary>
    public class ClearQueryStoreScriptGenerator : IScriptGenerator, IUndoScriptGenerator
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
        public ClearQueryStoreScriptGenerator(string database)
        {
            Database = database;
        }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            create.AppendLine(string.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.SetQueryStoreClear.sql"), Database));
            return (create.ToString());
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            return (create.ToString());
        }
    }
}