using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginAddInView;
using System.AddIn;
using Idera.SQLdm.Service.Core;
using System.Diagnostics;
using Idera.SQLdm.Service.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using Idera.SQLdm.Service.ServiceContracts.v1;
using Idera.SQLdm.Service.DataContracts.v1.Errors;
using Idera.SQLdm.Common.Messages;
using Idera.SQLdm.Service.Core.Security;
using System.Threading;
using PluginCommon;
using PluginAddInView;

namespace Idera.SQLdm.Service
{

    [AddIn("SQLdm", Version = "11.1.0.0",Description="SQL diagnostic manager", Publisher="IDERA Inc.")]

    public class SQLdmAddInHook:IPluginV2
    {
        HostObject _sqlDMRESTHost = null;
        System.Diagnostics.EventLog _eventLog = null;
        private Dictionary<string, WebServiceHost> _allHosts = new Dictionary<string, WebServiceHost>();
        /// <summary>
        /// SQLdm9.0 (Gaurav): passing the host object from IderaCoreServices and initializing the service class
        /// </summary>
        /// <param name="hostObj"></param>
        public void Initialize(HostObject hostObj)
        {
            //_sqldmRestService = new SQLdmRestService();
            //if(_sqldmRestService!=null) _sqldmRestService.InitializeHost(hostObj);
            _sqlDMRESTHost = hostObj;
            
            //START: setting up logging in the event manager
            _eventLog = new EventLog("Application");

            if (!System.Diagnostics.EventLog.SourceExists(CoreSettings.BaseServiceName))//initiating the event log with name of service
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    CoreSettings.BaseServiceName, CoreSettings.BaseServiceName + "log");
            }
            _eventLog.Source = CoreSettings.BaseServiceName;
            _eventLog.Log = CoreSettings.BaseServiceName + "log";
            //END: : setting up logging in the event manager
        }

        /// <summary>
        /// SQLdm9.0 (Gaurav): starting the REST service
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="urlAuthority"></param>
        public void StartREST(string productId, string urlAuthority)
        {
            //if (_sqldmRestService != null) _sqldmRestService.StartWebService(productId,urlAuthority);
            _eventLog.WriteEntry("entering StartWebService method of the services. Web Services will be hosted on: " + urlAuthority);
            try
            {
                //Start: SQLdm 9.0 (Gaurav Karwal): Here we are configuring the existing service and adding endpoints and bindings. Code for authentication has been removed from here. 
                WebService SQLdmRestService_ForWebUI = new WebService(_sqlDMRESTHost); //initializing the service class
                WebHttpBinding restServiceBinding = null;
                WebServiceHost _sqldmWebServiceHost = new WebServiceHost(SQLdmRestService_ForWebUI, new Uri(urlAuthority)); //declaring a web service host
                restServiceBinding = new WebHttpBinding(WebHttpSecurityMode.TransportCredentialOnly);
                //Start: SQLdm 10.2 (Srishti Purohit): Defect SQLDM-27742 fix. Request was failing due to large size object. Reference : "http://stackoverflow.com/questions/14636407/maxreceivedmessagesize-not-fixing-413-request-entity-too-large"
                restServiceBinding.MaxBufferPoolSize = 2147483647;
                restServiceBinding.MaxReceivedMessageSize = 2147483647;
                restServiceBinding.MaxBufferSize = 2147483647;
                //Start: SQLdm 10.2 (Srishti Purohit): Defect SQLDM-27742 fix.

                //SQLdm 9.0 (Gaurav): creating the endpoints for the web services at /v1
                ServiceEndpoint se = _sqldmWebServiceHost.AddServiceEndpoint(typeof(IWebService1), restServiceBinding != null ? restServiceBinding : new WebHttpBinding(), urlAuthority);
                se.Behaviors.Add(new ErrorHandlerBehavior());
                se.Behaviors.Add(new WebHttpBehaviorEx() { HelpEnabled = true });

                //SQLdm 9.0 (Gaurav): enabling exception details if a fault is returned
                _sqldmWebServiceHost.Description.Behaviors.Find<ServiceDebugBehavior>().IncludeExceptionDetailInFaults = true;
                _sqldmWebServiceHost.Open(); //finally opening hte service host

                _allHosts.Add(productId, _sqldmWebServiceHost);//adding the service host to the global collection

            }
            catch (Exception ex_running_service)
            {
                _eventLog.WriteEntry("exception thrown in StartWebService while hosting on the url:" + urlAuthority + ". error details:" + ex_running_service.Message + "::inner exception: " + (ex_running_service.InnerException!=null?ex_running_service.InnerException.Message:string.Empty));
            }

            _eventLog.WriteEntry("exiting StartWebService method of the services. Web services successfully hosted on:" + urlAuthority);

        }

        public void StopREST(string productId)
        {
            //if (_sqldmRestService != null) _sqldmRestService.StopWebService(productId);
            _eventLog.WriteEntry("entering StopWebService method of the services for the product with id:" + productId);
            try
            {
                WebServiceHost myHost = null;

                if (_allHosts.TryGetValue(productId, out myHost))
                {
                    if (myHost != null && (myHost.State == CommunicationState.Opened || myHost.State == CommunicationState.Opening)) myHost.Close();
                }
                else
                {
                    _eventLog.WriteEntry("hosts collection could not come up with the host object while stopping services  for the product with id:" + productId);
                }

            }
            catch (Exception ex_running_service)
            {
                _eventLog.WriteEntry("exception thrown in StopWebService  for the product with id:" + productId + ". error details: " + ex_running_service.Message);
            }

            _eventLog.WriteEntry("exiting StopWebService method of the services for the product with id:" + productId);

        }

        public string getRESTUrl(string productId)
        {
            return "/" + productId + "/v1";
        }

        public string getWebUrl(string productId)
        {
            return "/" + productId + "/sqldmweb";
        }

        public void ManageDatabase(Product product, string status, Database database) { }
        public void ManageInstance(Product product, string status, Instance instance) { }
        public void OnElevatedLocalTag(Product product, Tag newTag) { }
        public void OnMigration(Product product, MigrateProductResult result) { }
    }
}
