using System;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using System.Collections.Generic;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DatabaseNoFixRecommendation : Recommendation, IProvideDatabase
    {

         public string Database { get; private set; }

         public DatabaseNoFixRecommendation(RecommendationType type, string database)
             : base(type)
        {
            this.Database = database;
        }

         public DatabaseNoFixRecommendation(RecommendationType type, RecommendationProperties recProp)
             : base(type, recProp)
         {
             Database = recProp.GetString("Database");
         }

         public override Dictionary<string, string> GetProperties()
         {
             var prop = base.GetProperties();
             prop.Add("Database", Database.ToString());
             return prop;
         }
    }
}