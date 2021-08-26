package com.idera.sqldm_10_3.data.category.resources;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown=true)
public class DiskIOPerSecondEntry {
	@JsonProperty("Value") private Long Value;
	@JsonProperty("Key") private String Key;
	public Long getValue() {
		return Value;
	}
	public void setValue(Long value) {
		Value = value;
	}
	public String getKey() {
		return Key;
	}
	public void setKey(String key) {
		Key = key;
	}
}
