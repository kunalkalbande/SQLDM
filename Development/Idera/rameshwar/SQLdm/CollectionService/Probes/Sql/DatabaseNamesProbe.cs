//------------------------------------------------------------------------------
// <copyright file="DatabaseNamesProbe.cs" company="Idera, Inc.">
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
    internal class DatabaseNamesProbe : SqlBaseProbe
    {
        #region fields

        private DatabaseNamesSnapshot snapshot = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the  class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public DatabaseNamesProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            snapshot = new DatabaseNamesSnapshot(connectionInfo);
            LOG = Logger.GetLogger("DatabaseNamesProbe");
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
            StartDatabaseNamesCollector();
        }

        /// <summary>
        /// Define the DatabaseNames collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void DatabaseNamesCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildDatabaseNamesCommand(conn, ver, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DatabaseNamesCallback));
        }

        /// <summary>
        /// Starts the DatabaseNames collector.
        /// </summary>
        void StartDatabaseNamesCollector()
        {
            StartGenericCollector(new Collector(DatabaseNamesCollector), snapshot, "StartDatabaseNamesCollector", "DatabaseNames", null, new object[] { });
        }

        /// <summary>
        /// Define the DatabaseNames callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void DatabaseNamesCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretDatabaseNames(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the DatabaseNames collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void DatabaseNamesCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(DatabaseNamesCallback), snapshot, "DatabaseNamesCallback", "DatabaseNames",
                            sender, e);
        }

        private void InterpretDatabaseNames(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretDatabaseNames"))
            {
                try
                {
                    snapshot.Databases = new System.Collections.Generic.Dictionary<int, string>();

                    while (datareader.Read())
                    {
                        snapshot.Databases.Add(ProbeHelpers.ToInt32(datareader, "ID"), ProbeHelpers.ToString(datareader, "DatabaseName"));
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting DatabaseNames Collector: {0}",
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
