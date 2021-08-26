using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.ServiceModel.Web;
using System.ComponentModel;
using System.IO;
using PluginCommon;
using System.Security.Principal;
namespace Idera.SQLdm.Service.ServiceContracts.v1
{
    /// <summary>
    /// SQldm 10.1 (Pulkit Puri) - For adding new class for tag implementation
    /// </summary>
    [ServiceContract]
    public interface ITagManager
    {
        // Start SQLdm 10.1 (Pulkit Puri) - 
        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/Tags/Global", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        ICollection<Idera.SQLdm.Common.CWFDataContracts.GlobalTag> GetGlobalTags();

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/Tags/Resources", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void SynchronizeResources(TagResourcesList list);


        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "POST", UriTemplate = "/Tags", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void SynchronizeTags(CreateTags tags);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "DELETE", UriTemplate = "/Tags", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void  DeleteTags(Tags tags);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/Tags/{tagId}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Tag GetTag(string tagId);

        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "DELETE", UriTemplate = "/Tags/Resources", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void DeleteTagResources(TagResourcesList resourceList);


        [OperationContract(AsyncPattern = false)]
        [WebInvoke(Method = "GET", UriTemplate = "/Tags/{tagId}/Instances", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Idera.SQLdm.Common.CWFDataContracts.Instance> GetTagInstances(string tagId);




    }

}
