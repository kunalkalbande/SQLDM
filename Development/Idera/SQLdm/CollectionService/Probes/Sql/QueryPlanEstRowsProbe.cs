//------------------------------------------------------------------------------
// <copyright file="QueryPlanEstRowsProbe.cs" company="Idera, Inc.">
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
    using System.Data;

    /// <summary>
    /// //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) - New Probe class
    /// </summary>
    internal class QueryPlanEstRowsProbe : SqlBaseProbe
    {
        #region fields

        private QueryPlanEstRowsSnapshot snapshot = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryPlanEstRowsProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public QueryPlanEstRowsProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
            : base(connectionInfo)
        {
            snapshot = new QueryPlanEstRowsSnapshot(connectionInfo);
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
            StartQueryPlanEstRowsCollector();
        }

        /// <summary>
        /// Define the QueryPlanEstRows collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void QueryPlanEstRowsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildQueryPlanEstRowsCommand(conn, ver, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(QueryPlanEstRowsCallback));
        }

        /// <summary>
        /// Starts the QueryPlanEstRows collector.
        /// </summary>
        void StartQueryPlanEstRowsCollector()
        {
            StartGenericCollector(new Collector(QueryPlanEstRowsCollector), snapshot, "StartQueryPlanEstRowsCollector", "QueryPlanEstRows", QueryPlanEstRowsCallback, new object[] { });
        }

        /// <summary>
        /// Define the QueryPlanEstRows callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void QueryPlanEstRowsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretQueryPlanEstRows(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the QueryPlanEstRows collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void QueryPlanEstRowsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(QueryPlanEstRowsCallback), snapshot, "QueryPlanEstRowsCallback", "QueryPlanEstRows",
                            sender, e);
        }

        private void InterpretQueryPlanEstRows(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretQueryPlanEstRows"))
            {
                try
                {
                    snapshot.QueryPlanEstRows.Columns.Add("Text", typeof(string));
                    snapshot.QueryPlanEstRows.Columns.Add("EstRows", typeof(double));

                    while (datareader.Read())
                    {
                        snapshot.QueryPlanEstRows.Rows.Add(
                            ProbeHelpers.ToString(datareader, "Text"), 
                            ProbeHelpers.ToDouble(datareader, "EstRows")
                            );
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting QueryPlanEstRows Collector: {0}",
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
