//------------------------------------------------------------------------------
// <copyright file="ManagementServiceConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.Configuration
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.IO;
    using System.Management.Instrumentation;
    using System.Reflection;
    using System.Security;
    using System.Security.AccessControl;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using BBS.TracerX;
    using Idera.SQLdm.Common;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Objects;

    /// <summary>
    /// One stop shopping for Management Service configuration data
    /// </summary>
    public sealed class ManagementServiceConfiguration
    {
        //TODO:  Add code to loop until a repository connection is established.  
        //TODO:  Retry interval needs to be configurable (config file only).
        //TODO:  What happens when the repository is not available.
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("ManagementServiceConfiguration");

        public delegate void ConfigValueChangedDelegate<T>(T oldValue, T newValue);
        public static event ConfigValueChangedDelegate<int> OnServicePortChanged;
        public static event ConfigValueChangedDelegate<SqlConnectionInfo> OnRepositoryConnectInfoChanged;

        private static ReaderWriterLock syncRoot;

        private static string    instanceName;

//        private static string    repositoryInstance;
//        private static string    repositoryDatabase;
//        private static bool      repositorySSPI;
//        private static string    repositoryUser;
//        private static string    repositoryPassword;
        private static int       servicePort;

        private static string configDataDirectory;
        private static string registryDataDirectory;
        private static string resolvedDataDirectory;

        private static TimeSpan? heartbeatInterval;

        private static SqlConnectionInfo connectInfo;
        private static ManagementServiceConfig publishedConfiguration;

        private static bool enableEvents;
        private static bool configDirty;
        private static bool needRefresh;

        static ManagementServiceConfiguration()
        {
            syncRoot = new ReaderWriterLock();
            needRefresh = true;

            connectInfo = new SqlConnectionInfo();
            connectInfo.ApplicationName = Constants.ManagementServiceConnectionStringApplicationName;

            // set the database connection factory for the object cache
            CachedObjectRepositoryConnectionFactory.ConnectionFactory = GetRepositoryConnection;
        }

        public static void LogConfiguration()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.AppendFormat("Management Service Instance Name: {0}\n", InstanceName);
            buffer.AppendFormat("Managemetn Service Remoting Port: {0}\n", ServicePort);
            buffer.AppendFormat("Management Service Data Path: {0}\n", DataPath);
            buffer.AppendFormat("Collection Service Heartbeat Interval: {0}\n", HeartbeatInterval);
            //buffer.AppendFormat("Repository Connection String: {0}\n", ConnectionString);
            buffer.AppendFormat("Repository Instance: {0}\n", RepositoryHost);
            buffer.AppendFormat("Repository Database: {0}\n", RepositoryDatabase);
            buffer.AppendFormat("Use Windows Authentication: {0}", RepositoryUseSSPI);
            if (!RepositoryUseSSPI)
            {
                buffer.AppendFormat("\nRepository User: {0}", RepositoryUser);
                buffer.AppendFormat("\nRepository Password: {0}", String.IsNullOrEmpty(RepositoryPassword) ? "Not Set!" : "********");
            }

            LOG.Info(buffer.ToString());
        }

        public static bool IsEventsEnabled
        {
            get
            {
                syncRoot.AcquireReaderLock(-1);
                bool result = enableEvents;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        public static void SetEventEnabled(bool newValue)
        {
            syncRoot.AcquireWriterLock(-1);
            enableEvents = newValue;
            syncRoot.ReleaseWriterLock();
        }

        public static bool RefreshNeeded
        {
            get
            {
                syncRoot.AcquireReaderLock(-1);
                bool result = needRefresh;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        /// <summary>
        /// Refresh the internal values from the config file values
        /// </summary>
        private static void internal_refresh()
        {
            syncRoot.AcquireWriterLock(-1);
            try
            {
                ManagementServiceElement element = GetManagementServiceElement();

                servicePort = element.ServicePort;
                connectInfo.InstanceName = element.RepositoryServer;
                connectInfo.DatabaseName = element.RepositoryDatabase;
                connectInfo.UseIntegratedSecurity = element.RepositoryWindowsAuthentication;
                if (!connectInfo.UseIntegratedSecurity)
                {
                    connectInfo.UserName = element.RepositoryUsername;
                    connectInfo.Password = element.RepositoryPassword;
                }

                configDataDirectory = element.DataDirectory;
                resolvedDataDirectory = String.Empty;

                needRefresh = false;
            }
            finally
            {
                syncRoot.ReleaseWriterLock();
            }

            CachedObjectRepositoryConnectionFactory.ConnectionFactory = GetRepositoryConnection;
        }

        public static void Save()
        {
            ManagementServiceElement oldElement = GetManagementServiceElement();

            syncRoot.AcquireWriterLock(0);
            try
            {
                if (configDirty)
                {
                    ManagementServiceElement element = new ManagementServiceElement();
                    element.InstanceName = instanceName;
                    element.ServicePort = servicePort;
                    element.RepositoryServer = connectInfo.InstanceName;
                    element.RepositoryDatabase = connectInfo.DatabaseName;
                    element.RepositoryWindowsAuthentication = connectInfo.UseIntegratedSecurity;
                    if (!connectInfo.UseIntegratedSecurity)
                    {
                        element.RepositoryUsername = connectInfo.UserName;
                        element.RepositoryPassword = connectInfo.Password;
                    }
                    element.TracerXSectionName = oldElement.TracerXSectionName;
//                    element.HeartbeatIntervalSeconds = (int)heartbeatInterval.TotalSeconds;
                    element.Save();

                    try
                    {
                        PublishConfiguration();
                    } catch (Exception e)
                    {
                        LOG.Error("Error resetting published configuration.", e);
                    }
                    configDirty = false;
                }
            }
            catch (Exception e)
            {
                LOG.Error("Error saving configuration", e);
            }
            finally
            {
                syncRoot.ReleaseWriterLock();
            }
            LogConfiguration();
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

                        if (!path.EndsWith("" + Path.DirectorySeparatorChar + "ManagementService", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (!path.EndsWith("" + Path.DirectorySeparatorChar + "Data", StringComparison.InvariantCultureIgnoreCase))
                                path = Path.Combine(path, "Data");

                            path = Path.Combine(path, "ManagementService");
                        }

                        resolvedDataDirectory = path;
                    }
                     else
                        path = resolvedDataDirectory;
                } finally
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
            } catch (SecurityException e)
            {
                LOG.Info("The service account is not authorized to read the data path from the registry: ", e);
            } catch (Exception e)
            {
                LOG.Warn("Unable read the data path from the registry: ", e);
            }

            return String.Empty;
        }

        public static string InstanceName
        {
            get
            {
                syncRoot.AcquireReaderLock(-1);
                string result = instanceName;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        public static void SetInstanceName(string value)
        {
            Debug.Assert(value != null);
            syncRoot.AcquireWriterLock(-1);
            instanceName = value;
            syncRoot.ReleaseWriterLock();
        }


        public static string RepositoryHost
        {
            get
            {
                if (RefreshNeeded)
                    internal_refresh();

                syncRoot.AcquireReaderLock(-1);
                string result = connectInfo.InstanceName;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        public static string RepositoryDatabase
        {
            get
            {
                if (RefreshNeeded)
                    internal_refresh();

                syncRoot.AcquireReaderLock(-1);
                string result = connectInfo.DatabaseName;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        public static string RepositoryUser
        {
            get
            {
                if (RefreshNeeded)
                    internal_refresh();

                syncRoot.AcquireReaderLock(-1);
                string result = connectInfo.UserName;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        public static string RepositoryPassword
        {
            get
            {
                if (RefreshNeeded)
                    internal_refresh();

                syncRoot.AcquireReaderLock(-1);
                string result = connectInfo.Password;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        public static bool RepositoryUseSSPI
        {
            get
            {
                if (RefreshNeeded)
                    internal_refresh();

                syncRoot.AcquireReaderLock(-1);
                bool result = connectInfo.UseIntegratedSecurity;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        public static TimeSpan HeartbeatInterval
        {
            get {
                if (RefreshNeeded)
                    internal_refresh();

                // TODO add code to get this from the database
                syncRoot.AcquireReaderLock(-1);
                TimeSpan result = heartbeatInterval.HasValue ? heartbeatInterval.Value : TimeSpan.FromSeconds(180);
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        public static void SetHeartbeatInterval(TimeSpan interval)
        {
            syncRoot.AcquireWriterLock(-1);
            heartbeatInterval = interval;
            configDirty = true;
            syncRoot.ReleaseWriterLock();
        }

        public static void SetRepositoryConnectInfo(SqlConnectionInfo sqlConnectInfo)
        {
            SqlConnectionInfo oldInfo = connectInfo;

            syncRoot.AcquireWriterLock(-1);
            try
            {
                if (RefreshNeeded)
                    internal_refresh();

                connectInfo = sqlConnectInfo;
                configDirty = true;
            }
            finally
            {
                syncRoot.ReleaseWriterLock();
            }

            if (OnRepositoryConnectInfoChanged != null)
                OnRepositoryConnectInfoChanged(oldInfo, sqlConnectInfo);
        }


        public static int ServicePort
        {
            get
            {
                if (RefreshNeeded)
                    internal_refresh();
                syncRoot.AcquireReaderLock(-1);
                int result = servicePort;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        public static void SetServicePort(int port)
        {
            if (port == -1)
                return;

            int oldPort = ServicePort;
            if (oldPort != port)
            {
                servicePort = port;
                configDirty = true;
                OnServicePortChanged(oldPort, port);
            }
        }

        public static string ConnectionString
        {
            get
            {
                if (RefreshNeeded)
                    internal_refresh();

                syncRoot.AcquireReaderLock(-1);
                string result = connectInfo.ConnectionString;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        public static SqlConnection GetRepositoryConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public static ManagementServiceElement GetManagementServiceElement()
        {
            ManagementServiceElement element = null;
            string instanceName = InstanceName;
            ManagementServicesSection section = ManagementServicesSection.GetSection();
            if (section != null)
            {
                element = section.ManagementServices[instanceName];
            }
            if (element == null)
            {
                element = new ManagementServiceElement();
                element.InstanceName = instanceName;
                element.RepositoryServer = Environment.MachineName;
                element.RepositoryDatabase = "SQLdmRepository";
                element.Save();
            }

            return element;
        }

        public static void ConfigureLogging(string overrideLogConfiguration) 
        {
            string logSection = null;

            Logger.FileLogging.Directory = "%EXEDIR%\\Logs";
            if (overrideLogConfiguration == null) 
            {
                // see if we can get the logging configuration 
                ManagementServiceElement element = GetManagementServiceElement();
                if (element != null) 
                {
                    logSection = element.TracerXSectionName;
                }
            } 
            else 
            {
                logSection = overrideLogConfiguration;
            }

            Exception logException = null;
            bool rc;
            try 
            {
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
            } catch (Exception e) {
                logException = e;
            }

            if (Environment.UserInteractive)
            {
                Logger.ConsoleLogging.FormatString = "{5:HH:mm:ss.fff} {1} {3} {2}+{6} {7}{8}";
                Logger.Root.ConsoleTraceLevel = BBS.TracerX.TraceLevel.Verbose;
            }
            Logger.FileLogging.Name = "SQLdmManagementService.tx1";

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

        /// <summary>
        /// Publish the management service configuration.  Does not
        /// throw an exception if the operation does not complete 
        /// successfully.
        /// </summary>
        internal static void PublishConfiguration()
        {
            try
            {
                // first see if the assembly is registered with wmi
                Assembly assembly = typeof(ManagementServiceConfiguration).Assembly;
                if (!Instrumentation.IsAssemblyRegistered(assembly))
                {
                    Instrumentation.RegisterAssembly(assembly);
                }
                bool registered = Instrumentation.IsAssemblyRegistered(assembly);

                RevokeConfiguration();

                ManagementServiceConfig wmiConfig = new ManagementServiceConfig();
                wmiConfig.InstanceName = InstanceName;
                wmiConfig.RepositoryHost = RepositoryHost;
                wmiConfig.RepositoryDatabase = RepositoryDatabase;
                wmiConfig.ServicePort = ServicePort;
//                wmiConfig.Published = true;
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
            if (publishedConfiguration != null)
            {
                Instrumentation.Revoke(publishedConfiguration);
            }
                
            publishedConfiguration = null;
        }
    }
    
    [InstrumentationClass(InstrumentationType.Instance)]
    [ManagedName("ManagementServiceConfiguration")]
    public class ManagementServiceConfig  // : System.Management.Instrumentation.Instance
    {
        public string InstanceName;
        public string RepositoryHost;
        public string RepositoryDatabase;
        public int ServicePort;
    }
    
    
}
