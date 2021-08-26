using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Resources;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdoctor.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    public class RecommendationFactory
    {
        public static Recommendation GetRecommendation(string recommendationId, Dictionary<string, string> properties)
        {
            RecommendationType[] recommendationTypes = FindingIdAttribute.GetRecommendationTypes(recommendationId);
            RecommendationType recommendationType;
            if (recommendationTypes.Length < 1)
            {
                return null;
            }
            recommendationType = recommendationTypes[0];
            Recommendation recommendation = null;
            RecommendationProperties recommendationProperties = new RecommendationProperties(properties);
            switch (recommendationType)
            {
                case RecommendationType.Unknown:
                    recommendation = null;
                    break;
                case RecommendationType.DiskQueueLength48:
                    recommendation = new DiskQueueLengthRecommendation1(recommendationProperties);
                    break;
                case RecommendationType.DiskQueueLength49:
                    recommendation = new DiskQueueLengthRecommendation2(recommendationProperties);
                    break;
                case RecommendationType.DiskWaiting:
                    recommendation = new DiskWaitingRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DiskQueueLength50:
                    recommendation = new DiskQueueLengthRecommendation3(recommendationProperties);
                    break;
                case RecommendationType.DiskBlockSize:
                    recommendation = new DiskBlockSizeRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DiskQueueLength52:
                    recommendation = new DiskQueueLengthRecommendation5(recommendationProperties);
                    break;
                case RecommendationType.DiskQueueLength53:
                    recommendation = new DiskQueueLengthRecommendation6(recommendationProperties);
                    break;
                case RecommendationType.DiskNeedSeManageVolumeName:
                    recommendation = new SeManageVolumeNameRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DiskTempDbSizeMismatch:
                    recommendation = new TempDbFileSizeMismatchRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DiskTempDbPresizeSettings:
                    recommendation = new TempDbInitialSizeRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DiskTempDbWaiting:
                    recommendation = new TempDbWaitingRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DiskTempDbNotEnoughFiles:
                    recommendation = null;// not used
                    break;
                case RecommendationType.DiskTempDbTooManyFiles:
                    recommendation = new TempDbTooManyFilesRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DiskEnableCompression:
                    recommendation = null;// not used
                    break;
                case RecommendationType.DiskTempDbRecoveryModel:
                    recommendation = new TempDbRecoveryModelRecommendation(recommendationProperties);
                    break;
                case RecommendationType.Disk16:
                    recommendation = null;// not used
                    break;
                case RecommendationType.FillFactor:
                    recommendation = new FillFactorRecommendation(recommendationProperties);
                    break;
                case RecommendationType.Disk18:
                    recommendation = null;// not used
                    break;
                case RecommendationType.AutoShrinkEnabled:
                    recommendation = new DatabaseAutoShrinkRecommendation(recommendationProperties);
                    break;
                case RecommendationType.Disk20:
                    recommendation = null;// not used
                    break;
                case RecommendationType.Disk21:
                    recommendation = null;// not used
                    break;
                case RecommendationType.DiskTempDbAutogrowth:
                    recommendation = new TempDbAutogrowthRecommendation(recommendationProperties);
                    break;
                case RecommendationType.AnsiTopEquivalent:
                    recommendation = new TSqlRecommendation(RecommendationType.AnsiTopEquivalent, recommendationProperties);//TBR
                    break;
                case RecommendationType.DeallocateCursor:
                    recommendation = new TSqlCursorRecommendation(RecommendationType.AnsiTopEquivalent, recommendationProperties);//TBR
                    break;
                case RecommendationType.FastForwardCursor:
                    recommendation = new TSqlCursorRecommendation(RecommendationType.FastForwardCursor, recommendationProperties);//TBR
                    break;
                case RecommendationType.FunctionInWhereClause:
                    recommendation = new TSqlRecommendation(RecommendationType.FunctionInWhereClause, recommendationProperties);//TBR
                    break;
                case RecommendationType.HintAbuse:
                    recommendation = new TSqlHintRecommendation(RecommendationType.HintAbuse, recommendationProperties);//TBR
                    break;
                case RecommendationType.LeftExpressionInWhereClause:
                    recommendation = new TSqlRecommendation(RecommendationType.LeftExpressionInWhereClause, recommendationProperties);//TBR
                    break;
                case RecommendationType.LikeUseNotNeeded:
                    recommendation = new TSqlRecommendation(RecommendationType.LikeUseNotNeeded, recommendationProperties);//TBR
                    break;
                case RecommendationType.NestedMinMax:
                    recommendation = new TSqlNestedMinMaxRecommendation(RecommendationType.NestedMinMax, recommendationProperties);//TBR
                    break;
                case RecommendationType.NakedInsert:
                    recommendation = new TSqlRecommendation(RecommendationType.NakedInsert, recommendationProperties);//TBR
                    break;
                case RecommendationType.SelectStarAbuse:
                    recommendation = new TSqlRecommendation(RecommendationType.SelectStarAbuse, recommendationProperties);//TBR
                    break;
                case RecommendationType.TopVsRowCount:
                    recommendation = new TSqlRecommendation(RecommendationType.TopVsRowCount, recommendationProperties);//TBR
                    break;
                case RecommendationType.TwoExpressionCoalesce:
                    recommendation = new TSqlRecommendation(RecommendationType.TwoExpressionCoalesce, recommendationProperties);//TBR
                    break;
                case RecommendationType.UnfilteredDelete:
                    recommendation = new TSqlRecommendationWithTable(RecommendationType.UnfilteredDelete, recommendationProperties);//TBR
                    break;
                case RecommendationType.UnionSetAbuse:
                    recommendation = new TSqlRecommendation(RecommendationType.UnionSetAbuse, recommendationProperties);//TBR
                    break;
                case RecommendationType.MissingIndex:
                    recommendation = new MissingIndexRecommendation(recommendationProperties);//TBR
                    break;
                case RecommendationType.NoJoinPredicate:
                    recommendation = new NoJoinPredicateRecommendation(recommendationProperties);//TBR
                    break;
                case RecommendationType.NoColumnStats:
                    recommendation = new NoColumnStatsRecommendation(recommendationProperties);//TBR
                    break;
                case RecommendationType.MemLimitExceededOld:
                    recommendation = null;
                    break;
                case RecommendationType.NetCardLost:
                    recommendation = new NetCardLostRecommendation(recommendationProperties);
                    break;
                case RecommendationType.NetBandwidthLost:
                    recommendation = new NetBandwidthLostRecommendation(recommendationProperties);
                    break;
                case RecommendationType.NetRetranSegs:
                    recommendation = new NetRetranSegsRecommendation(recommendationProperties);
                    break;
                case RecommendationType.NetRedirectorErrors:
                    recommendation = new NetRedirectorErrorsRecommendation(recommendationProperties);
                    break;
                case RecommendationType.NetCardErrors:
                    recommendation = new NetCardErrorsRecommendation(recommendationProperties);
                    break;
                case RecommendationType.NetCongestion:
                    recommendation = new NetCongestionRecommendation(recommendationProperties);
                    break;
                case RecommendationType.NetSSLEncryption:
                    recommendation = null;
                    break;
                case RecommendationType.NetNoCount:
                    recommendation = new NetNoCountRecommendation(recommendationProperties);
                    break;
                case RecommendationType.NetRedundantCards:
                    recommendation = new NetRedundantCardsRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DisabledIndex:
                    recommendation = new DisabledIndexRecommendation(recommendationProperties);
                    break;
                case RecommendationType.HypotheticalIndex:
                    recommendation = new HypotheticalIndexRecommendation(recommendationProperties);
                    break;
                case RecommendationType.FragmentedIndex:
                    recommendation = new FragmentedIndexRecommendation(recommendationProperties);
                    break;
                case RecommendationType.IndexPageLatchContention:
                    recommendation = new IndexPageLatchContentionRecommendation(recommendationProperties);
                    break;
                case RecommendationType.IndexLockContention:
                    recommendation = new IndexLockContentionRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DuplicateIndex:
                    recommendation = new DuplicateIndexRecommendation(recommendationProperties);
                    break;
                case RecommendationType.PartialDuplicateIndex:
                    recommendation = new PartialDuplicateIndexRecommendation(recommendationProperties);
                    break;
                case RecommendationType.IndexUnderutilized:
                    recommendation = new IndexUnderutilizedRecommendation(recommendationProperties);
                    break;
                case RecommendationType.IndexUnused:
                    recommendation = new IndexUnusedRecommendation(recommendationProperties);
                    break;
                case RecommendationType.OutOfDateStats:
                    recommendation = new OutOfDateStatsRecommendation(recommendationProperties);
                    break;
                case RecommendationType.ImplicitConversionInPredicate:
                    recommendation = new ImplicitConversionInPredicateRecommendation(recommendationProperties);//TBR
                    break;
                case RecommendationType.FunctionInPredicate:
                    recommendation = new FunctionInPredicateRecommendation(recommendationProperties);//TBR
                    break;
                case RecommendationType.CachedPlanMisuse:
                    recommendation = new CachedPlanMisuseRecommendation(recommendationProperties);//TBR
                    break;
                case RecommendationType.AutoStats:
                    recommendation = new AutoStatsRecommendation(recommendationProperties);
                    break;
                case RecommendationType.SQLModuleOptions:
                    recommendation = new SQLModuleOptionRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DMVMissingIndex:
                    recommendation = new DMVMissingIndexRecommendation(recommendationProperties);//TBR
                    break;
                case RecommendationType.HighCompilations:
                    recommendation = new HighCompilationsRecommendation(recommendationProperties);
                    break;
                case RecommendationType.CpuAffinity:
                    recommendation = new CpuAffinityRecommendation(recommendationProperties);
                    break;
                case RecommendationType.CpuAffinityReduced:
                    recommendation = new CpuAffinityReducedRecommendation(recommendationProperties);
                    break;
                case RecommendationType.CpuClockSpeedLost:
                    recommendation = new CpuClockSpeedLostRecommendation(recommendationProperties);
                    break;
                case RecommendationType.CpuFullFrequency:
                    recommendation = new CpuFullFrequencyRecommendation(recommendationProperties);
                    break;
                case RecommendationType.CpuConstantFrequency:
                    recommendation = new CpuConstantFrequencyRecommendation(recommendationProperties);
                    break;
                case RecommendationType.CpuMaxDOP:
                    recommendation = new CpuMaxDOPRecommendation(recommendationProperties);
                    break;
                case RecommendationType.CpuEncryptedDatabaseIO:
                    recommendation = null;
                    break;
                case RecommendationType.CpuDatabaseEncryptionAlgorithm:
                    recommendation = null;
                    break;
                case RecommendationType.CpuMultipleInstances:
                    recommendation = null;
                    break;
                case RecommendationType.CpuLightweightPooling:
                    recommendation = null;
                    break;
                case RecommendationType.CpuEncryptedConnections:
                    recommendation = null;
                    break;
                case RecommendationType.CpuInterruptsNetwork:
                    recommendation = new CpuInterruptsNetworkRecommendation(recommendationProperties);
                    break;
                case RecommendationType.CpuInterruptsDisk:
                    recommendation = new CpuInterruptsDiskRecommendation(recommendationProperties);
                    break;
                case RecommendationType.CpuInterrupts:
                    recommendation = new CpuInterruptsRecommendation(recommendationProperties);
                    break;
                case RecommendationType.CpuEncryptedVolume:
                    recommendation = new CpuEncryptedVolumeRecommendation(recommendationProperties);
                    break;
                case RecommendationType.CpuC2Audit:
                    recommendation = null;
                    break;
                case RecommendationType.CpuVirtualMachine:
                    recommendation = null;
                    break;
                case RecommendationType.CpuThemes:
                    recommendation = null;
                    break;
                case RecommendationType.Mem64bitLockPages:
                    recommendation = new Mem64bitLockPagesRecommendation(recommendationProperties);
                    break;
                case RecommendationType.Mem32bit16g:
                    recommendation = new Mem32bit16gRecommendation(recommendationProperties);
                    break;
                case RecommendationType.Mem32bit4to16g:
                    recommendation = new Mem32bit4to16gRecommendation(recommendationProperties);
                    break;
                case RecommendationType.Mem32bit4g:
                    recommendation = new Mem32bit4gRecommendation(recommendationProperties);
                    break;
                case RecommendationType.Mem32bit3to4g:
                    recommendation = new Mem32bit3to4gRecommendation(recommendationProperties);
                    break;
                case RecommendationType.Mem32bit3gOrLess:
                    recommendation = new Mem32bit3gOrLessRecommendation(recommendationProperties);
                    break;
                case RecommendationType.MemStarvation:
                    recommendation = new MemStarvationRecommendation(recommendationProperties);
                    break;
                case RecommendationType.MemPaging:
                    recommendation = new MemPagingRecommendation(recommendationProperties);
                    break;
                case RecommendationType.BlockingProcess:
                    recommendation = null;
                    break;
                case RecommendationType.OpenTransaction:
                    recommendation = new OpenTransactionRecommendation(recommendationProperties);
                    break;
                case RecommendationType.LongRunningJob:
                    recommendation = new LongRunningJobRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DeadlockTraceFlags:
                    recommendation = new DeadlockTraceFlagsRecommendation(recommendationProperties);
                    break;
                case RecommendationType.Deadlock:
                    recommendation = new DeadlockRecommendation(recommendationProperties);
                    break;
                case RecommendationType.MemRunningProcess:
                    recommendation = new MemRunningProcessRecommendation(recommendationProperties);
                    break;
                case RecommendationType.MemFileSharing:
                    recommendation = new MemFileSharingRecommendation(recommendationProperties);
                    break;
                case RecommendationType.MemIncreaseDefaultFillFactor:
                    recommendation = new MemIncreaseDefaultFillFactorRecommendation(recommendationProperties);
                    break;
                case RecommendationType.MemAdjustDefaultFillFactor:
                    recommendation = null;
                    break;
                case RecommendationType.MemDecreaseDefaultFillFactor:
                    recommendation = new MemDecreaseDefaultFillFactorRecommendation(recommendationProperties);
                    break;
                case RecommendationType.MemDecreased:
                    recommendation = new MemDecreasedRecommendation(recommendationProperties);
                    break;
                case RecommendationType.MemMaxServerMemory:
                    recommendation = new MemMaxServerMemoryRecommendation(recommendationProperties);
                    break;
                case RecommendationType.MemUserConnections:
                    recommendation = new MemUserConnectionsRecommendation(recommendationProperties);
                    break;
                case RecommendationType.MemIndexCreation:
                    recommendation = new MemIndexCreationRecommendation(recommendationProperties);
                    break;
                case RecommendationType.MemPerfOfServices:
                    recommendation = new MemPerfOfServicesRecommendation(recommendationProperties);
                    break;
                case RecommendationType.MemPerfOfCache:
                    recommendation = null;
                    break;
                case RecommendationType.MemOptimizeForAdhoc:
                    recommendation = new MemOptimizeForAdhocRecommendation(recommendationProperties);
                    break;
                case RecommendationType.MemTop10Processes:
                    recommendation = new MemTop10ProcessesRecommendation(recommendationProperties);
                    break;
                case RecommendationType.MemDomainController:
                    recommendation = new MemDomainControllerRecommendation(recommendationProperties);
                    break;
                case RecommendationType.MemLimitExceeded:
                    recommendation = new MemLimitExceededRecommendation(recommendationProperties);//TBR
                    break;
                case RecommendationType.NotInUsedOnNullableColumn:
                    recommendation = new TSqlRecommendationWithColumn(RecommendationType.NotInUsedOnNullableColumn, recommendationProperties);//TBR
                    break;
                case RecommendationType.DatabaseConfiguration:
                    recommendation = new DatabaseConfigurationRecommendation(recommendationProperties);
                    break;
                case RecommendationType.PageVerifyWriteable:
                    recommendation = new PageVerifyRecommendation(false, recommendationProperties);
                    break;
                case RecommendationType.PageVerifyReadonly:
                    recommendation = new PageVerifyRecommendation(true, recommendationProperties);
                    break;
                case RecommendationType.DatabaseCompatibility:
                    recommendation = new DatabaseCompatibilityRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DatabaseCollation:
                    recommendation = new DatabaseCollationRecommendation(recommendationProperties);
                    break;
                //Start Server configurations
                case RecommendationType.ServerConfiguration:
                    recommendation = new ServerConfigurationRecommendation(RecommendationType.ServerConfiguration, recommendationProperties);
                    break;
                case RecommendationType.ServerConfigurationRestartRequired:
                    recommendation = new ServerConfigurationRecommendationNoScript(RecommendationType.ServerConfigurationRestartRequired, recommendationProperties);
                    break;
                case RecommendationType.BlockedProcessThreshold:
                    recommendation = new ServerConfigurationRecommendation(RecommendationType.BlockedProcessThreshold, recommendationProperties);
                    break;
                case RecommendationType.LockConfiguration:
                    recommendation = new ServerConfigurationRecommendation(RecommendationType.LockConfiguration, recommendationProperties);
                    break;
                case RecommendationType.DefaultTraceEnabled:
                    recommendation = new ServerConfigurationRecommendation(RecommendationType.DefaultTraceEnabled, recommendationProperties);
                    break;
                case RecommendationType.DisallowResultsFromTriggers:
                    recommendation = new ServerConfigurationRecommendation(RecommendationType.DisallowResultsFromTriggers, recommendationProperties);
                    break;
                case RecommendationType.CLREnabled:
                    recommendation = new ServerConfigurationRecommendation(RecommendationType.CLREnabled, recommendationProperties);
                    break;
                case RecommendationType.McAfeeBufferOverflow:
                    recommendation = new Recommendation(RecommendationType.McAfeeBufferOverflow);
                    break;
                case RecommendationType.NetworkPacketSize:
                    recommendation = new ServerConfigurationRecommendation(RecommendationType.NetworkPacketSize, recommendationProperties);
                    break;
                case RecommendationType.MixedModeAuthentication:
                    recommendation = new MixedModeAuthenticationRecommendation(recommendationProperties);
                    break;
                case RecommendationType.BuiltinAdministratorIsSysadmin:
                    recommendation = new Recommendation(RecommendationType.BuiltinAdministratorIsSysadmin);
                    break;
                case RecommendationType.PublicEnabledSqlAgentProxyAccount:
                    recommendation = new Recommendation(RecommendationType.PublicEnabledSqlAgentProxyAccount);
                    break;
                //End: server configurations
                case RecommendationType.DeprecatedAgentTokenInUse:
                    recommendation = new DeprecatedAgentTokenInUseRecommendation(recommendationProperties);
                    break;
                case RecommendationType.VulnerableSqlLogin:
                    recommendation = new VulnerableSqlLoginRecommendation(recommendationProperties);
                    break;
                //start:dbsecurity analyzers
                case RecommendationType.IsTrustworthyVulnerable:
                    recommendation = new DatabaseWithFixRecommendation(RecommendationType.IsTrustworthyVulnerable, recommendationProperties);
                    break;
                case RecommendationType.SystemDatabaseSymmetricKey:
                    //Database name was getting set to Recommendation ID, corrected
                    recommendation = new DatabaseNoFixRecommendation(RecommendationType.SystemDatabaseSymmetricKey, recommendationProperties);
                    break;
                case RecommendationType.NonSystemDatabaseWeakKey:
                    //Database name was getting set to Recommendation ID, corrected
                    recommendation = new DatabaseNoFixRecommendation(RecommendationType.NonSystemDatabaseWeakKey, recommendationProperties);
                    break;
                case RecommendationType.GuestHasDatabaseAccess:
                    recommendation = new DatabaseWithFixRecommendation(RecommendationType.GuestHasDatabaseAccess, recommendationProperties);
                    break;
                //End:dbsecurity analyzers
                case RecommendationType.CpuDisableParallelism:
                    recommendation = new CpuDisableParallelismRecommendation(recommendationProperties);
                    break;
                case RecommendationType.WaitStatsPageIoLatchCxPacket:
                    recommendation = new WaitStatsPageIoLatchCxPacketRecommendation(recommendationProperties);
                    break;
                case RecommendationType.WaitStatsParallelismThreshold:
                    recommendation = new WaitStatsParallelismThresholdRecommendation(recommendationProperties);
                    break;
                case RecommendationType.WaitStatsMaxDOP:
                    recommendation = new WaitStatsMaxDOPRecommendation(recommendationProperties);
                    break;
                case RecommendationType.WaitStatsHighCXPacket:
                    recommendation = new WaitStatsHighCXPacketRecommendation(recommendationProperties);
                    break;
                case RecommendationType.WaitStatsAsyncNetIO:
                    recommendation = new WaitStatsAsyncNetIORecommendation(recommendationProperties);
                    break;
                case RecommendationType.WaitStatsThreadPool:
                    recommendation = new WaitStatsThreadPoolRecommendation(recommendationProperties);
                    break;
                case RecommendationType.WaitStatsPageIoLatchDatabase:
                    recommendation = new WaitStatsPageIoLatchDatabaseRecommendation(recommendationProperties);
                    break;
                case RecommendationType.WaitStatsPageLatchIndex:
                    recommendation = new WaitStatsPageLatchIndexRecommendation(recommendationProperties);
                    break;
                case RecommendationType.WaitStatsPageLatchTable:
                    recommendation = new WaitStatsPageLatchTableRecommendation(recommendationProperties);
                    break;
                case RecommendationType.WaitStatsPageLatchAllocPageContention:
                    recommendation = new WaitStatsPageLatchAllocPageContentionRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DatabaseSuspectPages:
                    recommendation = new DatabaseSuspectPagesRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DatabaseBackupsOnSameVolume:
                    recommendation = new DatabaseBackupsOnSameVolumeRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DatabaseCheckIntegrity:
                    recommendation = new DatabaseCheckIntegrityRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DatabaseRecoveryModel:
                    recommendation = new DatabaseRecoveryModelRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DatabaseOutdatedBackups:
                    recommendation = new DatabaseOutdatedBackupsRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DatabaseBackupsDeleted:
                    recommendation = new DatabaseBackupsDeletedRecommendation(recommendationProperties);
                    break;
                case RecommendationType.DatabaseNoRecentLogBackup:
                    recommendation = new DatabaseNoRecentLogBackupRecommendation(recommendationProperties);
                    break;
                case RecommendationType.OverlappingIndexes:
                    recommendation = new OverlappingIndexRecommendation(recommendationProperties);
                    break;
                case RecommendationType.NonClusteredMatchingClusteredIndex:
                    recommendation = new NonClusteredMatchingClusteredIndexRecommendation(recommendationProperties);
                    break;
                case RecommendationType.Mem32bit4gSS2012:
                    recommendation = new Mem32bit4gSS2012Recommendation(recommendationProperties);
                    break;
                // SQLdm10.0 -Srishti Purohit -  New Recommendations
                case RecommendationType.Flag4199LowCompatible:
                    recommendation = new Flag4199LowCompatibleRecommendation(recommendationProperties);
                    break;
                case RecommendationType.Flag4199AllDbCompatible:
                    recommendation = new Flag4199AllDbCompatibleRecommendation();
                    break;
                // SQLdm10.0 -Srishti Purohit -  New Recommendations SDR-M31, SDR-M32
                case RecommendationType.BufferPoolExtensionNotUseful:
                    recommendation = new BufferPoolExtensionNotUsefulRecommendation(recommendationProperties);
                    break;
                case RecommendationType.LargeBufferPoolExtensionSize:
                    recommendation = new LargeBufferPoolExtensionSizeRecommendation(recommendationProperties);
                    break;
                //SQLDm 10.0 - Srishti Puohit - New Recommendations - SDR-I23
                case RecommendationType.NonIncrementalColumnStatOnPartitionedTable:
                    recommendation = new NonIncrementalColumnStatRecommendation(recommendationProperties);
                    break;
                //SQLDm 10.0 - Srishti Puohit - New Recommendations - SDR-I25, 26, 27, 28
                case RecommendationType.HashIndexFewBuckets:
                    recommendation = new HashIndexFewBucketsRecommendation(recommendationProperties);
                    break;
                case RecommendationType.HashIndexLargeDuplicateKey:
                    recommendation = new HashIndexLargeDuplicateKeyRecommendation(recommendationProperties);
                    break;
                case RecommendationType.ScannedHashIndex:
                    recommendation = new ScannedHashIndexRecommendation(recommendationProperties);
                    break;
                case RecommendationType.HashIndexTooManyBuckets:
                    recommendation = new HashIndexTooManyBucketsRecommendation(recommendationProperties);
                    break;
                //SQLDm 10.0 - Srishti Puohit - New Recommendations - SDR-Q39, SDR-Q40, SDR-Q41,SDR-Q42
                case RecommendationType.QueryStoreDisabled:
                    recommendation = new QueryStoreDisabledRecommendation(recommendationProperties);
                    break;
                case RecommendationType.QueryStoreAlmostFull:
                    recommendation = new QueryStoreAlmostFullRecommendation(recommendationProperties);
                    break;
                case RecommendationType.PlanGuidesUsedOverQueryStore:
                    recommendation = new PlanGuidesUsedOverQueryStoreRecommendation(recommendationProperties);
                    break;
                case RecommendationType.QueryStoreOutOfSpace:
                    recommendation = new QueryStoreOutOfSpaceRecommendation(recommendationProperties);
                    break;
                //SQLDm 10.0 - Srishti Puohit - New Recommendations - SDR-I29
                case RecommendationType.RarelyUsedIndexOnInMemoryTable:
                    recommendation = new RarelyUsedIndexOnInMemoryTableRecommendation(recommendationProperties);
                    break;
                //SQLDm 10.0 - Srishti Puohit - New Recommendations - SDR-Q46,Q47,Q48,Q49,Q50
                case RecommendationType.Top10QueriesWithLongestAverageExecutionTime:
                    recommendation = new Top10QueriesWithLongestAverageExecutionTimeRecommendation(recommendationProperties);
                    break;
                case RecommendationType.Top10QueriesConsumingMostIO:
                    recommendation = new Top10QueriesConsumingMostIORecommendation(recommendationProperties);
                    break;
                case RecommendationType.QueriesWithDoubleIncreaseInExecutionTime:
                    recommendation = new QueriesWithDoubleIncreaseInExecutionTimeRecommendation(recommendationProperties);
                    break;
                case RecommendationType.Top10QueriesHavingLongerDurationInLastHour:
                    recommendation = new Top10QueriesHavingLongerDurationInLastHourRecommendation(recommendationProperties);
                    break;
                case RecommendationType.QueriesWithFourDifferentPlanInTwoDays:
                    recommendation = new QueriesWithFourDifferentPlanInTwoDaysRecommendation(recommendationProperties);
                    break;
                //SQLDm 10.0 - Srishti Puohit - New Recommendations - SDR-I30
                case RecommendationType.ColumnStoreIndexMissingOnLargeTables:
                    recommendation = new ColumnStoreIndexMissingOnLargeTablesRecommendation(recommendationProperties);
                    break;
                //SQLDm 10.0 - Srishti Puohit - New Recommendations - SDR-I31
                case RecommendationType.FilteredColumnNotInKeyOfFilteredIndex:
                    recommendation = new FilteredColumnNotInKeyOfFilteredIndexRecommendation(recommendationProperties);
                    break;
                //SQLDm 10.0 - Srishti Puohit - New Recommendations - SDR-Q43
                case RecommendationType.FrequentlyExecutedProcedureWithHighCPUTime:
                    recommendation = new FrequentlyExecutedProcedureWithHighCPUTimeRecommendation(recommendationProperties);
                    break;
                //SQLDm 10.0 - Srishti Puohit - New Recommendations - SDR-I24
                case RecommendationType.HighModificationsSinceLastStatUpdate:
                    recommendation = new HighModificationsSinceLastStatUpdateRecommendation(recommendationProperties);
                    break;
                //SQLDm 10.0 - Srishti Puohit - New Recommendations - SDR-D23
                case RecommendationType.ResourceGovernerIOStall:
                    recommendation = new ResourceGovernerIOStallRecommendation(recommendationProperties);
                    break;
                //SQLDm 10.0 - Srishti Puohit - New Recommendations - SDR-Q44
                case RecommendationType.NewCardinalityEstimatorNotBeingUsed:
                    recommendation = new NewCardinalityEstimatorNotBeingUsedRecommendation(recommendationProperties);
                    break;
                //SQLDm 10.0 - Srishti Puohit - New Recommendations - SDR-Q45
                case RecommendationType.BothNewAndOldCardinalityEstimatorInUse:
                    recommendation = new BothNewAndOldCardinalityEstimatorInUseRecommendation(recommendationProperties);
                    break;
                //SQLDm 10.0 - Srishti Puohit - New Recommendations - SDR-R8
                case RecommendationType.AvailabilityGroupNotEnabledForFailover:
                    recommendation = new AvailabilityGroupNotEnabledForFailoverRecommendation(recommendationProperties);
                    break;
                //SQLDm 10.0 - Srishti Puohit - New Recommendations - SDR-M33
                case RecommendationType.BufferPoolExtensionHighIO:
                    recommendation = new BufferPoolExtensionHighIORecommendation();
                    break;
            }
            return recommendation;
        }
    }
}
