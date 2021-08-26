package com.idera.sqldm.data.topten;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm.utils.Utility;

@JsonIgnoreProperties(ignoreUnknown = true)
public class Sessions extends IWidgetInstance {
	@JsonProperty("SessionID") 	private int sessionId;
	@JsonProperty("CPUUsageInMillisec")	private Double sessionCPUUsage;
	@JsonProperty("Host")	private String host;
	@JsonProperty("Status")	private String status;
	@JsonProperty("DatabaseName")	private String database;
	@JsonProperty("Severity")	private int severity;
	@JsonProperty("UTCCollectionDateTime")	private String utcCollectionDateTime;
	
	public int getSeverity() {
		return severity;
	}

	public void setSeverity(int severity) {
		this.severity = severity;
	}
	
	public int getSessionId() {
		return sessionId;
	}

	public void setSessionId(int sessionId) {
		this.sessionId = sessionId;
	}

	public Double getSessionCPUUsage() {
		return Utility.round(sessionCPUUsage, 2);
	}

	public void setSessionCPUUsage(Double sessionCPUUsage) {
		this.sessionCPUUsage = sessionCPUUsage;
	}

	public String getHost() {
		return host;
	}

	public void setHost(String host) {
		this.host = host;
	}

	public String getStatus() {
		return status;
	}

	public void setStatus(String status) {
		this.status = status;
	}

	public String getDatabase() {
		return database;
	}

	public void setDatabase(String database) {
		this.database = database;
	}

	public String getUtcCollectionDateTime() {
		return utcCollectionDateTime;
	}
	public void setUtcCollectionDateTime(String utcCollectionDateTime) {
		this.utcCollectionDateTime = utcCollectionDateTime;
	}
	
		
}
