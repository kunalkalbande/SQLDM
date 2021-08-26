package com.idera.sqldm_10_3.data;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.cwf.model.Product;

@JsonIgnoreProperties(ignoreUnknown = true)
public class DashboardWorstResponseTimeWidget {


	@JsonProperty("InstanceId")
	private int instanceId;

	@JsonProperty("InstanceName")
	private String instanceName;
	
	@JsonProperty("ResponseTimeMillis")
	private Integer responseTimeMillis;

	@JsonProperty("UTCCollectionDateTime")
	private String utcCollectionDateTime;
	
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
	
	
	public int getInstanceId() {
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

	public Integer getResponseTimeMillis() {
		return responseTimeMillis;
	}

	public void setResponseTimeMillis(Integer responseTimeMillis) {
		this.responseTimeMillis = responseTimeMillis;
	}

	public String getUtcCollectionDateTime() {
		return utcCollectionDateTime;
	}

	public void setUtcCollectionDateTime(String utcCollectionDateTime) {
		this.utcCollectionDateTime = utcCollectionDateTime;
	}

	public Product getProduct() {
		return product;
	}

	public void setProduct(Product product) {
		this.product = product;
	}
	

	
}
