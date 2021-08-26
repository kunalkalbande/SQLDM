//------------------------------------------------------------------------------
// <copyright file="NUMANodeCountersProbe.cs" company="Idera, Inc.">
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
    internal class NUMANodeCountersProbe : SqlBaseProbe
    {
        #region fields

        private NUMANodeCountersSnapshot snapshot = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NUMANodeCountersProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public NUMANodeCountersProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            snapshot = new NUMANodeCountersSnapshot(connectionInfo);
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
            StartNUMANodeCountersCollector();
        }

        /// <summary>
        /// Define the NUMANodeCounters collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void NUMANodeCountersCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildNUMANodeCountersCommand(conn, ver, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(NUMANodeCountersCallback));
        }

        /// <summary>
        /// Starts the NUMANodeCounters collector.
        /// </summary>
        void StartNUMANodeCountersCollector()
        {
            StartGenericCollector(new Collector(NUMANodeCountersCollector), snapshot, "StartNUMANodeCountersCollector", "NUMANodeCounters", NUMANodeCountersCallback, new object[] { });
        }

        /// <summary>
        /// Define the NUMANodeCounters callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void NUMANodeCountersCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretNUMANodeCounters(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the NUMANodeCounters collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void NUMANodeCountersCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(NUMANodeCountersCallback), snapshot, "NUMANodeCountersCallback", "NUMANodeCounters",
                            sender, e);
        }

        private void InterpretNUMANodeCounters(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretNUMANodeCounters"))
            {
                try
                {
                    snapshot.NUMANodeCounters.Columns.Add("NodeName", typeof(string));
                    snapshot.NUMANodeCounters.Columns.Add("PLE", typeof(long));
                    snapshot.NUMANodeCounters.Columns.Add("TargetPages", typeof(long));

                    while (datareader.Read())
                    {
                        snapshot.NUMANodeCounters.Rows.Add(
                            ProbeHelpers.ToString(datareader,"NodeName"),
                            ProbeHelpers.ToInt64(datareader,"PLE"),
                            ProbeHelpers.ToInt64(datareader,"TargetPages")
                        );
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting NUMANode Collector: {0}",
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
