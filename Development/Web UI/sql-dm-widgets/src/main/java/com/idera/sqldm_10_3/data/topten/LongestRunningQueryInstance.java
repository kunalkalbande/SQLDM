package com.idera.sqldm_10_3.data.topten;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown=true)
public class LongestRunningQueryInstance extends IWidgetInstance {
	@JsonProperty("Severity")	private int severity;
	@JsonProperty("LogicalWrites") private Long logicalWrites;
	@JsonProperty("PhysicalReads") private Long physicalReads;
	@JsonProperty("Database") private String database;
	@JsonProperty("CPUTime") private String cpuTime;
	@JsonProperty("QueryText") private String queryText;
	@JsonProperty("QueryExecTimeInMs") private String queryExecTimeInMs;
	@JsonProperty("LogicalReads") private Long logicalReads;
	
	public int getSeverity() {
		return severity;
	}
	public void setSeverity(int severity) {
		this.severity = severity;
	}
	
	public String getDatabase() {
		return database;
	}
	public void setDatabase(String database) {
		this.database = database;
	}
	
	public String getQueryText() {
		return queryText;
	}
	public void setQueryText(String queryText) {
		this.queryText = queryText;
	}
	public Long getLogicalWrites() {
		return logicalWrites;
	}
	public void setLogicalWrites(Long logicalWrites) {
		this.logicalWrites = logicalWrites;
	}
	public Long getPhysicalReads() {
		return physicalReads;
	}
	public void setPhysicalReads(Long physicalReads) {
		this.physicalReads = physicalReads;
	}
	public String getCpuTime() {
		return cpuTime;
	}
	public void setCpuTime(String cpuTime) {
		this.cpuTime = cpuTime;
	}
	public String getQueryExecTimeInMs() {
		return queryExecTimeInMs;
	}
	public void setQueryExecTimeInMs(String queryExecTimeInMs) {
		this.queryExecTimeInMs = queryExecTimeInMs;
	}
	public Long getLogicalReads() {
		return logicalReads;
	}
	public void setLogicalReads(Long logicalReads) {
		this.logicalReads = logicalReads;
	}

}
