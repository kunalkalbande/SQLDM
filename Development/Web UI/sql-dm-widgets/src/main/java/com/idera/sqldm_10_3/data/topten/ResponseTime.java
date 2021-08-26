package com.idera.sqldm_10_3.data.topten;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm_10_3.data.alerts.Alert.AlertSeverity;

@JsonIgnoreProperties(ignoreUnknown = true)
public class ResponseTime extends IWidgetInstance {
	@JsonProperty("Severity") private int severity;
	@JsonProperty("ResponseTimeMillis") protected int responseTime;
	@JsonProperty("Criticality") protected AlertSeverity criticality;

	public int getSeverity() {
		return severity;
	}

	public void setSeverity(int severity) {
		this.severity = severity;
	}

	public int getResponseTime() {
		return responseTime;
	}

	public void setResponseTime(int responsetime) {
		this.responseTime = responsetime;
	}

	public AlertSeverity getCriticality() {
		return criticality;
	}

	public void setCriticality(AlertSeverity criticality) {
		this.criticality = criticality;
	}
}
