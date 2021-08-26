using Idera.SQLdm.Service.DataContracts.v1;
using Idera.SQLdm.Service.ServiceContracts.v1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdm.Service.Repository;

using Idera.SQLdm.Service.Core.Enums;
using Idera.SQLdm.Service.Core;
using Idera.SQLdm.Service.Configuration;

namespace Idera.SQLdm.Service.Web
{
    public partial class WebService : IWebService1
    {
        //public void ForceCleanup()
        //{
        //    using (_logX.InfoCall("ForceCleanup()"))
        //    {
        //        //LogProcessInfo();
        //        _logX.InfoFormat("Forced GC of {0} begin...", GC.MaxGeneration);
        //        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        //        _logX.InfoFormat("Forced GC of {0} complete", GC.MaxGeneration);
        //        //LogProcessInfo();
        //        _logX.Info("WaitForPendingFinalizers begin...");
        //        GC.WaitForPendingFinalizers();
        //        _logX.Info("WaitForPendingFinalizers complete");
        //        //LogProcessInfo();
        //        _logX.InfoFormat("Post finalization GC of {0} begin...", GC.MaxGeneration);
        //        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        //        _logX.InfoFormat("Post finalization GC of {0} complete", GC.MaxGeneration);
        //        //LogProcessInfo();
        //    }
        //}
        
        //public string GetVersion() { 
        //    return (typeof(WebService).Assembly.GetName().Version.ToString()); 
        //}

        //public DataContracts.v1.GetServiceStatusResponse GetServiceStatus()
        //{
        //    GetServiceStatusResponse response = new GetServiceStatusResponse();
        //    response.CanRun = true;
        //    response.ServiceStatuses = new List<ServiceStatus>();

        //    var state = RepositoryHelper.GetDatabaseState(RestServiceConfiguration.SQLConnectInfo, CoreSettings.RepositoryDatabase);
        //    if (null == state)
        //    {
        //        throw new ApplicationException("The repository database is not available.");               
        //    }
        //    else if (!state.Online)
        //    {
        //        throw new ApplicationException(string.Format("The repository database is {0}.", state.StateDescription));               
        //    }

        //    //ToDo: Use Database state to find out other possible errors
        //    switch (state.State)
        //    {
        //        case 0: { break; }
        //        default: {
        //            response.ServiceStatuses.Add(new ServiceStatus(ServiceState.RepositoryAccessError, new Exception(state.StateDescription)));
        //            response.CanRun = false;
        //            break;
        //        }
        //    }
            
        //    response.Version = RepositoryHelper.GetRepositoryVersion(RestServiceConfiguration.SQLConnectInfo);
        //    return response;
        //}
    }
}
