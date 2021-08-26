package com.idera.sqldm.data.databases;

import com.fasterxml.jackson.annotation.JsonProperty;

public class DatabaseSummary {
	@JsonProperty("dataFileCount") 
	private Integer dataFileCount;
	@JsonProperty("logFileSpaceUsed") 
	private String logFileSpaceUsed;
	@JsonProperty("databaseCount") 
	private Integer databaseCount;
	@JsonProperty("dataFileSpaceUsed") 
	private String dataFileSpaceUsed;
	@JsonProperty("dataFileSpaceAllocated") 
	private String dataFileSpaceAllocated;
	@JsonProperty("logFileSpaceAllocated") 
	private String logFileSpaceAllocated;
	@JsonProperty("logFileCount") 
	private Integer logFileCount;
}
