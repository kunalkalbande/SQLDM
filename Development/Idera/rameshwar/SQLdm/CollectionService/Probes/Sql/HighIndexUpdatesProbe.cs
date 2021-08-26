//------------------------------------------------------------------------------
// <copyright file="HighIndexUpdatesProbe.cs" company="Idera, Inc.">
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
    internal class HighIndexUpdatesProbe : SqlBaseProbe
    {
        #region fields

        private HighIndexUpdatesSnapshot snapshot = null;
        private string db;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HighIndexUpdatesProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public HighIndexUpdatesProbe(SqlConnectionInfo connectionInfo, string DB, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            db = DB;
            snapshot = new HighIndexUpdatesSnapshot(connectionInfo);
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
            StartHighIndexUpdatesCollector();
        }

        /// <summary>
        /// Define the HighIndexUpdates collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void HighIndexUpdatesCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildHighIndexUpdatesCommand(conn, ver, db);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(HighIndexUpdatesCallback));
        }

        /// <summary>
        /// Starts the HighIndexUpdates collector.
        /// </summary>
        void StartHighIndexUpdatesCollector()
        {
            StartGenericCollector(new Collector(HighIndexUpdatesCollector), snapshot, "StartHighIndexUpdatesCollector", "HighIndexUpdates", HighIndexUpdatesCallback, new object[] { });
        }

        /// <summary>
        /// Define the HighIndexUpdates callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void HighIndexUpdatesCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretHighIndexUpdates(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the HighIndexUpdates collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void HighIndexUpdatesCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(HighIndexUpdatesCallback), snapshot, "HighIndexUpdatesCallback", "HighIndexUpdates",
                            sender, e);
        }

        private void InterpretHighIndexUpdates(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretHighIndexUpdates"))
            {
                try
                {
                    snapshot.HighIndexUpdates.Columns.Add("DatabaseName", typeof(string));
                    snapshot.HighIndexUpdates.Columns.Add("Schema", typeof(string));

                    snapshot.HighIndexUpdates.Columns.Add("TableName", typeof(string));
                    snapshot.HighIndexUpdates.Columns.Add("IndexName", typeof(string));

                    snapshot.HighIndexUpdates.Columns.Add("UserReads", typeof(long));
                    snapshot.HighIndexUpdates.Columns.Add("UserWrites", typeof(long));

                    snapshot.HighIndexUpdates.Columns.Add("WritesPerRead", typeof(decimal));
                    snapshot.HighIndexUpdates.Columns.Add("DaysSinceTableCreated", typeof(long));

                    while (datareader.Read())
                    {
                        snapshot.HighIndexUpdates.Rows.Add(
                            ProbeHelpers.ToString(datareader,"DatabaseName"),
                            ProbeHelpers.ToString(datareader, "Schema"),

                            ProbeHelpers.ToString(datareader,"TableName"),
                            ProbeHelpers.ToString(datareader,"IndexName"),

                            ProbeHelpers.ToInt64(datareader,"UserReads"),
                            ProbeHelpers.ToInt64(datareader,"UserWrites"),

                            ProbeHelpers.ToDecimal(datareader,"WritesPerRead"),
                            ProbeHelpers.ToInt64(datareader,"DaysSinceTableCreated")
                        );
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting HighIndexUpdates Collector: {0}",
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
