using System;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using System.Collections.Generic;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// //SQLDm 10.0 - Srishti Purohit - New Recommendations - SDR-Q41- adding new class
    /// </summary>
    [Serializable]
    public class QueryStoreAlmostFullRecommendation : Recommendation, IProvideDatabase, IScriptGeneratorProvider, ITransactionLessScript//, IUndoScriptGeneratorProvider
    {
        public string Database { get; private set; }

        public QueryStoreAlmostFullRecommendation(string database)
            : base(RecommendationType.QueryStoreAlmostFull)
        {
            Database = database;
        }
        public QueryStoreAlmostFullRecommendation(RecommendationProperties recProp)
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

        public IScriptGenerator GetScriptGenerator()
        {
            return new ClearQueryStoreScriptGenerator(Database);
        }

        public bool IsScriptRunnable { get { return (true); } }

        //public IUndoScriptGenerator GetUndoScriptGenerator()
        //{
        //    return new ClearQueryStoreScriptGenerator(Database);
        //}

        //public bool IsUndoScriptRunnable { get { return (true); } }
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
    }
}