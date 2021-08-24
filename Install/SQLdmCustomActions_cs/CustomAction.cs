using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Microsoft.Deployment.WindowsInstaller;
//using System.DirectoryServices.AccountManagement;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using BBS.License;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

namespace CustomActions
{
    public class CustomActions
    {
        const string CA_PROPERTY_HTTP_PORT = "WebAppPort";
        const string CA_PROPERTY_REST_PORT = "RestAppPort";
        const string CA_PROPERTY_INSTALL_FOLDER = "InstallFolder";
        const string SERVICE_NAME = "SQLdmWebApplicationService";
        const string SERVICE_ACCOUNT = "SERVICE_ACCOUNT";
        const string SERVICE_PASSWORD = "SERVICE_PASSWORD";
        //Start : SQL DM 9.0 (Vineet Kumar) License changes -- Adding constants for installshield properties
        const string EXISTING_KEYS = "EXISTING_KEYS";
        const string EXISTING_LICENSES = "EXISTING_LICENSES";
        const string MONITORED_INSTANCES = "MONITORED_INSTANCES";
        const string IS_NEW_KEY_REQUIRED = "IS_NEW_KEY_REQUIRED";
        const string INSTANCE = "INSTANCE";
        const string REPOSITORY = "REPOSITORY";
        const string NEW_LICENSE_KEYS = "NEW_LICENSE_KEYS";
        const string KEY_VALIDATION_ERR_MSG = "KEY_VALIDATION_ERR_MSG";
        //END : SQL DM 9.0 (Vineet Kumar) License changes -- Adding constants for installshield properties

        //Start : SQL DM 9.0 (Vineet Kumar) CWF changes -- Adding constants for CWF info properties
        const string CWF_HOSTNAME = "CWF_HOSTNAME";
        const string CWF_PORT = "CWF_PORT";
        const string CWF_SERVICE_USER_NAME = "CWF_SERVICE_USER_NAME";
        const string CWF_SERVICE_PASSWORD = "CWF_SERVICE_PASSWORD";
        const string CWF_INSTANCE_NAME = "CWF_INSTANCE_NAME";
        const string INSTALL_DIRECTORY = "INSTALL_DIRECTORY";
        const string InstallInfoFileName = "InstallInfo.ini";
        //End : SQL DM 9.0 (Vineet Kumar) CWF changes -- Adding constants for CWF info properties

        //Start : SQL DM 9.0 (Vineet Kumar) License and CWF changes -- Adding constants for CWF and license info properties for customactiondate
        const string CA_PROPERTY_CWF_HOSTNAME = "CWFHostname";
        const string CA_PROPERTY_CWF_PORT = "CWFPort";
        const string CA_PROPERTY_CWF_SERVICE_USER_NAME = "CWFServiceUserName";
        const string CA_PROPERTY_CWF_SERVICE_PASSWORD = "CWFServicePassword";
        const string CA_PROPERTY_CWF_INSTANCE_NAME = "CWFInstanceName";
        const string CA_PROPERTY_LICENSE_KEY = "LicenseKey";
        const string CA_PROPERTY_REPOSITORY = "Repository";
        //End : SQL DM 9.0 (Vineet Kumar) License and CWF changes -- Adding constants for CWF and license info properties for customactiondate

        //SQL DM 9.0 (Vineet Kumar) License changes -- Adding constant to define a format to copy existing license information.
        const string CopyExistingLicenseInfoFormat = "Existing License Key(s) : {0} \n SQLdm Repository : {1} \n Total Licenses : {2} \n Monitored Instances : {3}";

        const string RestServiceName = "SQLdmRestService$Default";
        public static string installDirect = string.Empty;

        /// <summary>
        /// SQL Dm 9.0 (Vineet Kumar) -- Fixing DE44144. A file in use message comes while upgrading. This custom action stops the web ui and rest service just at the time of upgrade starts.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        ///
        [CustomAction]
        public static ActionResult StopServices(Session session)
        {
            try
            {
                string installDir = session[INSTALL_DIRECTORY];

                session.Log("sqldmwebapplicationservice Stoping...");
                StopWebUIService(installDir);
                session.Log("sqldmwebapplicationservice Stoped");
                session.Log(RestServiceName + " Stoping...");
                StopRestService();
                session.Log(RestServiceName + " Stoped");
            }
            catch (Exception ex)
            {
                session.Log("Error occured in stopping services . Error:" + ex.Message);
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        /// <summary>
        /// SQL Dm 9.0 (Vineet Kumar) -- Method to stop Rest Service
        /// </summary>
        public static void StopRestService()
        {
                string registryKey = @"SYSTEM\CurrentControlSet\Services\" + RestServiceName;
                RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey);
                if (key != null)
                {
                    System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + "net stop " + RestServiceName);
                    procStartInfo.UseShellExecute = false;
                    procStartInfo.CreateNoWindow = true;
                    var p_StopRest = Process.Start(procStartInfo);
                    p_StopRest.WaitForExit();
                }
        }

        /// <summary>
        /// SQL Dm 9.0 (Vineet Kumar) -- Method to stop Web UI service
        /// </summary>
        /// <param name="installDir"></param>
        public static void StopWebUIService(string installDir)
        {

            string registryKey = @"SYSTEM\CurrentControlSet\Services\" + SERVICE_NAME;
            RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey);
            if (key != null)
            {
                var psi_stopService = new ProcessStartInfo(installDir + @"\WebApplication\SQLdm-webapp.exe")
                {
                    Arguments = "//SS//" + SERVICE_NAME,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = installDir
                };

                var p_stopService = Process.Start(psi_stopService);
                p_stopService.WaitForExit();
            }
        }

        /// <summary>
        /// SQL Dm 9.0 (Vineet Kumar) -- Method to get INI file content to be written while installation.
        /// </summary>
        /// <param name="installInfoObject"></param>
        /// <returns></returns>
        public static string GetIniFileContent(InstallInfo installInfoObject)
        {
            IniFile file = new IniFile();
            IniSection cwfSection = new IniSection("CWFInformation");
            cwfSection.Members.Add(new IniKeyValuePair("Host", installInfoObject.CWFHostName));
            cwfSection.Members.Add(new IniKeyValuePair("Port", installInfoObject.CWFPort));
            cwfSection.Members.Add(new IniKeyValuePair("ServiceUserName", installInfoObject.CWFServiceUserName));
            cwfSection.Members.Add(new IniKeyValuePair("ServicePassword", installInfoObject.CWFServicePassword));
            cwfSection.Members.Add(new IniKeyValuePair("InstanceName", installInfoObject.CWFInstanceName));
            file.Sections.Add(cwfSection);
            IniSection licenseSection = new IniSection("LicenseInformation");
            licenseSection.Members.Add(new IniKeyValuePair("LicenseKey", installInfoObject.LicenseKey));
            file.Sections.Add(licenseSection);
            return file.ToString();
        }


        
        /// <summary>
        /// //SQLdm 10.1 - Praveen Suhalka - Deregistring SQLdm from CWF when unstalling
        /// </summary>
        /// <param name="SQLdmInstallDir"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetSQLdmRepositoryDetails(string SQLdmInstallDir)
        {
            Dictionary<string, string> details = new Dictionary<string, string>();
            string configPath = Path.Combine(SQLdmInstallDir , "SQLdmManagementService.exe.config");
            using (XmlTextReader reader = new XmlTextReader(configPath))
            {
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "ManagementService"))
                    {

                        details["repositoryDatabase"] = reader.GetAttribute("repositoryDatabase") != null ? reader.GetAttribute("repositoryDatabase") : "";
                        details["repositoryServer"] = reader.GetAttribute("repositoryServer") != null ? reader.GetAttribute("repositoryServer") : "";
                        details["windowsAuthentication"] = reader.GetAttribute("windowsAuthentication") != null ? reader.GetAttribute("windowsAuthentication") : "";
                        details["repositoryUserName"] = reader.GetAttribute("repositoryUserName") != null ? reader.GetAttribute("repositoryUserName") : "";
                        details["repositoryPassword"] = reader.GetAttribute("repositoryPassword") != null ? reader.GetAttribute("repositoryPassword") : "";
                    }
                }
            }
            return details;
        }

        /// <summary>
        /// SQL Dm 9.0 (Vineet Kumar) License Changes -- Adding custom action to write new license info and cwf info to file while upgrading to 9.0
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult WriteUpgradeInfoToFile(Session session)
        {
            try
            {
                InstallInfo installInfoObject = new InstallInfo();
                installInfoObject.CWFHostName = session[CWF_HOSTNAME];
                installInfoObject.CWFPort = session[CWF_PORT];
                installInfoObject.CWFServiceUserName = session[CWF_SERVICE_USER_NAME];
                installInfoObject.CWFServicePassword = session[CWF_SERVICE_PASSWORD];
                installInfoObject.CWFInstanceName = session[CWF_INSTANCE_NAME];
                installInfoObject.LicenseKey = session[NEW_LICENSE_KEYS];
                string installDirectory = session[INSTALL_DIRECTORY];
                string installInfoFileNameFull = installDirectory + InstallInfoFileName;
                string fileContent = GetIniFileContent(installInfoObject);
                FileStream fs = new FileStream(installInfoFileNameFull, FileMode.OpenOrCreate);
                using (StreamWriter myStream = new StreamWriter(fs))
                {
                    myStream.Write(fileContent);
                }
                fs.Close();
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log("Error occured in WriteInstallationInfo . Error:" + ex.Message + "\n" + ex.StackTrace);
                return ActionResult.Failure;
            }
        }

        public static ActionResult ModifyConfigurationFileForStartupParameter(String param, Session session)
        {

            try
            {
                string installDirectory = session.CustomActionData["/InstallDir"]; //session[INSTALL_DIRECTORY];
                installDirect = installDirectory;
                if (File.Exists(Path.Combine(installDirectory, param)))
                {
                    using (StreamReader sr = new StreamReader(Path.Combine(installDirectory, param)))
                    {
                        String content = sr.ReadToEnd();
                        //bool shouldAppend = content.IndexOf("<startup>") == -1 && content.IndexOf("<startup ") == -1;
                        sr.Close();
                        content = content.Replace("<startup/>", string.Empty);
                        content = Regex.Replace(content, "<startup.*</startup>", string.Empty);
                        content = content.Replace("</configuration>", "<startup useLegacyV2RuntimeActivationPolicy=\"true\"><supportedRuntime version=\"v4.0\" sku=\".NETFramework,Version=v4.0\"/></startup></configuration>");
                        using (StreamWriter sw = new StreamWriter(Path.Combine(installDirectory, param)))
                        {
                            sw.WriteLine(content);
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                session.Log("Error while modifying " + param + " (Reason: " + ex.Message + ")");
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ExtraConfigurationToDesktopClient(Session session)
        {
            return ModifyConfigurationFileForStartupParameter("SQLdmDesktopClient.exe.config", session);
        }

        [CustomAction]
        public static ActionResult ExtraConfigurationToCollectionService(Session session)
        {
            return ModifyConfigurationFileForStartupParameter("SQLdmCollectionService.exe.config", session);
        }

        [CustomAction]
        public static ActionResult ExtraConfigurationToMagamenetService(Session session)
        {
            return ModifyConfigurationFileForStartupParameter("SQLdmManagementService.exe.config", session);
        }

        [CustomAction]
        public static ActionResult ExtraConfigurationToPredictiveAnalyticsService(Session session)
        {
            return ModifyConfigurationFileForStartupParameter("SQLdmPredictiveAnalyticsService.exe.config", session);
        }

        [CustomAction]
        public static ActionResult ExtraConfigurationToRegistrationService(Session session)
        {
            return ModifyConfigurationFileForStartupParameter("SQLdmRegistrationService.exe.config", session);
        }

        /// <summary>
        /// SQL Dm 9.0 (Vineet Kumar) CWF Changes -- Adding custom action to write cwf info to file for fresh installation
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult WriteInstallInfoToFile(Session session)
        {
            try
            {
                InstallInfo installInfoObject = new InstallInfo();
                installInfoObject.CWFHostName = session.CustomActionData[CA_PROPERTY_CWF_HOSTNAME];
                installInfoObject.CWFPort = session.CustomActionData[CA_PROPERTY_CWF_PORT];
                installInfoObject.CWFServiceUserName = session.CustomActionData[CA_PROPERTY_CWF_SERVICE_USER_NAME];
                installInfoObject.CWFServicePassword = session.CustomActionData[CA_PROPERTY_CWF_SERVICE_PASSWORD];
                installInfoObject.CWFInstanceName = session.CustomActionData[CA_PROPERTY_CWF_INSTANCE_NAME];
                installInfoObject.LicenseKey = session.CustomActionData[CA_PROPERTY_LICENSE_KEY];
                string installDirectory = session.CustomActionData[CA_PROPERTY_INSTALL_FOLDER];
                string installInfoFileNameFull = installDirectory + InstallInfoFileName;
                string fileContent = GetIniFileContent(installInfoObject);
                FileStream fs = new FileStream(installInfoFileNameFull, FileMode.OpenOrCreate);
                using (StreamWriter myStream = new StreamWriter(fs))
                {
                    myStream.Write(fileContent);
                }
                fs.Close();
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log("Error occured in WriteInstallationInfo . Error:" + ex.Message + "\n" + ex.StackTrace);
                return ActionResult.Failure;
            }
        }

        /// <summary>
        /// SQL Dm 9.0 (Vineet Kumar) License Changes -- Adding custom action to fetch the existing license info
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult GetExistingLicenses(Session session)
        {
            try
            {
                string instance = session[INSTANCE];
                string database = session[REPOSITORY];

                LicenseSummary license = RepositoryHelper.GetLicenseSummary(session, instance, database);
                string keys = String.Empty;
                foreach (var key in license.CheckedKeys)
                {
                    if (!string.IsNullOrEmpty(keys))
                        keys += ";";
                    keys += key.KeyString;
                }
                session[EXISTING_KEYS] = keys;
                session[EXISTING_LICENSES] = license.LicensedServers.ToString();
                session[MONITORED_INSTANCES] = license.MonitoredServers.ToString();
                session[IS_NEW_KEY_REQUIRED] = "0";
                if (license.IsTrial)
                {
                    //if existing license is trial, it should continue to use that
                    session[IS_NEW_KEY_REQUIRED] = "0";
                }
                else
                {
                    session[IS_NEW_KEY_REQUIRED] = "1";
                    if (license.Status == LicenseStatus.OK)
                    {
                        //if license state is OK and version is >=9 it should continue to use existing key
                        if (license.ProductVersion.Major >= 9)
                            session[IS_NEW_KEY_REQUIRED] = "0";
                    }
                }
                if (license.Status == LicenseStatus.CountExceeded)
                {
                    session[IS_NEW_KEY_REQUIRED] = "1";
                }
                else if (license.Status == LicenseStatus.Expired)
                {
                    session[IS_NEW_KEY_REQUIRED] = "1";
                }
                else if (license.Status == LicenseStatus.NoValidKeys)
                {
                    session[IS_NEW_KEY_REQUIRED] = "1";
                }
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log("Error occured in GetExistingLicenses . Error:" + ex.Message + "\n" + ex.StackTrace);
                return ActionResult.Failure;
            }
        }

        /// <summary>
        /// SQL Dm 9.0 (Vineet Kumar) License Changes -- Adding custom action to validate new entered key. Key must be a 9.0 or greater version production key with sufficient licenses
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult ValidateNewLicenseKeys(Session session)
        {
            try
            {
                string errorMessage = string.Empty;
                string instance = session[INSTANCE];
                string database = session[REPOSITORY];
                int registeredServers = 0;
                int.TryParse(session[MONITORED_INSTANCES], out registeredServers);
                string newLicenseKeyString = session[NEW_LICENSE_KEYS];

                if (string.IsNullOrEmpty(newLicenseKeyString))
                {
                    errorMessage = "Please enter atleast one license key";
                    session[KEY_VALIDATION_ERR_MSG] = errorMessage;
                    return ActionResult.Success;
                }

                //Multiple keys will be seperated by semi colon (;)
                string[] delimiter = new string[] { "," };
                string[] newLicenseKeys = newLicenseKeyString.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                List<string> keyList = new List<string>();
                foreach (string key in newLicenseKeys)
                {
                    keyList.Add(key);
                }

                if (newLicenseKeys.Length == 0)
                {
                    errorMessage = "Please enter atleast one license key";
                    session[KEY_VALIDATION_ERR_MSG] = errorMessage;
                    return ActionResult.Success;
                }
                else
                {
                    LicenseSummary license = LicenseSummary.SummarizeKeys(
                          registeredServers,
                          instance,
                          keyList);
                    if (license.Status == LicenseStatus.OK)
                    {
                        if (license.IsTrial)
                            errorMessage = "A trial key can not be entered. Please enter a production key to continue.";
                        else
                        {
                            if (license.ProductVersion.Major < 9)
                                errorMessage = "The license key is for an older version of SQL diagnostic manager.  Please visit the customer portal to acquire a new license key at http://www.idera.com/licensing.";
                            else
                                foreach (var key in license.CheckedKeys)
                                {
                                    if (!key.IsValid)
                                        errorMessage = "Invalid Key : " + key.KeyString + " : " + key.Comment;
                                }
                        }
                    }
                    else if (license.Status == LicenseStatus.CountExceeded)
                    {
                        errorMessage = "The number of currently monitored servers is greater than the number allowed by the specified key(s).";
                    }
                    else if (license.Status == LicenseStatus.Expired)
                    {
                        errorMessage = "The entered key has already expired.";
                    }
                    else if (license.Status == LicenseStatus.NoValidKeys)
                    {
                        errorMessage = "The specified license key(s) is invalid.";
                    }

                }
                session[KEY_VALIDATION_ERR_MSG] = errorMessage;
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log("Error occured in ValidateNewLicenseKeys . Error:" + ex.Message + "\n" + ex.StackTrace);
                return ActionResult.Failure;
            }
        }

        /// <summary>
        /// SQL Dm 9.0 (Vineet Kumar) License Changes -- Adding cutom action to copy existing license information.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult CopyExistingLicenseInfo(Session session)
        {
            try
            {
                Native.OpenClipboard(IntPtr.Zero);
                Native.EmptyClipboard();
                var licenseInfo = string.Format(CopyExistingLicenseInfoFormat, session[EXISTING_KEYS], session[REPOSITORY], session[EXISTING_LICENSES], session[MONITORED_INSTANCES]);
                var ptr = Marshal.StringToHGlobalUni(licenseInfo);
                Native.SetClipboardData(13, ptr);
                Native.CloseClipboard();
                Marshal.FreeHGlobal(ptr);
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log("Error occured in CopyExistingLicenseInfo . Error:" + ex.Message + "\n" + ex.StackTrace);
                return ActionResult.Success;//Returns success because we dont want to fail installation if copy is not successfull
            }
        }

        /// <summary>
        ///SQLdm 9.0 (Vineet Kumar) --  Custom Action To delete Rest Service
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult DeleteRestService(Session session)
        {
            try
            {
                session.Log(RestServiceName + " Deleting...");
                string registryKey = @"SYSTEM\CurrentControlSet\Services\" + RestServiceName;
                RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey);
                if (key != null)
                {
                    System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + "sc delete " + RestServiceName);
                    procStartInfo.UseShellExecute = false;
                    procStartInfo.CreateNoWindow = true;
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo = procStartInfo;
                    proc.Start();
                }
                session.Log(RestServiceName + " Deleted");

            }
            catch (Exception ex)
            {
                session.Log("Error occured in stopping " + RestServiceName + ". Error:" + ex.Message);
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult DeleteWebUIService(Session session)
        {
            try
            {
                string installDir = session.CustomActionData[CA_PROPERTY_INSTALL_FOLDER];

                session.Log("sqldmwebapplicationservice Deleting...");
                DeleteWebUIService(installDir);
                session.Log("sqldmwebapplicationservice Deleted");
            }
            catch (Exception ex)
            {
                session.Log("Error occured in stopping " + SERVICE_NAME + ". Error:" + ex.Message);
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        public static void DeleteWebUIService(string installDir)
        {

            string registryKey = @"SYSTEM\CurrentControlSet\Services\" + SERVICE_NAME;
            RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey);
            if (key != null)
            {
                var psi_deleteService = new ProcessStartInfo(installDir + @"\WebApplication\SQLdm-webapp.exe")
                {
                    Arguments = "//DS//" + SERVICE_NAME,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = installDir
                };

                var p_deleteService = Process.Start(psi_deleteService);
                p_deleteService.WaitForExit();
            }
        }
        [CustomAction]
        public static ActionResult CACreateWebPropertiesConfigRollback(Session session)
        {
            try
            {
                string installDir = session.CustomActionData[CA_PROPERTY_INSTALL_FOLDER];

                session.Log("sqldmwebapplicationservice Deleting...");
                DeleteWebUIService(installDir);
                session.Log("sqldmwebapplicationservice Deleted");
            }
            catch (Exception ex)
            {
                session.Log("Rolling Back - Error occured in stopping " + SERVICE_NAME + ". Error:" + ex.Message);
                return ActionResult.Failure;
            }

            return ActionResult.Success;

        }
        [CustomAction]
        public static ActionResult CACreateWebPropertiesConfig(Session session)
        {
            //Debugger.Launch();
            try
            {
                // Get web port and URL properties.
                string webPort = session.CustomActionData[CA_PROPERTY_HTTP_PORT];
                string restPort = session.CustomActionData[CA_PROPERTY_REST_PORT];
                string installDir = session.CustomActionData[CA_PROPERTY_INSTALL_FOLDER];
                installDirect = installDir;
                restPort = restPort.Replace("{5}", "").Trim();
                webPort = webPort.Replace("{5}", "").Trim();
                installDir = installDir.Trim();
                CAWebPropertiesConfigChanges(installDir, webPort, restPort);
                session.Log("Completed Config Changes. Web Port: " + webPort + " Rest Port: " + restPort + " InstallFolder = " + installDir);

                // Install service
                string account = session.CustomActionData[SERVICE_ACCOUNT];
                string password = session.CustomActionData[SERVICE_PASSWORD]; //This is needed for Process.Start.  Removing the logging.

                string procRunClassPath = installDir + @"\WebApplication\idera-lib\lib\annotations-api.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\ant.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\antlr-2.7.6.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\bcprov-jdk16-140.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\bsh.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\catalina.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\commons-cli-1.0-beta-2-dev.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\commons-codec-1.6.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\commons-fileupload.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\commons-lang-2.4.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\commons-logging-1.1.1.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\derby.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\derbytools.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\httpclient-4.2.5.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\httpcore-4.2.4.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\i18nlog-1.0.9.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\jackson-annotations-2.2.0.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\jackson-core-2.2.0.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\jackson-databind-2.2.0.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\log4j-1.2.15.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\org.springframework.binding-2.0.8.RELEASE.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\org.springframework.js-2.0.8.RELEASE.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\servlet-api-2.5-6.1.7.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\servlet-api.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\spring-aspects.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\spring-jms-2.5.6.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\spring-security-acl-2.0.5.RELEASE.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\spring-security-core-2.0.5.RELEASE.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\spring-security-core-tiger-2.0.5.RELEASE.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\spring-security-taglibs-2.0.5.RELEASE.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\spring.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\tomcat-coyote.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\tomcat-dbcp.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\tomcat-juli.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\idera-lib.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\idera-core.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\idera-common.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\idera-main.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\commons-beanutils-1.7.0.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\commons-collections-3.2.1.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\commons-digester-2.1.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\DynamicJasper-5.0.0.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\jasperreports-5.5.0.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\jasperreports-fonts-5.5.0.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\itext-2.1.7.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\poi-3.10-FINAL-20140208.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\commons-io-2.4.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\opencsv-2.3.jar;"
                                            + installDir + @"\WebApplication\idera-lib\lib\jdt-compiler-3.1.1.jar;"
                                            + installDir + @"\WebApplication\idera-main\conf";

                string LogPath = installDir + @"\WebApplication\log";
                string StartClass = "com.idera.Main";
                string StartMode = "java";
                string StartParams = "start";
                string StartPath = installDir + @"\WebApplication\idera-main";
                string StopMode = "java";
                string StopClass = "com.idera.Main";
                string StopParams = "stop";
                string StopPath = installDir + @"\WebApplication\idera-main";
                string JavaHome = installDir + @"\WebApplication\JRE";
                string ServiceCreds = string.Empty;
                string Description = "Provides UI Hosting for SQLdm";
                string startup = "auto";
                string displayName = "SQLdm WebUI Service (Default)";


                // only add service Creds if specified
                if (!string.IsNullOrEmpty(account))
                {
                    ServiceCreds = string.Format("--ServiceUser={0} --ServicePassword={1}", account, password);
                }

                string ProcessArgs = string.Format(@"//IS//{0} {1} --Description=""{2}"" --DisplayName=""{3}"" --Startup={4} --LogPath=""{5}"" --StartClass={6} --StartMode={7} ++StartParams={8} --StartPath=""{9}"" --StopMode={10} --StopClass={11} ++StopParams={12} --StopPath=""{13}"" --JavaHome=""{14}"" --Classpath=""{15}""",
                     SERVICE_NAME, ServiceCreds, Description, displayName, startup, LogPath, StartClass, StartMode, StartParams, StartPath, StopMode, StopClass, StopParams, StopPath, JavaHome, procRunClassPath);

                string ProcessArgsForLog = string.Format(@"//IS//{0} {1} --Description=""{2}"" --DisplayName=""{3}"" --Startup={4} --LogPath=""{5}"" --StartClass={6} --StartMode={7} ++StartParams={8} --StartPath=""{9}"" --StopMode={10} --StopClass={11} ++StopParams={12} --StopPath=""{13}"" --JavaHome=""{14}"" --Classpath=""{15}""",
                     SERVICE_NAME, "service_credentials_here", Description, displayName, startup, LogPath, StartClass, StartMode, StartParams, StartPath, StopMode, StopClass, StopParams, StopPath, JavaHome, procRunClassPath);

                //remove the logging to get the build running.
                session.Log("Creating sqldmwebapplicationservice with args=" + ProcessArgsForLog);

                // Check if service is already present and installed
                string registryKey = @"SYSTEM\CurrentControlSet\Services\" + SERVICE_NAME;
                RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey);
                if (key != null)
                {
                    session.Log("sqldmwebapplicationservice found. Removing...");
                    var psi_uninstall = new ProcessStartInfo(installDir + @"\WebApplication\SQLdm-webapp.exe")
                    {
                        Arguments = "//DS//" + SERVICE_NAME,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WorkingDirectory = installDir
                    };

                    var p_uninstall = Process.Start(psi_uninstall);
                    p_uninstall.WaitForExit();
                    session.Log("sqldmwebapplicationservice found. Removed.");
                }

                // install new
                try
                {
                    var psi_install = new ProcessStartInfo(installDir + @"\WebApplication\SQLdm-webapp.exe")
                    {
                        Arguments = ProcessArgs,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WorkingDirectory = installDir
                    };

                    var p_install = Process.Start(psi_install);
                    p_install.WaitForExit();

                }
                catch (Exception installEx)
                {
                    session.Log("installation web app fail occured while setting up web properties " + SERVICE_NAME + ". Warning:" + installEx.Message);
                    return ActionResult.Success;
                }
            }

            catch (Exception ex)
            {
                session.Log("Error occured in setting up " + SERVICE_NAME + ". Error:" + ex.Message);
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult CAWebPropertiesConfigChanges(string installDir, string webAppPort, string restPort)
        {
            const string WEB_PROPERTIES_HTTP_PORT = "http-port";
            const string WEB_PROPERTIES_WEBAPP_URL = "IderaSqlDMServicesBaseUrl";
            string WEB_APP_FOLDER = installDir + "\\WebApplication";
            string WEB_PROPERTES_FILE = WEB_APP_FOLDER + @"\idera-main\conf\web.properties";
            string WEB_PROPERTES_BACKUP_FILE = WEB_APP_FOLDER + @"\idera-main\conf\web.properties.bak";
            string webAppBaseUrl = string.Format("http://{0}:{1}/SQLdm/v1", Dns.GetHostName(), restPort);

            try
            {
                if (File.Exists(WEB_PROPERTES_FILE))
                {
                    string webAppConfig = File.ReadAllText(WEB_PROPERTES_FILE);
                    webAppConfig = Regex.Replace(webAppConfig, WEB_PROPERTIES_HTTP_PORT + @"\s{0,}=\s{0,}.*\n", WEB_PROPERTIES_HTTP_PORT + "=" + webAppPort + Environment.NewLine);
                    webAppConfig = Regex.Replace(webAppConfig, WEB_PROPERTIES_WEBAPP_URL + @"\s{0,}=\s{0,}.*\n", WEB_PROPERTIES_WEBAPP_URL + "=" + webAppBaseUrl + Environment.NewLine);
                    File.WriteAllText(WEB_PROPERTES_FILE, webAppConfig);
                }

                else
                {
                    CACreateWebPropertiesConfigFile(installDir, webAppPort, webAppBaseUrl);
                }

                if (File.Exists(WEB_PROPERTES_BACKUP_FILE))
                {
                    string webAppConfig = File.ReadAllText(WEB_PROPERTES_FILE);
                    webAppConfig = Regex.Replace(webAppConfig, WEB_PROPERTIES_HTTP_PORT + @"\s{0,}=\s{0,}.*\n", WEB_PROPERTIES_HTTP_PORT + "=" + webAppPort + Environment.NewLine);
                    webAppConfig = Regex.Replace(webAppConfig, WEB_PROPERTIES_WEBAPP_URL + @"\s{0,}=\s{0,}.*\n", WEB_PROPERTIES_WEBAPP_URL + "=" + webAppBaseUrl + Environment.NewLine);
                    File.WriteAllText(WEB_PROPERTES_FILE, webAppConfig);
                }
            }
            catch
            {
                throw;
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult CACreateWebPropertiesConfigFile(string installDir, string webAppPort, string webAppBaseUrl)
        {
            const string WEB_PROPERTIES_HTTP_PORT = "http-port=";
            const string WEB_PROPERTIES_WEBAPP_URL = "IderaSqlDMServicesBaseUrl=";
            string WEB_APP_FOLDER = installDir + "\\WebApplication";
            string WEB_PROPERTES_FILE = WEB_APP_FOLDER + @"\idera-main\conf\web.properties";
            string WEB_PROPERTES_BACKUP_FILE = WEB_APP_FOLDER + @"\idera-main\conf\web.properties.bak";

            try
            {
                // Delete the file if it exists.
                if (File.Exists(WEB_PROPERTES_FILE))
                {
                    File.Delete(WEB_PROPERTES_FILE);
                }

                // Write to it.
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(WEB_PROPERTES_FILE))
                {
                    sw.WriteLine(WEB_PROPERTIES_HTTP_PORT + webAppPort);
                    sw.WriteLine(WEB_PROPERTIES_WEBAPP_URL + webAppBaseUrl);
                }

                //prepare a backup
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(WEB_PROPERTES_BACKUP_FILE))
                {
                    sw.WriteLine(WEB_PROPERTIES_HTTP_PORT + webAppPort);
                    sw.WriteLine(WEB_PROPERTIES_WEBAPP_URL + webAppBaseUrl);
                }
            }
            catch (Exception ex)
            {
                return ActionResult.Failure;
            }

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult CAVerifyPorts(Session session)
        {
            const string VALID_PORT_NUMBER_HINT = "Ensure you specify an unused port number between 1 and 65535.";
            string msg = string.Empty;

            // Get the two port numbers.
            string restServicePort = session["RESTSERVICEPORT"];
            string webServicePort = session["WEBSERVICEPORT"];
            if (string.IsNullOrEmpty(restServicePort) || string.IsNullOrEmpty(webServicePort))
            {
                msg = "One or more port numbers are not specified.  " + VALID_PORT_NUMBER_HINT;
            }

            // Conver the port numbers to integers.
            int restPort = 0, webPort = 0;
            if (string.IsNullOrEmpty(msg))
            {
                try
                {
                    restPort = Convert.ToInt32(restServicePort);
                    webPort = Convert.ToInt32(webServicePort);
                }
                catch //(Exception ex)
                {
                    msg = "One or more port numbers specified are not valid numbers.  " + VALID_PORT_NUMBER_HINT;
                }
            }

            // Validate port number values.
            if (string.IsNullOrEmpty(msg))
            {
                if (restPort < 1 || restPort > 65535 || webPort < 1 || webPort > 65535)
                {
                    msg = "One or more port numbers specified are not valid.   " + VALID_PORT_NUMBER_HINT;
                }
            }

            // Same port number values.
            if (string.IsNullOrEmpty(msg))
            {
                if (restPort == webPort)
                {
                    msg = "The Collection and Web Application service port numbers are the same.   Ensure you specify different port numbers for both the services.";
                }
            }

            // Check if port number is in use.
            if (string.IsNullOrEmpty(msg))
            {
                bool isRestInUse = isPortInUse(restPort);
                bool isWebInUse = isPortInUse(webPort);

                if (isRestInUse && isWebInUse)
                {
                    msg = "The REST and Web Application service ports are in use. " + VALID_PORT_NUMBER_HINT;
                }
                else if (isRestInUse)
                {
                    msg = "The REST service port is in use. " + VALID_PORT_NUMBER_HINT;
                }
                else if (isWebInUse)
                {
                    msg = "The Web Application service port is in use. " + VALID_PORT_NUMBER_HINT;
                }
            }

            session["PORT_NUMBERS_VERIFIED"] = string.IsNullOrEmpty(msg) ? "1" : "0";
            session["PORT_NUMBER_VERIFY_ERRORMSG"] = string.IsNullOrEmpty(msg) ? "" : msg;

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult CADeregister(Session session)
        {
            session.Log("Entered CADeregister Custom Action");

            string installdirectory = Path.GetFullPath(@"C:\Program Files\Idera\Idera SQL diagnostic manager");
            if (string.IsNullOrEmpty(installDirect))
                try
                {
                    installdirectory = session.CustomActionData["/InstallDir"];
                }
                catch (Exception ex)
                {
                    session.Log("Could not get install directory. Using default. Error details: " + ex.Message);
                }
            else
                installdirectory = installDirect;
            try
            {
                session.Log("installdier: " + installdirectory);
                Dictionary<string, string> repoDetails = GetSQLdmRepositoryDetails(installdirectory);
                foreach (KeyValuePair<string, string> vk in repoDetails)
                {
                    session.Log(vk.Key + " : " + vk.Value);
                }
                Dictionary<string, string> cwfDetails = RepositoryHelper.GetRegisteredCWFInformation(repoDetails);
                if (cwfDetails != null && cwfDetails.Count > 0)
                {
                    foreach (KeyValuePair<string, string> vk in cwfDetails)
                    {
                        session.Log(vk.Key + " : " + vk.Value);
                    }
                    CwfHelper cwfHelper = new CwfHelper(cwfDetails);
                    cwfHelper.DeRegisterProduct();
                    RepositoryHelper.RemoveCWFInformation(repoDetails);
                }
                else
                {
                    session.Log("Skipping Deregister with Idera Dashboard. There is no registration information found in repository");
                }
            }
            catch (Exception ex)
            {
                session.Log("Deregister with Idera Dashboard is unsuccessfull. CADeregister exception : " + ex.Message + ex.StackTrace);
            }
            return ActionResult.Success;
        }

        private static bool isPortInUse(int port)
        {
            bool inUse = false;

            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endpoint in tcpConnInfoArray)
            {
                if (endpoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }

            return inUse;
        }

    }
}