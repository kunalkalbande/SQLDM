package com.idera.sqldm.data;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.cwf.model.Product;

@JsonIgnoreProperties(ignoreUnknown = true)
public class DashboardAlertsByCategoryWidget {
	
	@JsonProperty("Category")
	private String category;
	
	@JsonProperty("NumOfAlerts")
	private Integer numOfAlerts;
	
	@JsonProperty("InstanceID")
	private Integer instanceID;
	
	private Product product ;
	
	
	public DashboardAlertsByCategoryWidget() {}
	public DashboardAlertsByCategoryWidget(String category, Integer numOfAlerts) {
		this.category = category;
		this.numOfAlerts = numOfAlerts;
	}
	
	public String getCategory() {
		return category;
	}

	public Integer getNumOfAlerts() {
		return numOfAlerts;
	}

	public Integer getInstanceID() {
		return instanceID;
	}
	public Product getProduct() {
		return product;
	}
	public void setProduct(Product product) {
		this.product = product;
	}
}
