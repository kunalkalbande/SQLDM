/////-----------Auth:Rishabh Mishra---------------
package com.idera.sqldm_10_3.ui.dashboard;

import com.fasterxml.jackson.annotation.JsonProperty;

public class InstanceScaleFactorList {
	@JsonProperty("InstanceHealthScaleFactor")
	private Double instanceHealthScaleFactor;
	@JsonProperty("InstanceName")
	private String displayName;
	@JsonProperty("IsActive")
	private boolean IsActive;
	@JsonProperty("SQLServerId")
	private int SQLServerId;
	@JsonProperty("IsInstanceHealthScaleFactorSet")
	private boolean IsInstanceHealthScaleFactorSet;
	
	public boolean isIsInstanceHealthScaleFactorSet() {
		return IsInstanceHealthScaleFactorSet;
	}

	public void setIsInstanceHealthScaleFactorSet(boolean isInstanceHealthScaleFactorSet) {
		IsInstanceHealthScaleFactorSet = isInstanceHealthScaleFactorSet;
	}

	public InstanceScaleFactorList() {

	}
	
	public Double getInstanceHealthScaleFactor() {
		return instanceHealthScaleFactor;
	}

	public void setInstanceHealthScaleFactor(Double instanceHealthScaleFactor) {
		this.instanceHealthScaleFactor = instanceHealthScaleFactor;
	}

	public String getDisplayName() {
		return this.displayName;
	}

	public void setDisplayName(String instanceName) {
		this.displayName = instanceName;
	}

	public boolean isIsActive() {
		return IsActive;
	}

	public void setIsActive(boolean isActive) {
		this.IsActive = isActive;
	}

	public int getSQLServerId() {
		return SQLServerId;
	}

	public void setSQLServerId(int sQLServerId) {
		this.SQLServerId = sQLServerId;
	}

	/*@Override
	public boolean equals(Object obj) {
		if(obj instanceof InstanceScaleFactorList)
		if(this.displayName.equals(((InstanceScaleFactorList) obj).getDisplayName()))
				return true;
		return false;
	}*/
	
	@Override
	public boolean equals(Object obj) {
		if(obj instanceof InstanceScaleFactorList)
		if(this.SQLServerId==((InstanceScaleFactorList) obj).getSQLServerId())
				return true;
		return false;
	}
	
	

}
