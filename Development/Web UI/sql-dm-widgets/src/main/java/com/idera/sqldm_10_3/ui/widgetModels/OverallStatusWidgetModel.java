package com.idera.sqldm_10_3.ui.widgetModels;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown = true)
public class OverallStatusWidgetModel {

	@JsonProperty("AlertStatus")
	private AlertStatusModel alertStatus;

	/*
	 * @JsonProperty("TimeStamp")
	 * 
	 * @JsonDeserialize(using = DataContractDateDeserializer.class)
	 */
	private Date lastSeen;

	public AlertStatusModel getAlertStatus() {
		return alertStatus;
	}

	public void setAlertStatus(AlertStatusModel alertStatus) {
		this.alertStatus = alertStatus;
	}

	public Date getLastSeen() {
		return lastSeen;
	}

	public void setLastSeen(Date lastSeen) {
		this.lastSeen = lastSeen;
	}

}
