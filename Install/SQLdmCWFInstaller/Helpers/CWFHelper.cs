using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
//using BBS.TracerX;
using SQLdmCWFInstaller.Helpers;

namespace SQLdmCWFInstaller
{
    /// <summary>
    /// SQLdm 9.0 (Ankit Srivastava) - CWF Installer Wrapper - Created New helper class to interact with the CWF APIs
    /// </summary>
    class CWFHelper
    {
        #region Fields
        private static readonly Logger Log = Logger.GetLogger("SQLdmCWFInstaller_CWFHelper");
        private string serviceAdminUser;
        private string serviceAdminPassword;
        private string coreServiceURL = Constants.CWFAPIRootDefaultURL; 
        #endregion

        /// <summary>
        /// parametrized constructor
        /// </summary>
        /// <param name="serviceAdminUser"></param>
        /// <param name="serviceAdminPassword"></param>
        /// <param name="coreServiceURL"></param>
        public CWFHelper(string serviceAdminUser, string serviceAdminPassword, string coreServiceURL)
        {
            this.serviceAdminUser = serviceAdminUser;
            this.serviceAdminPassword = serviceAdminPassword;
            this.coreServiceURL = coreServiceURL ?? this.coreServiceURL;
        }

        #region Private Methods

        /// <summary>
        /// This method returns the JSON response in strin format given an API URL path 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetResponse(string url)
        {
            var request = CreateRequest(url);
            var responseValue = string.Empty;
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                        throw new ApplicationException(message);
                    }

                    // grab the response
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
            catch (Exception ex)
            {
                if (ex.Message.Equals("The remote server returned an error: (403) Forbidden.", StringComparison.CurrentCultureIgnoreCase))
                    return "Authentication Error";
                Log.Error("Exception thrown in GetResponse where coreServiceURL = " + this.coreServiceURL, ex);
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
            var request = CreateRequest(url);

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
        private HttpWebRequest CreateRequest(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentLength = 0;
                request.ContentType = "application/json";
                string username = serviceAdminUser;
                string password = serviceAdminPassword;

                string header=String.Empty;
                if(!(String.IsNullOrWhiteSpace(username) && String.IsNullOrWhiteSpace(password)))
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

        #region Internal Methods

        /// <summary>
        /// This method returns a list of prroduct details given the product name and the version
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        internal string GetProducts(string productName, string version)
        {
            //string url = coreServiceURL + Constants.CWFAPIProductsPath
              //                          + ((String.IsNullOrWhiteSpace(productName) || String.IsNullOrWhiteSpace(version)) ?
                //                         String.Empty : ("/"+productName+"?version=" + version ));
            string url = coreServiceURL + Constants.CWFAPIProductsPath + "/" + productName;//SQLdm 9.1 (Vineet Kumar) -- Removed version to get list off all products with all versions.
            return GetResponse(url);
        }

        /// <summary>
        /// This method checks if a product is already registerd with the given product name and version and instance name
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="version"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        internal bool CheckIfDuplicate(string productName, string version, string instance)
        {
            string url = coreServiceURL + Constants.CWFAPIIsAvailablePath
                                        + ((String.IsNullOrWhiteSpace(productName) || String.IsNullOrWhiteSpace(version))
                                        ? String.Empty : ("?productname=" + productName
                                                            + (String.IsNullOrWhiteSpace(instance) ? String.Empty : ("&instancename=" + instance))
                                                            + "&version=" + version));

            Log.Info("IsAvailable URL: " + url);
            return !CheckIfResponseIsOK(url);
        }

        /// <summary>
        /// This method returns the Service Status
        /// </summary>
        /// <returns></returns>
        internal string GetStatus()
        {
            string url = coreServiceURL + Constants.CWFAPIStatusPath;
            var response= GetResponse(url);
            if (response.Equals("Authentication Error", StringComparison.CurrentCultureIgnoreCase))
                return null;
            else
                return response;
        }

        /// <summary>
        /// This method returns the Service Status
        /// </summary>
        /// <returns></returns>
        internal bool GetLocalInstallionStatus()
        {
            string url = Constants.CWFAPIRootDefaultURL + Constants.CWFAPIStatusPath;//SQLdm 9.1 (Vineet) -- Always Use localpath to detect local installation
            if (GetResponse(url).Equals("Authentication Error", StringComparison.CurrentCultureIgnoreCase))
                return true;

            return false;
        }

        #endregion


    }
}
