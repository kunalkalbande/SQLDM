using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class ImplicitConversionInPredicateRecommendation : PredicateRecommendation
    {
        public readonly string DataType;

        public ImplicitConversionInPredicateRecommendation(string db, string schema, string table, string column, string stmt, string batch, string op, string objectName, string applicationName, string hostName, string userName, UInt64 totalDuration, int traceDurationMinutes, string dataType)
            : base(RecommendationType.ImplicitConversionInPredicate, db, schema, table, column, stmt, batch, op, objectName, applicationName, hostName, userName, totalDuration, traceDurationMinutes)
        {
            DataType = dataType;
        }

        public ImplicitConversionInPredicateRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.ImplicitConversionInPredicate, recProp)
        {
            DataType = recProp.GetString("DataType");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("DataType", DataType.ToString());
            return prop;
        }

    }
}
