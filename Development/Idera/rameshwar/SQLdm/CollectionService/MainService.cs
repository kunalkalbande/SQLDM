namespace Idera.SQLdm.CollectionService
{
    using System;
    using System.Diagnostics;
    using System.Net.NetworkInformation;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.ServiceProcess;
    using System.Threading;
    using Idera.SQLdm.CollectionService.Configuration;
    using Idera.SQLdm.Common.Messages;
    using Idera.SQLdm.Common.Services;
    using System.Text;

    partial class MainService : ServiceBase
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("MainService");
        private static int FLUSH_INTERVAL_DEFAULT = 5000;
        private static bool clustered = false;
        private static MessageDll messageDll;

        private Thread mainThread;
        private ManualResetEvent stopEvent;
        private bool reconfigure;
        private bool running;
        private bool consoleMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MainService"/> class.
        /// </summary>
        public MainService()
        {
            InitializeComponent();
            AutoLog = false;
        }

        public bool IsRunning
        {
            get { return running; }
        }

        internal static bool IsClustered
        {
            get { return clustered; }
        }

        /// <summary>
        /// Runs from console.
        /// </summary>
        public void RunFromConsole()
        {
            LOG.InfoFormat("Current FileTraceLevel = {0}", LOG.FileTraceLevel);
            using (LOG.InfoCall( "RunFromConsole"))
            {
                consoleMode = true;
                OnStart(null);

                LOG.Info("Running Service.  Press a key to exit.");
                Console.ReadKey();
                LOG.Info("Exiting Service.");

                OnStop();
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            LOG.InfoFormat("Current FileTraceLevel = {0}", LOG.FileTraceLevel);
            using (LOG.InfoCall( "OnStart"))
            {
                if (!consoleMode)
                    RequestAdditionalTime(60000);

                string instanceName = CollectionServiceConfiguration.InstanceName;
                // make sure the event log is connected
                Collection.EventLog = CollectionServiceInstaller
                    .RegisterEventSource(instanceName);

                LOG.Info("Starting main thread...");

                running = true;
                stopEvent = new ManualResetEvent(false);
                mainThread = new Thread(new ThreadStart(Run));
                mainThread.Start();
            }
        }

        /// <summary>
        /// SVCs the main.
        /// </summary>
        protected void Run()
        {
            using (LOG.InfoCall("Run"))
            {
                CheckConfiguration();
                reconfigure = false;

                NetworkMonitor monitor = NetworkMonitor.Default;

                int tries = -1;

                // configure remoting
                RemotingConfig remotingConfiguration = new RemotingConfig();
                while (IsRunning)
                {
                    tries++;
                    try
                    {
                        remotingConfiguration.Configure();
                        break;
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Error configuring communication - will retry in 15 seconds.", e);
                        if ((tries % 4) == 0)
                        {
                            Collection.WriteEvent(Collection.EventLog, EventLogEntryType.Error,
                                                  Status.CollectionServiceRemotingConfigError, Category.General,
                                                  e.Message);
                        }
                    }
                    if (stopEvent.WaitOne(15000, true) == false)
                        continue;
                }
                if (!IsRunning)
                    return;

                Collection.WriteEvent(Status.CollectionServiceStart, Category.General, CollectionServiceConfiguration.InstanceName);

                // publish configuration using wmi
                CollectionServiceConfiguration.PublishConfiguration();

                CollectionServiceConfiguration.OnServicePortChanged += ServicePortChanged;
                CollectionServiceConfiguration.OnManagementServiceChanged += ManagementServiceAddressChanged;

                int flushInterval = FLUSH_INTERVAL_DEFAULT;
                do
                {
                    LOG.Info("Opening session to Management Service");
                    do  //Loop attempting to open a session until the service receives a stop request or opens the session
                    {
                        if (!Collection.IsPaused)
                        {
                            if (reconfigure)
                            {
                                // this will make sure remoting is configured to use the ms in the config file
                                Collection.ManagementService.Initialize();
                            }
                            try
                            {
                                // request the collection service workload
                                Collection.ManagementService.OpenSession();
                                break;
                            }
                            catch (ServiceException se)
                            {
                                if (se.HRESULT == ServiceException.StatusToInt(Status.ErrorInvalidManagementServiceId))
                                {
                                    LOG.ErrorFormat(
                                        "The management service at {0} is not registered in the SQLdm Repository.  Please configure the management service.", 
                                        CollectionServiceConfiguration.ManagementServiceUri);
                                }
                                else
                                    LOG.Error("Error opening session: ", se);
                            }
                            catch (Exception exception)
                            {
                                if (exception is InvalidOperationException)
                                {
                                    LOG.Info("Management Service contacted --- attempting registration.");
                                    RegisterWithManagementService();
                                }
                                else
                                    LOG.Error("Error opening session: ", exception);
                            }
                        } else
                            LOG.Info("Collection is paused --- waiting to reacquire workload.");
                    } while (stopEvent.WaitOne(CollectionServiceConfiguration.DisconnectInterval*1000, false) == false);

                    reconfigure = false;

                    // start remoting the Collection service
                    RemotingConfiguration.RegisterWellKnownServiceType(typeof (CollectionService), "Collection",
                                                                       WellKnownObjectMode.SingleCall);

                    int deliveryTimeoutMs = CollectionServiceConfiguration.GetCollectionServiceElement().SnapshotDeliveryTimeoutInSeconds * 1000;

                    LOG.InfoFormat("Session to Management Service opened.  Work started. Delivery timeout is {0}ms.", deliveryTimeoutMs);

                    //SQLDM-28476 -- Enforce License Check at the beginning of Collection Service
					Collection.ManagementService.CheckLicense(true);


                    // Scheduled collections are queued and this loop handles delivery.
                    flushInterval = FLUSH_INTERVAL_DEFAULT;
                    int maxWThreads, maxCPThreads, availWThreads, availCPThreads;
                    while (!reconfigure && !Collection.IsPaused && stopEvent.WaitOne(flushInterval, false) == false)
                    {
                        ThreadPool.GetMaxThreads(out maxWThreads, out maxCPThreads);
                        ThreadPool.GetAvailableThreads(out availWThreads, out availCPThreads);
                        if (availWThreads < 10)
                        {
                            LOG.DebugFormat("There are {0} of {1} work threads and {2} of {3} CP threads available.",
                                            availWThreads, maxWThreads, availCPThreads, maxCPThreads);
                        }
                        if (availWThreads == 0)
                        {
                            Stopwatch stopwatch = new Stopwatch();
                            stopwatch.Start();
                            ThreadPool.QueueUserWorkItem(DisplayQueueWaitTime, stopwatch);
                        }

                        // allow sending a snapshot to the management service to take up to 60 seconds
                        flushInterval = Collection.Scheduled.FlushSnapshotQueue(deliveryTimeoutMs);
                    }
                } while (stopEvent.WaitOne(0, false) == false);
 
                // stop scheduled collection
                Collection.Scheduled.Stop();

                // wait for the work queue to stop
                Collection.JoinWorkQueue();
                Collection.OnDemandJoinWorkQueue();//SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix

                // if the last flush was successful flush any waiting data
                if (flushInterval == FLUSH_INTERVAL_DEFAULT)
                    Collection.Scheduled.FlushSnapshotQueue(10000);

                try
                {
                    // tell the management service we are going away
                    Collection.ManagementService.CloseSession();
                }
                catch (Exception exception)
                {
                    LOG.Warn("Error closing session", exception);
                }
                
                // kill the published configuration object
                CollectionServiceConfiguration.RevokeConfiguration();

                Collection.WriteEvent(Status.CollectionServiceStop, Category.General, CollectionServiceConfiguration.InstanceName);               
                LOG.Info("SvcMain received stop event.  Exiting");
            }
        }

        private void DisplayQueueWaitTime(object state)
        {
            Stopwatch stopwatch = state as Stopwatch; 
            if (stopwatch != null)
            {
                LOG.DebugFormat("Current threadpool wait time is {0}.", stopwatch.Elapsed);
                stopwatch.Stop();
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            using (LOG.InfoCall( "OnStop"))
            {
                // TODO - Have to synchronize this with the actual shutdown - can't return until shutdown has completed.
                running = false;
                stopEvent.Set();
            }
        }

#region ClusterNameChangeCheck
        enum COMPUTER_NAME_FORMAT
        {
            ComputerNameNetBIOS,
            ComputerNameDnsHostname,
            ComputerNameDnsDomain,
            ComputerNameDnsFullyQualified,
            ComputerNamePhysicalNetBIOS,
            ComputerNamePhysicalDnsHostname,
            ComputerNamePhysicalDnsDomain,
            ComputerNamePhysicalDnsFullyQualified
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool GetComputerNameEx(COMPUTER_NAME_FORMAT NameType, [Out] StringBuilder lpBuffer, ref int lpnSize);

        protected void CheckConfiguration()
        {
            int size = 260;
            StringBuilder nonClusteredName = new StringBuilder(size);

            // get the physical netbios name of the machine
            if (GetComputerNameEx(COMPUTER_NAME_FORMAT.ComputerNamePhysicalNetBIOS, nonClusteredName, ref size))
            {
                string machineName = Environment.MachineName; 
                string managementServiceName = CollectionServiceConfiguration.ManagementServiceAddress;
                
                // we are probably clustered if the machine name does not equal the physical machine netbios name
                clustered = !String.Equals(machineName, nonClusteredName.ToString(), StringComparison.InvariantCultureIgnoreCase);

                if (!string.Equals(machineName, managementServiceName, StringComparison.InvariantCultureIgnoreCase))
                {
                    // see if we are switching from clustered to non or vice-versa
                    if (string.Equals(nonClusteredName.ToString(), managementServiceName))
                    {
                        CollectionServiceConfiguration
                            .SetManagementServiceAddress(Environment.MachineName, CollectionServiceConfiguration.ManagementServicePort);
                        CollectionServiceConfiguration.Save();
                    }
                }
            }
        }
#endregion

        protected void RegisterWithManagementService()
        {
            string machineName = Environment.MachineName;
            string instanceName = CollectionServiceConfiguration.InstanceName;
            int port = CollectionServiceConfiguration.ServicePort;
            string address = null;
            string diskDriveOption = CollectionServiceConfiguration.DiskDriveOption;

            if (!IsClustered)
            {   // if clustered then we want to use machine name since it is probably the virtual server name
                try
                {
                    address = IPGlobalProperties.GetIPGlobalProperties().HostName;
                }
                catch (Exception e)
                {
                    /* */
                }
            }

            if (address == null)
                address = machineName;

            try
            {
                LOG.InfoFormat("Registering collection service: m={0}, i={1}, a={2}, p={3}, c={4}",
                                machineName, instanceName, address, port, IsClustered);

                IManagementService2 mgmtSvc = RemotingHelper.GetObject<IManagementService2>();
                Guid id = mgmtSvc.RegisterCollectionService(machineName, instanceName, address, port, false);
                if (id != Guid.Empty)
                    LOG.Info("Registration successful: id=" + id.ToString());
            } catch (Exception e)
            {
                LOG.Debug("Error registering collectin service", e);
            }
         }

        public void ServicePortChanged(int oldPort, int newPort)
        {
            LOG.DebugFormat("Service port changed old={0} new={1}", oldPort, newPort);
            new RemotingConfig().Reconfigure();
        }

       
        public void ManagementServiceAddressChanged(Uri oldUri, Uri newUri)
        {
            LOG.DebugFormat("Management service address changed old={0} new={1}", oldUri, newUri);

            reconfigure = true;
            Collection.IsPaused = false;
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
                LOG.ErrorFormat("Error formatting event log message: {0:F} - {1}", messageId, builder.ToString());
            }
        }
    }
}
