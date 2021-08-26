using BBS.TracerX;
using Idera.SQLdm.CollectionService.Probes.Sql.Batches;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots.Cloud;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Idera.SQLdm.CollectionService.Probes.Collectors
{
    class AzureMetricCollector : BaseCollector
    {
        protected static Logger Log = Logger.GetLogger("AWS Metric Probe");

        private List<string> azureDBList = new List<string>();
        private SqlConnection connection = null;
        private string _instanceName;
        private string _username;
        private string _password;
        private string dbName;

        private Dictionary<string, Dictionary<string, object>> azureMetrics = new Dictionary<string, Dictionary<string, object>>();

        public AzureMetricCollector(SqlConnection conn, List<string> azureDBNames, string instanceName, string username, string password)
        {
            connection = conn;
            azureDBList = azureDBNames;
            _instanceName = instanceName;
            _username = username;
            _password = password;
        }

        public override IAsyncResult BeginCollection(EventHandler<CollectorCompleteEventArgs> collectionCompleteCallback)
        {
            if (collectionCompleteCallback == null)
                throw new ArgumentNullException("collectionCompleteCallback");

            this.collectionCompleteCallback = collectionCompleteCallback;

            if (AsyncWaitHandle != null)
                throw new Exception("Attempt to call BeginCollection() twice on one instance");

            IAsyncResult queryResult = null;

            GetAzureMetrics();

            Log.InfoFormat("Azure Collector completed with {0} collection.", "Azure Metric");

            collectionCompleteCallback(this, new CollectorCompleteEventArgs(azureMetrics, 0, Result.Success));

            return queryResult;
        }

        public override IAsyncResult BeginCollectionNonQueryExecution(EventHandler<CollectorCompleteEventArgs> collectionNonQueryExecutionCompleteCallback)
        {
            throw new NotImplementedException();
        }

        private static bool CheckIfServceTierChanged(string databaseName, SqlConnectionInfo connInfo)
        {
            bool result = false;
            try
            {
                string serviceTierChangedBatch = BatchFinder.AzureDatabaseServiceTierChanged();
                serviceTierChangedBatch = serviceTierChangedBatch.Replace("{dbname}", databaseName);
                
                using (SqlConnection conn2 = connInfo.GetConnectionDatabase("master"))
                {
                    conn2.Open();
                    using (SqlDataReader dataReader =
                        SqlHelper.ExecuteReader(conn2, CommandType.Text, serviceTierChangedBatch))
                    {
                        int entries = 0;
                        while (dataReader.Read())
                        {
                            entries++;
                        }

                        if (entries > 1)
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Log.InfoFormat("Error while checking if service tier has changed {0}", exception.Message);
            }
            return result;
        }

        private  void GetAzureMetrics()
        {
            try
            {
                string[] metricnames = Enum.GetNames(typeof(CloudMetricList.AzureMetric));      
                foreach (string azureDBName in azureDBList)
                {
                    try
                    {
                        Dictionary<string, object> currentAzureDBMetrics = new Dictionary<string, object>();
                        SqlConnectionInfo connInfo = new SqlConnectionInfo(_instanceName, azureDBName, _username, _password);
                        using (SqlConnection tempConn = connInfo.GetConnection())
                        {
                            tempConn.Open();
                            using (SqlDataReader dataReader = SqlHelper.ExecuteReader(tempConn, CommandType.Text, BatchFinder.AzureSQLMetric()))
                            {
                                while (dataReader.Read())
                                {
                                    if (dataReader.IsDBNull(0))
                                    {
                                        continue;
                                    }
                                    currentAzureDBMetrics.Add(metricnames[0], Convert.ToDouble(dataReader.GetValue(0), CultureInfo.InvariantCulture));
                                    currentAzureDBMetrics.Add(metricnames[1], Convert.ToDouble(dataReader.GetValue(1), CultureInfo.InvariantCulture));
                                    currentAzureDBMetrics.Add(metricnames[2], Convert.ToDouble(dataReader.GetValue(2), CultureInfo.InvariantCulture));
                                    currentAzureDBMetrics.Add(metricnames[3], Convert.ToDouble(dataReader.GetValue(3), CultureInfo.InvariantCulture));
                                    currentAzureDBMetrics.Add(metricnames[4], CheckIfServceTierChanged(azureDBName, connInfo));
                                    currentAzureDBMetrics.Add(metricnames[5], Convert.ToDouble(dataReader.GetValue(5), CultureInfo.InvariantCulture));                         
                                    currentAzureDBMetrics.Add(metricnames[6], Convert.ToDouble(dataReader.GetValue(6), CultureInfo.InvariantCulture));

                                    //START 5.4.2
                                    currentAzureDBMetrics.Add(metricnames[7], Convert.ToDouble(dataReader.GetValue(0), CultureInfo.InvariantCulture));
                                    currentAzureDBMetrics.Add(metricnames[8], Convert.ToDouble(dataReader.GetValue(1), CultureInfo.InvariantCulture));
                                    currentAzureDBMetrics.Add(metricnames[9], Convert.ToDouble(dataReader.GetValue(2), CultureInfo.InvariantCulture));
                                    currentAzureDBMetrics.Add(metricnames[10], Convert.ToDouble(dataReader.GetValue(3), CultureInfo.InvariantCulture));
                                    currentAzureDBMetrics.Add(metricnames[11], Convert.ToDouble(dataReader.GetValue(5), CultureInfo.InvariantCulture));
                                    currentAzureDBMetrics.Add(metricnames[12], Convert.ToDouble(dataReader.GetValue(6), CultureInfo.InvariantCulture));

                                    if (azureMetrics.ContainsKey(azureDBName))
                                    {
                                        azureMetrics.Remove(azureDBName);
                                    }
                                    if(currentAzureDBMetrics.Count > 0)
                                        azureMetrics.Add(azureDBName, currentAzureDBMetrics);
                                }
                            }
                            tempConn.Close();
                        }

                    }
                    catch (Exception exception)
                    {
                        Log.InfoFormat("Error while fetching Azure metrics {0} in collector, {1} for db {2}", exception.Message, "Azure Metric", azureDBName);
                    }
                }
            }
            catch (Exception exception)
            {
                Log.InfoFormat("Error while fetching Azure metrics {0} in collector, {1}", exception.Message, "Azure Metric");
            }
        }
    }
}
