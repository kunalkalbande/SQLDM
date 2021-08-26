package com.idera.sqldm_10_3.data.category.resources;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown=true)
public class ServerWaitStatistics {
	@JsonProperty("TotalWaitInMils") private ServerWaitType TotalWaitInMils;
	@JsonProperty("SignalWaitInMils") private ServerWaitType SignalWaitInMils;
	@JsonProperty("ResourceWaitInMils") private ServerWaitType ResourceWaitInMils;
	public ServerWaitType getTotalWaitInMils() {
		return TotalWaitInMils;
	}
	public ServerWaitType getSignalWaitInMils() {
		return SignalWaitInMils;
	}
	public ServerWaitType getResourceWaitInMils() {
		return ResourceWaitInMils;
	}
}
