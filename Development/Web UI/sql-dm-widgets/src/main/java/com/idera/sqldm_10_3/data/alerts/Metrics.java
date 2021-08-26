package com.idera.sqldm_10_3.data.alerts;

import com.fasterxml.jackson.annotation.JsonProperty;

public class Metrics {
	@JsonProperty("Name") private String Name;
	@JsonProperty("MinValue") private Integer MinValue;
	@JsonProperty("Rank") private Integer Rank;
	@JsonProperty("Description") private Object Description;
	@JsonProperty("MetricId") private Integer MetricId;
	@JsonProperty("DefaultWarningThreshold") private Long DefaultWarningThreshold;
	@JsonProperty("DefaultInfoValue") private String DefaultInfoValue;
	@JsonProperty("DefaultCriticalThreshold") private Long DefaultCriticalThreshold;
	@JsonProperty("MaxValue") private Long MaxValue;
	@JsonProperty("MetricCategory") private String MetricCategory;
	
    @JsonProperty("WarningThreshold") private String WarningThreshold;
    @JsonProperty("CriticalThreshold") private String CriticalThreshold;
    @JsonProperty("IsMetricNumeric") private boolean IsMetricNumeric;
	
	public String getName() {
		return Name;
	}
	public void setName(String name) {
		Name = name;
	}
	public Integer getMinValue() {
		return MinValue;
	}
	public Integer getRank() {
		return Rank;
	}
	public Object getDescription() {
		return Description;
	}
	public Integer getMetricId() {
		return MetricId;
	}
	public Long getDefaultWarningThreshold() {
		return DefaultWarningThreshold;
	}
	public String getDefaultInfoValue() {
		return DefaultInfoValue;
	}
	public Long getDefaultCriticalThreshold() {
		return DefaultCriticalThreshold;
	}
	public Long getMaxValue() {
		return MaxValue;
	}
	public String getMetricCategory() {
		return MetricCategory;
	}
	public String getWarningThreshold() {
		return WarningThreshold;
	}
	public String getCriticalThreshold() {
		return CriticalThreshold;
	}
	
	public boolean getIsMetricNumeric() {
		return this.IsMetricNumeric;
	}
}
