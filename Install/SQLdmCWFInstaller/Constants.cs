
namespace SQLdmCWFInstaller
{
    /// <summary>
    /// SQLdm 9.0 (Ankit Srivastava) - CWF Installer Wrapper - Created New Constant class 
    /// </summary>
    class Constants
    {
        public const string UpgradeCodeRegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UpgradeCodes";

        public const string ProductName = "SQLdm";//TODO "SQLDiagnosticsManager"
        public static string CWFConfirmationHeader = "IDERA Dashboard Install Validation";
        public static string CWFDetailsConfirmation = "We could not connect to the IDERA Dashboard with the details you provided. Click OK to continue or click Cancel to try again.";
        public const string InstallationCommandArgs = "/v\"CWF_PORT={0} CWF_HOSTNAME=\"{1}\" CWF_SERVICE_USER_NAME=\"{2}\" CWF_SERVICE_PASSWORD=\"{3}\"";//SQLdm 9.0 (Ankit Srivastava) Added instance name parameter as well
        public const string RemoteCWFOutdatedMessage = "IDERA Dashboard installed on the remote machine is not compatible with this version of SQL Diagnostic Manager. Upgrade the IDERA Dashboard on the remote machine to the current version.";//SQLdm 9.1 (Vineet) -- Changes for upgrading Idera Dashboard

        public const string UpgradeCodeX86 = "CD133B71-4D61-4E61-A479-B36B1F642256";
        public const string ProductCodeX86 = "E468BA9F-47F7-4994-9CB7-5DA122621B66";
        
        public const string UpgradeCodeX64 = "7A62FDE2-4452-4644-BFD4-1E864922BD4F";
        public const string ProductCodeX64 = "FDCAD5F0-67BE-4327-97D0-C527C516FCFD";

        public static string CWFAPIRootDefaultURL = @"http://localhost:9292";
        public static string CWFAPIStatusPath = "/IderaCoreServices/v1/Status";
        public static string CWFAPIIsAvailablePath = "/IderaCoreServices/v1/Products/IsAvailable";
        public static string CWFAPIProductsPath = "/IderaCoreServices/v1/Products";
        public static string CWFAPIVersionPath = "/IderaCoreServices/v1/Version";//SQLdm 9.1 (Vineet) -- Changes for upgrading Idera Dashboard
        public static System.Version CWFVersionWithoutRevision = new System.Version("2.2");//SQLdm 9.1 (Vineet) -- Changes for upgrading Idera Dashboard

        //10.0 SQLdm Srishti Purohit
        //Commenting below as per requirement -- defect DE46192 for Intro Video Link change
        //public static string IderaLink = "http://www.idera.com";//dm 10.0 vineet--added link for idera.com
        public static string CWFIntroVideoLink = "http://www.idera.com/global/content/resources/videos/products/cwf/overview?utm_source=sqlcwf&utm_medium=inproduct&utm_content=tl"; 
        
    }
}
