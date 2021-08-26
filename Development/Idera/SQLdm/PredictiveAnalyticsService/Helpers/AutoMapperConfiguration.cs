using AutoMapper;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;
using PASnapshots = Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Snapshots;
using Idera.SQLdm.Common.Snapshots;
using System;
using System.Globalization;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Values;

namespace Idera.SQLdm.PredictiveAnalyticsService.Helpers
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(config =>
            {
                config.AddProfile<SnapshotMapping>();
                config.AddProfile<ResultMapping>();
            });
        }
    }

    public class SnapshotMapping : Profile
    {
        public SnapshotMapping()
        {
            CreateMap<string, int>().ConvertUsing(s => Convert.ToInt32(s, CultureInfo.InvariantCulture));
            CreateMap<int, string>().ConvertUsing(i => i.ToString(CultureInfo.InvariantCulture));

            CreateMap<Snapshot, PASnapshots.PASnapshot>();
            CreateMap<PASnapshots.PASnapshot, Snapshot>();

            CreateMap<PASnapshots.PAFragmentedIndexesSnapshot, FragmentedIndexesSnapshot>();
            CreateMap<PASnapshots.PADisabledIndexesSnapshot, DisabledIndexesSnapshot>();
            CreateMap<PASnapshots.PADBSecuritySnapshot, DBSecuritySnapshot>();
            CreateMap<PASnapshots.PABackupAndRecoverySnapshot, BackupAndRecoverySnapshot>();
            CreateMap<PASnapshots.PASampleServerResourcesSnapshot, SampleServerResourcesSnapshot>();
            CreateMap<PASnapshots.PAWaitingBatchesSnapshot, WaitingBatchesSnapshot>();
            CreateMap<PASnapshots.PAWmiProcessorSnapshot, WmiProcessorSnapshot>();
            CreateMap<PASnapshots.PAWmiPerfOSSystemSnapshot, WmiPerfOSSystemSnapshot>();
            CreateMap<PASnapshots.PAWmiServiceSnapshot, WmiServiceSnapshot>();
            CreateMap<PASnapshots.PAWmiPhysicalMemorySnapshot, WmiPhysicalMemorySnapshot>();
            CreateMap<PASnapshots.PAWmiComputerSystemSnapshot, WmiComputerSystemSnapshot>();
            CreateMap<PASnapshots.PAWmiEncryptableVolumeSnapshot, WmiEncryptableVolumeSnapshot>();
            CreateMap<PASnapshots.PANUMANodeCountersSnapshot, NUMANodeCountersSnapshot>();
            CreateMap<PASnapshots.PAWorstFillFactorIndexesSnapshot, WorstFillFactorIndexesSnapshot>();
            CreateMap<PASnapshots.PAWmiProcessSnapshot, WmiProcessSnapshot>();
            CreateMap<PASnapshots.PASampleServerResourcesSnapshot, SampleServerResourcesSnapshot>();
            CreateMap<PASnapshots.PAHighIndexUpdatesSnapshot, HighIndexUpdatesSnapshot>();
            CreateMap<PASnapshots.PAIndexContentionSnapshot, IndexContentionSnapshot>();
            CreateMap<PASnapshots.PAOutOfDateStatsSnapshot, OutOfDateStatsSnapshot>();
            CreateMap<PASnapshots.PAOverlappingIndexesSnapshot, OverlappingIndexesSnapshot>();
            CreateMap<PASnapshots.PABufferPoolExtIOSnapshot, BufferPoolExtIOSnapshot>();
            CreateMap<PASnapshots.PALargeTableStatsSnapshot, LargeTableStatsSnapshot>();
            CreateMap<PASnapshots.PAHighCPUTimeProcedureSnapshot, HighCPUTimeProcedureSnapshot>();
            CreateMap<PASnapshots.PAFilteredColumnNotInKeyOfFilteredIndexSnapshot, FilteredColumnNotInKeyOfFilteredIndexSnapshot>();
            CreateMap<PASnapshots.PAColumnStoreIndexSnapshot, ColumnStoreIndexSnapshot>();
            CreateMap<PASnapshots.PAQueryAnalyzerSnapshot, QueryAnalyzerSnapshot>();
            CreateMap<PASnapshots.PARarelyUsedIndexOnInMemoryTableSnapshot, RarelyUsedIndexOnInMemoryTableSnapshot>();
            CreateMap<PASnapshots.PAQueryStoreSnapshot, QueryStoreSnapshot>();
            CreateMap<PASnapshots.PAHashIndexSnapshot, HashIndexSnapshot>();
            CreateMap<PASnapshots.PANonIncrementalColumnStatSnapshot, NonIncrementalColumnStatSnapshot>();
            CreateMap<PASnapshots.PADatabaseConfigurationSnapshot, DatabaseConfigurationSnapshot>();
            CreateMap<PASnapshots.PAWaitingBatchesSnapshot, WaitingBatchesSnapshot>();
            CreateMap<PASnapshots.PAQueryPlanEstRowsSnapshot, QueryPlanEstRowsSnapshot>();
            CreateMap<PASnapshots.PALockedPageKBSnapshot, LockedPageKBSnapshot>();
            CreateMap<PASnapshots.PAAdhocCachedPlanBytesSnapshot, AdhocCachedPlanBytesSnapshot>();
            CreateMap<PASnapshots.PADatabaseRankingSnapshot, DatabaseRankingSnapshot>();
            CreateMap<PASnapshots.PASQLModuleOptionsSnapshot, SQLModuleOptionsSnapshot>();
            CreateMap<PASnapshots.PAWmiNetworkInterfaceSnapshot, WmiNetworkInterfaceSnapshot>();
            CreateMap<PASnapshots.PAWmiPageFileSnapshot, WmiPageFileSnapshot>();
            CreateMap<PASnapshots.PAWmiPerfDiskPhysicalDiskSnapshot, WmiPerfDiskPhysicalDiskSnapshot>();
            CreateMap<PASnapshots.PAWmiPerfDiskLogicalDiskSnapshot, WmiPerfDiskLogicalDiskSnapshot>();
            CreateMap<PASnapshots.PAWmiPageFileSnapshot, WmiPageFileSnapshot>();
            CreateMap<PASnapshots.PAServerConfigurationSnapshot, ServerConfigurationSnapshot>();
            CreateMap<PASnapshots.PASampleServerResourcesSnapshot, SampleServerResourcesSnapshot>();
            CreateMap<PASnapshots.PAConfigurationSnapshot, ConfigurationSnapshot>();
            CreateMap<PASnapshots.PAWaitStatisticsSnapshot, WaitStatisticsSnapshot>();
            CreateMap<PASnapshots.PAWmiVolumeSnapshot, WmiVolumeSnapshot>();
            CreateMap<PASnapshots.PAWmiPerfOSProcessorSnapshot, WmiPerfOSProcessorSnapshot>();
            CreateMap<PASnapshots.PAWmiPerfOSSystemSnapshot, WmiPerfOSSystemSnapshot>();
            CreateMap<PASnapshots.PAWmiPerfDiskLogicalDiskSnapshot, WmiPerfDiskLogicalDiskSnapshot>();
            CreateMap<PASnapshots.PAWmiNetworkInterfaceSnapshot, WmiNetworkInterfaceSnapshot>();
            CreateMap<PASnapshots.PAWmiTCPv4Snapshot, WmiTCPv4Snapshot>();
            CreateMap<PASnapshots.PAWmiTCPv6Snapshot, WmiTCPv6Snapshot>();
            CreateMap<PASnapshots.PAWmiTCPSnapshot, WmiTCPSnapshot>();
            CreateMap<PASnapshots.PAWmiNetworkRedirectorSnapshot, WmiNetworkRedirectorSnapshot>();
            CreateMap<PASnapshots.PAWmiPerfDiskPhysicalDiskSnapshot, WmiPerfDiskPhysicalDiskSnapshot>();
            CreateMap<PASnapshots.PAGetMasterFilesSnapshot, GetMasterFilesSnapshot>();
            CreateMap<PASnapshots.PAWmiPerfOSProcessorSnapshot, WmiPerfOSProcessorSnapshot>();
            CreateMap<PASnapshots.PAWmiPerfOSMemorySnapshot, WmiPerfOSMemorySnapshot>();
            CreateMap<PASnapshots.PAWmiNetworkRedirectorSnapshot, WmiNetworkRedirectorSnapshot>();
            CreateMap<PASnapshots.PAWmiTCPv4Snapshot, WmiTCPv4Snapshot>();
            CreateMap<PASnapshots.PAWmiTCPv6Snapshot, WmiTCPv6Snapshot>();
            CreateMap<PASnapshots.PAWmiTCPSnapshot, WmiTCPSnapshot>();

            CreateMap<FragmentedIndexesSnapshot, PASnapshots.PAFragmentedIndexesSnapshot>().ConstructUsing(ctor => new PASnapshots.PAFragmentedIndexesSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<DisabledIndexesSnapshot, PASnapshots.PADisabledIndexesSnapshot>().ConstructUsing(ctor => new PASnapshots.PADisabledIndexesSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<DBSecuritySnapshot, PASnapshots.PADBSecuritySnapshot>().ConstructUsing(ctor => new PASnapshots.PADBSecuritySnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo, string.Empty));
            CreateMap<BackupAndRecoverySnapshot, PASnapshots.PABackupAndRecoverySnapshot>().ConstructUsing(ctor => new PASnapshots.PABackupAndRecoverySnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<SampleServerResourcesSnapshot, PASnapshots.PASampleServerResourcesSnapshot>().ConstructUsing(ctor => new PASnapshots.PASampleServerResourcesSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<WaitingBatchesSnapshot, PASnapshots.PAWaitingBatchesSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWaitingBatchesSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<WmiProcessorSnapshot, PASnapshots.PAWmiProcessorSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiProcessorSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiPerfOSSystemSnapshot, PASnapshots.PAWmiPerfOSSystemSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiPerfOSSystemSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiServiceSnapshot, PASnapshots.PAWmiServiceSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiServiceSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiPhysicalMemorySnapshot, PASnapshots.PAWmiPhysicalMemorySnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiPhysicalMemorySnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiComputerSystemSnapshot, PASnapshots.PAWmiComputerSystemSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiComputerSystemSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiEncryptableVolumeSnapshot, PASnapshots.PAWmiEncryptableVolumeSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiEncryptableVolumeSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<NUMANodeCountersSnapshot, PASnapshots.PANUMANodeCountersSnapshot>().ConstructUsing(ctor => new PASnapshots.PANUMANodeCountersSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<WorstFillFactorIndexesSnapshot, PASnapshots.PAWorstFillFactorIndexesSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWorstFillFactorIndexesSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<WmiProcessSnapshot, PASnapshots.PAWmiProcessSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiProcessSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<SampleServerResourcesSnapshot, PASnapshots.PASampleServerResourcesSnapshot>().ConstructUsing(ctor => new PASnapshots.PASampleServerResourcesSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<HighIndexUpdatesSnapshot, PASnapshots.PAHighIndexUpdatesSnapshot>().ConstructUsing(ctor => new PASnapshots.PAHighIndexUpdatesSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<IndexContentionSnapshot, PASnapshots.PAIndexContentionSnapshot>().ConstructUsing(ctor => new PASnapshots.PAIndexContentionSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<OutOfDateStatsSnapshot, PASnapshots.PAOutOfDateStatsSnapshot>().ConstructUsing(ctor => new PASnapshots.PAOutOfDateStatsSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<OverlappingIndexesSnapshot, PASnapshots.PAOverlappingIndexesSnapshot>().ConstructUsing(ctor => new PASnapshots.PAOverlappingIndexesSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<BufferPoolExtIOSnapshot, PASnapshots.PABufferPoolExtIOSnapshot>().ConstructUsing(ctor => new PASnapshots.PABufferPoolExtIOSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<LargeTableStatsSnapshot, PASnapshots.PALargeTableStatsSnapshot>().ConstructUsing(ctor => new PASnapshots.PALargeTableStatsSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<HighCPUTimeProcedureSnapshot, PASnapshots.PAHighCPUTimeProcedureSnapshot>().ConstructUsing(ctor => new PASnapshots.PAHighCPUTimeProcedureSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<FilteredColumnNotInKeyOfFilteredIndexSnapshot, PASnapshots.PAFilteredColumnNotInKeyOfFilteredIndexSnapshot>().ConstructUsing(ctor => new PASnapshots.PAFilteredColumnNotInKeyOfFilteredIndexSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<ColumnStoreIndexSnapshot, PASnapshots.PAColumnStoreIndexSnapshot>().ConstructUsing(ctor => new PASnapshots.PAColumnStoreIndexSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<QueryAnalyzerSnapshot, PASnapshots.PAQueryAnalyzerSnapshot>().ConstructUsing(ctor => new PASnapshots.PAQueryAnalyzerSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<RarelyUsedIndexOnInMemoryTableSnapshot, PASnapshots.PARarelyUsedIndexOnInMemoryTableSnapshot>().ConstructUsing(ctor => new PASnapshots.PARarelyUsedIndexOnInMemoryTableSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<QueryStoreSnapshot, PASnapshots.PAQueryStoreSnapshot>().ConstructUsing(ctor => new PASnapshots.PAQueryStoreSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<HashIndexSnapshot, PASnapshots.PAHashIndexSnapshot>().ConstructUsing(ctor => new PASnapshots.PAHashIndexSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<NonIncrementalColumnStatSnapshot, PASnapshots.PANonIncrementalColumnStatSnapshot>().ConstructUsing(ctor => new PASnapshots.PANonIncrementalColumnStatSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<DatabaseConfigurationSnapshot, PASnapshots.PADatabaseConfigurationSnapshot>().ConstructUsing(ctor => new PASnapshots.PADatabaseConfigurationSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<WaitingBatchesSnapshot, PASnapshots.PAWaitingBatchesSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWaitingBatchesSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<QueryPlanEstRowsSnapshot, PASnapshots.PAQueryPlanEstRowsSnapshot>().ConstructUsing(ctor => new PASnapshots.PAQueryPlanEstRowsSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<LockedPageKBSnapshot, PASnapshots.PALockedPageKBSnapshot>().ConstructUsing(ctor => new PASnapshots.PALockedPageKBSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<AdhocCachedPlanBytesSnapshot, PASnapshots.PAAdhocCachedPlanBytesSnapshot>().ConstructUsing(ctor => new PASnapshots.PAAdhocCachedPlanBytesSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<DatabaseRankingSnapshot, PASnapshots.PADatabaseRankingSnapshot>().ConstructUsing(ctor => new PASnapshots.PADatabaseRankingSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<SQLModuleOptionsSnapshot, PASnapshots.PASQLModuleOptionsSnapshot>().ConstructUsing(ctor => new PASnapshots.PASQLModuleOptionsSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<WmiNetworkInterfaceSnapshot, PASnapshots.PAWmiNetworkInterfaceSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiNetworkInterfaceSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiPageFileSnapshot, PASnapshots.PAWmiPageFileSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiPageFileSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiPerfDiskPhysicalDiskSnapshot, PASnapshots.PAWmiPerfDiskPhysicalDiskSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiPerfDiskPhysicalDiskSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiPerfDiskLogicalDiskSnapshot, PASnapshots.PAWmiPerfDiskLogicalDiskSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiPerfDiskLogicalDiskSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiPageFileSnapshot, PASnapshots.PAWmiPageFileSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiPageFileSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<ServerConfigurationSnapshot, PASnapshots.PAServerConfigurationSnapshot>().ConstructUsing(ctor => new PASnapshots.PAServerConfigurationSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<SampleServerResourcesSnapshot, PASnapshots.PASampleServerResourcesSnapshot>().ConstructUsing(ctor => new PASnapshots.PASampleServerResourcesSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<ConfigurationSnapshot, PASnapshots.PAConfigurationSnapshot>().ConstructUsing(ctor => new PASnapshots.PAConfigurationSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<WaitStatisticsSnapshot, PASnapshots.PAWaitStatisticsSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWaitStatisticsSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<WmiVolumeSnapshot, PASnapshots.PAWmiVolumeSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiVolumeSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiPerfOSProcessorSnapshot, PASnapshots.PAWmiPerfOSProcessorSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiPerfOSProcessorSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiPerfOSSystemSnapshot, PASnapshots.PAWmiPerfOSSystemSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiPerfOSSystemSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiPerfDiskLogicalDiskSnapshot, PASnapshots.PAWmiPerfDiskLogicalDiskSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiPerfDiskLogicalDiskSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiNetworkInterfaceSnapshot, PASnapshots.PAWmiNetworkInterfaceSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiNetworkInterfaceSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiTCPv4Snapshot, PASnapshots.PAWmiTCPv4Snapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiTCPv4Snapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiTCPv6Snapshot, PASnapshots.PAWmiTCPv6Snapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiTCPv6Snapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiTCPSnapshot, PASnapshots.PAWmiTCPSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiTCPSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiNetworkRedirectorSnapshot, PASnapshots.PAWmiNetworkRedirectorSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiNetworkRedirectorSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiPerfDiskPhysicalDiskSnapshot, PASnapshots.PAWmiPerfDiskPhysicalDiskSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiPerfDiskPhysicalDiskSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<GetMasterFilesSnapshot, PASnapshots.PAGetMasterFilesSnapshot>().ConstructUsing(ctor => new PASnapshots.PAGetMasterFilesSnapshot(PrescriptiveAnalysisService.PASqlConnectionInfo));
            CreateMap<WmiPerfOSProcessorSnapshot, PASnapshots.PAWmiPerfOSProcessorSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiPerfOSProcessorSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiPerfOSMemorySnapshot, PASnapshots.PAWmiPerfOSMemorySnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiPerfOSMemorySnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiNetworkRedirectorSnapshot, PASnapshots.PAWmiNetworkRedirectorSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiNetworkRedirectorSnapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiTCPv4Snapshot, PASnapshots.PAWmiTCPv4Snapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiTCPv4Snapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiTCPv6Snapshot, PASnapshots.PAWmiTCPv6Snapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiTCPv6Snapshot(PrescriptiveAnalysisService.ServerName));
            CreateMap<WmiTCPSnapshot, PASnapshots.PAWmiTCPSnapshot>().ConstructUsing(ctor => new PASnapshots.PAWmiTCPSnapshot(PrescriptiveAnalysisService.ServerName));

            CreateMap<PrescriptiveAnalyticsSnapshot, PASnapshots.PAPrescriptiveAnalyticsSnapshot>()
                .ForMember(cfg => cfg.FragmentedIndexesSnapshotList, cfg => cfg.MapFrom(src => src.FragmentedIndexesSnapshotList))
                .ForMember(cfg => cfg.DisabledIndexesSnapshotList, cfg => cfg.MapFrom(src => src.DisabledIndexesSnapshotList))
                .ForMember(cfg => cfg.DBSecuritySnapshotList, cfg => cfg.MapFrom(src => src.DBSecuritySnapshotList))
                .ForMember(cfg => cfg.BackupAndRecoverySnapshotList, cfg => cfg.MapFrom(src => src.BackupAndRecoverySnapshotList))
                //.ForMember(cfg => cfg.BlockingProcessSnapshotValueInterval, cfg => cfg.MapFrom(src => src.BlockingProcessSnapshotValueInterval))
                .ForMember(cfg => cfg.SampleServerResourcesSnapshotValueInterval, cfg => cfg.MapFrom(src => src.SampleServerResourcesSnapshotValueInterval))
                .ForMember(cfg => cfg.WaitingBatchesSnapshotValueInterval, cfg => cfg.MapFrom(src => src.WaitingBatchesSnapshotValueInterval))
                .ForMember(cfg => cfg.WmiProcessorSnapshotValueInterval, cfg => cfg.MapFrom(src => src.WmiProcessorSnapshotValueInterval))
                .ForMember(cfg => cfg.WmiPerfOSSystemSnapshotValueInterval, cfg => cfg.MapFrom(src => src.WmiPerfOSSystemSnapshotValueInterval))
                //.ForMember(cfg => cfg.DeadlockSnapshotValueShutdown, cfg => cfg.MapFrom(src => src.DeadlockSnapshotValueShutdown))
                .ForMember(cfg => cfg.WmiServiceSnapshotValueShutdown, cfg => cfg.MapFrom(src => src.WmiServiceSnapshotValueShutdown))
                .ForMember(cfg => cfg.WmiPhysicalMemorySnapshotValueShutdown, cfg => cfg.MapFrom(src => src.WmiPhysicalMemorySnapshotValueShutdown))
                //.ForMember(cfg => cfg.WmiBiosSnapshotShutDown, cfg => cfg.MapFrom(src => src.WmiBiosSnapshotShutDown))
                .ForMember(cfg => cfg.WmiComputerSystemSnapshotValueShutdown, cfg => cfg.MapFrom(src => src.WmiComputerSystemSnapshotValueShutdown))
                .ForMember(cfg => cfg.WmiEncryptableVolumeSnapshotValueShutdown, cfg => cfg.MapFrom(src => src.WmiEncryptableVolumeSnapshotValueShutdown))
                .ForMember(cfg => cfg.NUMANodeCountersSnapshotValueShutdown, cfg => cfg.MapFrom(src => src.NUMANodeCountersSnapshotValueShutdown))
                .ForMember(cfg => cfg.WorstFillFactorIndexesSnapshotValueShutdown, cfg => cfg.MapFrom(src => src.WorstFillFactorIndexesSnapshotValueShutdown))
                .ForMember(cfg => cfg.WmiProcessSnapshotValueShutdown, cfg => cfg.MapFrom(src => src.WmiProcessSnapshotValueShutdown))
                .ForMember(cfg => cfg.SampleServerResourcesSnapshotValueShutdown, cfg => cfg.MapFrom(src => src.SampleServerResourcesSnapshotValueShutdown))
                .ForMember(cfg => cfg.HighIndexUpdatesSnapshotList, cfg => cfg.MapFrom(src => src.HighIndexUpdatesSnapshotList))
                .ForMember(cfg => cfg.IndexContentionSnapshotList, cfg => cfg.MapFrom(src => src.IndexContentionSnapshotList))
                .ForMember(cfg => cfg.OutOfDateStatsSnapshotList, cfg => cfg.MapFrom(src => src.OutOfDateStatsSnapshotList))
                .ForMember(cfg => cfg.OverlappingIndexesSnapshotList, cfg => cfg.MapFrom(src => src.OverlappingIndexesSnapshotList))
                .ForMember(cfg => cfg.BufferPoolExtIOSnapshotValueInterval, cfg => cfg.MapFrom(src => src.BufferPoolExtIOSnapshotValueInterval))
                .ForMember(cfg => cfg.LargeTableStatsSnapshotList, cfg => cfg.MapFrom(src => src.LargeTableStatsSnapshotList))
                .ForMember(cfg => cfg.HighCPUTimeProcedureSnapshotList, cfg => cfg.MapFrom(src => src.HighCPUTimeProcedureSnapshotList))
                .ForMember(cfg => cfg.FilteredIndexSnapshotList, cfg => cfg.MapFrom(src => src.FilteredIndexSnapshotList))
                .ForMember(cfg => cfg.ColumnStoreIndexSnapshotList, cfg => cfg.MapFrom(src => src.ColumnStoreIndexSnapshotList))
                .ForMember(cfg => cfg.QueryAnalyzerSnapshotList, cfg => cfg.MapFrom(src => src.QueryAnalyzerSnapshotList))
                .ForMember(cfg => cfg.RarelyUsedIndexOnInMemoryTableSnapshotList, cfg => cfg.MapFrom(src => src.RarelyUsedIndexOnInMemoryTableSnapshotList))
                .ForMember(cfg => cfg.QueryStoreSnapshotList, cfg => cfg.MapFrom(src => src.QueryStoreSnapshotList))
                .ForMember(cfg => cfg.HashIndexSnapshotList, cfg => cfg.MapFrom(src => src.HashIndexSnapshotList))
                .ForMember(cfg => cfg.NonIncrementalColStatsSnapshotList, cfg => cfg.MapFrom(src => src.NonIncrementalColStatsSnapshotList))
                .ForMember(cfg => cfg.DatabaseConfigurationSnapshotValue, cfg => cfg.MapFrom(src => src.DatabaseConfigurationSnapshotValue))
                .ForMember(cfg => cfg.WaitingBatchesSnapshotValueStartup, cfg => cfg.MapFrom(src => src.WaitingBatchesSnapshotValueStartup))
                .ForMember(cfg => cfg.QueryPlanEstRowsSnapshotValue, cfg => cfg.MapFrom(src => src.QueryPlanEstRowsSnapshotValue))
                .ForMember(cfg => cfg.LockedPageKBSnapshotValue, cfg => cfg.MapFrom(src => src.LockedPageKBSnapshotValue))
                .ForMember(cfg => cfg.AdhocCachedPlanBytesSnapshotValue, cfg => cfg.MapFrom(src => src.AdhocCachedPlanBytesSnapshotValue))
                .ForMember(cfg => cfg.DatabaseRankingSnapshotValue, cfg => cfg.MapFrom(src => src.DatabaseRankingSnapshotValue))
                //.ForMember(cfg => cfg.LongRunningSnapshotValueShutDown, cfg => cfg.MapFrom(src => src.LongRunningSnapshotValueShutDown))
                //.ForMember(cfg => cfg.TransactionSnapshotValueShutDown, cfg => cfg.MapFrom(src => src.TransactionSnapshotValueShutDown))
                .ForMember(cfg => cfg.SQLModuleOptionsSnapshotList, cfg => cfg.MapFrom(src => src.SQLModuleOptionsSnapshotList))
                .ForMember(cfg => cfg.WmiNetworkInterfaceSnapshotValueShutdown, cfg => cfg.MapFrom(src => src.WmiNetworkInterfaceSnapshotValueShutdown))
                .ForMember(cfg => cfg.WmiPageFileSnapshotValueShutdown, cfg => cfg.MapFrom(src => src.WmiPageFileSnapshotValueShutdown))
                .ForMember(cfg => cfg.WmiPerfDiskPhysicalDiskSnapshotValueShutdown, cfg => cfg.MapFrom(src => src.WmiPerfDiskPhysicalDiskSnapshotValueShutdown))
                .ForMember(cfg => cfg.WmiPerfDiskLogicalDiskSnapshotValueShutdown, cfg => cfg.MapFrom(src => src.WmiPerfDiskLogicalDiskSnapshotValueShutdown))
                //.ForMember(cfg => cfg.BlockingProcessSnapshotValueStartup, cfg => cfg.MapFrom(src => src.BlockingProcessSnapshotValueStartup))
                .ForMember(cfg => cfg.WmiPageFileSnapshotValueStartup, cfg => cfg.MapFrom(src => src.WmiPageFileSnapshotValueStartup))
                .ForMember(cfg => cfg.ServerConfigurationSnapshotValueStartup, cfg => cfg.MapFrom(src => src.ServerConfigurationSnapshotValueStartup))
                .ForMember(cfg => cfg.SampleServerResourcesSnapshotValueStartup, cfg => cfg.MapFrom(src => src.SampleServerResourcesSnapshotValueStartup))
                .ForMember(cfg => cfg.ConfigurationSnapshotValueStartup, cfg => cfg.MapFrom(src => src.ConfigurationSnapshotValueStartup))
                .ForMember(cfg => cfg.WaitStatisticsSnapshotValueStartup, cfg => cfg.MapFrom(src => src.WaitStatisticsSnapshotValueStartup))
                .ForMember(cfg => cfg.WmiVolumeSnapshotValueStartup, cfg => cfg.MapFrom(src => src.WmiVolumeSnapshotValueStartup))
                .ForMember(cfg => cfg.WmiPerfOSProcessorSnapshotValueStartup, cfg => cfg.MapFrom(src => src.WmiPerfOSProcessorSnapshotValueStartup))
                .ForMember(cfg => cfg.WmiPerfOSSystemSnapshotValueStartup, cfg => cfg.MapFrom(src => src.WmiPerfOSSystemSnapshotValueStartup))
                .ForMember(cfg => cfg.WmiPerfDiskLogicalDiskSnapshotValueStartup, cfg => cfg.MapFrom(src => src.WmiPerfDiskLogicalDiskSnapshotValueStartup))
                .ForMember(cfg => cfg.WmiNetworkInterfaceSnapshotValueStartup, cfg => cfg.MapFrom(src => src.WmiNetworkInterfaceSnapshotValueStartup))
                .ForMember(cfg => cfg.WmiTCPv4SnapshotValueStartup, cfg => cfg.MapFrom(src => src.WmiTCPv4SnapshotValueStartup))
                .ForMember(cfg => cfg.WmiTCPv6SnapshotValueStartup, cfg => cfg.MapFrom(src => src.WmiTCPv6SnapshotValueStartup))
                .ForMember(cfg => cfg.WmiTCPSnapshotValueStartup, cfg => cfg.MapFrom(src => src.WmiTCPSnapshotValueStartup))
                .ForMember(cfg => cfg.WmiNetworkRedirectorSnapshotValueStartup, cfg => cfg.MapFrom(src => src.WmiNetworkRedirectorSnapshotValueStartup))
                .ForMember(cfg => cfg.WmiPerfDiskPhysicalDiskSnapshotValueStartup, cfg => cfg.MapFrom(src => src.WmiPerfDiskPhysicalDiskSnapshotValueStartup))
                .ForMember(cfg => cfg.GetMasterFilesSnapshotValueStartup, cfg => cfg.MapFrom(src => src.GetMasterFilesSnapshotValueStartup))
                .ForMember(cfg => cfg.ConnectionString, cfg => cfg.MapFrom(src => src.ConnectionString))
                .ForMember(cfg => cfg.MachineName, cfg => cfg.MapFrom(src => src.MachineName))
                //.ForMember(cfg => cfg.RDSDBInstanceSnapshotValueStartup, cfg => cfg.MapFrom(src => src.RDSDBInstanceSnapshotValueStartup))
                //.ForMember(cfg => cfg.RDSCPUUtilizationSnapshotValueStartup, cfg => cfg.MapFrom(src => src.RDSCPUUtilizationSnapshotValueStartup))
                //.ForMember(cfg => cfg.RDSInstanceStorageFreeSnapshotValueStartup, cfg => cfg.MapFrom(src => src.RDSInstanceStorageFreeSnapshotValueStartup))
                .ForMember(cfg => cfg.WmiPerfOSProcessorSnapshotValueShutdown, cfg => cfg.MapFrom(src => src.WmiPerfOSProcessorSnapshotValueShutdown))
                .ForMember(cfg => cfg.WmiPerfOSMemorySnapshotValueShutdown, cfg => cfg.MapFrom(src => src.WmiPerfOSMemorySnapshotValueShutdown))
                .ForMember(cfg => cfg.WmiNetworkRedirectorSnapshotValueShutdown, cfg => cfg.MapFrom(src => src.WmiNetworkRedirectorSnapshotValueShutdown))
                .ForMember(cfg => cfg.WmiTCPv4SnapshotValueShutdown, cfg => cfg.MapFrom(src => src.WmiTCPv4SnapshotValueShutdown))
                .ForMember(cfg => cfg.WmiTCPv6SnapshotValueShutdown, cfg => cfg.MapFrom(src => src.WmiTCPv6SnapshotValueShutdown))
                .ForMember(cfg => cfg.WmiTCPSnapshotValueShutdown, cfg => cfg.MapFrom(src => src.WmiTCPSnapshotValueShutdown))
                .IncludeBase<Snapshot, PASnapshots.PASnapshot>()
                .ReverseMap()
                .IncludeBase<PASnapshots.PASnapshot, Snapshot>();
            //.ForMember(cfg => cfg.AzurePublicNetworkAccessSnapshotValueStartup, cfg => cfg.MapFrom(src => src.AzurePublicNetworkAccessSnapshotValueStartup))
            //.ForMember(cfg => cfg.AzureAccessAllIPsSnapshotValueStartup, cfg => cfg.MapFrom(src => src.AzureAccessAllIPsSnapshotValueStartup))
            //.ForMember(cfg => cfg.AzureQueryStoreDefaultsChangedSnapshotValue, cfg => cfg.MapFrom(src => src.AzureQueryStoreDefaultsChangedSnapshotValue))
            //.ForMember(cfg => cfg.AzureGeoReplicatedSecondarysuspendedSnapshotValue, cfg => cfg.MapFrom(src => src.AzureGeoReplicatedSecondarysuspendedSnapshotValue))
            //.ForMember(cfg => cfg.AzureGeoReplicatedSecondarySnapshotValue, cfg => cfg.MapFrom(src => src.AzureGeoReplicatedSecondarySnapshotValue))
            //.ForMember(cfg => cfg.AzureDatabaseUserSessionsSnapshotValueStartup, cfg => cfg.MapFrom(src => src.AzureDatabaseUserSessionsSnapshotValueStartup))
            //.ForMember(cfg => cfg.AzureDatabaseWorkerThreadsSnapshotValueStartup, cfg => cfg.MapFrom(src => src.AzureDatabaseWorkerThreadsSnapshotValueStartup))
            //.ForMember(cfg => cfg.AzureDatabaseStorageCapacitySnapshotValueStartup, cfg => cfg.MapFrom(src => src.AzureDatabaseStorageCapacitySnapshotValueStartup))
            //.ForMember(cfg => cfg.AzureDatabaseDTUCapacitySnapshotValueStartup, cfg => cfg.MapFrom(src => src.AzureDatabaseDTUCapacitySnapshotValueStartup))
            //.ForMember(cfg => cfg.AzureDatabaseServiceTierSnapshotValue, cfg => cfg.MapFrom(src => src.AzureDatabaseServiceTierSnapshotValue))
            //.ForMember(cfg => cfg.EnterpriseDatabaseSnapshotValue, cfg => cfg.MapFrom(src => src.EnterpriseDatabaseSnapshotValue))
            //.ForMember(cfg => cfg.AzureGeoReplicationSnapshotValueStartup, cfg => cfg.MapFrom(src => src.AzureGeoReplicationSnapshotValueStartup))
            //.ForMember(cfg => cfg.RDSDiskQueueDepthSnapshotValueStartup, cfg => cfg.MapFrom(src => src.RDSDiskQueueDepthSnapshotValueStartup))
            //.ForMember(cfg => cfg.RDSInstanceDiskLatencySnapshotValueStartup, cfg => cfg.MapFrom(src => src.RDSInstanceDiskLatencySnapshotValueStartup))

        }
    }


    public class ResultMapping : Profile
    {
        public ResultMapping()
        {
            CreateMap<string, int>().ConvertUsing(s => Convert.ToInt32(s, CultureInfo.InvariantCulture));
            CreateMap<int, string>().ConvertUsing(i => i.ToString(CultureInfo.InvariantCulture));

            //CreateMap<SnapshotValues, PrescriptiveAnalyzer.Common.Values.SnapshotValues>();
            //CreateMap<PrescriptiveAnalyzer.Common.Values.SnapshotValues, SnapshotValues>();

            //CreateMap<AnalyzerResult, PrescriptiveAnalyzer.Common.Recommendations.AnalyzerResult>()
            //    .ForMember(ctx => ctx.AnalyzerID, ctx => ctx.MapFrom(src => src.AnalyzerID))
            //    .ForMember(ctx => ctx.Status, ctx => ctx.MapFrom(src => src.Status))
            //    .ForMember(ctx => ctx.RecommendationCount, ctx => ctx.MapFrom(src => src.RecommendationCount))
            //    .ForMember(ctx => ctx.RecommendationList, ctx => ctx.Ignore())
            //    .ReverseMap();

            //CreateMap<PrescriptiveAnalyzer.Common.Recommendations.Result, Result>()
            //    .ForMember(ctx => ctx.Type, ctx => ctx.MapFrom(src => src.Type))
            //    .ForMember(ctx => ctx.Error, ctx => ctx.MapFrom(src => src.Error))
            //    .ForMember(ctx => ctx.SQLServerID, ctx => ctx.MapFrom(src => src.SQLServerID))
            //    .ForMember(ctx => ctx.AnalysisStartTime, ctx => ctx.MapFrom(src => src.AnalysisStartTime))
            //    .ForMember(ctx => ctx.AnalysisCompleteTime, ctx => ctx.MapFrom(src => src.AnalysisCompleteTime))
            //    .ForMember(ctx => ctx.TotalRecommendationCount, ctx => ctx.MapFrom(src => src.TotalRecommendationCount))
            //    .ForMember(ctx => ctx.LatestSnapshot, ctx => ctx.MapFrom(src => src.LatestSnapshot))
            //    .ForMember(ctx => ctx.AnalyzerRecommendationList, ctx => ctx.MapFrom(src => src.AnalyzerRecommendationList))
            //    .ReverseMap();
        }
    }
}
