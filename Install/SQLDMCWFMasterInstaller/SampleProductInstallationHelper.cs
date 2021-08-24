using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Installer_form_application
{
    class SampleProductInstallationHelper
    {
        private static Dictionary<string, string> getProductDetailsFromRegistry(Dictionary<string, string> installationDetails)
        {
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Idera\\" + installationDetails["Product"]);
            if (regKey != null)
            {
                string[] regProperties = regKey.GetValueNames();
                foreach (string property in regProperties)
                {
                    installationDetails[property] = regKey.GetValue(property).ToString();
                }
            }
            return installationDetails;
        }

        public static bool checkIfProductInstanceExists(string url, string instanceName, string username, string password)
        {
            url = url + "/IderaCoreServices/v1/Products/IsAvailable?productname=SQLdm&instancename=" + instanceName + "&version=10.1.0.0";
            Response response = GetRequest(url, username, password);
            bool instanceExists = false;
            if (response.httpStatusCode != HttpStatusCode.OK)
                instanceExists = true;
            else
                instanceExists = false;
            return instanceExists;
        }

        public static bool checkIfDashboardAdministrator(string url, string username, string password)
        {
            url = url + "/IderaCoreServices/v1/Users/IsDashboardAdministrator";
            Response response = GetRequest(url, username, password);
            if (response.httpStatusCode != HttpStatusCode.OK)
                return false;
            return true;
        }

        public static bool checkPreviousProductVersion(string url, string username, string password)
        {
            // this web request gets all the products intalled with shortname SQLdm
            url = url + "/IderaCoreServices/v1/Products/SQLdm";
            Response response = GetRequest(url, username, password);
            if (response.httpStatusCode == HttpStatusCode.OK)
            {
                Products products = JsonHelper.FromJSON<Products>(response.response);
                int count = products.Count;
                if (count == 0)
                {
                    return false;
                }
                Version temp = new Version("1.0.0.0");
                Version latest = new Version("1.0.0.0");

                foreach (Product product in products)
                {
                    temp = new Version(product.Version);
                    if (latest.CompareTo(temp) < 0)
                    {
                        latest = temp;
                    }
                }

                // got the latest version in latest variable
                // compare it with current product version i.e. "1.0.0.0" in our case
                temp = new Version(getCurrentVersion());
                if (latest.CompareTo(temp) != 0)
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
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
                case HttpStatusCode.Forbidden: msg = "User does not have permissions to register the product";
                    break;
                case HttpStatusCode.BadRequest: msg = description;
                    break;
            }
            return msg;
        }

        public static string getCurrentVersion()
        {
            return "10.1.0.0";
        }
    }

    public class Response
    {
        public string response;
        public HttpStatusCode httpStatusCode;
        public Response(string responseValue, HttpStatusCode httpStatusCode)
        {
            this.response = responseValue;
            this.httpStatusCode = httpStatusCode;
        }
    }
}
