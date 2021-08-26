//------------------------------------------------------------------------------
// <copyright file="ColumnStoreIndexProbe.cs" company="Idera, Inc.">
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
    internal class ColumnStoreIndexProbe : SqlBaseProbe
    {
        #region fields

        private ColumnStoreIndexSnapshot snapshot = null;
        private string db;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnStoreIndexProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public ColumnStoreIndexProbe(SqlConnectionInfo connectionInfo, string DB, int? cloudProviderId)
            : base(connectionInfo)
        {
            db = DB;
            snapshot = new ColumnStoreIndexSnapshot(connectionInfo);
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
            StartColumnStoreIndexCollector();
        }

        /// <summary>
        /// Define the ColumnStoreIndex collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void ColumnStoreIndexCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildColumnStoreIndexCommand(conn, ver, db, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ColumnStoreIndexCallback));
        }

        /// <summary>
        /// Starts the Configuration collector.
        /// </summary>
        void StartColumnStoreIndexCollector()
        {
            StartGenericCollector(new Collector(ColumnStoreIndexCollector), snapshot, "StartColumnStoreIndexCollector", "ColumnStoreIndex", ColumnStoreIndexCallback, new object[] { });
        }

        /// <summary>
        /// Define the ColumnStoreIndex callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void ColumnStoreIndexCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretColumnStoreIndex(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the ColumnStoreIndex collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void ColumnStoreIndexCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(ColumnStoreIndexCallback), snapshot, "ColumnStoreIndexCallback", "ColumnStoreIndex",
                            sender, e);
        }

        private void InterpretColumnStoreIndex(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretColumnStoreIndex"))
            {
                try
                {
                    snapshot.ColumnStoreIndexInfo.Columns.Add("DatabaseName", typeof(string));
                    snapshot.ColumnStoreIndexInfo.Columns.Add("Schema", typeof(string));
                    snapshot.ColumnStoreIndexInfo.Columns.Add("Table", typeof(string));
                    if (datareader.Read())
                    {
                        if (!datareader.IsDBNull(0))
                        {
                            snapshot.Edition = datareader.GetString(0);
                        }
                    }

                    if (snapshot.Edition.ToLower().Contains("enterprise"))
                    {
                        if (datareader.NextResult())
                        {
                            while (datareader.Read())
                            {
                                snapshot.ColumnStoreIndexInfo.Rows.Add(
                                    ProbeHelpers.ToString(datareader, "dbname"),
                                    ProbeHelpers.ToString(datareader, "SchemaName"),
                                    ProbeHelpers.ToString(datareader, "TableName")
                                    );
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting ColumnStoreIndex Collector: {0}",
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
