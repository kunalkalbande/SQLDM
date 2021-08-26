package com.idera.sqldm.data.topten;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown=true)
public class InstanceAlertWidgetInstance extends IWidgetInstance {
	@JsonProperty("MaxSeverity")	private int severity;
	@JsonProperty("AlertCount") private int numberOfAlerts ;
	
	
	public int getSeverity() {
		return severity;
	}
	public void setSeverity(int severity) {
		this.severity = severity;
	}
	public int getNumberOfAlerts() {
		return numberOfAlerts;
	}
	public void setNumberOfAlerts(int numberOfAlerts) {
		this.numberOfAlerts = numberOfAlerts;
	}
}
