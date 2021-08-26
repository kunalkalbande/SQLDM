package com.idera.sqldm_10_3.data;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;


/*
 * Author:Accolite
 * Date : 15th Nov, 2016
 * History Panel - SQLDM- 10.2 release
 * User settings stored from last its last session from DB 
 */
@JsonIgnoreProperties(ignoreUnknown = true)
public class UserSettings {

	@JsonProperty("Key")
	private String key;
	
	@JsonProperty("Value")
	private String value;

	public String getKey() {
		return key;
	}

	public void setKey(String key) {
		this.key = key;
	}

	public String getValue() {
		return value;
	}

	public void setValue(String value) {
		this.value = value;
	}
	
	
}
