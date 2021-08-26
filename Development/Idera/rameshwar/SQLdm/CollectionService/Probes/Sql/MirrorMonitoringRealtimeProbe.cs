//------------------------------------------------------------------------------
// <copyright file="DatabaseMirroringRealtimeProbe.cs" company="Idera, Inc.">
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
    internal sealed class MirrorMonitoringRealtimeProbe : SqlBaseProbe
    {
        #region fields

        private MirrorMonitoringRealtimeSnapshot mirroringSnapshot = null;
        private MirrorMonitoringRealtimeConfiguration config = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of DatabaseMirroringRealtimeProbe
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <param name="config"></param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public MirrorMonitoringRealtimeProbe(SqlConnectionInfo connectionInfo, MirrorMonitoringRealtimeConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("MirrorMonitoringRealtimeProbe");
            mirroringSnapshot = new MirrorMonitoringRealtimeSnapshot(connectionInfo);
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
                StartMirrorMonitoringRealtimeCollector();
            }
            else
            {
                FireCompletion(mirroringSnapshot, Result.Success);
            }
        }


        /// <summary>
        /// Define the MirrorMonitoringRealtime collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void MirrorMonitoringRealtimeCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildMirrorMonitoringRealtimeCommand(conn, ver);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(MirrorMonitoringRealtimeCallback));
        }

        /// <summary>
        /// Starts the Mirror Monitoring Real-time collector.
        /// </summary>
        private void StartMirrorMonitoringRealtimeCollector()
        {
            StartGenericCollector(new Collector(MirrorMonitoringRealtimeCollector), mirroringSnapshot, "StartMirrorMonitoringRealtimeCollector", "Mirror Monitoring Real-time", MirrorMonitoringRealtimeCallback, new object[] { });
        }

        /// <summary>
        /// Define the MirrorMonitoringRealtime callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void MirrorMonitoringRealtimeCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretMirrorMonitoringRealtime(rd);
            }
            FireCompletion(mirroringSnapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the MirrorMonitoringRealtime collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void MirrorMonitoringRealtimeCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(MirrorMonitoringRealtimeCallback), mirroringSnapshot, "MirrorMonitoringRealtimeCallback", "Mirror Monitoring Real-time",
                            sender, e);
        }

        /// <summary>
        /// Interpret MirrorMonitoringRealtime data
        /// </summary>
        private void InterpretMirrorMonitoringRealtime(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretMirrorMonitoringRealtime"))
            {
                try
                {
                    string dbName = null;
                    string instance = null;
                    bool resultOne = false;
                    bool resultTwo = false;

                    MirrorMonitoringDatabaseDetail detail = null;
                    resultOne = resultTwo = false;

                    do
                    {
                        if (dataReader.Read())
                        {
                            if (dataReader.GetString(0).Equals("endMirrorMonitoring")) break;
                            
                            resultOne = true;

                            if (dataReader.FieldCount == 10)
                            {
                                if (!dataReader.IsDBNull(0)) dbName = dataReader.GetString(0).TrimEnd();
                                if (!dataReader.IsDBNull(1)) instance = dataReader.GetString(1).TrimEnd();
                                detail = new MirrorMonitoringDatabaseDetail(instance, dbName);
                                if (!dataReader.IsDBNull(2)) detail.Partner = dataReader.GetString(2);
                                if (!dataReader.IsDBNull(3)) detail.CurrentMirroringMetrics.MirroringState = (MirroringMetrics.MirroringStateEnum)dataReader.GetInt32(3);
                                if (!dataReader.IsDBNull(4)) detail.CurrentMirroringMetrics.WitnessStatus = (MirroringMetrics.WitnessStatusEnum)dataReader.GetInt32(4);
                                if (!dataReader.IsDBNull(5)) detail.MirroringGuid = new Guid(dataReader.GetSqlGuid(5).ToByteArray());
                                if (!dataReader.IsDBNull(6)) detail.SafetyLevel = (MirrorMonitoringDatabaseDetail.SafetyLevelEnum)dataReader.GetInt32(6);
                                if (!dataReader.IsDBNull(7)) detail.WitnessAddress = dataReader.GetString(7);
                                if (!dataReader.IsDBNull(8)) detail.PartnerAddress = dataReader.GetString(8);

                                if (!dataReader.IsDBNull(9)) detail.CurrentMirroringMetrics.Role = (MirroringMetrics.MirroringRoleEnum) dataReader.GetInt32(9);
                            }
                        }
                        if (dataReader.NextResult())
                        {
                            //read the next row of the resultset
                            if (!dataReader.Read()) break;

                            if (dataReader.GetString(0).Equals("endMirrorMonitoring")) break;

                            resultTwo = true;

                            if (detail != null)
                            {
                                if (dataReader.FieldCount == 15)
                                {
                                    //if (!dataReader.IsDBNull(1)) detail.CurrentMirroringMetrics.Role = (MirroringMetrics.MirroringRoleEnum)dataReader.GetByte(1);
                                    //if (!dataReader.IsDBNull(2)) detail.CurrentMirroringMetrics.MirroringState = (MirroringMetrics.MirroringStateEnum)dataReader.GetByte(2);
                                    //if (!dataReader.IsDBNull(3)) detail.CurrentMirroringMetrics.WitnessStatus = (MirroringMetrics.WitnessStatusEnum)dataReader.GetByte(3);
                                    if (!dataReader.IsDBNull(4)) detail.CurrentMirroringMetrics.LogGenerationRate = dataReader.GetInt32(4);
                                    if (!dataReader.IsDBNull(5)) detail.CurrentMirroringMetrics.UnsentLog = dataReader.GetInt32(5);
                                    if (!dataReader.IsDBNull(6)) detail.CurrentMirroringMetrics.SendRate = dataReader.GetInt32(6);
                                    if (!dataReader.IsDBNull(7)) detail.CurrentMirroringMetrics.UnrestoredLog = dataReader.GetInt32(7);
                                    if (!dataReader.IsDBNull(8)) detail.CurrentMirroringMetrics.RecoveryRate = dataReader.GetInt32(8);
                                    if (!dataReader.IsDBNull(9)) detail.CurrentMirroringMetrics.TransactionDelay = dataReader.GetInt32(9);
                                    if (!dataReader.IsDBNull(10)) detail.CurrentMirroringMetrics.TransactionsPerSec = dataReader.GetInt32(10);
                                    if (!dataReader.IsDBNull(11)) detail.CurrentMirroringMetrics.AverageDelay = dataReader.GetInt32(11);
                                    if (!dataReader.IsDBNull(12)) detail.CurrentMirroringMetrics.TimeRecorded = dataReader.GetDateTime(12);
                                    if (!dataReader.IsDBNull(13)) detail.CurrentMirroringMetrics.TimeBehind = dataReader.GetDateTime(13);
                                    if (!dataReader.IsDBNull(14)) detail.CurrentMirroringMetrics.LocalTime = dataReader.GetDateTime(14);
                                }
                            }
                        } else break;

                        if (dbName != null && resultOne && resultTwo)
                        {
                            mirroringSnapshot.Databases.Add(dbName, detail);
                        }

                    } while (dataReader.NextResult());
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(mirroringSnapshot, LOG, "Error interpreting Mirror Monitoring Real-time Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(mirroringSnapshot);
                }
            }
        }
        #endregion

        #region interface implementations

        #endregion
    }
}
