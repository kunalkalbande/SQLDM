package com.idera.sqldm.ui.preferences;

import java.util.Date;
import java.util.List;

import org.zkoss.zk.ui.Sessions;
import org.zkoss.zul.ListModelList;

import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.alerts.Alert;
import com.idera.sqldm.data.alerts.AlertCategoriesDetails;

public class SingleInstanceAlertsPreferencesBean {

	public final static String SESSION_VARIABLE_NAME = "SingleInstanceAlertsSessionDataBean";

	private final int instanceId;
	
	private Date fromDate = null;
	private Date endDate = null;
	private Date fromTime = null;
	private Date endTime = null;
	
	private Double offsetHours;	
	private List<Alert> modelData;
	private ListModelList<AlertCategoriesDetails> categoryOptions;

	// It can only be initialized from PreferencesUtil
	SingleInstanceAlertsPreferencesBean(int instanceId) {
		this.instanceId = instanceId;
		this.offsetHours = ((int)Sessions.getCurrent().getAttribute(WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET))/(3600*1000.0);
	}

	public int getInstanceId() {
		return instanceId;
	}

	public void setFromDate(Date fromDate) {
		this.fromDate = fromDate;
	}

	public void setEndDate(Date toDate) {
		this.endDate = toDate;
	}

	public void setFromTime(Date fromTime) {
		this.fromTime = fromTime;
	}

	public void setEndTime(Date toTime) {
		this.endTime = toTime;
	}
	
	public Date getFromDate() {
		return fromDate;
	}

	public Date getEndDate() {
		return endDate;
	}

	public Date getFromTime() {
		return fromTime;
	}

	public Date getEndTime() {
		return endTime;
	}

	public Double getOffsetHours() {
		return offsetHours;
	}

	public void setOffsetHours(Double offsetHours) {
		this.offsetHours = offsetHours;
	}

	public List<Alert> getModelData() {
		return modelData;
	}

	public void setModelData(List<Alert> modelData) {
		this.modelData = modelData;
	}

	public ListModelList<AlertCategoriesDetails> getCategoryOptions() {
		return categoryOptions;
	}

	public void setCategoryOptions(
			ListModelList<AlertCategoriesDetails> categoryOptions) {
		this.categoryOptions = categoryOptions;
	}


}
