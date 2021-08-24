﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CWFInstallerService;

namespace Installer_form_application
{
    public static class IderaDashboardDetails
    {
        private static string accountForInstalledVersion = string.Empty;
        private static string pathForInstalledVersion = string.Empty;
        private static string dBNameForInstalledVersion = string.Empty;
        private static string sqlInstanceNameForInstalledVersion = string.Empty;
        private static string servicePortForInstalledVersion = string.Empty;
        private static string webAppMonitorPortForInstalledVersion = string.Empty;
        private static string installedVersion = string.Empty;
        private static bool isDashboardInstalledOnLocal = false;
        private static bool isUpgradationRequiredOnLocal = false;

        public static bool IsDashboardInstalledOnLocal
        {
            get
            {                
                return isDashboardInstalledOnLocal;
            }
        }

        public static bool IsLatestVersionInstalledOnLocal
        {
            get
            {
                if (IderaDashboardDetails.InstalledVersion != string.Empty)
                {
                    Version installedVer = new Version(IderaDashboardDetails.InstalledVersion);
                    installedVer = new Version(installedVer.Major, installedVer.Minor, installedVer.Build);

                    Version installingVer = new Version(properties.ShippingVersionOfDashboard);  //Do not use past SQLDM 10.4.   The InstallationHelper.getCurrentVersion() in CWF needs to be changed to return the correct version!
                    installingVer = new Version(installingVer.Major, installingVer.Minor, installingVer.Build);

                    return IderaDashboardDetails.IsDashboardInstalledOnLocal && (installedVer == installingVer);
                }
                return false;
            }
        }

        public static bool IsRestrictedVersionInstalledOnLocal
        {
            get
            {
                if (IderaDashboardDetails.InstalledVersion != string.Empty)
                {
                    Version installedVer = new Version(IderaDashboardDetails.InstalledVersion);
                    installedVer = new Version(installedVer.Major, installedVer.Minor, installedVer.Build);

                    Version restrictedVersion = new Version(Properties.Settings.Default.RestrictedIDVersionsForLocal);
                    restrictedVersion = new Version(restrictedVersion.Major, restrictedVersion.Minor, restrictedVersion.Build);

                    return IderaDashboardDetails.IsDashboardInstalledOnLocal && (installedVer <= restrictedVersion);
                }
                return false;   
            }
        }

        public static bool IsUpgradationRequiredOnLocal
        {
            get
            {
                if (IderaDashboardDetails.InstalledVersion != string.Empty)
                {
                    Version installedVer = new Version(IderaDashboardDetails.InstalledVersion);
                    installedVer = new Version(installedVer.Major, installedVer.Minor, installedVer.Build);

                    Version installingVer = new Version(properties.ShippingVersionOfDashboard);    //Do not use past SQLDM 10.4.   The InstallationHelper.getCurrentVersion() in CWF needs to be changed to return the correct version!
                    installingVer = new Version(installingVer.Major, installingVer.Minor, installingVer.Build);

                    Version restrictedVersion = new Version(Properties.Settings.Default.RestrictedIDVersionsForLocal);
                    restrictedVersion = new Version(restrictedVersion.Major, restrictedVersion.Minor, restrictedVersion.Build);

                    if (IderaDashboardDetails.IsDashboardInstalledOnLocal && (installedVer < installingVer) && (installedVer > restrictedVersion))
                        isUpgradationRequiredOnLocal = true;
                    else
                        isUpgradationRequiredOnLocal = false;
                    return isUpgradationRequiredOnLocal;
                }
                return true;
            }
        }

        public static string InstalledVersion
        {
            get
            {                
                return installedVersion;
            }
        }
        public static string WebAppMonitorPortForInstalledVersion
        {
            get
            {                
                return webAppMonitorPortForInstalledVersion;
            }
        }

        public static string ServicePortForInstalledVersion 
        {
            get
            {                
                return servicePortForInstalledVersion;
            }
        }

        public static string SqlInstanceNameForInstalledVersion
        {
            get
            {                
                return sqlInstanceNameForInstalledVersion;
            }
        }

        public static string DBNameForInstalledVersion
        {
            get
            {                
                return dBNameForInstalledVersion;
            }
        }

        public static string PathForInstalledVersion
        {
            get
            {                
                return pathForInstalledVersion;
            }
        }

        public static string AccountForInstalledVersion
        {
            get
            {                
                return accountForInstalledVersion;
            }
        }

        public static bool ShouldDashboardOptionBeEnabled
        {
            get
            {
                return !IderaDashboardDetails.IsRestrictedVersionInstalledOnLocal;
            }
        }

        public static void PopulateDetails()
        {
            
            Dictionary<string, string> installDetails = InstallationHelper.checkIfDashboardIsAlreadyInstalled();
            if (installDetails != null || installDetails.Count > 0)
            {
                if (installDetails.ContainsKey("isInstalled"))
                    isDashboardInstalledOnLocal = Convert.ToBoolean(installDetails["isInstalled"]);
                else
                    isDashboardInstalledOnLocal = false;               

                if (installDetails.ContainsKey("Account"))
                    accountForInstalledVersion = installDetails["Account"];
                else
                    accountForInstalledVersion = string.Empty;

                if (installDetails.ContainsKey("ProductVersion"))                
                    installedVersion = installDetails["ProductVersion"]; 
                else if(installDetails.ContainsKey("Version"))  //Supporting registry read, in case of dashboard ver <= 2.1.0
                    installedVersion = installDetails["Version"];
                else               
                    installedVersion = string.Empty;

                if (installDetails.ContainsKey("InstallDir"))
                {
                    pathForInstalledVersion = installDetails["InstallDir"];
                    Dictionary<string, string> configValues = new Dictionary<string, string>();
                    try
                    {
                        configValues = InstallationHelper.getOldConfigValuesFromLocal(pathForInstalledVersion);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("'IderaDashboardCoreService.exe.config' could not be parsed OR found: " + ex.Message);
                    }

                    if (configValues != null && configValues.ContainsKey("RepositoryHost"))
                        sqlInstanceNameForInstalledVersion = configValues["RepositoryHost"];
                    else
                        sqlInstanceNameForInstalledVersion = String.Empty;

                    if (configValues != null && configValues.ContainsKey("RepositoryDatabase"))
                        dBNameForInstalledVersion = configValues["RepositoryDatabase"];
                    else
                        dBNameForInstalledVersion = String.Empty;

                    if (configValues != null && configValues.ContainsKey("ServicePort"))
                        servicePortForInstalledVersion = configValues["ServicePort"];
                    else
                        servicePortForInstalledVersion = String.Empty;

                    if (configValues != null && configValues.ContainsKey("WebAppMonitorPort"))
                        webAppMonitorPortForInstalledVersion = configValues["WebAppMonitorPort"];
                    else
                        webAppMonitorPortForInstalledVersion = String.Empty;
                }
            }
            else
            {
                isDashboardInstalledOnLocal = false;
                isUpgradationRequiredOnLocal = false;
                accountForInstalledVersion = string.Empty;
                installedVersion = string.Empty;
                pathForInstalledVersion = string.Empty;
                sqlInstanceNameForInstalledVersion = string.Empty;
                dBNameForInstalledVersion = string.Empty;
                servicePortForInstalledVersion = string.Empty;
                webAppMonitorPortForInstalledVersion = string.Empty;
            }
        }
    }
}
