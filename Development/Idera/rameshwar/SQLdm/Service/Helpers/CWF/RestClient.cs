// ===============================
// AUTHOR       : CWF Team - Gowrish 
// PURPOSE      : Backend Isolation
// TICKET       : SQLDM-29086
// ===============================
using BBS.TracerX;
using Idera.SQLdm.Service.Helpers.CWF;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Idera.SQLdm.Service.Helpers.CWF
{
    public enum HttpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public partial class RestClient
    {
        private static readonly Logger LogX = Logger.GetLogger("RestClient");

        public static int TIMEOUT_MS = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;

        public static WebHeaderCollection StandardHeaders = new WebHeaderCollection();

        public string EndPoint { get; set; }
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }

        private string authHeader;

        public RestClient()
        {
            EndPoint = "";
            Method = HttpVerb.GET;
            ContentType = "application/json";
            PostData = "";
        }
        public RestClient(string endpoint)
        {
            EndPoint = endpoint;
            Method = HttpVerb.GET;
            ContentType = "application/json";
            PostData = "";
        }
        public RestClient(string endpoint, HttpVerb method)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/json";
            PostData = "";
        }

        public RestClient(string endpoint, HttpVerb method, string postData)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/json";
            PostData = postData;
        }

        public void SetAuthenticationHeader(string userId, string password)
        {
            authHeader = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(userId + ":" + password));
        }

        public void SetAuthenticationHeader(string authenticationHeader)
        {
            authHeader = authenticationHeader;
        }

        public string MakeRequest()
        {
            return MakeRequest("");
        }


        [SuppressMessage("Microsoft.Usage", "CA2202:DoNotDisposeObjectsMultipleTimes")]
        public string MakeRequest(string parameters)
        {
            var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);

            request.Method = Method.ToString();
            request.ContentLength = 0;
            request.ContentType = ContentType;

            if (request.Timeout < TIMEOUT_MS)
                request.Timeout = TIMEOUT_MS;

            if (!String.IsNullOrEmpty(authHeader))
                request.Headers.Add("Authorization", authHeader);
            else
                request.Headers.Add(StandardHeaders);

            if (!string.IsNullOrEmpty(PostData) && (Method == HttpVerb.POST || Method == HttpVerb.PUT || Method == HttpVerb.DELETE))
            {
                var bytes = Encoding.UTF8.GetBytes(PostData);
                request.ContentLength = bytes.Length;
                try
                {
                    using (var writeStream = request.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }
                catch (Exception e)
                {
                    LogX.WarnFormat("Error writing encoded stream to request data. Exception: {0}", e.Message);
                    LogX.Debug(e);
                }
            }
            HttpWebResponse response = null;
            int refreshRetryCount = 0;
            while (true && refreshRetryCount < 5)
            {
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    LogX.DebugFormat("The response Code received {0}", response.StatusCode);
                    break;
                }
                catch (WebException e)
                {
                    response = (HttpWebResponse)e.Response;
                    if (e.Status == WebExceptionStatus.ProtocolError && response.StatusCode.Equals(HttpStatusCode.ProxyAuthenticationRequired))
                    {
                        LogX.InfoFormat("The auth token expired. Obtaining the new Auth token");
                        refreshRetryCount++;
                        try
                        {
                            CWFHelper.RefreshToken();
                            //Continue to retry the original request again.
                            continue;
                        }
                        catch (Exception ex)
                        {
                            LogX.Error("Error Trying to refresh the token.", ex);
                            continue;
                        }
                    }
                    else
                    {
                        //It is not a 407 credentials exception.
                        LogX.DebugFormat("The response Code received {0}", response.StatusCode);
                        break;
                    }
                }
            }

            {
                var responseValue = string.Empty;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    LogX.ErrorFormat("Request Failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                // grab the response
                if (response.ContentLength > 0)
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                    }
                }

                return responseValue;
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:DoNotDisposeObjectsMultipleTimes")]
        public byte[] MakeRequestAndReturnByteArray(string parameters)
        {
            var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);

            request.Method = Method.ToString();
            request.ContentLength = 0;
            request.ContentType = ContentType;

            if (request.Timeout < TIMEOUT_MS)
                request.Timeout = TIMEOUT_MS;

            if (!String.IsNullOrEmpty(authHeader))
                request.Headers.Add("Authorization", authHeader);
            else
                request.Headers.Add(StandardHeaders);

            if (!string.IsNullOrEmpty(PostData) && (Method == HttpVerb.POST || Method == HttpVerb.PUT || Method == HttpVerb.DELETE))
            {
                var bytes = Encoding.UTF8.GetBytes(PostData);
                request.ContentLength = bytes.Length;
                try
                {
                    using (var writeStream = request.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }
                catch (Exception e)
                {
                    LogX.WarnFormat("Error writing encoded stream to request data. Exception: {0}", e.Message);
                    LogX.Debug(e);
                }
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                byte[] responseValue = null;
                // grab the response
                if (response.ContentLength > 0)
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (var reader = new StreamReader(responseStream, Encoding.UTF8))
                            {
                                return ToByteArray(response.GetResponseStream());
                            }
                    }


                }
                return responseValue;
            }
        }
        public byte[] ToByteArray(Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }
    } // class
}
