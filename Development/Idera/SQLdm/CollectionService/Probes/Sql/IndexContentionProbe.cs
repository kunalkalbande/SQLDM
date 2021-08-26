//------------------------------------------------------------------------------
// <copyright file="IndexContentionProbe.cs" company="Idera, Inc.">
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
    internal class IndexContentionProbe : SqlBaseProbe
    {
        #region fields

        private IndexContentionSnapshot snapshot = null;
        private string db;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexContentionProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public IndexContentionProbe(SqlConnectionInfo connectionInfo, string DB, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            db = DB; 
            snapshot = new IndexContentionSnapshot(connectionInfo);
            LOG = Logger.GetLogger("SessionsProbe");
            //reconfig = configuration;
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
            StartIndexContentionCollector();
        }

        /// <summary>
        /// Define the IndexContention collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void IndexContentionCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildIndexContentionCommand(conn, ver, db);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(IndexContentionCallback));
        }

        /// <summary>
        /// Starts the IndexContention collector.
        /// </summary>
        void StartIndexContentionCollector()
        {
            StartGenericCollector(new Collector(IndexContentionCollector), snapshot, "StartIndexContentionCollector", "IndexContention", null, new object[] { });
        }

        /// <summary>
        /// Define the IndexContention callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void IndexContentionCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretIndexContention(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the IndexContention collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void IndexContentionCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(IndexContentionCallback), snapshot, "IndexContentionCallback", "IndexContention",
                            sender, e);
        }

        private void InterpretIndexContention(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretIndexContention"))
            {
                try
                {
                    snapshot.PageLatchIndexContention.Columns.Add("DatabaseName", typeof(string));
                    snapshot.PageLatchIndexContention.Columns.Add("Schema", typeof(string));
                    snapshot.PageLatchIndexContention.Columns.Add("TableName", typeof(string));
                    snapshot.PageLatchIndexContention.Columns.Add("IndexName", typeof(string));
                    snapshot.PageLatchIndexContention.Columns.Add("Partition", typeof(int));
                    snapshot.PageLatchIndexContention.Columns.Add("PageLatchWaitCount", typeof(System.Int64));
                    snapshot.PageLatchIndexContention.Columns.Add("PageLatchWaitInMs", typeof(System.Int64));
                    snapshot.PageLatchIndexContention.Columns.Add("AvgPageLatchWaitInMs", typeof(System.Decimal));

                    while (datareader.Read())
                    {
                        snapshot.PageLatchIndexContention.Rows.Add(
                            ProbeHelpers.ToString(datareader,"DatabaseName"),
                            ProbeHelpers.ToString(datareader, "Schema"),
                            ProbeHelpers.ToString(datareader,"TableName"),
                            ProbeHelpers.ToString(datareader,"IndexName"),
                            ProbeHelpers.ToInt32(datareader,"Partition"),
                            ProbeHelpers.ToInt64(datareader,"PageLatchWaitCount"),
                            ProbeHelpers.ToInt64(datareader,"PageLatchWaitInMs"),
                            ProbeHelpers.ToDecimal(datareader,"AvgPageLatchWaitInMs")
                        );
                    }

                    snapshot.PageLockIndexContention.Columns.Add("DatabaseName", typeof(string));
                    snapshot.PageLockIndexContention.Columns.Add("Schema", typeof(string));
                    snapshot.PageLockIndexContention.Columns.Add("TableName", typeof(string));
                    snapshot.PageLockIndexContention.Columns.Add("IndexName", typeof(string));
                    snapshot.PageLockIndexContention.Columns.Add("Partition", typeof(int));
                    snapshot.PageLockIndexContention.Columns.Add("PageLockCount", typeof(System.Int64));
                    snapshot.PageLockIndexContention.Columns.Add("PageLockWaitCount", typeof(System.Int64));
                    snapshot.PageLockIndexContention.Columns.Add("PageLockPercent", typeof(System.Decimal));
                    snapshot.PageLockIndexContention.Columns.Add("PageLockWaitInMs", typeof(System.Int64));
                    snapshot.PageLockIndexContention.Columns.Add("AvgPageLockWaitInMs", typeof(System.Decimal));


                    if (datareader.NextResult())
                    {
                        while (datareader.Read())
                        {
                            snapshot.PageLockIndexContention.Rows.Add(
                            ProbeHelpers.ToString(datareader,"DatabaseName"),
                            ProbeHelpers.ToString(datareader, "Schema"),
                            ProbeHelpers.ToString(datareader,"TableName"),
                            ProbeHelpers.ToString(datareader,"IndexName"),
                            ProbeHelpers.ToInt32(datareader,"Partition"),
                            ProbeHelpers.ToInt64(datareader,"PageLockCount"),
                            ProbeHelpers.ToInt64(datareader,"PageLockWaitCount"),
                            ProbeHelpers.ToDecimal(datareader,"PageLockPercent"),
                            ProbeHelpers.ToInt64(datareader,"PageLockWaitInMs"),
                            ProbeHelpers.ToDecimal(datareader,"AvgPageLockWaitInMs")
                        );
                        }
                    }

                    snapshot.RowLockIndexContention.Columns.Add("DatabaseName", typeof(string));
                    snapshot.RowLockIndexContention.Columns.Add("Schema", typeof(string));
                    snapshot.RowLockIndexContention.Columns.Add("TableName", typeof(string));
                    snapshot.RowLockIndexContention.Columns.Add("IndexName", typeof(string));
                    snapshot.RowLockIndexContention.Columns.Add("Partition", typeof(int));
                    snapshot.RowLockIndexContention.Columns.Add("RowLockCount", typeof(System.Int64));
                    snapshot.RowLockIndexContention.Columns.Add("RowLockWaitCount", typeof(System.Int64));
                    snapshot.RowLockIndexContention.Columns.Add("RowLockPercent", typeof(System.Decimal));
                    snapshot.RowLockIndexContention.Columns.Add("RowLockWaitInMs", typeof(System.Int64));
                    snapshot.RowLockIndexContention.Columns.Add("AvgRowLockWaitInMs", typeof(System.Decimal));

                    if (datareader.NextResult())
                    {
                        while (datareader.Read())
                        {
                            snapshot.RowLockIndexContention.Rows.Add(
                            ProbeHelpers.ToString(datareader,"DatabaseName"),
                            ProbeHelpers.ToString(datareader, "Schema"),
                            ProbeHelpers.ToString(datareader,"TableName"),
                            ProbeHelpers.ToString(datareader,"IndexName"),
                            ProbeHelpers.ToInt32(datareader,"Partition"),
                            ProbeHelpers.ToInt64(datareader,"RowLockCount"),
                            ProbeHelpers.ToInt64(datareader,"RowLockWaitCount"),
                            ProbeHelpers.ToDecimal(datareader,"RowLockPercent"),
                            ProbeHelpers.ToInt64(datareader,"RowLockWaitInMs"),
                            ProbeHelpers.ToDecimal(datareader,"AvgRowLockWaitInMs")
                        );
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting IndexContention Collector: {0}",
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
