package com.idera.sqldm.data.queries;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.annotation.JsonInclude.Include;

@JsonIgnoreProperties(ignoreUnknown = true)
public class FilteredInstanceDatabasesProperties {

	@JsonProperty("ID")
	@JsonInclude(Include.NON_DEFAULT)
	private int id;
	
	@JsonProperty("DataSizeMB")
	private long dataSize;
	
	@JsonProperty("DatabaseSizeMB")
	private long databaseSize;
	
	@JsonProperty("InstanceID")
	private int instanceID;
	
	@JsonProperty("InstanceName")
	private String instanceName;
	
	@JsonProperty("LogSizeMB")
	private long logSize;
	
	@JsonProperty("SqlVersion")
	private String sqlVersion;
	
	@JsonProperty("TransactionsPerSec")
	private long transactionsPerSec;

	@JsonProperty("Name")
	private String name;
	
	@JsonProperty("Enabled")
	private boolean enabled;
	
	@JsonProperty("Availability")
	private int availability;

	// @author Saumyadeep 
	// Friendly Begin
	@JsonProperty("FriendlyServerName")
	private String friendlyServerName;

	public String getFriendlyServerName() {
		return this.friendlyServerName;
	}

	private String displayName;
	
	public String getDisplayName() {
		if(this.getFriendlyServerName()!= null)	
			return this.getFriendlyServerName();
		else
			return this.getInstanceName();
	}
	// Friendly End
	
	
	public boolean isEnabled() {
		return enabled;
	}

	public int getAvailability() {
		return availability;
	}

	public int getId() {
		return id;
	}

	public long getDataSize() {
		return dataSize;
	}

	public long getDatabaseSize() {
		return databaseSize;
	}

	public int getInstanceID() {
		return instanceID;
	}

	public String getInstanceName() {
		return instanceName;
	}

	public long getLogSize() {
		return logSize;
	}

	public String getSqlVersion() {
		return sqlVersion;
	}

	public long getTransactionsPerSec() {
		return transactionsPerSec;
	}

	public String getName() {
		return name;
	}
}
