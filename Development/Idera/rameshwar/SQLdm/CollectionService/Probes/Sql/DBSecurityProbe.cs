//------------------------------------------------------------------------------
// <copyright file="DBSecurityProbe.cs" company="Idera, Inc.">
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
    internal class DBSecurityProbe : SqlBaseProbe
    {
        #region fields

        private DBSecuritySnapshot snapshot = null;
        private string db;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DBSecurityProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public DBSecurityProbe(SqlConnectionInfo connectionInfo,string DB, int? cloudProviderId)
            : base(connectionInfo)
        {
            db = DB;
            this.cloudProviderId = cloudProviderId;
            //To fix recomm generation problem with SDR-S7
            snapshot = new DBSecuritySnapshot(connectionInfo, db);
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
            StartDBSecurityCollector();
        }

        /// <summary>
        /// Define the DBSecurity collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void DBSecurityCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildDBSecurityCommand(conn, ver, db, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DBSecurityCallback));
        }

        /// <summary>
        /// Starts the Configuration collector.
        /// </summary>
        void StartDBSecurityCollector()
        {
            StartGenericCollector(new Collector(DBSecurityCollector), snapshot, "StartDBSecurityCollector", "DBSecurity", DBSecurityCallback, new object[] { });
        }

        /// <summary>
        /// Define the DBSecurity callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void DBSecurityCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretDBSecurity(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the DBSecurity collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void DBSecurityCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(DBSecurityCallback), snapshot, "DBSecurityCallback", "DBSecurity",
                            sender, e);
        }

        private void InterpretDBSecurity(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretDBSecurity"))
            {
                try
                {
                    snapshot.DBSecurity.Columns.Add("Name", typeof(string));
                    snapshot.DBSecurity.Columns.Add("Value", typeof(bool));
                    do
                    {
                        while (datareader.Read())
                        {
                            if (datareader.IsDBNull(0) || datareader[0] == null) continue;
                            snapshot.DBSecurity.Rows.Add(
                                datareader.GetString(0),
                                Convert.ToBoolean(datareader["value"])
                            );
                        }
                    }
                    while (datareader.NextResult());
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting DBSecurity Collector: {0}",
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
