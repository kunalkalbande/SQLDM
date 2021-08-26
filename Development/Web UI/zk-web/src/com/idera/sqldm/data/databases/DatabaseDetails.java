package com.idera.sqldm.data.databases;


import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class DatabaseDetails {

	@JsonProperty("DataSizeMB")
	private double dataSizeMB;
	public double getDataSizeMB() {
		return dataSizeMB;
	}

	@JsonProperty("LogSizeMB")
	private double logSizeMB;
	public double getLogSizeMB() {
		return logSizeMB;
	}
	
	@JsonProperty("Database")
	private Database database;
	public Database getDatabase() {
		return database;
	}
	
	@JsonProperty("DatabaseFiles")
	private List<DatabaseFile> databaseFiles;
	public List<DatabaseFile> getDatabaseFiles() {
		return databaseFiles;
	}

	@JsonProperty("InstanceEnabled")
	private boolean instanceEnabled;
	public boolean getInstanceEnabled() {
		return instanceEnabled;
	}

}
