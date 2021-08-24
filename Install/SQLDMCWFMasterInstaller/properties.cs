using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Installer_form_application
{
    public static class properties
    {
        public static string localDashboardUrl = string.Empty;
        public static string localDashboardServiceUsername = string.Empty;
        public static string localDashboardServicePassword = string.Empty;        
        public static string remoteDashboardUrl = string.Empty;
        public static string remoteDashboardServiceUsername = string.Empty;
        public static string remoteDashboardServicePassword = string.Empty;
        public static string remoteHostname = string.Empty;
        public static string remoteCoreServicePortNumber = string.Empty;
        public static string remoteDashbaordVersion = string.Empty;
        public static bool localRegister = false;
        public static bool remoteRegister = false;
        public static bool notRegister = true;
        // SQLDM-28056 (Varun Chopra) Creating Shortcuts for All users / Current Users
        public static bool allUsers = true;
        public static string dmInstallationPath = @"C:\Program Files\Idera\Idera SQL diagnostic manager";
        public static string idInstallationPath = @"C:\Program Files\Idera\Dashboard";
        public static bool installSQLDMConsoleOption = true; 
        public static bool installSQLDMServiceAndRepositoryOption = true;
        public static bool installDashboardOption = true;
        public static string DisplayName = string.Empty;
        public static bool useSameCreds = true;
        public static string IDSUsername = string.Empty;
        public static string IDSPassword = string.Empty;
        public static string SPSPassword = string.Empty;
        public static string SPSUsername = string.Empty;
        public static string CoreServicesPort = "9292";
        public static string WebAppMonitorPort = "9094";
        public static string WebAppServicePort = "9290";
        public static string WebAppSSLPort = "9291";
        public static bool needSqlAuthForConnectingToDmRepo = false;
        public static bool needSqlAuthForConnectingToDmService = false;
        public static bool needSqlAuthForConnectingToId = false;
        public static string IDDBName = "IderaDashboardRepository";
        public static string IDInstance = "(local)";
        public static string SQLUsernameID = string.Empty;
        public static string SQLPasswordID = string.Empty;
        public static string DMInstance = "(local)";
        public static string DMDBName = "SQLdmRepository";
        public static string SQLUsername_DM_Service = string.Empty;
        public static string SQLPassword_DM_Service = string.Empty;
        public static string SQLUsername_DM_Repo = string.Empty;
        public static string SQLPassword_DM_Repo = string.Empty;       
        public static bool AGREETOLICENSE = false;       
        public static string ValidLicenseKeys = string.Empty;
        public static bool isDisplayNameAvailable = false;
        public static string ShippingVersionOfDashboard = "4.6.0.40";  //Do not use past SQLDM 10.4.   The InstallationHelper.getCurrentVersion() in CWF needs to be changed to return the correct version!

        /// <summary>
        /// Maximum number of Retry attemps allowed for failed upgrades
        /// </summary>
        public const int MAXRETRYCOUNTSQLDMUPGRADE = 1;

        /// <summary>
        /// Indicates Current retry attempt for failed upgrades
        /// </summary>
        public static int RetryCountSQLdmUpgrade = 0;

        /// <summary>
        /// Set if Dm successfully installed
        /// </summary>
        public static bool CheckDmSuccess = true;

        /// <summary>
        /// To validate upgrade scenarios
        /// </summary>
        public static bool IsDmInstalledOnLocal = false;
    }
}
