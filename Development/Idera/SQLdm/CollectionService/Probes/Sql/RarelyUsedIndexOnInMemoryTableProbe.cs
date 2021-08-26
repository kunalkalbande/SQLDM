//------------------------------------------------------------------------------
// <copyright file="RarelyUsedIndexOnInMemoryTableProbe.cs" company="Idera, Inc.">
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
    /// //SQLdm 10.0 (srishti purohit) (Embedded SQLDoctor Analytics) - New Probe class
    /// </summary>
    internal class RarelyUsedIndexOnInMemoryTableProbe : SqlBaseProbe
    {
        #region fields

        private RarelyUsedIndexOnInMemoryTableSnapshot snapshot = null;
        private string db;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RarelyUsedIndexOnInMemoryTableProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public RarelyUsedIndexOnInMemoryTableProbe(SqlConnectionInfo connectionInfo, string DB, int? cloudProviderId)
            : base(connectionInfo)
        {
            db = DB;
            snapshot = new RarelyUsedIndexOnInMemoryTableSnapshot(connectionInfo);
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
            StartRarelyUsedIndexOnInMemoryTableCollector();
        }

        /// <summary>
        /// Define the RarelyUsedIndexOnInMemoryTable collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void RarelyUsedIndexOnInMemoryTableCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildRarelyUsedIndexOnInMemoryTableCommand(conn, ver, db);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(RarelyUsedIndexOnInMemoryTableCallback));
        }

        /// <summary>
        /// Starts the Configuration collector.
        /// </summary>
        void StartRarelyUsedIndexOnInMemoryTableCollector()
        {
            StartGenericCollector(new Collector(RarelyUsedIndexOnInMemoryTableCollector), snapshot, "StartRarelyUsedIndexOnInMemoryTableCollector", "RarelyUsedIndexOnInMemoryTable", RarelyUsedIndexOnInMemoryTableCallback, new object[] { });
        }

        /// <summary>
        /// Define the RarelyUsedIndexOnInMemoryTable callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void RarelyUsedIndexOnInMemoryTableCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretRarelyUsedIndexOnInMemoryTable(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the RarelyUsedIndexOnInMemoryTable collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void RarelyUsedIndexOnInMemoryTableCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(RarelyUsedIndexOnInMemoryTableCallback), snapshot, "RarelyUsedIndexOnInMemoryTableCallback", "RarelyUsedIndexOnInMemoryTable",
                            sender, e);
        }

        private void InterpretRarelyUsedIndexOnInMemoryTable(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretRarelyUsedIndexOnInMemoryTable"))
            {
                try
                {
                    snapshot.RarelyUsedIndexOnInMemoryTableInfo.Columns.Add("dbname", typeof(string));
                    snapshot.RarelyUsedIndexOnInMemoryTableInfo.Columns.Add("SchemaName", typeof(string));
                    snapshot.RarelyUsedIndexOnInMemoryTableInfo.Columns.Add("TableName", typeof(string));

                    snapshot.RarelyUsedIndexOnInMemoryTableInfo.Columns.Add("IndexName", typeof(string));
                    snapshot.RarelyUsedIndexOnInMemoryTableInfo.Columns.Add("rows_returned", typeof(long));
                    snapshot.RarelyUsedIndexOnInMemoryTableInfo.Columns.Add("scans_started", typeof(long));

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
                            snapshot.RarelyUsedIndexOnInMemoryTableInfo.Rows.Add(
                                ProbeHelpers.ToString(datareader, "dbname"),
                                ProbeHelpers.ToString(datareader, "SchemaName"),
                                ProbeHelpers.ToString(datareader, "TableName"),

                                ProbeHelpers.ToString(datareader, "IndexName"),
                                ProbeHelpers.ToInt64(datareader, "rows_returned"),
                                ProbeHelpers.ToInt64(datareader, "scans_started")

                                );
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting RarelyUsedIndexOnInMemoryTable Collector: {0}",
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
