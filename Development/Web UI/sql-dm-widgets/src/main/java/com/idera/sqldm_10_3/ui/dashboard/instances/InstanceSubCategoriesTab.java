package com.idera.sqldm_10_3.ui.dashboard.instances;

import java.util.EnumSet;
import java.util.HashMap;
import java.util.Map;

public enum InstanceSubCategoriesTab {

	OVERVIEW_SUMMARY(1,"overview", InstanceCategoryTab.OVERVIEW, ""),
	DATABASES_SUMMARY(1,"summary", InstanceCategoryTab.DATABASES, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/databases/summary.zul"),
	DATABASES_TEMPDB(2,"tempdb", InstanceCategoryTab.DATABASES, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/databases/tempdb.zul"),
	DATABASES_AVAILABILITYGROUPS(3,"availabilityGroups", InstanceCategoryTab.DATABASES, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/databases/availabilityGroups.zul"),
	QUERIES_SUMMARY(1,"summary", InstanceCategoryTab.QUERIES, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queries/summary.zul"),
	//QUERIES_WAITS(1,"waits", InstanceCategoryTab.QUERY_WAITS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queries/queryWaits.zul"),
	/*QUERIES_STATEMENTS(2,"statements", InstanceCategoryTab.QUERIES, "~./com/idera/sqldm/ui/dashboard/instances/queries/statements.zul"),
	QUERIES_OVERVIEW(3,"applications", InstanceCategoryTab.QUERIES, "~./com/idera/sqldm/ui/dashboard/instances/queries/applications.zul"),
	QUERIES_DATABASES(4,"databases", InstanceCategoryTab.QUERIES, "~./com/idera/sqldm/ui/dashboard/instances/queries/databases.zul"),
	QUERIES_CLIENTS(5,"clients", InstanceCategoryTab.QUERIES, "~./com/idera/sqldm/ui/dashboard/instances/queries/clients.zul"),
	QUERIES_SESSIONS(6,"sessions", InstanceCategoryTab.QUERIES, "~./com/idera/sqldm/ui/dashboard/instances/queries/sessions.zul"),
	QUERIES_USERS(7,"users", InstanceCategoryTab.QUERIES, "~./com/idera/sqldm/ui/dashboard/instances/queries/users.zul"),*/
	/*RESOURCES_SUMMARY(1,"summary", InstanceCategoryTab.RESOURCES,  "~./com/idera/sqldm/ui/dashboard/instances/resources/summary.zul"),*/
	RESOURCES_CPUVIEW(2,"cpuView", InstanceCategoryTab.RESOURCES,  "~./sqldm/com/idera/sqldm/ui/dashboard/instances/resources/cpuView.zul"),
	RESOURCES_MEMORYVIEW(3,"memoryView", InstanceCategoryTab.RESOURCES, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/resources/memoryView.zul"),
	RESOURCES_DISKVIEW(4,"diskView", InstanceCategoryTab.RESOURCES, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/resources/diskView.zul"),
	RESOURCES_FILEACTIVITY(5,"fileActivity", InstanceCategoryTab.RESOURCES, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/resources/fileActivity.zul"),
	RESOURCES_SERVERWAITS(6,"serverWaits", InstanceCategoryTab.RESOURCES, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/resources/serverWaits.zul"),
	SESSIONS_SUMMARY(1,"summary", InstanceCategoryTab.SESSIONS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/sessions/summary.zul"),
	SESSIONS_OVERVIEW(2,"overview", InstanceCategoryTab.SESSIONS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/sessions/details.zul"),
	SESSIONS_LOCKS(3,"summary", InstanceCategoryTab.SESSIONS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/sessions/locks.zul"),
	ALERTS_SUMMARY(1,"summary", InstanceCategoryTab.ALERTS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/alerts.zul"),
	SESSIONS_BLOCKING(4,"summary", InstanceCategoryTab.SESSIONS, "~./sqldm/com/idera/sqldm/ui/dashboard/instances/sessions/blocking.zul");

	private int subTabId;
	private String status;
	private InstanceCategoryTab ict;
	private String url;

	private InstanceSubCategoriesTab(int subTabId, String status, InstanceCategoryTab ict, String url) {
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

	private static final Map<String, InstanceSubCategoriesTab> lookup = new HashMap<>();
	private static final Map<String, InstanceSubCategoriesTab> lookupById = new HashMap<>();

	static {
		for(InstanceSubCategoriesTab tab : EnumSet.allOf(InstanceSubCategoriesTab.class)) {
		    lookup.put(tab.getStatus().toLowerCase(), tab);
		    lookupById.put(getKey(tab.getInstanceCategory().getTabId(), tab.getSubTabId()), tab);
		}
	}

	public static InstanceSubCategoriesTab findByStatus(String status) { 
		return lookup.get(status); 
	}
	
	public static InstanceSubCategoriesTab findById(int tabId, int subTabId) {
		return lookupById.get(getKey(tabId, subTabId));
	}
	private static String getKey(int tabId, int subTabId) {
		return tabId  + "_" + subTabId;
	}

}
