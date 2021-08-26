package com.idera.sqldm_10_3.data.category.resources;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm_10_3.data.topten.IWidgetInstance;

@JsonIgnoreProperties(ignoreUnknown = true)
public class TopSessionsIOActivity extends IWidgetInstance {
	@JsonProperty("SessionID")
	private int sessionId;

	@JsonProperty("PhysicalIO")
	private Double physicalIO;
	
	@JsonProperty("UTCCollectionDateTime")
	private String utcCollectionDateTime;

	@JsonProperty("Severity")	private int severity;

	public int getSessionId() {
		return sessionId;
	}

	public void setSessionId(int sessionId) {
		this.sessionId = sessionId;
	}

	public Double getPhysicalIO() {
		return physicalIO;
	}

	public void setPhysicalIO(Double physicalIO) {
		this.physicalIO = physicalIO;
	}

	public String getUtcCollectionDateTime() {
		return utcCollectionDateTime;
	}

	public void setUtcCollectionDateTime(String utcCollectionDateTime) {
		this.utcCollectionDateTime = utcCollectionDateTime;
	}

	public int getSeverity() {
		return severity;
	}

	public void setSeverity(int severity) {
		this.severity = severity;
	}

}
