package com.idera.sqldm_10_3.rest;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown = true)
public class TimedValue {

	@JsonProperty("UTCCollectionDateTime")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	protected Date utcUpdated;

	@JsonProperty("Value") 
	protected Double value;

	public Date getUtcUpdated() {
		return utcUpdated;
	}

	public Double getValue() {
		return value;
	}
	
	
}
