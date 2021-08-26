package com.idera.sqldm_10_3.data.queries;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.ArrayList;
import java.util.List;

@JsonIgnoreProperties(ignoreUnknown = true)
public class AggregateFilteredInstancesContainer {

	@JsonProperty("Aggregates")
	private List<AggregateFilteredInstancesProperties> aggregateFilteredInstancesProperties = new ArrayList<AggregateFilteredInstancesProperties>();

	public List<AggregateFilteredInstancesProperties> getAggregateFilteredInstancesProperties() {
		return aggregateFilteredInstancesProperties;
	}
	
	@JsonProperty("TotalNumberOfEntries")
	private int totalNumberOfEntries;

	public int getTotalNumberOfEntries() {
		return totalNumberOfEntries;
	}
}