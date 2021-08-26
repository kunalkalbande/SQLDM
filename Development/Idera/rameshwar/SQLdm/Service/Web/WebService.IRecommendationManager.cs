using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.Service.Configuration;
using Idera.SQLdm.Service.DataContracts.v1;
using Idera.SQLdm.Service.Repository;
using Idera.SQLdm.Service.ServiceContracts.v1;
using System.Collections.Generic;

namespace Idera.SQLdm.Service.Web
{
    public partial class WebService : IRecommendationManager
    {
        public IList<UIRecommendation> GetRecommendationDetails(string instanceId, string id)
        {
            int monitoredServerId = 0;
            int.TryParse(instanceId, out monitoredServerId);
            int analysisId = 0;
            int.TryParse(id, out analysisId);

            return RepositoryHelper.GetRecommendations(RestServiceConfiguration.SQLConnectInfo, monitoredServerId, analysisId);
        }

        public IList<Analysis> GetRecommendationList(string instanceId)
        {
            int monitoredServerId = 0;
            int.TryParse(instanceId, out monitoredServerId);
            return RepositoryHelper.GetAnalysisListing(RestServiceConfiguration.SQLConnectInfo, monitoredServerId);
        }
    }
}
