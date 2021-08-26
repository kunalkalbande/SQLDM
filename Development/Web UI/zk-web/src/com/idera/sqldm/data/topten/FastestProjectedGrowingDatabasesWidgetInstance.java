package com.idera.sqldm.data.topten;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown=true)
public class FastestProjectedGrowingDatabasesWidgetInstance extends IWidgetInstance {
	@JsonProperty("DatabaseName") private String databaseName;
	@JsonProperty("TotalSizeDiffernceKb")private long totalSizeDiffernceKb;
	@JsonProperty("Severity")	private int severity;
	//@JsonProperty("Severity")	private int severity;
	
	public int getSeverity() {
		return severity;
	}
	public void setSeverity(int severity) {
		this.severity = severity;
	}
	public String getDatabaseName() {
		return databaseName;
	}
	public void setDatabaseName(String databaseName) {
		this.databaseName = databaseName;
	}
	
	public long getTotalSizeDiffernceKb() {
		return totalSizeDiffernceKb;
	}
	public void setTotalSizeDiffernceKb(long totalSizeDiffernceKb) {
		this.totalSizeDiffernceKb = totalSizeDiffernceKb;
	}

//	public int getSeverity() {
//		return severity;
//	}
//	public void setSeverity(int severity) {
//		this.severity = severity;
//	}
}
