using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
//using Idera.SQLdoctor.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    interface IAnalyze
    {
        Int32 ID { get; }

        void Analyze(SnapshotMetrics sm, SqlConnection conn);

        string GetDescription();
        // clear recommendations
        void Clear();
        // get recommendations
        IEnumerable<IRecommendation> GetRecommendations();
        // get exceptions
        IEnumerable<Exception> GetExceptions();
    }
}
