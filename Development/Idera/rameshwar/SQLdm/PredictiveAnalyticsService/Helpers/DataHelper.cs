using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using Microsoft.ApplicationBlocks.Data;

using Idera.SQLdm.PredictiveAnalyticsService.Configuration;
using Idera.SQLdm.PredictiveAnalyticsService.Classifiers;
using System.IO;

using System.Collections;
using Wintellect.PowerCollections;
using Idera.SQLdm.Common.Services;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Idera.SQLdm.Common.Configuration;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Resources;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Values;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Objects;
using Idera.SQLdm.Common;

namespace Idera.SQLdm.PredictiveAnalyticsService.Helpers
{
    internal static partial class DataHelper
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("DataHelper");

        private static bool isServerChannelRegistered = false;

        static DataHelper()
        {
            using (LOG.InfoCall("DataHelper.ctor"))
            {
                LOG.Info("Registering client channels...");

                RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;

                IDictionary properties           = new Hashtable();
                properties["name"]               = "tcp-client";
                properties["impersonationLevel"] = "None";
                properties["impersonate"]        = false;
                properties["secure"]             = false;

                BinaryClientFormatterSinkProvider clientSinkProvider = new BinaryClientFormatterSinkProvider();
                TcpClientChannel tcpClientChannel = new TcpClientChannel(properties, clientSinkProvider);
                ChannelServices.RegisterChannel(tcpClientChannel, false);

                LOG.Info("Client channels registered successfully.");
            }
        }

        public static void RegisterServerChannel()
        {
            using (LOG.InfoCall("DataHelper.RegisterServerChannel"))
            {
                if (isServerChannelRegistered == false)
                {
                    LOG.Info("Registering server channels...");

                    IDictionary properties = new Hashtable();
                    properties = new System.Collections.Specialized.ListDictionary();
                    properties["name"] = "tcp-server";
                    properties["port"] = Program.OPTION_PRESCRIPTIVE_PORT_DEFAULT;
                    //                properties["impersonationLevel"] = "None";
                    //                properties["impersonate"] = false;
                    properties["secure"] = false;

                    // serialization provider that supports full serialization
                    BinaryServerFormatterSinkProvider serverSinkProvider = new BinaryServerFormatterSinkProvider();
                    serverSinkProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
                    // 
                    TcpServerChannel tcpServerChannel = new TcpServerChannel(properties, serverSinkProvider);
                    ChannelServices.RegisterChannel(tcpServerChannel, false);
                    LOG.Info("server channels registered successfully.");
                    isServerChannelRegistered = true;
                }
            }
        }

        #region Predictive

        private static IPredictiveAnalytics GetManagementService()
        {
            string address = PredictiveAnalyticsConfiguration.ManagementServiceAddress;
            int    port    = PredictiveAnalyticsConfiguration.ManagementServicePort;

            try
            {
                Uri uri = new Uri(String.Format("tcp://{0}:{1}/Management", address, port));

                ServiceCallProxy     proxy = new ServiceCallProxy(typeof(IPredictiveAnalytics), uri.ToString());
                IPredictiveAnalytics ims   = proxy.GetTransparentProxy() as IPredictiveAnalytics;

                return ims;
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception contacting managment service.", ex);
                return null;
            }
        }

        private static IManagementService GetManagementService2()
        {
            string address = PredictiveAnalyticsConfiguration.ManagementServiceAddress;
            int    port    = PredictiveAnalyticsConfiguration.ManagementServicePort;

            try
            {
                Uri uri = new Uri(String.Format("tcp://{0}:{1}/Management", address, port));

                ServiceCallProxy   proxy = new ServiceCallProxy(typeof(IManagementService), uri.ToString());
                IManagementService ims   = proxy.GetTransparentProxy() as IManagementService;

                return ims;
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception contacting managment service.", ex);
                return null;
            }
        }

        public static bool IsPredictiveAnalyticsEnabled()
        {
            IManagementService service = GetManagementService2();

            if (service == null)
                return false;

            try
            {
                return service.GetPredictiveAnalyticsEnabled();
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in IsPredictiveAnalyticsEnabled", ex);
                return false;
            }
        }

        public static Dictionary<int, List<int>> GetServerAlerts()
        {
            IPredictiveAnalytics service = GetManagementService();

            if (service == null)
                return new Dictionary<int, List<int>>();

            try
            {
                return service.GetServerAlerts();
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in GetServerAlerts", ex);
                return new Dictionary<int, List<int>>();
            }
        }

        public static DataTable GetPredictiveModelServers()
        {
            IPredictiveAnalytics service = GetManagementService();

            if (service == null)
                return null;

            try
            {
                return service.GetPredictiveModelServers();
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in GetPredictiveModelServers", ex);
                return null;
            }
        }

        public static Dictionary<Triple<int,int,int>, NaiveBayes> GetPredictiveModelsForServer(int serverid)
        {
            IPredictiveAnalytics service = GetManagementService();

            if (service == null)
                return new Dictionary<Triple<int,int,int>,NaiveBayes>();

            try
            {
                Dictionary<Triple<int, int, int>, NaiveBayes> models    = new Dictionary<Triple<int, int, int>, NaiveBayes>();
                Dictionary<Triple<int, int, int>, byte[]>     rawModels = service.GetPredictiveModelsForServer(serverid);

                foreach (Triple<int,int,int> key in rawModels.Keys)
                {
                    try
                    {
                        NaiveBayes bayes = new NaiveBayes();
                        bayes.InitFromBytes(rawModels[key]);

                        models.Add(key, bayes);
                    }
                    catch (Exception initex)
                    {
                        string msg = string.Format("{0}, {1}, {2}", key.First, key.Second, key.Third);
                        LOG.Error("Caught exception creating model from repository byes. ["+ msg +"]", initex);
                    }
                }

                return models;
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in GetPredictiveModelsForServer", ex);
                return new Dictionary<Triple<int,int,int>,NaiveBayes>();
            }            
        }

        public static DataRow GetPredictiveModelInput(int serverid, int intervalMinutes, DateTime cutoffDateTime)
        {
            IPredictiveAnalytics service = GetManagementService();

            if (service == null)
                return null;

            try
            {
                DataTable t = service.GetPredictiveModelInput(serverid, intervalMinutes, cutoffDateTime);

                if (t == null || t.Rows.Count == 0)
                    return null;

                return t.Rows[0];
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in GetPredictiveModelInput", ex);
                return null;
            }
        }

        public static DataTable GetPredictiveTrainingData(int serverid, int metricid, int severity, int timeframe, DateTime cutoffDateTime, ref List<DateTime> alertDateTimes)
        {
            if (alertDateTimes == null)
                alertDateTimes = new List<DateTime>();

            IPredictiveAnalytics service = GetManagementService();

            if (service == null)
                return null;            
            
            try
            {
                Pair<DataTable, List<DateTime>> data = service.GetPredictiveTrainingData(serverid, metricid, severity, timeframe, cutoffDateTime);

                if (data == null || data.First == null)
                    return null;

                alertDateTimes = data.Second;
                return data.First;
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in GetPredictiveTrainingData", ex);
                return null;
            }                        
        }        

        public static void SavePredictiveModel(int serverid, int metricid, int severity, int timeframe, NaiveBayes model)
        {
            IPredictiveAnalytics service = GetManagementService();

            if (service == null)
                return;

            try
            {
                service.SavePredictiveModel(serverid, metricid, severity, timeframe, model.GetBytes());
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in SavePredictiveModel", ex);
            }            
        }        

        public static void SavePredictiveForecast(int serverid, int metricid, int severity, int timeframe, int forecast, double accuracy, DateTime expiration)
        {
            IPredictiveAnalytics service = GetManagementService();

            if (service == null)
                return;

            try
            {
                service.SavePredictiveForecast(serverid, metricid, severity, timeframe, forecast, accuracy, expiration);
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in SavePredictiveForecast", ex);
            }
        }

        public static void GroomExpiredForecasts()
        {
            IPredictiveAnalytics service = GetManagementService();

            if (service == null)
                return;

            try
            {
                service.GroomExpiredForecasts();
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in GroomExpiredForecasts", ex);
            }
        }

        public static DataTable EncodePredictiveTrainingDataForTimeframe(DataTable dataTable, int timeframe, List<DateTime> alertDateTimes)
        {
            int stateColumnIndex    = 0;
            int dateTimeColumnIndex = 1;

            DateTime cutoffDateTime;
            DateTime offsetDateTime;
            DateTime rowTstamp;

            foreach (DataRow row in dataTable.Rows)
            {
                rowTstamp = PredictiveAnalyticsConfiguration.TrainingDataReferenceDateTime.AddMilliseconds(((long)(double)row[dateTimeColumnIndex]));

                for (int i = 0; i < alertDateTimes.Count; i++)
                {
                    cutoffDateTime = alertDateTimes[i];
                    offsetDateTime = cutoffDateTime.AddMinutes(-timeframe);

                    // if the times are between cutoff and timeframe, then it's a 1, otherwise, a 0
                    if (offsetDateTime <= rowTstamp && rowTstamp <= cutoffDateTime)
                    {
                        row[stateColumnIndex] = 1;
                        break;
                    }
                    else
                        row[stateColumnIndex] = 0;
                }                
            }

            return dataTable;
        }

        public static void SetNextPredictiveAnalyticsModelRebuild(DateTime nextRebuild)
        {
            IManagementService service = GetManagementService2();

            if (service == null)
                return;

            try
            {
                service.SetNextPredictiveAnalyticsModelRebuild(nextRebuild.ToUniversalTime());
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in SetNextPredictiveAnalyticsModelRebuild", ex);
            }
        }

        public static void SetNextPredictiveAnalyticsForecast(DateTime nextForecast)
        {
            IManagementService service = GetManagementService2();

            if (service == null)
                return;

            try
            {
                service.SetNextPredictiveAnalyticsForecast(nextForecast.ToUniversalTime());
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in SetNextPredictiveAnalyticsForecast", ex);
            }
        }

        #endregion


        /// <summary>
        /// //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics)
        /// </summary>
        /// <param name="res"></param>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        public static int SaveRecommendations(PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result res,int monitoredSqlServerId)
        {
            int analysisID = 0;
            IManagementService service = GetManagementService2();

            if (service == null)
                throw new Exception("Not able to connect to management service.");

            try
            {
                analysisID = service.SaveRecommendations(res, monitoredSqlServerId);
                return analysisID;
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in SaveRecommendations", ex);
                throw;
            }
        }

        /// <summary>
        /// //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics)
        /// </summary>
        /// <returns></returns>
        public static List<Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.PAScheduledTask> GetPrescriptiveAnalysisSchedule()
        {
            IManagementService service = GetManagementService2();

            if (service == null)
                return new List<PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.PAScheduledTask>();

            try
            {
                return service.GetPrescriptiveAnalysisScheduleList();
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in GetPrescriptiveAnalysisSchedule", ex);
                return new List<PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.PAScheduledTask>();
            }
        }

        /// <summary>
        /// //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics)
        /// </summary>
        /// <param name="sqlServerID"></param>
        /// <returns></returns>
        public static ScheduledPrescriptiveAnalysisConfiguration GetConfiguration(int sqlServerID)
        {
            IManagementService service = GetManagementService2();

            if (service == null)
                throw new Exception("Not able to connect to management service.");

            try
            {
                return service.GetAnalysisConfigurationForServer(sqlServerID);
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in GetPrescriptiveAnalysisSchedule", ex);
                throw;
            }
        }
        /// <summary>
        /// //SQLdm 10.0 (Srishti Purohit) 
        /// To support SDR-M16 and get SnapshotValues Object
        /// </summary>
        public static SnapshotValues GetPreviousSnapshotValuesForPrescriptiveAnalysis(int monitoredSqlServerId)
        {
            IManagementService service = GetManagementService2();

            if (service == null)
                throw new Exception("Not able to connect to management service.");

            try
            {
                return service.GetSnapshotValuesForServerForPrescriptiveAnalysis(monitoredSqlServerId);
            }
            catch (Exception ex)
            {
                LOG.Error("Caught exception in GetSnapshotValuesForServer", ex);
                throw;
            }
        }


        /// <summary>
        /// //SQLdm 10.0 (Srishti Purohit) (To get master recommendation data from database through Management Service)
        /// </summary>
        /// <param name="res"></param>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        public static void PopulateMasterRecommendations()
        {
			try
			{
				MasterRecommendations.MasterRecommendationsInformation = GetMasterRecommondation();
			}
			catch (Exception ex)
			{
				LOG.Error("Caught exception in getting masterRecommendations", ex);
			}
		}

		#region Recommendation helper
		internal static List<MasterRecommendation> GetMasterRecommondation()
		{
			var materRecomm = new List<MasterRecommendation>();

			var values = Enum.GetValues(typeof(PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.RecommendationType));
			foreach (PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.RecommendationType value in values)
			{
				var recommendationId = FindingIdAttribute.GetFindingId(value);

				if (string.IsNullOrEmpty(recommendationId)) continue;

				try
				{
					var recomm = new MasterRecommendation(recommendationId)
					{
						Description = SQLdoctorResources.GetDescription(recommendationId),
						Bitly = SQLdoctorResources.GetBitly(recommendationId),
						Additional_Considerations = SQLdoctorResources.GetAdditionalConsiderations(recommendationId),
						Category = SQLdoctorResources.GetCategory(recommendationId),
						Confidence_Factor = SQLdoctorResources.GetConfidenceFactor(recommendationId),
						Finding = SQLdoctorResources.GetFinding(recommendationId),
						Impact_Explanation = SQLdoctorResources.GetImpactExplanation(recommendationId),
						Impact_Factor = SQLdoctorResources.GetImpactFactor(recommendationId),
						Plural_Form_Finding = SQLdoctorResources.GetPluralFinding(recommendationId),
						Plural_Form_Impact_Explanation = SQLdoctorResources.GetPluralImpactExplanation(recommendationId),
						Plural_Form_Recommendation = SQLdoctorResources.GetPluralRecommendation(recommendationId),
						Problem_Explanation = SQLdoctorResources.GetProblemExplanation(recommendationId),
						Recommendation = SQLdoctorResources.GetRecommendation(recommendationId),
						RecommendationID = recommendationId,
						Relevance = SQLdoctorResources.GetRelevance(recommendationId),
						Tags = SQLdoctorResources.GetTags(recommendationId)
					};

					var links = SQLdoctorResources.GetInfoLinks(recommendationId);
					recomm.InfoLinks = string.Empty;

					if (links != null)
					{
						recomm.InfoLinks = new RecommendationLinksConverter().ConvertToInvariantString(links); //string.Join("\r\n", links.Select(x => x.Link));
					}
					materRecomm.Add(recomm);
				}
				catch (Exception ex)
				{

				}
			}
			return materRecomm;
		}
		#endregion
        
        internal static List<string> GetRecommendationListForTargetPlatform(int? sqlServerId = null)
        {
            var service = GetManagementService2();

            if (service == null)
            {
                throw new Exception("Not able to connect to Management Service.");
            }

            try
            {
                return service.GetRecommendationListForTargetPlatform(sqlServerId);
            }

            catch (Exception ex)
            {
                LOG.Error("Caught Exception in GetRecommendationListForTargetPlatform", ex);
                throw;
            }
        }

        internal static Dictionary<int, string> GetBlockedCategoryListForTargetPlatform(int sqlServerId)
        {
            var service = GetManagementService2();

            if (service == null)
            {
                throw new Exception("Not able to connect to Management Service.");
            }

            try
            {
                return service.GetBlockedCategoryListForTargetPlatform(sqlServerId);
            }

            catch (Exception ex)
            {
                LOG.Error("Caught Exception in GetBlockedCategoryListForTargetPlatform", ex);
                throw;
            }
        }

    }
}
