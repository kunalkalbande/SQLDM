using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security;
using System.Reflection;
using System.Diagnostics;
using BBS.TracerX;
using System.Xml;
using System.Configuration;

namespace Idera.SQLdm.Service.Configuration
{
    public sealed class RestServiceConfiguration      
    {
        //TODO:  Add code to loop until a repository connection is established.  
        //TODO:  Retry interval needs to be configurable (config file only).
        //TODO:  What happens when the repository is not available.
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("RestServiceConfiguration");

        public delegate void ConfigValueChangedDelegate<T>(T oldValue, T newValue);
        public static event ConfigValueChangedDelegate<int> OnServicePortChanged;
        public static event ConfigValueChangedDelegate<SqlConnectionInfo> OnRepositoryConnectInfoChanged;

        private static ReaderWriterLock syncRoot;
        private static string    instanceName;
        private static int       servicePort; 
        private static SqlConnectionInfo connectInfo;
  
        private static bool enableEvents;
        private static bool configDirty;
        private static bool needRefresh;

        static RestServiceConfiguration()
        {
            syncRoot = new ReaderWriterLock();
            connectInfo = new SqlConnectionInfo();
            connectInfo.ApplicationName = Constants.RestServiceConnectionStringApplicationName;

            // set the database connection factory for the object cache
            CachedObjectRepositoryConnectionFactory.ConnectionFactory = GetRepositoryConnection;
        }

        public static void LogConfiguration()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.AppendFormat("Rest Service Instance Name: {0}\n", InstanceName);
            buffer.AppendFormat("Rest Service Remoting Port: {0}\n", ServicePort);
            buffer.AppendFormat("Repository Connection String: {0}\n", ConnectionString);
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
                RestServiceElement element = GetRestServiceElement();

                servicePort = element.ServicePort;
                connectInfo.InstanceName = element.RepositoryServer;
                connectInfo.DatabaseName = element.RepositoryDatabase;
                connectInfo.UseIntegratedSecurity = element.RepositoryWindowsAuthentication;
                if (!connectInfo.UseIntegratedSecurity)
                {
                    connectInfo.UserName = element.RepositoryUsername;
                    connectInfo.Password = element.RepositoryPassword;
                }
            }
            finally
            {
                syncRoot.ReleaseWriterLock();
            }

            CachedObjectRepositoryConnectionFactory.ConnectionFactory = GetRepositoryConnection;
        }

        public static void Save()
        {
            //RestServiceElement oldElement = GetRestServiceElement();

            syncRoot.AcquireWriterLock(0);
            try
            {
                if (configDirty)
                {
                    RestServiceElement element = new RestServiceElement();
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
                    element.TracerXSectionName = "REST API Configuration";
                    element.Save();
                    
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
                //if (RefreshNeeded)
                //    internal_refresh();

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
                //if (RefreshNeeded)
                //    internal_refresh();

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
                //if (RefreshNeeded)
                //    internal_refresh();

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
                //if (RefreshNeeded)
                //    internal_refresh();

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
                //if (RefreshNeeded)
                //    internal_refresh();

                syncRoot.AcquireReaderLock(-1);
                bool result = connectInfo.UseIntegratedSecurity;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }

        public static void SetRepositoryConnectInfo(SqlConnectionInfo sqlConnectInfo)
        {
            SqlConnectionInfo oldInfo = connectInfo;

            syncRoot.AcquireWriterLock(-1);
            try
            {
                //if (RefreshNeeded)
                //    internal_refresh();

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
                //if (RefreshNeeded)
                //    internal_refresh();
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
                //if (RefreshNeeded)
                //    internal_refresh();

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

        public static RestServiceElement GetRestServiceElement()
        {
            //Debugger.Launch();
            RestServiceElement element = null;
            string instanceName = InstanceName;
            RestServicesSection section = RestServicesSection.GetSection();
            if (section != null)
            {
                element = section.RestServices[instanceName];
            }
            if (element == null)
            {
                element = new RestServiceElement();
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
                RestServiceElement element = GetRestServiceElement();
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
            Logger.FileLogging.Name = "SQLdmRestService.tx1";

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

        public static SqlConnectionInfo SQLConnectInfo
        {
            get
            {
                //if (RefreshNeeded)
                //    internal_refresh();

                syncRoot.AcquireReaderLock(-1);
                SqlConnectionInfo result = connectInfo;
                syncRoot.ReleaseReaderLock();
                return result;
            }
        }     
    }
}
