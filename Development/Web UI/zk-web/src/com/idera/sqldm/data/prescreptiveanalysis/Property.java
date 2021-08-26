package com.idera.sqldm.data.prescreptiveanalysis;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

public class Property
{
    @JsonProperty("Key")
    private String key;
    
    @JsonProperty("Value")
    private Object value;

	public void setKey(String key) {
		this.key = key;
	}

	public String getKey() {
		return key;
	}

	public void setValue(Object value) {
		this.value = value;
	}

	public Object getValue() {
        if(key == "sourceObjects" || key == "statement"){
            return "";
        }
        return value;
	}

	@Override
	public String toString() {
		return "Property => {key=" + key + ", value=" + value + "}";
	}
}