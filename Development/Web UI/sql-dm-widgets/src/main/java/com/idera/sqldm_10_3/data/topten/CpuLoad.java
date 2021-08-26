package com.idera.sqldm_10_3.data.topten;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CpuLoad extends IWidgetInstance {
	@JsonProperty("RepositoryName")
	protected String repositoryName;
	
	@JsonProperty("CPUUsageInPercentage") 
	protected double cpuLoad;
	
	@JsonProperty("Severity")
	private int severity;
	
	@JsonProperty("Criticality") 
	protected Severity criticality;

	public int getSeverity() {
		return severity;
	}

	public void setSeverity(int severity) {
		this.severity = severity;
	}

	public String getRepositoryName() {
		return repositoryName;
	}

	public void setRepositoryName(String repositoryName) {
		this.repositoryName = repositoryName;
	}

	public double getCpuLoad() {
		return cpuLoad;
	}

	public void setCpuLoad(double cpuLoad) {
		this.cpuLoad = cpuLoad;
	}

	public Severity getCriticality() {
		return criticality;
	}

	public void setCriticality(Severity criticality) {
		this.criticality = criticality;
	}
}
