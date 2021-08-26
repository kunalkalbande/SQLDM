package com.idera.sqldm.data.queries;

import java.util.ArrayList;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonProperty;

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
