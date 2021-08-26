using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    /// <summary>
    /// //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-Q37
    /// </summary>
    public class Flag4199LowCompatibleScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        public List<DbWithCompatibility> DbWithCompatibilityLowerThan130 { get; set; }
        private int defaultCompatibility = 130;

        public Flag4199LowCompatibleScriptGenerator(List<DbWithCompatibility> dbCompatibilty)
        {
            DbWithCompatibilityLowerThan130 = dbCompatibilty;
        }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            foreach (DbWithCompatibility dbWithCompObj in DbWithCompatibilityLowerThan130)
            {
                create.AppendLine(string.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.SetCompatibilityForDatabase.sql"), dbWithCompObj.DbName, defaultCompatibility));
            }
            create.AppendLine(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.DisableTrace4199.sql"));
            return (create.ToString());
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            foreach (DbWithCompatibility dbWithCompObj in DbWithCompatibilityLowerThan130)
            {
                int compatibility = dbWithCompObj.Compatibility;
                create.AppendLine(string.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.SetCompatibilityForDatabase.sql"), dbWithCompObj.DbName, compatibility));
            }
            create.AppendLine(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.EnableTrace4199.sql"));
            return (create.ToString());
        }
    }
}
