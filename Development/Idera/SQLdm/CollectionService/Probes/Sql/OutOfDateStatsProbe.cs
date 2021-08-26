//------------------------------------------------------------------------------
// <copyright file="OutOfDateStatsProbe.cs" company="Idera, Inc.">
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
    using Common.Services;
    using Common.Snapshots;

    /// <summary>
    /// //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) - New Probe class
    /// </summary>
    internal class OutOfDateStatsProbe : SqlBaseProbe
    {
        #region fields

        private OutOfDateStatsSnapshot snapshot = null;
        private string db;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OutOfDateStatsProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public OutOfDateStatsProbe(SqlConnectionInfo connectionInfo, string DB, int? cloudProviderId)
            : base(connectionInfo)
        {
            db = DB;
            snapshot = new OutOfDateStatsSnapshot(connectionInfo);
            LOG = Logger.GetLogger("SessionsProbe");
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
            StartOutOfDateStatsCollector();
        }

        /// <summary>
        /// Define the OutOfDateStats collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void OutOfDateStatsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildOutOfDateStatsCommand(conn, ver, db);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(OutOfDateStatsCallback));
        }

        /// <summary>
        /// Starts the Configuration collector.
        /// </summary>
        void StartOutOfDateStatsCollector()
        {
            StartGenericCollector(new Collector(OutOfDateStatsCollector), snapshot, "StartOutOfDateStatsCollector", "OutOfDateStats", OutOfDateStatsCallback, new object[] { });
        }

        /// <summary>
        /// Define the OutOfDateStats callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void OutOfDateStatsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretOutOfDateStats(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the OutOfDateStats collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void OutOfDateStatsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(OutOfDateStatsCallback), snapshot, "OutOfDateStatsCallback", "OutOfDateStats",
                            sender, e);
        }

        private void InterpretOutOfDateStats(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretOutOfDateStats"))
            {
                try
                {
                    snapshot.OutOfDateStatsInfo.Columns.Add("DatabaseName", typeof(string));
                    snapshot.OutOfDateStatsInfo.Columns.Add("Schema", typeof(string));
                    snapshot.OutOfDateStatsInfo.Columns.Add("Table", typeof(string));

                    snapshot.OutOfDateStatsInfo.Columns.Add("Name", typeof(string));
                    snapshot.OutOfDateStatsInfo.Columns.Add("RowCount", typeof(UInt64));
                    snapshot.OutOfDateStatsInfo.Columns.Add("ModCount", typeof(UInt64));

                    snapshot.OutOfDateStatsInfo.Columns.Add("StatsDate", typeof(DateTime));
                    snapshot.OutOfDateStatsInfo.Columns.Add("HoursSinceUpdated", typeof(UInt64));

                    while (datareader.Read())
                    {
                        snapshot.OutOfDateStatsInfo.Rows.Add(
                            ProbeHelpers.ToString(datareader, "Database"),
                            ProbeHelpers.ToString(datareader, "Schema"),
                            ProbeHelpers.ToString(datareader, "Table"),

                            ProbeHelpers.ToString(datareader, "Name"),
                            ProbeHelpers.ToInt64(datareader, "RowCount"),
                            ProbeHelpers.ToInt64(datareader, "ModCount"),

                            ProbeHelpers.ToDateTime(datareader, "StatsDate"),
                            ProbeHelpers.ToInt64(datareader, "HoursSinceUpdated")
                            );
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting OutOfDateStats Collector: {0}",
                                                        e,
                                                        false);
                    GenericFailureDelegate(snapshot);
                }
            }
        }


        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
