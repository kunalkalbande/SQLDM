using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Stats;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan.Analyzers
{
    interface IAnalyzePlan
    {
        Int32 ID { get; }

        void Analyze(TraceEventStatsCollection tesc);

        // clear recommendations
        void Clear();
        // get recommendations
        IEnumerable<IRecommendation> GetRecommendations();
        IEnumerable<Exception> GetExceptions();
    }
}
