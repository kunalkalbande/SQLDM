package com.idera.sqldm.data.topten;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm.utils.Utility;

@JsonIgnoreProperties(ignoreUnknown = true)
public class WaitWidgetInstance extends IWidgetInstance {
	@JsonProperty("WaitTimeInMs") private double waitTime ;
	@JsonProperty("Application") private String application;
	@JsonProperty("Severity")	private int severity;
	
	public int getSeverity() {
		return severity;
	}

	public void setSeverity(int severity) {
		this.severity = severity;
	}
	
	public void setWaitTime(double waitTime) {
		this.waitTime = waitTime;
	}
	
	
	public void setApplication(String application) {
		this.application = application;
	}

	
	public double getWaitTime() {
		return Utility.round(waitTime, 2);
	}
	
	public String getApplication() {
		return application;
	}
}
