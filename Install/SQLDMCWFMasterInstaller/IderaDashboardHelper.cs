using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using CWFInstallerService;
using System.Web.Script.Serialization;
using Idera.SQLdm.Common.Configuration;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using BBS.TracerX;

namespace Installer_form_application
{
    public static class IderaDashboardHelper
    {
        private static Logger LOG = Logger.GetLogger("IderaDashboardHelper");
        private static string GetDisplayNameCommand = "select top 1 InstanceName from WebFramework order by WebFrameworkID desc";

        public static bool IsRemoteDashbaordVersionCompatible(string remoteVersion)
        {
            Version installedVersionOnRemote = new Version(remoteVersion);
            installedVersionOnRemote = new Version(installedVersionOnRemote.Major, installedVersionOnRemote.Minor, installedVersionOnRemote.Build, installedVersionOnRemote.Revision);

            Version restrictedIDVersionForRemote = new Version(Properties.Settings.Default.RestrictedIDVersionForRemote);
            restrictedIDVersionForRemote = new Version(restrictedIDVersionForRemote.Major, restrictedIDVersionForRemote.Minor, restrictedIDVersionForRemote.Build, restrictedIDVersionForRemote.Revision);

            return (installedVersionOnRemote >= restrictedIDVersionForRemote);
        }
        public static void ValidateServiceCredentials(string username, string password)
        {
            Validator.ValidateServiceCredentials(username, password);
        }
        public static void ValidateDashboardUrl(string url)
        {
            Validator.validateDashboardUrl(url);
        }
        public static bool checkIfDashboardAdministrator(string url, string username, string password)
        {
            url = url + "/IderaCoreServices/v1/Users/IsDashboardAdministrator";
            Response response = GetRequest(url, username, password);
            if (response.httpStatusCode != HttpStatusCode.OK)
                return false;
            return true;
        }

        public static string GetExistingDisplayName(string instanceName, string databaseName, bool windowsAuthRqd, string userName, string password)
        {
            try
            {
                string existingDisplayName;
                SqlConnectionInfo sqlConnectionInfo = new SqlConnectionInfo(instanceName, databaseName, userName, password);
                sqlConnectionInfo.UseIntegratedSecurity = windowsAuthRqd;
                existingDisplayName = (string)SqlHelper.ExecuteScalar(sqlConnectionInfo.GetConnection(), CommandType.Text, GetDisplayNameCommand);
                return existingDisplayName;
            }
            catch(Exception ex)
            {
                LOG.ErrorFormat("GetExistingDisplayName: {0}",ex.Message);
                //if due to some error, display name can not be fetched. Dont block the user
                return string.Empty;
            }
        }

        public static bool DoesDisplayNameAlreadyExist(string ip, string coreServicePort, string displayName, string username, string password)
        {
            try
            {
                string url = string.Format("http://{0}:{1}/IderaCoreServices/v1/Products/SQLdm?instancename={2}", ip, coreServicePort, displayName);
                string jsonResponse = GetResponse(url, username, password);
                List<Product> products = new List<Product>();
                if (!String.IsNullOrWhiteSpace(jsonResponse))
                {
                    products.AddRange(new JavaScriptSerializer().Deserialize<List<Product>>(jsonResponse));
                }
                if (products.Count > 0)
                    return true;
                return false;
            }
            catch(Exception ex)
            {
                LOG.ErrorFormat("DoesDisplayNameAlreadyExist: {0}", ex.Message);
                //if the url can not pinged, let the flow continue
                return true;
            }
        }

        private static HttpWebRequest CreateRequest(string url, string serviceAdminUser, string serviceAdminPassword)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentLength = 0;
                request.ContentType = "application/json";
                string username = serviceAdminUser;
                string password = serviceAdminPassword;

                string header = String.Empty;
                if (!(String.IsNullOrWhiteSpace(username) && String.IsNullOrWhiteSpace(password)))
                    header = "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(Regex.Replace(username + ":" + password, "\\\\", "\\")));

                request.Headers.Add(HttpRequestHeader.Authorization, header);
                return request;
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public static string GetResponse(string url, string username, string password)
        {
            var request = CreateRequest(url, username, password);
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
                throw ex;
            }
            return responseValue;
        }

        private static Response GetRequest(string url, string username, string password)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentLength = 0;
            request.ContentType = "application/json";

            string msg = string.Empty;
            string header = "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(Regex.Replace(username + ":" + password, "\\\\", "\\")));
            request.Headers.Add(HttpRequestHeader.Authorization, header);

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var responseValue = string.Empty;
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
                    return new Response(responseValue, response.StatusCode);
                }

            }
            catch (WebException e)
            {
                var response = ((HttpWebResponse)e.Response);
                msg = GetErrorMessage(response.StatusCode, response.StatusDescription);
                return new Response(msg, response.StatusCode);
            }
        }

        private static string GetErrorMessage(HttpStatusCode status, string description)
        {
            string msg = String.Empty;
            switch (status)
            {
                case HttpStatusCode.Forbidden:
                    msg = "User does not have permissions to register the product";
                    break;
                case HttpStatusCode.BadRequest:
                    msg = description;
                    break;
            }
            return msg;
        }
    }
}