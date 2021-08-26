package com.idera.sqldm.data;


import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown=true)
public class QueryStatisticsEntry {
	@JsonProperty("QueryName") private String queryName;
	public String getQueryName() {
		return queryName;
	}
	public void setQueryName(String queryName) {
		this.queryName = queryName;
	}
	@JsonProperty("ApplicationName") private String applicationName;
	public void setApplicationName(String applicationName) {
		this.applicationName = applicationName;
	}
	public void setAverageCPU(String averageCPU) {
		this.averageCPU = averageCPU;
	}
	public void setAverageDuration(String averageDuration) {
		this.averageDuration = averageDuration;
	}
	public void setAverageReads(String averageReads) {
		this.averageReads = averageReads;
	}
	public void setCount(Long count) {
		this.count = count;
	}
	public void setStatementText(String statementText) {
		this.statementText = statementText;
	}
	public void setStatementType(Integer statementType) {
		this.statementType = statementType;
	}
	public void setAverageWrites(String averageWrites) {
		this.averageWrites = averageWrites;
	}
	@JsonProperty("AverageCPU") private String averageCPU;
	@JsonProperty("AverageDuration") private String averageDuration;
	@JsonProperty("AverageReads") private String averageReads;
	@JsonProperty("Count") private Long count;
	@JsonProperty("StatementText") private String statementText;
	@JsonProperty("StatementType") private Integer statementType;
	@JsonProperty("AverageWrites") private String averageWrites;
	public String getApplicationName() {
		return applicationName;
	}
	public String getAverageCPU() {
		return averageCPU;
	}
	public String getAverageDuration() {
		return averageDuration;
	}
	public String getAverageReads() {
		return averageReads;
	}
	public Long getCount() {
		return count;
	}
	public String getStatementText() {
		return statementText;
	}
	public Integer getStatementType() {
		return statementType;
	}
	public String getAverageWrites() {
		return averageWrites;
	}
}
