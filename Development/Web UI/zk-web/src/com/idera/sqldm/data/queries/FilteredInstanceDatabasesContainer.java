package com.idera.sqldm.data.queries;

import java.util.ArrayList;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class FilteredInstanceDatabasesContainer {

	@JsonProperty("Databases")
	private List<FilteredInstanceDatabasesProperties> filteredInstanceDatabasesProperties = new ArrayList<FilteredInstanceDatabasesProperties>();

	public List<FilteredInstanceDatabasesProperties> getFilteredInstanceDatabasesProperties() {
		return filteredInstanceDatabasesProperties;
	}
	
	@JsonProperty("TotalNumberOfEntries")
	private int totalNumberOfEntries;

	public int getTotalNumberOfEntries() {
		return totalNumberOfEntries;
	}
}
