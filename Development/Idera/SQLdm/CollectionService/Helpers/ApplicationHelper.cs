using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Security;
using System;
using System.Collections.Generic;
using Microsoft.ApplicationBlocks.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.CollectionService.Helpers
{
    internal static class ApplicationHelper
    {
        #region Assembly Attribute Accessors

        public static string AssemblyTitle
        {
            get
            {
                // Get all Title attributes on this assembly
				//this comment is for github testing, it can be removed.
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                // If there is at least one Title attribute
                if (attributes.Length > 0)
                {
                    // Select the first one
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    // If it is not an empty string, return it
                    if (titleAttribute.Title != "")
                        return titleAttribute.Title;
                }
                // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static string AssemblyVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public static string AssemblyDescription
        {
            get
            {
                // Get all Description attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                // If there aren't any Description attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Description attribute, return its value
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                // Get all Product attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // If there aren't any Product attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Product attribute, return its value
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                // Get all Copyright attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                // If there aren't any Copyright attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Copyright attribute, return its value
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                // Get all Company attributes on this assembly
                object[] attributes =
                    Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                // If there aren't any Company attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Company attribute, return its value
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        #endregion

        #region Encryption Helpers

        public static SecureString ConvertStringToSecureString(string plaintext)
        {
            if (plaintext == null || plaintext.Length == 0)
            {
                return null;
            }
            else
            {
                SecureString secureString = new SecureString();
                char[] plaintextCharArray = plaintext.ToCharArray();

                foreach (char character in plaintextCharArray)
                {
                    secureString.AppendChar(character);
                }

                return secureString;
            }
        }

        #endregion
    }

    internal class CollectionHelper
    {
        private const string FullTextFeatureQuery = "SELECT FullTextServiceProperty('IsFullTextInstalled') AS Result"; //SQLdm 8.6 -- (Ankit Srivastava) -- Query to get the Full text feature installation status

        /// <summary>
        /// To check if the Full-Text feature is installed with the current instance or not
        /// </summary>
        /// <returns></returns>
        public static bool IsFullTextInstalled(string connectionString)
        {
            bool isInstalled = false;

            try
            {
                if (!String.IsNullOrEmpty(connectionString))
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            using (SqlCommand command = new SqlCommand(FullTextFeatureQuery, connection))
                            {
                                command.CommandType = CommandType.Text;
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        int ordinal = reader.GetOrdinal("Result");
                                        if (!reader.IsDBNull(ordinal))
                                            isInstalled = reader.GetInt32(ordinal) == 1;
                                    }
                                }
                            }
                        }
                        catch(Exception e)
                        {
                            throw new Exception(String.Format("IsFullTextInstalled--Error while accessing the SQL Server with the Connection String :[{0}]./n {1}", connectionString, e.Message));
                        }
                        finally
                        {
                            if (connection.State != ConnectionState.Closed)
                                connection.Close();
                        }
                        
                    }
            }
            catch (Exception ex)
            {
                throw;
            }

            return isInstalled;
        }
        //start sqldm 30013
     
        public static List<String> GetFilteredDatabasesForMonitoringAzure(SqlConnectionInfo connInfo, String sqlCommand, BBS.TracerX.Logger LOG)
        {
            try
            {
                List<String> databaseList = new List<String>();

                using (SqlConnection connection = connInfo.GetConnection())
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sqlCommand, connection))
                    {
                        using (SqlDataReader dataReader = cmd.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                if (!(dataReader.IsDBNull(0)))
                                {
                                    String databaseName = dataReader.GetString(0);
                                    databaseList.Add(databaseName);
                                }
                            }
                        }
                        return databaseList;
                    }
                }
            }
            catch (Exception e)
            {
                LOG.Verbose(e);
                return (new List<string>());
            }
        }

        public static List<String> GetDatabases(SqlConnectionInfo connInfo, BBS.TracerX.Logger LOG)
        {
            try
            {

                List<String> databaseList = new List<String>();

                using (SqlConnection connection = connInfo.GetConnection())
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand("select name from sys.databases", connection))
                    {
                        using (SqlDataReader dataReader = cmd.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                int ordinal = dataReader.GetOrdinal("name");
                                if (!(dataReader.IsDBNull(ordinal)))
                                {
                                   
                                    String databaseName = dataReader["name"].ToString();

                                    databaseList.Add(databaseName);
                                }
                            }
                        }
                        return databaseList;
                    }
                }
            }
            catch (Exception e)
            {
                LOG.Verbose(e);
                return (new List<string>());
            }
        }
        //end sqldm 30013

        public static string GetProductEdition(string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = Probes.Sql.Batches.BatchConstants.GetEdition;
                            object productEdition = command.ExecuteScalar();
                            if (productEdition != null)
                                return (string)productEdition;
                            else
                                return String.Empty;
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception(String.Format("GetProductEdition--Error while accessing the SQL Server with the Connection String :[{0}]./n {1}", (connection !=null && connection.DataSource != null) ? connection.DataSource : string.Empty, e.Message));
                    }
                    finally
                    {
                        if (connection.State != ConnectionState.Closed)
                            connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
