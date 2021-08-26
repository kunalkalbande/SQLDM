//------------------------------------------------------------------------------
// <copyright file="ManagementService.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.Common.Services
{
    using System;
    using System.Collections.Generic;
    using Events;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Notification;
    using Idera.SQLdm.Common.Thresholds;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Objects.ApplicationSecurity;
    using Idera.SQLdm.Common.Auditing;
    using Idera.SQLdm.Common.CWFDataContracts;
    using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;
    using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Resources;
    using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Values;
    using Recommendations;

    /// <summary>
    /// The interface implemented by the management service's hosted remotable object.  These methods
    /// are for the Desktop and Management Service consoles.
    /// </summary>
    public interface IManagementService : IOnDemandClient
    {
        /* ******************************************************************* */
        /* Service Information Stuff                                           */
        /* ******************************************************************* */
        /// <summary>
        /// Get the basic configuration info stored in the management service config file.
        /// </summary>
        /// <returns></returns>
        ManagementServiceConfigurationMessage GetManagementServiceConfiguration();

        /// <summary>
        /// Set the basic configuration info stored in the management service config file.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        bool SetManagementServiceConfiguration(ManagementServiceConfigurationMessage config);

        /// <summary>
        /// Returns a list of Collection Services registered to this Management Service.
        /// </summary>
        /// <returns></returns>
        IList<CollectionServiceInfo> GetCollectionServices();

        /// <summary>
        /// Requests the status of the management service and its registered collection services.
        /// </summary>
        /// <returns></returns>
        ManagementServiceStatus GetServiceStatus();

        /// <summary>
        /// Returns the xml returned by the status query - this object can be stale as it is collected
        /// by the management service every 30 seconds.
        /// </summary>
        /// <returns></returns>
        String GetMonitoredSQLServerStatusDocument();

        /// <summary>
        /// Forces a scheduled refresh to start, waits for completion, returns the status document
        /// for the server.
        /// </summary>
        /// <param name="monitoredSqlServerID"></param>
        /// <returns></returns>
        String ForceScheduledRefresh(int monitoredSqlServerID);

        /* ******************************************************************* */
        /* License Stuff                                                       */
        /* ******************************************************************* */

        ///// <summary>
        ///// Gets the current number of registered servers and the list of license keys.
        ///// </summary>
        ///// <param name="registeredServers">number of registered servers</param>
        ///// <param name="keyList">list of keys in the database</param>
        //void GetLicenseKeys(out int registeredServers, out IEnumerable<string> keyList);

        /// <summary>
        /// Sets the license keys to the provided list.  The Management Service will provide 
        /// logic to validate the list of keys before storing them.  A ServiceException will be
        /// thrown if the validation fails.  Returns the new list of keys in the repository.
        /// </summary>
        LicenseSummary SetLicenseKeys(LicenseKeyOperation operation, IEnumerable<string> keyList);

        /* ******************************************************************* */
        /* Monitored SQL Server Stuff                                          */
        /* ******************************************************************* */

        /// <summary>
        /// check if user is sysadmin or not
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns>boolean</returns>
        TestSqlConnectionResult isSysAdmin(SqlConnectionInfo sqlConnectionInfo);

        /// <summary>
        /// SQLdm 10.0.2 (Barkha Khatri)
        /// gets the product vesion of an SQL Server given the connection string 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        ServerVersion GetProductVersion(int instanceId);

        /// <summary>
        /// Returns the available SQL Server instance from the default collection service
        /// associated with the management service.
        /// </summary>
        /// <returns>The list of available SQL Server instances.</returns>
        DataTable GetAvailableSqlServerInstances();

        /// <summary>
        /// Tests SQL connection through to the default collection service.
        /// </summary>
        /// <param name="connectionInfo">The target SQL Server connection info.</param>
        /// <returns>The test result.</returns>
        TestSqlConnectionResult TestSqlConnection(SqlConnectionInfo connectionInfo);

        /// <summary>
        /// Adds a collection of new monitored SQL Servers.
        /// </summary>
        /// <param name="configurations">The configurations for the new monitored SQL Servers.</param>
        /// <returns></returns>
        ICollection<MonitoredSqlServer> AddMonitoredSqlServers(
            ICollection<MonitoredSqlServerConfiguration> configurations);

        /// <summary>
        /// Adds a new monitored SQL Server
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        MonitoredSqlServer AddMonitoredSqlServer(MonitoredSqlServerConfiguration configuration);

        /// <summary>
        /// Update a list of monitored SQL Servers.
        /// </summary>
        /// <param name="configurations"></param>
        /// <returns></returns>
        IEnumerable<MonitoredSqlServer> UpdateMonitoredSqlServers(IEnumerable<Pair<int, MonitoredSqlServerConfiguration>> configurations);

        /// <summary>
        /// Updated existing monitored SQL Server.
        /// </summary>
        /// <param name="id">The Id of the SQL Server instance to update.</param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        MonitoredSqlServer UpdateMonitoredSqlServer(int id, MonitoredSqlServerConfiguration configuration);

        /// <summary>
        /// SQLdm 9.0 (Abhishek Joshi) -Get the CWF Web URL
        /// </summary>
        /// <returns></returns>
        string GetCWFWebURL();

        /// <summary>
        /// SQLdm 10.0 (Rajesh Gupta) -Get the CWF Base URL
        /// </summary>
        /// <returns></returns>
        string GetCWFBaseURL();

        /// <summary>
        /// SQLdm 10.0 (Rajesh Gupta) -Get the CWF ProductID
        /// </summary>
        /// <returns></returns>
        string GetCWFProductID();

        /// <summary>
        /// SQLdm 10.0 (Rajesh Gupta) -Writes To Registry key-value at Path specified
        /// </summary>
        /// <param name="path"></param>
        /// <param name="keyname"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        void WriteToRegistry(string path, string keyname, string value);


			
		//SQLDM-30197
		string GetPreferredClusterNode(int instanceId);
		
        /// <summary>
        /// SQLdm 10.0 (Rajesh Gupta) -Registers License Manager,Writes Keys to Registry
        /// </summary>        
        /// <returns></returns>
        void RegisterLicenseManager();

        /// <summary>
        /// SQLdm 9.0 (Abhishek Joshi) -Get the list of instances from CWF
        /// </summary>
        /// <returns></returns>
        List<Instance> GetInstancesFromCWF();

        /// <summary>
        /// SQLdm 9.1 (Abhishek Joshi) -Get a list of WebUIURL, WebURI, registered instance name and query statistics Id (applicable to sql statements) 
        /// </summary>
        /// <returns></returns>
        List<string> GetWebUIQueryInfo();

        /// <summary>
        /// Activates a monitored SQL Server instance by id.
        /// </summary>
        /// <param name="id">The target instances id.</param>
        void ActivateMonitoredSqlServer(int id);

        /// <summary>
        /// Deactivates a collection of monitored SQL Servers by id.
        /// </summary>
        /// <param name="ids"></param>
        void DeactivateMonitoredSqlServers(ICollection<int> ids);

        /// <summary>
        /// Deactivates a monitored SQL Server instance by id.
        /// </summary>
        /// <param name="id">The target instance id.</param>
        void DeactivateMonitoredSqlServer(int id);

        /// <summary>
        /// Deletes a collection of monitored SQL Servers by id.
        /// </summary>
        /// <param name="ids"></param>
        void DeleteMonitoredSqlServers(ICollection<int> ids);

        /// <summary>
        /// Deletes the specified monitored SQL Server and all collected data.
        /// </summary>
        /// <param name="id"></param>
        void DeleteMonitoredSqlServer(int id);

        /// <summary>
        /// Returns an array of the agent job names on a monitored server instance.
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        IEnumerable<string> GetAgentJobNames(int monitoredSqlServerId);

        /// <summary>
        /// Returns an array of the agent job categories on a monitored server instance.
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        IEnumerable<string> GetAgentJobCategories(int monitoredSqlServerId);

        /// <summary>
        /// Returns an array of the agent job steps (with category and job names) on a monitored server instance.
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        IEnumerable<CategoryJobStep> GetAgentJobStepList(int monitoredSqlServerId);

        /* ******************************************************************* */
        /* Notification Stuff                                                  */
        /* ******************************************************************* */

        /// <summary>
        /// Gets the notification providers.
        /// </summary>
        /// <returns></returns>
        IList<NotificationProviderInfo> GetNotificationProviders();

        /// <summary>
        /// Adds a new notification provider configuration.
        /// </summary>
        /// <returns></returns>
        NotificationProviderInfo AddNotificationProvider(NotificationProviderInfo providerInfo);

        void DeleteNotificationProvider(Guid providerId);

        void UpdateNotificationProvider(NotificationProviderInfo providerInfo, bool updateRules);

        /// <summary>
        /// Gets the notification rules.
        /// </summary>
        /// <returns></returns>
        IList<NotificationRule> GetNotificationRules();

        /// <summary>
        /// Add the notification rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        NotificationRule AddNotificationRule(NotificationRule rule);

        /// <summary>
        /// Deletes the notification rule.
        /// </summary>
        /// <param name="ruleId">The ID of the rule</param>
        void DeleteNotificationRule(Guid ruleId);

        /// <summary>
        /// Updates the notification rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        void UpdateNotificationRule(NotificationRule rule);

        /// <summary>
        /// Validate the information in the destination.  Chuck an exception if not valid.
        /// </summary>
        void ValidateDestination(NotificationDestinationInfo destination);

        /// <summary>
        /// Test the specified action.  Percolate exceptions.
        /// </summary>
        int TestAction(NotificationProviderInfo providerInfo, NotificationDestinationInfo destinationInfo, object data);

        /* ******************************************************************* */
        /* Threshold Stuff                                                     */
        /* ******************************************************************* */

        /// <summary>
        /// Get the list of thresholds for the monitored server with the given id.
        /// </summary>
        /// <param name="userViewID"></param>
        /// <returns></returns>
        IList<MetricThresholdEntry> GetDefaultMetricThresholds(int userViewID);

        /// <summary>
        /// Get AWS resource principle details
        /// </summary>
        /// <param name="instanceName">Instance name</param>
        /// <returns>AWS resource principle details</returns>
        AWSAccountProp GetAWSResourcePrincipleDetails(string instanceName);

        /// <summary>
        /// Get the list of thresholds for the monitored server with the given id.
        /// </summary>
        /// <param name="monitoredServerID"></param>
        /// <returns></returns>
        IList<MetricThresholdEntry> GetMetricThresholds(int monitoredServerID);

        //        void AddDefaultMetricThresholdEntries(IEnumerable<MetricThresholdEntry> entries);

        void ChangeAlertTemplateConfiguration(AlertConfiguration configuration);
        void ChangeAlertConfiguration(AlertConfiguration configuration);

        void UpdateAlertConfigurations(IEnumerable<MetricThresholdEntry> changedItems, IEnumerable<int> targetInstances);

        void ReplaceDefaultAlertConfiguration(IEnumerable<MetricThresholdEntry> configurationItems, int userViewID);
        void ReplaceAlertConfiguration(IEnumerable<MetricThresholdEntry> configurationItems, IEnumerable<int> targetInstances);
        void AddAlertTemplate(int TemplateID, IEnumerable<int> targetInstances);
        SnoozeInfo SnoozeAlerts(int instanceId, int? metricId, int minutesToSnooze, string requestingUser);
        void SnoozeServersAlerts(IList<int> serverListId, int? metricId, int minutesToSnooze, string requestingUser);
        SnoozeInfo UnSnoozeAlerts(int instanceId, int[] metrics, string requestingUser);
        void UnSnoozeServersAlerts(IList<int> serverListId, int[] metrics, string requestingUser);

        /* ******************************************************************* */
        /* Task Stuff                                                          */
        /* ******************************************************************* */

        /// <summary>
        /// Updates a task in the repository.  If status is set to completed then 
        /// TaskCompletedOn is set to the current datetime otherwise it is set to null.
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="status"></param>
        /// <param name="owner"></param>
        /// <param name="comments"></param>
        void UpdateTask(int taskID, TaskStatus status, string owner, string comments);

        /// <summary>
        /// Delete a task from the repository.
        /// </summary>
        /// <param name="taskIDs">list or array of task ids to delete</param>
        void DeleteTask(IEnumerable<int> taskIDs);

        /* ******************************************************************* */
        /* Grooming Stuff                                                      */
        /* ******************************************************************* */
        void UpdateGrooming(GroomingConfiguration configuration);
        void CreateGroomJob();
        void GroomNow();
        void AggregateNow();
        GroomingConfiguration GetGrooming();
        void SetQueryAggregationFlag(long signatureID, bool doNotAggregate);
        KeyValuePair<bool, string> GetGroomingTimedOutState(int sqlServerId); //SQLdm 9.0 -(Ankit Srivastava) - added new method to get the timed out instances

        /* ******************************************************************* */
        /* Alert Stuff                                                         */
        /* ******************************************************************* */
        void ClearActiveAlerts(long alertID, bool allAlerts);

        /// <summary>
        /// Method to Update AG Role Change alert records in Alerts table
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="server"></param>
        /// <param name="metricID"></param>
        /// <param name="MinutesAgeAlerts"></param>
        void UpdateAGAlertLogRecord(DateTime dateTime, string server, int metricID, int MinutesAgeAlerts);

        /* ******************************************************************* */
        /* Custom Counter                                                      */
        /* ******************************************************************* */
        int AddCustomCounter(MetricDefinition metricDefinition, MetricDescription metricDescription, CustomCounterDefinition counterDefinition, bool alertOnFailureToCollect);
        void UpdateCustomCounter(MetricDefinition metricDefinition, MetricDescription metricDescription, CustomCounterDefinition counterDefinition, bool alertOnFailureToCollect);
        void UpdateCustomCounterStatus(int metricID, bool enabled);
        void DeleteCustomCounter(int metricID);
        void AddCounterToServers(int metricId, IEnumerable<int> tags, IEnumerable<int> monitoredSqlServers);

        /* ******************************************************************* */
        /* Application Security                                                */
        /* ******************************************************************* */
        void EnableSecurity();
        void DisableSecurity();
        Configuration GetSecurityConfiguration();
        bool DoesLoginExist(string login);
        void AddPermission(string login, bool isSQLLogin, string password, PermissionType permission, IEnumerable<int> tags, IEnumerable<int> servers, string comment, bool webAppPermission);
        Dictionary<int, string> GetBlockedCategoryListForTargetPlatform(int sqlServerId);
        List<string> GetRecommendationListForTargetPlatform(int? sqlServerId = null);
        void EditPermission(int permissionID, bool enabled, PermissionType permission, IEnumerable<int> tags, IEnumerable<int> servers, string comment, bool webAppPermission);
        void SetPermissionStatus(int permissionID, bool flag);
        void SetWebAccessStatus(int permissionID, bool flag);
        void DeletePermission(int permissionID);

        /* ******************************************************************* */
        /* Tags                                                                */
        /* ******************************************************************* */
        int AddTag(string name);
        int UpdateTagConfiguration(Tag tag);
        void RemoveTags(IList<int> tagIds);

        ICollection<CWFDataContracts.GlobalTag> GetGlobalTags(); // SQLdm 10.1 - (Praveen Suhalka) - CWFIntegration

        /* ******************************************************************* */
        /* Mirroring Stuff                                                     */
        /* ******************************************************************* */
        void SetMirroringPreferredConfig(MirroringSession session);
        Dictionary<Guid, MirroringSession> GetMirroringPreferredConfig();

        ///* ******************************************************************* */
        ///* Replication                                                     */
        ///* ******************************************************************* */
        ///// <summary>
        ///// The key is the hashcode of the instance, publisherdb and publication
        ///// </summary>
        ///// <param name="ServerID"></param>
        ///// <returns></returns>
        //Dictionary<string, Idera.SQLdm.Common.Objects.Replication.ReplicationSession> GetReplicationTopology(int ServerID);

        /* Custom report                                                       */
        /* ******************************************************************* */
        /// <summary>
        /// Delete = 1, Update, Append, Rename, Create New
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="reportID"></param>
        /// <param name="reportName"></param>
        /// <param name="reportShortDescription"></param>
        /// <param name="reportText"></param>
        int UpdateCustomReport(CustomReport.Operation operation, int? reportID, string reportName, string reportShortDescription, Serialized<string> reportText, Boolean showTopServers);
        /// <summary>
        /// Insert a new counter for the specified report
        /// </summary>
        /// <param name="repositoryConnectionString"></param>
        /// <param name="ReportName"></param>
        /// <param name="GraphNumber"></param>
        /// <param name="CounterName"></param>
        /// <param name="ShortDescription"></param>
        /// <param name="Aggregation"></param>
        /// <param name="Source"></param>
        /// <returns></returns>
        bool InsertCounterToGraph(string repositoryConnectionString, string ReportName, int GraphNumber,
                                  string CounterName, string ShortDescription, int Aggregation, int Source);
        /// <summary>
        /// Delete all counters from the specified report
        /// </summary>
        /// <param name="repositoryConnectionString"></param>
        /// <param name="ReportName"></param>
        /// <returns></returns>
        bool DeleteCustomReportCounters(string repositoryConnectionString, string ReportName);
        /*

                /// <summary>
                /// Starts the monitoring server.
                /// </summary>
                /// <param name="monitoredServer">The monitored server.</param>
                /// <param name="collectionServiceId">The collection service id.</param>
                /// <returns></returns>
                Result StartMonitoringServer(MonitoredServerWorkload monitoredServerWorkload, Guid collectionServiceId);

                /// <summary>
                /// Stops the monitoring server.
                /// </summary>
                /// <param name="monitoredServer">The monitored server.</param>
                /// <param name="collectionServiceId">The collection service id.</param>
                /// <returns></returns>
                Result StopMonitoringServer(MonitoredServer monitoredServer, Guid collectionServiceId);

                /// <summary>
                /// Gets the monitored server workload.
                /// </summary>
                /// <param name="monitoredServer">The monitored server.</param>
                /// <returns></returns>
                MonitoredServerWorkload GetMonitoredServerWorkload(MonitoredServer monitoredServer);

                /// <summary>
                /// Reconfigures the monitored server.
                /// </summary>
                /// <param name="monitoredServerWorkload">The monitored server workload.</param>
                /// <param name="collectionServiceId">The collection service id.</param>
                /// <returns></returns>
                Result ReconfigureMonitoredServer(MonitoredServerWorkload monitoredServerWorkload, Guid collectionServiceId);

                /// <summary>
                /// Gets the notification providers.
                /// </summary>
                /// <returns></returns>
                IEnumerable<NotificationProviderInfo> GetNotificationProviders();

                /// <summary>
                /// Updates the notification provider.
                /// </summary>
                /// <param name="provider">The provider.</param>
                /// <returns></returns>
                Result UpdateNotificationProvider(NotificationProviderInfo provider);

                /// <summary>
                /// Gets the notification destinations.
                /// </summary>
                /// <returns></returns>
                IEnumerable<NotificationDestinationInfo> GetNotificationDestinations();

                /// <summary>
                /// Creates the notification destination.
                /// </summary>
                /// <param name="destination">The destination.</param>
                /// <returns></returns>
                Result CreateNotificationDestination(ref NotificationDestinationInfo destination);

                /// <summary>
                /// Updates the notification destination.
                /// </summary>
                /// <param name="destination">The destination.</param>
                /// <returns></returns>
                Result UpdateNotificationDestination(ref NotificationDestinationInfo destination);

                /// <summary>
                /// Deletes the notification destination.
                /// </summary>
                /// <param name="destination">The destination.</param>
                /// <returns></returns>
                Result DeleteNotificationDestination(NotificationDestinationInfo destination);

                /// <summary>
                /// Gets the notification rules.
                /// </summary>
                /// <returns></returns>
                IEnumerable<NotificationRule> GetNotificationRules();


                #endregion
        */

        void LogAuditEvent(AuditableEntity auditEntity, short typeId);
        DataTable GetAuditEvents();
        Dictionary<int, string> GetAuditHeaderTemplates();

        void SetPredictiveAnalyticsEnabled(bool predictiveAnalyticsEnabled);
        bool GetPredictiveAnalyticsEnabled();

        void SetNextPredictiveAnalyticsModelRebuild(DateTime nextRebuild);
        DateTime GetNextPredictiveAnalyticsModelRebuild();
        void SetNextPredictiveAnalyticsForecast(DateTime nextForecast);
        DateTime GetNextPredictiveAnalyticsForecast();
        bool GetPredictiveAnalyticsHasModels();

        int GetPulseApplicationId();
        string GetPulseServerName();

        /// <summary>
        /// SQLdm 10.0 vineet kumar -- doctor integration-- get prescriptive analysis result
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result GetPrescriptiveAnalysisResult(int monitoredSqlServerId, AnalysisConfiguration config);

        /// <summary>
        /// SQLdm 10.0 preaveen suhalka -- doctor integration-- get prescriptive analysis result
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result GetAdHocBatchAnalysisResult(int monitoredSqlServerId, string queryText, string database, AnalysisConfiguration config);

        /// <summary>
        /// SQLdm 10.0 preaveen suhalka -- doctor integration-- get prescriptive analysis result
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result GetWorkLoadAnalysisResult(int monitoredSqlServerId, AnalysisConfiguration config, ActiveWaitsConfiguration waitConfig, QueryMonitorConfiguration queryConfig);
        /// <summary>
        /// SQLdm 10.0 srishti purohit -- doctor integration-- get prescriptive analysis result for notifications
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result GetPrescriptiveAnalysisResult(int monitoredSqlServerId, AnalysisConfiguration config, AnalysisType analysisType);


        /// <summary>
        /// SQLdm 10.0 preaveen suhalka -- doctor integration-- get prescriptive analysis schedule
        /// </summary>
        /// <returns></returns>
        List<Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.PAScheduledTask> GetPrescriptiveAnalysisScheduleList();

        /// <summary>
        /// SQLdm 10.0 preaveen suhalka -- doctor integration-- Get the prescriptive scheduled analysis configuration for server
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        ScheduledPrescriptiveAnalysisConfiguration GetAnalysisConfigurationForServer(int serverID);

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) -To support SDR-M16 and get SnapshotValues Object
        /// </summary>
        /// <returns>SnapshotValues</returns>
        SnapshotValues GetSnapshotValuesForServerForPrescriptiveAnalysis(int serverID);

        /// <summary>
        /// SQLdm 10.0 Tarun Sapra-- Raise the alert recommendation alert
        /// </summary>
        /// <param name="serverID"></param>
        /// <returns></returns>
        void LogOperationalAlert(string monitoredObjectName, MonitoredState monitoredState, string heading, string message);

        /// <summary>
        /// SQLDM-29041 -- Add availability group alert options.
        /// </summary>
        /// <returns></returns>
        List<MonitoredSqlServer> GetMonitoredSqlServers();

        /// <summary>
        /// SQLDM-29041 -- Add availability group alert options.
        /// </summary>
        /// <returns></returns>
        MetricThresholdEntry GetMetricThresholdEntry(MonitoredSqlServer server, int metricId);
      
        /// <summary>
        /// SQLDM-30244 -- Getting datetime of collection service.
        /// </summary>
        /// <returns></returns>
        DateTime GetDateTime(int id,DateTime dt);
        
        #region QueryMonitor

        Pair<long, string> GetQuerySignatureText(long signatureID);
        Pair<long, string> GetQuerySignatureText(string signatureHash);
        Pair<long, string> GetQueryStatementText(long statementID);
        Pair<long, string> GetQueryStatementText(string statementHash);

        #endregion

        #region Recommendation Doctor's UI for Analysis
        /// <summary>
        /// SQLdm 10.0 (srishti purohit) -Get the list of Master data for Recommendations
        /// </summary>
        /// <returns>MasterRecommendations</returns>
        List<MasterRecommendation> GetMasterRecommendations();


        /// <summary>
        /// SQLdm 10.0 (srishti purohit) -Save the list of Recommendation
        /// </summary>
        /// <returns></returns>
        Serialized<int> SaveRecommendations(Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result analysisResult, int sqlServerID, bool isRunAnalysis = false, int tempAnalysisID = 0);

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) -Update status of the list of Recommendation
        /// </summary>
        /// <param name="recommendation"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        Serialized<bool> UpdateRecommendationOptimizationStatus(List<IRecommendation> recommendation);

        /// <summary>
        /// SQLdm 10.0 (srishti purohit) -Save the list of blocked Recommendation and blocked database
        /// </summary>
        /// <returns></returns>
        Serialized<bool> BlockRecommendationDatabaseAnalysisConfiguration(int sqlServerID, List<string> blockedRecommendations, List<int> blockedDatabases);

        /// <summary>
        /// SQLdm 10.1 (Sristhi Purohit) -get the SWA Web URL for instance views
        /// </summary>
        /// <returns></returns>
        string GetSWAWebURL(string instanceName);

        AnalysisListCollection GetAnalysisListing(int monitoredSqlServerId);
        #endregion

        IAzureProfile GetAzureProfile(long profileId, string resourceUri);

    }
}
