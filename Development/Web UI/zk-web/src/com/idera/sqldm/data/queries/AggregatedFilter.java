package com.idera.sqldm.data.queries;

import java.util.ArrayList;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class AggregatedFilter {

	@JsonProperty("FilterInstanceNames")
	private List<String> filterInstanceNames = new ArrayList<String>();
	
	@JsonProperty("FilterLocations")
	private List<String> filterLocations = new ArrayList<String>();
	
	@JsonProperty("FilterOwners")
	private List<String> filterOwners = new ArrayList<String>();
	
	@JsonProperty("FilterTags")
	private List<String> filterTags = new ArrayList<String>();
	
	@JsonProperty("FilterVersions")
	private  List<String> filterVersions = new ArrayList<String>();
	
	@JsonProperty("GroupBy")
	private String groupBy;
	
	@JsonProperty("MaxCount")
	private int maxCount;
	
	@JsonProperty("SortBy")
	private String sortBy;
	
	public List<String> getFilterInstanceNames() {
		return filterInstanceNames;
	}
	
	public void setFilterInstanceNames(List<String> filterInstanceNames) {
		this.filterInstanceNames = filterInstanceNames;
	}

	public List<String> getFilterLocations() {
		return filterLocations;
	}
	
	public void setFilterLocations(List<String> filterLocations) {
		this.filterLocations = filterLocations;
	}

	public List<String> getFilterOwners() {
		return filterOwners;
	}
	
	public void setFilterOwners(List<String> filterOwners) {
		this.filterOwners = filterOwners;
	}

	public List<String> getFilterTags() {
		return filterTags;
	}
	
	public void setFilterTags(List<String> filterTags) {
		this.filterTags = filterTags;
	}

	public List<String> getFilterVersions()	{
		return filterVersions;
	}
	
	public void setFilterVersions(List<String> filterVersions) {
		this.filterVersions = filterVersions;
	}

	public void setGroupBy(String groupBy) {
		this.groupBy = groupBy;
	}

	public void setMaxCount(int maxCount) {
		this.maxCount = maxCount;
	}

	public void setSortBy(String sortBy) {
		this.sortBy = sortBy;
	}
	
}
