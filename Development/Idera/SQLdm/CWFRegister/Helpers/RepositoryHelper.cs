using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using BBS.TracerX;
using Microsoft.ApplicationBlocks.Data;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.CWFRegister.Objects;

namespace Idera.SQLdm.CWFRegister.Helpers
{
    class RepositoryHelper
    {

        static Logger Log = Logger.GetLogger("RepositoryHelper");

        #region Web Framework

        private const string AddTheProductRegistrationInformation = "p_AddTheProductRegistrationInformation";
        private const string GetTheProductRegistrationInformation = "p_GetTheProductRegistrationInformation";
        private const string GetRepositoryVersionSqlCommand = "select dbo.fn_GetDatabaseVersion()";


        //Let the calling method catch any exceptions
        public static void UpdateTheRegistrationInformation(SqlConnectionInfo connectInfo, CWFDetails cwfDetails) //product might not be there at the time of update
        {
            using (Log.InfoCall("UpdateTheRegistrationInformation"))
            {
                using (SqlConnection connection = new SqlConnection(connectInfo.ConnectionString))
                {
                    connection.Open();

                    Log.Info("Connection opened");
                    string passwordEncrypted = CWFDetails.EncryptPassword(cwfDetails.Password);

                    SqlParameter[] parameters = SqlHelperParameterCache.GetSpParameterSet(connection, AddTheProductRegistrationInformation);
                    parameters[0].Value = cwfDetails.HostName;
                    parameters[1].Value = cwfDetails.Port.ToString();
                    parameters[2].Value = cwfDetails.UserName;
                    parameters[3].Value = passwordEncrypted;
                    parameters[4].Value = cwfDetails.DisplayName;
                    parameters[5].Value = cwfDetails.ProductID;

                    Log.Debug("Register to update");
                    Log.DebugFormat("HostName = {0}", cwfDetails.HostName);
                    Log.DebugFormat("Port = {0}", cwfDetails.Port.ToString());
                    Log.DebugFormat("UserName = {0}", cwfDetails.UserName);
                    Log.DebugFormat("Password = {0}", passwordEncrypted);
                    Log.DebugFormat("Name = {0}", cwfDetails.DisplayName);
                    Log.DebugFormat("ProductID = {0}", cwfDetails.ProductID);

                    using (SqlCommand command = new SqlCommand(AddTheProductRegistrationInformation, connection))
                    {
                        command.CommandTimeout = SqlHelper.CommandTimeout;
                        command.Parameters.AddRange(parameters);
                        command.CommandType = CommandType.StoredProcedure;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static CWFDetails GetTheRegistrationInformation(SqlConnectionInfo connectInfo)
        {
            using (Log.InfoCall("GetTheRegistrationInformation"))
            {
                CWFDetails details = new CWFDetails();

                using (SqlConnection connection = new SqlConnection(connectInfo.ConnectionString))
                {
                    using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetTheProductRegistrationInformation))
                    {
                        Log.Info("Connection opened");

                        while (dataReader.Read())
                        {
                            int ordinal = dataReader.GetOrdinal("ProductID");
                            if (!(dataReader.IsDBNull(ordinal)))
                            {
                                Log.InfoFormat("Id registered product = {0}", ordinal);
                                details.ProductID = (dataReader.GetInt32(ordinal));
                                details.HostName = dataReader["HostName"] == null ? "" : dataReader["HostName"].ToString();
                                details.UserName = dataReader["UserName"] == null ? "" : dataReader["UserName"].ToString();
                                details.Password = dataReader["Password"] == null ? "" : CWFDetails.DecryptPassword(dataReader["Password"].ToString());
                                details.DisplayName = dataReader["InstanceName"] == null ? null : dataReader["InstanceName"].ToString();
                                details.Port = dataReader["Port"] == null ? 0 : Convert.ToInt32(dataReader["Port"].ToString());
                                Log.Debug("Information of product");
                                Log.DebugFormat("HostName = {0}", details.HostName);
                                Log.DebugFormat("UserName = {0}", details.UserName);
                                Log.DebugFormat("Password = {0}", dataReader["Password"]);
                                Log.DebugFormat("DisplayName = {0}", details.DisplayName);
                                Log.DebugFormat("Port = {0}", details.Port);
                            }
                        }
                    }
                }          
                return details;
            }
        }

        #endregion

        public static bool IsValidRepository(SqlConnectionInfo connectInfo)
        {
            using (Log.InfoCall("IsValidRepository"))
            {
                using (SqlConnection connection = new SqlConnection(connectInfo.ConnectionString))
                {

                    connection.Open();

                    Log.Info("Connection opened");

                    try
                    {
                        string repositorySchema =
                            (string)SqlHelper.ExecuteScalar(connection, CommandType.Text, GetRepositoryVersionSqlCommand);
                        bool IsValid = repositorySchema == Idera.SQLdm.Common.Constants.ValidRepositorySchemaVersion;

                        Log.InfoFormat("RepositorySchemaVersion = {0}", repositorySchema);

                        return IsValid;
                    }
                    catch (SqlException e)
                    {
                        // Assuming that a valid connection can be established to the SQL Server, 
                        // an invalid call to the version function would indicate an invalid database;
                        // all other exceptions will be passed on.
                        //
                        // Error 208 = is invalid object in SQL Server 2000
                        // Error 4121 - invalid object in SQL Server 2005
                        //
                        Log.ErrorFormat("Error SQL exception ={0}, {1}", e.Message, e);

                        if (e.Number == 208 || e.Number == 4121)
                        {
                            return false;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }   
            }
        }
    }
}
