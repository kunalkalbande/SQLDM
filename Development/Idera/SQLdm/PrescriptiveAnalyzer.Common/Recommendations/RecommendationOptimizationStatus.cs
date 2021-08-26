using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    //enum mapped with DB table OptimiztionExecutionStatus for presscriptive analsis Recommendation optimization status master data
    [Serializable]
    public enum RecommendationOptimizationStatus
    {
        NotOptimized = 1,
        OptimizationCompleted = 2,
        OptimizationException = 3,
        OptimizationUndone = 4,
        OptimizationUndoneException = 5
    }
}
