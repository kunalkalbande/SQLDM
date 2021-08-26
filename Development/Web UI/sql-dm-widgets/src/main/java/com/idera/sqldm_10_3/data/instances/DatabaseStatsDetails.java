package com.idera.sqldm_10_3.data.instances;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown=true)
public class DatabaseStatsDetails {

	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	@JsonProperty("UTCDateTime") private Date UTCCollectionDateTime;
	@JsonProperty("DatabaseID") private int DatabaseID;
	@JsonProperty("DatabaseName") private String DatabaseName;
	@JsonProperty("IOStallMSPerSec") private Double IOStallPerSec;
	@JsonProperty("LogflushesPerSec") private Double LogFlushesPerSec;
	@JsonProperty("NumberReadsPerSec") private Double ReadsPerSec;
	@JsonProperty("NumberWritesPerSec") private Double WritesPerSec;
	@JsonProperty("TransactionsPerSec") private Double TransactionsPerSec;
	public Date getUTCCollectionDateTime() {
		return UTCCollectionDateTime;
	}
	public void setUTCCollectionDateTime(Date uTCCollectionDateTime) {
		UTCCollectionDateTime = uTCCollectionDateTime;
	}
	public int getDatabaseID() {
		return DatabaseID;
	}
	public void setDatabaseID(int databaseID) {
		DatabaseID = databaseID;
	}
	public String getDatabaseName() {
		return DatabaseName;
	}
	public void setDatabaseName(String databaseName) {
		DatabaseName = databaseName;
	}
	public Double getIOStallPerSec() {
		return IOStallPerSec;
	}
	public void setIOStallPerSec(Double iOStallPerSec) {
		IOStallPerSec = iOStallPerSec;
	}
	public Double getLogFlushesPerSec() {
		return LogFlushesPerSec;
	}
	public void setLogFlushesPerSec(Double logFlushesPerSec) {
		LogFlushesPerSec = logFlushesPerSec;
	}
	public Double getReadsPerSec() {
		return ReadsPerSec;
	}
	public void setReadsPerSec(Double readsPerSec) {
		ReadsPerSec = readsPerSec;
	}
	public Double getWritesPerSec() {
		return WritesPerSec;
	}
	public void setWritesPerSec(Double writesPerSec) {
		WritesPerSec = writesPerSec;
	}
	public Double getTransactionsPerSec() {
		return TransactionsPerSec;
	}
	public void setTransactionsPerSec(Double transactionsPerSec) {
		TransactionsPerSec = transactionsPerSec;
	}
}
