package com.idera.sqldm_10_3.data.topten;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm_10_3.utils.Utility;

@JsonIgnoreProperties(ignoreUnknown=true)
public class DiskSpaceWidgetInstance extends IWidgetInstance {
	@JsonProperty("Severity")	private int severity;
	@JsonProperty("DiskSpaceUtilizationPercentage") private String diskSpaceUtilization;
	
	private double relativeSeverityValue;

	public int getSeverity() {
		return severity;
	}
	
	public String getDiskSpaceUtilization() {
		return Utility.round(diskSpaceUtilization, 2);
	}
	
	public double getRelativeSeverityValue() {
		return relativeSeverityValue;
	}

	public void setRelativeSeverityValue(double relativeSeverityValue) {
		this.relativeSeverityValue = relativeSeverityValue;
	}

}
