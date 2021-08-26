//------------------------------------------------------------------------------
// <copyright file="SampleServerResourcesProbe.cs" company="Idera, Inc.">
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
    internal class SampleServerResourcesProbe : SqlIntervalProbe
    {
        #region fields

        private SampleServerResourcesSnapshot snapshot = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleServerResourcesProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public SampleServerResourcesProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
            : base(connectionInfo, cloudProviderId)
        {
            this.cloudProviderId = cloudProviderId;
            snapshot = new SampleServerResourcesSnapshot(connectionInfo);
            LOG = Logger.GetLogger("SampleServerResourcesProbe");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleServerResourcesProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public SampleServerResourcesProbe(SqlConnectionInfo connectionInfo, int interval, int max, int? cloudProviderId)
            : base(connectionInfo, interval, max, cloudProviderId)
        {
            this.cloudProviderId = cloudProviderId;
            snapshot = new SampleServerResourcesSnapshot(connectionInfo);
            LOG = Logger.GetLogger("SampleServerResourcesProbe");
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
            StartSampleServerResourcesCollector();
        }

        /// <summary>
        /// Define the SampleServerResources collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void SampleServerResourcesCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildSampleServerResourcesCommand(conn, ver, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(SampleServerResourcesCallback));
        }

        /// <summary>
        /// Starts the SampleServerResources collector.
        /// </summary>
        void StartSampleServerResourcesCollector()
        {
            StartGenericCollector(new Collector(SampleServerResourcesCollector), snapshot, "StartSampleServerResourcesCollector", "SampleServerResources", SampleServerResourcesCallback, new object[] { });
        }

        /// <summary>
        /// Define the SampleServerResources callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void SampleServerResourcesCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretSampleServerResources(rd);
            }
            if (!IsInterval)
            {
                FireCompletion(snapshot, Result.Success);
                return;
            }
            Thread.Sleep(Interval * 1000);
            if (IsRunnable)
            {
                StartSampleServerResourcesCollector();
            }
            else
                FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the SampleServerResources collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void SampleServerResourcesCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(SampleServerResourcesCallback), snapshot, "SampleServerResourcesCallback", "SampleServerResources",
                            sender, e);
        }

        private void InterpretSampleServerResources(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretSampleServerResources"))
            {
                try
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("Name", typeof(string));
                    dt.Columns.Add("Value", typeof(string));

                    while (datareader.Read())
                    {
                        dt.Rows.Add(
                       ProbeHelpers.ToString(datareader, "Name"),
                       ProbeHelpers.ToString(datareader, "Value"));

                    }
                    if (IsInterval)
                    {
                        if (snapshot.LstSampleServerResources == null)
                            snapshot.LstSampleServerResources = new System.Collections.Generic.List<DataTable>();
                        snapshot.LstSampleServerResources.Add(dt);
                        ExecutionCount++;
                    }
                    else
                        snapshot.SampleServerResources = dt;
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting SampleServerResources Collector: {0}",
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
