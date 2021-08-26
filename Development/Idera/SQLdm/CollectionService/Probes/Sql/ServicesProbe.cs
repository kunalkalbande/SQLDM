//------------------------------------------------------------------------------
// <copyright file="ServicesProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using Idera.SQLdm.CollectionService.Probes.Wmi;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    using System;
    using System.Data.SqlClient;
    using BBS.TracerX;
    using Collectors;
    using Common;
    using Common.Configuration;
    using Common.Snapshots;
    using Common.Services;


    /// <summary>
    /// Enter a description for this class
    /// </summary>
    internal sealed class ServicesProbe : SqlBaseProbe
    {
        #region fields

        private ServicesSnapshot services = null;
        private HandleSqlErrorDelegate sqlErrorDelegate;
        private ClusterCollectionSetting clusterCollectionSetting = ClusterCollectionSetting.Default;

        private WmiConfiguration _wmiConfig;
        private ServiceStatusProbe _statusProbe;
        private IAsyncResult _statusIAR;
        private string _machineName;
        private string _serverName;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skip permissions for CloudProviders</param>
        public ServicesProbe(SqlConnectionInfo connectionInfo, WmiConfiguration wmiConfiguration, ClusterCollectionSetting clusterCollectionSetting, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            sqlErrorDelegate = new HandleSqlErrorDelegate(HandleSqlException);
            LOG = Logger.GetLogger("ServicesProbe");
            services = new ServicesSnapshot(connectionInfo);
            _wmiConfig = wmiConfiguration;
            this.clusterCollectionSetting = clusterCollectionSetting;
        }

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Starts the probe, virtual method called by BaseProbe.BeginProbe().
        /// If this returns cleanly, probe must call FireCompletion()
        /// </summary>
        protected override void Start()
        {
            StartServicesCollector();
        }

        /// <summary>
        /// Define the Services collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void ServicesCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            // Permissions to be used by SQLBase Probe
            var hasServiceControlExecuteAccess = CollectionPermissions != CollectionPermissions.None &&
                                                 CollectionPermissions.HasFlag(CollectionPermissions
                                                     .EXECUTEMASTERXPSERVICECONTROL);

            SqlCommand cmd = SqlCommandBuilder.BuildServicesCommand(conn, ver, _wmiConfig, clusterCollectionSetting,
                hasServiceControlExecuteAccess);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ServicesCallback));
        }

        /// <summary>
        /// Starts the Services collector.
        /// </summary>
        private void StartServicesCollector()
        {
            StartGenericCollector(new Collector(ServicesCollector), services, "StartServicesCollector", "Services", null, sqlErrorDelegate, ServicesCallback, new object[] { });
        }

        /// <summary>
        /// Define the Services callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ServicesCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                if (rd != null)
                {
                    if (_wmiConfig.DirectWmiEnabled)
                    {
                        if (rd.Read())
                        {
                            if(cloudProviderId != CLOUD_PROVIDER_ID_AZURE)
                              {
                                _machineName = rd.GetString(0);
                                _serverName = rd.GetString(1);
                              }
                            return;
                        }
                        else
                            throw new InvalidOperationException("Data reader state is invalid");
                    }
                    InterpretServices(rd);
                }
            }

        }

        private void AWSServicesCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                if (rd != null)
                {
                
                    InterpretRDSServices(rd);
                }
            }

        }

        /// <summary>
        /// Callback used to process the data returned from the Services collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ServicesCallback(object sender, CollectorCompleteEventArgs e)
        {
          
           var nextCollector = _wmiConfig.DirectWmiEnabled
                                    ?new NextCollector(StartMachineNameCollector)//START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe before WmiServiceProbe
                                    //? new NextCollector(StartWmiServicesProbe)
                                    : new NextCollector(StartDTCStatusCollector);
            bool isSysAdmin = false;
            try
            {
                if (cloudProviderId == Constants.AmazonRDSId)
                {
                    using (SqlConnection connection = new SqlConnection(connectionInfo.ConnectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = @"select is_srvrolemember('sysadmin') AS SysAdmin;";//SQLdm 10.1 (srishti purohit) -IsSysAdmin check
                            using (var reader = command.ExecuteReader())
                            {

                                while (reader.Read())
                                {
                                    isSysAdmin = Convert.ToBoolean(reader["SysAdmin"]);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }


            if (cloudProviderId == Constants.AmazonRDSId && !isSysAdmin)
                nextCollector = new NextCollector(StartAWSCollector);
                GenericCallback(new CollectorCallback(ServicesCallback), services, "ServicesCallback", "Services",
                            null,new FailureDelegate(GenericFailureDelegate),
                            nextCollector,
                            sender, e,false,true);
        }

        private void StartAWSCollector()
        {
            StartGenericCollector(new Collector(StartAWSCollector), services, "StartAWSCollector", "Services", null, sqlErrorDelegate, AWSServicesCallback, new object[] { });

        }
        private void StartAWSCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            
            // Permissions to be used by SQLBase Probe
           
            SqlCommand cmd = SqlCommandBuilder.BuildAWSServicesCommand(conn);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(AWSServicesCallback));
        }
        private void AWSServicesCallback(object sender, CollectorCompleteEventArgs e)
        {
            var nextCollector = _wmiConfig.DirectWmiEnabled
                                   ? new NextCollector(StartMachineNameCollector)//START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe before WmiServiceProbe
                                                                                 //? new NextCollector(StartWmiServicesProbe)
                                   : new NextCollector(StartDTCStatusCollector);
          
            GenericCallback(new CollectorCallback(AWSServicesCallback), services, "AWSServicesCallback", "AWSServices",
                        null, new FailureDelegate(GenericFailureDelegate),
                        nextCollector,
                        sender, e, false, true);
        }

        //START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
        /// <summary>
        /// Starts the Machine Name collector.
        /// before the WMI probe for Service Status which needs the machine name 
        /// </summary>
        private void StartMachineNameCollector()
        {
            MachineNameProbe machineProbe = new MachineNameProbe(connectionInfo,cloudProviderId);
            machineProbe.BeginProbe(MachineNameCallback);
        }

        /// <summary>
        /// Machine Name Collector Call back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MachineNameCallback(object sender, SnapshotCompleteEventArgs e)
        {
            // This means we cancelled out the probe
            if (e.Result == Result.Failure)
                return;

            if (e.Result == Result.PermissionViolation)
            {
                this.services.ProbeError = e.Snapshot.ProbeError;
                this.FireCompletion(this.services, Result.PermissionViolation);
                ((IDisposable)sender).Dispose();
                return;
            }

            if (e.Snapshot != null)
            {
                var _machineSnapshot = e.Snapshot as MachineNameSnapshot;
                if (_machineSnapshot != null)
                    _machineName = _machineSnapshot.MachineName;
            }
            
            ((IDisposable)sender).Dispose();

            // start the next probe 
            StartWmiServicesProbe();
        }
		//END SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm

        private void StartWmiServicesProbe()
        {
            // Update Server Name
            if (string.IsNullOrEmpty(_serverName))
            {
                _serverName = _machineName;
            }

            // Update Product Version
            if (services.ProductVersion == null)
            {
                using (var connection = OpenConnection())
                {
                    services.ProductVersion = new ServerVersion(connection.ServerVersion);
                }
            }
            _statusProbe = new ServiceStatusProbe(_machineName, _wmiConfig);
            _statusProbe.ServiceNames = ServiceStatusProbe.GetServiceNames(null, _serverName, services.ProductVersion);
            _statusProbe.BeginProbe(OnServiceStatusComplete);
        }

        private void OnServiceStatusComplete(object sender, ProbeCompleteEventArgs args)
        {
            var servicevalues = args.Data as Dictionary<string, Service>;
            if (servicevalues != null)
            {
                foreach (var entry in servicevalues)
                {
                    var service = entry.Value;
                    var serviceName = service.ServiceType;
                    if (!services.ServiceDetails.ContainsKey(serviceName))
                    {
                        services.ServiceDetails.Add(serviceName, new Service(service.ServiceType));
                    }
                    services.ServiceDetails[serviceName].ServiceName = service.ServiceName;
                    services.ServiceDetails[serviceName].RunningState = service.RunningState;
                    services.ServiceDetails[serviceName].ProcessID = service.ProcessID;
                    services.ServiceDetails[serviceName].RunningSince = service.RunningSince;
                    services.ServiceDetails[serviceName].StartupType = service.StartupType;
                    services.ServiceDetails[serviceName].LogOnAs = service.LogOnAs;
                    services.ServiceDetails[serviceName].Description = service.Description;
                }
            }

            if (services.ProductVersion.Major > 9)
            {
                services.ServiceDetails.Remove(ServiceName.FullTextSearch);
            }

            FireCompletion(services, Result.Success);
        }

       
        /// <summary>
        /// Starts the DTC Status collector.
        /// </summary>
        void StartDTCStatusCollector()
        {
            StartGenericCollector(new Collector(DTCStatusCollector), services, "StartDTCStatusCollector", "DTC Status", services.ProductVersion, null, new object[] { });
        }

        /// <summary>
        /// Create DTC Status Collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void DTCStatusCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            // SQLdm 10.3 (Varun Chopra) Linux Support - skip DTC Status
            if (cloudProviderId.HasValue && cloudProviderId.Value == Constants.LinuxId)
            {
                DTCStatusFailureDelegate(null, null);
            }
            else
            {
                // Permissions to be used by SQLBase Probe
                var hasServiceControlExecuteAccess = CollectionPermissions != CollectionPermissions.None &&
                                 CollectionPermissions.HasFlag(CollectionPermissions.EXECUTEMASTERXPSERVICECONTROL);

                SqlCommand cmd = SqlCommandBuilder.BuildDTCStatusCommand(conn, ver, connectionInfo, _wmiConfig,
                    clusterCollectionSetting, hasServiceControlExecuteAccess, cloudProviderId);
                sdtCollector = new SqlCollector(cmd, true);
                sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DTCStatusCallback));
            }
        }

        /// <summary>
        /// DTC Status failure delegate
        /// We will receive a SQL error if DTC is not installed so we need to handle that
        /// </summary>
        /// <param name="snapshot">Snapshot to modify</param>
        private void DTCStatusFailureDelegate(Snapshot snapshot, Exception e)
        {
            if (snapshot.ProbeError != null)
            {
                this.services.ProbeError = snapshot.ProbeError;
                this.FireCompletion(this.services, Result.PermissionViolation);
                return;
            }
            services.ServiceDetails[ServiceName.DTC].RunningState = ServiceState.NotInstalled;
            FireCompletion(services, Result.Success);
        }

        /// <summary>
        /// Define the DTC Status callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void DTCStatusCallback(CollectorCompleteEventArgs e)
        {
            using (var rd = e.Value as SqlDataReader)
            {
                InterpretDTCStatus(rd);
            }

            FireCompletion(services, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void DTCStatusCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(DTCStatusCallback), services, "DTCStatusCallback", "DTC Status", new FailureDelegate(DTCStatusFailureDelegate),
                new FailureDelegate(GenericFailureDelegate), sender, e);   
        }

        /// <summary>
        /// Interpret Services data
        /// </summary>
        private void InterpretRDSServices(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretServices"))
            {
                try
                {
                    // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id and handle Service States
                   
                    while (dataReader.Read()) 
                    {
                        //if (dataReader.Read())
                        {
                            if (dataReader.FieldCount > 1)
                            {
                                {
                                    string _servicename=string.Empty;
                                    if (!dataReader.IsDBNull(0)) _servicename = dataReader.GetString(0);
                                    if (!string.IsNullOrEmpty(_servicename))
                                    { 
                                        ServiceName serviceName = ServiceName.SqlServer;
                                        if(_servicename.ToLower().Contains("agent"))
                                        {
                                            serviceName = ServiceName.Agent;
                                        }
                                        else if (_servicename.ToLower().Contains("full-text"))
                                        {
                                            serviceName = ServiceName.FullTextSearch;
                                        }
                                       
                                        if (!dataReader.IsDBNull(0))
                                            services.ServiceDetails[serviceName].ServiceName =_servicename;
                                        if (!dataReader.IsDBNull(5))
                                            services.ServiceDetails[serviceName].ProcessID = Convert.ToInt64(dataReader[5]);
                                        try
                                        {
                                            DateTime date=new DateTime();
                                            if (!dataReader.IsDBNull(6))
                                                    DateTime.TryParse(dataReader[6].ToString(),out date) ;
                                            if(date!=new DateTime())
                                                services.ServiceDetails[serviceName].RunningSince = date;

                                        }
                                        catch
                                        {

                                        }
                                        if (!dataReader.IsDBNull(2))
                                            services.ServiceDetails[serviceName].StartupType = dataReader.GetString(2);
                                        if (!dataReader.IsDBNull(7))
                                            services.ServiceDetails[serviceName].LogOnAs = dataReader.GetString(7);
                                        //if (!dataReader.IsDBNull(6))
                                        //    services.ServiceDetails[serviceName].Description = dataReader.GetString(6);

                                        ServiceState tempState = ServiceState.UnableToMonitor;

                                        if (!dataReader.IsDBNull(4))
                                        {
                                            tempState = ProbeHelpers.GetServiceState(dataReader.GetString(4));
                                            if (tempState == ServiceState.UnableToMonitor)
                                                services.OsMetricsUnavailable = true;
                                        }

                                        if (services.ServiceDetails[serviceName].RunningState ==
                                            ServiceState.UnableToMonitor)
                                            services.ServiceDetails[serviceName].RunningState = tempState;
                                    }
                                }
                            }
                            else break;
                        }
                    } 

                    services.ServiceDetails[ServiceName.SqlServer].RunningState = ServiceState.Running;

                    // This is confusing, but if the Full Text Service is not installed we get an error we cannot read past,
                    // so we need to try to read it last.  The "select '1'" is so we can pick up the error on the dataReader.NextResult()
                    // without breaking the above loop
                 

                    // We will get legit data back for SQL 2008 but we don't want to display "Not Installed" for Full Text Search
                    // so we remove it here

                    if (services.ProductVersion.Major > 9)
                    {
                        services.ServiceDetails.Remove(ServiceName.FullTextSearch);
                    }

                }
                catch (Exception e)
                {
                    //ProbeHelpers.LogAndAttachToSnapshot(services, LOG, "Error interpreting Services Collector: {0}", e,
                    //                                    false);
                    //GenericFailureDelegate(services);
                }
            }
        }
        private void InterpretServices(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretServices"))
            {
                try
                {
                    // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id and handle Service States
                    if (dataReader.FieldCount == 1)
                    {
                        services.ServiceDetails[ServiceName.Agent].RunningState =
                            ProbeHelpers.ReadServiceState(dataReader, LOG, cloudProviderId);
                        if (cloudProviderId != Constants.LinuxId)
                        {

                            services.ServiceDetails[ServiceName.SqlServer].RunningState =
                                ProbeHelpers.ReadServiceState(dataReader, LOG, cloudProviderId);

                            //SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --populate the scheduled refresh object with the browser service state
                            services.ServiceDetails[ServiceName.Browser].RunningState =
                                ProbeHelpers.ReadServiceState(dataReader, LOG, cloudProviderId);
                        }
                        else
                        {
                            services.ServiceDetails[ServiceName.SqlServer].RunningState = ServiceState.UnableToMonitor;
                            services.ServiceDetails[ServiceName.Browser].RunningState = ServiceState.UnableToMonitor;
                        }
                    }

                    do
                    {
                        if (dataReader.Read())
                        {
                            if (dataReader.FieldCount > 1)
                            {
                                if (dataReader.FieldCount == 2)
                                {
                                    services.LightweightPoolingEnabled = true;
                                }
                                else
                                {
                                    int serviceNameControl = -1;
                                    if (!dataReader.IsDBNull(0)) serviceNameControl = dataReader.GetInt32(0);
                                    if (serviceNameControl > -1 && serviceNameControl < 4)
                                    {
                                        ServiceName serviceName = (ServiceName) serviceNameControl;
                                        if (!dataReader.IsDBNull(1))
                                            services.ServiceDetails[serviceName].ServiceName = dataReader.GetString(1);
                                        if (!dataReader.IsDBNull(2))
                                            services.ServiceDetails[serviceName].ProcessID = dataReader.GetInt64(2);
                                        if (!dataReader.IsDBNull(3))
                                            services.ServiceDetails[serviceName].RunningSince =
                                                dataReader.GetDateTime(3);
                                        if (!dataReader.IsDBNull(4))
                                            services.ServiceDetails[serviceName].StartupType = dataReader.GetString(4);
                                        if (!dataReader.IsDBNull(5))
                                            services.ServiceDetails[serviceName].LogOnAs = dataReader.GetString(5);
                                        if (!dataReader.IsDBNull(6))
                                            services.ServiceDetails[serviceName].Description = dataReader.GetString(6);

                                        ServiceState tempState = ServiceState.UnableToMonitor;

                                        if (!dataReader.IsDBNull(7))
                                        {
                                            tempState = ProbeHelpers.GetServiceState(dataReader.GetString(7));
                                            if (tempState == ServiceState.UnableToMonitor)
                                                services.OsMetricsUnavailable = true;
                                        }

                                        if (services.ServiceDetails[serviceName].RunningState ==
                                            ServiceState.UnableToMonitor)
                                            services.ServiceDetails[serviceName].RunningState = tempState;
                                    }
                                }
                            }
                            else break;
                        }
                    } while (dataReader.NextResult());

                    services.ServiceDetails[ServiceName.SqlServer].RunningState = ServiceState.Running;

                    // This is confusing, but if the Full Text Service is not installed we get an error we cannot read past,
                    // so we need to try to read it last.  The "select '1'" is so we can pick up the error on the dataReader.NextResult()
                    // without breaking the above loop
                    try
                    {
                        dataReader.NextResult();
                        if (dataReader.HasRows)
                        {
                            services.ServiceDetails[ServiceName.FullTextSearch].RunningState =
                                ProbeHelpers.ReadServiceState(dataReader, LOG, cloudProviderId);
                        }
                    }
                    catch
                    {
                        services.ServiceDetails[ServiceName.FullTextSearch].RunningState = ServiceState.NotInstalled;
                    }

                    // We will get legit data back for SQL 2008 but we don't want to display "Not Installed" for Full Text Search
                    // so we remove it here

                    if (services.ProductVersion.Major > 9)
                    {
                        services.ServiceDetails.Remove(ServiceName.FullTextSearch);
                    }

                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(services, LOG, "Error interpreting Services Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(services);
                }
            }
        }

        private void InterpretDTCStatus(SqlDataReader dataReader)
        {
            try
            {
                // SQLdm 10.3 (Varun Chopra) Linux Support to pass cloud provider id
                services.ServiceDetails[ServiceName.DTC].RunningState = ProbeHelpers.ReadServiceState(dataReader, LOG, cloudProviderId);
            }
            catch (Exception )
            {
                return;
            }
        }

        /// <summary>
        /// Override the HandleSqlException method, because in this probe both Paused and unable to monitor are not errors
        /// </summary>
        private new void HandleSqlException(Snapshot snapshot, string collectorName, SqlException sqlException, SqlCollector collector)
        {
            if (sqlException.Number == 17142)
            {
                LOG.Info("Monitored server is paused");
                //Clear out any data collected so far
                services = new ServicesSnapshot(services.ServerName);
                //Set the server as paused
                services.ServiceDetails[ServiceName.SqlServer].RunningState = ServiceState.Paused;
                if (collector != null)
                    collector.Dispose();
                FireCompletion(services, Result.Success);
            }
            else
            {
                if (sqlException.Number == 2 || sqlException.Number == 53)
                {
                    LOG.Info("Monitored server cannot be contacted: " + sqlException.Message);
                    //Clear out any data collected so far
                    services = new ServicesSnapshot(services.ServerName);
                    //Set the server as paused
                    services.ServiceDetails[ServiceName.SqlServer].RunningState = ServiceState.UnableToMonitor;
                    if (collector != null)
                        collector.Dispose();
                    FireCompletion(services, Result.Success);
                }
                else
                {
                    //Any other error is unhandled and should follow the usual path
                    if (collector != null)
                    {
                        HandleSqlException(snapshot, collectorName, sqlException, collector.SqlText);
                        collector.Dispose();
                    }
                    else
                    {
                        HandleSqlException(snapshot, collectorName, sqlException);
                    }
                    FireCompletion(snapshot, Result.Failure);
                }
            }
        }


        #endregion

        #region interface implementations

        #endregion
    }
}
