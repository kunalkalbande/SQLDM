using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    public class ConfigureIndexCreateMemoryScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        public UInt64 ObservedIndexMemory { get; private set; }
        public ConfigureIndexCreateMemoryScriptGenerator(UInt64 memoryInKB)
        {
            ObservedIndexMemory = memoryInKB;
        }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            return (FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.ConfigureIndexCreateMemory.sql"), this));
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            return (FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.ConfigureIndexCreateMemoryUndo.sql"), this));
        }
    }
}
