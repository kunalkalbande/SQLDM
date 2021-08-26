//------------------------------------------------------------------------------
// <copyright file="WaitStatsProbe.cs" company="Idera, Inc.">
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
    using Common.Snapshots;
    using Common.Services;


    /// <summary>
    /// Enter a description for this class
    /// </summary>
    [Serializable]
    internal sealed class WaitStatisticsProbe : SqlBaseProbe
    {
        #region fields

        private WaitStatisticsSnapshot refresh = null;
        private WaitStatisticsConfiguration config = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitStatisticsProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skip permissions for CloudProviders</param>
        public WaitStatisticsProbe(SqlConnectionInfo connectionInfo, WaitStatisticsConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("WaitStatsProbe");
            refresh = new WaitStatisticsSnapshot(connectionInfo);
            this.config = config;
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
             if (config != null && config.ReadyForCollection)
            {
                StartWaitStatsCollector();
            }
            else
            {
                FireCompletion(refresh, Result.Success);
            }
        }

        /// <summary>
        /// Define the WaitStats collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void WaitStatsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            if (ver.Major > 8)
            {
                SqlCommand cmd =
                    SqlCommandBuilder.BuildWaitStatsCommand(conn, ver, cloudProviderId);
                sdtCollector = new SqlCollector(cmd, true);
                sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(WaitStatsCallback));
            }
            else
            {
                refresh.SetError("Version not supported.",
                                 new Exception("Wait Statistics are not available for SQL Server versions before 2005."));
                FireCompletion(refresh,Result.Success);
            }

        }

        /// <summary>
        /// Starts the Wait Stats collector.
        /// </summary>
        private void StartWaitStatsCollector()
        {
            StartGenericCollector(new Collector(WaitStatsCollector), refresh, "StartWaitStatsCollector", "Wait Stats", WaitStatsCallback, new object[] { });
        }

        /// <summary>
        /// Define the WaitStats callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void WaitStatsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretWaitStats(rd);
            }
            FireCompletion(refresh, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the WaitStats collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void WaitStatsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(WaitStatsCallback),
                            refresh,
                            "WaitStatsCallback",
                            "Wait Stats",
                            sender,
                            e);
        }

        /// <summary>
        /// Interpret WaitStats data
        /// </summary>
        private void InterpretWaitStats(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretWaitStats"))
            {
                try
                {
                    TimeSpan? timeDelta = null;

                    if (config.PreviousServerStartup.HasValue && refresh.ServerStartupTime != config.PreviousServerStartup)
                    {
                        LOG.Info("Server restart detected.  Disposing of previous metrics.");
                        config.PreviousWaits = null;
                    }
                    else
                    if (config.PreviousTimeStamp.HasValue)
                    {
                        timeDelta = refresh.TimeStamp - config.PreviousTimeStamp;
                    }

                    refresh.Waits = ProbeHelpers.ReadWaitStatistics(dataReader, timeDelta);

                    if (config.PreviousWaits != null)
                    {
                        refresh.CalculateWaitsDeltas(config.PreviousWaits);
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(refresh,
                                                        LOG,
                                                        "Error interpreting Wait Stats Collector: {0}",
                                                        e,
                                                        false);
                    GenericFailureDelegate(refresh);
                }
            }
        }

        
        #endregion

        #region interface implementations

        #endregion
    }
}
