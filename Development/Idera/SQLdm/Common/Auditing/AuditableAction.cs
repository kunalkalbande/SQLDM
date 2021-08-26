// -----------------------------------------------------------------------
// <copyright file="AuditableAction.cs" company="Idera">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel;

namespace Idera.SQLdm.Common.Auditing
{
    /// <summary>
    /// Action types
    /// </summary>
    public enum AuditableActionType
    {
        [Description("Never used")]
        None = 0,
        [Description("Server Added to Monitor")]
        AddServer = 1,
        [Description("Monitored Server Deleted")]
        DeleteServer = 2,
        [Description("Server Properties Changed")]
        ServerPropertiesChanged = 3,
        [Description("Alert Configuration Changed")]
        AlertConfigurationChanged = 4, //Needs to stay
        [Description("Alert Configuration Copied from One Server to Another")]
        CopyAlertConfigFromServerToServer = 5,//Needs to stay
        [Description("Maintenance Mode Manually Enabled")]
        MaintenanceModeManuallyEnabled = 6,
        [Description("Maintenance Mode Manually Disabled")]
        MaintenanceModeManuallyDisabled = 7,
        [Description("Maintenance Mode Schedule Changed")]
        MaintenanceModeScheduleChanged = 8,
        [Description("Alert template Applied to Server")]
        ApplyAlertTemplateToServer = 9,
        [Description("Single Alert Snoozed")]
        SingeAlertSnooze = 10,
        [Description("Multiple Alerts Snoozed")]
        MultipleAlertsSnoozed = 11,
        [Description("Tag Added")]
        AddTag = 12,
        [Description("Tag Removed")]
        DeleteTag = 13,
        [Description("Session Killed")]
        SessionKilled = 14,
        [Description("License Updated")]
        LicenseUpdated = 15,
        [Description("Multiple Alerts Resumed")]
        AlertsUnSnoozed = 16,
        [Description("Alert Configuration Copied from Server")]
        CopyAlertConfigurationFromServer = 17,
        [Description("Maintenance Mode Schedule Started")]
        MaintenanceModeScheduledStart = 18,
        [Description("Maintenance Mode Schedule Stopped")]
        MaintenanceModeScheduledStop = 19,
        [Description("Alert Response Added")]
        AddAlertResponse = 20,
        [Description("Alert Response Edited")]
        EditAlertResponse = 21,
        [Description("Alert Response Removed")]
        RemoveAlertResponse = 22,
        [Description("Alert Response Copied")]
        CopyAlertResponse = 23,
        [Description("Predictive Analytics Enabled")]
        PredictiveAnalyticsEnabled = 24,
        [Description("Predictive Analytics Disabled")]
        PredictiveAnalyticsDisabled = 25,
        [Description("Alert Template Added")]
        AddAlertTemplate = 26,// Desktop Client Repository Helper
        [Description("Alert Template Deleted")]
        DeleteAlertTemplate = 27,// Desktop Client Repository Helper
        [Description("Default Alert Template Set")]
        SetDefaultAlertTemplate = 28,// Desktop Client Repository Helper
        [Description("Alert Template Edited")]
        EditAlertTemplate = 29,// Desktop Client Repository Helper
        [Description("Baseline Configuration Changed")]
        BaselineConfigurationChanged = 30,
        [Description("VMWare Virtualization Host Added")]
        AddVmCenter = 31,// Desktop Client Repository Helper
        [Description("VMWare Virtualization Host Edited")]
        EditVmCenter = 32,// Desktop Client Repository Helper
        [Description("VMWare Virtualization Host Removed")]
        DeleteVmCenter = 33,// Desktop Client Repository Helper
        [Description("Grooming Configuration Changed")]
        GroomingConfigurationChanged = 34,
        [Description("Custom Counter Linked To Tag")]
        CustomCounterAddedToTag = 35,
        [Description("Aggregate Now Triggered")]
        AggregateNow = 36,
        [Description("Groom Now Triggered")]
        GroomNow = 37,
        [Description("Server Added to Tag")]
        AddServerToTag = 38,
        [Description("Server Deleted from Tag")]
        DeleteServerToTag = 39,
        [Description("Action Provider Added")]
        AddActionProvider = 40,
        [Description("Action Provider Edited")]
        EditActionProvider = 41,
        [Description("Action Provider Removed")]
        RemoveActionProvider = 42,
        [Description("VMWare Server linked To Virtualization Host")]
        ServerLinkedTovCenter = 43,
        [Description("VMWare Server unlinked from Virtualization Host")]
        ServerUnlinkedFromvCenter = 44,
        [Description("VMWare Server link configuration to Virtualization Host Changed")]
        ServerLinkConfigurationTovCenterChanged = 45,

        // Actions Running from Management Services
        [Description("Full Text Search Optimized")]
        FullTextSearchOptimized = 46,
        [Description("Full Text Search Rebuild")]
        FullTextSearchRebuilt = 47,
        [Description("Clear Procedure Cache Triggered")]
        ClearProcedureCacheTriggered = 48,
        [Description("Blocked Process Threshold Configuration Edited")]
        BlockedProcessThresholdConfigurationEdited = 49,
        [Description("SQL Server Agent Service Started")]
        SQLServerAgentServiceStarted = 50,
        [Description("SQL Server Agent Service Stopped")]
        SQLServerAgentServiceStopped = 51,
        [Description("SQL Job Started")]
        SQLJobStarted = 52,
        [Description("SQL Job Stopped")]
        SQLJobStopped = 53,
        [Description("Application Security Enabled")]
        ApplicationSecurityEnabled = 54,
        [Description("Application Security Disabled")]
        ApplicationSecurityDisabled = 55,
        [Description("User Account Deleted")]
        ApplicationSecurityDeleteUserAccount = 56,
        [Description("User Account Edited")]
        ApplicationSecurityEditUserAccount = 57,
        [Description("User Account Added")]
        ApplicationSecurityAddUserAccount = 58,
        [Description("Distributed Transaction Coordinator Service Started")]
        DTCServiceStarted = 59,
        [Description("Distributed Transaction Coordinator Service Stopped")]
        DTCServiceStopped = 60,
        [Description("Full-Text Search Service Started")]
        FTSServiceStarted = 61,
        [Description("Full-Text Search Service Stopped")]
        FTSServiceStopped = 62,
        [Description("SQL Server Service Started")]
        SQLServerServiceStarted = 63,
        [Description("SQL Server Service Stopped")]
        SQLServerServiceStopped = 64,
        [Description("SQL Server Logs Recycling Triggered")]
        RecycleLogSQLServer = 65,
        [Description("SQL Server Agent Logs Recycling Triggered")]
        RecycleLogSQLServerAgent = 66,
        [Description("Query Executed")]
        RunQuery = 67,
        [Description("All Server Alerts Snoozed")]
        SnoozeAllServerAlerts = 68,// Moved
        [Description("All Server Alerts Resumed")]
        ResumeAllServerAlerts = 69,// Moved
        [Description("Statistics Updated")]
        UpdateStatistics = 70,
        [Description("Configuration Value Changed")]
        ConfigurationValueChanged = 71,
        [Description("Mirror Data Base Resume Triggered")]
        MirrorResume = 72,
        [Description("Mirror Data Base Fail Over Triggered")]
        MirrorFailOver = 73,
        [Description("Mirror Data Base Suspend Triggered")]
        MirrorSuspend = 74,
        [Description("Trace Session")]
        TraceSession = 75,
        [Description("Custom Counter Unlinked From Tag")]
        CustomCounterRemovedToTag = 76,
        [Description("Application Security User linked to Tag")]
        ApplicationSecurityLinkedToTag = 77,
        [Description("Application Security User unlinked from Tag")]
        ApplicationSecurityUnlinkedFromTag = 78,

        //Auditlogs for HyperV
        [Description("HyperV Virtualization Host Added")]
        AddHyperV = 79,
        [Description("HyperV Virtualization Host Edited")]
        EditHyperV = 80,
        [Description("HyperV Virtualization Host Removed")]
        DeleteHyperV = 81,
        [Description("HyperV Server linked To Virtualization Host")]
        ServerLinkedToHyperV = 82,
        [Description("HyperV Server unlinked from Virtualization Host")]
        ServerUnlinkedFromHyperV = 83,
        [Description("HyperV Server link configuration to Virtualization Host Changed")]
        ServerLinkConfigurationToHyperVChanged = 84,
        //START : SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --added the action type for the new SQL services
        [Description("SQL Server Browser Service Started")]
        BrowserServiceStarted = 85,
        [Description("SQL Server Browser Service Stopped")]
        BrowserServiceStopped = 86,
        [Description("SQL Active Directory Helper Service Started")]
        ActiveDirectoryHelperServiceStarted = 87,
        [Description("SQL Active Directory Helper Service Stopped")]
        ActiveDirectoryHelperServiceStopped = 88,
        //END : SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --added the action type for the new SQL services
        
		//START 10.0 (srishti purohit) : To handle analysis configuration change log for auditing
        
        [Description("Analysis Configuration Changed")]
        AnalysisConfigurationChanged = 89
        //END 10.0 (srishti purohit) : To handle analysis configuration change log for auditing
    }
}
