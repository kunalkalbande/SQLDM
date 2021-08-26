using Idera.SQLdm.ManagementService.Monitoring.Data;

namespace Idera.SQLdm.ManagementService
{
    using System;
    using System.Diagnostics;
    using System.Runtime.Remoting;
    using System.ServiceProcess;
    using System.Text;
    using System.Threading;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Messages;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.ManagementService.Configuration;
    using Idera.SQLdm.ManagementService.Helpers;

    partial class MainService : ServiceBase
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("MainService");
        private const string BaseServiceName = "SQLdmManagementService";
        private static MessageDll messageDll;

        private ManualResetEvent stopEvent;
        private Thread main;
        private bool running;

        private object sync = new object();
        private bool consoleMode;

        public MainService()
        {
            InitializeComponent();
            AutoLog = false;

            // set the correct service name if we are running from the console
            if (Environment.UserInteractive && ServiceName.Equals("MainService"))
                ServiceName = BaseServiceName + "$" + ManagementServiceConfiguration.InstanceName;
        }

        public bool IsRunning
        {
            get { return running; }
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

                LOG.Info("Starting main thread...");
                stopEvent = new ManualResetEvent(false);
                main = new Thread(new ThreadStart(Run));
                main.Start();
            }
        }

        protected override void OnStop()
        {
            using (LOG.InfoCall( "OnStop"))
            {
                LOG.Info("Starting shutdown...");
                running = false;

                try
                {
                    // wait for the work queue to shutdown;
                    Management.JoinWorkQueue();
                }
                catch (Exception e)
                {
                    LOG.Error("Exception while waiting for worker thread termination", e);
                }
                try {
                    AskServiceControlManagerForMoreTime(65000);
                    // signal the main thread to stop
                    stopEvent.Set();
                    main.Interrupt();
                    // give it a minute to die
                    if (Thread.CurrentThread != main && !main.Join(TimeSpan.FromMinutes(1)))
                        main.Abort();
                }
                catch (Exception e)
                {
                    LOG.Error("Exception while waiting for main thread termination", e);
                }

                // set the clean shutdown flag and force a syncpoint of the prevalence object
                PersistenceManager.SyncPointPrevalenceData(true);
            }
        }

        protected override void OnShutdown()
        {
            using (LOG.InfoCall("OnShutdown"))
            {
                base.Stop();
            }
        }

        public void Run()
        {
            bool tryUpgrade = running = true;
            bool managementInitialized = false;

            NetworkMonitor monitor = NetworkMonitor.Default;

            using (LOG.InfoCall( "Run"))
            {
                ManagementServiceConfiguration.LogConfiguration();
                string instanceName = ManagementServiceConfiguration.InstanceName;
                
                ConfigureEventLog();

                // start remoting so we can start getting connections
                int tries = -1;
                RemotingConfig remotingConfiguration = new RemotingConfig();
                while (IsRunning)
                {
                    tries++;
                    try
                    {
                        remotingConfiguration.Configure();
                        break;
                    } catch (Exception e)
                    {
                        LOG.Error("Error configuring communication - will retry in 15 seconds.", e);
                        if ((tries % 4) == 0)
                            WriteEvent(this.EventLog, EventLogEntryType.Error, Status.ManagementServiceRemotingConfigError, Category.General, e.Message);
                    }
                    if (stopEvent.WaitOne(15000, true) == false)
                        continue;
                }
                if (!IsRunning)
                    return;
                
                WriteEvent(this.EventLog, EventLogEntryType.Information, Status.ManagementServiceStart, Category.General, instanceName);

                // publish our configuration object so we are easier to find
                ManagementServiceConfiguration.PublishConfiguration();
                while (IsRunning)
                {
                    try
                    {
                        do
                        {
                            // wait until we are able to get a valid database connection
                            string connect = string.Empty;
                            try
                            {
                                connect = ManagementServiceConfiguration.ConnectionString;
                            }
                            catch (System.Data.SqlClient.SqlException e)
                            {
                                LOG.ErrorFormat("Error initializing the Repository connection string- will retry in 60 seconds. {0}", e);
                                continue;
                            }
                            if (RepositoryHelper.TestRepositoryConnection(connect, this.EventLog))
                            {
                                try
                                {
                                    if (tryUpgrade)
                                    {
                                        Management.QueueDelegate(
                                            delegate() { RepositoryHelper.PostInstallUpgrade(connect); });
                                        tryUpgrade = false;
                                    }

                                    // initialize the management class 
                                    Management.Initialize();
                                    managementInitialized = true;
                                    // start the scheduled collection writer
                                    Management.ScheduledCollection.Start();
                                    // synchronize assets with pulse
                                    WebClient.WebClient.KickoffAssetSynchronization(null);
                                    break;
                                }
                                catch (ServiceException se)
                                {
                                    if (se.HRESULT ==
                                        ServiceException.StatusToInt(Status.ErrorInvalidManagementServiceId))
                                    {
                                        Management.ManagementServiceUnhappyNotification(Environment.MachineName,
                                                                                        instanceName, 101,
                                                                                        new object[]
                                                                                            {
                                                                                                "no longer registered with the repository"
                                                                                            });
                                        Management.WriteNoLongerRegisteredMessage("[no id set]");
                                    }
                                    else
                                    {
                                        LOG.Error(
                                            "Error initializing the Management class - will retry in 60 seconds.", se);
                                        if (managementInitialized)
                                            Management.ManagementServiceUnhappyNotification(Environment.MachineName,
                                                                                            instanceName, 102,
                                                                                            new object[]
                                                                                                {
                                                                                                    "no longer registered with the repository"
                                                                                                    ,
                                                                                                });
                                    }
                                }
                                catch (Exception e)
                                {
                                    LOG.Warn("Error initializing the Management class - will retry in 60 seconds.", e);
                                    if (managementInitialized)
                                        Management.ManagementServiceUnhappyNotification(Environment.MachineName,
                                                                                        instanceName, 103,
                                                                                        new object[]
                                                                                            {
                                                                                                "unable to connect to the SQLDM Repository"
                                                                                            });
                                }
                            }
                            else
                            {
                                LOG.Warn("Waiting until repository connection info updated or repository is online.");
                                if (managementInitialized)
                                    Management.ManagementServiceUnhappyNotification(Environment.MachineName,
                                                                                    instanceName, 100,
                                                                                    new object[]
                                                                                        {
                                                                                            "unable to connect to the SQLDM Repository"
                                                                                        });
                            }
                        } while (IsRunning && stopEvent.WaitOne(60000, true) == false);
                    }
                    catch(ThreadInterruptedException ex)
                    {
                        LOG.Info(ex.Message);
                    }
                    // reset ms alert flag
                    Management.ClearManagementServiceAlert();

                    if (IsRunning)
                    {
                        // start remoting the IManagementService interface
                        System.Runtime.Remoting.RemotingConfiguration.RegisterWellKnownServiceType(
                            typeof (ManagementService), "Management", WellKnownObjectMode.SingleCall);

                        System.Runtime.Remoting.RemotingConfiguration.RegisterWellKnownServiceType(
                            typeof(WebClient.WebClient), 
                            typeof(Idera.Newsfeed.Shared.Interface.IApplicationExec).FullName, 
                            WellKnownObjectMode.SingleCall);
                    }
                    int registrationCheckCounter = -1;
                    int maxWThreads, maxCPThreads, availWThreads, availCPThreads;

					
                    try
                    {
						//SQLDM-30528
						ApplicationHelper.GenerateRecommendationAlert(60);
						
                        while (IsRunning && stopEvent.WaitOne(60000, true) == false)
                        {
                            if (++registrationCheckCounter >= 5)
                            {
                                // check to see if we are the currently registered server every 5 minutes
                                try
                                {
                                    Management.CheckRegistration();
                                    registrationCheckCounter = 0;
                                }
                                catch (Exception)
                                {
                                    break;
                                }
                            }

                            // Check the license once per day.
                            LicenseHelper.CheckLicenseIfIntervalExpired(TimeSpan.FromDays(1));
                            if (Management.QueryMonitorUpgradeHelper.QueryMonitorNeedsUpgrading)
                            {
                                LOG.Info("Query Monitor needs upgrading.");
                                Management.QueryMonitorUpgradeHelper.Start();
                            }
                            else
                            {
                                LOG.Debug("Query Monitor does not need upgrading.");
                            }

                            if (Management.AlertsUpgradeHelper.AlertsNeedUpgrading)
                            {
                                LOG.Info("Alerts need upgrading.");
                                Management.AlertsUpgradeHelper.Start();
                            }
                            else
                            {
                                LOG.Debug("Alerts do not need upgrading.");
                            }

                            if (Management.BaselineUpgradeHelper.BaselineTemplatesNeedUpgrading)
                            {
                                LOG.Info("Baseline templates need upgrading.");
                                Management.BaselineUpgradeHelper.Start();
                            }
                            else
                            {
                                LOG.Debug("Baseline Templates do not need ugprading.");
                            }

                            if (Management.DatabaseStatisticsUpgradeHelper.DBStatsNeedUpgrading &&
                                !Management.DatabaseStatisticsUpgradeHelper.UpgradeRunning)
                            {
                                LOG.Info("Database Statistics need upgrading");
                                Management.DatabaseStatisticsUpgradeHelper.Start();
                            }
                            else
                            {
                                if (Management.DatabaseStatisticsUpgradeHelper.DBStatsNeedUpgrading)
                                {
                                    if (Management.DatabaseStatisticsUpgradeHelper.RowsRemaining > 0)
                                    {
                                        LOG.Info(String.Format("Database statistics upgrade running. {0} rows remain.",
                                                               Management.DatabaseStatisticsUpgradeHelper.RowsRemaining));
                                    }
                                    else
                                    {
                                        if (Management.DatabaseStatisticsUpgradeHelper.RowsRemaining == -1)
                                        {
                                            LOG.Info("Database statistics upgrade has started but rows remaining is not yet known.");
                                        }
                                        else
                                        {
                                            LOG.Info("Database statistics upgrade has started but no rows are known to remain. Upgrade loop stopping.");
                                        }
                                    }
                                }
                                else
                                {
                                    if(Management.DatabaseStatisticsUpgradeHelper!=null)
                                    Management.DatabaseStatisticsUpgradeHelper.Stop();
                                    LOG.Debug("Database Statistics do not need upgrading.");
                                }
                            }


                            ThreadPool.GetMaxThreads(out maxWThreads, out maxCPThreads);
                            ThreadPool.GetAvailableThreads(out availWThreads, out availCPThreads);


                            if (availWThreads < 10)
                            {
                                LOG.DebugFormat(
                                    "There are {0} of {1} work threads and {2} of {3} CP threads available.",
                                    availWThreads, maxWThreads, availCPThreads, maxCPThreads);
                            }
                        }
                    }
                    catch (ThreadInterruptedException ex)
                    {
                        LOG.Info(ex.Message);
                    }
					catch(Exception ex){
						LOG.Error(ex.Message);
					}
                }

                Management.ScheduledCollection.Stop(TimeSpan.FromSeconds(15));
                Management.QueryMonitorUpgradeHelper.Stop(TimeSpan.FromSeconds(5));
                if(Management.DatabaseStatisticsUpgradeHelper!=null) Management.DatabaseStatisticsUpgradeHelper.Stop();

                // revoke our configuration object
                ManagementServiceConfiguration.RevokeConfiguration();

                WriteEvent(this.EventLog, EventLogEntryType.Information, Status.ManagementServiceStop, Category.General, instanceName);
            }
        }

        public void AskServiceControlManagerForMoreTime(int milliSeconds)
        {
            if (!Environment.UserInteractive)
            {
                try
                {
                    this.RequestAdditionalTime(milliSeconds);
                } catch
                {
                    /* */
                }             
            }
        }

        public void RepositoryConnectInfoChanged(SqlConnectionInfo oldInfo, SqlConnectionInfo newInfo)
        {
            LOG.DebugFormat("Management service address changed old={0} new={1}", oldInfo.ConnectionString,
                            newInfo.ConnectionString);
        }

        private void ConfigureEventLog()
        {
            EventLog configuredEventLog =
                ManagementServiceInstaller.RegisterEventSource(ManagementServiceConfiguration.InstanceName);

            this.EventLog.Source = configuredEventLog.Source;
        }

        public static void WriteEvent(EventLog eventLog, EventLogEntryType entryType, Status messageId, Category categoryId, params string[] vars)
        {
            try
            {
                EventInstance instance = new EventInstance((long)messageId, (int)categoryId);
                instance.EntryType = entryType;
                eventLog.WriteEvent(instance, vars);
            }
            catch (Exception e)
            {
                LogWriteEventError(e, messageId, vars);
            }
        }

        public static void LogWriteEventError(Exception exception, Status messageId, params string[] vars)
        {
            LOG.ErrorFormat("Error writing to the event log: {0}", exception.Message);
            try
            {
                if (messageDll == null)
                    messageDll = new MessageDll();

                string message = messageDll.Format(messageId, vars);
                LOG.ErrorFormat(message);
            }
            catch (Exception e)
            {
                StringBuilder builder = new StringBuilder();
                if (vars != null)
                {
                    foreach (string var in vars)
                    {
                        if (builder.Length > 0)
                            builder.Append(",");
                        builder.AppendFormat("'{0}'", var);
                    }
                }
                LOG.ErrorFormat("Error formatting event log message: {0:F} - {1}", messageId, builder.ToString(), e);
            }
        }

    }
}
