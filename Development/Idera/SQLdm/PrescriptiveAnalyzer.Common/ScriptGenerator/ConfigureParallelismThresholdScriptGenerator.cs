using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    public class ConfigureParallelismThresholdScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        public UInt32 RecommendedThreshold { get; private set; }
        public UInt32 ObservedThreshold { get; private set; }
        public ConfigureParallelismThresholdScriptGenerator(UInt32 suggested, UInt32 observed)
        {
            RecommendedThreshold = suggested;
            ObservedThreshold = observed;
        }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            return (FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.ConfigureParallelismThreshold.sql"), this));
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            return (FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.ConfigureParallelismThresholdUndo.sql"), this));
        }
    }
}
