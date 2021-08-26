package com.idera.sqldm.data.queries;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class AggregateFilteredInstancesProperties {

	@JsonProperty("DatabaseCount")
	private long databaseCount;
	
	@JsonProperty("InstanceCount")
	private long instanceCount;
	
	@JsonProperty("TotalDataSizeMB")
	private long totalDataSize;
	
	@JsonProperty("TotalDatabaseSizeMB")
	private long totalDatabaseSize;
    
	@JsonProperty("TotalLogSizeMB")
	private long totalLogSize;
    
	@JsonProperty("TotalTransactions")
	private long totalTransactions;

	@JsonProperty("Name")
	private String name;
	
	@JsonProperty("TotalTransactionsPerSec")
	private long transactionsPerSec;
	

	public long getTransactionsPerSec() {
		return transactionsPerSec;
	}

	public long getDatabaseCount() {
		return databaseCount;
	}

	public long getInstanceCount() {
		return instanceCount;
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

	public long getTotalTransactions() {
		return totalTransactions;
	}

	public String getName() {
		return name;
	}

}
