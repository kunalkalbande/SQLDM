package com.idera.sqldm_10_3.data.topten;

import com.fasterxml.jackson.annotation.JsonProperty;

public class TotalSessionWidgetInstance extends IWidgetInstance {
	@JsonProperty("Severity")	private int severity;
	@JsonProperty("SessionIDCount") private Integer numberOfSessions;
	@JsonProperty("UTCCollectionDateTime") private String utcCollectionDateTime;
	
	public int getSeverity() {
		return severity;
	}
	public void setSeverity(int severity) {
		this.severity = severity;
	}
	public Integer getNumberOfSessions() {
		return numberOfSessions;
	}
	public void setNumberOfSessions(Integer numberOfSessions) {
		this.numberOfSessions = numberOfSessions;
	}
	public String getUtcCollectionDateTime() {
		return utcCollectionDateTime;
	}
	public void setUtcCollectionDateTime(String utcCollectionDateTime) {
		this.utcCollectionDateTime = utcCollectionDateTime;
	}
}
