//------------------------------------------------------------------------------
// <copyright file="DatabaseAlwaysOnStatisticsProbe.cs" company="Idera, Inc.">
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
    internal sealed class DatabaseAlwaysOnStatisticsProbe : SqlBaseProbe
    {
        #region fields

        private AlwaysOnAvailabilityGroupsSnapshot alwaysOnStatisticsSnapshot = null;
        AlwaysOnAvailabilityGroupsConfiguration config = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseAlwaysOnStatisticsProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public DatabaseAlwaysOnStatisticsProbe(SqlConnectionInfo connectionInfo,
                                    AlwaysOnAvailabilityGroupsConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("DatabaseAlwaysOnStatisticsProbe");
            alwaysOnStatisticsSnapshot = new AlwaysOnAvailabilityGroupsSnapshot(connectionInfo);
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
                StartDatabaseAlwaysOnStatisticsCollector();
            }
            else
            {
                FireCompletion(alwaysOnStatisticsSnapshot, Result.Success);
            }
        }

        /// <summary>
        /// Define the Database AlwaysOn Statistics collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void DatabaseAlwaysOnStatisticsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            var cmd = SqlCommandBuilder.BuildDatabaseAlwaysOnStatisticsCommand(conn, ver);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DatabaseAlwaysOnStatisticsCallback));
        }

        /// <summary>
        /// Starts the Database Summary collector.
        /// </summary>
        private void StartDatabaseAlwaysOnStatisticsCollector()
        {
            StartGenericCollector(new Collector(DatabaseAlwaysOnStatisticsCollector), alwaysOnStatisticsSnapshot, "StartDatabaseAlwaysOnStatisticsCollector", "Database AlwaysOn Statistics", null, new object[] { });
        }

        /// <summary>
        /// Define the Database AlwaysOn Statistics callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DatabaseAlwaysOnStatisticsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretDatabaseAlwaysOnStatistics(rd);
            }
            FireCompletion(alwaysOnStatisticsSnapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the Database AlwaysOn Statistics collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void DatabaseAlwaysOnStatisticsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(DatabaseAlwaysOnStatisticsCallback), alwaysOnStatisticsSnapshot, "DatabaseAlwaysOnStatisticsCallback", "Database AlwaysOn Statistics", sender, e);
        }

        /// <summary>
        /// Interpret Database AlwaysOn Statistics data
        /// </summary>
        private void InterpretDatabaseAlwaysOnStatistics(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretDatabaseAlwaysOnStatistics"))
            {
                try
                {
                    while (dataReader.Read())
                    {
                        AlwaysOnStatistics alwaysOnStatistics = new AlwaysOnStatistics();
                        if (!dataReader.IsDBNull(0)) alwaysOnStatistics.ReplicaId = dataReader.GetGuid(0);
                        if (!dataReader.IsDBNull(1)) alwaysOnStatistics.GroupId = dataReader.GetGuid(1);
                        if (!dataReader.IsDBNull(2)) alwaysOnStatistics.DatabaseId = dataReader.GetInt32(2);
                        if (!dataReader.IsDBNull(3)) alwaysOnStatistics.IsFailoverReady = dataReader.GetBoolean(3);

                        if (!dataReader.IsDBNull(4)) alwaysOnStatistics.SynchronizationDatabaseState = 
                            AlwaysOnStatistics.ConvertToAlwaysOnSynchronizationState( dataReader.GetByte(4));

                        if (!dataReader.IsDBNull(5)) alwaysOnStatistics.SynchronizationDatabaseHealth =
                            AlwaysOnStatistics.ConvertToAlwaysOnSynchronizationHealth(dataReader.GetByte(5));

                        if (!dataReader.IsDBNull(6)) alwaysOnStatistics.DatabaseState = 
                            AlwaysOnStatistics.ConvertToDatabaseStatusNoMaskBit(dataReader.GetByte(6));

                        if (!dataReader.IsDBNull(7)) alwaysOnStatistics.IsSuspended = dataReader.GetBoolean(7);
                        if (!dataReader.IsDBNull(8)) alwaysOnStatistics.LastHardenedTime = dataReader.GetDateTime(8);
                        if (!dataReader.IsDBNull(9)) alwaysOnStatistics.LogSendQueueSize = dataReader.GetInt64(9);
                        if (!dataReader.IsDBNull(10)) alwaysOnStatistics.LogSendRate = dataReader.GetInt64(10);
                        if (!dataReader.IsDBNull(11)) alwaysOnStatistics.RedoQueueSize = dataReader.GetInt64(11);
                        if (!dataReader.IsDBNull(12)) alwaysOnStatistics.RedoRate = dataReader.GetInt64(12);
                        
                        if (!dataReader.IsDBNull(13)) alwaysOnStatistics.ReplicaRole =
                            AlwaysOnStatistics.ConvertToReplicaRole(dataReader.GetByte(13));

                        if (!dataReader.IsDBNull(14)) alwaysOnStatistics.OperationalState =
                            AlwaysOnStatistics.ConvertToOperationalState(dataReader.GetByte(14));

                        if (!dataReader.IsDBNull(15)) alwaysOnStatistics.ConnectedState =
                            dataReader.GetByte(15) == 1 ? ConnectedState.Connected : ConnectedState.Disconnected;

                        if (!dataReader.IsDBNull(16)) alwaysOnStatistics.LastConnectionErrorNumber = dataReader.GetInt32(16);
                        if (!dataReader.IsDBNull(17)) alwaysOnStatistics.LastConnectedErrorDescription = dataReader.GetString(17);
                        if (!dataReader.IsDBNull(18)) alwaysOnStatistics.LastConnectErrorTimestamp = dataReader.GetDateTime(18);
                        if (!dataReader.IsDBNull(19)) alwaysOnStatistics.FileStreamSendRate = dataReader.GetInt64(19);
                        if (!dataReader.IsDBNull(20)) alwaysOnStatistics.SynchronizationPerformace = dataReader.GetInt32(20);
                        if (!dataReader.IsDBNull(21)) alwaysOnStatistics.EstimatedDataLossTime = dataReader.GetInt32(21);
                        if (!dataReader.IsDBNull(22)) alwaysOnStatistics.EstimatedRecoveryTime = dataReader.GetInt32(22);
                        if (!dataReader.IsDBNull(23)) alwaysOnStatistics.GroupDatabaseId = dataReader.GetGuid(23);

                        alwaysOnStatisticsSnapshot.AddReplicaStatistic(alwaysOnStatistics);
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(alwaysOnStatisticsSnapshot, LOG, "Error interpreting DatabaseAlwaysOnStatistics Collector: {0}", e, false);
                    GenericFailureDelegate(alwaysOnStatisticsSnapshot);
                }
            }
        }

        #endregion

        #region interface implementations

        #endregion
    }
}
