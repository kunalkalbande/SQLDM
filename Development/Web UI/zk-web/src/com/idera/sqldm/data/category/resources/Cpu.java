package com.idera.sqldm.data.category.resources;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown=true)
public class Cpu {
	@JsonProperty("OSCPUUsage") private Double OSCPUUsage;
	@JsonProperty("ProcessorQueueLength") private Double ProcessorQueueLength;
	@JsonProperty("SqlCPUUsage") private Double SqlCPUUsage;
	@JsonProperty("ProcessorUserTimePercent") private Double ProcessorTime;
	@JsonProperty("ProcessorPrivilegedTimePercent") private Double ProcessorPrivilegedTimePercent;
	
	public Double getOSCPUUsage() {
		return OSCPUUsage;
	}
	public void setOSCPUUsage(Double oSCPUUsage) {
		OSCPUUsage = oSCPUUsage;
	}
	public Double getProcessorQueueLength() {
		return ProcessorQueueLength;
	}
	public void setProcessorQueueLength(Double processorQueueLength) {
		ProcessorQueueLength = processorQueueLength;
	}
	public Double getSqlCPUUsage() {
		return SqlCPUUsage;
	}
	public void setSqlCPUUsage(Double sqlCPUUsage) {
		SqlCPUUsage = sqlCPUUsage;
	}
	public Double getProcessorTime() {
		return ProcessorTime;
	}
	public void setProcessorTime(Double processorTime) {
		ProcessorTime = processorTime;
	}
	public Double getProcessorPrivilegedTimePercent() {
		return ProcessorPrivilegedTimePercent;
	}
	public void setProcessorPrivilegedTimePercent(
			Double processorPrivilegedTimePercent) {
		ProcessorPrivilegedTimePercent = processorPrivilegedTimePercent;
	}
	
	
}
