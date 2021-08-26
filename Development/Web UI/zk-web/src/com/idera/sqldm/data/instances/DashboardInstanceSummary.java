package com.idera.sqldm.data.instances;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonProperty;

public class DashboardInstanceSummary {

  	@JsonProperty("id")
  	private int id;

  	public int getId() {
  		return id;
  	}

  	@JsonProperty("name")
  	private String name;

  	public String getName() {
  		return name;
  	}

    @JsonProperty("state")
  	private int state;

  	public int getState() {
  		return state;
  	}
    @JsonProperty("severity")
 	private String severity;

    public String getSeverity() {
 		return severity;
 	}

    @JsonProperty("status")
 	private String status;

    public String getStatus() {
 		return severity;
 	}

    @JsonProperty("tags")
 	private List<String> tags;

    public List<String> getTags() {
 		return tags;
 	}

	public DashboardInstanceSummary(int id, String name, int state,
			String severity) {
		super();
		this.id = id;
		this.name = name;
		this.state = state;
		this.severity = severity;
	}

}
