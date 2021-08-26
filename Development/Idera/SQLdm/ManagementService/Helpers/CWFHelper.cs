using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Idera.SQLdm.ManagementService.Configuration;
using System.Runtime.Serialization;
using Idera.SQLdm.Common.CWFDataContracts;
using Newtonsoft.Json.Converters;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Security.Encryption;

namespace Idera.SQLdm.ManagementService.Helpers
{
    //SQLdm 9.0 (Ankit Srivastava) -- CWF Synchronization -  New helper class for calling CWF APIs
    class CWFHelper
    {
        #region Fields
        private static readonly Logger Log = Logger.GetLogger("ManagementService_CWFHelper");
        //private string serviceAdminUser;
        //private string serviceAdminPassword;
        //private string CORE_SERVICE_DEFAULT_BASE_URL = "http://localhost:9292";
        private CommonWebFramework cwfInfo = null;
        private const string REGISTRATION_PACKAGE_NAME = "CWFPackage";
        private const string REGISTRATION_PACKAGE_EXTENSION = "zip";
        private const string PRODUCT_NAME = "SQLdm";//DO NOT CHANGE THE CASE: very important to keep it this case.
        private const string PRODUCT_SHORT_NAME = "SQLdm";//DO NOT CHANGE THE CASE: very important to keep it this case.
        private const string PRODUCT_WEB_UI_URI = "sqldm"; //DO NOT CHANGE THE CASE: very important to keep it small case.
        private readonly string PRODUCT_VERSION = string.Empty;
		private readonly string PRODUCT_VERSION_JAR = string.Empty;//SQLDM10.2.1 To change product version in jarFile common of CWF registered product table
        private const string PRODUCT_STATUS = "Green";
        private const string PRODUCT_REST_API_PLUGIN = "SQLdmPlugin";
        private const string PRODUCT_REST_API_PLUGIN_EXTENSION = "dll";
        private const string PRODUCT_JAR_PLUGIN = "idera-sqldm_cwf_product_widgets-";
        private const string PRODUCT_JAR_PLUGIN_EXTENSION = "jar";
        private const string PRODUCT_WAR_PLUGIN = "idera-sqldm-";
        private const string PRODUCT_WAR_PLUGIN_EXTENSION = "war";
        private const string CWF_BASE_URI = "/IderaCoreServices/v1";
        private const string CWF_REGISTRATION_URI = "/RegisterProduct/";
        private const string CWF_PRODUCT_DETAIL_URI_PATTERN = "/Products/{0}?version={1}";
        private const string CWF_UNREGISTRATION_URI = "/Products";
        private const string PRODUCT_DESCRIPTION = "SQL Diagnostic Manager";
        private const string PRODUCT_WEB_UI_BASE_PAGE = "home";
        private const string PRODUCT_INSTANCES_URI = "/GetAllProductInstances";
        private const string GLOBAL_TAGS_URI = "/Tags/Global"; // SQLdm 10.1 - Praveen Suhalka - CWF Integration
        private const string INSTANCES_URI_PATTERN = "/Instances?name={0}"; // SQLdm 10.1 - Srishti Purohit - CWF Integration

        #endregion

        /// <summary>
        /// parametrized constructor
        /// </summary>
        /// <param name="serviceAdminUser"></param>
        /// <param name="serviceAdminPassword"></param>
        /// <param name="coreServiceURL"></param>
        public CWFHelper(CommonWebFramework cwfDetail)
        {
            cwfInfo = cwfDetail;
            Version prodVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            PRODUCT_VERSION = prodVer.Major + "." + prodVer.Minor + "." + prodVer.Build + ".0";
			PRODUCT_VERSION_JAR = prodVer.Major + "." + prodVer.Minor + "." + prodVer.Build + ".0";//SQLDM10.2.1 To change product version in jarFile common of CWF registered product table
        }

        public void RefreshCWFDetails(CommonWebFramework cwfDets) 
        {
            cwfInfo = cwfDets;
        }

        #region Private Methods
        private string GetData(string url) 
        {
            HttpWebRequest request = CreateRequest(url, "GET");
            Log.Info("requesting for url" + url);
            HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();

            // Processing and saving response
            StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
            string fullResponse = responseReader.ReadToEnd();
            webResponse.Close();
            return fullResponse;
        }
        /// <summary>
        /// This method returns the JSON response in strin format given an API URL path 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool SendDELETEData(string url)
        {
            var request = CreateRequest(url, "DELETE");
            bool _deleteSuccess = false;
            var responseValue = string.Empty;
            Log.Info("requesting for url" + url + " for DELETE");
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Log.Error(String.Format("Request failed. Received HTTP {0}", response.StatusCode));
                        //throw new ApplicationException(message);
                        _deleteSuccess = false;
                    }
                    else if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        _deleteSuccess = true ;
                    }
                }
            }
            catch (Exception ex)
            {
                //if (ex.Message.Equals("The remote server returned an error: (403) Forbidden.", StringComparison.CurrentCultureIgnoreCase))
                //    return "Authentication Error";
                if (ex.Message.Contains("400"))
                {
                    _deleteSuccess = true;
                }
                else
                {
                    Log.Error("Exception thrown in SendDELETEData: ", ex);
                    _deleteSuccess = false;
                }
            }
            finally
            {
                request = null;
            }
            return _deleteSuccess;

        }
        /// <summary>
        /// This method returns the JSON response in strin format given an API URL path 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string SendPostData(string url, String postData)
        {
            var request = CreateRequest(url,"POST");
            var responseValue = string.Empty;
            Log.Info("requesting for url" + url + " for POST." + "POST DATA: " + postData);
            try
            {
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    //initiate the request
                    
                    streamWriter.Write(postData);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                        throw new ApplicationException(message);
                    }

                    // grab the response
                    var httpResponse = (HttpWebResponse)request.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("The remote server returned an error: (403) Forbidden.", StringComparison.CurrentCultureIgnoreCase))
                    return "Authentication Error";
                Log.Error("Exception thrown in GetResponse for url " + url + ": ", ex);
            }
            finally
            {
                request = null;
            }
            return responseValue;

        }

        /// <summary>
        /// This method checks if the response received by given API URL path is OK (200)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool CheckIfResponseIsOK(string url)
        {
            var request = CreateRequest(url,"POST");

            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                return (response != null && response.StatusCode == HttpStatusCode.OK);

            }
            catch (WebException ex)
            {
                Log.Warn(" The response was not OK because: ", ex);
                return false;// need not log erro because the exception is expected

            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown in CheckIfResponseIsOK: ", ex);
            }
            finally
            {
                request = null;
            }
            return false;

        }

        /// <summary>
        /// This method returns an HttpWebRequest object given an API URL path 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private HttpWebRequest CreateRequest(string url, string method)
        {
            try
            {
                Log.Info("creating request for url" + url);
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method;
                //request.ContentLength = 
                request.ContentType = "application/json";
                //string username = serviceAdminUser;
                //string password = serviceAdminPassword;

                string username = cwfInfo.UserName;
                string password = cwfInfo.DecryptedPassword;

                string header=String.Empty;
                if(!(String.IsNullOrEmpty(username) && String.IsNullOrEmpty(password)))
                    header= "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(Regex.Replace(username + ":" + password, "\\\\", "\\")));

                request.Headers.Add(HttpRequestHeader.Authorization, header);
                return request;
            }
            catch (Exception ex)
            {
                Log.Error("Exception thrown in CreateRequest: ", ex);
            }
            return null;
        } 
        #endregion


		//[START] - SQLdm 9.0 (Ankit Srivastava) - CWF Integration - Modified the add alert sync method
        internal void AddAlerts(IList<Alert> alertsList)
        {
            string json = JsonHelper.ToJSON<IList<Alert>>(alertsList);
            //string url = cwfInfo.BaseURL + cwfInfo.RestURI + "/SyncAlerts";
            string url = cwfInfo.RestURI + "/SyncAlerts";
            SendPostData(url, json);
        }
		//[END] - SQLdm 9.0 (Ankit Srivastava) - CWF Integration - Modified the add alert sync method

		//[START] - SQLdm 9.0 (Ankit Srivastava) - CWF Integration - Modified the add instance sync method
        internal void AddInstances(IList<Instance> instances)
        {
            string json = JsonHelper.ToJSON<IList<Instance>>(instances);
            //string url = cwfInfo.BaseURL + cwfInfo.RestURI + "/SyncInstances";
            string url = cwfInfo.RestURI + "/SyncInstances";
            SendPostData(url, json);
        }
		//[END] - SQLdm 9.0 (Ankit Srivastava) - CWF Integration - Modified the add instance sync method

		//[START] - SQLdm 9.0 (Ankit Srivastava) - CWF Integration - Added the add users sync method
        internal void AddUsers(IList<User> users)
        {
            string json = JsonHelper.ToJSON<IList<User>>(users);
            //string url = cwfInfo.BaseURL + cwfInfo.RestURI + "/SyncUsers";
            string url = cwfInfo.RestURI + "/SyncUsers";
            SendPostData(url, json);
        }
		//[END] - SQLdm 9.0 (Ankit Srivastava) - CWF Integration - Added the add users sync method

        //[START] - SQLdm 9.1 (Gaurav Karwal) - CWF Integration - to add widgets to idera dashboard
        internal void AddDashboardWidgets()
        {
            string json = "";
            //string url = cwfInfo.BaseURL + cwfInfo.RestURI + "/AddDashboardWidgets";
            string url = cwfInfo.RestURI + "/AddDashboardWidgets";
            SendPostData(url, json);
        }
        //[END] - SQLdm 9.1 (Gaurav Karwal) - CWF Integration - to add widgets to idera dashboard

        /// <summary>
        /// SQLdm 9.0 (Gaurav Karwal): registers the product with CWF
        /// </summary>
        /// <returns></returns>
        internal void RegisterProduct() 
        {
            using (Log.InfoCall("RegisterProduct"))
            {
            
                try
                {
                    Dictionary<string, object> registrationData = GetDataToRegister();
                    if (registrationData != null && registrationData.Count > 0)
                    {
                        string postURL = cwfInfo.BaseURL + CWF_BASE_URI + CWF_REGISTRATION_URI;
                        GetResponse(postURL, registrationData);
                        //assuming the product registration went through successfully, registering all the widgets
                        //if registration went through successfully, error in the addition of widgets should not affect registration flow 
                        try
                        {
                            AddDashboardWidgets();
                        }
                        catch (Exception ex_widgets)
                        {
                            Log.Error("Error while registering the widgets:" + ex_widgets.Message + (ex_widgets.InnerException != null ? ex_widgets.InnerException.Message : ""));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
                    throw ex;
                }
            
            }
            
        }

        /// <summary>
        /// Gets the list of all the widgets to be registered on Idera Dashboard
        /// </summary>
        /// <returns></returns>
        //private IList<DashboardWidget> GetDashboardWidgetsList() 
        //{
        //    IList<DashboardWidget> allDashboardWidgets = new List<DashboardWidget>();

        //    //SQLdm 9.1 (Gaurav Karwal): Overall status widget - shows up on the Overview tab on Idera dashboard
        //    DashboardWidget overallStatusWidget = new DashboardWidget();
        //    overallStatusWidget.Name = "Overall Status | SQLdm";
        //    overallStatusWidget.Type = "Product Status";
        //    overallStatusWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
        //    overallStatusWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/overallStatusWidget.zul";
        //    overallStatusWidget.DataURI = "/ProductStatus";
        //    overallStatusWidget.Description = "Provides an Overview of the total alerts";
        //    overallStatusWidget.Version = "9.1.0.0";
            
        //    Dictionary<string, string> overallStatusWidgetSettings = new Dictionary<string, string>();
        //    overallStatusWidgetSettings.Add("Limit", "10");//for limiting the records to 10 by default
        //    overallStatusWidget.Settings = overallStatusWidgetSettings;

        //    overallStatusWidget.DefaultViews = "Overview";
        //    overallStatusWidget.Collapsed = true;

        //    //SQLdm 9.1 (Gaurav Karwal): Instance status widget - shows up on the Overview tab on Idera dashboard
        //    DashboardWidget instanceStatusWidget = new DashboardWidget();
        //    instanceStatusWidget.Name = "Instance Status | SQLdm";
        //    instanceStatusWidget.Type = "Instance Status";
        //    instanceStatusWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
        //    instanceStatusWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instanceStatusWidget.zul";
        //    instanceStatusWidget.DataURI = "/InstanceStatus";
        //    instanceStatusWidget.Description = "Provides an overview of the instance at each level";
        //    instanceStatusWidget.Version = "9.1.0.0";
        //    Dictionary<string, string> instanceStatusWidgetSettings = new Dictionary<string, string>();
        //    instanceStatusWidgetSettings.Add("Limit", "10");//for limiting the records to 10 by default
        //    instanceStatusWidget.Settings = instanceStatusWidgetSettings;

        //    instanceStatusWidget.Settings = new Dictionary<string, string>();
        //    instanceStatusWidget.DefaultViews = "Overview";
        //    instanceStatusWidget.Collapsed = true;

        //    //SQLdm 9.1 (Gaurav Karwal): Top Instances by Alert Count widget - shows up on the Details tab on Idera dashboard
        //    DashboardWidget topInstancesbyAlertCountWidget = new DashboardWidget();
        //    topInstancesbyAlertCountWidget.Name = "SQLDM – Top Instances by Alert Count";
        //    topInstancesbyAlertCountWidget.Type = "Top X";
        //    topInstancesbyAlertCountWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
        //    topInstancesbyAlertCountWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesByAlertsCountWidget.zul";
        //    topInstancesbyAlertCountWidget.DataURI = "/Instances/ByAlerts";
        //    topInstancesbyAlertCountWidget.Description = "List top instances by alert count.";
        //    topInstancesbyAlertCountWidget.Version = "9.1.0.0";
        //    Dictionary<string, string> settingTopInstanceWidgets = new Dictionary<string, string>();
        //    settingTopInstanceWidgets.Add("Limit", "10");//for limiting the records to 10 by default
        //    topInstancesbyAlertCountWidget.Settings = settingTopInstanceWidgets;
        //    topInstancesbyAlertCountWidget.DefaultViews = "Details";
        //    topInstancesbyAlertCountWidget.Collapsed = true;

        //    //SQLdm 9.1 (Gaurav Karwal): Top Databases by Alert Count widget - shows up on the Details tab on Idera dashboard
        //    DashboardWidget topDatabasesByAlertCountWidget = new DashboardWidget();
        //    topDatabasesByAlertCountWidget.Name = "SQLDM – Top Databases by Alert Counts";
        //    topDatabasesByAlertCountWidget.Type = "Top X";
        //    topDatabasesByAlertCountWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
        //    topDatabasesByAlertCountWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/databasesByAlertsCountWidget.zul";
        //    topDatabasesByAlertCountWidget.DataURI = "/Instances/Databases/ByAlerts";
        //    topDatabasesByAlertCountWidget.Description = "List top databases by alert counts.";
        //    topDatabasesByAlertCountWidget.Version = "9.1.0.0";
        //    Dictionary<string, string> settingTopDatabasesByAlertWidgets = new Dictionary<string, string>();
        //    settingTopDatabasesByAlertWidgets.Add("Limit", "10");//for limiting the records to 10 by default
        //    topDatabasesByAlertCountWidget.Settings = settingTopDatabasesByAlertWidgets;
            
        //    topDatabasesByAlertCountWidget.DefaultViews = "Details";
        //    topDatabasesByAlertCountWidget.Collapsed = true;

        //    //SQLdm 9.1 (Gaurav Karwal): Top Instances by CPU Usage widget - shows up on the Details tab on Idera dashboard
        //    DashboardWidget topInstanceByCPUUsageWidget = new DashboardWidget();
        //    topInstanceByCPUUsageWidget.Name = "SQLDM – Top Instances by CPU Usage";
        //    topInstanceByCPUUsageWidget.Type = "Top X";
        //    topInstanceByCPUUsageWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
        //    topInstanceByCPUUsageWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesByCpuUsageWidget.zul";
        //    topInstanceByCPUUsageWidget.DataURI = "/Instances/BySqlCpuLoad";
        //    topInstanceByCPUUsageWidget.Description = "List top instances by CPU usage.";
        //    topInstanceByCPUUsageWidget.Version = "9.1.0.0";
            
        //    Dictionary<string, string> settingTopInstanceByCPUUsageWidget = new Dictionary<string, string>();
        //    settingTopInstanceByCPUUsageWidget.Add("Limit", "10");//for limiting the records to 10 by default
        //    topInstanceByCPUUsageWidget.Settings = settingTopInstanceByCPUUsageWidget;

        //    topInstanceByCPUUsageWidget.DefaultViews = "Details";
        //    topInstanceByCPUUsageWidget.Collapsed = true;

        //    //SQLdm 9.1 (Gaurav Karwal): Alert Count By Category widget - shows up on the Details tab on Idera dashboard
        //    DashboardWidget alertCountsByCategoryWidget = new DashboardWidget();
        //    alertCountsByCategoryWidget.Name = "SQLDM – Alert Counts by Category";
        //    alertCountsByCategoryWidget.Type = "Top X";
        //    alertCountsByCategoryWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
        //    alertCountsByCategoryWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/alertCountsByCategory.zul";
        //    alertCountsByCategoryWidget.DataURI = "/numAlertsByCategory";
        //    alertCountsByCategoryWidget.Description = "Shows alert counts by alert category.";
        //    alertCountsByCategoryWidget.Version = "9.1.0.0";

        //    Dictionary<string, string> settingAlertCountsByCategoryWidget = new Dictionary<string, string>();
        //    settingAlertCountsByCategoryWidget.Add("Limit", "10");//for limiting the records to 10 by default
        //    alertCountsByCategoryWidget.Settings = settingAlertCountsByCategoryWidget;
            
        //    alertCountsByCategoryWidget.DefaultViews = "Details";
        //    alertCountsByCategoryWidget.Collapsed = true;

        //    //SQLdm 9.1 (Gaurav Karwal): List of Active Alerts widget - shows up on the Overview tab on Idera dashboard
        //    DashboardWidget activeAlertListWidget = new DashboardWidget();
        //    activeAlertListWidget.Name = "SQLDM – Active Alerts List";
        //    activeAlertListWidget.Type = "Top X";
        //    activeAlertListWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
        //    activeAlertListWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/alertsListWidget.zul";
        //    activeAlertListWidget.DataURI = "/AlertsForWebConsole";
        //    activeAlertListWidget.Description = "List of Active Alerts";
        //    activeAlertListWidget.Version = "9.1.0.0";

        //    Dictionary<string, string> settingActiveAlertListWidget = new Dictionary<string, string>();
        //    settingActiveAlertListWidget.Add("Limit", "10");//for limiting the records to 10 by default
        //    activeAlertListWidget.Settings = settingActiveAlertListWidget;

        //    activeAlertListWidget.DefaultViews = "Overview";
        //    activeAlertListWidget.Collapsed = true;

        //    //SQLdm 9.1 (Gaurav Karwal): List of Instances widget - shows up on the Overview tab on Idera dashboard
        //    DashboardWidget instanceListWidget = new DashboardWidget();
        //    instanceListWidget.Name = "SQLDM – Instances List";
        //    instanceListWidget.Type = "Top X";
        //    instanceListWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
        //    instanceListWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesListWidget.zul";
        //    instanceListWidget.DataURI = "/Instances";
        //    instanceListWidget.Description = "List of Instances";
        //    instanceListWidget.Version = "9.1.0.0";

        //    Dictionary<string, string> settingInstanceListWidget = new Dictionary<string, string>();
        //    settingInstanceListWidget.Add("Limit", "10");//for limiting the records to 10 by default
        //    instanceListWidget.Settings = settingInstanceListWidget;

        //    instanceListWidget.DefaultViews = "Overview";
        //    instanceListWidget.Collapsed = true;

        //    //adding all widges to the list
        //    allDashboardWidgets.Add(overallStatusWidget);
        //    allDashboardWidgets.Add(instanceStatusWidget);
        //    allDashboardWidgets.Add(topInstancesbyAlertCountWidget);
        //    allDashboardWidgets.Add(topDatabasesByAlertCountWidget);
        //    allDashboardWidgets.Add(topInstanceByCPUUsageWidget);
        //    allDashboardWidgets.Add(alertCountsByCategoryWidget);
        //    allDashboardWidgets.Add(activeAlertListWidget);
        //    allDashboardWidgets.Add(instanceListWidget);

        //    return allDashboardWidgets;
        //}
        /// <summary>
        /// SQLdm 9.0 (Gaurav Karwal): unregisters the product with CWF
        /// </summary>
        /// <returns></returns>
        internal bool UnRegisterProduct(int productId)
        {
            bool _retValue = false;
            using (Log.InfoCall("UnRegisterProduct"))
            {   
                if (productId > 0)
                {
                    string postURL = cwfInfo.BaseURL + CWF_BASE_URI + CWF_UNREGISTRATION_URI + "/" + productId.ToString();
                    _retValue = SendDELETEData(postURL);
                    if (_retValue == true)
                        Log.Info("product with product id:" + productId.ToString() + " has been successfully unregistered");
                    else Log.Error("product unregistration did not go through successfully");
                }

                return _retValue;
                    //GetResponse(postURL, registrationData);
                
            }

        }

        /// <summary>
        /// SQLdm 9.0 (Gaurav Karwal): gets the latest product data
        /// </summary>
        /// <returns></returns>
        internal void LoadProductDetails(int productId)
        {
            using (Log.InfoCall("GetProductDetails"))
            {
                if (productId > 0)
                {
                    string postURL = cwfInfo.BaseURL + CWF_BASE_URI + String.Format(CWF_PRODUCT_DETAIL_URI_PATTERN,PRODUCT_SHORT_NAME,Idera.SQLdm.Common.Helpers.RegistryHelper.GetValueFromRegistry("Version").ToString(),cwfInfo.RegisteredInstanceName);
                    postURL += (string.IsNullOrEmpty(cwfInfo.RegisteredInstanceName)!=true)? string.Format("&instancename={0}", cwfInfo.RegisteredInstanceName):string.Empty;

                    string productDetails = GetData(postURL);
                    
                    if (!String.IsNullOrEmpty(productDetails))
                    {
                        Log.Info("product data returned" + productDetails);
                        Products listProducts = JsonHelper.FromJSON<Products>(productDetails);
                        listProducts.ForEach(prod => 
                        {
                            Log.Info("product with product id:" + prod.Id.ToString() + " has been found on CWF");
                            if (prod.Id == productId) 
                            {
                                cwfInfo.LoadNonPersistentInfo(cwfInfo.ProductID, prod.WebURL, prod.RestURL);
                                Log.Info("product with product id:" + prod.Id.ToString() + " has been found and matched on CWF");
                            }
                        });
                        
                    }
                    else
                    {
                        Log.Error("product was not found");
                    }
                }
                //GetResponse(postURL, registrationData);

            }

        }

        /// <summary>
        /// SQLdm 9.0 (Gaurav Karwal): sending the request and getting response from CWF
        /// </summary>
        /// <param name="postURL"></param>
        /// <param name="postParameters"></param>
        private void GetResponse(string postURL, Dictionary<string, object> postParameters)
        {
            // Create request and receive response

            Log.Info("requesting for url" + postURL);
            string userAgent = "";
            HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(postURL, userAgent, postParameters, cwfInfo.UserName, cwfInfo.DecryptedPassword);

            
            // Processing and saving response
            StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
            string fullResponse = responseReader.ReadToEnd();
            
            Product product = JsonHelper.FromJSON<Product>(fullResponse);
            
            if(product.Id > 0) cwfInfo.Save(product.Id,product.WebURL,product.RestURL,product.InstanceName);
            Idera.SQLdm.Common.Helpers.RegistryHelper.SetValueInRegistry("Version", product.Version);
            Idera.SQLdm.Common.Helpers.RegistryHelper.SetValueInRegistry("DashboardHost", cwfInfo.Host);
            Idera.SQLdm.Common.Helpers.RegistryHelper.SetValueInRegistry("DashboardPort", cwfInfo.Port);
            Idera.SQLdm.Common.Helpers.RegistryHelper.SetValueInRegistry("DashboardAdministrator", cwfInfo.UserName);
            Idera.SQLdm.Common.Helpers.RegistryHelper.SetValueInRegistry("ProductID", product.Id.ToString());
            Idera.SQLdm.Common.Helpers.RegistryHelper.SetValueInRegistry("DisplayName", product.InstanceName);
            webResponse.Close();
        }

        /// <summary>
        /// SQLdm 9.0 (Gaurav Karwal): Getting the data for registration
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, object> GetDataToRegister()
        {
            FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + REGISTRATION_PACKAGE_NAME + "." + REGISTRATION_PACKAGE_EXTENSION, FileMode.Open, FileAccess.Read);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();

            // Generate post objects
            Dictionary<string, object> postParameters = new Dictionary<string, object>();
            postParameters.Add("Name", PRODUCT_NAME);//keeping the product name same as product short name as of now.
            postParameters.Add("ShortName", PRODUCT_SHORT_NAME);
            postParameters.Add("Version", Idera.SQLdm.Common.Helpers.RegistryHelper.GetValueFromRegistry("Version", Idera.SQLdm.Common.Constants.REGISTRY_PATH).ToString());//the version is the same as the version of the management service
            postParameters.Add("Status", PRODUCT_STATUS);
            postParameters.Add("Location", String.Join(";", new string[] {ManagementServiceConfiguration.RepositoryHost,ManagementServiceConfiguration.RepositoryDatabase}));
            //postParameters.Add("ConnectionUser", Idera.SQLdm.Common.Helpers.RegistryHelper.GetValueFromRegistry("ServiceAccount").ToString());
            //postParameters.Add("ConnectionPassword", Idera.SQLdm.Common.Helpers.RegistryHelper.GetValueFromRegistry("ServicePassword").ToString());
            postParameters.Add("ConnectionUser", EncryptionHelper.QuickDecrypt(Idera.SQLdm.Common.Helpers.RegistryHelper.GetValueFromRegistry("ServiceAccount").ToString()));
            postParameters.Add("ConnectionPassword", EncryptionHelper.QuickDecrypt(Idera.SQLdm.Common.Helpers.RegistryHelper.GetValueFromRegistry("ServicePassword").ToString()));

            postParameters.Add("RegisteringUser", System.Security.Principal.WindowsIdentity.GetCurrent().Name); //setting the name of the user under which the management service is running under
            postParameters.Add("WebURL", "/" + PRODUCT_WEB_UI_URI);
            postParameters.Add("RestURL", Idera.SQLdm.Common.Constants.SQLdmRestBaseURL);
            postParameters.Add("productserviceshost", Environment.MachineName);
            postParameters.Add("productservicesport", Common.Constants.REGISTRATION_SERVICE_PORT_DEFAULT.ToString());
            postParameters.Add("JarFile", PRODUCT_JAR_PLUGIN + PRODUCT_VERSION_JAR + "." + PRODUCT_JAR_PLUGIN_EXTENSION); //tight coupling the version and the jar file name
            postParameters.Add("Description", PRODUCT_DESCRIPTION);
            postParameters.Add("DefaultPage", PRODUCT_WEB_UI_BASE_PAGE);
            postParameters.Add("InstanceName", cwfInfo.RegisteredInstanceName);
            postParameters.Add("RestFile", PRODUCT_REST_API_PLUGIN + "." + PRODUCT_REST_API_PLUGIN_EXTENSION);
            postParameters.Add("IsTaggable", 1); //SQLdm 10.1 (GK): making the product taggable during registration so that tags can be added
            postParameters.Add("product", new FormUpload.FileParameter(data, REGISTRATION_PACKAGE_NAME + "." + REGISTRATION_PACKAGE_EXTENSION, "application/x-zip-compressed"));
            //Commenting these parameters to adddress SQLDM-29504 - These params required in case of CWE version 4.6. Not with 4.4
            postParameters.Add("IsSelfHosted", true);
            postParameters.Add("IsWarEnabled", true);
            postParameters.Add("WarFileName", PRODUCT_WAR_PLUGIN + PRODUCT_VERSION_JAR + "." + PRODUCT_WAR_PLUGIN_EXTENSION);
            return postParameters;
        }

        /// <summary>
        /// SQLdm 9.0 (Abhishek Joshi): get all the instances from the cwf
        /// </summary>
        /// <returns></returns>
        internal List<Instance> GetAllProductInstancesFromCWF()
        {
            using (Log.InfoCall("GetAllProductInstances"))
            {
                //string getURL = cwfInfo.BaseURL + cwfInfo.RestURI + PRODUCT_INSTANCES_URI;
                string getURL = cwfInfo.RestURI + PRODUCT_INSTANCES_URI;
                string instancesDetails = GetData(getURL);

                List<Instance> cwfInstancesList = null;

                if (!String.IsNullOrEmpty(instancesDetails))
                {
                    cwfInstancesList = JsonHelper.FromJSON<List<Instance>>(instancesDetails);
                    Log.Info("Instances List is successfully retrieved from the CWF");
                }

                return cwfInstancesList;
            }
        }

        //Start: SQLdm 10.1 - (Praveen Suhalka) - CWF Integration - get global tags
        internal ICollection<Idera.SQLdm.Common.CWFDataContracts.GlobalTag> GetGlobalTags()
        {

            //string getURL = cwfInfo.BaseURL + cwfInfo.RestURI + GLOBAL_TAGS_URI;
            string getURL = cwfInfo.RestURI + GLOBAL_TAGS_URI;
            string tagsDetails = GetData(getURL);

            ICollection<Idera.SQLdm.Common.CWFDataContracts.GlobalTag> tags = null;

            if (!string.IsNullOrEmpty(tagsDetails))
            {
                tags = JsonHelper.FromJSON<List<Idera.SQLdm.Common.CWFDataContracts.GlobalTag>>(tagsDetails);
            }

            return tags;
        }
        //End: SQLdm 10.1 - (Praveen Suhalka) - CWF Integration - get global tags
        #region SWALaunch Info
        public int GetSWAProductId(string instanceName)
        {
            int SWA_prodId = 0;
            string url = cwfInfo.BaseURL + CWF_BASE_URI + string.Format(INSTANCES_URI_PATTERN, instanceName);
            try
            {
                HttpWebRequest request = CreateRequest(url, "GET");
                Log.InfoFormat("requesting SWA ID for instance : {0} and url used : {1}.", instanceName, url);
                //HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();

                //// Processing and saving response
                //StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
                //string fullResponse = responseReader.ReadToEnd();
                //webResponse.Close();
                string instanceDetails = GetData(url);

                if (!String.IsNullOrEmpty(instanceDetails))
                {
                    Log.Info("product data returned" + instanceDetails);
                    List<Instance> instances = JsonHelper.FromJSON<List<Instance>>(instanceDetails);
                    instances[0].Products.ForEach(prod =>
                    {
                        Log.Info("product with product id:" + prod.Id.ToString() + " has been found on CWF.");
                        if (prod.Name == "SQLWorkloadAnalysis")
                        {
                            Log.Info("product with product id:" + prod.Id.ToString() + " has been found .");
                            SWA_prodId = prod.Id;
                        }
                    });

                }
                else
                {
                    Log.Error("product was not found");
                }
            }
            catch(Exception ex)
            {
                Log.ErrorFormat("Exception occured in GetSWAProductId: {0}, for url : {1}.", ex, url);
            }
            return SWA_prodId;
        }
        #endregion

    }

    
    /// <summary>
    /// for serializing and desirializing json
    /// </summary>
    public static class JsonHelper
    {
        private static Logger LOG = Logger.GetLogger("JsonHelper");

        public static T FromJSON<T>(string json) where T : class
        {
            
            if (string.IsNullOrEmpty(json)) return (null);
            try
            {   
                return (JsonConvert.DeserializeObject<T>(json) as T);
            }
            catch (Exception ex)
            {
                LOG.Error(ex.Message + ex.InnerException != null ? ex.InnerException.Message : "");
                //ExceptionLogger.Log(string.Format("JsonHelper.FromJSON<{0}>({1})", typeof(T), json), ex);
                return (null);
            }
            
        }

        public static string ToJSON<T>(T obj) where T : class
        {
            if (obj==null) return (string.Empty);
            try
            {
			//[START] - SQLdm 9.0 (Ankit Srivastava) - CWF Integration - modified the defauls datetime format
                JsonSerializerSettings customJsonSettings = new JsonSerializerSettings()
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                    DateTimeZoneHandling= DateTimeZoneHandling.Utc
                };
                return JsonConvert.SerializeObject(obj, customJsonSettings);
			//[END] - SQLdm 9.0 (Ankit Srivastava) - CWF Integration - modified the defauls datetime format
            }
            catch (Exception ex)
            {
                //ExceptionLogger.Log(string.Format("JsonHelper.FromJSON<{0}>({1})", typeof(T), json), ex);
                return (null);
            }
        }

    }
    /// <summary>
    /// SQLdm 9.0: Gaurav Karwal: Adding the form upload class to enable upload of the form multipart data
    /// </summary>
    public static class FormUpload
    {
        private static readonly Logger LogX = Logger.GetLogger("RestClient");
        private static readonly Encoding encoding = Encoding.UTF8;
        public static HttpWebResponse MultipartFormDataPost(string postUrl, string userAgent, Dictionary<string, object> postParameters, string username, string password)
        {
            string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;

            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

            return PostForm(postUrl, userAgent, contentType, formData, username, password);
        }
        private static HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData, string username, string password)
        {
            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            // Set up the request properties.
            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            // You could add authentication here as well if needed:
            // request.PreAuthenticate = true;
            // request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
            request.Accept = "application/json";
            string header = "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(Regex.Replace(username + ":" + password, "\\\\", "\\")));
            request.Headers.Add("Authorization", header);

            // Send the form data to the request.
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;

        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                // Add a CRLF to allow multiple parameters to be added.
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsCLRF)
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                if (param.Value is FileParameter)
                {
                    FileParameter fileToUpload = (FileParameter)param.Value;

                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName ?? param.Key,
                        fileToUpload.ContentType ?? "application/octet-stream");

                    formDataStream.Write(encoding.GetBytes(header), 0, encoding.GetByteCount(header));

                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(fileToUpload.File, 0, fileToUpload.File.Length);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }

        public class FileParameter
        {
            public byte[] File { get; set; }
            public string FileName { get; set; }
            public string ContentType { get; set; }
            public FileParameter(byte[] file) : this(file, null) { }
            public FileParameter(byte[] file, string filename) : this(file, filename, null) { }
            public FileParameter(byte[] file, string filename, string contenttype)
            {
                File = file;
                FileName = filename;
                ContentType = contenttype;
            }
        }
    }
}
