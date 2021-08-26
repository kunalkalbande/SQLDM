package com.idera.sqldm.data.topten;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown=true)
public class QueryWidgetInstance extends IWidgetInstance {
	@JsonProperty("Severity")private int severity;
	@JsonProperty("NoOfQueries") private Integer numberOfQueries;

	public int getSeverity() {
		return severity;
	}
	public void setSeverity(int severity) {
		this.severity = severity;
	}
	public Integer getNumberOfQueries() {
		return numberOfQueries;
	}
	public void setNumberOfQueries(Integer numberOfQueries) {
		this.numberOfQueries = numberOfQueries;
	}
}
