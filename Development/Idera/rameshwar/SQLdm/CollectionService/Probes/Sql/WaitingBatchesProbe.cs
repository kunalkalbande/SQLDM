//------------------------------------------------------------------------------
// <copyright file="WaitingBatchesProbe.cs" company="Idera, Inc.">
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
    using System.Threading;

    /// <summary>
    /// //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) - New Probe class
    /// </summary>
    internal class WaitingBatchesProbe : SqlIntervalProbe
    {
        #region fields

        private WaitingBatchesSnapshot snapshot = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitingBatchesProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public WaitingBatchesProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
            : base(connectionInfo, cloudProviderId)
        {
            this.cloudProviderId = cloudProviderId;
            snapshot = new WaitingBatchesSnapshot(connectionInfo);
            LOG = Logger.GetLogger("SessionsProbe");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitingBatchesProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public WaitingBatchesProbe(SqlConnectionInfo connectionInfo, int interval, int max, int? cloudProviderId)
            : base(connectionInfo, interval, max, cloudProviderId)
        {
            this.cloudProviderId = cloudProviderId;
            snapshot = new WaitingBatchesSnapshot(connectionInfo);
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
            StartWaitingBatchesCollector();
        }

        /// <summary>
        /// Define the WaitingBatches collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void WaitingBatchesCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildWaitingBatchesCommand(conn, ver, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(WaitingBatchesCallback));
        }

        /// <summary>
        /// Starts the WaitingBatches collector.
        /// </summary>
        void StartWaitingBatchesCollector()
        {
            StartGenericCollector(new Collector(WaitingBatchesCollector), snapshot, "StartWaitingBatchesCollector", "WaitingBatches", WaitingBatchesCallback, new object[] { });
        }

        /// <summary>
        /// Define the WaitingBatches callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void WaitingBatchesCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretWaitingBatches(rd);
            }
            if (!IsInterval)
            {
                FireCompletion(snapshot, Result.Success);
                return;
            }
            Thread.Sleep(Interval * 1000);
            if (IsRunnable)
            {
                StartWaitingBatchesCollector();
            }
            else
                FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the WaitingBatches collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void WaitingBatchesCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(WaitingBatchesCallback), snapshot, "WaitingBatchesCallback", "WaitingBatches",
                            sender, e);
        }

        private void InterpretWaitingBatches(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretWaitingBatches"))
            {
                try
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("wait_type", typeof(string));
                    dt.Columns.Add("wait_duration_ms", typeof(long));
                    dt.Columns.Add("session_id", typeof(Int32));
                    dt.Columns.Add("resource_description", typeof(string));
                    dt.Columns.Add("program_name", typeof(string));
                    //snapshot.WaitingBatches.Columns.Add("sql_handle", typeof());
                    //snapshot.WaitingBatches.Columns.Add("statement_start_offset", typeof(bool));
                    //snapshot.WaitingBatches.Columns.Add("statement_end_offset", typeof(bool));
                    dt.Columns.Add("text", typeof(string));

                    while (datareader.Read())
                    {
                        dt.Rows.Add(
                            ProbeHelpers.ToString(datareader,"wait_type"),
                            ProbeHelpers.ToInt64(datareader,"wait_duration_ms"),
                            ProbeHelpers.ToInt32(datareader,"session_id"),
                            ProbeHelpers.ToString(datareader,"resource_description"),
                            ProbeHelpers.ToString(datareader,"program_name"),
                            //datareader.GetBoolean(datareader.GetOrdinal("sql_handle")),
                            //datareader.GetBoolean(datareader.GetOrdinal("statement_start_offset")),
                            //datareader.GetBoolean(datareader.GetOrdinal("statement_end_offset")),
                            ProbeHelpers.ToString(datareader,"text")
                        );
                    }
                    if (IsInterval)
                    {
                        if (snapshot.ListWaitingBatches == null)
                            snapshot.ListWaitingBatches = new System.Collections.Generic.List<DataTable>();
                        snapshot.ListWaitingBatches.Add(dt);
                        ExecutionCount++;
                    }
                    else
                        snapshot.WaitingBatches = dt;
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting WaitingBatches Collector: {0}",
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
