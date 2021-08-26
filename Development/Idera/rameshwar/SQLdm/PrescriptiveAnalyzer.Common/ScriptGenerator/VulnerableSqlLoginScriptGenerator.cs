using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    public class VulnerableSqlLoginScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        private string _name;
        public string ObservedCheckPolicy { get; private set; }
        public string ObservedCheckExpiration { get; private set; }

        public VulnerableSqlLoginScriptGenerator(string name, string observedCheckPolicy, string observedCheckExpiration)
        {
            this._name = name;
            ObservedCheckPolicy = observedCheckPolicy;
            ObservedCheckExpiration = observedCheckExpiration;
        }

        public string Name
        {
            get
            {
                if (String.IsNullOrEmpty(_name))
                    return null;
                return SQLHelper.CreateBracketedString(_name);
            }
        }

        #region IScriptGenerator Members

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            create.AppendLine(
                FormatHelper.Format(
                    ApplicationHelper.GetEmbededResource(GetType().Assembly,
                                                         "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.FixVulnerableLogin.sql"),
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
                                                         "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.FixVulnerableLoginUndo.sql"),
                    this));
            return (create.ToString());
        }

        #endregion
    }
}
