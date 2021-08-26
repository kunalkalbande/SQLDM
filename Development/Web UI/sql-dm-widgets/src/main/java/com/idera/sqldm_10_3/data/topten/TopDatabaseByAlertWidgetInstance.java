package com.idera.sqldm_10_3.data.topten;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown=true)
public class TopDatabaseByAlertWidgetInstance extends IWidgetInstance {
	@JsonProperty("MaxSeverity")	private int severity;
	@JsonProperty("AlertCount") private Long numberOfAlerts ;
	@JsonProperty("DatabaseName")	private String databaseName="------";
	
	public int getSeverity() {
		return severity;
	}
	public void setSeverity(int severity) {
		this.severity = severity;
	}
	
	public Long getNumberOfAlerts() {
		return numberOfAlerts;
	}
	public void setNumberOfAlerts(Long numberOfAlerts) {
		this.numberOfAlerts = numberOfAlerts;
	}
	public String getDatabaseName() {
		return databaseName;
	}
	public void setDatabaseName(String databaseName) {
		this.databaseName = databaseName;
	}
}
