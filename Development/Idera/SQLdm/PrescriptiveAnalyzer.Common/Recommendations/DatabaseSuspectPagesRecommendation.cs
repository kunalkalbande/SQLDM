using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DatabaseSuspectPagesRecommendation : DatabaseRecommendation
    {
        public DatabaseSuspectPagesRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DatabaseSuspectPages, recProp)
        { }

        public DatabaseSuspectPagesRecommendation(string database)
            : base(RecommendationType.DatabaseSuspectPages, database)
        {
        }

        public override Dictionary<string, string> GetProperties()
        {
            return base.GetProperties();
        }
    }
}
