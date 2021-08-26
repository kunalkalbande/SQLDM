// ===============================
// AUTHOR       : CWF Team - Gowrish 
// PURPOSE      : Backend Isolation
// TICKET       : SQLDM-29086
// ===============================
using BBS.TracerX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Service.Helpers.CWF
{
    public class HttpRequestHelper
    {
        private static readonly Logger LogX = Logger.GetLogger("HttpRequestHelper");
        public static T Get<T>(string uri, string authenticationHeader) where T : class
        {
            LogX.DebugFormat("Making an GET request to {0}", uri);
            var client = new RestClient(uri);
            client.Method = HttpVerb.GET;
            client.SetAuthenticationHeader(authenticationHeader);
            var json = client.MakeRequest();
            LogX.DebugFormat("Response received for the GET request to {0}", uri);
            return JsonHelper.FromJSON<T>(json);
        }

        public static T Post<T>(string json, string uri, string authenticationHeader) where T : class
        {
            LogX.DebugFormat("Making an POST request to {0}", uri);
            var client = new RestClient(uri);
            client.Method = HttpVerb.POST;
            client.PostData = json;
            client.SetAuthenticationHeader(authenticationHeader);
            var resJson = client.MakeRequest();
            LogX.DebugFormat("Response received for the POST request to {0}", uri);
            return JsonHelper.FromJSON<T>(resJson);
        }

        public static T Put<T>(string json, string uri, string authenticationHeader) where T : class
        {
            LogX.DebugFormat("Making an PUT request to {0} with request data", uri);
            var client = new RestClient(uri);
            client.Method = HttpVerb.PUT;
            client.PostData = json;
            client.SetAuthenticationHeader(authenticationHeader);
            var res = client.MakeRequest();
            LogX.DebugFormat("Response received for the PUT request to {0}", uri);
            return JsonHelper.FromJSON<T>(res);
        }

        public static void Delete(String json, string uri, string authenticationHeader)
        {
            LogX.DebugFormat("Making an DELETE request to {0} with request data", uri);
            var client = new RestClient(uri);
            client.Method = HttpVerb.DELETE;
            if (!String.IsNullOrEmpty(json))
            {
                client.PostData = json;
            }
            client.SetAuthenticationHeader(authenticationHeader);
            var resJson = client.MakeRequest();
            LogX.DebugFormat("Response received for the DELETE request to {0}", uri);
        }
    }
}
