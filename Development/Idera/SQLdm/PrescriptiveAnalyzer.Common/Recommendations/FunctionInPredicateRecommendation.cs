using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public enum FunctionType
    {
        Intrinsic,
        UserDefined
    }

    [Serializable]
    public class FunctionInPredicateRecommendation : PredicateRecommendation
    {
        public readonly string FunctionName;
        public readonly FunctionType FunctionType;

        public FunctionInPredicateRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.FunctionInPredicate,recProp)
        {
            FunctionName = recProp.GetString("FunctionName");
            FunctionType = (FunctionType) recProp.GetInt("FunctionType");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("FunctionName", FunctionName.ToString());
            prop.Add("FunctionType", ((int)FunctionType).ToString());
            return prop;
        }

        public FunctionInPredicateRecommendation(FunctionType functionType, string db, string schema, string table, string column, string stmt, string batch, string op, string objectName, string applicationName, string hostName, string userName, UInt64 totalDuration, int traceDurationMinutes, string function)
            : base(RecommendationType.FunctionInPredicate, db, schema, table, column, stmt, batch, op, objectName, applicationName, hostName, userName, totalDuration, traceDurationMinutes)
        {
            FunctionName = HandleFunctionName(function, stmt);
            FunctionType = functionType;
            GenerateFocusSelectionsWithinPredicate(FunctionName);
        }

        private string HandleFunctionName(string function, string stmt)
        {
            if (0 != string.Compare(function.Trim(), "substring", true)) return (function);
            string stmtlower = stmt.ToLower();
            if (stmtlower.Contains(function.ToLower())) return (function);
            if (stmtlower.Contains("right")) return ("right");
            if (stmtlower.Contains("left")) return ("left");
            return (function);
        }

    }
}
