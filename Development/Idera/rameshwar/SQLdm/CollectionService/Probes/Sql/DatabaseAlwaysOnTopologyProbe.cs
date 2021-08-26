//------------------------------------------------------------------------------
// <copyright file="DatabaseAlwaysOnTopologyProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using Idera.SQLdm.CollectionService.Probes.Wmi;

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    using System;
    using System.Data.SqlClient;
    using BBS.TracerX;
    using Collectors;
    using Common;
    using Common.Configuration;
    using Common.Snapshots;
    using Common.Snapshots.AlwaysOn;
    using Common.Services;


    /// <summary>
    /// Enter a description for this class
    /// </summary>
    internal sealed class DatabaseAlwaysOnTopologyProbe : SqlBaseProbe
    {
        #region fields

        private AlwaysOnAvailabilityGroupsSnapshot alwaysOnTopologySnapshot = null;
        AlwaysOnAvailabilityGroupsConfiguration config = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseAlwaysOnTopologyProbe"/> class.
        /// </summary>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public DatabaseAlwaysOnTopologyProbe(SqlConnectionInfo connectionInfo,
                                    AlwaysOnAvailabilityGroupsConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("DatabaseAlwaysOnTopologyProbe");
            alwaysOnTopologySnapshot = new AlwaysOnAvailabilityGroupsSnapshot(connectionInfo);
            this.config = config;
            this.cloudProviderId = cloudProviderId;
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
            if (config != null && config.ReadyForCollection && cloudProviderId!=CLOUD_PROVIDER_ID_AZURE)//sqldm-30299 changes
            {
                StartDatabaseAlwaysOnTopologyCollector();
            }
            else
            {
                FireCompletion(alwaysOnTopologySnapshot, Result.Success);
            }
        }

        /// <summary>
        /// Define the Database AlwaysOn Statistics collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void DatabaseAlwaysOnTopologyCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            var cmd = SqlCommandBuilder.BuildDatabaseAlwaysOnTopologyCommand(conn, ver);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DatabaseAlwaysOnTopologyCallback));
        }

        /// <summary>
        /// Starts the Database Summary collector.
        /// </summary>
        private void StartDatabaseAlwaysOnTopologyCollector()
        {
            StartGenericCollector(new Collector(DatabaseAlwaysOnTopologyCollector), alwaysOnTopologySnapshot, "StartDatabaseAlwaysOnTopologyCollector", "Database AlwaysOn Statistics", null, new object[] { });
        }

        /// <summary>
        /// Define the Database AlwaysOn Statistics callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DatabaseAlwaysOnTopologyCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretDatabaseAlwaysOnTopology(rd);
            }
            FireCompletion(alwaysOnTopologySnapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the Database AlwaysOn Statistics collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DatabaseAlwaysOnTopologyCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(DatabaseAlwaysOnTopologyCallback), alwaysOnTopologySnapshot, "DatabaseAlwaysOnTopologyCallback", "Database AlwaysOn Statistics", sender, e);
        }

        /// <summary>
        /// Interpret Database AlwaysOn Statistics data
        /// </summary>
        private void InterpretDatabaseAlwaysOnTopology(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretDatabaseAlwaysOnTopology"))
            {
                try
                {
                    while (dataReader.Read())
                    {
                        // Fill 'Availability Group'.
                        AvailabilityGroup availabilityGroup = new AvailabilityGroup();

                        int ordinalColumnIndex;

                        if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("group_id")))
                        {
                            availabilityGroup.GroupId = dataReader.GetGuid(ordinalColumnIndex);
                        }

                        if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("Groupname")))
                        {
                            availabilityGroup.GroupName = dataReader.GetString(ordinalColumnIndex);
                        }

                        if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("listenerDNSName")))
                        {
                            availabilityGroup.ListenerDnsName = dataReader.GetString(ordinalColumnIndex);
                        }

                        if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("listnerPort")))
                        {
                            availabilityGroup.ListenerPort = dataReader.GetInt32(ordinalColumnIndex);
                        }

                        if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("listenerIPAddress")))
                        {
                            availabilityGroup.ListenerIPAddress = dataReader.GetString(ordinalColumnIndex);
                        }

                        if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("ServerSourceName")))
                        {
                            availabilityGroup.ServerSourceName = dataReader.GetString(ordinalColumnIndex);
                        }

                        alwaysOnTopologySnapshot.AddAvailabilityGroup(availabilityGroup);
                    }

                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            int ordinalColumnIndex;

                            if (dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("group_id")))
                                continue; //IF the group Id is not present then continue with the next record
                            Guid groupID = dataReader.GetGuid(ordinalColumnIndex); //get the Group ID

                            AvailabilityGroup availabilityGroup = null;
                            try
                            {
                                availabilityGroup = alwaysOnTopologySnapshot.GetAvailabilityGroup(groupID);
                            }
                            catch (Exception e)
                            {
                                LOG.Warn(string.Format("Alwayson: Group GUID {0} For the Replica {1} is not found in this Availability group",
                                    groupID, dataReader.GetGuid(0)));
                                if (!(e is KeyNotFoundException))
                                    LOG.Error(e);
                            }
                            if (availabilityGroup == null) //the group id is not found in the list
                                continue;

                            // Build 'AvailabilityReplica' based in the probe response.
                            AvailabilityReplica availabilityReplica = new AvailabilityReplica(availabilityGroup.GroupId);

                            if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("replica_id")))
                            {
                                availabilityReplica.ReplicaId = dataReader.GetGuid(ordinalColumnIndex);
                            }
                            if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("Replica_Name")))
                            {
                                availabilityReplica.ReplicaName = dataReader.GetString(ordinalColumnIndex);
                            }
                            if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("failover_mode")))
                            {
                                availabilityReplica.FailoverMode =
                                    AvailabilityReplica.ConvertToFailoverMode(dataReader.GetByte(ordinalColumnIndex));
                            }
                            if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("availability_mode")))
                            {
                                availabilityReplica.AvailabilityMode =
                                    AvailabilityReplica.ConvertToAvailabilityMode(dataReader.GetByte(ordinalColumnIndex));
                            }
                            if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("Replica_Role")))
                            {
                                availabilityReplica.ReplicaRole =
                                    AlwaysOnStatistics.ConvertToReplicaRole(dataReader.GetByte(ordinalColumnIndex));
                            }
                            if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("primary_role_allow_connections")))
                            {
                                availabilityReplica.PrimaryConnectionMode =
                                    (PrimaryConnectionMode)dataReader.GetByte(ordinalColumnIndex);
                            }
                            if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("secondary_role_allow_connections")))
                            {
                                availabilityReplica.SecondaryConnectionMode =
                                    (SecondaryConnectionMode) dataReader.GetByte(ordinalColumnIndex);
                            }
                            if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("redo_queue_size")))
                            {
                                availabilityReplica.RedoQueueSize = dataReader.GetInt64(ordinalColumnIndex);
                            }
                            if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("redo_rate")))
                            {
                                availabilityReplica.RedoRate = dataReader.GetInt64(ordinalColumnIndex);
                            }
                            if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("log_send_queue_size")))
                            {
                                availabilityReplica.LogSendQueueSize = dataReader.GetInt64(ordinalColumnIndex);
                            }
                            if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("log_send_rate")))
                            {
                                availabilityReplica.LogSendRate = dataReader.GetInt64(ordinalColumnIndex);
                            }
                            if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("ServerSourceName")))
                            {
                                availabilityReplica.ServerSourceName = dataReader.GetString(ordinalColumnIndex);
                            }

                            availabilityGroup.AddReplica(availabilityReplica);
                        }
                    }

                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            int ordinalColumnIndex;

                            if (dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("group_id")))
                                continue; //IF the group Id is not present then continue with the next record

                            Guid groupID = dataReader.GetGuid(ordinalColumnIndex); //get the Group ID

                            AvailabilityGroup availabilityGroup = null;

                            try
                            {
                                availabilityGroup = alwaysOnTopologySnapshot.GetAvailabilityGroup(groupID);
                            }
                            catch (Exception e)
                            {
                                LOG.Warn(string.Format("Alwayson: Group GUID {0} For the Replica {1} is not found in this Availability group",
                                    groupID, dataReader.GetGuid(0)));
                                if (!(e is KeyNotFoundException))
                                    LOG.Error(e);
                            }

                            if (availabilityGroup == null) //the group id is not found in the list
                                continue;

                            if (dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("replica_id")))
                                continue; //IF the replica Id is not present then continue with the next record

                            Guid replicaID = dataReader.GetGuid(ordinalColumnIndex); //get the replica ID

                            if (!availabilityGroup.Replicas.ContainsKey(replicaID))
                                continue;

                            AvailabilityReplica availabilityReplica = availabilityGroup.Replicas[replicaID];

                            // Build 'AlwaysOnDatabase' based in the probe response.
                            AlwaysOnDatabase availabilityDatabase = new AlwaysOnDatabase(availabilityReplica.ReplicaId, availabilityReplica.GroupId);

                            if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("group_database_id")))
                            {
                                availabilityDatabase.GroupDatabaseId = dataReader.GetGuid(ordinalColumnIndex);
                            }
                            if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("DatabaseName")))
                            {
                                availabilityDatabase.DatabaseName = dataReader.GetString(ordinalColumnIndex);
                            }
                            if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("database_id")))
                            {
                                availabilityDatabase.DatabaseId = dataReader.GetInt32(ordinalColumnIndex);
                            }
                            if (!dataReader.IsDBNull(ordinalColumnIndex = dataReader.GetOrdinal("ServerSourceName")))
                            {
                                availabilityDatabase.ServerSourceName = dataReader.GetString(ordinalColumnIndex);
                            }

                            availabilityReplica.AddDatabase(availabilityDatabase);
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(alwaysOnTopologySnapshot, LOG, "Error interpreting DatabaseAlwaysOnTopology Collector: {0}", e, false);
                    GenericFailureDelegate(alwaysOnTopologySnapshot);
                }
            }
        }

        #endregion

        #region interface implementations

        #endregion
    }
}
