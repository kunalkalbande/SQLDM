using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator
{
    /// <summary>
    /// //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-I23
    /// </summary>
    public class NonIncrementalColumnStatOnPartitionedTableScriptGenerator : IScriptGenerator, IUndoScriptGenerator
    {
        private string Database { get; set; }
        private string OptimizeScript { get; set; }
        private string UndoScript { get; set; }

        public NonIncrementalColumnStatOnPartitionedTableScriptGenerator(string db, string opti, string undo)
        {
            OptimizeScript = opti;
            UndoScript = undo;
            Database = db;
        }

        public string GetTSqlFix(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            create.Append("use [" + Database + "];");
            create.Append(Environment.NewLine);
            create.Append(OptimizeScript);
            return (create.ToString());
        }

        public string GetTSqlUndo(SqlConnectionInfo connectionInfo)
        {
            StringBuilder create = new StringBuilder();
            create.Append("use [" + Database + "];");
            create.Append(Environment.NewLine);
            create.Append(UndoScript);
            return (create.ToString());
        }
    }
}
