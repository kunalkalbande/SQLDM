package com.idera.sqldm_10_3.data.customdashboard;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class Types {
	
	@JsonProperty("Key")
	private int id;
	
	@JsonProperty("Value")
	private String value;

	public int getId() {
		return id;
	}

	public String getValue() {
		return value;
	}
}
