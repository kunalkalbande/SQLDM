using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    /// <summary>
    /// //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-D23
    /// </summary>
    public class ResourceGovernerIOStallScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        public List<string> ResourcePoolNameList { get; private set; }
        private int NewMaxIOPSPerVolume = 10000;
        private int NewMinIOPSPerVolume = 0;
        private int DefaultMaxIOPSPerVolume = 0;
        private int DefaultMinIOPSPerVolume = 0;

        public ResourceGovernerIOStallScriptGenerator(List<string> namelist)
        {
            ResourcePoolNameList = namelist;
        }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            foreach (string name in ResourcePoolNameList)
            {
                create.AppendLine(string.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.SetIOPSPerVolumeForResourcePool.sql"), name, NewMinIOPSPerVolume, NewMaxIOPSPerVolume));
            }
            return (create.ToString());
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            foreach (string name in ResourcePoolNameList)
            {
                create.AppendLine(string.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.SetIOPSPerVolumeForResourcePool.sql"), name, DefaultMinIOPSPerVolume, DefaultMaxIOPSPerVolume));
            }
            return (create.ToString());
        }
    }
}
