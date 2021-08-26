//------------------------------------------------------------------------------
// <copyright file="NonIncrementalColumnStatProbe.cs" company="Idera, Inc.">
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
    /// //SQLDm 10.0  - Srishti Purohit - New Recommendations - SDR-I23 Adding new analyzer 
    /// </summary>
    internal class NonIncrementalColumnStatProbe : SqlBaseProbe
    {
        #region fields

        private NonIncrementalColumnStatSnapshot snapshot = null;
        private string db;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NonIncrementalColumnStatProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public NonIncrementalColumnStatProbe(SqlConnectionInfo connectionInfo, string DB, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            db = DB;
            snapshot = new NonIncrementalColumnStatSnapshot(connectionInfo);
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
            StartNonIncrementalColumnStatsCollector();
        }

        /// <summary>
        /// Define the NonIncrementalColumnStat collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void NonIncrementalColumnStatsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildNonIncrementalColumnStatsCommand(conn, ver, db);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(NonIncrementalColumnStatsCallback));
        }

        /// <summary>
        /// Starts the Configuration collector.
        /// </summary>
        void StartNonIncrementalColumnStatsCollector()
        {
            StartGenericCollector(new Collector(NonIncrementalColumnStatsCollector), snapshot, "StartNonIncrementalColumnStatsCollector", "NonIncrementalColumnStats", NonIncrementalColumnStatsCallback, new object[] { });
        }

        /// <summary>
        /// Define the NonIncrementalColumnStat callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void NonIncrementalColumnStatsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretNonIncrementalColumnStats(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the NonIncremental col stats collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void NonIncrementalColumnStatsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(NonIncrementalColumnStatsCallback), snapshot, "NonIncrementalColumnStatsCallback", "NonIncrementalColumnStats",
                            sender, e);
        }

        private void InterpretNonIncrementalColumnStats(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretNonIncrementalColumnStat"))
            {
                try
                {
                    snapshot.NonIncrementalColumnStatInfo.Columns.Add("Database", typeof(string));
                    snapshot.NonIncrementalColumnStatInfo.Columns.Add("TableName", typeof(string));

                    snapshot.NonIncrementalColumnStatInfo.Columns.Add("StateName", typeof(string));

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
                            snapshot.NonIncrementalColumnStatInfo.Rows.Add(
                                ProbeHelpers.ToString(datareader, "DatabaseName"),
                                ProbeHelpers.ToString(datareader, "TableName"),

                                ProbeHelpers.ToString(datareader, "StatsName")
                                );
                        }
                        //To provide support for optimize and undo feature of I23
                        DataTable ScriptsDT = new DataTable();
                        ScriptsDT.Columns.Add("TableName", typeof(string));
                        ScriptsDT.Columns.Add("StateName", typeof(string));
                        ScriptsDT.Columns.Add("OptiScript", typeof(string));
                        ScriptsDT.Columns.Add("UndoScript", typeof(string));
                        datareader.NextResult();
                        while (datareader.Read())
                        {
                            ScriptsDT.Rows.Add(
                                ProbeHelpers.ToString(datareader, "TableName"),

                                ProbeHelpers.ToString(datareader, "StatsName"),
                                ProbeHelpers.ToString(datareader, "DropCreateSQL"),
                                ProbeHelpers.ToString(datareader, "RollBackSQL")
                                );
                        }
                        snapshot.NonIncrementalColumnStatInfo.Columns.Add("OptiScript");
                        snapshot.NonIncrementalColumnStatInfo.Columns.Add("UndoScript");

                        for (int i = 0; i < snapshot.NonIncrementalColumnStatInfo.Rows.Count; i++)
                        {
                            DataRow[] dr = ScriptsDT.Select("TableName = '" + snapshot.NonIncrementalColumnStatInfo.Rows[i]["TableName"] + "' AND StateName = '" + snapshot.NonIncrementalColumnStatInfo.Rows[i]["StateName"] + "'");
                            snapshot.NonIncrementalColumnStatInfo.Rows[i]["OptiScript"] = dr[0]["OptiScript"];
                            snapshot.NonIncrementalColumnStatInfo.Rows[i]["UndoScript"] = dr[0]["UndoScript"];
                        }

                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting NonIncrementalColumnStat Collector: {0}",
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
