package com.idera.sqldm_10_3.data.instances;

import com.fasterxml.jackson.annotation.JsonProperty;


public class CategoriesEntry {
	@JsonProperty("MaxSeverity") private Integer MaxSeverity;
	@JsonProperty("name") private String name;
	
	public Integer getMaxSeverity() {
		return MaxSeverity;
	}
	public String getName() {
		return name;
	}
}
