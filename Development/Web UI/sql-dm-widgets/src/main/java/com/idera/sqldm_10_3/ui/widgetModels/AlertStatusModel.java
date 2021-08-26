package com.idera.sqldm_10_3.ui.widgetModels;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class AlertStatusModel {

	private int totalAlerts;
	
	@JsonProperty("CriticalAlertCount")
	private int totalCriticalAlerts;
	
	@JsonProperty("WarningAlertCount")
	private int totalWarningAlerts;
	
	@JsonProperty("InformationalAlertCount")
	private int totalInformationalAlerts;
	
	public int getTotalAlerts() {
		return totalCriticalAlerts + totalWarningAlerts + totalInformationalAlerts;
	}

	public void setTotalAlerts(int totalAlerts) {
		this.totalAlerts = totalAlerts;
	}

	public int getTotalCriticalAlerts() {
		return totalCriticalAlerts;
	}

	public void setTotalCriticalAlerts(int totalCriticalAlerts) {
		this.totalCriticalAlerts = totalCriticalAlerts;
	}

	public int getTotalWarningAlerts() {
		return totalWarningAlerts;
	}

	public void setTotalWarningAlerts(int totalWarningAlerts) {
		this.totalWarningAlerts = totalWarningAlerts;
	}

	public int getTotalInformationalAlerts() {
		return totalInformationalAlerts;
	}

	public void setTotalInformationalAlerts(int totalInformationalAlerts) {
		this.totalInformationalAlerts = totalInformationalAlerts;
	}

}
