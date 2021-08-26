//------------------------------------------------------------------------------
// <copyright file="LargeTableStatsProbe.cs" company="Idera, Inc.">
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
    /// //SQLdm 10.0 (srishti purohit) (new recommendation -I30) - New Probe class
    /// </summary>
    internal class LargeTableStatsProbe : SqlBaseProbe
    {
        #region fields

        private LargeTableStatsSnapshot snapshot = null;
        private string db;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LargeTableStatsProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public LargeTableStatsProbe(SqlConnectionInfo connectionInfo, string DB, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            db = DB;
            snapshot = new LargeTableStatsSnapshot(connectionInfo);
            LOG = Logger.GetLogger("SessionsProbe");
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
            StartLargeTableStatsCollector();
        }

        /// <summary>
        /// Define the LargeTableStats collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void LargeTableStatsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildLargeTableStatsCommand(conn, ver, db);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(LargeTableStatsCallback));
        }

        /// <summary>
        /// Starts the Configuration collector.
        /// </summary>
        void StartLargeTableStatsCollector()
        {
            StartGenericCollector(new Collector(LargeTableStatsCollector), snapshot, "StartLargeTableStatsCollector", "LargeTableStats", LargeTableStatsCallback, new object[] { });
        }

        /// <summary>
        /// Define the LargeTableStats callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void LargeTableStatsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretLargeTableStats(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the LargeTableStats collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void LargeTableStatsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(LargeTableStatsCallback), snapshot, "LargeTableStatsCallback", "LargeTableStats",
                            sender, e);
        }

        private void InterpretLargeTableStats(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretLargeTableStats"))
            {
                try
                {
                    snapshot.LargeTableStatsInfo.Columns.Add("DatabaseName", typeof(string));
                    snapshot.LargeTableStatsInfo.Columns.Add("Schema", typeof(string));
                    snapshot.LargeTableStatsInfo.Columns.Add("Table", typeof(string));

                    while (datareader.Read())
                    {
                        snapshot.LargeTableStatsInfo.Rows.Add(
                            ProbeHelpers.ToString(datareader, "dbname"),
                            ProbeHelpers.ToString(datareader, "SchemaName"),
                            ProbeHelpers.ToString(datareader, "TableName")
                            );
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting LargeTableStats Collector: {0}",
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
