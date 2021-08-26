//------------------------------------------------------------------------------
// <copyright file="PublisherDetailsProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Text;
using Idera.SQLdm.CollectionService.Probes.Wmi;
using Wintellect.PowerCollections;

//using System.Text.RegularExpressions;

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
    /// On-Demand probe for Publisher Queue
    /// </summary>
    internal sealed class PublisherDetailsProbe : SqlBaseProbe
    {
        #region fields

        private PublisherDetails publisherDetails = null;
        private int numberOfRecords = 200;
        private PublisherDetailsConfiguration config = null;
        private int _spOAContext;
        private bool _disableOle;
        private ClusterCollectionSetting clusterCollectionSetting = ClusterCollectionSetting.Default;
        //private Regex commandTypeRegex = new Regex(@"(?<=.*{CALL.*sp_MS)...", RegexOptions.IgnoreCase);
        //private Regex objectNameRegex = new Regex(@"(?<=.*{CALL.*sp_MS.*_).*(?=\])", RegexOptions.IgnoreCase);
       // private Regex argumentListRegex = new Regex(@"(?<=.*{.*}).*", RegexOptions.IgnoreCase);

        private WmiConfiguration _wmiConfig;
        private ServiceStatusProbe _statusProbe;
        private IAsyncResult _statusIAR;
        private string _machineName;
        private string _serviceName;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherDetailsProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="config"></param>
        /// <param name="disableOle"></param>
        /// <param name="spOAContext"></param>
        /// <param name="cloudProviderId">Skip permissions for CloudProviders</param>
        public PublisherDetailsProbe(SqlConnectionInfo connectionInfo, PublisherDetailsConfiguration config, WmiConfiguration wmiConfiguration, ClusterCollectionSetting clusterCollectionSetting, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("PublisherDetailsProbe");
            publisherDetails = new PublisherDetails(connectionInfo);
            this.config = config;
            _wmiConfig = wmiConfiguration;
            cloudProviderId = cloudProviderId;
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
            if (config != null && config.ReadyForCollection)
            {
                StartReplicationStatusCollector();
            }
            else
            {
                FireCompletion(publisherDetails, Result.Success);
            }
        }

        /// <summary>
        /// Define the ReplicationStatus collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void ReplicationStatusCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = SqlCommandBuilder.BuildReplicationStatusCommand(conn, ver, _wmiConfig, clusterCollectionSetting);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ReplicationStatusCallback));
        }

        /// <summary>
        /// Starts the Replication Status collector.
        /// exec sp_helpdistributor
        /// </summary>
        private void StartReplicationStatusCollector()
        {
            StartGenericCollector(new Collector(ReplicationStatusCollector), publisherDetails, "StartReplicationStatusCollector", "Replication Status", ReplicationStatusCallback, new object[] { });
        }

        /// <summary>
        /// Define the ReplicationStatus callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ReplicationStatusCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretReplicationStatus(rd);
            }
            if (_wmiConfig.DirectWmiEnabled)
                StartMachineNameCollector();//START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe before ServiceStatusProbe
                //StartAgentStatusCollection();
            else
                StartPublisherCountersCollector();
        }

        /// <summary>
        /// Callback used to process the data returned from the ReplicationStatus collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ReplicationStatusCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(ReplicationStatusCallback),
                            publisherDetails,
                            "ReplicationStatusCallback",
                            "Replication Status",
                            new FailureDelegate(ReplicationStatusFailureDelegate),
                            new FailureDelegate(GenericFailureDelegate),
                            sender,
                            e, true);
        }

        /// <summary>
        /// ReplicationStatus failure delegate
        /// </summary>
        /// <param name="snapshot">Snapshot to return with failure</param>
        private void ReplicationStatusFailureDelegate(Snapshot snapshot, Exception e)
        {
            StartAlternateDistributionStatusCollector();
        }

		//START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
        /// <summary>
        /// Starts the Machine Name collector.
        /// before the WMI probe for agent status which needs the machine name 
        /// </summary>
        private void StartMachineNameCollector()
        {
            // Passing CloudProvider information
            MachineNameProbe machineProbe = new MachineNameProbe(connectionInfo, cloudProviderId);
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
            if (e.Snapshot != null)
            {
                var _machineSnapshot = e.Snapshot as MachineNameSnapshot;
                if (_machineSnapshot != null)
                    _machineName = _machineSnapshot.MachineName;
            }

            ((IDisposable)sender).Dispose();

            // start the next probe 
            StartAgentStatusCollection();
        }
		//END SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm

        void StartAgentStatusCollection()
        {
            using (LOG.DebugCall("StartAgentStatusCollection"))
            {
                _statusProbe = new ServiceStatusProbe(_machineName, _wmiConfig);
                _statusProbe.ServiceNames = new string[] { _serviceName };
                _statusIAR = _statusProbe.BeginProbe(OnAgentStatusComplete);
            }
        }

        private void OnAgentStatusComplete(object sender, ProbeCompleteEventArgs args)
        {
            var services = args.Data as Dictionary<string, Service>;
            if (services != null && services.Count > 0)
            {
                var entries = Algorithms.ToArray(services);
                var entry = entries[0];
                publisherDetails.SQLAgentStatus = ProbeHelpers.GetServiceState(entry.Value.RunningState);
            }

            StartPublisherCountersCollector();
        }


        /// <summary>
        /// Define the AlternateDistributionStatus collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void AlternateDistributionStatusCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildAlternateDistributionStatusCommand(conn, ver);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(AlternateDistributionStatusCallback));
        }

        /// <summary>
        /// Define the Publication counter collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void PublicationCountersCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildPublicationCountersCommand(conn, ver, this.config);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(PublisherCountersCallback));
        }

        /// <summary>
        /// Starts the Alternate Replication Status collector.
        /// </summary>
        private void StartAlternateDistributionStatusCollector()
        {
            StartGenericCollector(new Collector(AlternateDistributionStatusCollector), publisherDetails, "StartAlternateDistributionStatusCollector", "Alternate Distribution Status", null, new object[] { });
        }

        /// <summary>
        /// ReplicationStatus failure delegate
        /// </summary>
        /// <param name="snapshot">Snapshot to return with failure</param>
        private void AlternateDistributionStatusFailureDelegate(Snapshot snapshot, Exception e)
        {
            StartPublisherCountersCollector();
        }

        /// <summary>
        /// Define the AlternateDistributionStatus callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void AlternateDistributionStatusCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretAlternateDistributionStatus(rd);
            }
            StartPublisherCountersCollector();
        }

        /// <summary>
        /// Starts the Publisher Counter collector.
        /// </summary>
        private void StartPublisherCountersCollector()
        {
            if (publisherDetails.ReplicationStatus != ReplicationState.NotInstalled || publisherDetails.ReplicationStatus == ReplicationState.Unknown)
            {
                StartGenericCollector(new Collector(PublisherCountersCollector), publisherDetails,
                                      "StartPublisherCountersCollector", "Publisher Counters", null, new object[] { });
            }
            else
            {
                LOG.Info("Skipping publisher counters - replication status is " + publisherDetails.ReplicationStatus);
                FireCompletion(publisherDetails, Result.Success);
            }
        }

        /// <summary>
        /// Define the Publisher Counter collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void PublisherCountersCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildPublicationCountersCommand(conn, ver, this.config);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(PublisherCountersCallback));
        }

        /// <summary>
        /// Define the AlternateDistributionStatus callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void PublisherCountersCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretPublisherCounters(rd);
            }
            FireCompletion(publisherDetails, Result.Success);
        }
        /// <summary>
        /// Callback used to process the data returned from the Publisher Counters collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void PublisherCountersCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(PublisherCountersCallback), publisherDetails, "PublisherDetailsCallback", "Publisher Details",
                            sender, e);
        }

        /// <summary>
        /// Interpret Replication Publisher Counters data
        /// </summary>
        private void InterpretPublisherCounters(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretPublisherCounters"))
            {
                try
                {
                    //publisherDetails.SubscriptionsDictionary
                    publisherDetails.PublishedDatabases.BeginLoadData();
                    do
                    {
                        while (dataReader.Read())
                        {
                            DataRow dr = publisherDetails.PublishedDatabases.NewRow();

                            if (!dataReader.IsDBNull(0)) dr["publisherDB"] = dataReader.GetString(0);
                            if (!dataReader.IsDBNull(1)) dr["replicatedTransactions"] = dataReader.GetInt32(1);
                            if (!dataReader.IsDBNull(2)) dr["replicationRate"] = dataReader.GetFloat(2);
                            if (!dataReader.IsDBNull(3)) dr["replicationLatency"] = dataReader.GetFloat(3);

                            publisherDetails.PublishedDatabases.Rows.Add(dr);
                        }
                    } while (dataReader.NextResult());

                    publisherDetails.PublishedDatabases.EndLoadData();
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(publisherDetails, LOG, "Error interpreting Publisher Details Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(publisherDetails);
                }
            }
        }

        /// <summary>
        /// Callback used to process the data returned from the AlternateDistributionStatus collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void AlternateDistributionStatusCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(AlternateDistributionStatusCallback),
                           publisherDetails,
                           "AlternateDistributionStatusCallback",
                           "Alternate Distribution Status",
                           new FailureDelegate(AlternateDistributionStatusFailureDelegate),
                           new FailureDelegate(GenericFailureDelegate),
                           sender,
                           e, true);
        }

        /// <summary>
        /// Interpret ReplicationStatus data
        /// </summary>
        private void InterpretReplicationStatus(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretReplicationStatus"))
            {
                try
                {
                    if (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(1))
                            publisherDetails.ReplicationStatus =
                                (ReplicationState)dataReader.GetInt32(1);
                        dataReader.NextResult();
                    }

                    if (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(1))
                            publisherDetails.Distributor = dataReader.GetString(1);
                        if (!dataReader.IsDBNull(2))
                            publisherDetails.DistributionDatabase = dataReader.GetString(2);
                        dataReader.NextResult();
                    }

                    if (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(0))
                        {
                            if (_wmiConfig.DirectWmiEnabled)
                            {
                                //_machineName = dataReader.GetString(0);//START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe to get this value
                                _serviceName = dataReader.GetString(1);
                            }
                            publisherDetails.SQLAgentStatus = dataReader.GetString(0);
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(publisherDetails, LOG, "Error interpreting Replication Status Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(publisherDetails);
                }
            }
        }

        /// <summary>
        /// Interpret AlternateDistributionStatus data
        /// </summary>
        private void InterpretAlternateDistributionStatus(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretAlternateDistributionStatus"))
            {
                try
                {
                    if (dataReader.Read())
                    {
                        if (!dataReader.IsDBNull(0))
                            publisherDetails.ReplicationStatus = (dataReader.GetBoolean(0))
                                                                   ? ReplicationState.Installed
                                                                   : ReplicationState.NotInstalled;
                        if (!dataReader.IsDBNull(1))
                            publisherDetails.Distributor = dataReader.GetString(1);
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(publisherDetails, LOG, "Error interpreting Alternate Distribution Status Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(publisherDetails);
                }
            }
        }

        #endregion

        #region interface implementations

        #endregion
    }
}
