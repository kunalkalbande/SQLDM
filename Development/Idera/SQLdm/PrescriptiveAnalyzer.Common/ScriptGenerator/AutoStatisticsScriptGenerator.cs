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
    public class AutoStatisticsScriptGenerator : IScriptGenerator, IUndoScriptGenerator
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
        public bool AutoCreate { get; private set; }
        public bool AutoUpdate { get; private set; }
        public AutoStatisticsScriptGenerator(string database, bool autoCreate, bool autoUpdate)
        {
            Database = database;
            AutoCreate = autoCreate;
            AutoUpdate = autoUpdate;
        }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            if (!AutoCreate)
            {
                create.AppendLine(FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.AutoCreateStatistics.sql"), this));
            }
            if (!AutoUpdate)
            {
                create.AppendLine(FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.AutoUpdateStatistics.sql"), this));
            }
            create.AppendLine(FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.UpdateDatabaseStatistics.sql"), this));
            return (create.ToString());
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            if (!AutoCreate)
            {
                create.AppendLine(FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.AutoCreateStatisticsUndo.sql"), this));
            }
            if (!AutoUpdate)
            {
                create.AppendLine(FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.AutoUpdateStatisticsUndo.sql"), this));
            }
            return (create.ToString());
        }
    }
}
