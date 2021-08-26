package com.idera.sqldm_10_3.rest;

public enum RestMethods {
	SERVER_STATISTICS("/ServerStatistics"),
	SQL_INSTANCE_LOCATION("/Instances/GetSQLInstanceLocations"),
	GET_DATABASE_DETAILS("/Databases/GetDatabaseDetails"),
	GET_DASHBOARD_INSTANCES("/instances"),// ("/Instances/GetDashboardInstances"),
	GET_FILTERED_DASHBOARD_INSTANCES("/instances?FilterField=<field>&FilterValue=<value>"),//("/Instances/GetDashboardInstances"),
	GET_INSTANCES_SUMMARY("/instances/abridged"),
	GET_DASHBOARD_TAGS("/tags"),
	GET_NUM_ALERT_BY_DATABASE("/Instances/Databases/ByAlerts"),
	GET_NUM_ALERT_BY_CATEGORY("/numAlertsByCategory"),
	GET_LATEST_RESPONSE_TIME_BY_INSTANCE("/Instances/TopInstanceByResponseTime/tzo/{timeZoneOffset}"),//Offset Added 14
	ADD_SQL_INSTANCE("/Instances/AddSQLInstance"),
	UPDATE_SQL_INSTANCE("/Instances/UpdateSQLInstance"),
	GET_TAGS("/GetTags"),
	GET_METRICS("/Metrics/tzo/{timeZoneOffset}"),//Added timezone param
	GET_INSTANCE_TAGS("/Instances/GetInstanceTags"),
	GET_INSTANCE_OVERVIEW("/Instances/GetInstanceOverview"),
	GET_ENVIRONMENT_OVERVIEW("/GetEnvironmentOverview"),
	GET_INSTANCE_CONFIGURATIONS("/Instances/GetInstanceConfigurations"),
	GET_DATABASES_FOR_SINGLE_INSTANCE_VIEW("/databases"),
	GET_SESSIONS_FOR_SINGLE_INSTANCE_VIEW("/Sessions"),
	GET_SESSIONS_FOR_SINGLE_INSTANCE_GRAPH_VIEW("/Sessions/statistics"),
	GET_DATABASES_CAPACITY_USAGE("/capacityusage"),
	GET_DATABASES_TEMPDB_USAGE("/tempdb"),
	GET_EMAIL_CONFIGURATION("/GetEmailConfiguration"),
	SET_EMAIL_CONFIGURATION("/SetEmailConfiguration"),
	SEND_TEST_EMAIL("/SendTestEmail"),
	GET_TOP_DATABASES_BY_SIZE("/Databases/GetTopDatabasesBySize"),
	GET_TOP_DATABASES_BY_ACTIVITY("/Databases/GetTopDatabasesByActivity"),
	GET_SUBSCRIPTIONS("/Alerts/GetSubscriptions"),
	SET_SUBSCRIPTIONS("/Alerts/SetSubscriptions"),
	GET_AGGREGATED_FILTER_INSTANCE("/DataViews/AggregateFilteredInstances"),
	GET_FILTERED_INSTANCE_DATABASE("/DataViews/SelectFilteredInstanceDatabases"),
	GET_SELECTED_FILTERED_DATABASE("/DataViews/SelectFilteredInstances"),
	GET_ALERTS("/AlertsForWebConsole/tzo/{timeZoneOffset}"),//Added timezone param
	GET_ALERTS_ABRIDGE("/AlertsForWebConsole/abridged/tzo/{timeZoneOffset}"),//Added timezone param
	GET_ALERT_HISTORY("/Alerts/{alertId}/MetricHistory/tzo/{timeZoneOffset}"),//Added timezone param
	/*--Not Used : 
	 * Alerts/{alertId}/tzo/{timeZoneOffset}
	 * Instances/{InstanceId}/Databases/Tempdb/tzo/{timeZoneOffset}
	 * Instances/{InstanceId}/databases/tzo/{timeZoneOffset
	 * Instances/{InstanceId}/ServerStatistics/tzo/{timeZoneOffset}
	 * Instances/{InstanceId}/Sessions/Statistics/tzo/{timeZoneOffset}
	 * Instances/{InstanceId}/Sessions/tzo/{timeZoneOffset}
	 */
	GET_ALERT_META_DATA("/Alerts/GetAlertMetaData"),
	GET_ALERT_THRESHOLD("/Alerts/GetAlertThreshold"),
	CLEAR_ALERTS("/Alerts/ClearAlerts"),
	REFRESH_ALERTS("/Alerts/RefreshAlerts"),
	RUN_JOB("/Scheduler/RunJob"),
	SQL_INSTANCE_VERSION("/Instances/GetSQLInstanceVersions"),
	GET_VERSION("/GetVersion"),
	ADD_SQL_INSTANCES("/Instances/BulkAddSQLInstances"),
	GET_UNREGISTERED_SQL_INSTANCES("/Instances/GetUnregisteredSQLInstances"),
	GET_REMOVED_SQL_INSTANCES("/Instances/GetRemovedSQLInstances"),
	VALIDATE_BULK_SQL_INSTANCE_CONNECTION("/Instances/BulkValidateSQLInstanceConnections"),
	GET_LICENSE_DETAILS("/License/GetLicense"),
	GET_SERVICE_STATUS("/public/GetServiceStatus"),
	GET_SQL_INSTANCE_COUNTS("/Instances/GetSQLInstanceCounts"),
	GET_REGISTERED_SQL_INSTANCES("/Instances/GetRegisteredSQLInstances"),
	GET_FILTERED_INSTANCE_VIEW("/InstanceViews/FilteredSQLInstanceView"),
	GET_LONGEST_RUNNING_QUERY("/Instances/TopInstanceByQueryDuration/tzo/{timeZoneOffset}"),//Offset Added 1, Used in TopX Enum
	GET_LONGEST_WAIT_INSTANCES("/Instances/TopInstancesByWaits"),
	GET_TEMPDB_UTILIZATION_INSTANCES("/Instances/InstancesByTempDbUtilization/tzo/{timeZoneOffset}"),//Offset Added 2
	GET_LARGESTDBS_SIZE_INSTANCES("/Instances/TopDatabasesBySize/tzo/{timeZoneOffset}"),//Offset Added 3
	GET_HIGHEST_IO_INSTANCES("/Instances/ByIOPhysicalCount"),
	GET_DATABASE_BY_ACTIVITY_INSTANCES("/Instances/TopDatabaseByActivity/tzo/{timeZoneOffset}"),//Offset Added 4 
	GET_FASTEST_GROWING_DATABASE_INSTANCES("/Instances/GetTopDatabasesByGrowth/tzo/{timeZoneOffset}"),//Offset Added 5
	GET_HIGHEST_QUERY_INSTANCES("/Instances/TopInstanceByQueryCount/tzo/{timeZoneOffset}"),//Offset Added 6
	GET_HIGHEST_DISK_SPACE_INSTANCES("/Instances/TopInstanceByDiskSpace/tzo/{timeZoneOffset}"),//Offset Added 7
	GET_TOTAL_SESSION_INSTANCES("/Instances/TopInstanceBySessionCount/tzo/{timeZoneOffset}"), //Offset Added 8
	GET_MOST_ALERT_INSTANCES("/Instances/ByAlerts"),
	GET_TOP_DATABASE_BY_ALERT_INSTANCES("/Instances/Databases/ByAlerts"),
	GET_MOST_ACTIVE_CONNECTION("/Instances/TopInstancesByConnCount/tzo/{timeZoneOffset}"),//Offset Added 9
	GET_BLOCKED_SESSION("/Instances/TopInstanceByBlockedSessions/tzo/{timeZoneOffset}"),//OffSet Added 10
	GET_SESSION_BY_CPU("/Instances/TopSessionsByCPUUsage/tzo/{timeZoneOffset}"),//Offset Added 11
	GET_HIGHEST_MEMORY_USAGE("/Instances/BySqlMemoryUsage"),
	GET_HIGHEST_CPU_LOAD("/Instances/BySqlCpuLoad"), 
	GET_QUERY_INSTANCE("/instances/{id}/queries/tzo/{timeZoneOffset}"),//Offset Added 
	GET_CATEGORY_RESOURCES("/instances/{id}/resources/tzo/{timeZoneOffset}"),//Offset Added 
	GET_BASELINE("/Instances/{INSTANCEID}/Metric/{METRICID}/Baseline/tzo/{timeZoneOffset}"),//Offset Added 
	GET_FILEDRIVES("/instances/{id}/FileDrives"),
	GET_FILEACTIVITY("/instances/{id}/FileActivity/tzo/{timeZoneOffset}"),//Offset added
	GET_SERVER_WAITS("/instances/{id}/ServerWaits/tzo/{timeZoneOffset}"),//Offset added
	GET_INSTANCE_SESSION_BY_CPU_ACTIVITY("/Instances/TopSessionsByCPUUsage/tzo/{timeZoneOffset}"),//Offset Added 12
	GET_INSTANCE_SESSION_BY_IO_ACTIVITY("/Instances/TopSessionsByIOActivity/tzo/{timeZoneOffset}"),//Offset Added 13
	GET_INSTANCE_FILES_BY_IO_ACTIVITY(""),
	GET_DATABASE_AVAILABILITY_GRPS("/instances/{id}/databases/AvailabilityGroup/tzo/{timeZoneOffset}"),//Offset Added
	GET_DATABASE_AVAILABILITY_GRP_STATS("/instances/{id}/databases/AvailabilityGroup/Statistics/tzo/{timeZoneOffset}"),//Offset Added
	GET_QUERY_INSTANCE_WAITS("/Instances/{INSTANCEID}/Queries/Waits/tzo/{TIMEZONEOFFSET}"), //Offset Added
	GET_OVERVIEW_QUERY_INSTANCE_WAITS("/Instances/{INSTANCEID}/Overview/Queries/Waits/tzo/{TIMEZONEOFFSET}"), //Offset Added
	GET_QUERY_APPLICATIONS("/instances/{id}/queries/applications"), 
	GET_QUERY_DATABASES("/instances/{id}/queries/databases"), 
	GET_QUERY_CLIENTS("/instances/{id}/queries/clients"), 
	GET_QUERY_USERS("/instances/{id}/queries/users"), 
	GET_QUERY_GROUPS("/instances/queries/supportedgrouping"), 
	GET_QUERY_VIEW_METRICS("/instances/queries/supportedmetrics"), 
	GET_QUERY_LIST("/instances/{instanceId}/queries/view/{viewId}/groupby/{groupId}/tzo/{tzo}"), //Offset Added
	GET_QUERY_PLAN("/instances/{instanceId}/queries/{queryId}/queryplan"),
	GET_QUERY_GRAPH("/instances/{instanceId}/graphdata/view/{viewId}/groupby/{groupId}/tzo/{tzo}"),//Offset Added
	CREATE_CUSTOMDASHBOARD("/customdashboard/create"),
	DELETE_CUSTOMDASHBOARD("/customdashboard/{customDashboardid}"),
	CREATE_WIDGET("/customdashboard/{customDashboardid}/widgets/create"),
	GET_CUSTOMDASHBOARD_WIDGETS("/customdashboard/{customDashboardid}/widgets"),
	GET_CUSTOMDASHBOARDS("/customdashboards"),
	GET_SCALE_FACTORS("/GetScaleFactors"),
	GET_ANALSIS_SUMMARY("/Instances/{instanceId}/GetConsolidatedInstanceOverview"),
	GET_MATCH_TYPES("/customdashboard/MatchTypes"),
	GET_WIDGET_TYPES("/customdashboard/WidgetTypes"),
	GET_WIDGET_DATA("/customdashboard/{customDashboardid}/widget/{widgetId}/tzo/{tzo}/data"),
	DELETE_WIDGET("/customdashboard/{customDashboardid}/widget/{widgetId}"),
	UPDATE_CUSTOMDASHBOARD("/customdashboard/{customDashboardid}/save"),
	UPDATE_SCALE_FACTORS("/UpdateScaleFactors"),
	SAVE_USER_SETTINGS("/SetUserSessionSettings"),
	GET_USER_SETTINGS("/GetUserSessionSettings"),
	UPDATE_WIDGET("/customdashboard/{customDashboardid}/widget/{widgetId}/save"),
	COPY_CUSTOMDASHBOARD("/customdashboard/{customDashboardid}/copy"),
	GET_CPU_STATS("/GetCPUStats"), 
	GET_OS_PAGING_STATS("/GetOSPaging"), 
    GET_DB_STATS("/GetDBStats"),
	GET_CUSTOM_COUNTER_STATS("/GetCustomCounterStats"),
	GET_NETWORK_STATS("/GetNetworkStats"),
	GET_FILE_STATS("/FileStats"),
    GET_LOCK_WAITS_STATS("/GetLockWaitsStats"),
	GET_VIRTUALIZATION_STATS("/GetVirtualizationStats");
	private String methodName;
	
	RestMethods(String methodName) {
		this.methodName = methodName;
	}
	
	public String getMethodName() {
		return methodName;
	}
}
