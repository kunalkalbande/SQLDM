using System;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using System.Collections.Generic;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class PageVerifyRecommendation : Recommendation, IScriptGeneratorProvider, IProvideDatabase, IUndoScriptGeneratorProvider
    {
        public string Database { get; private set; }
        public string ObservedVerifyOption { get; private set; }


        public PageVerifyRecommendation(bool isReadOnly)
            : base(isReadOnly ? RecommendationType.PageVerifyReadonly : RecommendationType.PageVerifyWriteable)
        {
        }

        public PageVerifyRecommendation(string database, bool isReadOnly, string observedVerifyOption)
            : base(isReadOnly ? RecommendationType.PageVerifyReadonly : RecommendationType.PageVerifyWriteable)
        {
            Database = database;
            ObservedVerifyOption = observedVerifyOption;
        }

        public PageVerifyRecommendation(bool isReadOnly, RecommendationProperties recProp)
            : base(isReadOnly ? RecommendationType.PageVerifyReadonly : RecommendationType.PageVerifyWriteable, recProp)
        {
            ObservedVerifyOption = recProp.GetString("ObservedVerifyOption");
            Database = recProp.GetString("Database");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("ObservedVerifyOption", ObservedVerifyOption.ToString());
            prop.Add("Database", Database.ToString());
            return prop;
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new DatabaseConfigurationScriptGenerator(Database,"PAGE_VERIFY",RecommendationType == RecommendationType.PageVerifyReadonly ? "NONE" : "CHECKSUM");
        }

        public bool IsScriptRunnable { get { return (true); } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new DatabaseConfigurationScriptGenerator(Database, "PAGE_VERIFY", ObservedVerifyOption);
        }

        public bool IsUndoScriptRunnable 
        { 
            get 
            {
                return (!string.IsNullOrEmpty(ObservedVerifyOption));
            } 
        }

    }
}