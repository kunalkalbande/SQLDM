using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ComponentModel;
using Idera.SQLdm.Service.DataContracts.v1;
using Idera.SQLdm.Service.DataContracts.v1.Errors;
using Idera.SQLdm.Service.DataContracts.v1.Widgets;
using Idera.SQLdm.Service.DataContracts.v1.Database;
using Idera.SQLdm.Service.Web;
using Idera.SQLdm.Common.Configuration;
namespace Idera.SQLdm.Service.ServiceContracts.v1
{
    /// <summary>
    /// SQLdm 9.1 (Gaurav Karwal) : Added to get product related information.
    /// </summary>
    [ServiceContract]
    public interface IProductInfo
    {
        
        /// <summary>
        /// SQLdm 9.1 (Gaurav Karwal) : Added to get product status information e.g. alerts count
        /// </summary>
        /// <returns>Product Status Object that contains Objects like Alert Status</returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/ProductStatus", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Gets the product status of SQLdm")]
        ProductStatus GetProductStatus();

        /// <summary>
        /// SQLdm 9.1 (Sanjali Makkar) (Instance Status) : Added to get Instances related information e.g. Total Monitored Servers, Disabled Servers Count, Critical Servers
        /// </summary>
        /// <returns> Instance Status Object that contains Objects like Alert Status, Instance Overview </returns>
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/InstanceStatus", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [Description("Gets the status of Instances")]
        InstanceStatus GetInstanceStatus();


    }
}
