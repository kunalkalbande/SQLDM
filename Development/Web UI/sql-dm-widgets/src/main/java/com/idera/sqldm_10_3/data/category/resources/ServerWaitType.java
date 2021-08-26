package com.idera.sqldm_10_3.data.category.resources;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown=true)
public class ServerWaitType {
	
	@JsonProperty("TotalWait") private String totalWait;
	@JsonProperty("Wait") private String wait;
	public String getTotalWait() {
		return totalWait;
	}
	public void setTotalWait(String totalWait) {
		this.totalWait = totalWait;
	}
	public String getWait() {
		return wait;
	}
	public void setWait(String wait) {
		this.wait = wait;
	}

}
