using System.Collections.Generic;

namespace Idera.SQLdm.Common
{
    public static class Constants
    {
        // The valid repository schema version
        //public const string ValidRepositorySchemaVersion = "8.0.1";
        //upgrading to 9.0(Gaurav Karwal)
		//upgrading to 9.1 (Ankit Srivastava)
        //SQLdm 10.1 - (Praveen Suhalka)
        public const string ValidRepositorySchemaVersion = "11.1";


        //SQL DM 9.0 (Vineet Kumar) (License Changes) -- Moving the link for customer portal to constants so that it can be used at all places without variation.
        public const string CustomerPortalLink = "http://www.idera.com/licensing";

        // SQL DM 10.3 - To allow use of xp_fixeddrives only for sysadmin
        public const int MajorVersion2008 = 10;
        public const string ExecMasterDboXpFixedDrives = "EXEC master.dbo.xp_fixeddrives";
        public const string ExecMasterDboXpFixedDrivesRemote = "EXEC master.dbo.xp_fixeddrives 1";
        public const string FixedDrives2008 = @"
IF ((IS_SRVROLEMEMBER('sysadmin') = 1))
BEGIN
    {0} 
END
ELSE
BEGIN
    SELECT DISTINCT UPPER(LEFT(volume_mount_point, 1)) AS drive, available_bytes/1024/1024 AS 'MB free' 
        FROM sys.master_files AS f 
		CROSS APPLY 
		sys.dm_os_volume_stats(f.database_id, f.file_id);
END
";

        // Connection string constants
        public const string ConnectionStringApplicationNamePrefix = "SQL Diagnostic Manager";
        public const string DesktopClientConnectionStringApplicationName =
            ConnectionStringApplicationNamePrefix + " Desktop Client";
        public const string CollectionServceConnectionStringApplicationName =
            ConnectionStringApplicationNamePrefix + " Collection Service";
        public const string ManagementServiceConnectionStringApplicationName =
            ConnectionStringApplicationNamePrefix + " Management Service";
        public const string RestServiceConnectionStringApplicationName =
            ConnectionStringApplicationNamePrefix + " Rest Service"; //SQLDM 8.5 Mahesh: New field required for Rest Service
        public const string CustomCounterConnectionStringApplicationName =
            "User Defined Counter - " + CollectionServceConnectionStringApplicationName;
        public const string HelpFileName = "SQLdm Help.chm";

        // Check For Updates constants
        public const string CheckForUpdatesUrl = "http://www.idera.com/webscripts/VersionCheck.aspx?productid=sdm&v=";
        public const int VersionCheckProductId = 111000;

        public const string LastStartTimeAzureQsFormat = "DATETIMEOFFSETFROMPARTS ({0} , 0, 0, 0, 0)";

        // Help menu links to website
        public const string SearchKnowledgeBaseUrl = "http://www.idera.com/support/ServiceFrame.aspx";
        public const string CommunitySiteBaseUrl = "http://community.idera.com/DMIntegration"; //SQLdm 9.1 (Vineet Kumar) (Community Integration) -- Add community link to help menu
        public const string ContactSupportUrl = "http://www.idera.com/Support/ProductSupport.aspx";
        public const string AboutIderaProductsUrl = "http://www.idera.com/productssolutions/sqlserver.aspx";
        public const string IderaUrl = "http://www.idera.com";

        public const int ProductId = 1200;
        public const string ProductName = "SQLdm";

        public const int DefaultConnectionTimeout = 60;
        public const int DefaultQueryMonitorConnectionTimeout = 120;//SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43572 -- Added new timeout value
        public const int DefaultCommandTimeout = 300;
        public const int QuietTimeCommandTimeout = 1800;

        // Duration of Query Monitor Trace in Hours (24)
        public const int QueryMonitorTraceDefaultDuration = 24;

        //For maintenance Mode
        public const string Never = "Never";
        public const string Always = "Until further notice";
        public const string Once = "Occurring once at the specified time";
        public const string Recurring = "Recurring every week at the specified time";
        public const string Monthly = "Monthly";

        //For System Diagnostics
        public const string SQLdmClientLogs = @"Idera\SQLdm\Logs";
        public const string SQLdmServicesLogs = "Logs";
        public const string SysDiagnosticsLogs="Logs";
        public const string SysDiaCompName = "Component Name = ";
        public const string SysDiaInstName = "Instance Name = ";
        public const string SysDiaTestName = "Test Name = " ;
        public const string SysDiaTestResult = "Test Result = ";
        public const string SysDiaMessage = "Message = ";
        public const string SysDiaSepartor = "================================================================";
     
		//SQLdm 9.0 (Ankit Srivastava): Query Monitoring with Extended Event Session  --  Added new constants
        //for Query Monitoring 
        public const string ExtendedEventActionsApplicationName = "sqlserver.client_app_name";
        public const string ExtendedEventActionsDatabaseName = "sqlserver.database_name";
        public const string ExtendedEventActionsFileName = "QMExtendedEventLog";

        //SQLdm 9.1 (Sanjali Makkar): Health Index -- Adding default values for coefficients for specific alerts
        public const int HEALTH_INDEX_COEFFICIENT_CRITICAL_ALERT = 6;
        public const int HEALTH_INDEX_COEFFICIENT_WARNING_ALERT = 2;
        public const int HEALTH_INDEX_COEFFICIENT_INFO_ALERT = 1;

        //[START] SQLdm 10.0 (Rajesh Gupta) : LM 2.0 Integration-Adding a link to License Manager Interface
        public const string LICENSE_MANAGER_PATH = @"\License Manager\License Manager Utility.exe";
        public const string PRODUCT_SHORT_NAME = "SQLdm";
        public const string REGISTRY_PATH_LM = @"SOFTWARE\Idera\LM\SQLdm";
        public const string REGISTRY_PATH = @"SOFTWARE\Idera\SQLdm";
        //[END] SQLdm 10.0 (Rajesh Gupta) : LM 2.0 Integration-Adding a link to License Manager Interface


        public const string NO_DATA_AVAILABLE = "No Data Available";//SQLdm 10.1 (Pulkit Puri): Sysadmin issue: Adding proper message on screen if views are not available

        //[START] SQLdm 10.1 (Srishti Purohit)
        
        public const string BaselineAlertText = "Baseline Alert: ";
        public const string DEFAULT_BASELINE_NAME = "Default";
        public const string ADD_BASELINE_NAME = "<Add New Baseline>";
        public const string INVALID_BASELINE_NAME = "Invalid Baseline Name";
        public const string GetSpecificRecommendations = "p_GetSpecificRecommendations"; // added new procedure to get all recommendations for current platform
        public const string GetBlockedCategoriesForPlatform = "p_GetBlockedCategoriesForPlatform";
        public const string GetAzureProfileForResourceUri = "p_GetAzureProfileForResourceUri";
        //Defect SQLCORE-2406 fix
        //Assigning port and host while installing dm and registering it with dashboard at same time
        public const int REGISTRATION_SERVICE_PORT_DEFAULT = 5170;
        //[END] SQLdm 10.1 (Srishti Purohit)
        public const int REGISTRATION_SERVICE_HTTPS_PORT_DEFAULT = 5171;

        //[START] SQLdm 10.1 (Barkha Khatri)
        public const string LOADING = @"Loading...";
        public const string SCOMDefaultRuleDescription = "SCOM default Rule - Send all Alerts to SCOM as Event";
        //[END] SQLdm 10.1 (Barkha Khatri)
        //SQLDM-26048 FIX
        public const string BASELINE_DATE_VALIDATION_MSG = "The start date must be before the end date for the baseline period.";
        //SQLDm 10.2 (Mitul Kapoor)
        //User persist settings, for Desktop Console only
        public const string KEY_USER_PERSIST_SETTINGS = "HistoryRangeValue-DM";
    
        //SQLdm 10.2 (Anshul Aggarwal) - Adding support for new history browser to query waits chart
        public const string QUERY_WAITS_CHART_NAME = "Query Waits Chart";

        // SQLdm 10.3 (Varun Chopra) Linux Support
        public const int LinuxId = 3;
        // Cloudprovider constants
        public const int MicrosoftAzureId = 2;
        public const int MicrosoftAzureManagedInstanceId = 5;
        public const int AmazonRDSId = 1;
        public const int Windows = 4;


        /* CWF Team - Backend Isolation - SQLDM-29086
         * Added following constants to be used for calling CWF Apis */
        public const string Underscore = "_";
        public const string AuthTokenRegistryKey = "AuthToken";
        public const string RefreshTokenRegistryKey = "RefreshToken";
        public static string SQLdmRestBaseURL = "http://" + System.Environment.MachineName + ":" + REGISTRATION_SERVICE_PORT_DEFAULT + "/SQLdm/Rest/";
        //public static string AuthToken = null;
        //public static string RefreshToken = null;

        /// <summary>
        /// Unique key identifier for Recommendations Engine
        /// </summary>
        public const int BlockedCategoryPlatformRestriction = -1;
    }

    //SQLdm10.1 (Srishti Purohit) -- To use every sql version check
    public enum SQLVersionMajor
    {
        SQLServer7= 7,
        SQL2000,
        SQL2005,
        SQL2008And2008R2,
        SQL2012,
        SQL2014,
        SQL2016
    }
}