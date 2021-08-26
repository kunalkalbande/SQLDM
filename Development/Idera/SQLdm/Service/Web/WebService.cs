using BBS.TracerX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using Idera.SQLdm.Service.Repository;
using Idera.SQLdm.Common.Configuration;
using System.Diagnostics;
using Idera.SQLdm.Service.DataContracts.v1.Errors;
using System.ServiceModel.Channels;
using Idera.SQLdm.Service.Configuration;
using PluginAddInView;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Security.Principal;
using System.Net;
using PluginAddInView;
using PluginCommon;
using Idera.SQLdm.Common.Security.Encryption;
using System.Globalization;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Service.Helpers.Auth;
using Idera.SQLdm.Service.Core;
using Idera.SQLdm.Service.Helpers.CWF;
using Idera.SQLdm.Service.DataModels;
using Idera.SQLdm.Common.Helpers;

namespace Idera.SQLdm.Service.Web
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,ConcurrencyMode = ConcurrencyMode.Multiple)]    
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public partial class WebService //: IServiceBehavior, IDispatchMessageInspector
    {
        private static Logger _logX = Logger.GetLogger("WebService");
        System.Diagnostics.EventLog _eventLog = null;
        private UserToken userToken = new UserToken();
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("ApplicationModel");

        //SQLdm9.0(Gaurav): declaring the service host variable which will represent hte CWF host
        HostObject _myHost = null;

        /// <summary>
        /// SQLdm 9.0 (Gaurav Karwal): Getting the principal object from CWF host
        /// </summary>
        /* CWF Team - Backend Isolation SQLDM-29086
         * Returning Principal object from CWF API instead of returning IPrincipal from CWF HostObject*/
        private Principal GetPrincipalFromCWFHost()
        {
            var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
            _eventLog.WriteEntry("call started (GetPrincipal) at " + DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss:ffff"));
            /* CWF Team - Backend Isolation SQLDM-29086
             * Commenting out HostObject Call and invoking CWF Rest API*/
            //IPrincipal principal = _myHost!=null? _myHost.GetPrincipalWithDomain(header):null;//calling cwf for getting the principal based on the header
            Principal principal = CWFHelper.GetPrincipal(header);

            _eventLog.WriteEntry("call ended (GetPrincipal) at " + DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss:ffff"));
            if (principal == null)
            {
                _eventLog.WriteEntry("Principal found is null, header valuse : " + header);
                throw new WebFaultException(HttpStatusCode.Forbidden);
            }
            _eventLog.WriteEntry("Principal found is right. : " + principal.ToString() + " userInfo.Identity.Name :  " + principal.user.DisplayName + " , header valuse : " + header);
            return principal;
        }

        class UserCredentialsForRESTAPI
        {
            public string UserName { get; set; }

            public string Password { get; set; }
        }

        /// <summary>
        /// SQLdm 10.0(Gaurav Karwal): used to get the user name and password from header
        /// </summary>
        /// <returns></returns>

        private UserCredentialsForRESTAPI GetUserCredentialsFromHeader()
        {
            var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
            UserCredentialsForRESTAPI creds = new UserCredentialsForRESTAPI();
            creds.UserName = string.Empty;
            creds.Password = string.Empty;
            if (header != null && header.StartsWith("BASIC ", StringComparison.InvariantCultureIgnoreCase))
            {
                string authEncodedToken = System.Text.RegularExpressions.Regex.Replace(header, "BASIC", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
                byte[] arrEncoded = Convert.FromBase64String(authEncodedToken);
                string authDecodedToken = Encoding.UTF8.GetString(arrEncoded);
                if (authDecodedToken != null && !String.IsNullOrWhiteSpace(authDecodedToken))
                {
                    string[] splittedAuthToken = authDecodedToken.Split(':');
                    if (splittedAuthToken.Length >= 0)
                    {
                        creds.UserName = null != splittedAuthToken[0] ? splittedAuthToken[0] : string.Empty;
                        creds.Password = null != splittedAuthToken[1] ? splittedAuthToken[1] : string.Empty;

                    }
                }

            }

            return creds;
        }

        /* CWF Team - Backend Isolation - SQLDM-29086
         * Converting Type of UserInfoFromCWFHost to Principal */
        //public IPrincipal UserInfoFromCWFHost = null;
        public Principal UserInfoFromCWFHost = null;

        /// <summary>
        /// SQLdm 9.0 (Gaurav Karwal): setting the repo connection credentials object from CWF host
        /// </summary>
        private void SetConnectionCredentiaslFromCWFHost()
        {
            /* CWF Team - Backend Isolation SQLDM-29086
             * Changing Type of userInfo from IPrincipal to Principal*/
            //IPrincipal userInfo = GetPrincipalFromCWFHost();
            Principal userInfo = GetPrincipalFromCWFHost();
            UserInfoFromCWFHost = userInfo;

            //getting the principal from cwf
            //WARNING - TO BE REMOVED AFTER SQLCORE - 2406: get the detail from basic token
            //UserCredentialsForRESTAPI userDetailsFromBASICToken = GetUserCredentialsFromHeader();

            if (userInfo == null)
                throw new WebFaultException(HttpStatusCode.Forbidden);


            string productId = GetProductIdFromRequest();
            string[] repositoryInformation = null;
            string repositoryHost = string.Empty;
            string repositoryDatabase = string.Empty;
            /* CWF Team - Backend Isolation - SQLDM-29086
             * Calling GetConnectionCredentialsOfProductInstance CWF API instead of calling CWF Host Object */
            //ConnectionCredentials credentials = _myHost.GetConnectionCredentialsOfProductInstance(productId, userInfo);
            ConnectionCredentials credentials = CWFHelper.GetConnectionCredentialsOfProductInstance(productId);
            //[START] SQLdm 10.0 (Gaurav Karwal): this was done to avoid changing the original connection object due to which an error was coming
            ConnectionCredentials deepCopyCredentials = new ConnectionCredentials();

            deepCopyCredentials.ConnectionPassword = string.Empty; //WARNING: setting password to empty as it does not matter
            //userInfo.Identity.Name
            deepCopyCredentials.ConnectionUser = userInfo.user.Account.ToString();//WARNING: name of the incoming user, but will not be used as we are using integrated security
            deepCopyCredentials.Location = credentials.Location;
            //[END] SQLdm 10.0 (Gaurav Karwal): this was done to avoid changing the original connection object due to which an error was coming

            repositoryInformation = deepCopyCredentials.Location.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            repositoryHost = repositoryInformation[0];
            repositoryDatabase = repositoryInformation[1];
            //SQLdm 10.0: line below commented by Gaurav Karwal as we are now using the credentials of incoming user only
            //deepCopyCredentials.ConnectionPassword = EncryptionHelper.QuickDecryptFromCWF(deepCopyCredentials.ConnectionPassword);
            //checking if the sql conn info is there and if it is there checking if the conn credentials are diff than the ones already there
            RestServiceConfiguration.SetInstanceName(repositoryHost);

            //SqlConnectionInfo repoConnInfo = new SqlConnectionInfo(repositoryHost, repositoryDatabase, deepCopyCredentials.ConnectionUser, deepCopyCredentials.ConnectionPassword);
            SqlConnectionInfo repoConnInfo = new SqlConnectionInfo(repositoryHost, repositoryDatabase);
            repoConnInfo.UserName = userInfo.user.Account.ToString();
            repoConnInfo.UseIntegratedSecurity = true;//since CWF supports only windows credentials for now
            RestServiceConfiguration.SetRepositoryConnectInfo(repoConnInfo); //resetting the connection credentials

            RefreshUserToken(userInfo.user.Account.ToString(), repoConnInfo);//SQLdm 10.0 (Swati Gogia): Refresh the permissions assigned to the connected user.
        }

        /// <summary>
        ///SQLdm 10.0 (Swati Gogia): Refresh the permissions assigned to the connected user.
        /// </summary>
        public void RefreshUserToken(string userName, SqlConnectionInfo repoConnInfo)
        {
            using (LOG.DebugCall("RefreshUserToken"))
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                try
                {
                    if (repoConnInfo.UseIntegratedSecurity)
                    {
                        try
                        {
                            userToken.RefreshForWebUI(repoConnInfo.UseIntegratedSecurity, userName, repoConnInfo.ConnectionString);
                        }
                        catch
                        {
                            // Ignore this exception, we don't have windows groups at this point.
                        }
                    }
                }
                catch (Exception e)
                {
                    LOG.Error("Error updating user token from the repository. ", e);
                }
                finally
                {
                    stopWatch.Stop();
                    LOG.InfoFormat("RefreshPermissionToken took {0} milliseconds.",
                                   stopWatch.ElapsedMilliseconds);
                }
            }
        }




        /// <summary>
        /// SQLdm 9.0 (Gaurav Karwal): getting the product id from the CWF request
        /// </summary>
        private string GetProductIdFromRequest()
        {
            //string absolutePath = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.AbsolutePath;
            //string[] parts = absolutePath.Split('/');
            //string productID = parts.Length>=3? parts[2]:string.Empty;//assuming that the third entry will be the product id. while registration of the product, we need to make sure product id is at the third position
            //return productID;
            return RegistryHelper.GetValueFromRegistry("ProductID") == null ? "0" : RegistryHelper.GetValueFromRegistry("ProductID").ToString();

        }

        /// <summary>
        /// OBSOLETE: This will not be used after this service becomes a plugin into the CWF
        /// Load DB configuration Data 
        /// </summary>
        /// <param name="instanceName">Instance Name</param>
        /// <param name="databaseName">Database Name</param>
        public static void LoadTestDBConfigurations(string instanceName, string databaseName)
        {
#if DEBUG
            RestServiceConfiguration.SetRepositoryConnectInfo(new SqlConnectionInfo(instanceName, databaseName));
#endif
        }
        /* CWF Team - Backend Isolation - SQLDM-29086
         * Changing the parameter type from IPrincipal to Principal */
        private List<Idera.SQLdm.Common.CWFDataContracts.GlobalTag> GetGlobalTagsFromCWF(Principal userInfo)
        {
            string productID = GetProductIdFromRequest();
            var header = WebOperationContext.Current.IncomingRequest.Headers.Get("Authorization");
            /* CWF Team - Backend Isolation - SQLDM-29086
             * Calling CWF API of Get Tags */
            //List<Idera.SQLdm.Common.CWFDataContracts.GlobalTag> globalTags = CWFHelper.ObjectTranslator.TranslateTags(_myHost.GetTags(productID, userInfo));
            List<Idera.SQLdm.Common.CWFDataContracts.GlobalTag> globalTags = ObjectTranslator.TranslateTags(CWFHelper.GetTags(header));

            if (globalTags != null && globalTags.Count > 0)
            {
                foreach (Idera.SQLdm.Common.CWFDataContracts.GlobalTag gtag in globalTags)
                {
                    /* CWF Team - Backend Isolation - SQLDM-29086
                     * Calling CWF API of Get Tag Instances */
                    //List<Idera.SQLdm.Common.CWFDataContracts.Instance> tagInstances = CWFHelper.ObjectTranslator.TranslateToCWFContract(_myHost.GetTagInstances(productID, gtag.ID.ToString(), userInfo));
                    List<Idera.SQLdm.Common.CWFDataContracts.Instance> tagInstances = ObjectTranslator.TranslateToCWFContract(CWFHelper.GetTagInstances(gtag.ID.ToString(), header));
                    gtag.Instances = new List<string>();
                    if (tagInstances != null && tagInstances.Count > 0)
                    {
                        foreach (Idera.SQLdm.Common.CWFDataContracts.Instance instance in tagInstances)
                        {
                            gtag.Instances.Add(instance.Name);
                        }
                    }
                }
            }

            return globalTags;
        }

        /// <summary>
        /// SQLdm9.0 (Gaurav): Initiating the web service object with the host parameter
        /// </summary>
        /// <param name="myHost"></param>
        public WebService(HostObject myHost)
        {
            _myHost = myHost;
            _eventLog = new EventLog("Application");
            _eventLog.Source = CoreSettings.BaseServiceName;
            _eventLog.Log = CoreSettings.BaseServiceName + "log";
        }

        /// <summary>
        /// SQLdm 9.0 (Gaurav): Maintaining the constructor with zero arguments
        /// </summary>
        public WebService()
        {
            _eventLog = new EventLog("Application");
            _eventLog.Source = CoreSettings.BaseServiceName;
            _eventLog.Log = CoreSettings.BaseServiceName + "log";
        }
    }

    public class DateTimeInspectorAttribute : Attribute, IParameterInspector, IOperationBehavior
    {
        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
        }

        public object BeforeCall(string operationName, object[] inputs)
        {
            float timeOffset;

            if (inputs != null && inputs.Length > 0)
            {
                float.TryParse((string)inputs[0], out timeOffset); //assuming the first parameter will always be TimeZoneOffset
                Math.Round(timeOffset, 2);

                if ((Convert.ToInt32(Math.Abs(timeOffset)) < 0) || (Convert.ToInt32(Math.Abs(timeOffset)) > 23))  // return null, if the timeZoneOffset is not valid
                    return null;

                IList<DateTime> dateList = new List<DateTime>(2);
                for (int i = 0; i < inputs.Length - 1; i++)
                {
                    if (inputs[i] == null)
                    {
                        continue;
                    }
                    if (inputs[i].GetType() == typeof(DateTime))
                    {
                        // Set Default Values
                        // case 1: start = null and end = null
                        if ((DateTime)inputs[i] == default(DateTime) && (DateTime)inputs[i + 1] == default(DateTime))
                        {
                            inputs[i] = DateTime.Now.AddDays(-680).AddHours(timeOffset);
                            inputs[i + 1] = DateTime.Now.AddHours(timeOffset);
                            inputs[i + 1] = ((DateTime)inputs[i + 1]) > DateTime.Now ? DateTime.Now : (DateTime)inputs[i + 1]; // make sure max value is <= current date 
                        }
                        // case 2: start != null and end == null
                        else if ((DateTime)inputs[i] != default(DateTime) && (DateTime)inputs[i + 1] == default(DateTime))
                        {
                            inputs[i] = ((DateTime)inputs[i]).AddHours(timeOffset);
                            inputs[i + 1] = ((DateTime)inputs[i]).AddDays(7);
                            inputs[i + 1] = ((DateTime)inputs[i + 1]) > DateTime.Now ? DateTime.Now : (DateTime)inputs[i + 1]; // make sure max value is <= current date 
                        }
                        // case 3: start == null and end != null
                        else if ((DateTime)inputs[i] == default(DateTime) && (DateTime)inputs[i + 1] != default(DateTime))
                        {
                            inputs[i + 1] = ((DateTime)inputs[i + 1]).AddHours(timeOffset);
                            inputs[i] = ((DateTime)inputs[i + 1]).AddDays(-7);
                            inputs[i + 1] = ((DateTime)inputs[i + 1]) > DateTime.Now ? DateTime.Now : (DateTime)inputs[i + 1]; // make sure max value is <= current date 
                        }
                        // case 4: start != null and end != null
                        else if ((DateTime)inputs[i] != default(DateTime) && (DateTime)inputs[i + 1] != default(DateTime))
                        {
                            inputs[i] = ((DateTime)inputs[i]).AddHours(timeOffset);
                            inputs[i + 1] = ((DateTime)inputs[i + 1]).AddHours(timeOffset);
                            inputs[i + 1] = ((DateTime)inputs[i + 1]) > DateTime.Now ? DateTime.Now : ((DateTime)inputs[i + 1]); // make sure max value is <= current date 
                        }

                        // Basic validations after setting Default values
                        if ((DateTime)inputs[i + 1] > DateTime.Now)
                            throw new FaultException("EndDate cannot be greater than current Date");
                        if ((DateTime)inputs[i] > DateTime.Now)
                            throw new FaultException("StartDate cannot be greater than current Date");
                        if ((DateTime)inputs[i] > (DateTime)inputs[i + 1])
                            throw new FaultException("StartDate cannot be greater than EndDate");

                        return null;
                    }
                }
            }

            return null;
        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.ParameterInspectors.Add(this);
        }

        public void Validate(OperationDescription operationDescription)
        {
        }
    }

    public class WebHttpBehaviorEx : WebHttpBehavior
    {

        protected override void AddServerErrorHandlers(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            // clear default erro handlers.
            endpointDispatcher.ChannelDispatcher.ErrorHandlers.Clear();

            // add our own error handler.
            endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(new ErrorHandler());
        }

        // this is needed if request param is a csv. Ex usage: ?status=active,suspended,sleeping
        //protected override QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
        //{
        //    return new ArrayQueryStringConverter();
        //}
    }

    public class ArrayQueryStringConverter : QueryStringConverter
    {
        public override bool CanConvert(Type type)
        {
            if (type.IsArray)
            {
                return base.CanConvert(type.GetElementType());
            }
            else
            {
                return base.CanConvert(type);
            }
        }

        public override object ConvertStringToValue(string parameter, Type parameterType)
        {
            if (parameter == null) return null;
            if (parameterType.IsArray)
            {
                Type elementType = parameterType.GetElementType();
                string[] parameterList = parameter.Split(',');
                Array result = Array.CreateInstance(elementType, parameterList.Length);
                for (int i = 0; i < parameterList.Length; i++)
                {
                    result.SetValue(base.ConvertStringToValue(parameterList[i], elementType), i);
                }

                return result;
            }
            else
            {
                return base.ConvertStringToValue(parameter, parameterType);
            }
        }
    }
}