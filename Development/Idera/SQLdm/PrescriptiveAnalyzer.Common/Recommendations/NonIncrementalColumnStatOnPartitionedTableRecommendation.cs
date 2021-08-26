using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// // 10.0  - Srishti Purohit - New Recommendations (SDR-I23)
    /// Partitioned table has column statistics that are not incremental.
    /// </summary>
    [Serializable]
    public class NonIncrementalColumnStatRecommendation : Recommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider
    {

        public string Database;
        public string TableName;
        public string StatsName;
        public string OptiScript;
        public string UndoScript;

        public NonIncrementalColumnStatRecommendation(string db, string tableName, string statsName, string opti, string undo)
            : base(RecommendationType.NonIncrementalColumnStatOnPartitionedTable)
        {
            Database = db;
            TableName = tableName;
            StatsName = statsName;
            OptiScript = opti;
            UndoScript = undo;
        }
        public NonIncrementalColumnStatRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.NonIncrementalColumnStatOnPartitionedTable, recProp)
        {
            Database = recProp.GetString("Database");
            TableName = recProp.GetString("TableName");
            StatsName = recProp.GetString("StatsName");
            OptiScript = recProp.GetString("OptiScript");
            UndoScript = recProp.GetString("UndoScript");
        }
        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Database", Database.ToString());
            prop.Add("TableName", TableName.ToString());
            prop.Add("StatsName", StatsName.ToString());
            prop.Add("OptiScript", OptiScript.ToString());
            prop.Add("UndoScript", UndoScript.ToString());
            return prop;
        }
        public IScriptGenerator GetScriptGenerator()
        {
            return new NonIncrementalColumnStatOnPartitionedTableScriptGenerator(Database, OptiScript, UndoScript);
        }

        public bool IsScriptRunnable { get { return (true); } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new NonIncrementalColumnStatOnPartitionedTableScriptGenerator(Database, OptiScript, UndoScript);
        }

        public bool IsUndoScriptRunnable { get { return (true); } }
    }
}
