package com.idera.sqldm.data;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.cwf.model.Product;

@JsonIgnoreProperties(ignoreUnknown = true)
public class DashboardAlertsByDatabaseWidget {
	
	@JsonProperty("DatabaseName")
	private String databaseName;

	@JsonProperty("InstanceId")
	private int instanceId;

	@JsonProperty("InstanceName")
	private String instanceName;
	
	@JsonProperty("AlertCount")
	private Integer numOfAlerts;
	@JsonProperty("MaxSeverity")
	private Integer maxSeverity;

	private Product product;
	
	// @author Saumyadeep 
	// Friendly Begin
	@JsonProperty("FriendlyServerName")
	private String friendlyServerName;

	public void setFriendlyServerName(String friendlyServerName) {
		this.friendlyServerName = friendlyServerName;
	}
	
	public String getFriendlyServerName() {
		return this.friendlyServerName;
	}
	
	private String displayName;
	
	public String getDisplayName() {
		if(this.getFriendlyServerName()!= null)	
			return this.getFriendlyServerName();
		else
			return this.getInstanceName();
	}

	public void setDisplayName(String displayName) {
		this.displayName = displayName;
	}

	// Friendly End
	
	

	public Integer getMaxSeverity() {
		return maxSeverity;
	}

	public void setMaxSeverity(Integer maxSeverity) {
		this.maxSeverity = maxSeverity;
	}

	public String getDatabaseName() {
		return databaseName;
	}

	public void setDatabaseName(String databaseName) {
		this.databaseName = databaseName;
	}

	public Integer getInstanceId() {
		return instanceId;
	}

	public void setInstanceId(int instanceId) {
		this.instanceId = instanceId;
	}

	public String getInstanceName() {
		return instanceName;
	}

	public void setInstanceName(String instanceName) {
		this.instanceName = instanceName;
	}

	public Integer getNumOfAlerts() {
		return numOfAlerts;
	}

	public void setNumOfAlerts(Integer numOfAlerts) {
		this.numOfAlerts = numOfAlerts;
	}

	public Product getProduct() {
		return product;
	}

	public void setProduct(Product product) {
		this.product = product;
	}

	
}
