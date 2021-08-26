//------------------------------------------------------------------------------
// <copyright file="ICollectionService.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Data;
using Idera.SQLdm.Common.Events.AzureMonitor;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Services
{
    using System;
    using System.Management;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Thresholds;
    using Wintellect.PowerCollections;
    using Idera.SQLdm.Common.Data;
    using Idera.SQLdm.Common.Events;
    
    /// <summary>
    /// The interface implemented by the collection service's hosted remotable object.
    /// </summary>
    public interface ICollectionService : IOnDemandServer
    {
        #region properties

        #endregion

        #region events

        #endregion

        #region methods
        /// <summary>
        /// SQLDM-30244 -- Getting datetime of collection service.
        /// </summary>
        /// <returns></returns>
        DateTime GetDateTime(DateTime dt);
       
        CollectionServiceConfigurationMessage GetCollectionServiceConfiguration();
        Result SetCollectionServiceConfiguration(CollectionServiceConfigurationMessage message);
        CollectionServiceStatus GetServiceStatus();

        Result PauseService();
        Result ResumeService();
        Result ReinitializeService();

        /// <summary>
        /// Starts the monitoring server.
        /// </summary>
        /// <param name="monitoredServerWorkload">The monitored server workload.</param>
        /// <returns></returns>
        Result StartMonitoringServer(MonitoredServerWorkload monitoredServerWorkload);

        /// <summary>
        /// Stops the monitoring server.
        /// </summary>
        /// <param name="monitoredSqlServerId">The id of the monitored server.</param>
        /// <returns></returns>
        Result StopMonitoringServer(int monitoredSqlServerId);

        /// <summary>
        /// Reconfigures the monitored server.
        /// </summary>
        /// <param name="monitoredServerWorkload">The monitored server workload.</param>
        /// <returns></returns>
        Result ReconfigureMonitoredServer(MonitoredServerWorkload monitoredServerWorkload);


        /// <summary>
        /// Update the threshold entries for an instance.
        /// </summary>
        /// <param name="monitoredSqlServerId">Id of the instance to update</param>
        /// <param name="metricThresholdEntries">Enumeration of changed items</param>
        /// <returns></returns>
        Result UpdateThresholdEntries(int monitoredSqlServerId, IEnumerable<MetricThresholdEntry> metricThresholdEntries);

        /// <summary>
        /// Returns the list of SQL Server instances available to the Collection Service.
        /// </summary>
        /// <returns>The list of available SQL Server instances.</returns>
        DataTable GetAvailableSqlServerInstances();

        /// <summary>
        /// Test a SQL connection.
        /// </summary>
        /// <param name="connectionInfo">The SQL connection info to test.</param>
        /// <returns>The test result.</returns>
        TestSqlConnectionResult TestSqlConnection(SqlConnectionInfo connectionInfo);

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
        /// Returns an array of the agent job steps (with the job and category) on a monitored server instance.
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        IEnumerable<CategoryJobStep> GetAgentJobStepList(int monitoredSqlServerId);


        /// <summary>
        /// Gets the current cluster node for a clustered SQL Server
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        string GetCurrentClusterNode(int monitoredSqlServerId);
		
		//SQLDM-30197
		/// <summary>
        /// Gets the preferred cluster node for a clustered SQL Server
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
		string GetPreferredClusterNode(int monitoredSqlServerId);
		

        /// <summary>
        /// Gets a list of disk drives used by the given instance.
        /// </summary>
        /// <param name="SqlServerId"></param>
        /// <returns></returns>
        List<string> GetDisks(int SqlServerId);
            
        /// <summary>
        /// Gets a dictionary of database names for the given instance.  The key is the 
        /// database name and the value is a boolean indicating system tables. 
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        IDictionary<string, bool> GetDatabases(int monitoredSqlServerId, bool includeSystemDatabases, bool includeUserDatabases);

        /// <summary>
        /// SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --Get all the filegroups for a SQL server
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        IList<string> GetFilegroups(int monitoredSqlServerId, string databaseName, bool isDefaultThreshold);

        /// <summary>
        /// Gets a list of table names for the given instance/database.  
        /// Each Triple contains a schema name, table name, and bool that indicates
        /// if the table is a system table.
        /// </summary>
        List<Triple<string, string, bool>> GetTables(int monitoredSqlServerId, string database, bool includeSystemTables, bool includeUserTables);

        // Get current local server time and version
        Triple<ServerVersion, DateTime, DateTime> GetServerTimeAndVersion(int monitoredSqlServerId);

		// SQLdm Minimum Privileges - Varun Chopra - Read on demand permissions collections - Minimum, Metadata and Collection
        Tuple<MinimumPermissions, MetadataPermissions, CollectionPermissions> GetServerPermissions(int monitoredSqlServerId);

        // get list of available objects in sysperfinfo
        Serialized<DataTable> GetSysPerfInfoObjectList(int monitoredSqlServerId);
        Serialized<DataTable> GetSysPerfInfoCounterList(int monitroredSqlServerId, string objectName);
        Serialized<DataTable> GetSysPerfInfoInstanceList(int monitoredSqlServerId, string objectName);

        // Azure Monitor Namespaces
        Serialized<DataTable> GetAzureMonitorNamespaces(
            int monitoredSqlServerId,
            IMonitorManagementConfiguration monitorConfiguration);

        Serialized<DataTable> GetAzureMonitorDefinitions(int instanceId,
            IMonitorManagementConfiguration monitorConfiguration);

        // get list of available objects in the \root\cimv2 namespace
        Serialized<Pair<string,DataTable>> GetWmiObjectList(int monitoredSqlServerId, WmiConfiguration wmiConfiguration);

        Serialized<DataTable> GetAzureDatabase(int monitoredSqlServerId);

        Serialized<DataTable> GetWmiCounterList(string serverName, string objectName, WmiConfiguration wmiConfiguration);
        Serialized<DataTable> GetWmiInstanceList(string serverName, string objectName, WmiConfiguration wmiConfiguration);
        Serialized<DataTable> GetDriveConfiguration(SqlConnectionInfo connectionInfo, WmiConfiguration wmiConfiguration);

        Serialized<DataTable> GetVmCounterObjectList(int monitoredSqlServerId);

        // try to retrieve the custom counter 
        object TestCustomCounter(int monitoredSqlServer, CustomCounterDefinition counterDefinition);

        void AddCustomCounter(CustomCounterDefinition counterDefinition);
        void UpdateCustomCounter(CustomCounterDefinition counterDefinition);
        void UpdateCustomCounterStatus(int metricID, bool enabled);
        void DeleteCustomCounter(int metricID);

        void AddCounterToServers(IEnumerable<MetricThresholdEntry> thresholds, bool syncWithList);
        void RemoveCounterFromServers(int metricId, int[] monitoredSqlServers);
        
        // tag support
        void RemoveTags(IEnumerable<int> tagIds);
        void AddTagToServers(int tagId, IEnumerable<int> servers);
        void RemoveTagFromServers(int tagId, IEnumerable<int> servers);
        void AddTagToCustomCounters(int tagId, IEnumerable<int> metrics);
        void RemoveTagFromCustomCounters(int tagId, IEnumerable<int> metrics);
        void UpdateTagConfiguration(int tagId, IList<int> serverIds, IList<int> customCounterIds);

        void ClearEventState(int monitoredSqlServerId, int metricId, DateTime? cutoffTime, MonitoredObjectName objectName);

        /// <summary>
        /// Save mirroring preferred config to the workload object of the specified server
        /// </summary>
        /// <param name="session"></param>
        void SaveMirrorPreferredConfig(MirroringSession session);

        ///Ankit Nagpal --Sqldm10.0.0
        /// <summary>
        /// Check if user is sysadmin or not
        /// </summary>
        /// <param name="monitoredSqlServerId"></param>
        /// <returns></returns>
        TestSqlConnectionResult IsSysAdmin(SqlConnectionInfo sqlConnectionInfo);

        /// <summary>
        /// SQLdm 10.0.2 (Barkha Khatri)
        /// gets the product vesion of an SQL Server given the connection string 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns>Server version</returns>
        ServerVersion GetProductVersion(int monitoredSqlServer); 

        #endregion
    }
}
