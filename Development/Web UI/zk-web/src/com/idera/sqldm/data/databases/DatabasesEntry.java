package com.idera.sqldm.data.databases;


import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm.data.QueryStatisticsEntry;

import java.util.*;

@JsonIgnoreProperties(ignoreUnknown=true)
public class DatabasesEntry {
	public void setDatabaseName(String databaseName) {
		this.databaseName = databaseName;
	}
	public void setAlertCount(Long alertCount) {
		this.alertCount = alertCount;
	}
	public void setQueryStatistics(List<QueryStatisticsEntry> queryStatistics) {
		this.queryStatistics = queryStatistics;
	}
	@JsonProperty("DatabaseName") private String databaseName;
	public String getDatabaseName() {
		return databaseName;
	}
	public Long getAlertCount() {
		return alertCount;
	}
	public List<QueryStatisticsEntry> getQueryStatistics() {
		return queryStatistics;
	}
	@JsonProperty("AlertCount") private Long alertCount;
	@JsonProperty("QueryStatistics") private List<QueryStatisticsEntry> queryStatistics;
}
