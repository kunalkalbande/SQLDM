using BBS.TracerX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Text.RegularExpressions;

namespace Idera.SQLdm.RegistrationService.Configuration
{
    public class CustomHeaderMessageInspector : IDispatchMessageInspector
    {
        private static readonly Logger LogX = Logger.GetLogger("CustomHeaderMessageInspector");

        // This contains the list of all headers which will be returns to the api response
        Dictionary<string, string> requiredHeaders;

        public CustomHeaderMessageInspector(Dictionary<string, string> headers)
        {
            requiredHeaders = headers ?? new Dictionary<string, string>();
        }
            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }


        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            using (LogX.DebugCall("BeforeSendReply"))
            {
                WebOperationContext operationContext = WebOperationContext.Current;
                var httpHeader = reply.Properties["httpResponse"] as HttpResponseMessageProperty;

                if (operationContext.IncomingRequest.Method.Equals("OPTIONS", StringComparison.CurrentCultureIgnoreCase))
                {
                    //Setting this message as we returning a 200 ok message to the options request
                    //https://blogs.msdn.microsoft.com/dsnotes/2016/08/31/wcf-cors-support-for-self-hosted-wcf-rest-service/
                    //overrride the method not allow status code.
                    operationContext.OutgoingResponse.SuppressEntityBody = true;
                    httpHeader.StatusCode = HttpStatusCode.OK;
                    httpHeader.StatusDescription = String.Empty;
                }
                //Adds CORS headers to all responses
                foreach (var item in requiredHeaders)
                {
                    httpHeader.Headers.Add(item.Key, item.Value);
                }
                
            }
        }
    }

    public class EnableCrossOriginResourceSharingBehavior : BehaviorExtensionElement, IEndpointBehavior
    {

        public EnableCrossOriginResourceSharingBehavior()
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {

        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            var requiredHeaders = new Dictionary<string, string>();
            requiredHeaders.Add("Access-Control-Allow-Origin", "*");
            requiredHeaders.Add("Access-Control-Request-Method", "POST,GET,PUT,DELETE,OPTIONS");
            requiredHeaders.Add("Access-Control-Allow-Headers", "X-Requested-With,Content-Type,Authorization");
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new CustomHeaderMessageInspector(requiredHeaders));
        }

        public void Validate(ServiceEndpoint endpoint)
        {

        }

        public override Type BehaviorType
        {
            get { return typeof(EnableCrossOriginResourceSharingBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new EnableCrossOriginResourceSharingBehavior();
        }
    }

}
