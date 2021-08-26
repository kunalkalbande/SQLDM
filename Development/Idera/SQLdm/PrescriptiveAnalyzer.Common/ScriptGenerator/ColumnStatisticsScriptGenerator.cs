using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    public class ColumnStatisticsScriptGenerator : IScriptGenerator
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
        public string Schema { get; private set; }
        public string Table { get; private set; }
        public string Column { get; private set; }
        public string SafeColumn { get { return (SQLHelper.CreateSafeString(Column)); } }
        public string SafeSchemaTable { get { return (SQLHelper.CreateSafeString(SQLHelper.Bracket(Schema, Table))); } }
        public ColumnStatisticsScriptGenerator(string database, string schema, string table, string column)
        {
            Database = database;
            Schema = schema;
            Table = table;
            Column = column;
        }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            return (FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdoctor.Common.ScriptGenerator.Templates.UpdateColumnStatistics.sql"), this));
        }
    }
}
