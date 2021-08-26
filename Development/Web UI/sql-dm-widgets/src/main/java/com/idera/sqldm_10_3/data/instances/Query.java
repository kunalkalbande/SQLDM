package com.idera.sqldm_10_3.data.instances;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;
import com.idera.sqldm_10_3.utils.Utility;

import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown=true)
public class Query {

	@JsonProperty("storedProcedure") private boolean storedProcedure;
	@JsonProperty("sqlElements") private boolean sqlElements;
	@JsonProperty("sqlBatches") private boolean sqlBatches;
	@JsonProperty("StartTime")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date startTime;
	@JsonProperty("SqlText") private String sqlText;
	@JsonProperty("User") private String user;
	@JsonProperty("QueryName") private String queryName;
	@JsonProperty("QueryId") private String queryId;
	@JsonProperty("QueryNum") private String queryNum;
	@JsonProperty("Occurences") private Long occurences;
	@JsonProperty("EventType") private String eventType;
	@JsonProperty("DatabaseName") private String databaseName;
	@JsonProperty("client") private String client;
	@JsonProperty("ApplicationName") private String applicationName;
	@JsonProperty("AverageDuration") private Long averageDuration;
	@JsonProperty("TotalWrites") private Long totalWrites;
	@JsonProperty("AverageWrites") private Long averageWrites;
	@JsonProperty("TotalReads") private Long totalReads;
	@JsonProperty("AverageReads") private Long averageReads;
	@JsonProperty("Cputotal") private Long cpuTotal;
	@JsonProperty("AvgCPUPerSecond") private Double cpuPerSecond;
	@JsonProperty("Cpuaverage") private Long cpuAverage;
	public String getClient() {
		return client;
	}
	public void setClient(String client) {
		this.client = client;
	}
	
	public String getQueryName() {
		return queryName;
	}
	public void setQueryName(String queryName) {
		this.queryName = queryName;
	}
	
	public String getApplicationName() {
		return applicationName;
	}
	public void setApplicationName(String applicationName) {
		this.applicationName = applicationName;
	}
	public String getSqlText() {
		return sqlText;
	}
	public void setSqlText(String sqlText) {
		this.sqlText = sqlText;
	}
	public Long getOccurences() {
		return occurences;
	}
	public void setOccurences(Long occurences) {
		this.occurences = occurences;
	}
	public String getEventType() {
		return eventType;
	}
	public void setEventType(String eventType) {
		this.eventType = eventType;
	}
	
	
	public String getDatabaseName() {
		return databaseName;
	}
	public void setDatabaseName(String databaseName) {
		this.databaseName = databaseName;
	}
	
	public String getQueryId() {
		return queryId;
	}
	public void setQueryId(String queryId) {
		this.queryId = queryId;
	}
	public String getUser() {
		return user;
	}
	public void setUser(String user) {
		this.user = user;
	}
	public Long getAverageDuration() {
		return averageDuration;
	}
	public void setAverageDuration(Long averageDuration) {
		this.averageDuration = averageDuration;
	}
	public boolean isStoredProcedure() {
		return storedProcedure;
	}
	public void setStoredProcedure(boolean storedProcedure) {
		this.storedProcedure = storedProcedure;
	}
	public boolean isSqlElements() {
		return sqlElements;
	}
	public void setSqlElements(boolean sqlElements) {
		this.sqlElements = sqlElements;
	}
	public boolean isSqlBatches() {
		return sqlBatches;
	}
	public void setSqlBatches(boolean sqlBatches) {
		this.sqlBatches = sqlBatches;
	}
	public Long getTotalWrites() {
		return totalWrites;
	}
	public void setTotalWrites(Long totalWrites) {
		this.totalWrites = totalWrites;
	}
	public Long getAverageWrites() {
		return averageWrites;
	}
	public void setAverageWrites(Long averageWrites) {
		this.averageWrites = averageWrites;
	}
	public Long getTotalReads() {
		return totalReads;
	}
	public void setTotalReads(Long totalReads) {
		this.totalReads = totalReads;
	}
	public Long getAverageReads() {
		return averageReads;
	}
	public void setAverageReads(Long averageReads) {
		this.averageReads = averageReads;
	}
	public Long getCpuTotal() {
		return cpuTotal;
	}
	public void setCpuTotal(Long cpuTotal) {
		this.cpuTotal = cpuTotal;
	}
	public Double getCpuPerSecond() {
		if(cpuPerSecond == null){
			return 0d;
		}
		return Utility.round(cpuPerSecond, 2);
	}
	public void setCpuPerSecond(Double cpuPerSecond) {
		this.cpuPerSecond = cpuPerSecond;
	}
	
	public Long getCpuAverage() {
		return cpuAverage;
	}
	public void setCpuAverage(Long cpuAverage) {
		this.cpuAverage = cpuAverage;
	}
	public String getQueryNum() {
		return queryNum;
	}
	public void setQueryNum(String queryNum) {
		this.queryNum = queryNum;
	}
	
}
