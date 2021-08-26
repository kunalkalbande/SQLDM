using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    public class ConfigureAffinityMaskScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        public ulong? ObservedAffinityMask { get; private set; }
        public ConfigureAffinityMaskScriptGenerator(ulong? observed)
        {
            ObservedAffinityMask = observed;
        }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            return (ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.ConfigureAffinityMask.sql"));
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            return (FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.ConfigureAffinityMaskUndo.sql"), this));
        }
    }
}
