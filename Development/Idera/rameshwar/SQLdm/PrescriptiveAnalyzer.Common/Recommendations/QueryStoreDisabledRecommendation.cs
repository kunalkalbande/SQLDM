using System;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using System.Collections.Generic;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// //SQLDm 10.0 -Srishti Purohit- New Recommendations - SDR-Q39- adding new class
    /// </summary>
    [Serializable]
    public class QueryStoreDisabledRecommendation : Recommendation, IProvideDatabase, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, ITransactionLessScript
    {
        public string Database { get; private set; }

        public QueryStoreDisabledRecommendation(string database)
            : base(RecommendationType.QueryStoreDisabled)
        {
            Database = database;
        }
        public QueryStoreDisabledRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.QueryStoreDisabled, recProp)
        {
            Database = recProp.GetString("dbname");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("dbname", Database.ToString());
            return prop;
        }

        #region OptimizeUndo
        public IScriptGenerator GetScriptGenerator()
        {
            return new QueryStoreDisabledScriptGenerator(Database);
        }

        public bool IsScriptRunnable { get { return (true); } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new QueryStoreDisabledScriptGenerator(Database);
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