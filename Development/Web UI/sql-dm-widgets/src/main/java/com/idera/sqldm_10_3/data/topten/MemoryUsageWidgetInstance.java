package com.idera.sqldm_10_3.data.topten;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm_10_3.utils.Utility;

@JsonIgnoreProperties(ignoreUnknown=true)
public class MemoryUsageWidgetInstance extends IWidgetInstance {
	@JsonProperty("SqlMemoryUsedInMB") private float memoryUsage;
	@JsonProperty("SqlMemoryAllocatedInMB") private float memoryAllocated;
	@JsonProperty("Severity")	private int severity;
	
	public double getMemoryUsage() {
		return Utility.round(memoryUsage, 2);
	}
	public void setMemoryUsage(float memoryUsage) {
		this.memoryUsage = memoryUsage;
	}
	public double getMemoryAllocated() {
		return Utility.round(memoryAllocated, 2);
	}
	public void setMemoryAllocated(float memoryAllocated) {
		this.memoryAllocated = memoryAllocated;
	}
	public int getSeverity() {
		return severity;
	}
	public void setSeverity(int severity) {
		this.severity = severity;
	}
}
