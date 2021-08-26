package com.idera.sqldm_10_3.data.queries;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm_10_3.data.instances.ServerStatus;

@JsonIgnoreProperties(ignoreUnknown = true)
public class ViewMetrics {

	private static final ServerStatus DEFAULT_SERVER_STATUS = new ServerStatus();
	
	@JsonProperty("MetricName")
	private String metricName;

	@JsonProperty("MetricId")
	private int metricId;

	private ViewMetrics() { }
	
	private ViewMetrics(String metricName, int index) {
		this.metricName = metricName;
		this.metricId = index;
	}

	public String getMetricName() {
		return metricName;
	}

	public void setMetricName(String metricName) {
		this.metricName = metricName;
	}

	public int getMetricId() {
		return metricId;
	}

	public void setMetricId(int index) {
		this.metricId = index;
	}

}
