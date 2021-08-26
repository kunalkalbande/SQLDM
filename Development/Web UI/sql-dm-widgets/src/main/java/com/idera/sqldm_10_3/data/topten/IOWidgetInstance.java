package com.idera.sqldm_10_3.data.topten;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown=true)
public class IOWidgetInstance extends IWidgetInstance {
	@JsonProperty("Severity")	private int severity;
	@JsonProperty("SQLPhysicalIO") private String physicalIO ;
	
	public int getSeverity() {
		return severity;
	}
	public void setSeverity(int severity) {
		this.severity = severity;
	}
	public String getPhysicalIO() {
		return physicalIO;
	}
	public void setPhysicalIO(String physicalIO) {
		this.physicalIO = physicalIO;
	}
}
