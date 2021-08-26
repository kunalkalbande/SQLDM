package com.idera.sqldm.data.instances;

import com.fasterxml.jackson.annotation.JsonProperty;

public class OsMetricsStatistics {
	@JsonProperty("PercentDiskIdleTime") private Float PercentDiskIdleTime;
	@JsonProperty("ProcessorQueueLength") private Integer ProcessorQueueLength;
	@JsonProperty("PercentUserTime") private Float PercentUserTime;
	@JsonProperty("PercentDiskTime") private Float PercentDiskTime;
	@JsonProperty("PagesPersec") private Float PagesPersec;
	@JsonProperty("TotalPhysicalMemory") private String TotalPhysicalMemory;
	@JsonProperty("PercentPrivilegedTime") private Float PercentPrivilegedTime;
	@JsonProperty("AvgDiskQueueLength") private Float AvgDiskQueueLength;
	@JsonProperty("PercentProcessorTime") private Float PercentProcessorTime;
	@JsonProperty("AvailableBytes") private String AvailableBytes;
	
	public Float getPercentDiskIdleTime() {
		return PercentDiskIdleTime;
	}
	public Integer getProcessorQueueLength() {
		return ProcessorQueueLength;
	}
	public Float getPercentUserTime() {
		return PercentUserTime;
	}
	public Float getPercentDiskTime() {
		return PercentDiskTime;
	}
	public Float getPagesPersec() {
		return PagesPersec;
	}
	public String getTotalPhysicalMemory() {
		return TotalPhysicalMemory;
	}
	public Float getPercentPrivilegedTime() {
		return PercentPrivilegedTime;
	}
	public Float getAvgDiskQueueLength() {
		return AvgDiskQueueLength;
	}
	public Float getPercentProcessorTime() {
		return PercentProcessorTime;
	}
	public String getAvailableBytes() {
//		if (AvailableBytes == null || AvailableBytes.length() == 0) {
//			AvailableBytes = "0";
//		}
//		return Long.parseLong(AvailableBytes);
		return AvailableBytes;
	}
	
	public Double getAvailableMemoryInMB(){
		if (AvailableBytes == null || AvailableBytes.length() == 0) {
			return 0d;
		}
		else{
			return (Math.round(Double.parseDouble(AvailableBytes)/1024*100.0))/100.0;
		}
	}
	
}
