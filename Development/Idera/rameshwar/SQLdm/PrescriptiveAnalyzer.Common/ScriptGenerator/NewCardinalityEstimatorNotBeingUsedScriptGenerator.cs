using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    /// <summary>
    /// //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-Q44
    /// </summary>
    public class NewCardinalityEstimatorNotBeingUsedScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        public List<DbWithCompatibility> DbWithCompatibilityLowerThan120 { get; set; }
        private int defaultCompatibility = 120;

        public NewCardinalityEstimatorNotBeingUsedScriptGenerator(List<DbWithCompatibility> dbCompatibilty)
        {
            DbWithCompatibilityLowerThan120 = dbCompatibilty;
        }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            foreach (DbWithCompatibility dbWithCompObj in DbWithCompatibilityLowerThan120)
            {
                create.AppendLine(string.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.SetCompatibilityForDatabase.sql"), dbWithCompObj.DbName, defaultCompatibility));
            }
            return (create.ToString());
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            foreach (DbWithCompatibility dbWithCompObj in DbWithCompatibilityLowerThan120)
            {
                int compatibility = dbWithCompObj.Compatibility;
                create.AppendLine(string.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.SetCompatibilityForDatabase.sql"), dbWithCompObj.DbName, compatibility));
            }
            return (create.ToString());
        }
    }
}
