package com.idera.sqldm.data.category.resources;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown=true)
public class FileActivityStatistics {
	@JsonProperty("FileReadsPerSec") private Double FileReadsPerSec;
	@JsonProperty("DiskReadsPerSec") private Long DiskReadsPerSec;
	@JsonProperty("FileWritesPerSec") private Double FileWritesPerSec;
	
	@JsonProperty("DiskWritesPerSec") private Long DiskWritesPerSec;
	public Double getFileReadsPerSec() {
		return FileReadsPerSec;
	}
	public void setFileReadsPerSec(Double fileReadsPerSec) {
		FileReadsPerSec = fileReadsPerSec;
	}
	public Long getDiskReadsPerSec() {
		return DiskReadsPerSec;
	}
	public void setDiskReadsPerSec(Long diskReadsPerSec) {
		DiskReadsPerSec = diskReadsPerSec;
	}
	public Double getFileWritesPerSec() {
		return FileWritesPerSec;
	}
	public void setFileWritesPerSec(Double fileWritesPerSec) {
		FileWritesPerSec = fileWritesPerSec;
	}
	public Long getDiskWritesPerSec() {
		return DiskWritesPerSec;
	}
	public void setDiskWritesPerSec(Long diskWritesPerSec) {
		DiskWritesPerSec = diskWritesPerSec;
	}
}
