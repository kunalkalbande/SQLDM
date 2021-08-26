package com.idera.sqldm.data;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm.data.instances.ServerStatus;

public class DashboardSeverityGroup {
	
	@JsonProperty("Severity")
	private String Severity;
	@JsonProperty("SqlServerCollection")
	private List<ServerStatus> SqlServerCollection;
	
	private String highestStatus;
	public String getSeverity() {
		return Severity;
	}

	public List<ServerStatus> getSqlServerCollection() {
		return SqlServerCollection;
	}

	public void setSeverity(String severity) {
		Severity = severity;
	}

	public void setSqlServerCollection(List<ServerStatus> sqlServerCollection) {
		SqlServerCollection = sqlServerCollection;
	}

	public String getImageName(){
		switch(Severity.toLowerCase()){
		case "all" : {
			switch(highestStatus.toLowerCase()) {
				case "critical": return "ServerGroupCritical16x16";
				case "ok" : return "ServerGroupOK16x16";
				case "warning" : return "ServerGroupWarning16x16";
				case "maintenance" : return "ServerMaintenanceMode";				
			}
		};
		case "critical" : return "ServerGroupCritical16x16";
		case "ok" : return "ServerGroupOK16x16";
		case "warning" : return "ServerGroupWarning16x16";
		case "maintenance" : return "ServerMaintenanceMode";
		}
		return "";
	}
	public void setHighestStatus(String status) {
		this.highestStatus = status;
	}
	public String getLabel(){
		switch(Severity.toLowerCase()){
		case "all" : return "Labels.all";
		case "critical" : return "Labels.view-critical";
		case "ok" : return "Labels.view-ok";
		case "warning" : return "Labels.warning";
		case "maintenance" : return "Labels.view-maintenance-mode";
		}
		return "";
	}
	public int getCount(){
		return this.SqlServerCollection.size();
	}
}
