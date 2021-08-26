package com.idera.sqldm.data.topten;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm.data.alerts.Alert.AlertSeverity;

@JsonIgnoreProperties(ignoreUnknown = true)
public class MostActiveConnection extends IWidgetInstance {
	
	@JsonProperty("DatabaseName") protected String databaseName;
	@JsonProperty("ActiveConnectionCount") protected int connections;
	@JsonProperty("Severity") private int severity;
	
	public int getSeverity() {
		return severity;
	}

	public void setSeverity(int severity) {
		this.severity = severity;
	}

	@JsonProperty("Criticality") 
	protected AlertSeverity criticality;

	public AlertSeverity getCriticality() {
		return criticality;
	}

	public void setCriticality(AlertSeverity criticality) {
		this.criticality = criticality;
	}
	
	public String getDatabaseName() {
		return databaseName;
	}

	public void setDatabaseName(String databaseName) {
		this.databaseName = databaseName;
	}

	public int getConnections() {
		return connections;
	}

	public void setConnections(int connections) {
		this.connections = connections;
	}

}
