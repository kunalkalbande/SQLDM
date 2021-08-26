package com.idera.sqldm.data.topten;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.cwf.model.Product;

public abstract class IWidgetInstance {
	
	private double relativeSeverityValue;
	
	@JsonProperty("InstanceId") private int instanceId;
	@JsonProperty("InstanceName") private String instanceName;
	
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
	
	//Used in Idera Dashboard widgets
	private Product product;
	
	public double getRelativeSeverityValue() {
		return relativeSeverityValue < 0 ? 0d : relativeSeverityValue;
	}
	
	public void setRelativeSeverityValue(double relativeSeverityValue) {
		this.relativeSeverityValue = relativeSeverityValue;
	}
	
	public String getInstanceName() {
		return instanceName;
	}
	
	public void setInstanceName(String instanceName) {
		this.instanceName = instanceName;
	}
	
	public int getInstanceId() {
		return instanceId;
	}
	
	public void setInstanceId(int instanceId) {
		this.instanceId = instanceId;
	}

	public Product getProduct() {
		return product;
	}

	public void setProduct(Product product) {
		this.product = product;
	}
}
