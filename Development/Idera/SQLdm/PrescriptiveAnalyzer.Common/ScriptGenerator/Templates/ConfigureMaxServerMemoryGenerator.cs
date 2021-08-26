using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates
{
    public class ConfigureMaxServerMemoryGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        public ulong RecommendedSizeMB { get; set; }
        public ulong ObservedSizeMB { get; set; } 

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            return (FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.ConfigureMaxServerMemory.sql"), this));
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            return (FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.ConfigureMaxServerMemoryUndo.sql"), this));
        }
    }
}
