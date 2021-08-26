//------------------------------------------------------------------------------
// <copyright file="ProbeFactory.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
// Change Log   ----------------------------------------------------------------
// Modified By          :   Pruthviraj Nikam
// Modification ID      :   M1
// Date                 :   04-Feb-2019
// Description          :   Done changes for New Azure SQL DB alerts.
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Idera.SQLdm.CollectionService.Probes
{
    using Idera.SQLdm.CollectionService.Probes.Sql;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Snapshots;
    using Idera.SQLdm.Common.Configuration.ServerActions;
    using Collectors;

    /// <summary>
    /// The probe factory creates instances of probes from supplied arguments.
    /// </summary>
    public class ProbeFactory
    {
        #region fields

        #endregion

        #region constructors

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        // SQLdm 10.3 (Varun Chopra) Linux Support by passing cloudProviderId
        public static IProbe BuildContinuousProbe(SqlConnectionInfo connectionInfo, OnDemandConfiguration configuration, int? cloudProviderId)
        {
            if (configuration.GetType() == typeof(ActiveWaitsConfiguration))
                return BuildActiveWaitsProbe(connectionInfo, (ActiveWaitsConfiguration)configuration, cloudProviderId);
            if (configuration.GetType() == typeof(TableGrowthConfiguration))
                return BuildTableGrowthProbe(connectionInfo, (TableGrowthConfiguration)configuration, cloudProviderId);
            if (configuration.GetType() == typeof(TableFragmentationConfiguration))
                return BuildTableFragmentationProbe(connectionInfo, (TableFragmentationConfiguration)configuration, cloudProviderId);
            else
                return null;
        }

        /// <summary>
        /// Builds the ActiveWaits probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <returns></returns>
        public static IProbe BuildActiveWaitsProbe(SqlConnectionInfo connectionInfo, ActiveWaitsConfiguration configuration, int? cloudProviderId)
        {
            // SQLdm 10.3 (Varun Chopra) Linux Support by passing cloudProviderId
            ActiveWaitsProbe srvProbe = new ActiveWaitsProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the scheduled refresh probe.
        /// </summary>
        /// <param name="monitoredServer">The monitored server.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildScheduledRefreshProbe(MonitoredSqlServer monitoredServer, MonitoredServerWorkload workload, int? cloudProviderId)
        {
            ScheduledRefreshProbe srvProbe = new ScheduledRefreshProbe(monitoredServer, workload, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the server ping probe.
        /// </summary>
        /// <param name="monitoredServer">The monitored server.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildServerPingProbe(MonitoredSqlServer monitoredServer, MonitoredServerWorkload workload, int? cloudProviderId)
        {
            ServerPingProbe srvProbe = new ServerPingProbe(monitoredServer, workload, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the AgentJobHistory probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildAgentJobHistoryProbe(SqlConnectionInfo connectionInfo, AgentJobHistoryConfiguration configuration, int? cloudProviderId)
        {
            AgentJobHistoryProbe srvProbe = new AgentJobHistoryProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        // Modification Start ID: M1
        /// <summary>
        /// Builds the BuildAzureSQLMetric probe
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <param name="cloudProviderId">cloud Provider Id</param>
        /// <returns></returns>
        //public static IProbe BuildAzureSQLMetric(SqlConnectionInfo connectionInfo, int? cloudProviderId)
        //{
        //    AzureSQLMetricProbe asmProb = new AzureSQLMetricProbe(connectionInfo, cloudProviderId);
        //    return asmProb;
        //}
        // Modification End ID: M1

        //public static IProbe BuildAmazonRDSMetric(SqlConnectionInfo connectionInfo, int? cloudProviderId)
        //{
        //    AWSMetricProbe awsMetricProb = new AWSMetricProbe(connectionInfo, cloudProviderId);
        //    return awsMetricProb;
        //}

        /// <summary>
        /// Builds the AgentJobSummary probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildAgentJobSummaryProbe(SqlConnectionInfo connectionInfo, AgentJobSummaryConfiguration configuration, WmiConfiguration wmiConfig, ClusterCollectionSetting clusterCollectionSetting, MonitoredServerWorkload workload, int? cloudProviderId) // SQLdm 8.6 (Ankit Srivastava) -- Added workload parameter - solved defect DE43661
        {
            AgentJobSummaryProbe srvProbe = new AgentJobSummaryProbe(connectionInfo, configuration, wmiConfig, clusterCollectionSetting, workload, cloudProviderId); // SQLdm 8.6 (Ankit Srivastava) -- Added workload parameter - solved defect DE43661
            return srvProbe;
        }

        /// <summary>
        /// Builds the BackupRestoreHistory probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildBackupRestoreHistoryProbe(SqlConnectionInfo connectionInfo, BackupRestoreHistoryConfiguration configuration, int? cloudProviderId)
        {
            BackupRestoreHistoryProbe srvProbe = new BackupRestoreHistoryProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the configuration probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildConfigurationProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
        {
            ConfigurationProbe srvProbe = new ConfigurationProbe(connectionInfo, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the CustomCounter probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildCustomCounterProbe(SqlConnectionInfo connectionInfo, List<CustomCounterConfiguration> configuration, TimeSpan timeout, WmiConfiguration wmiConfiguration, VirtualizationConfiguration virtualizationConfiguration, int? cloudProviderId,
            TimeSpan scheduledCollectionInterval)
        {
            CustomCounterProbe srvProbe = new CustomCounterProbe(connectionInfo, configuration, timeout, wmiConfiguration, virtualizationConfiguration, cloudProviderId, scheduledCollectionInterval);
            return srvProbe;
        }

        /// <summary>
        /// Builds the DatabaseConfiguration probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildDatabaseConfigurationProbe(SqlConnectionInfo connectionInfo, DatabaseProbeConfiguration configuration, int? cloudProviderId)
        {
            DatabaseConfigurationProbe srvProbe = new DatabaseConfigurationProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the DatabaseFiles probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildDatabaseFilesProbe(SqlConnectionInfo connectionInfo, DatabaseFilesConfiguration configuration, WmiConfiguration wmiConfiguration, DiskCollectionSettings disksettings, int? cloudProviderId)
        {
            DatabaseFilesProbe srvProbe = new DatabaseFilesProbe(connectionInfo, configuration, wmiConfiguration, disksettings, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the DatabaseSize probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildDatabaseSizeProbe(SqlConnectionInfo connectionInfo, DatabaseSizeConfiguration configuration, WmiConfiguration wmiConfiguration, DiskCollectionSettings disksettings, int? cloudProviderId)
        {
            DatabaseSizeProbe srvProbe = new DatabaseSizeProbe(connectionInfo, configuration, wmiConfiguration, disksettings,cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the DatabaseSummary probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildDatabaseSummaryProbe(SqlConnectionInfo connectionInfo, DatabaseSummaryConfiguration configuration, 
                                                       WmiConfiguration wmiConfiguration, DiskCollectionSettings diskSettings, int? cloudProviderId)
        {
            DatabaseSummaryProbe srvProbe = new DatabaseSummaryProbe(connectionInfo, configuration, wmiConfiguration, diskSettings, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the Database AlwaysOn Statistics probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildDatabaseAlwaysOnStatisticsProbe(SqlConnectionInfo connectionInfo, AlwaysOnAvailabilityGroupsConfiguration configuration, int? cloudProviderId)
        {
            DatabaseAlwaysOnStatisticsProbe srvProbe = new DatabaseAlwaysOnStatisticsProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the Database AlwaysOn Topology probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildDatabaseAlwaysOnTopologyProbe(SqlConnectionInfo connectionInfo, AlwaysOnAvailabilityGroupsConfiguration configuration, int? cloudProviderId)
        {
            DatabaseAlwaysOnTopologyProbe srvProbe = new DatabaseAlwaysOnTopologyProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the DistributorQueue probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildDistributorQueueProbe(SqlConnectionInfo connectionInfo, DistributorQueueConfiguration configuration, int? cloudProviderId)
        {
            DistributorQueueProbe srvProbe = new DistributorQueueProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }
        /// <summary>
        /// Builds the Distributor details probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildDistributorDetailsProbe(SqlConnectionInfo connectionInfo, DistributorDetailsConfiguration configuration, int? cloudProviderId)
        {
            DistributorDetailsProbe srvProbe = new DistributorDetailsProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }


        /// <summary>
        /// Builds the ErrorLog probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildErrorLogProbe(SqlConnectionInfo connectionInfo, ErrorLogConfiguration configuration, MonitoredServerWorkload workload, int? cloudProviderId)
        {
            ErrorLogProbe srvProbe = new ErrorLogProbe(connectionInfo, configuration, workload, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the FileActivity probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildFileActivityProbe(SqlConnectionInfo connectionInfo, FileActivityConfiguration configuration, WmiConfiguration wmiConfiguration, DiskCollectionSettings disksettings, int? cloudProviderId)
        {
            FileActivityProbe srvProbe = new FileActivityProbe(connectionInfo, configuration, wmiConfiguration, disksettings,cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the FullTextCatalogs probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildFullTextCatalogsProbe(SqlConnectionInfo connectionInfo, OnDemandConfiguration configuration, WmiConfiguration wmiConfiguration, ClusterCollectionSetting clusterCollectionSetting, int? cloudProviderId)
        {
            FullTextCatalogsProbe srvProbe = new FullTextCatalogsProbe(connectionInfo, wmiConfiguration, clusterCollectionSetting, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the FullTextColumns probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildFullTextColumnsProbe(SqlConnectionInfo connectionInfo, FullTextColumnsConfiguration configuration, int? cloudProviderId)
        {
            FullTextColumnsProbe srvProbe = new FullTextColumnsProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the FullTextTables probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildFullTextTablesProbe(SqlConnectionInfo connectionInfo, FullTextTablesConfiguration configuration, int? cloudProviderId)
        {
            FullTextTablesProbe srvProbe = new FullTextTablesProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the IndexStatistics probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <returns></returns>
        public static IProbe BuildIndexStatisticsProbe(SqlConnectionInfo connectionInfo, IndexStatisticsConfiguration configuration, int? cloudProviderId)
        {
            IndexStatisticsProbe srvProbe = new IndexStatisticsProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the LockDetails probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <returns></returns>
        public static IProbe BuildLockDetailsProbe(SqlConnectionInfo connectionInfo, LockDetailsConfiguration configuration, int? cloudProviderId)
        {
            LockDetailsProbe srvProbe = new LockDetailsProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mount Point Monitoring Improvements --get the disk size details on demand
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <param name="configuration"></param>
        /// <param name="wmiConfig"></param>
		//// <param name="diskSettings"></param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildDiskSizeDetailsProbe(SqlConnectionInfo connectionInfo, DatabaseProbeConfiguration configuration, WmiConfiguration wmiConfig, DiskCollectionSettings diskSettings, int? cloudProviderId)
        {
            DiskSizeProbe srvProbe = new DiskSizeProbe(connectionInfo, configuration,wmiConfig,diskSettings, cloudProviderId);
            return srvProbe;
            
        }

        /// <summary>
        /// Builds the LogList probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildLogListProbe(SqlConnectionInfo connectionInfo, MonitoredServerWorkload workload, int? cloudProviderId) // SQLdm 8.6 (Ankit Srivastava) -- Added workload parameter - solved defect DE43661
        {
            LogListProbe srvProbe = new LogListProbe(connectionInfo, workload, cloudProviderId); // SQLdm 8.6 (Ankit Srivastava) -- Added workload parameter - solved defect DE43661
            return srvProbe;
        }

        /// <summary>
        /// Builds the MirrorMonitoringRealtime probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildMirrorMonitoringRealtimeProbe(SqlConnectionInfo connectionInfo, MirrorMonitoringRealtimeConfiguration configuration, int? cloudProviderId)
        {
            MirrorMonitoringRealtimeProbe srvProbe = new MirrorMonitoringRealtimeProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the MirrorMonitoringHistory probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildMirrorMonitoringHistoryProbe(SqlConnectionInfo connectionInfo, MirrorMonitoringHistoryConfiguration configuration, int? cloudProviderId)
        {
            MirrorMonitoringHistoryProbe srvProbe = new MirrorMonitoringHistoryProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the JobsAndSteps probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildJobsAndStepsProbe(SqlConnectionInfo connectionInfo, JobsAndStepsConfiguration configuration, int? cloudProviderId)
        {
            JobsAndStepsProbe srvProbe = new JobsAndStepsProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the ProcedureCache probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildProcedureCacheProbe(SqlConnectionInfo connectionInfo, ProcedureCacheConfiguration configuration, int? cloudProviderId)
        {
            ProcedureCacheProbe srvProbe = new ProcedureCacheProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        ///// <summary>
        ///// Builds the PublisherQueue probe.
        ///// </summary>
        ///// <param name="connectionInfo">The connection info.</param>
        ///// <returns></returns>
        //public static IProbe BuildPublisherQueueProbe(SqlConnectionInfo connectionInfo, PublisherQueueConfiguration configuration)
        //{
        //    PublisherQueueProbe srvProbe = new PublisherQueueProbe(connectionInfo, configuration);
        //    return srvProbe;
        //}
        /// <summary>
        /// Builds the Publisher Details probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="configuration"></param>
        /// <param name="disableOle"></param>
        /// <param name="spOAContext"></param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildPublisherDetailsProbe(SqlConnectionInfo connectionInfo, PublisherDetailsConfiguration configuration, WmiConfiguration wmiConfiguration, ClusterCollectionSetting clusterCollectionSetting, int? cloudProviderId)
        {
            PublisherDetailsProbe srvProbe = new PublisherDetailsProbe(connectionInfo, configuration, wmiConfiguration, clusterCollectionSetting, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the Subscriber Details probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildSubscriberDetailsProbe(SqlConnectionInfo connectionInfo, SubscriberDetailsConfiguration configuration, int? cloudProviderId)
        {
            SubscriberDetailsProbe srvProbe = new SubscriberDetailsProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }


        ///// <summary>
        ///// Builds the Resource probe.
        ///// </summary>
        ///// <param name="connectionInfo">The connection info.</param>
        ///// <returns></returns>
        //public static IProbe BuildResourceProbe(SqlConnectionInfo connectionInfo, ResourceConfiguration configuration, int spOAContext, bool disableOle, DiskCollectionSettings diskSettings)
        //{
        //    ResourceProbe srvProbe = new ResourceProbe(connectionInfo, configuration, spOAContext,disableOle, diskSettings);
        //    return srvProbe;
        //}

        /// <summary>
        /// Builds the server overview probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <returns></returns>

        public static IProbe BuildServerOverviewProbe(SqlConnectionInfo connectionInfo, ServerOverviewConfiguration configuration, WmiConfiguration wmiConfiguration, ClusterCollectionSetting clusterCollectionSetting, DiskCollectionSettings diskSettings, VirtualizationConfiguration virtualizationConfiguration, MonitoredSqlServer msi, int? cloudProviderId = null)//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support:Added a new optional param //SQLDM 11.0 Monitored Server
        {
            ServerOverviewProbe srvProbe = new ServerOverviewProbe(connectionInfo, configuration, wmiConfiguration, clusterCollectionSetting, diskSettings, virtualizationConfiguration, msi, cloudProviderId);//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support:Added a new optional param

            return srvProbe;
        }

        /// <summary>
        /// Builds the Services probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildServicesProbe(SqlConnectionInfo connectionInfo, OnDemandConfiguration configuration, WmiConfiguration wmiConfiguration, ClusterCollectionSetting clusterCollectionSetting, int? cloudProviderId)
        {
            ServicesProbe srvProbe = new ServicesProbe(connectionInfo, wmiConfiguration, clusterCollectionSetting,cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the sessions probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <returns></returns>
        public static IProbe BuildSessionsProbe(SqlConnectionInfo connectionInfo, SessionsConfiguration configuration, int? cloudProviderId = null) //SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
        {
            SessionsProbe srvProbe = new SessionsProbe(connectionInfo, configuration,cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the SessionDetails probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildSessionDetailsProbe(SqlConnectionInfo connectionInfo, SessionDetailsConfiguration configuration, int? cloudProviderId)
        {
            SessionDetailsProbe srvProbe = new SessionDetailsProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the SessionSummary probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildSessionSummaryProbe(SqlConnectionInfo connectionInfo, SessionSummaryConfiguration configuration, int? cloudProviderId)
        {
            SessionSummaryProbe srvProbe = new SessionSummaryProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the TableDetails probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildTableDetailsProbe(SqlConnectionInfo connectionInfo, TableDetailConfiguration configuration, int? cloudProviderId)
        {
            TableDetailsProbe srvProbe = new TableDetailsProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }


        /// <summary>
        /// Builds the TableFragmentation probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildTableFragmentationProbe(SqlConnectionInfo connectionInfo, TableFragmentationConfiguration configuration, int? cloudProviderId)
        {
            TableFragmentationProbe srvProbe = new TableFragmentationProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }


        /// <summary>
        /// Builds the TableGrowth probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildTableGrowthProbe(SqlConnectionInfo connectionInfo, TableGrowthConfiguration configuration, int? cloudProviderId)
        {
            TableGrowthProbe srvProbe = new TableGrowthProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }


        /// <summary>
        /// Builds the TableSummary probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildTableSummaryProbe(SqlConnectionInfo connectionInfo, TableSummaryConfiguration configuration, int? cloudProviderId)
        {
            TableSummaryProbe srvProbe = new TableSummaryProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Builds the WaitStats probe.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWaitStatisticsProbe(SqlConnectionInfo connectionInfo, WaitStatisticsConfiguration configuration, int? cloudProviderId)
        {
            WaitStatisticsProbe srvProbe = new WaitStatisticsProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildDisabledIndexesProbe(SqlConnectionInfo connectionInfo, string db, int? cloudProviderId)
        {
            if (!string.IsNullOrWhiteSpace(db))
            {
                connectionInfo.DatabaseName = db;
            }
            DisabledIndexesProbe srvProbe = new DisabledIndexesProbe(connectionInfo, db, cloudProviderId);
            return srvProbe;
        }
        //
        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWmiVolumeProbe(SqlConnectionInfo connectionInfo, WmiConfiguration config, int? cloudProviderId)
        {
            WmiVolumeProbe srvProbe = new WmiVolumeProbe(connectionInfo , config, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWmiProcessProbe(SqlConnectionInfo connectionInfo, WmiConfiguration config, int? cloudProviderId)
        {
            WmiProcessProbe srvProbe = new WmiProcessProbe(connectionInfo, config, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWmiTCPProbe(SqlConnectionInfo connectionInfo, WmiConfiguration config, int? cloudProviderId)
        {
            WmiTCPProbe srvProbe = new WmiTCPProbe(connectionInfo, config, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWmiTCPv4Probe(SqlConnectionInfo connectionInfo, WmiConfiguration config, int? cloudProviderId)
        {
            WmiTCPv4Probe srvProbe = new WmiTCPv4Probe(connectionInfo, config, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWmiTCPv6Probe(SqlConnectionInfo connectionInfo, WmiConfiguration config, int? cloudProviderId)
        {
            WmiTCPv6Probe srvProbe = new WmiTCPv6Probe(connectionInfo, config, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWmiPerfOSSystemProbe(SqlConnectionInfo connectionInfo, WmiConfiguration config, int? cloudProviderId)
        {
            return BuildWmiPerfOSSystemProbe(connectionInfo, config, false, 0, 0, cloudProviderId);
        }

        public static IProbe BuildWmiPerfOSSystemProbe(SqlConnectionInfo connectionInfo, WmiConfiguration config, bool isInterval, int interval, int max, int? cloudProviderId)
        {
            if (isInterval)
            {
                WmiPerfOSSystemProbe srvProbe = new WmiPerfOSSystemProbe(connectionInfo, config, interval, max, cloudProviderId);
                return srvProbe;
            }
            else
            {
                WmiPerfOSSystemProbe srvProbe = new WmiPerfOSSystemProbe(connectionInfo, config, cloudProviderId);
                return srvProbe;
            }
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWmiPerfOSMemoryProbe(SqlConnectionInfo connectionInfo, WmiConfiguration config, int? cloudProviderId)
        {
            WmiPerfOSMemoryProbe srvProbe = new WmiPerfOSMemoryProbe(connectionInfo, config, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWmiNetworkRedirectorProbe(SqlConnectionInfo connectionInfo, WmiConfiguration config, int? cloudProviderId)
        {
            WmiNetworkRedirectorProbe srvProbe = new WmiNetworkRedirectorProbe(connectionInfo, config, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWmiEncryptableVolumeProbe(SqlConnectionInfo connectionInfo, WmiConfiguration config, int? cloudProviderId)
        {
            WmiEncryptableVolumeProbe srvProbe = new WmiEncryptableVolumeProbe(connectionInfo, config, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWmiComputerSystemProbe(SqlConnectionInfo connectionInfo, WmiConfiguration config, int? cloudProviderId)
        {
            WmiComputerSystemProbe srvProbe = new WmiComputerSystemProbe(connectionInfo, config, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWmiProcessorProbe(SqlConnectionInfo connectionInfo, WmiConfiguration config, int? cloudProviderId)
        {
            return BuildWmiProcessorProbe(connectionInfo, config, false, 0, 0, cloudProviderId);
        }

        public static IProbe BuildWmiProcessorProbe(SqlConnectionInfo connectionInfo, WmiConfiguration config, bool isInterval, int interval, int max, int? cloudProviderId)
        {
            if (isInterval)
            {
                WmiProcessorProbe srvProbe = new WmiProcessorProbe(connectionInfo, config, interval, max, cloudProviderId);
                return srvProbe;
            }
            else
            {
                WmiProcessorProbe srvProbe = new WmiProcessorProbe(connectionInfo, config, cloudProviderId);
                return srvProbe;
            }
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWmiNetworkInterfaceProbe(SqlConnectionInfo connectionInfo, WmiConfiguration config, int? cloudProviderId)
        {
            WmiNetworkInterfaceProbe srvProbe = new WmiNetworkInterfaceProbe(connectionInfo, config, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWmiPageFileProbe(SqlConnectionInfo connectionInfo, WmiConfiguration config, int? cloudProviderId)
        {
            WmiPageFileProbe srvProbe = new WmiPageFileProbe(connectionInfo, config, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWmiPhysicalMemoryProbe(SqlConnectionInfo connectionInfo, WmiConfiguration config, int? cloudProviderId)
        {
            WmiPhysicalMemoryProbe srvProbe = new WmiPhysicalMemoryProbe(connectionInfo, config, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWmiPerfDiskLogicalDiskProbe(SqlConnectionInfo connectionInfo, WmiConfiguration config, int? cloudProviderId)
        {
            WmiPerfDiskLogicalDiskProbe srvProbe = new WmiPerfDiskLogicalDiskProbe(connectionInfo, config, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWmiPerfDiskPhysicalDiskProbe(SqlConnectionInfo connectionInfo, WmiConfiguration config, int? cloudProviderId)
        {
            WmiPerfDiskPhysicalDiskProbe srvProbe = new WmiPerfDiskPhysicalDiskProbe(connectionInfo, config, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWmiServiceProbe(SqlConnectionInfo connectionInfo, WmiConfiguration config, int? cloudProviderId)
        {
            WmiServiceProbe srvProbe = new WmiServiceProbe(connectionInfo, config, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildWmiPerfOSProcessorProbe(SqlConnectionInfo connectionInfo, WmiConfiguration config, int? cloudProviderId)
        {
            WmiPerfOSProcessorProbe srvProbe = new WmiPerfOSProcessorProbe(connectionInfo, config, cloudProviderId);
            return srvProbe;
        }
        //
        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildQueryPlanProbe(SqlConnectionInfo connectionInfo, string query, int? cloudProviderId)
        {
            QueryPlanProbe srvProbe = new QueryPlanProbe(connectionInfo, query, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// SQLdm 10.4 Nikhil Bansal - On Demand Collection of Estimated Query Plan
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="config">Cloud Provider server information</param>
        /// <returns></returns>
        public static IProbe BuildEstimatedQueryPlanProbe(SqlConnectionInfo connectionInfo, EstimatedQueryPlanConfiguration config, int? cloudProviderId)
        {
            EstimatedQueryPlanProbe srvProbe = new EstimatedQueryPlanProbe(connectionInfo, config, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildWorstFillFactorIndexesProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
        {
            WorstFillFactorIndexesProbe srvProbe = new WorstFillFactorIndexesProbe(connectionInfo, cloudProviderId);
            return srvProbe;
        }


        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildDatabaseRankingProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
        {
            DatabaseRankingProbe srvProbe = new DatabaseRankingProbe(connectionInfo, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildOverlappingIndexesProbe(SqlConnectionInfo connectionInfo, string db, int? cloudProviderId)
        {
            if (!string.IsNullOrWhiteSpace(db))
            {
                connectionInfo.DatabaseName = db;
            }
            OverlappingIndexesProbe srvProbe = new OverlappingIndexesProbe(connectionInfo, db, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildIndexContentionProbe(SqlConnectionInfo connectionInfo, string db, int? cloudProviderId)
        {
            if (!string.IsNullOrWhiteSpace(db))
            {
                connectionInfo.DatabaseName = db;
            }
            IndexContentionProbe srvProbe = new IndexContentionProbe(connectionInfo, db, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildHighIndexUpdatesProbe(SqlConnectionInfo connectionInfo, string db, int? cloudProviderId)
        {
            if (!string.IsNullOrWhiteSpace(db))
            {
                connectionInfo.DatabaseName = db;
            }
            HighIndexUpdatesProbe srvProbe = new HighIndexUpdatesProbe(connectionInfo, db, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildFragmentedIndexesProbe(SqlConnectionInfo connectionInfo, string db, int? cloudProviderId)
        {
            if (!string.IsNullOrWhiteSpace(db))
            {
                connectionInfo.DatabaseName = db;
            }
            FragmentedIndexesProbe srvProbe = new FragmentedIndexesProbe(connectionInfo, db, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildAdhocCachedPlanBytesProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
        {
            AdhocCachedPlanBytesProbe srvProbe = new AdhocCachedPlanBytesProbe(connectionInfo, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildLockedPageKBProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
        {
            LockedPageKBProbe srvProbe = new LockedPageKBProbe(connectionInfo, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildServerConfigurationProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
        {
            ServerConfigurationProbe srvProbe = new ServerConfigurationProbe(connectionInfo, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildWaitingBatchesProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
        {
            return BuildWaitingBatchesProbe(connectionInfo, false, 0, 0, cloudProviderId);
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildWaitingBatchesProbe(SqlConnectionInfo connectionInfo, bool isInterval, int interval, int max, int? cloudProviderId)
        {
            if (isInterval)
            {
                WaitingBatchesProbe srvProbe = new WaitingBatchesProbe(connectionInfo, interval, max, cloudProviderId);
                return srvProbe;
            }
            else
            {
                WaitingBatchesProbe srvProbe = new WaitingBatchesProbe(connectionInfo, cloudProviderId);
                return srvProbe;
            }
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildDatabaseNamesProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
        {
            DatabaseNamesProbe srvProbe = new DatabaseNamesProbe(connectionInfo, cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Praveen SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <returns></returns>
        public static IProbe BuildMachineNameProbe(SqlConnectionInfo connectionInfo,int? cloudProviderId = null)
        {
            MachineNameProbe srvProbe = new MachineNameProbe(connectionInfo,cloudProviderId);
            return srvProbe;
        }

        /// <summary>
        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildPrescriptiveOptimizationProbe(SqlConnectionInfo connectionInfo, PrescriptiveScriptConfiguration configuration, int? cloudProviderId)
        {
            PrescriptiveOptimizationProbe srvProbe = new PrescriptiveOptimizationProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }
        // <summary>
        /// Srishti Purohit SQLdm 10.0 - Doctor integration-- Learn more link for dependent objects
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildDependentObjectProbe(SqlConnectionInfo connectionInfo, Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.DatabaseObjectName ObjectName, int? cloudProviderId)
        {
            DependentObjectProbe srvProbe = new DependentObjectProbe(connectionInfo, ObjectName, cloudProviderId);
            return srvProbe;
        }

        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildDBSecurityProbe(SqlConnectionInfo connectionInfo, string db, int? cloudProviderId)
        {
            if (!string.IsNullOrWhiteSpace(db))
            {
                // Set the database name
                connectionInfo.DatabaseName = db;
            }

            DBSecurityProbe srvProbe = new DBSecurityProbe(connectionInfo, db, cloudProviderId);
            return srvProbe;
        }

        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildMasterFilesProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
        {
            GetMasterFilesProbe srvProbe = new GetMasterFilesProbe(connectionInfo, cloudProviderId);
            return srvProbe;
        }

        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildSqlModuleOptionsProbe(SqlConnectionInfo connectionInfo, string db, int? cloudProviderId)
        {
            if (!string.IsNullOrWhiteSpace(db))
            {
                connectionInfo.DatabaseName = db;
            }
            SQLModuleOptionsProbe srvProbe = new SQLModuleOptionsProbe(connectionInfo, db, cloudProviderId);
            return srvProbe;
        }

        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildSampleServerResourcesProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
        {
            return BuildSampleServerResourcesProbe(connectionInfo, false, 0, 0, cloudProviderId);
        }

        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildSampleServerResourcesProbe(SqlConnectionInfo connectionInfo, bool isInterval, int interval, int max, int? cloudProviderId)
        {
            if (isInterval)
            {
                SampleServerResourcesProbe srvProbe = new SampleServerResourcesProbe(connectionInfo, interval, max, cloudProviderId);
                return srvProbe;
            }
            else
            {
                SampleServerResourcesProbe srvProbe = new SampleServerResourcesProbe(connectionInfo, cloudProviderId);
                return srvProbe;
            }
        }

        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildQueryPlanEstRowsProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
        {
            QueryPlanEstRowsProbe srvProbe = new QueryPlanEstRowsProbe(connectionInfo, cloudProviderId);
            return srvProbe;
        }

        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildOutOfDateStatsProbe(SqlConnectionInfo connectionInfo, string db, int? cloudProviderId)
        {
            if (!string.IsNullOrWhiteSpace(db))
            {
                connectionInfo.DatabaseName = db;
            }
            OutOfDateStatsProbe srvProbe = new OutOfDateStatsProbe(connectionInfo, db, cloudProviderId);
            return srvProbe;
        }

        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildNUMANodeCountersProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
        {
            NUMANodeCountersProbe srvProbe = new NUMANodeCountersProbe(connectionInfo, cloudProviderId);
            return srvProbe;
        }

        /// Vineet SQLdm 10.0 - Doctor integration
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildBackupAndRecoveryProbe(SqlConnectionInfo connectionInfo, string db, int? cloudProviderId)
        {
            if (!string.IsNullOrWhiteSpace(db))
            {
                connectionInfo.DatabaseName = db;
            }
            BackupAndRecoveryProbe srvProbe = new BackupAndRecoveryProbe(connectionInfo, db, cloudProviderId);
            return srvProbe;
        }

        /// Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-I23
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildNonIncrementalColStatsProbe(SqlConnectionInfo connectionInfo, string db, int? cloudProviderId)
        {
            if (!string.IsNullOrWhiteSpace(db))
            {
                connectionInfo.DatabaseName = db;
            }
            NonIncrementalColumnStatProbe srvProbe = new NonIncrementalColumnStatProbe(connectionInfo, db, cloudProviderId);
            return srvProbe;
        }
        /// Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-I25, 26, 28
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildHashIndexProbe(SqlConnectionInfo connectionInfo, string db, int? cloudProviderId)
        {
            if (!string.IsNullOrWhiteSpace(db))
            {
                connectionInfo.DatabaseName = db;
            }
            HashIndexProbe srvProbe = new HashIndexProbe(connectionInfo, db, cloudProviderId);
            return srvProbe;
        }
        /// Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-Q39,Q40,Q41, Q42
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildQueryStoreProbe(SqlConnectionInfo connectionInfo, string db, int? cloudProviderId)
        {
            if (!string.IsNullOrWhiteSpace(db))
            {
                connectionInfo.DatabaseName = db;
            }
            QueryStoreProbe srvProbe = new QueryStoreProbe(connectionInfo, db, cloudProviderId);
            return srvProbe;
        }
        /// Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-I29
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildRarelyUsedIndexOnInMemoryTableProbe(SqlConnectionInfo connectionInfo, string db, int? cloudProviderId)
        {
            if (!string.IsNullOrWhiteSpace(db))
            {
                connectionInfo.DatabaseName = db;
            }
            RarelyUsedIndexOnInMemoryTableProbe srvProbe = new RarelyUsedIndexOnInMemoryTableProbe(connectionInfo, db, cloudProviderId);
            return srvProbe;
        }
        /// Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-Q46,Q47,Q48,Q49,Q50
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildQueryAnalyzerProbe(SqlConnectionInfo connectionInfo, string db, int? cloudProviderId)
        {
            if (!string.IsNullOrWhiteSpace(db))
            {
                connectionInfo.DatabaseName = db;
            }
            QueryAnalyzerProbe srvProbe = new QueryAnalyzerProbe(connectionInfo, db, cloudProviderId);
            return srvProbe;
        }
        /// Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-I30
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildColumnStoreIndexProbe(SqlConnectionInfo connectionInfo, string db, int? cloudProviderId)
        {
            if (!string.IsNullOrWhiteSpace(db))
            {
                connectionInfo.DatabaseName = db;
            }
            ColumnStoreIndexProbe srvProbe = new ColumnStoreIndexProbe(connectionInfo, db, cloudProviderId);
            return srvProbe;
        }
        /// Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-I31
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildFilteredColumnNotInKeyOfFilteredIndexProbe(SqlConnectionInfo connectionInfo, string db, int? cloudProviderId)
        {
            if (!string.IsNullOrWhiteSpace(db))
            {
                connectionInfo.DatabaseName = db;
            }
            FilteredColumnNotInKeyOfFilteredIndexProbe srvProbe = new FilteredColumnNotInKeyOfFilteredIndexProbe(connectionInfo, db, cloudProviderId);
            return srvProbe;
        }
        /// Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-Q43
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildHighCPUTimeProcedureProbe(SqlConnectionInfo connectionInfo, string db, int? cloudProviderId)
        {
            if (!string.IsNullOrWhiteSpace(db))
            {
                connectionInfo.DatabaseName = db;
            }
            HighCPUTimeProcedureProbe srvProbe = new HighCPUTimeProcedureProbe(connectionInfo, db, cloudProviderId);
            return srvProbe;
        }
        /// Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-I24
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildLargeTableStatsProbe(SqlConnectionInfo connectionInfo, string db, int? cloudProviderId)
        {
            if (!string.IsNullOrWhiteSpace(db))
            {
                connectionInfo.DatabaseName = db;
            }
            LargeTableStatsProbe srvProbe = new LargeTableStatsProbe(connectionInfo, db, cloudProviderId);
            return srvProbe;
        }
        /// Srishti Purohit SQLdm 10.0 - New Recommendations - SDR-M33
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildBufferPoolExtIOProbe(SqlConnectionInfo connectionInfo, int interval, int max, int? cloudProviderId)
        {
            BufferPoolExtIOProbe srvProbe = new BufferPoolExtIOProbe(connectionInfo, interval, max, cloudProviderId);
            return srvProbe;
        }
        #endregion

        #region Server Actions

        /// <summary>
        /// Builds the probe to perform an action on the monitored server
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        /// <returns></returns>
        public static IProbe BuildServerActionProbe(SqlConnectionInfo connectionInfo, IServerActionConfiguration configuration, int? cloudProviderId)
        {
            ServerActionProbe srvProbe = new ServerActionProbe(connectionInfo, configuration, cloudProviderId);
            return srvProbe;
        }

        public static IProbe BuildWmiConfigurationTestProbe(SqlConnectionInfo connectionInfo, TestWmiConfiguration configuration, int? cloudProviderId)
        {
            var probe = new WmiConnectionTestProbe(connectionInfo, configuration, cloudProviderId);
            return probe;
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
