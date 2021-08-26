package com.idera.sqldm_10_3.data.customdashboard;

import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.List;

public class CustomDashboardWidgetData {
	@JsonProperty("ServerID")
	private int serverID;
	@JsonProperty("WidgetID")
	private int widgetId;
	@JsonProperty("CustomDashboardId")
	private int customDashboardId;
	@JsonProperty("InstanceName")
	private String instanceName;
	@JsonProperty("MetricValuesforInstance")
	List<MetricValues> metricValuesforInstance;
	
	// @author Saumyadeep 
	// Friendly Begin
	
	@JsonProperty("FriendlyServerName")
	private String friendlyServerName;

	public String getFriendlyServerName() {
		return this.friendlyServerName;
	}

	private String displayName;
		
	public String getDisplayName() {
		if(this.getFriendlyServerName()!= null)	
			return this.getFriendlyServerName();
		else
			return this.getInstanceName();
	}
	// Friendly End
	
	public int getServerID() {
		return this.serverID;
	}
	public int getWidgetId() {
		return widgetId;
	}
	public int getCustomDashboardId() {
		return this.customDashboardId;
	}
	public String getInstanceName() {
		return this.instanceName;
	}
	public List<MetricValues> getmetricValuesforInstance() {
		return this.metricValuesforInstance;
	}
}
