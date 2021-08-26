package com.idera.sqldm.ui.dashboard.instances;

import java.util.EnumSet;
import java.util.HashMap;
import java.util.Map;

public enum InstanceCategoryTab {

	QUERIES(2,"queries"),
	QUERY_WAITS(6,"queryWaits"),
	RESOURCES(3,"resources"),
	SESSIONS(1,"sessions"),
	DATABASES(4,"databases"),
	OVERVIEW(0,"overview"),
	ALERTS(5,"alerts");
	
	private int tabId;
	private String status;
	
	private InstanceCategoryTab(int tabId, String status) {
		this.tabId = tabId;
		this.status = status; 
	}

	public int getTabId() {
		return tabId;
	}
	public int getId() {
		return tabId;
	}


	public String getStatus() {
		return status;
	}

	private static final Map<String, InstanceCategoryTab> lookup = new HashMap<>();
	private static final Map<Integer, InstanceCategoryTab> lookupById = new HashMap<>();

	static {
		for(InstanceCategoryTab tab : EnumSet.allOf(InstanceCategoryTab.class)) {
		    lookup.put(tab.getStatus().toLowerCase(), tab);
		    lookupById.put(tab.getTabId(), tab);
		}
	}

	public static InstanceCategoryTab findByStatus(String status) { 
		return lookup.get(status); 
	}
	
	public static InstanceCategoryTab findById(int input) {
		return lookupById.get(input);
	}
}
