using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-I24 adding new recommendation class
    /// </summary>
    [Serializable]
    public class HighModificationsSinceLastStatUpdateRecommendation : Recommendation, IProvideTableName, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, ITransactionLessScript
    {
        public string Database { get; private set; }
        public string Schema { get; private set; }
        public string Table { get; private set; }

        public HighModificationsSinceLastStatUpdateRecommendation(string db, string schema, string table)
            : base(RecommendationType.HighModificationsSinceLastStatUpdate)
        {
            Database = db;
            Schema = schema;
            Table = table;
        }
        public HighModificationsSinceLastStatUpdateRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.HighModificationsSinceLastStatUpdate, recProp)
        {
            Database = recProp.GetString("dbname");
            Table = recProp.GetString("TableName");
            Schema = recProp.GetString("SchemaName");
        }
        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("dbname", Database.ToString());
            prop.Add("TableName", Table.ToString());
            prop.Add("SchemaName", Schema.ToString());
            return prop;
        }

        #region OptimizationUndo
        public IScriptGenerator GetScriptGenerator()
        {
            return new HighModificationsSinceLastStatUpdateScriptGenerator();
        }

        public bool IsScriptRunnable { get { return (true); } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new HighModificationsSinceLastStatUpdateScriptGenerator();
        }

        public bool IsUndoScriptRunnable { get { return (true); } }

        /// <summary>
        /// Implemented ITransactionLessScript to support opti/undo scripts to run without transactions
        /// </summary>
        public bool IsScriptTransactionLess
        {
            get { return true; }
        }

        public bool IsUndoScriptTransactionLess
        {
            get { return true; }
        }
        #endregion
    }
}
