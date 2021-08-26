using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    /// <summary>
    /// //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-R8
    /// </summary>
    public class AvailabilityGroupDBFailoverScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        private string _name;
        public string AvailabilityGroupName
        {
            get
            {
                if (String.IsNullOrEmpty(_name))
                    return null;
                return SQLHelper.CreateBracketedString(_name);
            }
            private set { _name = value; }
        }

        public AvailabilityGroupDBFailoverScriptGenerator(string name)
        {
            AvailabilityGroupName = name;
        }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            create.AppendLine(FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.EnableAvailablityGroupDBFailover.sql"), this));
            return (create.ToString());
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();

            create.AppendLine(FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.DisableAvailablityGroupDBFailover.sql"), this));
            return (create.ToString());
        }
    }
}
