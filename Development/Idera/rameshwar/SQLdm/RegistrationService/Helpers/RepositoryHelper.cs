namespace Idera.SQLdm.RegistrationService.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using Microsoft.ApplicationBlocks.Data;
    using Configuration;
    using PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Resources;

    internal static partial class RepositoryHelper
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("RepositoryHelper");

        #region Constants

        public const string ConnectionStringApplicationNamePrefix = "SQL Diagnostic Manager";
        public const string RegistrationServiceConnectionStringApplicationName =
        ConnectionStringApplicationNamePrefix + " Registration Service";
        
        private const string AddTheProductRegistrationInformation = "p_AddTheProductRegistrationInformation"; // SQLdm 9.0 -(Abhishek Joshi) - added new procedure to update the web frameworks registration information
        private const string GetTheProductRegistrationInformation = "p_GetTheProductRegistrationInformation"; // SQLdm 9.0 -(Abhishek Joshi) - added new procedure to get the webframeworks registration information
        private const string DeleteTheProductRegistrationInformation = "p_DeleteTheProductRegistrationInformation"; // SQLdm 10.1 -(Praveen Suhalka) - added new procedure to Delete the webframeworks registration information on unregistration
        
        private const string GetMasterRecommendationsStoreProcedure = "p_GetMasterRecommendations"; // SQLdm 10.0 -(Srishti Purohit) - added new procedure to get all master records for recommendations from DB
        #endregion


        // START : SQLdm 9.0 (Abhishek Joshi) -CWF Integration -Helper methods to update and get the products web framework registration information
        #region Web Framework

        // SQLdm 9.0 (Abhishek Joshi) -CWF Integration -update the products web framework registration information
        public static void UpdateTheRegistrationInformation(string client, string port, string username, string password, int productId, string instance) //product might not be there at the time of update
        {
            try
            {
                var connectionString = RegistrationServiceConfiguration.ConnectionString;

                if (String.IsNullOrEmpty(connectionString))
                    throw new ArgumentNullException("ManagementServiceConfiguration.ConnectionString");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlParameter[] parameters = SqlHelperParameterCache.GetSpParameterSet(connection, AddTheProductRegistrationInformation);
                    parameters[0].Value = client;
                    parameters[1].Value = port;
                    parameters[2].Value = username;
                    parameters[3].Value = password;
                    parameters[4].Value = instance;
                    parameters[5].Value = productId;

                    using (SqlCommand command = new SqlCommand(AddTheProductRegistrationInformation, connection))
                    {
                        command.CommandTimeout = SqlHelper.CommandTimeout;
                        command.Parameters.AddRange(parameters);
                        command.CommandType = CommandType.StoredProcedure;
                        int affectedRowCount = command.ExecuteNonQuery();
                        if (affectedRowCount > 0)
                        {
                            LOG.InfoFormat(
                                "Web Framework registration information updated: HostName={0}, Port={1}, UserName={2}, Password={3}, ProductId={4}",
                                client, port, username, password, productId);
                        }
                        else
                        {
                            LOG.InfoFormat(
                                "Failed to update the Web Framework registration information: HostName={0}, Port={1}, UserName={2}, Password={3}, ProductId={4}",
                                client, port, username, password, productId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured in UpdateTheRegistrationInformation.", ex);
            }

        }

        // SQLdm 9.0 (Abhishek Joshi) -CWF Integration -get the products web framework registration information
        public static Dictionary<string, string> GetTheRegistrationInformation()
        {
            Dictionary<string, string> frameworkDetails = new Dictionary<string, string>();
            try
            {
                var connectionString = RegistrationServiceConfiguration.ConnectionString;

                if (connectionString == null)
                {
                    throw new ArgumentNullException("repositoryConnectionString");
                }


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, GetTheProductRegistrationInformation))
                    {
                        while (dataReader.Read())
                        {
                            int ordinal = dataReader.GetOrdinal("ProductID");
                            if (!(dataReader.IsDBNull(ordinal)))
                            {
                                frameworkDetails["ProductID"] = (dataReader.GetInt32(ordinal)).ToString();
                                frameworkDetails["HostName"] = dataReader["HostName"] == null ? null : dataReader["HostName"].ToString();
                                frameworkDetails["Port"] = dataReader["Port"] == null ? null : dataReader["Port"].ToString();
                                frameworkDetails["UserName"] = dataReader["UserName"] == null ? null : dataReader["UserName"].ToString();
                                frameworkDetails["Password"] = dataReader["Password"] == null ? null : dataReader["Password"].ToString();
                                frameworkDetails["InstanceName"] = dataReader["InstanceName"] == null ? null : dataReader["InstanceName"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error Occured in GetTheRegistrationInformation:", ex);
            }

            return frameworkDetails;
        }


        public static void DeleteTheRegistrationInformation()
        {
            var connectionString = RegistrationServiceConfiguration.ConnectionString;

            if (connectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlHelper.ExecuteNonQuery(connection, DeleteTheProductRegistrationInformation);
            }
        }
        #endregion
        // END : SQLdm 9.0 (Abhishek Joshi) -CWF Integration -Helper methods to update and get the products web framework registration information

        public static List<MasterRecommendation> GetMasterRecommendations(string connectionString)
        {
            MasterRecommendation singleRecommendation = null;
            List<MasterRecommendation> masterData = new List<MasterRecommendation>();
            try
            {
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, GetMasterRecommendationsStoreProcedure))
                {
                    int toHandleIntParsing = 0;
                    double toHandleDoubleParsing = 0;
                    #region Get  Master Values

                    while (reader.Read())
                    {
                        string recommendationID = reader.GetString(0);

                        singleRecommendation = new MasterRecommendation(recommendationID);
                        if (singleRecommendation != null)
                        {
                            singleRecommendation.Additional_Considerations = reader["AdditionalConsiderations"].ToString();
                            singleRecommendation.Bitly = reader["bitly"].ToString();
                            singleRecommendation.Category = reader["Category"].ToString();
                            if (!int.TryParse(reader["ConfidenceFactor"].ToString(), out toHandleIntParsing)) toHandleIntParsing = 0;
                            singleRecommendation.Confidence_Factor = toHandleIntParsing;
                            singleRecommendation.Description = reader["Description"].ToString();
                            singleRecommendation.Finding = reader["Finding"].ToString();
                            singleRecommendation.Impact_Explanation = reader["ImpactExplanation"].ToString();
                            if (!int.TryParse(reader["ImpactFactor"].ToString(), out toHandleIntParsing)) toHandleIntParsing = 0;
                            singleRecommendation.Impact_Factor = toHandleIntParsing;
                            singleRecommendation.InfoLinks = reader["InfoLinks"].ToString();

                            singleRecommendation.Plural_Form_Finding = reader["PluralFormFinding"].ToString();
                            singleRecommendation.Plural_Form_Impact_Explanation = reader["PluralFormImpactExplanation"].ToString();
                            singleRecommendation.Plural_Form_Recommendation = reader["PluralFormRecommendation"].ToString();

                            singleRecommendation.Problem_Explanation = reader["ProblemExplanation"].ToString();
                            singleRecommendation.Recommendation = reader["Recommendation"].ToString();
                            if (!double.TryParse(reader["Relevance"].ToString(), out toHandleDoubleParsing)) toHandleDoubleParsing = 0;
                            singleRecommendation.Relevance = toHandleDoubleParsing;
                            singleRecommendation.Tags = reader["Tags"].ToString().Split(',');
                            masterData.Add(singleRecommendation);
                        }
                    }
                    #endregion
                }
            }
            catch (SqlException ex)
            {
                LOG.Error(ex.Message + " Error occured in GetMasterRecommendations");
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error occured in GetMasterRecommendations", ex);
            }

            return masterData;
        }
    }
}
