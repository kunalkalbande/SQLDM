package com.idera.sqldm.ui.dashboard.instances;

import java.util.EnumSet;
import java.util.HashMap;
import java.util.Map;

public enum InstanceLeftPanelSubCategoryTab {

	OVERVIEW_SUMMARY(1,"overviewInstanceName", InstanceCategoryTab.OVERVIEW, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/overview.zul"),
	DATABASES_SUMMARY(1,"databasesSummary", InstanceCategoryTab.DATABASES, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/databases.zul"),
	DATABASES_TEMPDB(2,"databasesTempdb", InstanceCategoryTab.DATABASES, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/databases.zul"),
	DATABASES_AVAILABILITYGROUPS(3,"databasesAvailabilityGroups", InstanceCategoryTab.DATABASES, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/databases.zul"),
	QUERIES_SUMMARY(1,"queriesTab1", InstanceCategoryTab.QUERIES, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queries.zul"),
	QUERIES_WAITS_QUERIES_WAITS(0,"querieswaitWaits", InstanceCategoryTab.QUERY_WAITS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queryWaits.zul"),
	QUERIES_WAITS_WAIT_CATEGORY(1,"querieswaitWaitCategory", InstanceCategoryTab.QUERY_WAITS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queryWaits.zul"),
	QUERIES_WAITS_STATEMENTS(2,"querieswaitStatements", InstanceCategoryTab.QUERY_WAITS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queryWaits.zul"),
	QUERIES_WAITS_APPLICATION(3,"querieswaitApplication", InstanceCategoryTab.QUERY_WAITS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queryWaits.zul"),
	QUERIES_WAITS_DATABASES(4,"querieswaitDatabases", InstanceCategoryTab.QUERY_WAITS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queryWaits.zul"),
	QUERIES_WAITS_CLIENTS(5,"querieswaitClients", InstanceCategoryTab.QUERY_WAITS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queryWaits.zul"),
	QUERIES_WAITS_SESSIONS(6,"querieswaitSessions", InstanceCategoryTab.QUERY_WAITS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queryWaits.zul"),
	QUERIES_WAITS_USERS(7,"querieswaitUsers", InstanceCategoryTab.QUERY_WAITS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queryWaits.zul"),
	/*RESOURCES_SUMMARY(1,"summary", InstanceCategoryTab.RESOURCES,  "~./com/idera/sqldm/ui/dashboard/instances/resources/summary.zul"),*/
	RESOURCES_CPUVIEW(2,"resourcesCpu", InstanceCategoryTab.RESOURCES,  "~./sqldm/com/idera/sqldm/ui/dashboard/instances/resources.zul"),
	RESOURCES_MEMORYVIEW(3,"resourcesMemory", InstanceCategoryTab.RESOURCES, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/resources.zul"),
	RESOURCES_DISKVIEW(4,"resourcesDisk", InstanceCategoryTab.RESOURCES, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/resources.zul"),
	RESOURCES_FILEACTIVITY(5,"resourcesServerWaits", InstanceCategoryTab.RESOURCES, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/resources.zul"),
	RESOURCES_SERVERWAITS(6,"resourcesServerWaits", InstanceCategoryTab.RESOURCES, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/resources.zul"),
	SESSIONS_SUMMARY(1,"sessionsOverview", InstanceCategoryTab.SESSIONS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/sessions.zul"),
	SESSIONS_OVERVIEW(2,"sessionsSessionGraph", InstanceCategoryTab.SESSIONS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/sessions.zul"),
	SESSIONS_LOCKS(3,"summary", InstanceCategoryTab.SESSIONS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/sessions.zul"),
	ALERTS_SUMMARY(1,"alertsAlerts", InstanceCategoryTab.ALERTS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/alerts.zul"),
	SESSIONS_BLOCKING(4,"summary", InstanceCategoryTab.SESSIONS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/sessions.zul");

	private int subTabId;
	private String status;
	private InstanceCategoryTab ict;
	private String url;

	private InstanceLeftPanelSubCategoryTab(int subTabId, String status, InstanceCategoryTab ict, String url) {
		this.subTabId = subTabId;
		this.status = status; 
		this.ict = ict;
		this.url  = url;
	}
	
	public String getUrl() {
		return url;
	}
	
	public int getSubTabId() {
		return subTabId;
	}

	public String getStatus() {
		return status;
	}
	public InstanceCategoryTab getInstanceCategory() {
		return ict;
	}

	private static final Map<String, InstanceLeftPanelSubCategoryTab> lookup = new HashMap<>();
	private static final Map<String, InstanceLeftPanelSubCategoryTab> lookupById = new HashMap<>();

	static {
		for(InstanceLeftPanelSubCategoryTab tab : EnumSet.allOf(InstanceLeftPanelSubCategoryTab.class)) {
		    lookup.put(tab.getStatus().toLowerCase(), tab);
		    lookupById.put(getKey(tab.getInstanceCategory().getTabId(), tab.getSubTabId()), tab);
		}
	}

	public static InstanceLeftPanelSubCategoryTab findByStatus(String status) { 
		return lookup.get(status); 
	}
	
	public static InstanceLeftPanelSubCategoryTab findById(int tabId, int subTabId) {
		return lookupById.get(getKey(tabId, subTabId));
	}
	private static String getKey(int tabId, int subTabId) {
		return tabId  + "_" + subTabId;
	}
}
