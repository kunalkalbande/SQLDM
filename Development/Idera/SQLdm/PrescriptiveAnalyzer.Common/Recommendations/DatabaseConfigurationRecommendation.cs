using System;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using System.Collections.Generic;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DatabaseConfigurationRecommendation : Recommendation, IScriptGeneratorProvider, IProvideDatabase, IUndoScriptGeneratorProvider
    {
        public string Database { get; private set; }
        public readonly string Configuration;
        public readonly string CurrentValue;
        public readonly string DefaultValue;

        public DatabaseConfigurationRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DatabaseConfiguration,recProp)
        {
            Database = recProp.GetString("Database");
            Configuration = recProp.GetString("Configuration");
            CurrentValue = recProp.GetString("CurrentValue");
            DefaultValue = recProp.GetString("DefaultValue");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Database", Database.ToString());
            prop.Add("Configuration", Configuration.ToString());
            prop.Add("CurrentValue", CurrentValue.ToString());
            prop.Add("DefaultValue", DefaultValue.ToString());
            return prop;
        }
        
        public DatabaseConfigurationRecommendation(string database, string configuration, string currentValue, string defaultValue)
            : base(RecommendationType.DatabaseConfiguration)
        {
            Database = database;
            Configuration = configuration;
            CurrentValue = currentValue;
            DefaultValue = defaultValue;
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new DatabaseConfigurationScriptGenerator(Database, Configuration, DefaultValue);
        }

        public bool IsScriptRunnable { get { return (true); } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new DatabaseConfigurationScriptGenerator(Database, Configuration, CurrentValue);
        }

        public bool IsUndoScriptRunnable { get { return (true); } }

    }
}