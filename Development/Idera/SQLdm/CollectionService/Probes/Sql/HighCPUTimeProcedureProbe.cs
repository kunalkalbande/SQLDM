//------------------------------------------------------------------------------
// <copyright file="HighCPUTimeProcedureProbe.cs" company="Idera, Inc.">
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
    /// //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-Q43 Adding new analyzer 
    /// </summary>
    internal class HighCPUTimeProcedureProbe : SqlBaseProbe
    {
        #region fields

        private HighCPUTimeProcedureSnapshot snapshot = null;
        private string db;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HighCPUTimeProcedureProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public HighCPUTimeProcedureProbe(SqlConnectionInfo connectionInfo, string DB, int? cloudProviderId)
            : base(connectionInfo)
        {
            db = DB;
            snapshot = new HighCPUTimeProcedureSnapshot(connectionInfo);
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
            StartHighCPUTimeProceduresCollector();
        }

        /// <summary>
        /// Define the HighCPUTimeProcedure collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void HighCPUTimeProceduresCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildHighCPUTimeProcedureCommand(conn, ver, db);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(HighCPUTimeProceduresCallback));
        }

        /// <summary>
        /// Starts the Configuration collector.
        /// </summary>
        void StartHighCPUTimeProceduresCollector()
        {
            StartGenericCollector(new Collector(HighCPUTimeProceduresCollector), snapshot, "StartHighCPUTimeProceduresCollector", "HighCPUTimeProcedures", HighCPUTimeProceduresCallback, new object[] { });
        }

        /// <summary>
        /// Define the HighCPUTimeProcedure callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void HighCPUTimeProceduresCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretHighCPUTimeProcedures(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the NonIncremental col stats collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void HighCPUTimeProceduresCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(HighCPUTimeProceduresCallback), snapshot, "HighCPUTimeProceduresCallback", "HighCPUTimeProcedures",
                            sender, e);
        }

        private void InterpretHighCPUTimeProcedures(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretHighCPUTimeProcedure"))
            {
                try
                {
                    snapshot.HighCPUTimeProcedureInfo.Columns.Add("dbname", typeof(string));
                    snapshot.HighCPUTimeProcedureInfo.Columns.Add("ProcedureName", typeof(string));

                    if(datareader.Read())
                    {
                        if (!datareader.IsDBNull(0))
                        {
                            snapshot.Edition = datareader.GetString(0);
                        }
                    }
                    if (snapshot.Edition.ToLower().Contains("enterprise"))
                    {
                        datareader.NextResult();
                        while (datareader.Read())
                        {
                            snapshot.HighCPUTimeProcedureInfo.Rows.Add(
                                ProbeHelpers.ToString(datareader, "dbname"),
                                ProbeHelpers.ToString(datareader, "ProcedureName")
                                );
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting HighCPUTimeProcedure Collector: {0}",
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
