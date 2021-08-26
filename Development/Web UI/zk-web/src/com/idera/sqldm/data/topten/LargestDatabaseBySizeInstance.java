package com.idera.sqldm.data.topten;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm.utils.Utility;

@JsonIgnoreProperties(ignoreUnknown = true)
public class LargestDatabaseBySizeInstance extends IWidgetInstance {
	@JsonProperty("FileSizeInMB") private double fileSize ;
	@JsonProperty("Severity")	private int severity;
	@JsonProperty("DatabaseName") private String databaseName="------";
	
	public void setFileSize(int fileSize) {
		this.fileSize = fileSize;
	}

	public int getSeverity() {
		return severity;
	}

	public void setSeverity(int severity) {
		this.severity = severity;
	}
	public void setDatabaseName(String databaseName) {
		this.databaseName = databaseName;
	}
	public String getDatabaseName() {
		return databaseName;
	}
	public double getFileSize() {
		return Utility.round(fileSize, 2);
	}
}
