using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Net;
using BBS.TracerX;
using Idera.SQLdm.CWFRegister.Objects;
using Idera.SQLdm.Common;
using System.Text.RegularExpressions;
using Idera.SQLdm.Common.Helpers;
using Idera.SQLdm.Common.Security.Encryption;

namespace Idera.SQLdm.CWFRegister.Helpers
{
    class CWFCommHelper
    {
        static Logger Log = Logger.GetLogger("CWFCommHelper");


        public static void UnregisterProduct(CWFDetails cwfDetails)
        {
            if (cwfDetails.ProductID > Constants.NOT_REGISTERED)
            {
                using (Log.InfoCall("UnregisterProduct"))
                {
                    string postURL = cwfDetails.BaseURL + Constants.CWF_BASE_URI + "/Products/" + cwfDetails.ProductID;
                    var request = (HttpWebRequest)WebRequest.Create(postURL);

                    request.Method = "DELETE";
                    request.ContentLength = 0;
                    request.ContentType = "application/json";
                    string header = "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(Regex.Replace(cwfDetails.UserName + ":" + cwfDetails.Password, "\\\\", "\\")));
                    request.Headers.Add(HttpRequestHeader.Authorization, header);
                    var encoding = new UTF8Encoding();
                    var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes("");
                    request.ContentLength = bytes.Length;

                    Log.Info("Initialize request");
                    Log.InfoFormat("Request DELETE, URL = {0}", postURL);
                    Log.DebugFormat("Request Authorization = {0}", header);

                    using (var writeStream = request.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        var responseValue = string.Empty;
                        Log.InfoFormat("StatusCode = {0}", response.StatusCode);
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                            Log.Warn(message);
                            throw new ApplicationException(message);
                        }

                        // grab the response
                        Log.DebugFormat("Response ContentLength = {0}", response.ContentLength);
                        if (response.ContentLength > 0)
                        {
                            using (var responseStream = response.GetResponseStream())
                            {
                                if (responseStream != null)
                                    using (var reader = new StreamReader(responseStream))
                                    {
                                        responseValue = reader.ReadToEnd();
                                    }
                            }
                        }
                    }
                }
            }
        }
        
        public static int RegisterProduct(CWFDetails cwfDetailsIn, string repoHost, string repoDb)
        {
            using (Log.InfoCall("RegisterProduct"))
            {
                Log.InfoFormat("Starting register of product, InstanceName = {0}, DatabaseName = {1}", repoHost, repoDb);
                Dictionary<string, object> registrationData = GetDataToRegister(cwfDetailsIn, repoHost, repoDb);

                // Create request and receive response
                string postURL = cwfDetailsIn.BaseURL + Constants.CWF_BASE_URI + Constants.CWF_REGISTRATION_URI;
                Log.Debug("Register product");

                CWFDetails cwfInfo = MakeRequest(postURL, registrationData, cwfDetailsIn);
                Log.Debug("Add widgets");

                AddDashboardWidgets(cwfInfo.RESTuri, cwfDetailsIn);
                Log.InfoFormat("Product registered successfully, ProductID = {0}", cwfInfo.ProductID);

                return (cwfInfo.ProductID);
            }
        }

        private static void AddDashboardWidgets(string restURI, CWFDetails cwfDetails)
        {
            using(Log.InfoCall("AddDashboardWidgets"))
            {
                string json = "";
                //string url = cwfDetails.BaseURL + restURI + Constants.CWF_ADDDASHBOARD_WIDGET_URI;
                string url = restURI + Constants.CWF_ADDDASHBOARD_WIDGET_URI;
                SendPostData(url, json, cwfDetails);
            }
        }

        private static CWFDetails MakeRequest(string postURL, Dictionary<string, object> postParameters, CWFDetails cwfDetails)
        {
            using (Log.InfoCall("MakeRequest"))
            {
                // Create request and receive response
                CWFDetails cwfInfo = new CWFDetails();
                string userAgent = "";

                HttpWebResponse webResponse = FormUpload.MultipartFormDataPost(postURL, userAgent, postParameters, cwfDetails.UserName, cwfDetails.Password);
                Log.InfoFormat("StatusCode = {0}", webResponse.StatusCode);

                // Process response
                StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
                string fullResponse = responseReader.ReadToEnd();
                //LogX.Info("Product has been Upgraded/Registered");

                Product product = JsonHelper.FromJSON<Product>(fullResponse);
                cwfInfo.ProductID = Constants.NOT_REGISTERED;

                if (product.Id > 0)
                {
                    cwfInfo.ProductID = product.Id;
                    cwfInfo.RESTuri = product.RestURL;
                    Log.DebugFormat("Id of product registered={0}", product.Id);
                    Log.DebugFormat("RestURL of product registered={0}", product.RestURL);
                }   

                webResponse.Close();
                /* CWF Team - Backend Isolation - SQLDM-29086
                 * Updating Registry with Dashboard details to be picked to call CWF API */
                GenericHelpers.UpdateRegistryForAuthTokenAndRefreshToken(product.AuthToken, product.RefreshToken);
                RegistryHelper.SetValueInRegistry("Version",product.Version);
                RegistryHelper.SetValueInRegistry("DashboardHost", cwfDetails.HostName);
                RegistryHelper.SetValueInRegistry("DashboardPort",cwfDetails.Port.ToString());
                RegistryHelper.SetValueInRegistry("DashboardAdministrator", cwfDetails.UserName);
                RegistryHelper.SetValueInRegistry("ProductID", product.Id.ToString());
                RegistryHelper.SetValueInRegistry("DisplayName", product.InstanceName);

                return cwfInfo;
            }
        }

        public static bool RegisteredWithCWF(CWFDetails cwfDetails)
        {
            if (cwfDetails.ProductID == Constants.NOT_REGISTERED)
                return false;

            Version prodVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			Constants.PRODUCT_VERSION = prodVer.Major + "." + prodVer.Minor + "." + prodVer.Build + ".0";

            string URL;
            if (string.IsNullOrEmpty(cwfDetails.DisplayName))
            {
                URL = String.Format(cwfDetails.BaseURL + Constants.CWF_BASE_URI + "/Products/{0}?version={1}", Constants.PRODUCT_SHORT_NAME, RegistryHelper.GetValueFromRegistry("Version", Idera.SQLdm.Common.Constants.REGISTRY_PATH).ToString());
            }
            else
            {
                URL = String.Format(cwfDetails.BaseURL + Constants.CWF_BASE_URI + "/Products/{0}?version={1}&instancename={2}", Constants.PRODUCT_SHORT_NAME, RegistryHelper.GetValueFromRegistry("Version", Idera.SQLdm.Common.Constants.REGISTRY_PATH).ToString(), cwfDetails.DisplayName);
            }

            using (Log.InfoCall("RegisteredWithCWF"))
            {
                string response = GetRequest(URL, cwfDetails.UserName, cwfDetails.Password);
                Products products = JsonHelper.FromJSON<Products>(response);

                //there are no SQLdms registered
                Log.DebugFormat("SQLdms registered with CWF = {0}", products.Count);

                if (products.Count == 0)
                {
                    return false;
                }

                foreach (Product product in products)
                {
                    if (product.Id == cwfDetails.ProductID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

#region HTTP GET
        private static string GetRequest(string url, string username, string password)
        {
            using (Log.InfoCall("GetRequest"))
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentLength = 0;
                request.ContentType = "application/json";

                string header = "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(Regex.Replace(username + ":" + password, "\\\\", "\\")));
                request.Headers.Add(HttpRequestHeader.Authorization, header);
                Log.InfoFormat("Request GET, URL = {0}", url);
                Log.DebugFormat("Request header = {0}", header);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var responseValue = string.Empty;
                    Log.InfoFormat("StatusCode = {0}", response.StatusCode);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                        Log.Warn(message);
                        throw new ApplicationException(message);
                    }

                    // grab the response
                    Log.DebugFormat("Response ContentLength = {0}", response.ContentLength);
                    if (response.ContentLength > 0)
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            if (responseStream != null)
                            {
                                using (var reader = new StreamReader(responseStream))
                                {
                                    responseValue = reader.ReadToEnd();
                                }
                            }
                        }
                    }
                    return responseValue;
                }
            }
        }

#endregion 


#region HTTP POST
        /// <summary>
        /// This method returns the JSON response in strin format given an API URL path 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static string SendPostData(string url, String postData, CWFDetails cwfDetails)
        {
            using(Log.InfoCall("SendPostData"))
            {
                var request = CreateRequest(url, "POST", cwfDetails);
                Log.InfoFormat("Request POST, URL={0}", url);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    //initiate the request

                    streamWriter.Write(postData);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    Log.InfoFormat("StatusCode = {0}", response.StatusCode);

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                        Log.Warn(message);
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
        }

        /// <summary>
        /// This method returns an HttpWebRequest object given an API URL path 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static HttpWebRequest CreateRequest(string url, string method, CWFDetails cwfDetails)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            //request.ContentLength = 
            request.ContentType = "application/json";
            //string username = serviceAdminUser;
            //string password = serviceAdminPassword;

            string username = cwfDetails.UserName;
            string password = cwfDetails.Password;

            string header = String.Empty;
            if (!(String.IsNullOrEmpty(username) && String.IsNullOrEmpty(password)))
                header = "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(Regex.Replace(username + ":" + password, "\\\\", "\\")));

            request.Headers.Add(HttpRequestHeader.Authorization, header);
            return request;
        } 

#endregion

#region REST Data
        private static Dictionary<string, object> GetDataToRegister(CWFDetails cwfDetails, string repoHost, string repoDb)
        {
            using(Log.DebugCall("GetDataToRegister"))
            {
                FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + Constants.REGISTRATION_PACKAGE_NAME + "." + Constants.REGISTRATION_PACKAGE_EXTENSION, FileMode.Open, FileAccess.Read);
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                Log.DebugFormat("Size of CWFPackage.zip = {0}", fs.Length);
                fs.Close();

            	Version prodVer = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
				Constants.PRODUCT_VERSION = prodVer.Major + "." + prodVer.Minor + "." + prodVer.Build + ".0";
                // Generate post objects
                Dictionary<string, object> postParameters = new Dictionary<string, object>();
                postParameters.Add("Name", Constants.PRODUCT_NAME);//keeping the product name same as product short name as of now.
                postParameters.Add("ShortName", Constants.PRODUCT_SHORT_NAME);
                postParameters.Add("Version", RegistryHelper.GetValueFromRegistry("Version", Idera.SQLdm.Common.Constants.REGISTRY_PATH).ToString());//the version is the same as the version of the management service
                postParameters.Add("Status", Constants.PRODUCT_STATUS);
                postParameters.Add("Location", String.Join(";", new string[] { repoHost, repoDb }));
                //postParameters.Add("ConnectionUser", RegistryHelper.GetValueFromRegistry("ServiceAccount").ToString());
                //postParameters.Add("ConnectionPassword", RegistryHelper.GetValueFromRegistry("ServicePassword").ToString());
                postParameters.Add("ConnectionUser", EncryptionHelper.QuickDecrypt(RegistryHelper.GetValueFromRegistry("ServiceAccount").ToString()));
                postParameters.Add("ConnectionPassword", EncryptionHelper.QuickDecrypt(RegistryHelper.GetValueFromRegistry("ServicePassword").ToString()));

                postParameters.Add("productserviceshost", Environment.MachineName);
                postParameters.Add("productservicesport", Common.Constants.REGISTRATION_SERVICE_PORT_DEFAULT.ToString());
                postParameters.Add("RegisteringUser", System.Security.Principal.WindowsIdentity.GetCurrent().Name); //setting the name of the user under which the management service is running under
                postParameters.Add("WebURL", "/" + Constants.PRODUCT_WEB_UI_URI);
                postParameters.Add("RestURL", Idera.SQLdm.Common.Constants.SQLdmRestBaseURL);
                postParameters.Add("JarFile", Constants.PRODUCT_JAR_PLUGIN + Constants.PRODUCT_VERSION + "." + Constants.PRODUCT_JAR_PLUGIN_EXTENSION); //tight coupling the version and the jar file name
                postParameters.Add("Description", Constants.PRODUCT_DESCRIPTION);
                postParameters.Add("DefaultPage", Constants.PRODUCT_WEB_UI_BASE_PAGE);
                postParameters.Add("InstanceName", cwfDetails.DisplayName);
                postParameters.Add("RestFile", Constants.PRODUCT_REST_API_PLUGIN + "." + Constants.PRODUCT_REST_API_PLUGIN_EXTENSION);
                postParameters.Add("IsTaggable", 1); //SQLdm 10.1 (GK): making the product taggable during registration so that tags can be added
                postParameters.Add("product", new FormUpload.FileParameter(data, Constants.REGISTRATION_PACKAGE_NAME + "." + Constants.REGISTRATION_PACKAGE_EXTENSION, "application/x-zip-compressed"));
                //Commenting these parameters to adddress SQLDM-29504 - These params required in case of CWE version 4.6. Not with 4.4
                postParameters.Add("IsSelfHosted", true);
                postParameters.Add("IsWarEnabled", true);
                postParameters.Add("WarFileName", Constants.PRODUCT_WAR_PLUGIN + Constants.PRODUCT_VERSION_JAR + "." + Constants.PRODUCT_WAR_PLUGIN_EXTENSION);

                //SQLdm10.1(Srishti Purohit)
                //Defect SQLCORE-2406 fix
                //Assigning port and host while installing dm and registering it with dashboard at same time
                postParameters.Add("productserviceshost", Environment.MachineName);
                postParameters.Add("productservicesport", Common.Constants.REGISTRATION_SERVICE_PORT_DEFAULT.ToString());

                Log.DebugFormat("Location = {0}", postParameters["Location"]);
                Log.DebugFormat("ConnectionUser = {0}", postParameters["ConnectionUser"]);
                Log.DebugFormat("RegisteringUser = {0}", postParameters["RegisteringUser"]);
                Log.DebugFormat("InstanceName = {0}", postParameters["InstanceName"]);

                return postParameters;
            }
        }

        /// <summary>
        /// Gets the list of all the widgets to be registered on Idera Dashboard
        /// </summary>
        /// <returns></returns>
        private static IList<DashboardWidget> GetDashboardWidgetsList()
        {
            IList<DashboardWidget> allDashboardWidgets = new List<DashboardWidget>();

            //SQLdm 9.1 (Gaurav Karwal): Overall status widget - shows up on the Overview tab on Idera dashboard
            DashboardWidget overallStatusWidget = new DashboardWidget();
            overallStatusWidget.Name = "Overall Status | SQLdm";
            overallStatusWidget.Type = "Product Status";
            overallStatusWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            overallStatusWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/overallStatusWidget.zul";
            overallStatusWidget.DataURI = "/ProductStatus";
            overallStatusWidget.Description = "Provides an Overview of the total alerts";
            overallStatusWidget.Version = "9.1.0.0";
            overallStatusWidget.Settings = new Dictionary<string, string>();
            overallStatusWidget.DefaultViews = "Overview";
            overallStatusWidget.Collapsed = true;

            //SQLdm 9.1 (Gaurav Karwal): Instance status widget - shows up on the Overview tab on Idera dashboard
            DashboardWidget instanceStatusWidget = new DashboardWidget();
            instanceStatusWidget.Name = "Instance Status | SQLdm";
            instanceStatusWidget.Type = "Instance Status";
            instanceStatusWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            instanceStatusWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instanceStatusWidget.zul";
            instanceStatusWidget.DataURI = "/InstanceStatus";
            instanceStatusWidget.Description = "Provides an overview of the instance at each level";
            instanceStatusWidget.Version = "9.1.0.0";
            instanceStatusWidget.Settings = new Dictionary<string, string>();
            instanceStatusWidget.DefaultViews = "Overview";
            instanceStatusWidget.Collapsed = true;

            //SQLdm 9.1 (Gaurav Karwal): Top Instances by Alert Count widget - shows up on the Details tab on Idera dashboard
            DashboardWidget topInstancesbyAlertCountWidget = new DashboardWidget();
            topInstancesbyAlertCountWidget.Name = "SQLdm – Top Instances by Alert Count";
            topInstancesbyAlertCountWidget.Type = "Top X";
            topInstancesbyAlertCountWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            topInstancesbyAlertCountWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesByAlertsCountWidget.zul";
            topInstancesbyAlertCountWidget.DataURI = "/Instances/ByAlerts";
            topInstancesbyAlertCountWidget.Description = "List top instances by alert count.";
            topInstancesbyAlertCountWidget.Version = "9.1.0.0";
            Dictionary<string, string> settingTopInstanceWidgets = new Dictionary<string, string>();
            settingTopInstanceWidgets.Add("Limit", "10");//for limiting the records to 10 by default
            topInstancesbyAlertCountWidget.Settings = settingTopInstanceWidgets;
            topInstancesbyAlertCountWidget.DefaultViews = "Details";
            topInstancesbyAlertCountWidget.Collapsed = true;

            //SQLdm 9.1 (Gaurav Karwal): Top Databases by Alert Count widget - shows up on the Details tab on Idera dashboard
            DashboardWidget topDatabasesByAlertCountWidget = new DashboardWidget();
            topDatabasesByAlertCountWidget.Name = "SQLdm – Top Databases by Alert Counts";
            topDatabasesByAlertCountWidget.Type = "Top X";
            topDatabasesByAlertCountWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            topDatabasesByAlertCountWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/databasesByAlertsCountWidget.zul";
            topDatabasesByAlertCountWidget.DataURI = "/Instances/Databases/ByAlerts";
            topDatabasesByAlertCountWidget.Description = "List top databases by alert counts.";
            topDatabasesByAlertCountWidget.Version = "9.1.0.0";
            Dictionary<string, string> settingTopDatabasesByAlertWidgets = new Dictionary<string, string>();
            settingTopDatabasesByAlertWidgets.Add("Limit", "10");//for limiting the records to 10 by default
            topDatabasesByAlertCountWidget.Settings = settingTopDatabasesByAlertWidgets;

            topDatabasesByAlertCountWidget.DefaultViews = "Details";
            topDatabasesByAlertCountWidget.Collapsed = true;

            //SQLdm 9.1 (Gaurav Karwal): Top Instances by CPU Usage widget - shows up on the Details tab on Idera dashboard
            DashboardWidget topInstanceByCPUUsageWidget = new DashboardWidget();
            topInstanceByCPUUsageWidget.Name = "SQLdm – Top Instances by CPU Usage";
            topInstanceByCPUUsageWidget.Type = "Top X";
            topInstanceByCPUUsageWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            topInstanceByCPUUsageWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesByCpuUsageWidget.zul";
            topInstanceByCPUUsageWidget.DataURI = "/Instances/BySqlCpuLoad";
            topInstanceByCPUUsageWidget.Description = "List top instances by CPU usage.";
            topInstanceByCPUUsageWidget.Version = "9.1.0.0";

            Dictionary<string, string> settingTopInstanceByCPUUsageWidget = new Dictionary<string, string>();
            settingTopInstanceByCPUUsageWidget.Add("Limit", "10");//for limiting the records to 10 by default
            topInstanceByCPUUsageWidget.Settings = settingTopInstanceByCPUUsageWidget;

            topInstanceByCPUUsageWidget.DefaultViews = "Details";
            topInstanceByCPUUsageWidget.Collapsed = true;

            //SQLdm 9.1 (Gaurav Karwal): Alert Count By Category widget - shows up on the Details tab on Idera dashboard
            DashboardWidget alertCountsByCategoryWidget = new DashboardWidget();
            alertCountsByCategoryWidget.Name = "SQLdm – Alert Counts by Category";
            alertCountsByCategoryWidget.Type = "Top X";
            alertCountsByCategoryWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            alertCountsByCategoryWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/alertCountsByCategory.zul";
            alertCountsByCategoryWidget.DataURI = "/numAlertsByCategory";
            alertCountsByCategoryWidget.Description = "Shows alert counts by alert category.";
            alertCountsByCategoryWidget.Version = "9.1.0.0";

            Dictionary<string, string> settingAlertCountsByCategoryWidget = new Dictionary<string, string>();
            settingAlertCountsByCategoryWidget.Add("Limit", "10");//for limiting the records to 10 by default
            alertCountsByCategoryWidget.Settings = settingAlertCountsByCategoryWidget;

            alertCountsByCategoryWidget.DefaultViews = "Details";
            alertCountsByCategoryWidget.Collapsed = true;

            //SQLdm 9.1 (Gaurav Karwal): List of Active Alerts widget - shows up on the Overview tab on Idera dashboard
            DashboardWidget activeAlertListWidget = new DashboardWidget();
            activeAlertListWidget.Name = "SQLdm – Active Alerts List";
            activeAlertListWidget.Type = "Top X";
            activeAlertListWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            activeAlertListWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/alertsListWidget.zul";
            activeAlertListWidget.DataURI = "/AlertsForWebConsole";
            activeAlertListWidget.Description = "List of Active Alerts";
            activeAlertListWidget.Version = "9.1.0.0";

            Dictionary<string, string> settingActiveAlertListWidget = new Dictionary<string, string>();
            settingActiveAlertListWidget.Add("Limit", "10");//for limiting the records to 10 by default
            activeAlertListWidget.Settings = settingActiveAlertListWidget;

            activeAlertListWidget.DefaultViews = "Overview";
            activeAlertListWidget.Collapsed = true;

            //SQLdm 9.1 (Gaurav Karwal): List of Instances widget - shows up on the Overview tab on Idera dashboard
            DashboardWidget instanceListWidget = new DashboardWidget();
            instanceListWidget.Name = "SQLdm – Instances List";
            instanceListWidget.Type = "Top X";
            instanceListWidget.NavigationLink = "/SQLDM/{InstanceName}/index";
            instanceListWidget.PackageURI = "/sqldm/com/idera/sqldm/widgets/instancesListWidget.zul";
            instanceListWidget.DataURI = "/Instances";
            instanceListWidget.Description = "List of Instances";
            instanceListWidget.Version = "9.1.0.0";

            Dictionary<string, string> settingInstanceListWidget = new Dictionary<string, string>();
            settingInstanceListWidget.Add("Limit", "10");//for limiting the records to 10 by default
            instanceListWidget.Settings = settingInstanceListWidget;

            instanceListWidget.DefaultViews = "Overview";
            instanceListWidget.Collapsed = true;

            //adding all widges to the list
            allDashboardWidgets.Add(overallStatusWidget);
            allDashboardWidgets.Add(instanceStatusWidget);
            allDashboardWidgets.Add(topInstancesbyAlertCountWidget);
            allDashboardWidgets.Add(topDatabasesByAlertCountWidget);
            allDashboardWidgets.Add(topInstanceByCPUUsageWidget);
            allDashboardWidgets.Add(alertCountsByCategoryWidget);
            allDashboardWidgets.Add(activeAlertListWidget);
            allDashboardWidgets.Add(instanceListWidget);

            return allDashboardWidgets;
        }

#endregion

    }

    public class DashboardWidget
    {
        public string Name { get; set; }


        public string Type { get; set; }


        public string NavigationLink { get; set; }


        public string PackageURI { get; set; }


        public string DataURI { get; set; }


        public string Description { get; set; }


        public string Version { get; set; }


        public Dictionary<string, string> Settings { get; set; }


        public string DefaultViews { get; set; }


        public Boolean Collapsed { get; set; }

    }
}
