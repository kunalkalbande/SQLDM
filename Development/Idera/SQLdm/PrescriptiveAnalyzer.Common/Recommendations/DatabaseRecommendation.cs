using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DatabaseRecommendation : Recommendation, IProvideDatabase
    {
        public string Database { get; private set; }
        internal DatabaseRecommendation(RecommendationType rt, string database) : base(rt)
        {
            Database = database;
        }

        internal DatabaseRecommendation(RecommendationType rt)
            : base(rt)
        {
         
        }

        internal DatabaseRecommendation(RecommendationType rt, RecommendationProperties recProp)
            : base(rt, recProp)
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
