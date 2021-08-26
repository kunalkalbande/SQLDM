//------------------------------------------------------------------------------
// <copyright file="QueryStoreProbe.cs" company="Idera, Inc.">
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
    /// //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I23 Adding new analyzer 
    /// </summary>
    internal class QueryStoreProbe : SqlBaseProbe
    {
        #region fields

        private QueryStoreSnapshot snapshot = null;
        private string db;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryStoreProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public QueryStoreProbe(SqlConnectionInfo connectionInfo, string DB, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            db = DB;
            snapshot = new QueryStoreSnapshot(connectionInfo);
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
            StartQueryStoreCollector();
        }

        /// <summary>
        /// Define the QueryStore collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void QueryStoreCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildQueryStoreCommand(conn, ver, db);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(QueryStoreCallback));
        }

        /// <summary>
        /// Starts the Configuration collector.
        /// </summary>
        void StartQueryStoreCollector()
        {
            StartGenericCollector(new Collector(QueryStoreCollector), snapshot, "StartQueryStoreCollector", "QueryStore", QueryStoreCallback, new object[] { });
        }

        /// <summary>
        /// Define the QueryStore callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void QueryStoreCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretQueryStore(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the QueryStore collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void QueryStoreCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(QueryStoreCallback), snapshot, "QueryStoreCallback", "QueryStore",
                            sender, e);
        }

        private void InterpretQueryStore(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretQueryStore"))
            {
                try
                {
                    snapshot.QueryStoreInfo.Columns.Add("actual_state", typeof(Int32));

                    snapshot.QueryStoreInfo.Columns.Add("readonly_reason", typeof(Int32));
                    
                    snapshot.QueryStoreInfo.Columns.Add("remaining_space", typeof(double));

                    if(datareader.Read())
                    {
                        if (!datareader.IsDBNull(0))
                        {
                            snapshot.DbName = datareader.GetString(0);
                        }
                    }
                    if (!string.IsNullOrEmpty(snapshot.DbName))
                    {
                        datareader.NextResult();
                        while (datareader.Read())
                        {
                            snapshot.QueryStoreInfo.Rows.Add(
                                ProbeHelpers.ToInt64(datareader, "actual_state"),

                                ProbeHelpers.ToInt64(datareader, "readonly_reason"),
                                ProbeHelpers.ToDouble(datareader, "remaining_space")
                                );
                        }
                        datareader.NextResult();
                        while (datareader.Read())
                        {
                            if (!datareader.IsDBNull(0))
                            {
                                snapshot.PlanName.Add(datareader.GetString(0));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting QueryStore Collector: {0}",
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
