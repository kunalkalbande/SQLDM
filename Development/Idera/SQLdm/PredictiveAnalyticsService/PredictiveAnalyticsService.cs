using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PredictiveAnalyticsService.Helpers;
using Idera.SQLdm.PredictiveAnalyticsService.Configuration;
using Idera.SQLdm.PredictiveAnalyticsService.Classifiers;
using System.Data;
using System.Diagnostics;
using Wintellect.PowerCollections;
using Idera.SQLdm.Common.Services;

namespace Idera.SQLdm.PredictiveAnalyticsService
{
    static class PredictiveAnalyticsService
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("PredictiveAnalyticsService");

        // ids of the metrics we forecast on
        private static Dictionary<int, bool> forecastableMetricsLookup;
        private static int[] forecastableMetrics = new int[] { 0,8,9,13,22,24,25,26,27,28,29,30,31,32,33,57,58,59,60,62,63,64,68,69,70,71,76,82,83,84 };

        private static Dictionary<Guid, ISnapshotSink> snapshotSinks;  //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) - new private field

        static PredictiveAnalyticsService()
        {
            snapshotSinks = new Dictionary<Guid, ISnapshotSink>(); // SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics)

            forecastableMetricsLookup = new Dictionary<int, bool>();

            for (int i = 0; i < forecastableMetrics.Length; i++)
                forecastableMetricsLookup.Add(forecastableMetrics[i], true);            
        }

        //Start: SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) - new property
        public static Dictionary<Guid, ISnapshotSink> SnapshotSinks
        {
            get { return snapshotSinks; }
        }
        //End: SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics)


        public static void BuildModels()
        {
            // Load the servers and metrics for which alerting is enabled
            Dictionary<int, List<int>> serverAlerts = DataHelper.GetServerAlerts();

            if (serverAlerts.Count == 0)
            {
                LOG.Info("Cannot build predictive models: No servers with enabled alert metrics were found.");
                return;
            }

            // get the training data attributes
            DataAttribute[] attributes = GetTrainingDataAttributes();
            DateTime modelTrainingCutoffDateTime = PredictiveAnalyticsConfiguration.TrainingDataCutoffDateTime; // can use to test in the past - override from config

            LOG.DebugFormat("Training data cutoff datetime: {0}", modelTrainingCutoffDateTime);

            int forecastBlockSize = PredictiveAnalyticsConfiguration.ForecastBlockSize;
            int forecastNumBlocks = PredictiveAnalyticsConfiguration.ForecastNumBlocks;

            int[] severities = new int[] { 4, 8 }; // warning, critical
            int   metricid   = 0;
            int   severity   = 0;
            int   timeframe  = 0;
            List<DateTime> alertDateTimes = new List<DateTime>();

            Stopwatch sw1 = new Stopwatch();
            double avgSecondsPerServer = 0;

            try
            {
                // loop over the servers
                foreach (int serverid in serverAlerts.Keys)
                {
                    sw1.Reset();
                    sw1.Start();

                    // loop over the metrics
                    for (int j = 0; j < serverAlerts[serverid].Count; j++)
                    {
                        metricid = serverAlerts[serverid][j];

                        // skip the metric if we don't forecast for it
                        if (!forecastableMetricsLookup.ContainsKey(metricid))
                            continue;

                        // loop over severities
                        for (int k = 0; k < severities.Length; k++)
                        {
                            severity = severities[k];

                            for (int l = 0; l < forecastNumBlocks; l++)
                            {                                
                                timeframe = forecastBlockSize * (l + 1);
                                alertDateTimes.Clear();

                                // load the training data                                  
                                DataTable trainingData = DataHelper.GetPredictiveTrainingData(serverid, metricid, severity, timeframe, Configuration.PredictiveAnalyticsConfiguration.TrainingDataCutoffDateTime, ref alertDateTimes);
                                LOG.DebugFormat("Got training data.  Params: {0}, {1}, {2}, {3}, {4}", serverid, metricid, severity, timeframe, Configuration.PredictiveAnalyticsConfiguration.TrainingDataCutoffDateTime);

                                if (trainingData == null || trainingData.Rows.Count == 0 || alertDateTimes.Count <= 3)
                                {
                                    LOG.DebugFormat("No training data found (or insufficient) for server {0}, metric {1}, severity {2} for timeframe {3} at cutoff date {4}.", serverid, metricid, severity, timeframe, Configuration.PredictiveAnalyticsConfiguration.TrainingDataCutoffDateTime);
                                    continue;
                                }

                                // encode for this time frame                                
                                DataHelper.EncodePredictiveTrainingDataForTimeframe(trainingData, timeframe, alertDateTimes);

                                // build the model                                
                                NaiveBayes model = new NaiveBayes();                              
                                model.BuildModel(attributes, trainingData, 0, new int[] { 0, 1 });
                                LOG.DebugFormat("Built predictive model. Params: {0}, {1}, {2}, {3}, {4}, accuracy: {5:##.##}%", serverid, metricid, severity, timeframe, Configuration.PredictiveAnalyticsConfiguration.TrainingDataCutoffDateTime, model.Accuracy * 100);
                                
                                // save the model
                                DataHelper.SavePredictiveModel(serverid, metricid, severity, timeframe, model);
                                LOG.Debug("Predictive model was saved.");                                
                            }
                        }
                    }

                    sw1.Stop();
                    LOG.DebugFormat("Time to build model for server {0}: {1}", serverid, sw1.Elapsed);

                    avgSecondsPerServer += sw1.Elapsed.TotalSeconds;
                }

                LOG.DebugFormat("Average time to build model for servers: {0}", avgSecondsPerServer / serverAlerts.Keys.Count);
            }
            catch (Exception e)
            {
                LOG.Error("An error was encountered building the predictive models.", e);
            }
        }

        public static void MakeForecasts(int interval)
        {
            // check for models, if we have none, then nothing to do
            DataTable serversWithModels = DataHelper.GetPredictiveModelServers();

            if (serversWithModels == null || serversWithModels.Rows.Count == 0)
            {
                LOG.Debug("Cannot make forecasts; No forecast models were found.");
                return;
            }

            DateTime modelInputCutoffDateTime = DateTime.Now; // can override to use a point in the past
            Dictionary<Triple<int, int, int>, double[]> forecasts = new Dictionary<Triple<int, int, int>, double[]>(); // key: metric, severity, timeframe, value: [forecast, accuracy]

            int    serverid  = 0;
            double forecast  = 0;
            DateTime now = DateTime.Now;

            try
            {
                foreach (DataRow row in serversWithModels.Rows)
                {
                    // get server id
                    serverid = (int)row[0];

                    // clear forecasts for this server
                    forecasts.Clear();

                    // load the models for this server (key: [metric, severity, timeframe])
                    Dictionary<Triple<int, int, int>, NaiveBayes> models = DataHelper.GetPredictiveModelsForServer(serverid);

                    if (models == null || models.Count == 0)
                    {
                        LOG.DebugFormat("No predictive models were found for server {0}.  Forecasts for this server will be skipped.", serverid);
                        continue;
                    }
                    
                    // Get the model input for the last interval (average of all the values)
                    DataRow inputRow = DataHelper.GetPredictiveModelInput(serverid, interval, modelInputCutoffDateTime);

                    if (inputRow == null)
                    {
                        LOG.DebugFormat("No historical data was found for server {0} for the last {1} minutes.  Skipping forecast.", serverid, interval);
                        continue;
                    }

                    // Make the forecasts for each model                    
                    foreach (Triple<int, int, int> key in models.Keys)
                    {
                        NaiveBayes model = models[key];

                        try
                        {
                            // calculate the forecast for the given input
                            forecast = model.ClassifyInput(inputRow); // 0->will NOT alert, 1->will alert

                            // save the forecast                            
                            forecasts.Add(key, new double[] { forecast, model.Accuracy });
                        }
                        catch (Exception nbException)
                        {
                            LOG.ErrorFormat("Caught exception calculating forecast for server {0}, metric {1}, severity {2}, timeframe {3}, inputCutoffDateTime = {4}.", serverid, key.First, key.Second, key.Third, modelInputCutoffDateTime);
                            LOG.Error("Forecast exception", nbException);                            
                            continue;
                        }
                    }

                    // now save the forecasts for this server to the repository
                    DateTime expiration = DateTime.Now;
                    foreach (Triple<int, int, int> key in forecasts.Keys)
                    {
                        expiration = now.AddMinutes(key.Third); // key.Third is timeframe
                        DataHelper.SavePredictiveForecast(serverid, key.First, key.Second, key.Third, (int)forecasts[key][0], forecasts[key][1], expiration);
                    }
                }
            }
            catch (Exception e)
            {
                LOG.Error("An error was encountered forecasting future alerts.", e);
            }
        }

        private static DataAttribute[] GetTrainingDataAttributes()
        {
            // *****************************************
            // NOTE: must be one attribute per column!!
            // MUST also match order of columns!!!!!!
            // We do this dynamically here in the event
            // we later want to load from configuration.
            // *****************************************

            List<DataAttribute> attributes = new List<DataAttribute>();
            int numcolumns = 80;

            for (int i = 0; i < numcolumns; i++)
            {
                DataAttribute attribute = new DataAttribute();

                attribute.IsDiscrete = false;
                attribute.Ignore     = false;

                attributes.Add(attribute);
            }

            // set those columns which are discrete values
            attributes[0].NumDiscreteValues = 2;
            attributes[0].IsDiscrete        = true;            

            return attributes.ToArray();
        }
    }
}
