using CWFInnstallerService;
using Idera.InstallationHelper;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;

namespace CWFInstallerService
{
    public static class InstallationHelper
    {
        public static Dictionary<string, string> checkIfDashboardIsAlreadyInstalled()
        {
            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            string productName, version;
            Dictionary<string, string> installationDetails = new Dictionary<string, string>();
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        try
                        {
                            productName = subkey.GetValue("DisplayName").ToString();
                            if (productName == "Idera Dashboard")
                            {
                                version = subkey.GetValue("DisplayVersion").ToString();
                                installationDetails["ProductVersion"] = version;
                                installationDetails["Product"] = "CWF";
                                installationDetails = getProductDetailsFromRegistry(installationDetails);
                                break;
                            }
                        }
                        catch
                        {   
                        }
                    }
                }
            }
            if (installationDetails.Count == 0)
            {
                installationDetails["isInstalled"] = "false";
            }
            else
            {
                installationDetails["isInstalled"] = "true";
            }
            return installationDetails;
        }



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

        private static Dictionary<string, string> getProductDetailsFromRemoteRegistry(Dictionary<string, string> installationDetails, string hostname)
        {
            string regKey = "SOFTWARE\\Idera\\" + installationDetails["Product"];
            using (RegistryKey key = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, hostname).OpenSubKey(regKey))
            {
                if (key != null)
                {
                    string[] regProperties = key.GetValueNames();
                    foreach (string property in regProperties)
                    {
                        installationDetails[property] = key.GetValue(property).ToString();
                    }
                }
            }
            return installationDetails;
        }

        public static Dictionary<string, string> checkIfDashboardIsInstalledOnRemote(string username, string password, string hostname)
        {
            SafeTokenHandle safeTokenHandle = getSafeTokenHandle(username, password, hostname);
            try
            {
                using (safeTokenHandle)
                {
                    using (WindowsImpersonationContext impersonatedUser = WindowsIdentity.Impersonate(safeTokenHandle.DangerousGetHandle()))
                    {
                        string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
                        string productName, version;
                        Dictionary<string, string> installationDetails = new Dictionary<string, string>();
                        using (RegistryKey key = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, hostname).OpenSubKey(registry_key))
                        {
                            foreach (string subkey_name in key.GetSubKeyNames())
                            {
                                using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                                {
                                    try
                                    {
                                        productName = subkey.GetValue("DisplayName").ToString();
                                        if (productName == "Idera Dashboard")
                                        {
                                            version = subkey.GetValue("DisplayVersion").ToString();
                                            installationDetails["ProductVersion"] = version;
                                            installationDetails["Product"] = "CWF";
                                            installationDetails = getProductDetailsFromRemoteRegistry(installationDetails, hostname);
                                            break;
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }
                        if (installationDetails.Count == 0)
                        {
                            installationDetails["isInstalled"] = "false";
                        }
                        else
                        {
                            installationDetails["isInstalled"] = "true";
                        }
                        installationDetails["can_connect"] = "true";
                        return installationDetails;
                    }

                }
            }
            catch
            {
                Dictionary<string, string> installFail = new Dictionary<string, string>();
                installFail["can_connect"] = "false";
                installFail["is_installed"] = "false";
                return installFail;
            }
            
        }

        public static Dictionary<string, string> readPropertiesFile(string path)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            foreach (var row in File.ReadAllLines(path + "\\WebApplication\\conf\\web.PROPERTIES"))
                data.Add(row.Split('=')[0].Trim(), string.Join("=", row.Split('=').Skip(1).ToArray()).Trim());
            return data;
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeTokenHandle()
                : base(true)
            {
            }

            [DllImport("kernel32.dll")]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [SuppressUnmanagedCodeSecurity]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool CloseHandle(IntPtr handle);

            protected override bool ReleaseHandle()
            {
                return CloseHandle(handle);
            }
        }

        public static bool copyMsiToRemote(string username, string password, string hostname, string fileName)
        {
            SafeTokenHandle safeTokenHandle = getSafeTokenHandle(username, password, hostname);
            try
            {
                using (safeTokenHandle)
                {
                    using (WindowsImpersonationContext impersonatedUser = WindowsIdentity.Impersonate(safeTokenHandle.DangerousGetHandle()))
                    {
                        Directory.CreateDirectory(@"\\" + hostname + "\\C$\\temp");
                        File.Copy(fileName, @"\\" + hostname + "\\C$\\temp\\" + Path.GetFileName(fileName), true);
                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static void deleteMsifromRemote(string username, string password, string hostname, string fileName)
        {
            SafeTokenHandle safeTokenHandle = getSafeTokenHandle(username, password, hostname);
            try
            {
                using (safeTokenHandle)
                {
                    using (WindowsImpersonationContext impersonatedUser = WindowsIdentity.Impersonate(safeTokenHandle.DangerousGetHandle()))
                    {
                        File.Delete(@"\\" + hostname + "\\C$\\temp\\" + fileName);
                        Directory.Delete(@"\\" + hostname + "\\C$\\temp");
                    }

                }
            }
            catch
            {
                return;
            }
            return;
        }

        public static void validateRemoteServiceAccount(string username, string password, string hostname)
        {
            if(string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username)) throw new EmptyUsernameException();
            string domain = string.Empty;
            string user = string.Empty;
            if (username.IndexOf(@"\") != -1)
            {
                string[] array = username.Split('\\');
                domain = array[0];
                user = array[1];
            }
            if (string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(user)) throw new InvalidFormatException();
            if(string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password)) throw new EmptyPasswordException();
            if(string.IsNullOrEmpty(hostname) || string.IsNullOrWhiteSpace(hostname)) throw new EmptyHostnameException();

            SafeTokenHandle safeTokenHandle = getSafeTokenHandle(username, password, hostname);
            if (safeTokenHandle == null)
            {
                throw new InvalidCredentialsException();
            }
        }

        public static bool checkIfDirectoryExistsOnremotePC(string username, string password, string hostname, string path)
        {
            SafeTokenHandle safeTokenHandle = getSafeTokenHandle(username, password, hostname);
            try
            {
                using (safeTokenHandle)
                {
                    using (WindowsImpersonationContext impersonatedUser = WindowsIdentity.Impersonate(safeTokenHandle.DangerousGetHandle()))
                    {
                        bool status = Directory.Exists("\\\\" + hostname + "\\" + path.Replace(':', '$'));
                        return status;
                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool install_msi_on_remote(string userName, string password, string hostname, string productArgs)
        {
            try
            {
                uint processId;
                string[] userData = userName.Split('\\');
                string domainName = userData[0];
                userName = userData[1];
                AuthToken authToken = new AuthToken(userName, password);
                bool status = CreateProcess(hostname, @"msiexec " + productArgs, out processId, authToken);
                if (status)
                {
                    if (!WaitForProcessToComplete(hostname, processId, TimeSpan.FromMinutes(15), authToken))
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool CreateProcess(string hostname, string commandLine, out uint processId, AuthToken authToken)
        {
            try
            {
                using (ManagementClass processClass = new ManagementClass(GetManagementScopeCimV2(hostname, authToken), new ManagementPath("Win32_Process"), null))
                {
                    using (ManagementBaseObject inParms = processClass.GetMethodParameters("Create"))
                    {
                        inParms["CommandLine"] = commandLine;

                        using (ManagementBaseObject outParms = processClass.InvokeMethod("Create", inParms, null))
                        {
                            uint returnValue = (uint)outParms["returnValue"];

                            processId = (outParms["processId"] != null)
                                            ?
                                                (uint)outParms["processId"]
                                            : 0;

                            return (returnValue == 0 && outParms["processId"] != null);
                        }
                    }
                }
            }
            catch (COMException e)
            {
                throw new InvalidOperationException(string.Format("Could not connect to {0}.", hostname), e);
            }
        }

        private static ManagementScope GetManagementScopeCimV2(string hostname, AuthToken authToken)
        {
            string path = string.Format(@"\\{0}\root\CimV2", hostname);

            if (authToken == null)
            {
                return new ManagementScope(path);
            }
            else
            {
                ConnectionOptions options = new ConnectionOptions();
                options.Impersonation = ImpersonationLevel.Impersonate;
                options.Username = authToken.Username;
                options.Password = authToken.Password;
                options.EnablePrivileges = true;

                return new ManagementScope(path, options);
            }
        }

        public static bool WaitForProcessToComplete(string hostname, uint processId, TimeSpan timeout, AuthToken authToken)
        {
            try
            {
                ManagementScope scope = GetManagementScopeCimV2(hostname, authToken);
                ObjectQuery query = new ObjectQuery(string.Format("SELECT Name FROM Win32_Process WHERE ProcessId = {0}", processId));

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                {
                    DateTime whenToBreak = DateTime.Now.Add(timeout);

                    while (true)
                    {
                        if (searcher.Get().Count == 0)
                        {
                            return true;
                        }

                        if (DateTime.Now >= whenToBreak)
                        {
                            return false;
                        }

                        Thread.Sleep(2000);
                    }
                }
            }
            catch (COMException e)
            {
                throw new InvalidOperationException(string.Format("Could not connect to {0}.", hostname), e);
            }
        }

        public static Products checkProductList(string url, string username, string password)
        {
            var request = (HttpWebRequest)WebRequest.Create(url + @"/IderaCoreServices/v1/products");
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

                    }
                    if (response.ContentLength > 0)
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            if (responseStream != null)
                                using (var reader = new StreamReader(responseStream))
                                {
                                    String dataContent = reader.ReadToEnd();
                                    Products products = JsonHelper.FromJSON<Products>(dataContent);
                                    return products;
                                }
                        }
                    }
                    return null;
                }

            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public static Dictionary<string, string> _unsupportedProducts = new Dictionary<string, string>
        {
	        {"SQLBI", "1.1.0.0"}
        };

        public static string getCurrentVersion()
        {
            return Constants.version;
        }

        public static bool checkIfProductInstalledSuccessfully(string product)
        {
            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            string productName;
            Dictionary<string, string> installationDetails = new Dictionary<string, string>();
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        try
                        {
                            productName = subkey.GetValue("DisplayName").ToString();
                            if (productName == product)
                            {
                                //TODO: get current version and match with existing version
                                return true;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return false;
        }

        public static bool checkIfProductInstalledOnRemoteSuccessfully(string username, string password, string hostname, string product)
        {
            SafeTokenHandle safeTokenHandle = getSafeTokenHandle(username, password, hostname);
            try
            {
                using (safeTokenHandle)
                {
                    using (WindowsImpersonationContext impersonatedUser = WindowsIdentity.Impersonate(safeTokenHandle.DangerousGetHandle()))
                    {
                        RegistryKey regKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, hostname).OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
                        string productName;
                        foreach (string subkey_name in regKey.GetSubKeyNames())
                        {
                            using (RegistryKey subkey = regKey.OpenSubKey(subkey_name))
                            {
                                try
                                {
                                    productName = subkey.GetValue("DisplayName").ToString();
                                    if (productName == product)
                                    {
                                        //TODO: get current version and match with existing version
                                        return true;
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                        return false;
                    }

                }
            }
            catch
            {
                return false;
            }
        }

        public static Dictionary<string, string> getOldConfigValuesFromRemote(string username, string password, string hostname, string installDir)
        {
            SafeTokenHandle safeTokenHandle = getSafeTokenHandle(username, password, hostname);
            try
            {
                Dictionary<string, string> session = new Dictionary<string, string>();
                using (safeTokenHandle)
                {
                    using (WindowsImpersonationContext impersonatedUser = WindowsIdentity.Impersonate(safeTokenHandle.DangerousGetHandle()))
                    {
                        try
                        {
                            StringBuilder tempDir = new StringBuilder(installDir);
                            tempDir[1] = '$';
                            installDir = tempDir.ToString();
                            string configPath = @"\\" + hostname + "\\" + installDir + @"CoreService\IderaDashboardCoreService.exe.config";
                            using (XmlTextReader reader = new XmlTextReader(configPath))
                            {
                                while (reader.Read())
                                {
                                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "setting"))
                                    {
                                        if (reader.GetAttribute("name") == "ServicePort")
                                        {
                                            session["ServicePort"] = getValueFromXml(reader);
                                        }
                                        else if (reader.GetAttribute("name") == "WebApplicationRestartPort")
                                        {
                                            session["WebAppMonitorPort"] = getValueFromXml(reader);
                                        }
                                        else if (reader.GetAttribute("name") == "RepositoryDatabase")
                                        {
                                            session["RepositoryDatabase"] = getValueFromXml(reader);
                                        }
                                        else if (reader.GetAttribute("name") == "RepositoryHost")
                                        {
                                            session["RepositoryHost"] = getValueFromXml(reader);
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            return null;
                        }
                    } return session;
                }
            }
            catch
            {
                return null;
            }
        }

        private static SafeTokenHandle getSafeTokenHandle(string username, string password, string hostname)
        {
            string[] userData = username.Split('\\');
            string domainName = userData[0];
            string userName = userData[1];
            SafeTokenHandle safeTokenHandle;
            try
            {

                const int LOGON32_PROVIDER_DEFAULT = 0;
                const int LOGON32_LOGON_INTERACTIVE = 2;

                bool returnValue = LogonUser(userName, domainName, password,
                LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                out safeTokenHandle);

                if (false == returnValue)
                {
                    return null;
                }
                return safeTokenHandle;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Dictionary<string, string> getOldConfigValuesFromLocal(string installDir)
        {       Dictionary<string, string> session = new Dictionary<string, string>();
                string configPath = installDir + @"CoreService\IderaDashboardCoreService.exe.config";
                using (XmlTextReader reader = new XmlTextReader(configPath))
                {
                    while (reader.Read())
                    {
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "setting"))
                        {
                            if (reader.GetAttribute("name") == "ServicePort")
                            {
                                session["ServicePort"] = getValueFromXml(reader);
                            }
                            else if (reader.GetAttribute("name") == "WebApplicationRestartPort")
                            {
                                session["WebAppMonitorPort"] = getValueFromXml(reader);
                            }
                            else if (reader.GetAttribute("name") == "RepositoryDatabase")
                            {
                                session["RepositoryDatabase"] = getValueFromXml(reader);
                            }
                            else if (reader.GetAttribute("name") == "RepositoryHost")
                            {
                                session["RepositoryHost"] = getValueFromXml(reader);
                            }
                        }
                    }
                }
                return session;
        }


        public static string getValueFromXml(XmlTextReader reader)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(reader.ReadInnerXml());
            XmlNode value = doc.FirstChild;
            return value.InnerXml;
        }

        public static void checkIfRemotePathIsValid(string username, string password, string hostname,string path)
        {
            SafeTokenHandle safeTokenHandle = getSafeTokenHandle(username, password, hostname);
            try
            {
                Dictionary<string, string> session = new Dictionary<string, string>();
                using (safeTokenHandle)
                {
                    using (WindowsImpersonationContext impersonatedUser = WindowsIdentity.Impersonate(safeTokenHandle.DangerousGetHandle()))
                    {
                        try
                        {
                            StringBuilder tempDir = new StringBuilder(path);
                            tempDir[1] = '$';
                            path = tempDir.ToString();
                            path = @"\\" + hostname + @"\" + path;
                            if (Directory.Exists(path))
                            {
                                return;
                            }
                            DirectoryInfo di = Directory.CreateDirectory(path);
                            di.Delete();
                        }
                        catch
                        {
                            throw new InvalidPathException();
                        }
                    } 
                }
            }
            catch
            {
                
            }
        }

        public static void checkDiskSpace(string hostname, string username, string password, string driveName)
        {
            AuthToken authToken = new AuthToken(username, password);
            double diskSpace = (CalculateDiskSpaceOnRemote(hostname, driveName, authToken) / 1024f)/ 1024f;
            if (diskSpace < 500)
            {
                throw new DiskSpaceNotAvailableException();
            }
        }

        public static ulong CalculateDiskSpaceOnRemote(string hostname, string driveName, AuthToken authToken)
        {
            try
            {
                using (ManagementClass processClass = new ManagementClass(GetManagementScopeCimV2(hostname, authToken), new ManagementPath("Win32_Volume"), null))
                {
                    ManagementScope scope = GetManagementScopeCimV2(hostname, authToken);
                    string condition = "DriveLetter = '" + driveName + "'";
                    string[] selectedProperties = new string[] { "FreeSpace" };
                    SelectQuery query = new SelectQuery("Win32_Volume", condition, selectedProperties);

                    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                    using (ManagementObjectCollection results = searcher.Get())
                    {
                        ManagementObject volume = results.Cast<ManagementObject>().SingleOrDefault();

                        if (volume != null)
                        {
                            ulong freeSpace = (ulong)volume.GetPropertyValue("FreeSpace");
                            return freeSpace;
                        }
                        return 0;
                    }
                }
            }
            catch (COMException e)
            {
                return 0;
            }
        }
    }
}
