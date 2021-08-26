/////-----------Auth:Rishabh Mishra---------------
package com.idera.sqldm.ui.dashboard;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.annotation.JsonPropertyOrder;

/*@JsonPropertyOrder({ "HealthIndexCoefficientForCriticalAlert", "HealthIndexCoefficientForInformationalAlert",
		"HealthIndexCoefficientForWarningAlert" })*/
public class HealthIndexCoefficients {
	@JsonProperty("HealthIndexCoefficientForCriticalAlert")
	private double critical;
	@JsonProperty("HealthIndexCoefficientForInformationalAlert")
	private double informational;
	@JsonProperty("HealthIndexCoefficientForWarningAlert")
	private double warning;
	@JsonProperty("IsHealthIndexCoefficientForCriticalAlertSet")
	private boolean IsHealthIndexCoefficientForCriticalAlertSet;
	@JsonProperty("IsHealthIndexCoefficientForInformationalAlertSet")
	private boolean IsHealthIndexCoefficientForInformationalAlertSet;
	@JsonProperty("IsHealthIndexCoefficientForWarningAlertSet")
	private boolean IsHealthIndexCoefficientForWarningAlertSet;

	public boolean isIsHealthIndexCoefficientForCriticalAlertSet() {
		return IsHealthIndexCoefficientForCriticalAlertSet;
	}

	public void setIsHealthIndexCoefficientForCriticalAlertSet(boolean isHealthIndexCoefficientForCriticalAlertSet) {
		this.IsHealthIndexCoefficientForCriticalAlertSet = isHealthIndexCoefficientForCriticalAlertSet;
	}

	public boolean isIsHealthIndexCoefficientForWarningAlertSet() {
		return IsHealthIndexCoefficientForWarningAlertSet;
	}

	public void setIsHealthIndexCoefficientForWarningAlertSet(boolean isHealthIndexCoefficientForWarningAlertSet) {
		this.IsHealthIndexCoefficientForWarningAlertSet = isHealthIndexCoefficientForWarningAlertSet;
	}

	public boolean isIsHealthIndexCoefficientForInformationalAlertSet() {
		return IsHealthIndexCoefficientForInformationalAlertSet;
	}

	public void setIsHealthIndexCoefficientForInformationalAlertSet(
			boolean isHealthIndexCoefficientForInformationalAlertSet) {
		this.IsHealthIndexCoefficientForInformationalAlertSet = isHealthIndexCoefficientForInformationalAlertSet;
	}

	public double getCritical() {
		return critical;
	}

	public void setCritical(double critical) {
		this.critical = critical;
	}

	public double getWarning() {
		return warning;
	}

	public void setWarning(double warning) {
		this.warning = warning;
	}

	public double getInformational() {
		return informational;
	}

	public void setInformational(double informational) {
		this.informational = informational;
	}
}
