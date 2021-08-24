using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace CustomActions
{
    /// <summary>
     /// SQLdm 10.1 (Praveen Suhalka) (CWF Integration) -- Added this class to handle de regidteration of CWF on uninstall of SQLdm
     /// </summary>
     public class CwfHelper
     {
         private string _host;
         private string _port;
         private string _username;
         private string _password;
         private int _productId = 0;
         private string _instanceName;
         public string BaseURL
         {
             get { return "http://" + _host + ":" + _port; }
         }
 
         private CwfHelper() { }

         public CwfHelper(Dictionary<string, string> frameworkDetails)
         {
             if (frameworkDetails != null && frameworkDetails.Count > 0)
             {
                 this._host = frameworkDetails["HostName"];
                 this._port = frameworkDetails["Port"];
                 this._username = frameworkDetails["UserName"];
                 this._password = frameworkDetails["Password"];
                 if (frameworkDetails["ProductID"] != null && frameworkDetails["ProductID"] != string.Empty)
                 {
                     int productId;
                     if (Int32.TryParse(frameworkDetails["ProductID"], out productId))
                     {
                         this._productId = productId;
                     }
                 }
                 this._instanceName = frameworkDetails["InstanceName"];
             }
         }
 
         public void DeRegisterProduct()
         {
             string postURL = this.BaseURL + "/IderaCoreServices/v1/" + "Products/" + this._productId;
             postURL += "?notify=false";
             HttpWebRequest request = CreateBasicAuthorizationRequest(postURL, "DELETE", "application/json");
             HttpWebResponse response = request.GetResponse() as HttpWebResponse;

             var responseValue = string.Empty;
             if (response.StatusCode != HttpStatusCode.OK)
             {
                 var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                 throw new ApplicationException(message);
             }
         }
 
         private HttpWebRequest CreateBasicAuthorizationRequest(string url, string method, string contentType)
         {
             var request = (HttpWebRequest)WebRequest.Create(url);
             request.Method = method;
             request.ContentType = contentType;

             string header = String.Empty;
             if (!(String.IsNullOrEmpty(this._username) && String.IsNullOrEmpty(this._password)))
                 header = "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(Regex.Replace(this._username + ":" + this._password, "\\\\", "\\")));
 
             request.Headers.Add(HttpRequestHeader.Authorization, header);
             return request;
         }
     }
}
