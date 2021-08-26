//------------------------------------------------------------------------------
// <copyright file="CollectionServiceConfigurationSection.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.CollectionService.Configuration
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Management.Instrumentation;
    using System.Reflection;
    using System.Security;
    using System.Threading;
    using System.Xml;
    using BBS.TracerX;
    //using BBS.TracerX.Config;

    /// <summary>
    /// Custom configuration section for the collection service.  Contains all
    /// values needed for the collection service to run.
    /// </summary>
    public static class CollectionServiceConfiguration 
    {
        public delegate void ConfigValueChangedDelegate<T>(T oldValue, T newValue);

        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("CollectionServiceConfiguration");

        private static ReaderWriterLock syncRoot;
        
        public static event ConfigValueChangedDelegate<int> OnServicePortChanged;
        public static event ConfigValueChangedDelegate<TimeSpan> OnHeartbeatIntervalChanged;
        public static event ConfigValueChangedDelegate<Uri> OnManagementServiceChanged;

        private static Guid collectionServiceId;
        private static Uri managementServiceUri;

        private static string instanceName;
        private static string managementServiceAddress;
        private static int managementServicePort = -1;
        private static TimeSpan heartbeatInterval = TimeSpan.MinValue;
        private static int servicePort = -1;

        private static string diskDriveOption;
        private static string logName;
        private static TimeSpan wmiQueryTimeout = TimeSpan.FromSeconds(90);//SQLdm 8.5 (Gaurav Karwal): changing the default to 90 seconds as requested

        private static string configDataDirectory;
        private static string registryDataDirectory;
        private static string resolvedDataDirectory;

        private static CollectionServiceElement configElement;
        private static CollectionServiceConfig publishedConfiguration;

        private static bool enableEvents;
        private static bool configDirty;
        private static bool needRefresh;

        static CollectionServiceConfiguration()
        {
            needRefresh = true;
            syncRoot = new ReaderWriterLock();
        }

        public static string DataPath
        {
            get
            {
                if (RefreshNeeded)
                    internal_refresh();

                syncRoot.AcquireReaderLock(-1);
                string path = resolvedDataDirectory;
                syncRoot.ReleaseReaderLock();

                if (!String.IsNullOrEmpty(path))
                    return path;

                syncRoot.AcquireWriterLock(-1);
                try
                {   // check to make sure someone else didn't update it already
                    if (String.IsNullOrEmpty(resolvedDataDirectory))
                    {
                        if (registryDataDirectory == null)
                            registryDataDirectory = GetDataPathFromRegistry();

                        if (!String.IsNullOrEmpty(registryDataDirectory))
                        {
                            path = registryDataDirectory;
                        }
                        else if (!String.IsNullOrEmpty(configDataDirectory))
                        {
                            path = configDataDirectory;
                        }
                        else
                        {
                            path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                            path = Path.Combine(path, InstanceName);
                        }

                        if (!path.EndsWith("" + Path.DirectorySeparatorChar + "CollectionService", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (!path.EndsWith("" + Path.DirectorySeparatorChar + "Data", StringComparison.InvariantCultureIgnoreCase))
                                path = Path.Combine(path, "Data");

                            path = Path.Combine(path, "CollectionService");
                        }

                        resolvedDataDirectory = path;
                    }
                    else
                        path = resolvedDataDirectory;
                }
                finally
                {
                    syncRoot.ReleaseWriterLock();
                }
                return path;
            }
        }

        private const string REGISTRY_ROOT = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Idera\\SQLdm\\";
        private static string GetDataPathFromRegistry()
        {
            string key = REGISTRY_ROOT + InstanceName;
            try
            {
                return Microsoft.Win32.Registry.GetValue(key, "DataPath", String.Empty) as string;
            }
            catch (SecurityException e)
            {
                LOG.Info("The service account is not authorized to read the data path from the registry: ", e);
            }
            catch (Exception e)
            {
                LOG.Warn("Unable read the data path from the registry: ", e);
            }

            return String.Empty;
        }


        public static bool IsEventsEnabled
        {
            get
            {
                syncRoot.AcquireReaderLock(0);
                bool result = enableEvents;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        public static void SetEventEnabled(bool newValue)
        {
            syncRoot.AcquireWriterLock(0);
            enableEvents = newValue;
            syncRoot.ReleaseWriterLock();
        }

        public static bool RefreshNeeded
        {
            get
            {
                syncRoot.AcquireReaderLock(0);
                bool result = needRefresh;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        public static Guid CollectionServiceId
        {
            get
            {
                syncRoot.AcquireReaderLock(0);
                Guid result = collectionServiceId;
                syncRoot.ReleaseReaderLock();
                return result;
            }
            set
            {
                syncRoot.AcquireWriterLock(0);
                collectionServiceId = value;
                syncRoot.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Get/Set the name for this instance of the service.
        /// </summary>
        public static string InstanceName
        {
            get
            {
                syncRoot.AcquireReaderLock(0);
                string result = instanceName;
                syncRoot.ReleaseReaderLock();
                return result;
            }
            set
            {
                Debug.Assert(value != null);
                syncRoot.AcquireWriterLock(0);
                instanceName = value;
                syncRoot.ReleaseWriterLock();
            }
        }

        public static string ManagementServiceAddress
        {
            get
            {
                if (RefreshNeeded)
                {
                    internal_refresh();
                }
                syncRoot.AcquireReaderLock(0);
                string result = managementServiceAddress;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        /// <summary>
        /// Refresh the internal values from the config file values
        /// </summary>
        private static void internal_refresh()
        {
            syncRoot.AcquireWriterLock(0);

            CollectionServiceElement element = GetCollectionServiceElement();

            managementServiceAddress = element.ManagementServiceAddress;
            managementServicePort = element.ManagementServicePort;
            heartbeatInterval = TimeSpan.FromSeconds(element.HeartbeatIntervalSeconds);
            wmiQueryTimeout = TimeSpan.FromSeconds(element.WmiQueryTimeout);
            servicePort = element.ServicePort;
            //SQLDM-30012.Change DiskCollection statistics to Instance Specific.
            //Intialise DiskDriveOption
            diskDriveOption = element.DiskDriveOption;
            needRefresh = false;
            managementServiceUri = null;
            configDataDirectory = element.DataDirectory;

            syncRoot.ReleaseWriterLock();
        }

        public static int ManagementServicePort
        {
            get
            {
                if (RefreshNeeded)
                    internal_refresh();

                syncRoot.AcquireReaderLock(0);
                int result = managementServicePort;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        public static void SetManagementServiceAddress(string address, int? port)
        {
            bool fireEvent = false;

            Uri oldUri = ManagementServiceUri;

            syncRoot.AcquireWriterLock(0);
            try
            {
                if (RefreshNeeded)
                    internal_refresh();

                if (address != null && address != ManagementServiceAddress)
                {
                    managementServiceAddress = address;
                    managementServiceUri = null;
                    configDirty = true;
                    fireEvent = true;
                }
                if (port.HasValue && port.Value != ManagementServicePort)
                {
                    managementServicePort = port.Value;
                    managementServiceUri = null;
                    configDirty = true;
                    fireEvent = true;
                }
            }
            finally
            {
                syncRoot.ReleaseWriterLock();
            }
            if (fireEvent)
            {
                Uri newUri = ManagementServiceUri;
                FireOnManagementServiceChanged(oldUri, newUri);
            }
        }

        public static void FireOnManagementServiceChanged(Uri oldUri, Uri newUri)
        {
            if (OnManagementServiceChanged != null)
                OnManagementServiceChanged(oldUri, newUri);
        }

        public static int ServicePort
        {
            get
            {
                if (RefreshNeeded)
                    internal_refresh();
                syncRoot.AcquireReaderLock(0);
                int result = servicePort;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        //SQLDM-30012.Change DiskCollection statistics to Instance Specific.
        public static string DiskDriveOption
        {
            get
            {
                if (RefreshNeeded)
                    internal_refresh();
                syncRoot.AcquireReaderLock(0);
                string result = diskDriveOption;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        public static void SetServicePort(int port)
        {
            int oldPort = ServicePort;
            if (oldPort != port)
            {
                servicePort = port;
                configDirty = true;
                OnServicePortChanged(oldPort, port);
            }
        }

        //SQLDM-30012.Change DiskCollection statistics to Instance Specific.
        public static void SetDiskDriveOption(string diskOption)
        {
            string oldOption = DiskDriveOption;
            if (oldOption != diskOption)
            {
                diskDriveOption = diskOption;
                configDirty = true;
            }
        }

        private static void BuildManagementServiceUri()
        {
            UriBuilder u = new UriBuilder();
            u.Scheme = "tcp";

            CollectionServiceElement element = GetCollectionServiceElement();
            if (element != null)
            {
                u.Host = "127.0.0.1"; // element.ManagementServiceAddress;
                u.Port = element.ManagementServicePort;
            }
            else
            {
                u.Host = "localhost";
                u.Port = 5166;
            }

            managementServiceUri = u.Uri;
        }

        public static Uri ManagementServiceUri
        {
            get
            {
                if (RefreshNeeded)
                    internal_refresh();

                Uri result;
                // get a read lock
                syncRoot.AcquireReaderLock(0);
                if (managementServiceUri == null)
                {
                    // upgrade to a write lock if needed
                    syncRoot.ReleaseReaderLock();
                    syncRoot.AcquireWriterLock(0);
                    try
                    {
                        if (managementServiceUri == null)
                            BuildManagementServiceUri();
                    }
                    finally
                    {   // restore the read lock
                        syncRoot.ReleaseWriterLock();
                        syncRoot.AcquireReaderLock(0);
                    }
                }

                result = managementServiceUri;

                // release the read lock
                syncRoot.ReleaseReaderLock();

                return result;
            }
        }

        public static TimeSpan WMIQueryTimeout
        {
            get
            {
                return wmiQueryTimeout;
            }
        }

        public static TimeSpan HeartbeatInterval
        {
            get
            {
                if (RefreshNeeded)
                    internal_refresh();
                syncRoot.AcquireReaderLock(0);
                TimeSpan result = heartbeatInterval;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        public static void SetHeartbeatInterval(int seconds)
        {
            SetHeartbeatInterval(TimeSpan.FromSeconds(seconds));            
        }

        public static void SetHeartbeatInterval(TimeSpan newValue)
        {
            TimeSpan oldValue = HeartbeatInterval;
            if (oldValue != newValue)
            {
                syncRoot.AcquireWriterLock(0);
                heartbeatInterval = newValue;
                configDirty = true;
                syncRoot.ReleaseWriterLock();

                OnHeartbeatIntervalChanged(oldValue, newValue);
            }
        }

        public static CollectionServiceElement GetCollectionServiceElement()
        {
            CollectionServiceElement element = null;
            try
            {
                syncRoot.AcquireReaderLock(0);
                if (configElement == null)
                {
                    syncRoot.ReleaseReaderLock();
                    syncRoot.AcquireWriterLock(0);
                    try
                    {
                        if (configElement == null)
                            configElement = CollectionServiceElement.GetElement(instanceName);
                    }
                    finally
                    {
                        syncRoot.ReleaseWriterLock();
                        syncRoot.AcquireReaderLock(0);
                    }
                }
                element = configElement;
            }
            finally
            {
                syncRoot.ReleaseReaderLock();
            }
            return element;
        }

        public static void ConfigureLogging(string overrideLogConfiguration) {
            string logSection = null;
            Exception logException = null;

            Logger.FileLogging.Directory = "%EXEDIR%\\Logs";
            try
            {
                if (overrideLogConfiguration == null)
                {
                    // see if we can get the logging configuration 
                    CollectionServiceElement element = GetCollectionServiceElement();
                    if (element != null)
                    {
                        logSection = element.TracerXSectionName;
                    }
                }
                else
                {
                    logSection = overrideLogConfiguration;
                }

                bool rc;
                if (logSection == null)
                {
                    // Load configuration from the application config file.
                    rc = Logger.Xml.Configure();
                }
                else
                {
                    XmlElement section = ConfigurationManager.GetSection(logSection) as XmlElement;
                    if (section != null)
                    {
                        rc = Logger.Xml.ConfigureFromXml(section);
                    }
                }
            }
            catch (Exception e)
            {
                logException = e;
            }

            if (Environment.UserInteractive)
            {
                Logger.ConsoleLogging.FormatString = "{5:HH:mm:ss.fff} {1} {3} {2}+{6} {7}{8}";
                Logger.Root.ConsoleTraceLevel = BBS.TracerX.TraceLevel.Verbose;
            }

            Logger.FileLogging.Name = "SQLdmCollectionService.tx1";

            CheckAccess(Logger.FileLogging.Directory);
            
            Logger.FileLogging.Open();

            if (logException != null)
            {
                LOG.Error("Error configuring logger: ", logException);
            }
        }

        internal static void CheckAccess(string directory)
        {
            // make sure the logging directory is accessible
            if (!Directory.Exists(directory))
            {
                // attemtp to create the directory
                Directory.CreateDirectory(directory);
            }
            // see if we can open/create a file in the log directory
            using (FileStream stream = File.Create(Path.Combine(directory, "SQLdm.IO.Test"), 80, FileOptions.DeleteOnClose | FileOptions.RandomAccess))
            {
                stream.Close();
            }
        }

        public static void Save()
        {
            CollectionServiceElement oldElement = GetCollectionServiceElement();

            syncRoot.AcquireWriterLock(0);
            try
            {
                if (configDirty)
                {
                    CollectionServiceElement element = new CollectionServiceElement();
                    element.ServicePort = servicePort;
                    element.InstanceName = instanceName;
                    element.HeartbeatIntervalSeconds = (int)heartbeatInterval.TotalSeconds;
                    element.TracerXSectionName = oldElement.TracerXSectionName;
                    element.ManagementServicePort = managementServicePort;
                    element.ManagementServiceAddress = managementServiceAddress;
                    element.Save();

                    try
                    {
                        PublishConfiguration();
                    }
                    catch (Exception e)
                    {
                        LOG.Error("Error resetting published configuration.");
                    }

                    configDirty = false;
                }
            } catch (Exception e)
            {
                LOG.Error("Error saving configuration", e);
            } finally
            {
                syncRoot.ReleaseWriterLock();
            }
        }

        public static int DisconnectInterval
        {
            get { return 5; }
        }

        internal static void PublishConfiguration()
        {
            try
            {
                // first see if the assembly is registered with wmi
                Assembly assembly = typeof(CollectionServiceConfiguration).Assembly;
                if (!Instrumentation.IsAssemblyRegistered(assembly))
                {
                    Instrumentation.RegisterAssembly(assembly);
                }
                
                if (publishedConfiguration != null)
                    RevokeConfiguration();

                LOG.Debug("Publishing configuration...");
                CollectionServiceConfig wmiConfig = new CollectionServiceConfig();
                
                wmiConfig.InstanceName = InstanceName;
                wmiConfig.ServicePort = ServicePort;
                wmiConfig.ManagementServiceMachine = ManagementServiceAddress;
                wmiConfig.ManagementServicePort = ManagementServicePort;
                wmiConfig.HeartbeatIntervalSeconds = (int)HeartbeatInterval.TotalSeconds;
                
                Instrumentation.Publish(wmiConfig);

                publishedConfiguration = wmiConfig;
            }
            catch (Exception e)
            {
                LOG.Error("Error publishing config data via wmi", e);
            }
        }

        internal static void RevokeConfiguration()
        {
            LOG.Debug("Revoking published configuration...");
            if (publishedConfiguration != null)
            {
                Instrumentation.Revoke(publishedConfiguration);
            }
                
            publishedConfiguration = null;
        }
    }

          
    [InstrumentationClass(InstrumentationType.Instance)]
    [ManagedName("CollectionServiceConfiguration")]
    public class CollectionServiceConfig
    {
        public string   InstanceName;
        public int      ServicePort;
        public string   ManagementServiceMachine;
        public int      ManagementServicePort;
        public int      HeartbeatIntervalSeconds;
        //SQLDM-30012.Change DiskCollection statistics to Instance Specific.
        public int      DiskDriveOption;
    }
}
