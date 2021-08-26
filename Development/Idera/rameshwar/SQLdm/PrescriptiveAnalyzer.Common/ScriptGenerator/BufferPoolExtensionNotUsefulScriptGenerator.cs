using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    /// <summary>
    /// //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-M32
    /// </summary>
    public class BufferPoolExtensionNotUsefulScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        public string BufferPoolExtFilePath { get; set; }
        public long BufferPoolExtSize { get; set; }

        public BufferPoolExtensionNotUsefulScriptGenerator(string path, long size)
        {
            BufferPoolExtFilePath = path;
            BufferPoolExtSize = size;
        }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            create.AppendLine(FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.DisableBufferPoolExt.sql"), this));
            return (create.ToString());
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            create.AppendLine(FormatHelper.Format(ApplicationHelper.GetEmbededResource(GetType().Assembly, "Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator.Templates.EnableBufferPoolExt.sql"), this));
            return (create.ToString());
        }
    }
}
