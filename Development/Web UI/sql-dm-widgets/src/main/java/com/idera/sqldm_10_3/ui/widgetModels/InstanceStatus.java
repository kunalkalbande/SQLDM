package com.idera.sqldm_10_3.ui.widgetModels;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class InstanceStatus {

	@JsonIgnoreProperties(ignoreUnknown = true)
	public class Overview {

		@JsonProperty("MonitoredServersCount")
		private int totalMonitoredInstances;

		@JsonProperty("DisabledServersCount")
		private int totalDisabledInstances;
		
		public Overview() {
			
		}

		public int getTotalMonitoredInstances() {
			return totalMonitoredInstances;
		}

		public void setTotalMonitoredInstances(int totalmonitoredInstances) {
			this.totalMonitoredInstances = totalmonitoredInstances;
		}

		public int getTotalDisabledInstances() {
			return totalDisabledInstances;
		}

		public void setTotalDisabledInstances(int totalDisabledInstances) {
			this.totalDisabledInstances = totalDisabledInstances;
		}

	}

	@JsonIgnoreProperties(ignoreUnknown = true)
	public class AlertStatus {

		@JsonProperty("CriticalInstancesCount")
		private int totalCriticalInstances;

		@JsonProperty("WarningInstancesCount")
		private int totalWarningInstances;

		@JsonProperty("InformationalInstancesCount")
		private int totalInformationalInstances;

		@JsonProperty("OkInstancesCount")
		private int totalOkInstances;
		
		public AlertStatus() {
			
		}

		public int getTotalCriticalInstances() {
			return totalCriticalInstances;
		}

		public void setTotalCriticalInstances(int totalCriticalInstances) {
			this.totalCriticalInstances = totalCriticalInstances;
		}

		public int getTotalWarningInstances() {
			return totalWarningInstances;
		}

		public void setTotalWarningInstances(int totalWarningInstances) {
			this.totalWarningInstances = totalWarningInstances;
		}

		public int getTotalInformationalInstances() {
			return totalInformationalInstances;
		}

		public void setTotalInformationalInstances(
				int totalInformationalInstances) {
			this.totalInformationalInstances = totalInformationalInstances;
		}

		public int getTotalOkInstances() {
			return totalOkInstances;
		}

		public void setTotalOkInstances(int totalOkInstances) {
			this.totalOkInstances = totalOkInstances;
		}

	}

	@JsonProperty("Overview")
	private Overview overview;

	@JsonProperty("AlertStatus")
	private AlertStatus alertStatus;

	public Overview getOverview() {
		return overview;
	}

	public void setOverview(Overview overview) {
		this.overview = overview;
	}

	public AlertStatus getAlertStatus() {
		return alertStatus;
	}

	public void setAlertStatus(AlertStatus status) {
		this.alertStatus = status;
	}

	public int getTotalMonitoredInstances() {
		return overview.totalMonitoredInstances;
	}

	public int getTotalDisabledInstances() {
		return overview.totalDisabledInstances;
	}

	public int getTotalCriticalInstances() {
		return alertStatus.totalCriticalInstances;
	}

	public int getTotalWarningInstances() {
		return alertStatus.totalWarningInstances;
	}


	public int getTotalInformationalInstances() {
		return alertStatus.totalInformationalInstances;
	}

	public int getTotalOkInstances() {
		return alertStatus.totalOkInstances;
	}

}
