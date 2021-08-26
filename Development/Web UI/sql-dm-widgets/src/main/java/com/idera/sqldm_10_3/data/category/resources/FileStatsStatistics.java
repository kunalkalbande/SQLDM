package com.idera.sqldm_10_3.data.category.resources;

import com.fasterxml.jackson.annotation.JsonProperty;

public class FileStatsStatistics {

	@JsonProperty("DiskReadsPerSec") private Double diskReadsPerSec;
	@JsonProperty("DiskWritesPerSec") private Double diskWritesPerSec;
	@JsonProperty("FileReadsPerSec") private Double fileReadsPerSec;
	@JsonProperty("FileWritesPerSec") private Double fileWritesPerSec;
	public Double getDiskReadsPerSec() {
		return diskReadsPerSec;
	}
	public void setDiskReadsPerSec(Double diskReadsPerSec) {
		this.diskReadsPerSec = diskReadsPerSec;
	}
	public Double getDiskWritesPerSec() {
		return diskWritesPerSec;
	}
	public void setDiskWritesPerSec(Double diskWritesPerSec) {
		this.diskWritesPerSec = diskWritesPerSec;
	}
	public Double getFileReadsPerSec() {
		return fileReadsPerSec;
	}
	public void setFileReadsPerSec(Double fileReadsPerSec) {
		this.fileReadsPerSec = fileReadsPerSec;
	}
	public Double getFileWritesPerSec() {
		return fileWritesPerSec;
	}
	public void setFileWritesPerSec(Double fileWritesPerSec) {
		this.fileWritesPerSec = fileWritesPerSec;
	}
	
}
