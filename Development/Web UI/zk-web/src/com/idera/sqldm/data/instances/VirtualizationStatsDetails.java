package com.idera.sqldm.data.instances;

import java.util.ArrayList;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

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
