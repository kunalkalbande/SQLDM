package com.idera.sqldm.data.queries;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class QueryGroups {
	
	@JsonProperty("GroupName")
	private String groupName;
	
	@JsonProperty("GroupId")
	private int groupId;
	
	public QueryGroups() { }

	public QueryGroups(String groupName, int index) {
		this.groupName = groupName;
		this.groupId = index;
	}

	public String getGroupName() {
		return groupName;
	}

	public void setGroupName(String groupName) {
		this.groupName = groupName;
	}

	public int getGroupId() {
		return groupId;
	}

	public void setGroupId(int index) {
		this.groupId = index;
	}

}
