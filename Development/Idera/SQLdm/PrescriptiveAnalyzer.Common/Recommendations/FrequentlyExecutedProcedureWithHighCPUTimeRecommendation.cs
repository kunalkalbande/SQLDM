using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// // SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-Q43 adding new recommendation class
    /// </summary>
    [Serializable]
    public class FrequentlyExecutedProcedureWithHighCPUTimeRecommendation : Recommendation, IProvideDatabase
    {
        public string Database { get; private set; }
        public string ProcedureName { get; private set; }

        public FrequentlyExecutedProcedureWithHighCPUTimeRecommendation(string db, string procName)
            : base(RecommendationType.FrequentlyExecutedProcedureWithHighCPUTime)
        {
            Database = db;
            ProcedureName = procName;
        }
        public FrequentlyExecutedProcedureWithHighCPUTimeRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.FrequentlyExecutedProcedureWithHighCPUTime, recProp)
        {
            Database = recProp.GetString("dbname");
            ProcedureName = recProp.GetString("ProcedureName");
        }
        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("dbname", Database.ToString());
            prop.Add("ProcedureName", ProcedureName.ToString());
            return prop;
        }

    }
}
