using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

using System.Data.SqlClient;
using Microsoft.Deployment.WindowsInstaller;
using System.Windows.Forms;
using System.Data.SqlTypes;

namespace CustomActions
{
   class RepositoryHelper
   {
      public static string ValidateInput(string instance, string elementsDb, string coreDb, string useSqlAuth, string sqlUser, string sqlPassword)
      {
         if (string.IsNullOrEmpty(instance))
         {
            return "Specify the SQL Server instance that will host the Idera SQL Elements and Core databases.";
         }
         if (string.IsNullOrEmpty(elementsDb))
         {
            return "Specify the Idera SQL Elements database name.";
         }
         if (!IsValidDatabaseName(elementsDb))
         {
            return "The Idera SQL Elements database name contains invalid characters. Please provide a valid database name.";
         }
         if (string.IsNullOrEmpty(coreDb))
         {
            return "Specify the Idera Core database name.";
         }
         if (!IsValidDatabaseName(coreDb))
         {
            return "The Idera Core database name contains invalid characters. Please provide a valid database name.";
         }
         if (string.Compare(useSqlAuth, "1") == 0)
         {
             if (string.IsNullOrEmpty(sqlUser))
             {
                 return "Specify the SQL Server login name.";
             }
             if (string.IsNullOrEmpty(sqlPassword))
             {
                 return "Specify the SQL Server password.";
             }
         }
         return string.Empty;
      }

      public static string ValidateSQLInstance(Session session, string instance)
      {
         string msg = string.Empty;

         // Setup the connection string to the specificed instance.
         string connectionString = BuildConnectionString(session, instance, "master");

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
               msg = String.Format("Failed to connect to SQL instance, error='{0}'", ex.Message);
            }

            // Check if version is supported.
            if (string.IsNullOrEmpty(msg))
            {
               string sqlVersion = sqlConnection.ServerVersion;
               string[] sqlVersionSplit = sqlVersion.Split(new string[] { "." }, StringSplitOptions.None);
               int major = int.Parse(sqlVersionSplit[0]);
               int build = int.Parse(sqlVersionSplit[2]);
               if ((major < 9) || (major == 9 && build < 2047)) // SQL Server 2005 SP1 and higher
               {
                  msg = "The Idera SQL Elements and Core databases can only be installed on SQL Server 2005 SP1 or higher.";
               }
            }
         }

         return msg;
      }

      public static string ValidateCurrentUserSQLPermissions(Session session, string instance)
      {
         string msg = string.Empty;
         bool hasPermissons = false;
         try
         {
            // Setup connection to instance.
            string connectionString = BuildConnectionString(session, instance, "master");

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
            msg = String.Format("Failed to validate current user's permissions on SQL Server instance {0} - error {1}", instance, ex.Message);
         }

         if (!hasPermissons)
         {
            msg = String.Format("The current user does not have permissions to create databases or logins on the SQL Server instance {0}", instance);
         }

         return msg;
      }

      public static string CheckSQLDb(Session session, string instance, string database, ref bool doesDbExist, ref bool isValidCheckDone)
      {
         string msg = string.Empty;
         doesDbExist = isValidCheckDone = false;
         try
         {
            string connectionString = BuildConnectionString(session, instance, "master");

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
                  connectionString = BuildConnectionString(session, instance, database);
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
                              if (string.Compare(version, "1.0.0.0", true) == 0)
                              {
                                 msg = string.Format("The database {0} is not valid because it is an Idera SQL Elements Beta version repository", database);
                              }
                           }
                        }
                        catch
                        {
                           // Do nothing, the funcion probably does not exist move to 
                           // empty database test.
                        }
                     }

                     // If db is not valid because it is not a Idera db, check if it is empty.
                     if (string.IsNullOrEmpty(msg))
                     {
                        string cmdEmptyDb = "SELECT COUNT(object_id) FROM sys.objects WHERE type = 'U'";
                        using (SqlCommand sqlCommand = new SqlCommand(cmdEmptyDb, sqlConnection))
                        {
                           SqlDataReader rdr = sqlCommand.ExecuteReader();
                           if (rdr.HasRows)
                           {
                              rdr.Read();
                              int num = rdr.GetInt32(0);
                              if (num != 0)
                              {
                                 msg = string.Format("The database {0} is not valid.", database);
                              }
                           }
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
            msg = string.Format("Failed to check if database {0} exists and is valid, error {1}", database, ex.Message);
         }

         return msg;
      }

      public static string CheckServiceAcctSQLLogin(Session session, string instance, string serviceAcct, ref bool isLogin)
      {
         string msg = string.Empty;
         try
         {
            // Setup the connection string to the specificed instance.
            string connectionString = BuildConnectionString(session, instance, "master");

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
            msg = String.Format("The service account could not be granted access to the SQL Server instance {0}, error - {1}", instance, ex.Message);
         }

         return msg;
      }

      public static string CreateServiceAccountSQLLogin(Session session, string instance, string serviceAcct)
      {
         string msg = string.Empty;
         try
         {
            // Setup the connection string to the specificed instance.
            string connectionString = BuildConnectionString(session, instance, "master");

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
            msg = String.Format("The service account could not be granted access to the SQL Server instance {0}, error - {1}", instance, ex.Message);
         }

         return msg;
      }

      public static string CreateServiceAccountSQLDatabaseUser(Session session, string instance, string database, string serviceAcct)
      {
         string msg = string.Empty;
         try
         {
            // Setup the connection string to the specificed instance.
            string connectionString = BuildConnectionString(session, instance, database);

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
            msg = String.Format("The service account database user could not be created on SQL Server instance {0}, database {1}, error - {2}", instance, database, ex.Message);
         }
         return msg;
      }

      private static string BuildConnectionString(Session session, string instance, string database)
      {
          string useSqlAuth = session["REPOSITORY_SQLAUTH"];
          string sqlUser = session["REPOSITORY_SQLUSERNAME"];
          string sqlPassword = session["REPOSITORY_SQLPASSWORD"];

          SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
          sqlConnectionStringBuilder.DataSource = instance;
          sqlConnectionStringBuilder.InitialCatalog = database;

          if (string.Compare(useSqlAuth, "1") != 0)
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
              name.Contains("'")||
              name.Contains("<") ||
              name.Contains(">") ||
              name.Contains("]"))
          {
              return false;
          }

          return true;
      }

       /// <summary>
       ///SQL DM 9.0 (vineet kumar) (License Changes) -- Added method to get  existing license information. 
       /// </summary>
       /// <param name="session"></param>
       /// <param name="instance"></param>
       /// <param name="database"></param>
       /// <returns></returns>
      public static LicenseSummary GetLicenseSummary(Session session, string instance, string database)
      {
          string msg = string.Empty;
          int registeredServers = -1;
          string instanceRepo = null;
          List<string> keyList = new List<string>();
          try
          {
              session.Log("Entered GetLicenseSummary");
              string connectionString = BuildConnectionString(session, instance, database);
              using (SqlConnection sqlConnection = new SqlConnection(connectionString))
              {
                  session.Log("GetLicenseSummary: Connection establishes");
                  sqlConnection.Open();
                  string cmdGetLicenses = "[dbo].[p_GetLicenseKeys]";


                  using (SqlCommand sqlCommand = new SqlCommand(cmdGetLicenses, sqlConnection))
                  {
                      session.Log("GetLicenseSummary: SQL Command building");
                      sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                      SqlParameter paramLicenseID = new SqlParameter("@LicenseID", DBNull.Value);
                      paramLicenseID.Direction = System.Data.ParameterDirection.Input;
                      sqlCommand.Parameters.Add(paramLicenseID);

                      SqlParameter paramReturnServerCount = new SqlParameter();
                      paramReturnServerCount.ParameterName = "@ReturnServerCount";
                      paramReturnServerCount.SqlDbType = System.Data.SqlDbType.Int;
                      paramReturnServerCount.Value = DBNull.Value;
                      paramReturnServerCount.Direction = System.Data.ParameterDirection.InputOutput;
                      sqlCommand.Parameters.Add(paramReturnServerCount);

                      SqlParameter paramReturnInstanceName = new SqlParameter();
                      paramReturnInstanceName.ParameterName = "@ReturnInstanceName";
                      paramReturnInstanceName.SqlDbType = System.Data.SqlDbType.NVarChar;
                      paramReturnInstanceName.Size = 128;
                      paramReturnInstanceName.Value = DBNull.Value;
                      paramReturnInstanceName.Direction = System.Data.ParameterDirection.InputOutput;
                      sqlCommand.Parameters.Add(paramReturnInstanceName);


                      //SqlDataReader rdr = sqlCommand.ExecuteReader();
                      using (SqlDataReader reader = sqlCommand.ExecuteReader())
                      {
                          int fieldId = reader.GetOrdinal("LicenseKey");
                          while (reader.Read())
                          {
                              string key = reader.GetString(fieldId);
                              keyList.Add(key);
                          }
                      }
                      session.Log("GetLicenseSummary: command executed");
                      SqlParameter rsc = sqlCommand.Parameters["@ReturnServerCount"];
                      SqlInt32 sqlValue = (SqlInt32)rsc.SqlValue;
                      if (!sqlValue.IsNull)
                          registeredServers = sqlValue.Value;

                      SqlParameter repository = sqlCommand.Parameters["@ReturnInstanceName"];
                      SqlString strValue = (SqlString)repository.SqlValue;
                      instanceRepo = strValue.Value;

                      session.Log("GetLicenseSummary: Summary called");
                      LicenseSummary license = LicenseSummary.SummarizeKeys(
                          registeredServers,
                          instanceRepo,
                          keyList);

                      return license;
                     // return keyList;
                  }
              }
          }
          catch
          {
              throw new Exception("Could not retrieve keys from repository");
          }

      }


      /// <summary>
      /// //SQLdm 10.1 - Praveen Suhalka - Deregistring SQLdm from CWF when unstalling
      /// </summary>
      /// <param name="session"></param>
      /// <param name="repoDetails"></param>
      /// <returns></returns>
      public static Dictionary<string, string> GetRegisteredCWFInformation(Dictionary<string, string> repoDetails)
      {
          try
          {
              Dictionary<string, string> frameworkDetails = new Dictionary<string, string>();
              string connectionString = BuildConnectionStringFromRepoDetails(repoDetails);
              using (SqlConnection sqlConnection = new SqlConnection(connectionString))
              {
                  sqlConnection.Open();
                  string command = "p_GetTheProductRegistrationInformation";
                  using (SqlCommand sqlCommand = new SqlCommand(command, sqlConnection))
                  {
                      sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                      using (SqlDataReader reader = sqlCommand.ExecuteReader())
                      {
                          while (reader.Read())
                          {
                              int ordinal = reader.GetOrdinal("ProductID");
                              if (!(reader.IsDBNull(ordinal)))
                              {
                                  frameworkDetails["ProductID"] = (reader.GetInt32(ordinal)).ToString();
                                  frameworkDetails["HostName"] = reader["HostName"] == null ? "" : reader["HostName"].ToString();
                                  frameworkDetails["Port"] = reader["Port"] == null ? "" : reader["Port"].ToString();
                                  frameworkDetails["UserName"] = reader["UserName"] == null ? "" : reader["UserName"].ToString();
                                  frameworkDetails["Password"] = reader["Password"] == null ? "" : reader["Password"].ToString();
                                  frameworkDetails["InstanceName"] = reader["InstanceName"] == null ? "" : reader["InstanceName"].ToString();
                                  frameworkDetails["Password"] = EncryptionHelper.QuickDecrypt(frameworkDetails["Password"]); //decrypting password as password stored in database is encrypted
   }
                          }
                      }
                  }
              }
              return frameworkDetails;
          }
          catch (Exception ex)
          {
              throw new Exception("Could not retrieve CWF information from repository" + ex.Message+ex.StackTrace);
          }
      }

      /// <summary>
      /// //SQLdm 10.1 - Praveen Suhalka - Deregistring SQLdm from CWF when unstalling
      /// </summary>
      /// <param name="session"></param>
      /// <param name="repoDetails"></param>
      /// <returns></returns>
      public static int RemoveCWFInformation(Dictionary<string, string> repoDetails)
      {
          try
          {
              Dictionary<string, string> frameworkDetails = new Dictionary<string, string>();
              string connectionString = BuildConnectionStringFromRepoDetails(repoDetails);
              using (SqlConnection sqlConnection = new SqlConnection(connectionString))
              {
                  sqlConnection.Open();
                  string command = "delete from WebFramework";
                  using (SqlCommand sqlCommand = new SqlCommand(command, sqlConnection))
                  {
                      sqlCommand.CommandType = System.Data.CommandType.Text;
                      return sqlCommand.ExecuteNonQuery();
                  }
              }
          }
          catch (Exception ex)
          {
              throw new Exception("Could not remove CWF information from repository" + ex.Message + ex.StackTrace);
          }
      }

      /// <summary>
      /// //SQLdm 10.1 - Praveen Suhalka - Deregistring SQLdm from CWF when unstalling
      /// </summary>
      /// <param name="session"></param>
      /// <param name="repoDetails"></param>
      /// <returns></returns>
      private static string BuildConnectionStringFromRepoDetails(Dictionary<string, string> repoDetails)
      {
          string useSqlAuth = string.Compare(repoDetails["windowsAuthentication"], "true", true) != 0 ? "1" : "0";
          string sqlUser = repoDetails["repositoryUserName"];
          string sqlPassword = repoDetails["repositoryPassword"];

          SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
          sqlConnectionStringBuilder.DataSource = repoDetails["repositoryServer"];
          sqlConnectionStringBuilder.InitialCatalog = repoDetails["repositoryDatabase"];

          if (string.Compare(useSqlAuth, "1") != 0)
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

   }
}
