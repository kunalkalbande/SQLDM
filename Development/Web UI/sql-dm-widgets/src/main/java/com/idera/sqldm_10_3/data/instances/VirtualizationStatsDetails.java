package com.idera.sqldm_10_3.data.instances;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.ArrayList;

@JsonIgnoreProperties(ignoreUnknown=true)
public class VirtualizationStatsDetails {
	@JsonProperty("VirtualizationStats") private ArrayList<VirtualizationStats> VirtualizationStats;
	@JsonProperty("type") private String type;
	
	
	public ArrayList<VirtualizationStats> getVirtualizationStats() {
		return VirtualizationStats;
	}
	public void setVirtualizationStats(ArrayList<VirtualizationStats> virtualizationStats) {
		VirtualizationStats = virtualizationStats;
	}
	public String getType() {
		return type;
	}
	public void setType(String type) {
		this.type = type;
	}
	
}
