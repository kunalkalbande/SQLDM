using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.DesktopClient.Objects;
using BBS.TracerX;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Helpers
{
    /// <summary>
    /// Provides helpers for the Management Service remoting proxy.
    /// </summary>
    internal static class ManagementServiceHelper
    {
        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger(typeof(ManagementServiceHelper));

        static ManagementServiceHelper()
        {
            Initialize();
        }

        private static void Initialize()
        {
            using (Log.InfoCall("ManagementServiceHelper.Initialize"))
            {
                Log.Info("Registering client channels...");

                RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;

                IDictionary properties = new Hashtable();
                properties["name"] = "tcp-client";
                properties["impersonationLevel"] = "None";
                properties["impersonate"] = false;
                properties["secure"] = false;

                BinaryClientFormatterSinkProvider clientSinkProvider = new BinaryClientFormatterSinkProvider();
                TcpClientChannel tcpClientChannel = new TcpClientChannel(properties, clientSinkProvider);
                ChannelServices.RegisterChannel(tcpClientChannel, false);

                Log.Info("Client channels registered successfully.");

                Settings.Default.ActiveRepositoryConnectionChanged += new EventHandler(ConnectionChanged);
            }
        }

        private static void ConnectionChanged(object o, EventArgs e) {
            // Force the next reference to ManagementServiceHelper to look up IManagementService.
            Log.Debug("Setting ManagementServiceHelper._iManagementService to null.");
            _iManagementService = null;
        }

        /// <summary>
        /// This looks up the Management service address and port in the repository we
        /// are currently connected to and sets the IManagementService property to refer to that.
        /// It also returns IManagementService.
        /// </summary>
        public static IManagementService GetDefaultService() {
            return GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, true);
        }

        /// <summary>
        /// This looks up the Management service address and port in the repository specified
        /// by the connectionInfo parameter and sets the IManagementService property to refer to that.
        /// It also returns IManagementService.
        /// </summary>
        public static IManagementService GetDefaultService(SqlConnectionInfo connectionInfo) {
            return GetDefaultService(connectionInfo, true);
        }

        /// <summary>
        /// This looks up the Management service address and port in the repository specified
        /// by the connectionInfo parameter and returns the corresponding IManagementService.
        /// If the remember parameter is true, the properties IManagementService, ServerName,
        /// and ServerNameAndPort are all updated.
        /// </summary>
        public static IManagementService GetDefaultService(SqlConnectionInfo connectionInfo, bool remember) {
            using (Log.DebugCall()) {
                if (connectionInfo == null) {
                    Log.Warn("connectionInfo is null!");
                } else {
                    Log.DebugFormat("Instance = '{0}', Database = '{1}'.", connectionInfo.InstanceName, connectionInfo.DatabaseName);
                }

                Log.Debug("remember = ", remember);

                ManagementServiceConfiguration serviceConfig;

                lock (_locker) {
                    try {
                        serviceConfig = RepositoryHelper.GetDefaultManagementService(connectionInfo);
                        Log.Debug("Got service configuration: ", serviceConfig);
                    } catch (Exception e) {
                        throw new ApplicationException("Unable to retrieve a management service from SQLDM repository.", e);
                    }

                    if (serviceConfig == null) {
                        throw new ApplicationException("No management services have been registered with the SQLDM repository.");
                    } else {
                        IManagementService ims = GetService(serviceConfig);

                        if (remember) {
                            _iManagementService = ims;
                            _serverName = serviceConfig.Address;
                            _port = serviceConfig.Port;
                        }

                        return ims;
                    }
                }
            }
        }

        public static IManagementService GetService(ManagementServiceConfiguration serviceConfig)
        {
            //return GetService(serviceConfig.Address, serviceConfig.Port);
            if (serviceConfig.Address == null || serviceConfig.Address.Length == 0)
            {
                throw new ArgumentException("The management service host is invalid.", "host");
            }

            Uri uri = new Uri(String.Format("tcp://{0}:{1}/Management", serviceConfig.Address, serviceConfig.Port));

            ServiceCallProxy proxy = new ServiceCallProxy(typeof(IManagementService), uri.ToString());
            IManagementService ims = proxy.GetTransparentProxy() as IManagementService;

            return ims;
        }


        //private static IManagementService GetService(string host, int port)
        //{
        //    if (host == null || host.Length == 0)
        //    {
        //        throw new ArgumentException("The management service host is invalid.", "host");
        //    }

        //    Uri uri = new Uri(String.Format("tcp://{0}:{1}/Management", host, port));
        //    _iManagementService = RemotingHelper.GetObject<IManagementService>(uri.ToString());
        //    _serverName = host;
        //    _port = port;
        //    return _iManagementService;
        //}

        /// <summary>
        /// The Management Service object we are currently connected to.
        /// </summary>
        public static IManagementService IManagementService {
            get {
                lock (_locker) {
                    if (_iManagementService == null) {
                        return GetDefaultService();
                    } else {
                        return _iManagementService;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the server name and port in name:port format.
        /// </summary>
        public static string ServerAndPort
        {
            get {
                lock (_locker) {
                    if (_iManagementService == null) {
                        GetDefaultService();
                    }

                    return string.Format("{0}:{1}", _serverName, _port);
                }
            }
        }

        /// <summary>
        /// Gets just the server name sans port.
        /// </summary>
        public static string ServerName
        {
            get { 
                lock (_locker) {
                    if (_iManagementService == null) {
                        GetDefaultService();
                    }

                    return _serverName; 
                }
            }
        }

        private static object _locker = new object();
        private static IManagementService _iManagementService;
        private static string _serverName;
        private static int _port;
    }
}