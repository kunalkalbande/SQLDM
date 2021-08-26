using Idera.SQLdm.Service.DataContracts.v1;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Idera.SQLdm.Service.ServiceContracts.v1
{
    [ServiceContract]
    public interface IRecommendationManager
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{instanceId}/recommendations/listing", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Get a list of Recommendations")]
        IList<Analysis> GetRecommendationList(string instanceId);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "Instances/{instanceId}/recommendations/{id}/details", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        IList<UIRecommendation> GetRecommendationDetails(string instanceId, string id);
    }
}
