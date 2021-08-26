package com.idera.sqldm_10_3.data.category.resources;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

/*
 * Author:Accolite
 * Date : 20th Dec, 2016
 * Overview Graph Customization - SQLDM- 10.2 release
 */
public class CustomCounterStats {
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	@JsonProperty("UTCDateTime") private Date UTCCollectionDateTime;
	@JsonProperty("Value") private Integer value;
	@JsonProperty("metricID") private Integer metricID;
	@JsonProperty("name") private String name;
	public Date getUTCCollectionDateTime() {
		return UTCCollectionDateTime;
	}
	public void setUTCCollectionDateTime(Date uTCCollectionDateTime) {
		UTCCollectionDateTime = uTCCollectionDateTime;
	}
	public Integer getValue() {
		return value;
	}
	public void setValue(Integer value) {
		this.value = value;
	}
	public Integer getMetricID() {
		return metricID;
	}
	public void setMetricID(Integer metricID) {
		this.metricID = metricID;
	}
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	
	
}
