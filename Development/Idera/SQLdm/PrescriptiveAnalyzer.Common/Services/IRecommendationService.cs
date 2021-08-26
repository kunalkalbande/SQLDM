using System.Collections.Generic;
using Idera.SQLdoctor.Common.Configuration;
using Idera.SQLdoctor.Common.Recommendations;

namespace Idera.SQLdoctor.Common.Services
{
    [Impl("Idera.SQLdoctor.AnalysisEngine", "Idera.SQLdoctor.AnalysisEngine.RecommendationService")]
    public interface IRecommendationService
    {
        /// <summary>
        /// Start an analysis with the given configuration.  This method will not block.  
        /// The methods Wait and GetRecommendations will block until the analysis is complete.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        IRecommendationEngine BeginAnalysis(AnalysisConfiguration configuration);
        /// <summary>
        /// Get server overview.  This method will block until the result is complete.
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <returns></returns>
        IServerOverview GetServerOverview(SqlConnectionInfo connectionInfo);
        IOptimizationEngine BeginOptimization(OptimizationConfiguration configuration, IList<IRecommendation> recommendations);
        IGetBatches GetBatches();
        IRealTimeEngine GetRealTimeEngine(SqlConnectionInfo connectionInfo);
        void Init();
    }
}
