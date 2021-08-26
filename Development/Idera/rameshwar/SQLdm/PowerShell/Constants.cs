//------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.PowerShell
{
    internal static class Constants
    {
        public const string ProviderName = "SQLdm";

        public const string RepositoryInstanceParameter = "RepositoryInstance";
        public const string RepositoryNameParameter = "RepositoryName";

        // Monitored Instance Parameters

        internal const string ScheduledCollectionIntervalParameter = "ScheduledCollectionIntervalMinutes";
        internal const string WindowsAuthenticationParameter = "WindowsAuthentication";
        internal const string CredentialParameter = "Credential";
        internal const string PasswordParameter = "Password";
        internal const string EncryptParameter = "EncryptConnection";
        internal const string TrustCertsParameter = "TrustServerCertificates";
        internal const string CollectExtendedSessionDataParameter = "DisableExtendedSessionDataCollection";
        internal const string CollectReplicationDataParameter = "DisableReplicationStatisticsDataCollection";
        internal const string SetCollectExtendedSessionDataParameter = "ExtendedSessionDataCollection";
        internal const string SetCollectReplicationDataParameter = "ReplicationStatisticsDataCollection";
        internal const string DisableOleAutomationUseParameter = "DisableOleAutomationUse";
        internal const string TagsParameter = "Tags";           // New-Item and Set-Item
        internal const string AddTagParameter = "AddTag";       // Set-Item
        internal const string RemoveTagParameter = "RemoveTag"; // Set-Item
        internal const string DatabaseStatisticsParameter = "DatabaseStatisticsIntervalMinutes";

        //SQLDM-30516. Add New parameter for setting "Alert if server is inaccessible" in general settings
        internal const string ServerInAccessibleRateParameter = "ServerInAccessibleAlertMinutes";

        // Query Monitor
        internal const string QMEnabledParameter = "QMEnabled";
        internal const string QMDisabledParameter = "QMDisabled";
        internal const string CaptureStatementsParameter = "QMCaptureStatements";
        internal const string CaptureBatchesParameter = "QMCaptureBatches";
        internal const string CaptureProcsParameter = "QMCaptureProcs";
        internal const string CaptureDeadlocksParameter = "QMCaptureDeadlocks";
        internal const string QueryDurationParameter = "QMQueryDuration";
        internal const string QueryTopPlanCountParameter = "QMQueryTopPlanCount";
        internal const string QueryTopPlanCategoryParameter = "QMQueryTopPlanCategory";
        internal const string CPUUsageParameter = "QMCpuUsage";
        internal const string LogicalDiskReadsParameter = "QMLogicalDiskReads";
        internal const string PhysicalDiskWritesParameter = "QMPhysicalDiskWrites";
        internal const string ExcludedAppsParameter = "QMExcludedApps";
        internal const string ExcludedDatabasesParameter = "QMExcludedDatabases";
        internal const string ExcludedSqlParameter = "QMExcludedSql";
        internal const string EnableNonQueryActivityParameter = "NQAEnable";
        internal const string DisableNonQueryActivityParameter = "NQADisable";

        internal const string EnableNonQueryActivitySQLTrace = "NQAEnableSQLTrace";
        internal const string EnableNonQueryActivityExtendedEvent = "NQAEnableExtendedEvent";
        internal const string EnableNonQueryActivityAutoGrow = "NQAEnableAutoGrow";
        internal const string DisableNonQueryActivityAutoGrow = "NQADisableAutoGrow";
        internal const string EnableNonQueryActivityCaptureBlocking = "NQAEnableCaptureBlocking";
        internal const string DisableNonQueryActivityCaptureBlocking = "NQADisableCaptureBlocking";
        internal const string NonQueryActivityCaptureBlocking = "NQACaptureBlocking";

        internal const string EnableQueryMonitorTraceMonitoring = "QMEnableTraceMonitoring";
        internal const string EnableQueryMonitorExtendedEvents = "QMEnableExtendedEvents";
        // SQLdm 10.4 (Varun Chopra) - Query monitor using Query Store
        internal const string EnableQueryMonitorQueryStoreMonitoring = "QMEnableQueryStoreMonitoring";
        internal const string EnableQueryMonitorCollectQueryPlan = "QMEnableCollectQueryPlan";
        internal const string DisableQueryMonitorCollectQueryPlan = "QMDisableCollectQueryPlan";
        internal const string EnableQueryMonitorCollectEstimatedQueryPlan = "QMEnableCollectEstimatedQueryPlan";
        internal const string DisableQueryMonitorCollectEstimatedQueryPlan = "QMDisableCollectEstimatedQueryPlan";
        //Maintenance Mode
        internal const string MModeAlwaysParameter = "MMAlways";
        internal const string MModeNeverParameter = "MMNever";
        internal const string MModeRecurringParameter = "MMRecurring";
        internal const string MModeOnceParameter = "MMOnce";

        internal const string MModeDaysParameter = "MModeDays";
        internal const string MModeStartTimeParameter = "MModeStartTime";
        internal const string MModeDurationParameter = "MModeDuration";
        internal const string MModeStartDateParameter = "MModeStartDate";
        internal const string MModeEndDateParameter = "MModeEndDate";
        // Quiet Time
        internal const string QTDaysParameter = "QTDays";
        internal const string QTStartTimeParameter = "QTStartTime";
        internal const string QTReorgMinTableSizeParameter = "QTReorgMinTableSizeK";
        internal const string QTExcludedDatabasesParameter = "QTExcludedDatabases";

        // Set-SQLdmAppSecurity 
        internal const string EnabledParameter = "Enabled";
        internal const string DisabledParameter = "Disabled";

        // Set-SQLdmAppUser
        internal const string TypeParameter = "Type";
        internal const string CommentParameter = "Comment";
        internal const string AdmininstratorParameter = "Administrator";
        internal const string LoginTypeSqlParameter = "SQLLogin";
        internal const string LoginTypeWindowsUserParameter = "WindowsUser";
        internal const string LoginTypeWindowsGroupParameter = "WindowsGroup";

        // Grant-SQLdmPermission & RevokeSQLdmPermission
        internal const string PermissionParameter = "Permission";
        internal const string ServersParameter = "Servers";
        internal const string AddServersParameter = "AddServers";
        internal const string RemoveServersParameter = "RemoveServers";

        // Os Metrics
        internal const string OsMetricNotCollect = "OsMDisable";
        internal const string OsMetricCollectByOleParameter = "OsMCollectByOLE";
        internal const string OsMetricCollectByWmiParameter = "OsMCollectByWMI";
        internal const string OsMetricConnectAsCollectionService = "OsMConnectThruCollectionSvc";
        internal const string OsMetricWmiUser = "OsMWMILoginName";
        internal const string OsMetricWmiPassword = "OsMWMIPassword";

        //General Settings Configuration
        //SQLdm 10.1 -- Srishti Purohit
        internal const string FriendlyServerName = "FriendlyServerName";
        internal const string FriendlyServerNameBlank = "FriendlyServerNameBlank";
        internal const string InputBufferLimiter = "InputBufferLimiter";
        internal const string InputBufferLimiterEnable = "InputBufferLimiterEnable";
        internal const string InputBufferLimiterDisable = "InputBufferLimiterDisable";

        //START -- Active Wait configuration
        //SQLdm 10.1 -- Srishti Purohit
        //internal const string QWStatisticsEnable = "QWStatisticsEnable";
        internal const string QWStatisticsDisable = "QWStatisticsDisable";
        internal const string QWExtendedEnable = "QWExtendedEnable";
        internal const string QWExtendedDisable = "QWExtendedDisable";
        internal const string QWStatisticsCollectIndefinite = "QWStatisticsCollectIndefinite";

        internal const string QWStatisticsStartDate = "QWStatisticsStartDate";
        internal const string QWStatisticsDuration = "QWStatisticsDuration";
        //END -- Active Wait configuration

        //SQLDM 10.1 (PULKIT PURI)--MAXIMUM LENGTH OF FRIENDLY SERVER NAME
        public const int MAX_FRIENDLYSERVERNAME_LENGTH = 256;
    }
}
