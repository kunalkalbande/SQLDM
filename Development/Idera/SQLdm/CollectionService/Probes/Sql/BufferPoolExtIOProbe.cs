//------------------------------------------------------------------------------
// <copyright file="BufferPoolExtIOProbe.cs" company="Idera, Inc.">
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
    /// //SQLdm 10.0 (Srishti Purohit) (SDR-M33) - New Probe class
    /// </summary>
    internal class BufferPoolExtIOProbe : SqlIntervalProbe
    {
        #region fields

        private BufferPoolExtIOSnapshot snapshot = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferPoolExtIOProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public BufferPoolExtIOProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
            : base(connectionInfo, cloudProviderId)
        {
            snapshot = new BufferPoolExtIOSnapshot(connectionInfo);
            LOG = Logger.GetLogger("BufferPoolExtIOProbe");
            this.cloudProviderId = cloudProviderId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferPoolExtIOProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public BufferPoolExtIOProbe(SqlConnectionInfo connectionInfo, int interval, int max, int? cloudProviderId)
            : base(connectionInfo, interval, max, cloudProviderId)
        {
            snapshot = new BufferPoolExtIOSnapshot(connectionInfo);
            snapshot.Interval = interval;
            LOG = Logger.GetLogger("BufferPoolExtIOProbe");
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
            StartBufferPoolExtIOCollector();
        }

        /// <summary>
        /// Define the BufferPoolExtIO collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void BufferPoolExtIOCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildBufferPoolExtIOCommand(conn, ver, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(BufferPoolExtIOCallback));
        }

        /// <summary>
        /// Starts the BufferPoolExtIO collector.
        /// </summary>
        void StartBufferPoolExtIOCollector()
        {
            StartGenericCollector(new Collector(BufferPoolExtIOCollector), snapshot, "StartBufferPoolExtIOCollector", "BufferPoolExtIO", BufferPoolExtIOCallback, new object[] { });
        }

        /// <summary>
        /// Define the BufferPoolExtIO callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void BufferPoolExtIOCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretBufferPoolExtIO(rd);
            }
            if (!IsInterval)
            {
                FireCompletion(snapshot, Result.Success);
                return;
            }
            Thread.Sleep(Interval * 1000);
            if (IsRunnable)
            {
                StartBufferPoolExtIOCollector();
            }
            else
                FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the BufferPoolExtIO collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void BufferPoolExtIOCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(BufferPoolExtIOCallback), snapshot, "BufferPoolExtIOCallback", "BufferPoolExtIO",
                            sender, e);
        }

        private void InterpretBufferPoolExtIO(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretBufferPoolExtIO"))
            {
                try
                {
                    long IOCounter = 0;
                    while (datareader.Read())
                    {
                        snapshot.State = ProbeHelpers.ToInt32(datareader, "state");
                    }

                    datareader.NextResult();
                    while (datareader.Read())
                    {
                        IOCounter = ProbeHelpers.ToInt64(datareader, "cntr_value");
                        //IOCounter = 11;
                        if (snapshot.State > 0)
                        {
                            snapshot.CurrentValue.Add(IOCounter);
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting BufferPoolExtIO Collector: {0}",
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
