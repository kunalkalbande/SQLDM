using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using TracerX;
using System.Net;
using Idera.SQLdoctor.Common.Helpers;

namespace Idera.SQLdoctor.Common.Helper
{
    public static class WebBrowserHelper
    {
        private static readonly Logger Log = Logger.GetLogger("WebBrowserHelper");

        //public static void OpenUrl(string url)
        //{
        //    try
        //    {
        //        Process.Start(url);
        //    }
        //    catch (Exception ex)
        //    {
        //        ApplicationMessageBox.ShowError(SQLtoolboxApplication.Instance.MainForm, "An error occurred when trying to redirect to " + url, ex);
        //    }
        //}

        static string proxyserver;
        static string proxyport;
        static string proxyuser;
        static string proxypass;

        private static readonly object makeRequestLockObject = new object();

        public static HttpWebRequest MakeWebRequest(string url, ICredentialDialog credDialog)
        {
            lock (makeRequestLockObject)
            {
                return MakeWebRequest2(url, credDialog);
            }
        }

        public static HttpWebRequest MakeWebRequest2(string url, ICredentialDialog credDialog)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            // request method
            webRequest.Method = "GET";

            ////string srv = SQLtoolboxApplication.Instance.GetSetting<string>(Constants.SettingsProxyServer);
            ////string prt = SQLtoolboxApplication.Instance.GetSetting<string>(Constants.SettingsProxyServerPort);

            //if (!string.IsNullOrEmpty(prxServer))
            //{
            //    proxyserver = prxServer;
            //    proxyport = prxPort;
            //}

            try
            {
                IWebProxy webProxy = webRequest.Proxy;

                string uri = webProxy == null ? string.Empty : webRequest.Proxy.GetProxy(new Uri(url)).AbsoluteUri;
                bool isset = !uri.EndsWith("xml");

                // try the default if not already set
                if (!uri.EndsWith("xml") && string.IsNullOrEmpty(proxyserver))
                {
                    WebProxy proxy = new WebProxy();

                    string pserver = uri.IndexOf(':') != uri.LastIndexOf(':') ? uri.Substring(0, uri.LastIndexOf(':')) : uri;
                    string pport = webRequest.Proxy.GetProxy(new Uri(url)).Port.ToString();

                    string s = pserver;
                    if (!string.IsNullOrEmpty(pport))
                        s = pserver + ":" + pport;

                    proxy.Address = new Uri(s);
                    proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                    IWebProxy oldProxy = WebRequest.DefaultWebProxy; // save old one
                    WebRequest.DefaultWebProxy = null;

                    // create a new request
                    webRequest = (HttpWebRequest)WebRequest.Create(url);

                    WebRequest.DefaultWebProxy = oldProxy;

                    // save the proxy
                    webRequest.Proxy = proxy;

                    if (TryWebrequestForProxyAuth(webRequest))
                        return webRequest;
                }

                // if we have proxy, use it
                if (!string.IsNullOrEmpty(proxyserver))
                {
                    WebProxy proxy = new WebProxy();

                    string s = proxyserver;
                    if (!string.IsNullOrEmpty(proxyport))
                        s = proxyserver + ":" + proxyport;

                    proxy.Credentials = CredentialCache.DefaultCredentials;
                    proxy.Address = new Uri(s);

                    // try with integrated first
                    webRequest = (HttpWebRequest)WebRequest.Create(url);
                    WebRequest.DefaultWebProxy = null;
                    webRequest.Proxy = proxy;

                    if (TryWebrequestForProxyAuth(webRequest))
                        return webRequest;

                    // try the credentials we have
                    NetworkCredential credential = new NetworkCredential(proxyuser, proxypass);
                    //proxy.Credentials = credential;

                    CredentialCache cache = new CredentialCache();
                    cache.Add(new Uri(uri), "Basic", credential);
                    cache.Add(new Uri(uri), "NTLM", credential);
                    cache.Add(new Uri(uri), "Digest", credential);
                    cache.Add(new Uri(uri), "Kerberos", credential);
                    proxy.Credentials = cache;

                    // create a new request
                    webRequest = (HttpWebRequest)WebRequest.Create(url);

                    // save the proxy        
                    WebRequest.DefaultWebProxy = proxy;
                    webRequest.Proxy = proxy;

                    if (TryWebrequestForProxyAuth(webRequest))
                        return webRequest;
                }

                if (TryWebrequestForProxyAuth(webRequest))
                    return webRequest;

                if (!isset && string.IsNullOrEmpty(proxyserver))
                    proxyserver = string.Empty;

                // prompt for proxy auth, etc
                PromptForProxy(ref webRequest, url, credDialog);
            }
            catch (Exception e)
            {
                // check code?
                Log.Debug("Caught Exception while accessing " + url, e);
            }

            return webRequest;
        }

        private static void PromptForProxy(ref HttpWebRequest webRequest, string url, ICredentialDialog dialog)
        {
            string server = proxyserver;
            string port = proxyport;
            string username = proxyuser;
            string password = proxypass;

            while (!TryWebrequestForProxyAuth(webRequest))
            {
                // prompt the user for proxy credentials (so long as the credentials don't work)

  //              dialog.Server = proxyserver;
  //              dialog.Port = proxyport;
                dialog.Caption = "Web proxy authentication"; 
                dialog.Message = "Please enter your user id and password to access the proxy server.  We are attempting to access the Idera update site.";
                dialog.User = proxyuser;
                dialog.Password = proxypass;

                if (!dialog.ShowDialog(IntPtr.Zero))
                    return;

  //              server = dialog.Server;
  //              port = dialog.Port;
                username = dialog.User;
                password = dialog.Password;

                WebProxy proxy = new WebProxy();

                string s = server;
                if (!string.IsNullOrEmpty(port))
                    s = server + ":" + port;

                proxy.Address = new Uri(s);
                proxy.Credentials = new NetworkCredential(username, password);

                // create a new request
                webRequest = (HttpWebRequest)WebRequest.Create(url);

                // save the proxy
                webRequest.Proxy = proxy;
            }

            // save the good ones
            proxyserver = server;
            proxyport = port;
            proxyuser = username;
            proxypass = password;

            // save the settings
            SaveProxySettings();
        }

        private static bool TryWebrequestForProxyAuth(HttpWebRequest webRequest)
        {
            try
            {
                webRequest.GetResponse();

                return true;
            }
            catch (WebException e)
            {
                Log.Error(string.Format("Caught an exception while accessing {0}.", webRequest.Address), e);

                WebException we = e as WebException;

                if (we.Response != null)
                {
                    if (((HttpWebResponse)we.Response).StatusCode == HttpStatusCode.ProxyAuthenticationRequired || ((HttpWebResponse)we.Response).StatusCode == HttpStatusCode.Forbidden)
                        return false;
                    else
                        return true;
                }
                else
                if (e.Status == WebExceptionStatus.NameResolutionFailure || e.Status == WebExceptionStatus.ProxyNameResolutionFailure)
                    return true;
                else
                    return false;
            }
        }

        private static void SaveProxySettings()
        {
//            SQLtoolboxApplication.Instance.SetSetting(Constants.SettingsProxyServer, proxyserver);
//            SQLtoolboxApplication.Instance.SetSetting(Constants.SettingsProxyServerPort, proxyport);
//            SQLtoolboxApplication.Instance.SaveSettings();
        }
    }
}
