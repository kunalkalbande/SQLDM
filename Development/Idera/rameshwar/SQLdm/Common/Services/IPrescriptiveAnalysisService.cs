using System.Collections.Generic;
using Idera.SQLdm.Common.Configuration;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.Common.Services
{
    public interface IPrescriptiveAnalysisService
    {
        Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result GetPrescriptiveAnalysisResult(int monitoredSqlServerId, AnalysisConfiguration config);

        Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result GetAdHocBatchAnalysisResult(int monitoredSqlServerId, string queryText, string database, AnalysisConfiguration config);

        Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result GetWorkLoadAnalysisResult(int monitoredSqlServerId, AnalysisConfiguration config, ActiveWaitsConfiguration waitConfig, QueryMonitorConfiguration queryConfig);
        
        void SetScheduleTasks(List<PAScheduledTask> tasks);
    }
}
