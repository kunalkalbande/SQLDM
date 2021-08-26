using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class UserDefinedFunctionInPredicateRecommendation : FunctionInPredicateRecommendation
    {
        public UserDefinedFunctionInPredicateRecommendation(string db, string schema, string table, string column, string stmt, string batch, string op, string objectName, string applicationName, string hostName, string userName, UInt64 totalDuration, int traceDurationMinutes, string function)
            : base(FunctionType.UserDefined, db, schema, table, column, stmt, batch, op, objectName, applicationName, hostName, userName, totalDuration, traceDurationMinutes, function)
        {
        }
    }
}
