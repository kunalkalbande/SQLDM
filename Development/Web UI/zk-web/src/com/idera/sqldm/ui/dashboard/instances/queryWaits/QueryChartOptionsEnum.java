package com.idera.sqldm.ui.dashboard.instances.queryWaits;

import java.util.EnumSet;
import java.util.HashMap;
import java.util.Map;

public enum QueryChartOptionsEnum {
	
	WAITS("Waits", "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queryWaits/waits.zul", 0, "getWaitType", "getWaitTypeID"),
	WAIT_CATEGORY("Wait Category", "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queryWaits/waitCategory.zul", 1, "getWaitCategory", "getWaitCategoryID"),
	STATEMENTS("Statements", "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queryWaits/statements.zul", 2, "getSQLStatement", "getSQLStatementID"),
	APPLICATION("Application", "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queryWaits/applications.zul", 3, "getApplicationName", "getApplicationID"),
	DATABASES("Databases", "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queryWaits/databases.zul", 4, "getDatabaseName", "getDatabaseID"),
	CLIENTS("Clients", "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queryWaits/clients.zul", 5, "getHostName", "getHostID"),
	SESSIONS("Sessions", "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queryWaits/sessions.zul", 6, "getSessionID", "getSessionID"),
	USERS("Users", "~./sqldm/com/idera/sqldm/ui/dashboard/instances/queryWaits/users.zul", 7, "getLoginName", "getLoginID");
	
	private String label;
	private String url;
	private int tabId;
	private String getterMethod;
	private String getterMethodId;
	
	private QueryChartOptionsEnum(String label, String url, int tabId, String getterMethod, String getterMethodId) {
		this.label = label;
		this.url = url;
		this.tabId = tabId;
		this.getterMethod = getterMethod;
		this.getterMethodId = getterMethodId;
	}

	public String getUrl() {
		return url;
	}

	public String getLabel() {
		return label;
	}
	
	public int getTabId() {
		return tabId;
	}
	
	public String getGetterMethod() {
		return getterMethod;
	}

	public String getGetterMethodId() {
		return getterMethodId;
	}

	public static final Map<String, QueryChartOptionsEnum> lookupByLabel = new HashMap<>();

	static {
		for(QueryChartOptionsEnum option : EnumSet.allOf(QueryChartOptionsEnum.class)) {
			lookupByLabel.put(option.getLabel().toLowerCase(), option);
		}
	}
	public static QueryChartOptionsEnum findByLabel(String label) { 
		return lookupByLabel.get(label.toLowerCase()); 
	}		

}
