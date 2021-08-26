using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Idera.SQLdm.Common.CWFDataContracts;
using Newtonsoft.Json.Converters;
using Idera.SQLdm.RegistrationService.Configuration;
using Idera.SQLdm.Common.Security.Encryption;
using Idera.SQLdm.Common.Helpers;

namespace Idera.SQLdm.RegistrationService.Helpers
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
        private readonly string PRODUCT_VERSION_JAR = string.Empty;//SQLDM10.1.1 (Srishti Purohit)To change product version in jarFile common of CWF registered product table
        private const string PRODUCT_STATUS = "Green";
        private const string PRODUCT_REST_API_PLUGIN = "SQLdmPlugin";
        private const string PRODUCT_REST_API_PLUGIN_EXTENSION = "dll";
        private const string PRODUCT_JAR_PLUGIN = "idera-sqldm_cwf_product_widgets-";
        private const string PRODUCT_WAR_PLUGIN = "idera-sqldm-";
        private const string PRODUCT_WAR_PLUGIN_EXTENSION = "war";
        private const string PRODUCT_JAR_PLUGIN_EXTENSION = "jar";
        private const string CWF_BASE_URI = "/IderaCoreServices/v1";
        private const string CWF_REGISTRATION_URI = "/RegisterProduct/";
        private const string CWF_PRODUCT_DETAIL_URI_PATTERN = "/Products/{0}?version={1}";
        private const string CWF_UNREGISTRATION_URI = "/Products";
        private const string PRODUCT_DESCRIPTION = "SQL Diagnostic Manager";
        private const string PRODUCT_WEB_UI_BASE_PAGE = "home";
        private const string PRODUCT_INSTANCES_URI = "/GetAllProductInstances";

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
            PRODUCT_VERSION_JAR = prodVer.Major + "." + prodVer.Minor + "." + prodVer.Build + ".0";//SQLDM10.1.1 (Srishti Purohit)To change product version in jarFile common of CWF registered product table
        }

        public void RefreshCWFDetails(CommonWebFramework cwfDets) 
        {
            cwfInfo = cwfDets;
        }

        #region Private Methods
        /// <summary>
        /// This method returns the JSON response in string format given an API URL path 
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
        /// This method returns an HttpWebRequest object given an API URL path 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private  HttpWebRequest CreateRequest(string url, string method)
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

        //SQLdm 10.2 (Tushar)--FIx for SQLDM-28150
        private HttpWebRequest CreateRequest(string url, string method, string userName, string passsword)
        {
            try
            {
                Log.Info("creating request for url" + url);
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method;
                //request.ContentLength = 
                request.ContentType = "application/json";
                
                string username = userName;
                string password = passsword;

                string header = String.Empty;
                if (!(String.IsNullOrEmpty(username) && String.IsNullOrEmpty(password)))
                    header = "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(Regex.Replace(username + ":" + password, "\\\\", "\\")));

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
        /// //SQLdm 10.1 - Praveen Suhalka - CWF3 integration
        /// </summary>
        /// <param name="notify"></param>
        internal void SetDashboardLocation(PluginCommon.NotifyProduct notify)
        {
            if (notify.IsRegistered) 
            {
               // cwfInfo = CommonWebFramework.GetInstance();
                if (cwfInfo.IsSaved &&(cwfInfo.Host.ToLower()!=notify.Host.ToLower()))
                {
                    // unregister from the old one
                    if (isProductUnregistered(notify))
                    {
                        UnRegisterProduct(cwfInfo.ProductID);
                    }
                }

                // save new CWF properties in repo
                //cwfInfo = CommonWebFramework.GetInstance(notify.Host, notify.Port.ToString(), notify.User, EncryptionHelper.QuickEncrypt(notify.Password), notify.DisplayName);
                //SQLdm 10.2 (Tushar)--Fix for SQLDM-28150
                LoadProductDetails(notify);
                RegistryHelper.SetValueInRegistry("DashboardHost", notify.Host);
                RegistryHelper.SetValueInRegistry("DashboardPort", notify.Port.ToString());
                RegistryHelper.SetValueInRegistry("DashboardAdministrator", notify.User);
                RegistryHelper.SetValueInRegistry("ProductID", notify.ProductID.ToString());
                RegistryHelper.SetValueInRegistry("DisplayName", notify.DisplayName);
                cwfInfo.LoadCriticalInfo(notify.Host, notify.Port.ToString(), notify.User, EncryptionHelper.QuickEncrypt(notify.Password), notify.DisplayName);
                cwfInfo.Save(notify.ProductID, cwfInfo.WebURI, cwfInfo.RestURI, notify.DisplayName);
                //SQLdm 10.1 (Pulkit Puri)
                //SQLDM -26242 FIX (Pulkit Puri)
                //AddDashboardWidgets();
            }
            else
            {
                // remove the old CWF properties from repo
                RepositoryHelper.DeleteTheRegistrationInformation();
            }
        }

        /// <summary>
        /// //SQLdm 10.1 - Praveen Suhalka - CWF3 integration
        /// </summary>
        /// <returns></returns>
        internal byte[] GetProductData()
        {
            Dictionary<string, object> postParameters = GetDataToRegisterForCWF();
            return FormUpload.GetMultiPartFormDataForResponse(postParameters);
        }

        /// <summary>
        /// //SQLdm 10.1 - Praveen Suhalka - CWF3 integration
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, object> GetDataToRegisterForCWF()
        {
            FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + REGISTRATION_PACKAGE_NAME + "." + REGISTRATION_PACKAGE_EXTENSION, FileMode.Open, FileAccess.Read);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            fs.Close();

            // Generate post objects
            Dictionary<string, object> postParameters = new Dictionary<string, object>();
            postParameters.Add("Name", PRODUCT_NAME);//keeping the product name same as product short name as of now.
            postParameters.Add("ShortName", PRODUCT_SHORT_NAME);
            postParameters.Add("Version", RegistryHelper.GetValueFromRegistry("Version", Idera.SQLdm.Common.Constants.REGISTRY_PATH).ToString());//the version is the same as the version of the management service
            postParameters.Add("Status", PRODUCT_STATUS);
            postParameters.Add("Location", String.Join(";", new string[] { RegistrationServiceConfiguration.RepositoryHost, RegistrationServiceConfiguration.RepositoryDatabase }));
            postParameters.Add("ConnectionUser", RegistryHelper.GetValueFromRegistry("ServiceAccount").ToString());
            postParameters.Add("ConnectionPassword", RegistryHelper.GetValueFromRegistry("ServicePassword").ToString());
            postParameters.Add("RegisteringUser", System.Security.Principal.WindowsIdentity.GetCurrent().Name); //setting the name of the user under which the management service is running under
            postParameters.Add("WebURL", "/" + PRODUCT_WEB_UI_URI);
            postParameters.Add("RestURL", Idera.SQLdm.Common.Constants.SQLdmRestBaseURL);
            postParameters.Add("JarFile", PRODUCT_JAR_PLUGIN + PRODUCT_VERSION_JAR + "." + PRODUCT_JAR_PLUGIN_EXTENSION); //tight coupling the version and the jar file name
            postParameters.Add("Description", PRODUCT_DESCRIPTION);
            postParameters.Add("productserviceshost", Environment.MachineName);
            postParameters.Add("productservicesport", Common.Constants.REGISTRATION_SERVICE_PORT_DEFAULT.ToString());
            postParameters.Add("DefaultPage", PRODUCT_WEB_UI_BASE_PAGE);
            //postParameters.Add("InstanceName", cwfInfo.RegisteredInstanceName);
            postParameters.Add("IsTaggable", 1); //SQLdm 10.1 (GK): making the product taggable during registration so that tags can be added
            postParameters.Add("RestFile", PRODUCT_REST_API_PLUGIN + "." + PRODUCT_REST_API_PLUGIN_EXTENSION);
            postParameters.Add("product", new FormUpload.FileParameter(data, REGISTRATION_PACKAGE_NAME + "." + REGISTRATION_PACKAGE_EXTENSION, "application/x-zip-compressed"));
            //Commenting these parameters to adddress SQLDM-29504 - These params required in case of CWE version 4.6. Not with 4.4
            postParameters.Add("IsSelfHosted", true);
            postParameters.Add("IsWarEnabled", true);
            postParameters.Add("WarFileName", PRODUCT_WAR_PLUGIN + PRODUCT_VERSION_JAR + "." + PRODUCT_WAR_PLUGIN_EXTENSION);
            return postParameters;
        }
        /// <summary>
        /// This method returns the JSON response in string format given an API URL path 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string SendPostData(string url, String postData)
        {
            var request = CreateRequest(url, "POST");
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

        internal void AddDashboardWidgets()
        {
            Log.ErrorFormat("Inside AddDashboardWidgets Method");
            if (cwfInfo.BaseURL != null && cwfInfo.RestURI != null)
            {
                string json = "";
                //string url = cwfInfo.BaseURL + cwfInfo.RestURI + "/AddDashboardWidgets";
                string url = cwfInfo.RestURI + "/AddDashboardWidgets";
                SendPostData(url, json);
            }

        }
        /// <summary>
        /// SQLdm 9.0 (Gaurav Karwal): gets the latest product data
        /// </summary>
        /// <returns></returns>
        //SQLdm 10.2 (Tushar)--fix for SQLDM-28150
        internal void LoadProductDetails(PluginCommon.NotifyProduct notify)
        {
            using (Log.InfoCall("GetProductDetails"))
            {
                if (notify.ProductID > 0)
                {
                    string postURL = "http://" + notify.Host + ":" + notify.Port + CWF_BASE_URI + String.Format(CWF_PRODUCT_DETAIL_URI_PATTERN, PRODUCT_SHORT_NAME, RegistryHelper.GetValueFromRegistry("Version").ToString(), notify.DisplayName/* cwfInfo.RegisteredInstanceName*/);
                    postURL += (string.IsNullOrEmpty(notify.DisplayName) != true) ? string.Format("&instancename={0}", notify.DisplayName) : string.Empty;

                    string productDetails = GetData(postURL, notify);

                    if (!String.IsNullOrEmpty(productDetails))
                    {
                        Log.Info("product data returned " + productDetails);
                        Products listProducts = JsonHelper.FromJSON<Products>(productDetails);
                        listProducts.ForEach(prod =>
                        {
                            Log.Info("product with product id:" + prod.Id.ToString() + " has been found on CWF");
                            if (prod.Id == notify.ProductID)
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

        internal bool isProductUnregistered(PluginCommon.NotifyProduct notify)
        {
            using (Log.InfoCall("GetProductDetails"))
            {
                if (notify.ProductID > 0)
                {
                    string postURL = "http://" + notify.Host + ":" + notify.Port + CWF_BASE_URI + String.Format(CWF_PRODUCT_DETAIL_URI_PATTERN, PRODUCT_SHORT_NAME, RegistryHelper.GetValueFromRegistry("Version").ToString(), notify.DisplayName/* cwfInfo.RegisteredInstanceName*/);
                    postURL += (string.IsNullOrEmpty(notify.DisplayName) != true) ? string.Format("&instancename={0}", notify.DisplayName) : string.Empty;

                    string productDetails = GetData(postURL, notify);

                    if (!String.IsNullOrEmpty(productDetails))
                    {
                        Log.Info("product data returned " + productDetails);
                        Products listProducts = JsonHelper.FromJSON<Products>(productDetails);
                        foreach(Product prod in listProducts)
                        {
                            Log.Info("product with product id:" + prod.Id.ToString() + " has been found on CWF");
                            if (prod.Id == notify.ProductID)
                            {
                                return true;
                            }
                        }

                    }
                    else
                    {
                        Log.Error("product was not found");
                    }
                }
                return false;

            }

        }

        //SQLdm 10.2 (Tushar)--Fix for SQLDM-28150
        private string GetData(string url, PluginCommon.NotifyProduct notify)
        {
            HttpWebRequest request = CreateRequest(url, "GET", notify.User, notify.Password);
            Log.Info("requesting for url" + url);
            HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();
            // Processing and saving response
            StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
            string fullResponse = responseReader.ReadToEnd();
            responseReader.Close();
            webResponse.Close();
            return fullResponse;
        }
    }

    /// <summary>
    /// for serializing and desirializing json
    /// </summary>
    public static class JsonHelper
    {
        private static Logger LOG = Logger.GetLogger("JsonHelper");

        public static T FromJSON<T>(string json) where T : class
        {
            using (LOG.InfoCall("FromJSON"))
            {
                if (string.IsNullOrEmpty(json)) return (null);
                try
                {
                    return (JsonConvert.DeserializeObject<T>(json) as T);
                }
                catch (Exception ex)
                {
                    LOG.Error(ex.Message + ex.InnerException != null ? ex.InnerException.Message : "");
                    return (null);
                }
            }
        }

        public static string ToJSON<T>(T obj) where T : class
        {
            using (LOG.InfoCall("ToJSON"))
            {
                if (obj == null) return (string.Empty);
                try
                {
                    //[START] - SQLdm 9.0 (Ankit Srivastava) - CWF Integration - modified the defauls datetime format
                    JsonSerializerSettings customJsonSettings = new JsonSerializerSettings()
                    {
                        DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                        DateTimeZoneHandling = DateTimeZoneHandling.Utc
                    };
                    return JsonConvert.SerializeObject(obj, customJsonSettings);
                    //[END] - SQLdm 9.0 (Ankit Srivastava) - CWF Integration - modified the defauls datetime format
                }
                catch (Exception ex)
                {
                    LOG.Error("ToJSON failed", ex);
                    return (null);
                }
            }
        }
    }

    /// <summary>
    /// SQLdm 9.0: Gaurav Karwal: Adding the form upload class to enable upload of the form multipart data
    /// </summary>
    public static class FormUpload
    {
        private static readonly Logger LOG = Logger.GetLogger("FormUpload");
        private static readonly Encoding encoding = Encoding.UTF8;
        public static HttpWebResponse MultipartFormDataPost(string postUrl, string userAgent, Dictionary<string, object> postParameters, string username, string password)
        {
            using (LOG.InfoCall("MultipartFormDataPost"))
            {
                try
                {
                    string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
                    string contentType = "multipart/form-data; boundary=" + formDataBoundary;

                    byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);

                    return PostForm(postUrl, userAgent, contentType, formData, username, password);
                }
                catch(Exception ex)
                {
                    LOG.Error("Error on multipart data post", ex);
                    return null;
                }
            }
        }

        public static byte[] GetMultiPartFormDataForResponse(Dictionary<string, object> postParameters)
        {
            using (LOG.InfoCall("GetMultiPartFormDataForResponse"))
            {
                try
                {
                    string formDataBoundary = String.Format("----------{0:N}", Guid.NewGuid());
                    string contentType = "multipart/form-data; boundary=" + formDataBoundary;

                    return GetMultipartFormData(postParameters, formDataBoundary);
                }
                catch (Exception ex)
                {
                    LOG.Error("Error on get multipart data form data for response", ex);
                    return null;
                }
            }
        }

        private static HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData, string username, string password)
        {
            using (LOG.InfoCall("PostForm"))
            {
                try
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
                catch (Exception ex)
                {
                    LOG.Error("Error on get multipart data form data for response", ex);
                    return null;
                }
            }
        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            using (LOG.InfoCall("GetMultipartFormData"))
            {
                try
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
                catch (Exception ex)
                {
                    LOG.Error("Error on get multipart data form data", ex);
                    return null;
                }
            }
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
