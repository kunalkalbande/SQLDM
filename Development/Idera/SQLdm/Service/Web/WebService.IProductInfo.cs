using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using Idera.SQLdm.Service.ServiceContracts.v1;
using Idera.SQLdm.Service.Repository;
using System.Security.Principal;
using Idera.SQLdm.Service.DataContracts.v1;
using System.ServiceModel;
using Idera.SQLdm.Service.Configuration;
using System.ServiceModel.Web;
using CWFContracts = Idera.SQLdm.Common.CWFDataContracts;
using CWFHelper = Idera.SQLdm.Service.Helpers.CWF;
using PluginCommon;
using Idera.SQLdm.Service.DataContracts.v1.Widgets;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Service.Repository;


namespace Idera.SQLdm.Service.Web
{
    /// <summary>
    /// SQLdm 9.0 (Gaurav Karwal): implementing the caller to the cwf functions for synchronizing instances
    /// </summary>
    public partial class WebService : IProductInfo
    {
        /// <summary>
        /// implementation of the GetProductStatus method in the interface IProductInfo.
        /// </summary>
        /// <returns></returns>
        public ProductStatus GetProductStatus() 
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetProductStatus to Implement Instance level security
            return RepositoryHelper.GetProductStatus(RestServiceConfiguration.SQLConnectInfo, userToken);
        }
        /// <summary>
        // SQLdm 9.1 (Sanjali Makkar) (Instance Status) : Implementation of the GetInstanceStatus method in the interface IProductInfo.
        /// </summary>
        /// <returns></returns>
        public Idera.SQLdm.Service.DataContracts.v1.Widgets.InstanceStatus GetInstanceStatus()
        {
            SetConnectionCredentiaslFromCWFHost();
            //SQLdm 10.0 (Swati Gogia):Passed userToken to GetInstanceStatus to Implement Instance level security
            return RepositoryHelper.GetInstanceStatus(RestServiceConfiguration.SQLConnectInfo, userToken);
        }
    }

    
}
