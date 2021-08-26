using System;
using System.Collections.Generic;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// //SQLDm 10.0 - Srishti Purohit- New Recommendations - SDR-Q42- adding new class
    /// </summary>
    [Serializable]
    public class PlanGuidesUsedOverQueryStoreRecommendation : Recommendation, IProvideDatabase//, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, ITransactionLessScript
    {
        public string Database { get; private set; }
        public List<string> PlanGuideNames;

        public PlanGuidesUsedOverQueryStoreRecommendation(string database, List<string> names)
            : base(RecommendationType.PlanGuidesUsedOverQueryStore)
        {
            Database = database;
            PlanGuideNames = names;
        }
        public PlanGuidesUsedOverQueryStoreRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.PlanGuidesUsedOverQueryStore, recProp)
        {
            Database = recProp.GetString("dbname");
            //PlanGuideNames = recProp.GetString("planName");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("dbname", Database.ToString());
            //prop.Add("planName", PlanGuideNames);
            return prop;
        }
        //public IScriptGenerator GetScriptGenerator()
        //{
        //    return new PlanGuidesUsedOverQueryStoreScriptGenerator(Database);
        //}

        //public bool IsScriptRunnable { get { return (true); } }

        //public IUndoScriptGenerator GetUndoScriptGenerator()
        //{
        //    return new PlanGuidesUsedOverQueryStoreScriptGenerator(Database);
        //}

        //public bool IsUndoScriptRunnable { get { return (true); } }
        ///// <summary>
        ///// Implemented ITransactionLessScript to support opti/undo scripts to run without transactions
        ///// </summary>
        //public bool IsScriptTransactionLess
        //{
        //    get { return true; }
        //}

        //public bool IsUndoScriptTransactionLess
        //{
        //    get { return true; }
        //}
    }
}