package com.idera.sqldm.data.topten;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm.utils.Utility;

@JsonIgnoreProperties(ignoreUnknown = true)
public class TempDBUtilizationInstance extends IWidgetInstance {
	private static int MB_VALUE = 1024;
	@JsonProperty("Severity")private int severity;
	@JsonProperty("TempDBUsageInKB") private double spaceUtilization ;
	
	public void setSpaceUtilization(float spaceUtilization) {
		this.spaceUtilization = spaceUtilization;
	}
	
	public double getSpaceUtilization() {
		return ((double) Utility.round((double)(spaceUtilization/MB_VALUE), 2));
	}

	public int getSeverity() {
		return severity;
	}
}
