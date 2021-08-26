using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Collectors;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Analyzers
{
    interface IAnalyzeDataBucket
    {
        Int32 ID { get; }
        void Analyze(DataBucket bucket);
        IRecommendation[] GetRecommendations();
    }
}
