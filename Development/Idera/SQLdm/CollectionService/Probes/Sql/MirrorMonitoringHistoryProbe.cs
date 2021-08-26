//------------------------------------------------------------------------------
// <copyright file="MirrorMonitoringHistoryProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

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
    internal sealed class MirrorMonitoringHistoryProbe : SqlBaseProbe
    {
        #region fields

        private MirrorMonitoringHistorySnapshot snapshot = null;
        private MirrorMonitoringHistoryConfiguration config = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MirrorMonitoringHistoryProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="config">The configuration object.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public MirrorMonitoringHistoryProbe(SqlConnectionInfo connectionInfo, MirrorMonitoringHistoryConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("MirrorMonitoringHistoryProbe");
            this.cloudProviderId = cloudProviderId;
            snapshot = new MirrorMonitoringHistorySnapshot(connectionInfo);
            this.config = config;
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
                StartMirrorMonitoringHistoryCollector();
            }
            else
            {
                FireCompletion(snapshot, Result.Success);
            }
        }

        #endregion

        #region interface implementations

        #endregion

        /// <summary>
        /// Define the MirrorMonitoringHistory collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void MirrorMonitoringHistoryCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildMirrorMonitoringHistoryCommand(conn, ver, config);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(MirrorMonitoringHistoryCallback));
        }

        /// <summary>
        /// Starts the MirrorMonitoringHistory collector.
        /// </summary>
        private void StartMirrorMonitoringHistoryCollector()
        {
            StartGenericCollector(new Collector(MirrorMonitoringHistoryCollector), snapshot, "StartMirrorMonitoringHistoryCollector", "MirrorMonitoringHistory", MirrorMonitoringHistoryCallback, new object[] { });
        }

        /// <summary>
        /// Define the MirrorMonitoringHistory callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void MirrorMonitoringHistoryCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretMirrorMonitoringHistory(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the MirrorMonitoringHistory collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void MirrorMonitoringHistoryCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(MirrorMonitoringHistoryCallback), snapshot, "MirrorMonitoringHistoryCallback", "MirrorMonitoringHistory",
                            sender, e);
        }

        /// <summary>
        /// Interpret MirrorMonitoringHistory data
        /// </summary>
        private void InterpretMirrorMonitoringHistory(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretMirrorMonitoringHistory"))
            {
                try
                {
                    
                    while (dataReader.Read())
                    {
                        MirroringMetrics metrics = new MirroringMetrics();
                        
                        if (!dataReader.IsDBNull(1)) metrics.Role = (MirroringMetrics.MirroringRoleEnum)dataReader.GetByte(1);
                        if (!dataReader.IsDBNull(2)) metrics.MirroringState = (MirroringMetrics.MirroringStateEnum) dataReader.GetByte(2);
                        if (!dataReader.IsDBNull(3)) metrics.WitnessStatus = (MirroringMetrics.WitnessStatusEnum)dataReader.GetByte(3);
                        if (!dataReader.IsDBNull(4)) metrics.LogGenerationRate = dataReader.GetInt32(4);
                        if (!dataReader.IsDBNull(5)) metrics.UnsentLog = dataReader.GetInt32(5);
                        if (!dataReader.IsDBNull(6)) metrics.SendRate = dataReader.GetInt32(6);

                        if (!dataReader.IsDBNull(7)) metrics.UnrestoredLog = dataReader.GetInt32(7);
                        if (!dataReader.IsDBNull(8)) metrics.RecoveryRate = dataReader.GetInt32(8);
                        if (!dataReader.IsDBNull(9)) metrics.TransactionDelay = dataReader.GetInt32(9);
                        if (!dataReader.IsDBNull(10)) metrics.TransactionsPerSec = dataReader.GetInt32(10);

                        if (!dataReader.IsDBNull(11)) metrics.AverageDelay = dataReader.GetInt32(11);
                        if (!dataReader.IsDBNull(12)) metrics.TimeRecorded = dataReader.GetDateTime(12);
                        if (!dataReader.IsDBNull(13)) metrics.TimeBehind = dataReader.GetDateTime(13);
                        if (!dataReader.IsDBNull(14)) metrics.LocalTime = dataReader.GetDateTime(14);

                        snapshot.Metrics.Add(metrics);
                        snapshot.MirroredDatabase = config.MirroredDatabaseName;
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot, LOG, "Error interpreting MirrorMonitoringHistory Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(snapshot);
                }
            }
        }
    }
}
