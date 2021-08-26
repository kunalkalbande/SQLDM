package com.idera.sqldm.data.instances;

import com.fasterxml.jackson.annotation.JsonProperty;

public class DatabaseSummary {
	@JsonProperty("dataFileCount") private Integer dataFileCount;
	@JsonProperty("logFileSpaceUsed") private String logFileSpaceUsed;
	@JsonProperty("databaseCount") private Integer databaseCount;
	@JsonProperty("dataFileSpaceUsed") private String dataFileSpaceUsed;
	@JsonProperty("dataFileSpaceAllocated") private String dataFileSpaceAllocated;
	@JsonProperty("logFileSpaceAllocated") private String logFileSpaceAllocated;
	@JsonProperty("logFileCount") private Integer logFileCount;
	public Integer getDataFileCount() {
		return dataFileCount;
	}
	public String getLogFileSpaceUsed() {
		return logFileSpaceUsed;
	}
	public Integer getDatabaseCount() {
		return databaseCount;
	}
	public String getDataFileSpaceUsed() {
		return dataFileSpaceUsed;
	}
	public String getDataFileSpaceAllocated() {
		return dataFileSpaceAllocated;
	}
	public String getLogFileSpaceAllocated() {
		return logFileSpaceAllocated;
	}
	public Integer getLogFileCount() {
		return logFileCount;
	}
	
	
}
