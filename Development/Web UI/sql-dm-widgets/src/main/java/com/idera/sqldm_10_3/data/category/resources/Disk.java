package com.idera.sqldm_10_3.data.category.resources;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.List;
import java.util.Map;

@JsonIgnoreProperties(ignoreUnknown=true)
public class Disk {
	@JsonProperty("DiskWritesPerSecond") private List<DiskIOPerSecondEntry> DiskWritesPerSecond;
	@JsonProperty("DiskReadsPerSecond") private List<DiskIOPerSecondEntry> DiskReadsPerSecond;
	@JsonProperty("DiskTransfersPerSecond") private List<DiskIOPerSecondEntry> DiskTransfersPerSecond;
	@JsonProperty("SqlPhysicalIO") private Map<String, Double> SqlPhysicalIO;
	@JsonProperty("AverageDiskMillisecondsPerRead") private List<DiskIOPerSecondEntry> avgDiskMsPerRead;
	@JsonProperty("AverageDiskMillisecondsPerWrite") private List<DiskIOPerSecondEntry> avgDiskMsPerWrite;
	@JsonProperty("AverageDiskMillisecondsPerTransfers") private List<DiskIOPerSecondEntry> avgDiskMsPerTransfer;
	
	public List<DiskIOPerSecondEntry> getDiskWritesPerSecond() {
		return DiskWritesPerSecond;
	}
	public void setDiskWritesPerSecond(
			List<DiskIOPerSecondEntry> diskWritesPerSecond) {
		DiskWritesPerSecond = diskWritesPerSecond;
	}
	public List<DiskIOPerSecondEntry> getDiskReadsPerSecond() {
		return DiskReadsPerSecond;
	}
	public void setDiskReadsPerSecond(List<DiskIOPerSecondEntry> diskReadsPerSecond) {
		DiskReadsPerSecond = diskReadsPerSecond;
	}
	public Map<String, Double> getSqlPhysicalIO() {
		return SqlPhysicalIO;
	}
	public void setSqlPhysicalIO(Map<String, Double> sqlPhysicalIO) {
		SqlPhysicalIO = sqlPhysicalIO;
	}
	public List<DiskIOPerSecondEntry> getDiskTransfersPerSecond() {
		return DiskTransfersPerSecond;
	}
	public void setDiskTransfersPerSecond(
			List<DiskIOPerSecondEntry> diskTransfersPerSecond) {
		DiskTransfersPerSecond = diskTransfersPerSecond;
	}
	public List<DiskIOPerSecondEntry> getAvgDiskMsPerRead() {
		return avgDiskMsPerRead;
	}
	public void setAvgDiskMsPerRead(List<DiskIOPerSecondEntry> avgDiskMsPerRead) {
		this.avgDiskMsPerRead = avgDiskMsPerRead;
	}
	public List<DiskIOPerSecondEntry> getAvgDiskMsPerWrite() {
		return avgDiskMsPerWrite;
	}
	public void setAvgDiskMsPerWrite(List<DiskIOPerSecondEntry> avgDiskMsPerWrite) {
		this.avgDiskMsPerWrite = avgDiskMsPerWrite;
	}
	public List<DiskIOPerSecondEntry> getAvgDiskMsPerTransfer() {
		return avgDiskMsPerTransfer;
	}
	public void setAvgDiskMsPerTransfer(
			List<DiskIOPerSecondEntry> avgDiskMsPerTransfer) {
		this.avgDiskMsPerTransfer = avgDiskMsPerTransfer;
	}
}
