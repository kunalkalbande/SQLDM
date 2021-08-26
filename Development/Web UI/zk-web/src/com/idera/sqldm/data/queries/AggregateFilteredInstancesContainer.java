package com.idera.sqldm.data.queries;

import java.util.ArrayList;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

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