package com.idera.sqldm.data.instances;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
//@JsonInclude(JsonInclude.Include.NON_NULL)
public class InstanceActivityOverTimeSWA {

	@JsonProperty("data") private List<List<String>> data;
	@JsonProperty("labels") private List<SWAXaxisLabels> labels;
	@JsonProperty("xAxisTicks") private List<String> xAxisTicks;
	public List<List<String>> getData() {
		return data;
	}
	public void setData(List<List<String>> data) {
		this.data = data;
	}
	public List<SWAXaxisLabels> getLabels() {
		return labels;
	}
	public void setLabels(List<SWAXaxisLabels> labels) {
		this.labels = labels;
	}
	public List<String> getxAxisTicks() {
		return xAxisTicks;
	}
	public void setxAxisTicks(List<String> xAxisTicks) {
		this.xAxisTicks = xAxisTicks;
	}
}
