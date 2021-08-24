using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using CWFInstallerService;

namespace Installer_form_application
{
    /// <summary>
    /// //SQLDM 10.1 - Praveen Suhalka - CWF Integration 
    /// </summary>
    class SQLdmHelper
    {
        #region Fields
        //private static readonly Logger Log = Logger.GetLogger("SQLdmCWFInstaller_SQLdmHelper");
        internal static readonly int[] GuidRegistryFormatPattern = new[] { 8, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2 };
        internal static string UpgradeCodeRegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UpgradeCodes";
        internal static bool is64BitOperatingSystem = Environment.Is64BitOperatingSystem;
        internal static string UpgradeCodeX86 = "CD133B71-4D61-4E61-A479-B36B1F642256";
        internal static string ProductCodeX86 = "B8881827-11E4-4D1F-8E6E-76F9B15F080A";

        internal static string UpgradeCodeX64 = "7A62FDE2-4452-4644-BFD4-1E864922BD4F";

        internal static string ProductCodeX64 = "12C01263-DC2E-4CC1-9010-2BEBC3573014";

        #endregion


        public static bool CheckIfUpgrade()
        {
            try
            {
                string upgradeCode = is64BitOperatingSystem ? UpgradeCodeX64 : UpgradeCodeX86;
                string productCode = is64BitOperatingSystem ? ProductCodeX64 : ProductCodeX86;

                string[] productCodes = SQLdmHelper.GetProductCodes(new Guid(upgradeCode), is64BitOperatingSystem);
                if (productCodes != null && productCodes.Length > 0)//if upgrade code matches or product already installed
                {
                    // Convert the code to the format found in the registry 
                    var productCodeSearchString = SQLdmHelper.Reverse(productCode, SQLdmHelper.GuidRegistryFormatPattern);
                    if (!productCodes.Any(code => code.IndexOf(productCodeSearchString, StringComparison.OrdinalIgnoreCase) >= 0))//check if product code differs from the exisiting ones
                        return true;
                    //TODO else 

                }
            }
            catch (Exception ex)
            {
                //Log.Error("Exception thrown in CheckIfUpgrade: ", ex);
            }

            return false;
        }


        /// <summary>
        /// this method checks if the given productcode/upgradecode exists or not 
        /// </summary>
        /// <param name="upgradeCode"></param>
        /// <returns></returns>
        public static string[] GetProductCodes(Guid upgradeCode, bool is64Bit)
        {
            try
            {
                // Convert the code to the format found in the registry 
                var upgradeCodeSearchString = Reverse(upgradeCode, GuidRegistryFormatPattern);

                RegistryKey hklm;//Declare the Registry Key Object
                //Get the Registry Key object
                if (is64Bit)
                    hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                else
                    hklm = Registry.LocalMachine;

                // Open the upgrade code registry key
                var upgradeCodeRegistryRoot = hklm.OpenSubKey(UpgradeCodeRegistryKey);

                if (upgradeCodeRegistryRoot == null)
                    return null;

                // Iterate over each sub-key
                foreach (var subKeyName in upgradeCodeRegistryRoot.GetSubKeyNames())
                {
                    var subkey = upgradeCodeRegistryRoot.OpenSubKey(subKeyName);

                    if (subkey == null)
                        continue;

                    // Check for a value containing the input code(product or upgrade)
                    if (subkey.Name.IndexOf(upgradeCodeSearchString, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return subkey.GetValueNames();
                    }
                }
            }
            catch (Exception ex)
            {
                //Log.Error("Exception thrown in IfCodeExists: ", ex);
            }
            return null;
        }

        /// <summary>
        /// This method reverses the given GUID to the pattern given
        /// </summary>
        /// <param name="value">GUID</param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string Reverse(object value, params int[] pattern)
        {
            var returnString = new StringBuilder();

            try
            {
                // Strip the hyphens
                var inputString = value.ToString().Replace("-", "");
                var index = 0;

                // Iterate over the reversal pattern
                foreach (var length in pattern)
                {
                    // Reverse the sub-string and append it
                    returnString.Append(inputString.Substring(index, length).Reverse().ToArray());

                    // Increment our posistion in the string
                    index += length;
                }
            }
            catch (Exception ex)
            {
                //Log.Error("Exception thrown in Reverse: ", ex);
            }

            return returnString.ToString();
        }

        public static bool isSQLdmAlreadyInstalled()
        {
            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32));
            using (RegistryKey key = hklm.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        try
                        {
                            string productName = subkey.GetValue("DisplayName").ToString().ToLower();
                            //fixing SQLDM-26646
                            if (productName == "idera sql diagnostic manager (x64)" || productName == "idera sql diagnostic manager")
                            {
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

        // Pinvoke for API function
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
        out ulong lpFreeBytesAvailable,
        out ulong lpTotalNumberOfBytes,
        out ulong lpTotalNumberOfFreeBytes);

        private static bool GetAvailableDiskSpace(string folderName, out ulong freespace)
        {
            freespace = 0;
            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentNullException("folderName");
            }

            if (!folderName.EndsWith("\\"))
            {
                folderName += '\\';
            }

            ulong free = 0, dummy1 = 0, dummy2 = 0;

            if (GetDiskFreeSpaceEx(folderName, out free, out dummy1, out dummy2))
            {
                freespace = free;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void ValidateDiskSpaceForSelectedComponents(string driveName, bool dashboard, bool dmConsole, bool dmServices)
        {
            double memoryRqd = Properties.Settings.Default.MemoryRqdForBufferInMB;
            ulong freeSpace = 0;            

            if (dashboard)
                memoryRqd += Properties.Settings.Default.MemoryRqdForDashboardInMB;
            if (dmConsole)
                memoryRqd += Properties.Settings.Default.MemoryRqdForSQLdmConsoleInMB;
            if (dmServices)
                memoryRqd += Properties.Settings.Default.MemoryRqdForSQLdmServicesInMB;
            GetAvailableDiskSpace(driveName, out freeSpace);
            double freeSpaceInMB = (freeSpace / 1024f) / 1024f;
            if (freeSpaceInMB < memoryRqd)            
                throw new Exception("There is not enough disk space to install the product.  Please free up disk space or try to install in a different location.");                       
        }

        public static string getSQLdmInstallPath()
        {
            string installPath = string.Empty;
            string registry_key = @"SOFTWARE\Idera\SQLdm";
            var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32));
            RegistryKey regKey = hklm.OpenSubKey(registry_key);
            if (regKey != null)
            {
                installPath = regKey.GetValue("Path").ToString();
            }
            return installPath;
        }

        public static Dictionary<string, string> getSQLdmRepositoryDetails(string SQLdmInstallPath)
        {
            Dictionary<string, string> details = new Dictionary<string, string>();
            string configPath = SQLdmInstallPath + @"\SQLdmManagementService.exe.config";

            if (SQLdmInstallPath != string.Empty && File.Exists(SQLdmInstallPath + @"SQLdmManagementService.exe") && File.Exists(SQLdmInstallPath + @"SQLdmCollectionService.exe"))
            {
                using (XmlTextReader reader = new XmlTextReader(configPath))
                {
                    while (reader.Read())
                    {
                        if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "ManagementService"))
                        {
                            details["repositoryDatabase"] = reader.GetAttribute("repositoryDatabase");
                            details["repositoryServer"] = reader.GetAttribute("repositoryServer");
                        }
                    }
                }
            }
            return details;
        }

        private static bool IsValidDatabaseName(string name)
        {
            if (name.Contains("\"") ||
                name.Contains("*") ||
                name.Contains("?") ||
                name.Contains("|") ||
                name.Contains(":") ||
                name.Contains("\\") ||
                name.Contains("/") ||
                name.Contains("'") ||
                name.Contains("<") ||
                name.Contains(">") ||
                name.Contains("]"))
            {
                return false;
            }

            return true;
        }
        private static string BuildConnectionString(string instance, string database, bool useSqlAuth, string sqlUser, string sqlPassword)
        {

            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
            sqlConnectionStringBuilder.DataSource = instance;
            sqlConnectionStringBuilder.InitialCatalog = database;

            if (!useSqlAuth)
            {
                sqlConnectionStringBuilder.IntegratedSecurity = true;
            }
            else
            {
                sqlConnectionStringBuilder.IntegratedSecurity = false;
                sqlConnectionStringBuilder.UserID = sqlUser;
                sqlConnectionStringBuilder.Password = sqlPassword;
            }

            return sqlConnectionStringBuilder.ConnectionString;
        }
        public static void ValidateInput(string instance, string database, bool useSqlAuth, string sqlUser, string sqlPassword, string product)
        {
            if (string.IsNullOrEmpty(instance)) throw new EmptySQLServerInstanceException();
            if (string.IsNullOrEmpty(database)) throw new EmptyDatabaseException(product);
            if (!IsValidDatabaseName(database)) throw new InvalidDatabaseException(product);
            if (useSqlAuth)
            {
                if (string.IsNullOrEmpty(sqlUser)) throw new EmptySQLUserNameException();
                if (string.IsNullOrEmpty(sqlPassword)) throw new EmptySQLPasswordException();
            }
        }
        public static void ValidateSQLInstance(string instance, bool isSQLAuth, string sqlUser, string sqlPassword)
        {
            // Setup the connection string to the specificed instance.
            string connectionString = BuildConnectionString(instance, "master", isSQLAuth, sqlUser, sqlPassword);

            // Check if instance exists, and the version is supported.
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                // Open connection to SQL instance.
                try
                {
                    sqlConnection.Open();
                }
                catch (Exception ex)
                {
                    throw new FailedToVerifySQLServerException();
                }

                // Check if version is supported.
                string sqlVersion = sqlConnection.ServerVersion;
                string[] sqlVersionSplit = sqlVersion.Split(new string[] { "." }, StringSplitOptions.None);
                int major = int.Parse(sqlVersionSplit[0]);
                int build = int.Parse(sqlVersionSplit[2]);
                if (major < 9) // SQL Server 2005 and higher
                    throw new UnSupportedSQLServerException(sqlVersion);
            }
        }
        public static void checkIfRepositoryExists(string instance, string database, bool isSQLAuth, string sqlUser, string sqlPassword, ref bool doesDbExist)
        {
            try
            {
                string connectionString = BuildConnectionString(instance, "master", isSQLAuth, sqlUser, sqlPassword);

                // Check if db exists.
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    // Open connection to master database.
                    sqlConnection.Open();

                    // Query to see if db exists.
                    string cmdDbExists = string.Format("SELECT * FROM master.dbo.sysdatabases WHERE name='{0}'", database);
                    using (SqlCommand sqlCommand = new SqlCommand(cmdDbExists, sqlConnection))
                    {
                        SqlDataReader rdr = sqlCommand.ExecuteReader();
                        doesDbExist = rdr.HasRows;
                    }
                }
            }
            catch
            {
                throw new FailedToVerifyDatabaseException();
            }
        }
        public static bool ValidateCurrentUserSysAdminPermissions(string instance, bool isSQLAuth, string sqlUser, string sqlPassword)
        {
            string msg = string.Empty;
            bool hasPermissons = false;
            try
            {
                // Setup connection to instance.
                string connectionString = BuildConnectionString(instance, "master", isSQLAuth, sqlUser, sqlPassword);

                // Connect and query if member of sysadmin or dbcreator roles
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    // Open connection to master database.
                    sqlConnection.Open();

                    // Query to see if sysadmin.
                    string cmd = "IF IS_SRVROLEMEMBER('sysadmin') = 1 SELECT 1 ELSE SELECT 0";
                    using (SqlCommand sqlCommand = new SqlCommand(cmd, sqlConnection))
                    {
                        object obj = sqlCommand.ExecuteScalar();
                        if (obj != null && obj != System.DBNull.Value && obj is int)
                        {
                            hasPermissons = ((int)obj != 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return hasPermissons;
        }
        public static void ValidateRepositoryConnection(string server, string database, bool isSQLAuth, string username, string password, bool is2FA, ref bool dbExists, string product)
        {
            ValidateInput(server, database, isSQLAuth, username, password, product);
            ValidateSQLInstance(server, isSQLAuth, username, password);
            //ValidateCurrentUserSQLPermissions(server, isSQLAuth, username, password);
            checkIfRepositoryExists(server, database, isSQLAuth, username, password, ref dbExists);
        }
    }
}
