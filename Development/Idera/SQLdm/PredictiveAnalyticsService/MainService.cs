using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Idera.SQLdm.Common.Messages;

using Idera.SQLdm.PredictiveAnalyticsService.Configuration;
using Idera.SQLdm.PredictiveAnalyticsService.Helpers;
using Idera.SQLdm.Common.Configuration;
using System.Runtime.Remoting;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.Common.Services;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.WorkLoad.Collectors;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Resources;

namespace Idera.SQLdm.PredictiveAnalyticsService
{
    partial class MainService : ServiceBase
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("MainService");
        private static MessageDll messageDll;

        private Thread modelThread;
        private Thread forecastThread;
        private Thread baselineThread;
        private AutoResetEvent modelSignal;
        private AutoResetEvent forecastSignal;
        private AutoResetEvent baselineSignal;
        private bool reconfigure;
        private bool running;
        private bool consoleMode;

        // Start: SQL DM 10.0 -- Doctor Integration -- adding private field
        private static AutoResetEvent scheduleAnalysisSignal;
        private AutoResetEvent getMasterRecommendationSignal;
        private int minutesToRetryGetMasterRecommendation = 1;
        private Thread scheduleAnalysisThread;
        private Thread getMasterRecommendationThread;
        private PAScheduledTask previousScheduledAnalysisTask;
        public static List<PAScheduledTask> scheduleList;
        // End: SQL DM 10.0 -- Doctor Integration -- adding private field

        public bool IsRunning
        {
            get { return running; }
        }

        public MainService()
        {
            InitializeComponent();
            AutoLog = false;
            AutoMapperConfiguration.Configure();
        }

        public void RunFromConsole()
        {
            LOG.InfoFormat("Current FileTraceLevel = {0}", LOG.FileTraceLevel);
            using (LOG.InfoCall("RunFromConsole"))
            {
                consoleMode = true;

                OnStart(null);

                LOG.Info("Running Service.  Press a key to exit.");
                Console.ReadKey();
                LOG.Info("Exiting Service.");

                OnStop();
            }
        }

        protected override void OnStart(string[] args)
        {
            LOG.InfoFormat("Current FileTraceLevel = {0}", LOG.FileTraceLevel);
            using (LOG.InfoCall("OnStart"))
            {
                if (!consoleMode)
                    RequestAdditionalTime(60000);

                running = true;
                modelSignal = new AutoResetEvent(false);
                forecastSignal = new AutoResetEvent(false);
                baselineSignal = new AutoResetEvent(false);

                //Start: SQL DM 10.0 -- Doctor Integration -- initializing
                scheduleAnalysisSignal = new AutoResetEvent(false);
                getMasterRecommendationSignal = new AutoResetEvent(false);
                previousScheduledAnalysisTask = new PAScheduledTask();
                previousScheduledAnalysisTask.ServerID = 0;
                previousScheduledAnalysisTask.StartTime = DateTime.Now;
                scheduleAnalysisThread = new Thread(new ThreadStart(RunScheduledPriscriptiveAnalysis));
                getMasterRecommendationThread = new Thread(new ThreadStart(RunGetMasterRecommendations));
                //End: SQL DM 10.0 -- Doctor Integration -- initializing

                modelThread = new Thread(new ThreadStart(RunModel));
                forecastThread = new Thread(new ThreadStart(RunForecast));
                baselineThread = new Thread(new ThreadStart(RunBaselineAnalysis));

                LOG.Info("Starting model thread...");
                modelThread.IsBackground = true;
                modelThread.Start();

                LOG.Info("Starting forecast thread...");
                forecastThread.IsBackground = true;
                forecastThread.Start();

                LOG.Info("Starting baselining thread...");
                baselineThread.IsBackground = true;
                baselineThread.Start();

                //Start: SQL DM 10.0 -- Doctor Integration -- initializing
                LOG.Info("Starting scheduleAnalysis thread...");
                scheduleAnalysisThread.IsBackground = true;
                scheduleAnalysisThread.Start();

                LOG.Info("Starting getMasterRecommendationThread...");
                getMasterRecommendationThread.IsBackground = true;
                getMasterRecommendationThread.Start();
                //End: SQL DM 10.0 -- Doctor Integration -- initializing

                // start remoting the IPrescriptiveAnalysisService interface
                System.Runtime.Remoting.RemotingConfiguration.RegisterWellKnownServiceType(
                    typeof(PrescriptiveAnalysisService), "Prescriptive", WellKnownObjectMode.SingleCall);

            }
        }

        protected override void OnStop()
        {
            using (LOG.InfoCall("OnStop"))
            {
                running = false;
                modelSignal.Set();
                forecastSignal.Set();
                baselineSignal.Set();
            }
        }

        private void RunModel()
        {
            using (LOG.InfoCall("RunModel"))
            {
                DateTime now = DateTime.Now;
                DateTime configTime = PredictiveAnalyticsConfiguration.ModelRebuildDateTime;
                DateTime timeOfRebuild = new DateTime(now.Year, now.Month, now.Day, configTime.Hour, configTime.Minute, 0);
                int minutesToNextRebuild = (int)((TimeSpan)(timeOfRebuild - now)).TotalMinutes;
                Stopwatch sw = new Stopwatch();

                // if we've already passed the rebuild time for today, then the next rebuild is tomorrow
                if (timeOfRebuild < now)
                {
                    timeOfRebuild = timeOfRebuild.AddHours(24);
                    minutesToNextRebuild = (int)((TimeSpan)(timeOfRebuild - now)).TotalMinutes;
                }

                try
                {
                    while (running)
                    {
                        LOG.DebugFormat("Sleeping {0} minutes until next rebuild [TimeOfModelRebuild = {1}].", minutesToNextRebuild, timeOfRebuild.ToShortTimeString());

                        // log the time of the rebuild
                        DataHelper.SetNextPredictiveAnalyticsModelRebuild(timeOfRebuild.ToUniversalTime());

                        // wait until we are supposed to work next
                        modelSignal.WaitOne(minutesToNextRebuild * 60 * 1000, false);

                        if (DataHelper.IsPredictiveAnalyticsEnabled())
                        {
                            sw.Reset(); sw.Start();
                            LOG.Debug("Building predictive models.");

                            PredictiveAnalyticsService.BuildModels();

                            sw.Stop();
                            LOG.DebugFormat("Time taken to build all predictive models: {0}", sw.Elapsed);

                            // signal the forecast thread to go ahead
                            forecastSignal.Set();
                        }

                        // calculate minutes to next work items and sleep until the rebuild 
                        now = DateTime.Now;
                        timeOfRebuild = timeOfRebuild.AddHours(24);
                        minutesToNextRebuild = timeOfRebuild > now ? (int)((TimeSpan)(timeOfRebuild - now)).TotalMinutes : 0;

                        if (minutesToNextRebuild < 0)
                            minutesToNextRebuild = 0;
                    }
                }
                catch (Exception ex)
                {
                    LOG.Error("Caught exception during model processing.", ex);
                }
            }
        }

        private void RunForecast()
        {
            using (LOG.InfoCall("RunForecast"))
            {
                DateTime now = DateTime.Now;
                DateTime timeOfNextForecast = now;
                int minutesToNextForecast = 0;
                int forecastInterval = PredictiveAnalyticsConfiguration.ForecastInteval;
                Stopwatch sw = new Stopwatch();

                try
                {
                    while (running)
                    {
                        LOG.DebugFormat("Woke up. MinutesToNextForecast = {0}", minutesToNextForecast);

                        // update the time for the next forecast
                        timeOfNextForecast = timeOfNextForecast.AddMinutes(forecastInterval);

                        if (DataHelper.IsPredictiveAnalyticsEnabled())
                        {
                            sw.Reset(); sw.Start();
                            LOG.Debug("Making forecasts.");

                            PredictiveAnalyticsService.MakeForecasts(forecastInterval);

                            sw.Stop();
                            LOG.DebugFormat("Time taken to make forecast for all servers: {0}", sw.Elapsed);

                            DataHelper.GroomExpiredForecasts();
                            LOG.Debug("Groomed expired forecasts.");
                        }

                        LOG.DebugFormat("Time of next forecast: {0}", timeOfNextForecast);

                        // calculate minutes to next forecast
                        minutesToNextForecast = timeOfNextForecast > now ? (int)((TimeSpan)(timeOfNextForecast - now)).TotalMinutes : 0;

                        if (minutesToNextForecast < 0)
                            minutesToNextForecast = 0;

                        LOG.DebugFormat("Sleeping {0} minutes until next forecast.", minutesToNextForecast);

                        // log time of the next forecast
                        DataHelper.SetNextPredictiveAnalyticsForecast(timeOfNextForecast.ToUniversalTime());

                        // wait until we are supposed to work next                        
                        forecastSignal.WaitOne(minutesToNextForecast * 60 * 1000, false);
                        now = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    LOG.Error("Caught exception during forecasting.", ex);
                }
            }
        }

        private void RunBaselineAnalysis()
        {
            using (LOG.InfoCall("RunBaselineAnalysis"))
            {
                DateTime now = DateTime.Now;
                DateTime configTime = PredictiveAnalyticsConfiguration.BaselineAnalysisDateTime;
                DateTime timeOfAnalysis = new DateTime(now.Year, now.Month, now.Day, configTime.Hour, configTime.Minute, 0);
                int minutesToNextAnalysis = (int)((TimeSpan)(timeOfAnalysis - now)).TotalMinutes;
                Stopwatch sw = new Stopwatch();

                // if we've already passed the rebuild time for today, then the next rebuild is tomorrow
                if (timeOfAnalysis < now)
                {
                    timeOfAnalysis = timeOfAnalysis.AddHours(24);
                    minutesToNextAnalysis = (int)((TimeSpan)(timeOfAnalysis - now)).TotalMinutes;
                }

                try
                {
                    while (running)
                    {
                        LOG.DebugFormat("Sleeping {0} minutes until next analysis [TimeOfAnalysis = {1}].", minutesToNextAnalysis, timeOfAnalysis.ToShortTimeString());

                        // wait until we are supposed to work next
                        modelSignal.WaitOne(minutesToNextAnalysis * 60 * 1000, false);

                        sw.Reset(); sw.Start();
                        LOG.Debug("Performing baseline analysis.");

                        // perform the analysis
                        BaselineAnalyticsService.PerformAnalysis();

                        sw.Stop();
                        LOG.DebugFormat("Time taken to analyze baselines: {0}", sw.Elapsed);

                        // calculate minutes to next work items and sleep until the rebuild 
                        now = DateTime.Now;
                        timeOfAnalysis = timeOfAnalysis.AddHours(24);
                        minutesToNextAnalysis = timeOfAnalysis > now ? (int)((TimeSpan)(timeOfAnalysis - now)).TotalMinutes : 0;

                        if (minutesToNextAnalysis < 0)
                            minutesToNextAnalysis = 0;
                    }
                }
                catch (Exception ex)
                {
                    LOG.Error("Caught exception during baseline analysis.", ex);
                }
            }
        }


        //Start: SQL DM 10.0 -- Doctor Integration -- added methods for prescriptive analysis scheduling
        private void RunGetMasterRecommendations()
        {
            using (LOG.InfoCall("RunGetMasterRecommendations"))
            {
                try
                {
                    while (running)
                    {
                        if (MasterRecommendations.MasterRecommendationsInformation.Count == 0)
                        {
                            DataHelper.PopulateMasterRecommendations();
                        }
                        if (MasterRecommendations.MasterRecommendationsInformation.Count != 0) { break; }
                        else { getMasterRecommendationSignal.WaitOne(minutesToRetryGetMasterRecommendation * 60 * 1000, false); }
                    }
                }
                catch (Exception ex)
                {
                    LOG.Error("Caught exception during RunGetMasterRecommendations.", ex);
                }
            }
        }

        private void RunScheduledPriscriptiveAnalysis()
        {
            using (LOG.InfoCall("RunScheduledPriscriptiveAnalysis"))
            {
                int secondsToNextAnalysis = 0;
                try
                {
                    DataHelper.RegisterServerChannel();
                    while (running)
                    {
                        PAScheduledTask st = GetNextScheduleTask();
                        secondsToNextAnalysis = (int)(st.StartTime - DateTime.Now).TotalSeconds;
                        if (secondsToNextAnalysis <= 0) { secondsToNextAnalysis = 1; }
                        bool set = scheduleAnalysisSignal.WaitOne(secondsToNextAnalysis * 1000, false);
                        if (!set && st.ServerID > 0)
                        {
                            PrescriptiveAnalysisService svc = new PrescriptiveAnalysisService();
                            svc.GetPrescriptiveAnalysisResult(st.ServerID);
                            previousScheduledAnalysisTask = st;
                        }
                        if (set)
                        { scheduleAnalysisSignal.Reset(); }
                    }
                }
                catch (Exception ex)
                {
                    LOG.Error("Caught exception during schedule analysis.", ex);
                }
            }
        }

        private PAScheduledTask GetNextScheduleTask()
        {
            PAScheduledTask nextTask = new PAScheduledTask();
            if (scheduleList == null)
                GetListOfScheduleTask();
            scheduleList = GetListOfSortedScheduleTaskForDay(scheduleList, DateTime.Now.DayOfWeek);
            bool noScheduleTaskRemainsForToday = true;
            foreach (PAScheduledTask ST in scheduleList)
            {
                if (ST.StartTime >= previousScheduledAnalysisTask.StartTime)
                {
                    if (ST.StartTime == previousScheduledAnalysisTask.StartTime)
                    {
                        if (ST.ServerID <= previousScheduledAnalysisTask.ServerID) { continue; }
                        if (ST.ServerID > previousScheduledAnalysisTask.ServerID)
                        {
                            noScheduleTaskRemainsForToday = false;
                            nextTask = ST;
                            break;
                        }
                    }
                    if (ST.StartTime > previousScheduledAnalysisTask.StartTime && ST.StartTime < DateTime.Now)
                    {
                        noScheduleTaskRemainsForToday = false;
                        nextTask = ST;
                        break;
                    }
                    if (ST.StartTime >= DateTime.Now)
                    {
                        noScheduleTaskRemainsForToday = false;
                        nextTask = ST;
                        break;
                    }
                }
            }
            if (noScheduleTaskRemainsForToday)
            {
                nextTask.ServerID = 0;
                DateTime nextDay = DateTime.Now.AddDays(1);
                nextTask.StartTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 0, 0, 0);
            }
            return nextTask;
        }

        private List<PAScheduledTask> GetListOfSortedScheduleTaskForDay(List<PAScheduledTask> scheduleList, DayOfWeek day)
        {
            List<PAScheduledTask> result = new List<PAScheduledTask>();
            foreach (PAScheduledTask ST in scheduleList)
            {
                if ((ST.SelectedDays & DayOfWeekToShort(day)) == DayOfWeekToShort(day))
                {
                    result.Add(ST);
                }
            }
            result.Sort();
            return result;
        }

        public static void SetScheduledTask(List<PAScheduledTask> list)
        {
            // wait for day to change so that DateTime.Now.Day do not differ in GetListOfScheduleTask() and GetNextScheduleTime()
            if (DateTime.Now.AddMinutes(1).DayOfWeek != DateTime.Now.DayOfWeek) { Thread.Sleep(60000); }

            DateTime dt = DateTime.Now;
            foreach (PAScheduledTask ST in list)
            {
                ST.StartTime = new DateTime(dt.Year, dt.Month, dt.Day, ST.StartTime.Hour, ST.StartTime.Minute, ST.StartTime.Second);
            }
            scheduleList = list;
            scheduleAnalysisSignal.Set();
        }

        private void GetListOfScheduleTask()
        {
            List<PAScheduledTask> list = DataHelper.GetPrescriptiveAnalysisSchedule();

            // wait for day to change so that DateTime.Now.Day do not differ in GetListOfScheduleTask() and GetNextScheduleTime()
            if (DateTime.Now.AddMinutes(1).DayOfWeek != DateTime.Now.DayOfWeek) { Thread.Sleep(60000); }

            DateTime dt = DateTime.Now;
            foreach (PAScheduledTask ST in list)
            {
                ST.StartTime = new DateTime(dt.Year, dt.Month, dt.Day, ST.StartTime.Hour, ST.StartTime.Minute, ST.StartTime.Second);
            }
            scheduleList = list;
        }

        public short DayOfWeekToShort(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Sunday:
                    return 1;
                case DayOfWeek.Monday:
                    return 4;
                case DayOfWeek.Tuesday:
                    return 8;
                case DayOfWeek.Wednesday:
                    return 16;
                case DayOfWeek.Thursday:
                    return 32;
                case DayOfWeek.Friday:
                    return 64;
                case DayOfWeek.Saturday:
                    return 128;
                default:
                    return 0;
            }
        }
        //End: SQL DM 10.0 -- Doctor Integration -- added methods for prescriptive analysis scheduling

    }
}
