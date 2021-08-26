package com.idera.sqldm_10_3.data.customdashboard;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

public class MetricValues {
	@JsonProperty("CollectionTime")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date collectionTime;
	@JsonProperty("MetricValue")
	private double metricValue;
	
	public Date getCollectionTime() {
		return this.collectionTime;
	}
	public double getMetricValue() {
		//return this.metricValue;
		return Math.round(metricValue*100.0)/100.0;
	}
}
