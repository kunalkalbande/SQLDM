package com.idera.sqldm_10_3.data.queries;

import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.ArrayList;
import java.util.List;

public class FilteredInstancesContainer {
	
	@JsonProperty("Instances")
	private List<FilteredInstancesProperties> filteredInstancesProperties = new ArrayList<FilteredInstancesProperties>();

	public List<FilteredInstancesProperties> getFilteredInstancesProperties() {
		return filteredInstancesProperties;
	}
	
	@JsonProperty("TotalNumberOfEntries")
	private int totalNumberOfEntries;

	public int getTotalNumberOfEntries() {
		return totalNumberOfEntries;
	}

}
