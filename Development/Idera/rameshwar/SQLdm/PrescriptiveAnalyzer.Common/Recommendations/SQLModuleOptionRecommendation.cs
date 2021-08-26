using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class SQLModuleOptionRecommendation : Recommendation, IScriptGeneratorProvider, IProvideDatabase, IUndoScriptGeneratorProvider
    {
        public  bool UsesQuotedIdentifier;
        public  bool UsesAnsiNulls;
        public  string DatabaseName;
        public  string SchemaName;
        public  string ObjectName;
        public  string ObjectType;
        public  string ObjectDefinition;
        public string Database { get { return DatabaseName; } }

        public SQLModuleOptionRecommendation() : base(RecommendationType.SQLModuleOptions)
        {
            
            
        }

        public SQLModuleOptionRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.SQLModuleOptions, recProp)
        {
            UsesQuotedIdentifier = recProp.GetBool("UsesQuotedIdentifier");
            UsesAnsiNulls = recProp.GetBool("UsesAnsiNulls");
            DatabaseName = recProp.GetString("DatabaseName");
            SchemaName = recProp.GetString("SchemaName");
            ObjectName = recProp.GetString("ObjectName");
            ObjectType = recProp.GetString("ObjectType");
            ObjectDefinition = recProp.GetString("ObjectDefinition");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("UsesQuotedIdentifier", UsesQuotedIdentifier.ToString());
            prop.Add("UsesAnsiNulls", UsesAnsiNulls.ToString());
            prop.Add("DatabaseName", DatabaseName.ToString());
            prop.Add("SchemaName", SchemaName.ToString());
            prop.Add("ObjectName", ObjectName.ToString());
            prop.Add("ObjectType", ObjectType.ToString());
            prop.Add("ObjectDefinition", ObjectDefinition.ToString());
            return prop;
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new RecatalogModuleScriptGenerator(this);
        }

        public bool IsScriptRunnable { get { return true; } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new RecatalogModuleScriptGenerator(this);
        }

        public bool IsUndoScriptRunnable { get { return true; } }
    }
}
