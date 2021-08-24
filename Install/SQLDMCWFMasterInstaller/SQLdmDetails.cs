using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Xml;
using CWFInstallerService;

namespace Installer_form_application
{
    public static class SQLdmDetails
    {
        private static bool isDmInstalledOnLocal = false;
        private static bool isUpgradationRequiredOnLocal = false;
        private static bool ifConsoleIsInstalledOnLocal = false;
        private static bool ifServicesAreInstalledOnLocal = false;

        private static string pathForInstalledVersion = string.Empty;
        private static string dBNameForInstalledVersion = string.Empty;
        private static string sqlInstanceNameForInstalledVersion = string.Empty;
        private static string installedVersion = string.Empty;

        public static void PopulateDetails()
        {
            //Get values from registry: SOFTWARE\Idera\SQLdm      
            var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32));
            RegistryKey regKey = hklm.OpenSubKey(@"SOFTWARE\Idera\SQLdm");
            if (regKey != null)
            {
                if(regKey.GetValue("Version") != null)
                    installedVersion = regKey.GetValue("Version").ToString();
                if(regKey.GetValue("Path") != null)
                    pathForInstalledVersion = regKey.GetValue("Path").ToString();
            }

            //Get values from management service config            
            if (pathForInstalledVersion != string.Empty)
            {
                string configPath = pathForInstalledVersion + @"\SQLdmManagementService.exe.config";
                if (File.Exists(pathForInstalledVersion + @"SQLdmManagementService.exe") && 
                    File.Exists(pathForInstalledVersion + @"SQLdmCollectionService.exe") &&
                    File.Exists(configPath))
                {
                    using (XmlTextReader reader = new XmlTextReader(configPath))
                    {
                        while (reader.Read())
                        {
                            if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "ManagementService"))
                            {
                                if (reader.GetAttribute("repositoryDatabase") != null)
                                    dBNameForInstalledVersion = reader.GetAttribute("repositoryDatabase");
                                if(reader.GetAttribute("repositoryServer") != null)
                                    sqlInstanceNameForInstalledVersion = reader.GetAttribute("repositoryServer");
                            }
                        }
                    }
                }
            }

            //Flags
            isDmInstalledOnLocal = SQLdmHelper.isSQLdmAlreadyInstalled();
            isUpgradationRequiredOnLocal = SQLdmHelper.CheckIfUpgrade();
            ifConsoleIsInstalledOnLocal = SQLdmDetails.CheckIfConsoleIsInstalledOnLocal();
            ifServicesAreInstalledOnLocal = SQLdmDetails.CheckIfServicesAreInstalledOnLocal();
        }

        public static bool IfConsoleIsInstalledOnLocal
        {
            get { return ifConsoleIsInstalledOnLocal; }
        }

        public static bool IfServicesAreInstalledOnLocal
        {
            get { return ifServicesAreInstalledOnLocal; }
        }
        
        public static string SqlInstanceName
        {
            get
            {
                return sqlInstanceNameForInstalledVersion;
            }
        }

        public static string DbName
        {
            get
            {
                return dBNameForInstalledVersion;
            }
        }

        public static bool IsLatestVersionInstalled
        {
            get
            {
                if (SQLdmDetails.InstalledVersion != string.Empty)
                {
                    Version installedVer = new Version(SQLdmDetails.InstalledVersion);
                    installedVer = new Version(installedVer.Major, installedVer.Minor, installedVer.Build);

                    Version installingVer = new Version(Properties.Settings.Default.SQLdmProductVersion);
                    installingVer = new Version(installingVer.Major, installingVer.Minor, installingVer.Build);

                    return (installedVer == installingVer);
                }
                return false;
            }
        }

        public static bool IsUpgradationRequiredOnLocal
        {
            get
            {                
                return isUpgradationRequiredOnLocal;
            }
        }

        public static string InstalledVersion
        {
            get
            {
                return installedVersion;           
            }
        }

        public static bool IsDmInstalledOnLocal
        {
            get
            {                
                return isDmInstalledOnLocal;
            }
        }
        public static string PathForInstalledVersion
        {
            get
            {
                return pathForInstalledVersion;
            }
        }

        public static bool ShouldServicesOptionBeEnabled
        {
            get
            {
                return !(SQLdmDetails.IsLatestVersionInstalled || (!SQLdmDetails.IfServicesAreInstalledOnLocal && SQLdmDetails.IfConsoleIsInstalledOnLocal));
            }
        }

        public static bool ShouldConsoleOptionBeEnabled
        {
            get
            {
                return !(SQLdmDetails.IsLatestVersionInstalled || (SQLdmDetails.IfServicesAreInstalledOnLocal && !SQLdmDetails.IfConsoleIsInstalledOnLocal));
            }
        }

        private static bool CheckIfConsoleIsInstalledOnLocal()
        {
            bool ifConsoleIsInstalledOnLocal = false;
            if (SQLdmDetails.PathForInstalledVersion == string.Empty)
                return false;
            string SQLdmInstallPath = SQLdmDetails.PathForInstalledVersion;// SQLdmDetails.PathForInstalledVersion;
            string desktopClientExePath = SQLdmInstallPath + @"SQLdmDesktopClient.exe";
            if (SQLdmInstallPath == String.Empty || !File.Exists(desktopClientExePath))
                ifConsoleIsInstalledOnLocal = false;
            else
                ifConsoleIsInstalledOnLocal = true;
            return ifConsoleIsInstalledOnLocal;            
        }

        private static bool CheckIfServicesAreInstalledOnLocal()
        {            
            bool ifServicesAreInstalledOnLocal = false;
            if (SQLdmDetails.PathForInstalledVersion == string.Empty)
                return false;
            string SQLdmInstallPath = SQLdmDetails.PathForInstalledVersion;// SQLdmDetails.PathForInstalledVersion;
            string managementServiceExePath = SQLdmInstallPath + @"SQLdmManagementService.exe";
            string collectionServiceExePath = SQLdmInstallPath + @"SQLdmCollectionService.exe";
            if (SQLdmInstallPath == String.Empty || !(File.Exists(managementServiceExePath) && File.Exists(collectionServiceExePath)))
                ifServicesAreInstalledOnLocal = false;
            else
                ifServicesAreInstalledOnLocal = true;
            return ifServicesAreInstalledOnLocal;                           
        }
    }
}
