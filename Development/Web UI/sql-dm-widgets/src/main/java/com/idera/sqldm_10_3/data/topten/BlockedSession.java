package com.idera.sqldm_10_3.data.topten;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class BlockedSession extends IWidgetInstance {
	@JsonProperty("RepositoryName")
	protected String repositoryName;
	
	@JsonProperty("BlockedSessionCount") 
	protected int numberOfBlockedProcess;
	
	@JsonProperty("Severity")
	private int severity;
	
	@JsonProperty("Criticality") 
	private Severity criticality;
	
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

	public int getNumberOfBlockedProcess() {
		return numberOfBlockedProcess;
	}

	public void setNumberOfBlockedProcess(int numberOfBlockedProcess) {
		this.numberOfBlockedProcess = numberOfBlockedProcess;
	}

	public Severity getCriticality() {
		return criticality;
	}

	public void setCriticality(Severity criticality) {
		this.criticality = criticality;
	}
}
