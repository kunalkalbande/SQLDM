using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CWFInstallerService
{
    class RepositoryHelper
    {
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
                if ((major < 9) || (major == 9 && build < 2047)) // SQL Server 2005 SP1 and higher
                    throw new UnSupportedSQLServerException(sqlVersion);
            }
        }

        public static void ValidateCurrentUserSQLPermissions(string instance, bool isSQLAuth, string sqlUser, string sqlPassword)
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

                    // If not sysadmin, then check individual roles.
                    if (!hasPermissons)
                    {
                        // Query to see if dbcreator
                        bool isDbcreator = true;
                        cmd = "IF IS_SRVROLEMEMBER('dbcreator') = 1 SELECT 1 ELSE SELECT 0";
                        using (SqlCommand sqlCommand = new SqlCommand(cmd, sqlConnection))
                        {
                            object obj = sqlCommand.ExecuteScalar();
                            if (obj != null && obj != System.DBNull.Value && obj is int)
                            {
                                isDbcreator = ((int)obj != 0);
                            }
                        }

                        // Query to see if securityadmin
                        bool isSecurityadmin = true;
                        cmd = "IF IS_SRVROLEMEMBER('securityadmin') = 1 SELECT 1 ELSE SELECT 0";
                        using (SqlCommand sqlCommand = new SqlCommand(cmd, sqlConnection))
                        {
                            object obj = sqlCommand.ExecuteScalar();
                            if (obj != null && obj != System.DBNull.Value && obj is int)
                            {
                                isSecurityadmin = ((int)obj != 0);
                            }
                        }

                        // Roll up both the permissions.
                        hasPermissons = isDbcreator && isSecurityadmin;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FailedToVerifyUserPermissionsSQLServerException();
            }
            if (!hasPermissons) throw new UnauthorizedAccessException();
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

        public static void CheckSQLDb(string instance, string database, bool isSQLAuth, string sqlUser, string sqlPassword, ref bool doesDbExist, ref bool isValidCheckDone)
        {
            doesDbExist = isValidCheckDone = false;
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

                // Check if db is valid.
                if (doesDbExist)
                {
                    try
                    {
                        connectionString = BuildConnectionString(instance, database, isSQLAuth, sqlUser, sqlPassword);

                        using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                        {
                            // Open connection to SQL instance.
                            sqlConnection.Open();

                            // Call the version function to validate repository version.
                            string cmdCheckDbVersion = "SELECT [dbo].[f_GetRepositoryVersion] ()";
                            using (SqlCommand sqlCommand = new SqlCommand(cmdCheckDbVersion, sqlConnection))
                            {
                                try
                                {
                                    SqlDataReader rdr = sqlCommand.ExecuteReader();
                                    if (rdr.HasRows)
                                    {
                                        rdr.Read();
                                        string version = rdr.GetString(0);
                                        //We are able to execute the Procedure, So it is a valid idera database
                                    }
                                }
                                catch
                                {
                                    // Do nothing, the funcion probably does not exist move to 
                                    // empty database test.
                                }
                            }
                            // If db is not valid because it is not a Idera db, check if it is empty.
                            string cmdEmptyDb = "SELECT COUNT(object_id) FROM sys.objects WHERE type = 'U'";
                            using (SqlCommand sqlCommand = new SqlCommand(cmdEmptyDb, sqlConnection))
                            {
                                SqlDataReader rdr = sqlCommand.ExecuteReader();
                                if (rdr.HasRows)
                                {
                                    rdr.Read();
                                    int num = rdr.GetInt32(0);
                                    if (num != 0) throw new InvalidDatabaseException("");
                                }
                            }
                            
                        }
                        isValidCheckDone = true;
                    }
                    catch // (Exception ex)
                    {
                        // if we get an exception checking if db is valid, isValidCheckDone flag is not set.   
                        // The caller can deal with this situation.
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FailedToVerifyDatabaseException();
            }
        }

        public static void CheckServiceAcctSQLLogin(string instance, string serviceAcct, string database, bool isSQLAuth, string sqlUser, string sqlPassword, ref bool isLogin)
        {
            try
            {
                // Setup the connection string to the specificed instance.
                string connectionString = BuildConnectionString(instance, "master", isSQLAuth, sqlUser, sqlPassword);

                // Check if login exists.
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    // Open connection to SQL instance.
                    sqlConnection.Open();

                    // Query to see if login exists.
                    String cmd = string.Format("SELECT COUNT(principal_id) FROM sys.server_principals WHERE UPPER(name) = UPPER('{0}') AND type = 'U'", serviceAcct);
                    using (SqlCommand sqlCommand = new SqlCommand(cmd, sqlConnection))
                    {
                        object obj = sqlCommand.ExecuteScalar();
                        if (obj != null && obj != System.DBNull.Value && obj is int)
                        {
                            isLogin = ((int)obj == 1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FailedToVerifyServiceAccountLoginException();
            }

        }

        public static void CreateServiceAccountSQLLogin(string instance, string serviceAcct, string database, bool isSQLAuth, string sqlUser, string sqlPassword)
        {
            try
            {
                // Setup the connection string to the specificed instance.
                string connectionString = BuildConnectionString(instance, "master", isSQLAuth, sqlUser, sqlPassword);

                // Create service account login.
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    // Open connection to SQL instance.
                    sqlConnection.Open();

                    // Create service account login.
                    string cmdCreateLogin = string.Format("CREATE LOGIN [{0}] FROM WINDOWS", serviceAcct);
                    using (SqlCommand sqlCommand = new SqlCommand(cmdCreateLogin, sqlConnection))
                    {
                        sqlCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FailedToCreateServiceAccountLoginException();
            }
        }

        public static void CreateServiceAccountSQLDatabaseUser(string instance, string database, string serviceAcct, bool isSQLAuth, string sqlUser, string sqlPassword)
        {
            try
            {
                // Setup the connection string to the specificed instance.
                string connectionString = BuildConnectionString(instance, database, isSQLAuth, sqlUser, sqlPassword);

                // Create service account database user if does not exist.
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    // Open connection to SQL instance.
                    sqlConnection.Open();

                    // Check if user exists.
                    bool doesUserExist = false;
                    string cmd = string.Format("SELECT COUNT(dbusers.principal_id) FROM sys.database_principals dbusers"
                                               + " JOIN sys.server_principals slogins ON dbusers.sid = slogins.sid"
                                               + " WHERE slogins.type = 'U' and UPPER(slogins.name) = UPPER('{0}')", serviceAcct);
                    using (SqlCommand sqlCommand = new SqlCommand(cmd, sqlConnection))
                    {
                        object obj = sqlCommand.ExecuteScalar();
                        if (obj != null && obj != System.DBNull.Value && obj is int)
                        {
                            doesUserExist = ((int)obj != 0);
                        }
                    }

                    // Create service account database user.
                    if (!doesUserExist)
                    {
                        cmd = string.Format("CREATE USER [{0}]", serviceAcct);
                        using (SqlCommand sqlCommand = new SqlCommand(cmd, sqlConnection))
                        {
                            sqlCommand.ExecuteNonQuery();
                        }

                        // Add to db_owner group.
                        cmd = string.Format("EXEC sp_addrolemember N'db_owner', [{0}]", serviceAcct);
                        using (SqlCommand sqlCommand = new SqlCommand(cmd, sqlConnection))
                        {
                            sqlCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FailedToAssignUserToDatabaseException();
            }
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
    }
}
