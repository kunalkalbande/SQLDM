package com.idera.sqldm.data;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class SQLInstanceCounts {

	@JsonProperty("Disabled")
	private int disabled;

	public int getDisabled() {
		return disabled;
	}

	@JsonProperty("Discovered")
	private int discovered;

	public int getDiscovered() {
		return discovered;
	}

	@JsonProperty("Down")
	private int down;

	public int getDown() {
		return down;
	}

	@JsonProperty("Registered")
	private int registered;

	public int getRegistered() {
		return registered;
	}

	@JsonProperty("Removed")
	private int removed;

	public int getRemoved() {
		return removed;
	}

	@JsonProperty("Up")
	private int up;

	public int getUp() {
		return up;
	}

}
