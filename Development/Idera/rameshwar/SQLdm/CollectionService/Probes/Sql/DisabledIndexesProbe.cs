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
    internal class DisabledIndexesProbe : SqlBaseProbe
    {
        #region fields

        private DisabledIndexesSnapshot snapshot = null;
        private string db;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DisabledIndexesProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public DisabledIndexesProbe(SqlConnectionInfo connectionInfo, string DB, int? cloudProviderId)
            : base(connectionInfo)
        {
            db = DB;
            this.cloudProviderId = cloudProviderId;
            snapshot = new DisabledIndexesSnapshot(connectionInfo);
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
            StartDisabledIndexesCollector();
        }

        /// <summary>
        /// Define the DisabledIndexes collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void DisabledIndexesCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildDisabledIndexesCommand(conn, ver, db);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DisabledIndexesCallback));
        }

        /// <summary>
        /// Starts the DisabledIndexes collector.
        /// </summary>
        void StartDisabledIndexesCollector()
        {
            StartGenericCollector(new Collector(DisabledIndexesCollector), snapshot, "StartDisabledIndexesCollector", "DisabledIndexes", DisabledIndexesCallback, new object[] { });
        }

        /// <summary>
        /// Define the DisabledIndexes callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void DisabledIndexesCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretDisabledIndexes(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the DisabledIndexes collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void DisabledIndexesCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(DisabledIndexesCallback), snapshot, "DisabledIndexesCallback", "DisabledIndexes",
                            sender, e);
        }

        private void InterpretDisabledIndexes(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretDisabledIndexes"))
            {
                try
                {
                    snapshot.DisabledIndexes.Columns.Add("DatabaseName", typeof(string));
                    snapshot.DisabledIndexes.Columns.Add("Schema", typeof(string));
                    snapshot.DisabledIndexes.Columns.Add("TableName", typeof(string));
                    snapshot.DisabledIndexes.Columns.Add("IndexName", typeof(string));
                    snapshot.DisabledIndexes.Columns.Add("index_id", typeof(long));
                    snapshot.DisabledIndexes.Columns.Add("is_disabled", typeof(bool));
                    snapshot.DisabledIndexes.Columns.Add("is_hypothetical", typeof(bool));

                    while (datareader.Read())
                    {
                        snapshot.DisabledIndexes.Rows.Add(
                            ProbeHelpers.ToString(datareader,"DatabaseName"),
                            ProbeHelpers.ToString(datareader, "Schema"),
                            ProbeHelpers.ToString(datareader,"TableName"),
                            ProbeHelpers.ToString(datareader,"IndexName"),
                            ProbeHelpers.ToInt64(datareader,"index_id"),
                            ProbeHelpers.ToBoolean(datareader,"is_disabled"),
                            ProbeHelpers.ToBoolean(datareader,"is_hypothetical")
                        );
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting DisabledIndexes Collector: {0}",
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
