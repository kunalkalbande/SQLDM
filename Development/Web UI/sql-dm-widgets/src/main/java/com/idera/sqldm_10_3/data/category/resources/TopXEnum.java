package com.idera.sqldm_10_3.data.category.resources;

import com.fasterxml.jackson.core.type.TypeReference;
import com.idera.server.web.WebConstants;
import com.idera.sqldm_10_3.data.topten.*;
import com.idera.sqldm_10_3.rest.RestMethods;
import com.idera.sqldm_10_3.data.topten.LongestRunningQueryInstance;
import com.idera.sqldm_10_3.data.topten.Sessions;

import java.util.List;

@SuppressWarnings("rawtypes")
public enum TopXEnum {
	//offset
	RESPONSE_TIME_WIDGET("changeRTRowCount", "responseTimeLimit", 10, 50, -1, RestMethods.GET_LATEST_RESPONSE_TIME_BY_INSTANCE, ResponseTime.class, new TypeReference<List<ResponseTime>>() {}),
	DATABASE_WITH_MOST_ALERTS("changeDBMARowCount", "databaseWithMostAlertsLimit", 10, 50, -1, RestMethods.GET_TOP_DATABASE_BY_ALERT_INSTANCES, TopDatabaseByAlertWidgetInstance.class, new TypeReference<List<TopDatabaseByAlertWidgetInstance>>() {}),
	//offset
	MOST_ACTIVE_CONNECTIONS("changeACRowCount", "mostActiveConnectionsLimit", 10, 50, -1, RestMethods.GET_MOST_ACTIVE_CONNECTION, MostActiveConnection.class, new TypeReference<List<MostActiveConnection>>() {}),
	//offset
	BLOCKED_SESSIONS("changeBSRowCount", "blockedSessionsLimit", 10, 50, -1, RestMethods.GET_BLOCKED_SESSION, BlockedSession.class, new TypeReference<List<BlockedSession>>() {}),
	//offset
	LARGEST_DATABASES_BY_SIZE("changeLDBSRowCount", "largestDBBySizeLimit", 10, 50, -1, RestMethods.GET_LARGESTDBS_SIZE_INSTANCES, LargestDatabaseBySizeInstance.class, new TypeReference<List<LargestDatabaseBySizeInstance>>() {}),
	SQL_MEMORY_USAGE("changeSMURowCount", "sqlMemoryUsageLimit", 10, 50, -1, RestMethods.GET_HIGHEST_MEMORY_USAGE, MemoryUsageWidgetInstance.class, new TypeReference<List<MemoryUsageWidgetInstance>>() {}),
	IO("changeIORowCount", "ioLimit", 10, 50, -1, RestMethods.GET_HIGHEST_IO_INSTANCES, IOWidgetInstance.class, new TypeReference<List<IOWidgetInstance>>() {}),
	//offset
	TOP_DATABASES_BY_ACTIVITY("changeTDBARowCount", "topDBByActivityLimit", 10, 50, -1, RestMethods.GET_DATABASE_BY_ACTIVITY_INSTANCES, TopDatabaseByActivityWidgetInstance.class, new TypeReference<List<TopDatabaseByActivityWidgetInstance>>() {}),
	//offset
	FASTEST_PROJECTED_GROWING_DATABASES("changeFPGDRowCount", "fastestProjectedDBLimit", 10, 50, 7, RestMethods.GET_FASTEST_GROWING_DATABASE_INSTANCES, FastestProjectedGrowingDatabasesWidgetInstance.class, new TypeReference<List<FastestProjectedGrowingDatabasesWidgetInstance>>() {}),
	//offset
	TEMPDB_UTILIZATION("changeTDBURowCount", "tempDBUtilizationLimit", 10, 50, -1, RestMethods.GET_TEMPDB_UTILIZATION_INSTANCES, TempDBUtilizationInstance.class, new TypeReference<List<TempDBUtilizationInstance>>() {}),
	SQL_CPU_LOAD("changeSCLRowCount", "sqlCPULoadLimit", 10, 50, -1, RestMethods.GET_HIGHEST_CPU_LOAD, CpuLoad.class, new TypeReference<List<CpuLoad>>() {}),
	//offset
	QUERY_MONITOR_EVENTS("changeQMERowCount", "queryMonitorLimit", 10, 50, -1, RestMethods.GET_HIGHEST_QUERY_INSTANCES, QueryWidgetInstance.class, new TypeReference<List<QueryWidgetInstance>>() {}),
	//offset
	DISK_SPACE("changeDSRowCount", "diskSpaceLimit", 10, 50, -1, RestMethods.GET_HIGHEST_DISK_SPACE_INSTANCES, DiskSpaceWidgetInstance.class, new TypeReference<List<DiskSpaceWidgetInstance>>() {}),
	//offset
	SESSIONS("changeSESSIONRowCount", "sessionsLimit", 10, 50, -1, RestMethods.GET_SESSION_BY_CPU, Sessions.class, new TypeReference<List<Sessions>>() {}),
	INSTANCE_ALERT("changeIARowCount", "instanceAlertLimit", 10, 50, -1, RestMethods.GET_MOST_ALERT_INSTANCES, InstanceAlertWidgetInstance.class, new TypeReference<List<InstanceAlertWidgetInstance>>() {}),
	WAITS("changeWAITSRowCount", "waitsLimit", 10, 50, -1, RestMethods.GET_LONGEST_WAIT_INSTANCES, WaitWidgetInstance.class, new TypeReference<List<WaitWidgetInstance>>() {}),
	//offset
	LONGEST_RUNNING_QUERIES("changeLRQRowCount", "longestRunningLimit", 10, 50, -1, RestMethods.GET_LONGEST_RUNNING_QUERY, LongestRunningQueryInstance.class, new TypeReference<List<LongestRunningQueryInstance>>() {}),
	//offset
	NUMBER_OF_SESSIONS("changeNSRowCount", "numberOfSessionsLimit", 10, 50, -1, RestMethods.GET_TOTAL_SESSION_INSTANCES, TotalSessionWidgetInstance.class, new TypeReference<List<TotalSessionWidgetInstance>>() {}),
	//offset
	INSTANCE_SESSIONS_CPU_ACTIVITY("changeSCARowCount", "sessionsCpuLimit", 10, 50, -1, RestMethods.GET_INSTANCE_SESSION_BY_CPU_ACTIVITY, TopSessionsCpuActivity.class, new TypeReference<List<TopSessionsCpuActivity>>() {}),
	//offset
	INSTANCE_SESSIONS_IO_ACTIVITY("changeSIOARowCount", "sessionsIOLimit", 10, 50, -1, RestMethods.GET_INSTANCE_SESSION_BY_IO_ACTIVITY, TopSessionsIOActivity.class, new TypeReference<List<TopSessionsIOActivity>>() {});
	
	private String rowCountModEventName;
	private String rowCountVariableName;
	private int rowDefaultCount;
	private int rowMaxCount;
	private int rowDefaultDateTime;
	private RestMethods restMethod;
	private Class beanClassName;
	private TypeReference typeReference;
	private int instanceId;
	private String OFFSET_IN_HOURS = "0.0";
	private TopXEnum(String rowCountModEventName, String rowCountVariableName, int rowDefaultCount, int rowMaxCount, int rowDefaultDateTime, RestMethods restMethod, Class beanClassName, TypeReference typeReference) {
		setOffSet();
		this.rowCountModEventName = rowCountModEventName;
		this.rowCountVariableName = rowCountVariableName;
		this.rowDefaultCount = rowDefaultCount;
		this.rowMaxCount = rowMaxCount;
		this.rowDefaultDateTime = rowDefaultDateTime;
		this.restMethod = restMethod;
		this.beanClassName = beanClassName;
		this.typeReference = typeReference;
		this.instanceId = -1;
		
	}

	public String getRowCountModEventName() {
		return rowCountModEventName;
	}
	public String getRowCountVariableName() {
		return rowCountVariableName;
	}
	public int getRowDefaultCount() {
		return rowDefaultCount;
	}
	public String getRowTimeDaysVariableName() {
		return "days";
	}
	public int getRowMaxCount() {
		return rowMaxCount;
	}
	public int getRowDefaultDateTime() {
		return rowDefaultDateTime;
	}
	public RestMethods getRestMethod() {
		return restMethod;
	}
	public Class getBeanClassName() {
		return beanClassName;
	}
	public TypeReference getTypeReference() {
		return typeReference;
	}

	public int getInstanceId() {
		return instanceId;
	}

	public void setInstanceId(int instanceId) {
		this.instanceId = instanceId;
	}

	private void setOffSet(){
		Double offSet = null;
		if(org.zkoss.zk.ui.Sessions.getCurrent()!=null)
		{
			offSet = new Double((Integer)org.zkoss.zk.ui.Sessions.getCurrent().getAttribute(WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET))/(1000*60.0*60.0);
			offSet = -offSet;
		}
	if(offSet!=null)
	OFFSET_IN_HOURS = offSet.toString();
	}

	public String getOFFSET_IN_HOURS() {
		return OFFSET_IN_HOURS;
	}
}
