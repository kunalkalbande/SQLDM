//------------------------------------------------------------------------------
// <copyright file="SQLModuleOptionsProbe.cs" company="Idera, Inc.">
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
    internal class SQLModuleOptionsProbe : SqlBaseProbe
    {
        #region fields

        private SQLModuleOptionsSnapshot snapshot = null;
        private string db;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLModuleOptionsProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public SQLModuleOptionsProbe(SqlConnectionInfo connectionInfo, string DB, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            db = DB;
            snapshot = new SQLModuleOptionsSnapshot(connectionInfo);
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
            StartSQLModuleOptionsCollector();
        }

        /// <summary>
        /// Define the SQLModuleOptions collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void SQLModuleOptionsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildSQLModuleOptionsCommand(conn, ver, db);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(SQLModuleOptionsCallback));
        }

        /// <summary>
        /// Starts the SQLModuleOptions collector.
        /// </summary>
        void StartSQLModuleOptionsCollector()
        {
            StartGenericCollector(new Collector(SQLModuleOptionsCollector), snapshot, "StartSQLModuleOptionsCollector", "SQLModuleOptions", SQLModuleOptionsCallback, new object[] { });
        }

        /// <summary>
        /// Define the SQLModuleOptions callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void SQLModuleOptionsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretSQLModuleOptions(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the SQLModuleOptions collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void SQLModuleOptionsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(SQLModuleOptionsCallback), snapshot, "SQLModuleOptionsCallback", "SQLModuleOptions",
                            sender, e);
        }

        private void InterpretSQLModuleOptions(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretSQLModuleOptions"))
            {
                try
                {
                    snapshot.SQLModuleOptions.Columns.Add("DatabaseName", typeof(string));
                    snapshot.SQLModuleOptions.Columns.Add("Schema", typeof(string));
                    snapshot.SQLModuleOptions.Columns.Add("ObjectName", typeof(string));

                    snapshot.SQLModuleOptions.Columns.Add("ObjectType", typeof(string));
                    snapshot.SQLModuleOptions.Columns.Add("SQL", typeof(string));

                    snapshot.SQLModuleOptions.Columns.Add("uses_ansi_nulls", typeof(bool));
                    snapshot.SQLModuleOptions.Columns.Add("uses_quoted_identifier", typeof(bool));

                    while (datareader.Read())
                    {
                        snapshot.SQLModuleOptions.Rows.Add(
                            ProbeHelpers.ToString(datareader,"DatabaseName"),
                            ProbeHelpers.ToString(datareader, "Schema"),
                            ProbeHelpers.ToString(datareader,"ObjectName"),

                            ProbeHelpers.ToString(datareader,"ObjectType"),
                            ProbeHelpers.ToString(datareader,"SQL"),

                            ProbeHelpers.ToBoolean(datareader,"uses_ansi_nulls"),
                            ProbeHelpers.ToBoolean(datareader,"uses_quoted_identifier")
                        );
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting SQLModuleOptions Collector: {0}",
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
