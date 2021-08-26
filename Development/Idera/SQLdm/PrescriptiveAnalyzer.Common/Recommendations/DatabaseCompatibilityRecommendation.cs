using System;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using System.Collections.Generic;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DatabaseCompatibilityRecommendation : Recommendation, IScriptGeneratorProvider, IProvideDatabase, IUndoScriptGeneratorProvider
    {
        public string Database { get; private set; }
        public readonly string CurrentCompatibility;
        public readonly string TargetCompatibility;

        public DatabaseCompatibilityRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DatabaseCompatibility, recProp)
        {
            Database = recProp.GetString("Database");
            CurrentCompatibility = recProp.GetString("CurrentCompatibility");
            TargetCompatibility = recProp.GetString("TargetCompatibility");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Database", Database.ToString());
            prop.Add("CurrentCompatibility", CurrentCompatibility.ToString());
            prop.Add("TargetCompatibility", TargetCompatibility.ToString());
            return prop;
        }

        public DatabaseCompatibilityRecommendation(string database, string currentCompatibility, string targetCompatibility) : base(RecommendationType.DatabaseCompatibility)
        {
            Database = database;
            CurrentCompatibility = currentCompatibility;
            TargetCompatibility = targetCompatibility;
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new DatabaseCompatibilityScriptGenerator(Database, TargetCompatibility, CurrentCompatibility);
        }

        public bool IsScriptRunnable { get { return (true); } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new DatabaseCompatibilityScriptGenerator(Database, TargetCompatibility, CurrentCompatibility);
        }

        public bool IsUndoScriptRunnable { get { return (true); } }

    }
}