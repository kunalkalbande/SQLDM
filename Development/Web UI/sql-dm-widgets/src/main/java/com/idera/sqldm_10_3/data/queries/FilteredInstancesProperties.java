package com.idera.sqldm_10_3.data.queries;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.annotation.JsonInclude.Include;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class FilteredInstancesProperties {
	
	@JsonProperty("ID")
	@JsonInclude(Include.NON_DEFAULT)
	private int id;
	
	@JsonProperty("Name")
	private String name;
	
	@JsonProperty("DatabaseCount")
	private long databaseCount;
	
	@JsonProperty("ResponseTime")
	private int responseTime;
	
	@JsonProperty("SqlVersion")
	private String sqlVersion;

	@JsonProperty("TotalDataSizeMB")
	private long totalDataSize;
	
	@JsonProperty("TotalDatabaseSizeMB")
	private long totalDatabaseSize;
	
	@JsonProperty("TotalLogSizeMB")
	private long totalLogSize;
	
	@JsonProperty("Transactions")
	private long transactions;
	
	@JsonProperty("Availability")
	private int availability;
	
	@JsonProperty("Enabled")
	private boolean enabled;
	
	@JsonProperty("TransactionsPerSec")
	private long transactionsPerSec;
	

	public long getTransactionsPerSec() {
		return transactionsPerSec;
	}

	public boolean getEnabled() {
		return enabled;
	}

	public int getAvailability() {
		return availability;
	}

	public int getId() {
		return id;
	}

	public String getName() {
		return name;
	}

	public long getDatabaseCount() {
		return databaseCount;
	}

	public int getResponseTime() {
		return responseTime;
	}

	public String getSqlVersion() {
		return sqlVersion;
	}

	public long getTotalDataSize() {
		return totalDataSize;
	}

	public long getTotalDatabaseSize() {
		return totalDatabaseSize;
	}

	public long getTotalLogSize() {
		return totalLogSize;
	}

	public long getTransactions() {
		return transactions;
	}
	
	@JsonProperty("InstanceState")
	private int instanceState;
	
	public int getInstanceState()
	{
		return instanceState;
	}
	
}
