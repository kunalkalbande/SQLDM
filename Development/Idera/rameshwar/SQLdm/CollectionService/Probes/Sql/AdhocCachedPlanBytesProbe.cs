//------------------------------------------------------------------------------
// <copyright file="AdhocCachedPlanBytesProbe.cs" company="Idera, Inc.">
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
    internal class AdhocCachedPlanBytesProbe : SqlBaseProbe
    {
        #region fields

        private AdhocCachedPlanBytesSnapshot snapshot = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdhocCachedPlanBytesProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public AdhocCachedPlanBytesProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
            : base(connectionInfo)
        {
            snapshot = new AdhocCachedPlanBytesSnapshot(connectionInfo);
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
            StartAdhocCachedPlanBytesCollector();
        }

        /// <summary>
        /// Define the AdhocCachedPlanBytes collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void AdhocCachedPlanBytesCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildGetAdhocCachedPlanBytesCommand(conn, ver, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(AdhocCachedPlanBytesCallback));
        }

        /// <summary>
        /// Starts the AdhocCachedPlanBytes collector.
        /// </summary>
        void StartAdhocCachedPlanBytesCollector()
        {
            StartGenericCollector(new Collector(AdhocCachedPlanBytesCollector), snapshot, "StartAdhocCachedPlanBytesCollector", "AdhocCachedPlanBytes", AdhocCachedPlanBytesCallback, new object[] { });
        }

        /// <summary>
        /// Define the AdhocCachedPlanBytes callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void AdhocCachedPlanBytesCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretAdhocCachedPlanBytes(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the AdhocCachedPlanBytes collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void AdhocCachedPlanBytesCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(AdhocCachedPlanBytesCallback), snapshot, "AdhocCachedPlanBytesCallback", "AdhocCachedPlanBytes",
                            sender, e);
        }

        private void InterpretAdhocCachedPlanBytes(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretAdhocCachedPlanBytes"))
            {
                try
                {
                    snapshot.AdhocCachedPlanBytes.Columns.Add("Value", typeof(UInt64));

                    while (datareader.Read())
                    {
                        snapshot.AdhocCachedPlanBytes.Rows.Add(
                            datareader.IsDBNull(0) ? 0 : datareader.GetInt64(0)
                        );
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting AdhocCachedPlanBytes Collector: {0}",
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
