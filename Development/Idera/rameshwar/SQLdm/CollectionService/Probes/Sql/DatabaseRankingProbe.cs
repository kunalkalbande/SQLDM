//------------------------------------------------------------------------------
// <copyright file="DisabledIndexesProbe.cs" company="Idera, Inc.">
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
    internal class DatabaseRankingProbe : SqlBaseProbe
    {
        #region fields

        private DatabaseRankingSnapshot snapshot = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseRankingProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public DatabaseRankingProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
            : base(connectionInfo)
        {
            snapshot = new DatabaseRankingSnapshot(connectionInfo);
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
            StartDatabaseRankingCollector();
        }

        /// <summary>
        /// Define the DatabaseRanking collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void DatabaseRankingCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildDatabaseRankingCommand(conn, ver);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DatabaseRankingCallback));
        }

        /// <summary>
        /// Starts the DatabaseRanking collector.
        /// </summary>
        void StartDatabaseRankingCollector()
        {
            StartGenericCollector(new Collector(DatabaseRankingCollector), snapshot, "StartDatabaseRankingCollector", "DatabaseRanking", DatabaseRankingCallback, new object[] { });
        }

        /// <summary>
        /// Define the DatabaseRanking callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void DatabaseRankingCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretDatabaseRanking(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the DatabaseRanking collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void DatabaseRankingCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(DatabaseRankingCallback), snapshot, "DatabaseRankingCallback", "DatabaseRanking",
                            sender, e);
        }

        private void InterpretDatabaseRanking(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretDisabledIndexes"))
            {
                try
                {
                    snapshot.DatabaseRanks.Columns.Add("Database", typeof(string));
                    snapshot.DatabaseRanks.Columns.Add("IO", typeof(string));

                    while (datareader.Read())
                    {
                        snapshot.DatabaseRanks.Rows.Add(
                            ProbeHelpers.ToString(datareader, "Database"),
                            ProbeHelpers.ToInt64(datareader, "IO")
                        );
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting DatabaseRanking Collector: {0}",
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
