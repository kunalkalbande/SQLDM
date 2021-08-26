//------------------------------------------------------------------------------
// <copyright file="QueryAnalyzerProbe.cs" company="Idera, Inc.">
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
    using System.Text;
    using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;

    /// <summary>
    /// //SQLdm 10.0 (Srishti Purohit) New Recommendation Q46,Q47,Q48,Q49,Q50 - New Probe class
    /// </summary>
    internal class QueryAnalyzerProbe : SqlBaseProbe
    {
        #region fields

        private QueryAnalyzerSnapshot snapshot = null;
        private string db;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryAnalyzerProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public QueryAnalyzerProbe(SqlConnectionInfo connectionInfo, string DB, int? cloudProviderId)
            : base(connectionInfo)
        {
            db = DB;
            snapshot = new QueryAnalyzerSnapshot(connectionInfo);
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
            StartQueryAnalyzerCollector();
        }

        /// <summary>
        /// Define the QueryAnalyzer collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void QueryAnalyzerCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildQueryAnalyzerCommand(conn, ver, db);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(QueryAnalyzerCallback));
        }

        /// <summary>
        /// Starts the Configuration collector.
        /// </summary>
        void StartQueryAnalyzerCollector()
        {
            StartGenericCollector(new Collector(QueryAnalyzerCollector), snapshot, "StartQueryAnalyzerCollector", "QueryAnalyzer", QueryAnalyzerCallback, new object[] { });
        }

        /// <summary>
        /// Define the QueryAnalyzer callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void QueryAnalyzerCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretQueryAnalyzer(rd);
            }
            FireCompletion(snapshot, Idera.SQLdm.Common.Services.Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the QueryAnalyzer collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void QueryAnalyzerCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(QueryAnalyzerCallback), snapshot, "QueryAnalyzerCallback", "QueryAnalyzer",
                            sender, e);
        }

        private void InterpretQueryAnalyzer(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretQueryAnalyzer"))
            {
                try
                {
                    if (datareader.Read())
                    {
                        if (!datareader.IsDBNull(0))
                        {
                            snapshot.Dbname = datareader.GetString(0);
                        }
                    }

                    datareader.NextResult();
                    while (datareader.Read())
                    {
                        snapshot.ActualState = ProbeHelpers.ToInt32(datareader, "actual_state");
                    }

                    if (snapshot.ActualState != 0)
                    {
                        snapshot.AffectedBatchesQ46 = new AffectedBatches();
                        datareader.NextResult();
                        int nameLength;
                        string name = "";
                        string batch = "";
                        while (datareader.Read())
                        {
                            batch = ProbeHelpers.ToString(datareader, "query_sql_text");
                            if (string.IsNullOrEmpty(batch)) { continue; }
                            if (batch.Length > 50) { nameLength = 50; } else { nameLength = batch.Length; }
                            name = batch.Substring(0, nameLength) + "....";
                            AffectedBatch ab = new AffectedBatch(name, batch);
                            snapshot.AffectedBatchesQ46.Add(ab);
                        }

                        snapshot.AffectedBatchesQ47 = new AffectedBatches();
                        datareader.NextResult();
                        while (datareader.Read())
                        {
                            batch = ProbeHelpers.ToString(datareader, "query_sql_text");
                            if (string.IsNullOrEmpty(batch)) { continue; }
                            if (batch.Length > 50) { nameLength = 50; } else { nameLength = batch.Length; }
                            name = batch.Substring(0, nameLength) + "....";
                            AffectedBatch ab = new AffectedBatch(name, batch);
                            snapshot.AffectedBatchesQ47.Add(ab);
                        }

                        snapshot.AffectedBatchesQ48 = new AffectedBatches();
                        datareader.NextResult();
                        while (datareader.Read())
                        {
                            batch = ProbeHelpers.ToString(datareader, "query_sql_text");
                            if (string.IsNullOrEmpty(batch)) { continue; }
                            if (batch.Length > 50) { nameLength = 50; } else { nameLength = batch.Length; }
                            name = batch.Substring(0, nameLength) + "....";
                            AffectedBatch ab = new AffectedBatch(name, batch);
                            snapshot.AffectedBatchesQ48.Add(ab);
                        }

                        snapshot.AffectedBatchesQ49 = new AffectedBatches();
                        datareader.NextResult();
                        while (datareader.Read())
                        {
                            batch = ProbeHelpers.ToString(datareader, "query_text");
                            if (string.IsNullOrEmpty(batch)) { continue; }
                            if (batch.Length > 50) { nameLength = 50; } else { nameLength = batch.Length; }
                            name = batch.Substring(0, nameLength) + "....";
                            AffectedBatch ab = new AffectedBatch(name, batch);
                            snapshot.AffectedBatchesQ49.Add(ab);
                        }

                        snapshot.AffectedBatchesQ50 = new AffectedBatches();
                        datareader.NextResult();
                        while (datareader.Read())
                        {
                            batch = ProbeHelpers.ToString(datareader, "SQLText");
                            if (string.IsNullOrEmpty(batch)) { continue; }
                            if (batch.Length > 50) { nameLength = 50; } else { nameLength = batch.Length; }
                            name = batch.Substring(0, nameLength) + "....";
                            AffectedBatch ab = new AffectedBatch(name, batch);
                            snapshot.AffectedBatchesQ50.Add(ab);
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting QueryAnalyzer Collector: {0}",
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
