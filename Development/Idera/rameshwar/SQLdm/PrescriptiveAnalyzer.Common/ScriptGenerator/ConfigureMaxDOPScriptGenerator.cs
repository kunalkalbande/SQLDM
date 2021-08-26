using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    public class ConfigureMaxDOPScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        public int SuggestedMaxDOP { get; private set; }
        public UInt32 ObservedMaxDOP { get; private set; }
        public ConfigureMaxDOPScriptGenerator(int suggested, UInt32 observed)
        {
            SuggestedMaxDOP = suggested;
            ObservedMaxDOP = observed;
        }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            return (FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.ConfigureMaxDOP.sql"), this));
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            return (FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.ConfigureMaxDOPUndo.sql"), this));
        }
    }
}
