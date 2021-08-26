//------------------------------------------------------------------------------
// <copyright file="LockedPageKBProbe.cs" company="Idera, Inc.">
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
    internal class LockedPageKBProbe : SqlBaseProbe
    {
        #region fields

        private LockedPageKBSnapshot snapshot = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LockedPageKBProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public LockedPageKBProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            snapshot = new LockedPageKBSnapshot(connectionInfo);
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
            StartLockedPageKBCollector();
        }

        /// <summary>
        /// Define the LockedPageKB collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void LockedPageKBCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildGetLockedPageKBCommand(conn, ver, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(LockedPageKBCallback));
        }

        /// <summary>
        /// Starts the LockedPageKB collector.
        /// </summary>
        void StartLockedPageKBCollector()
        {
            StartGenericCollector(new Collector(LockedPageKBCollector), snapshot, "StartLockedPageKBCollector", "LockedPageKB", LockedPageKBCallback, new object[] { });
        }

        /// <summary>
        /// Define the LockedPageKB callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void LockedPageKBCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretLockedPageKB(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the LockedPageKB collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void LockedPageKBCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(LockedPageKBCallback), snapshot, "LockedPageKBCallback", "LockedPageKB",
                            sender, e);
        }

        private void InterpretLockedPageKB(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretLockedPageKB"))
            {
                try
                {
                    snapshot.LockedPageKB.Columns.Add("Value", typeof(UInt64));

                    while (datareader.Read())
                    {
                        snapshot.LockedPageKB.Rows.Add(
                            datareader.IsDBNull(0) ? 0 : datareader.GetInt64(0)
                        );
                    }

                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting LockedPageKB Collector: {0}",
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
