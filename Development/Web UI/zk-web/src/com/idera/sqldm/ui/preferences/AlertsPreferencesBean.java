package com.idera.sqldm.ui.preferences;

import com.idera.sqldm.ui.alerts.AlertFilter;
import com.idera.sqldm.ui.alerts.AlertsGroupingModel.GroupBy;

public class AlertsPreferencesBean {
	public final static String SESSION_VARIABLE__NAME = "AlertsSessionDataBean";
	
	private AlertFilter customFilter;
	private GroupBy selectedGroupBy;
	private Integer pageCount;
	
	public AlertFilter getCustomFilter() {
		return customFilter;
	}
	public void setCustomFilter(AlertFilter customFilter) {
		this.customFilter = customFilter;
	}
	public GroupBy getSelectedGroupBy() {
		return selectedGroupBy;
	}
	public void setSelectedGroupBy(GroupBy selectedGroupBy) {
		this.selectedGroupBy = selectedGroupBy;
	}
	public Integer getPageCount() {
		return pageCount;
	}
	public void setPageCount(Integer pageCount) {
		this.pageCount = pageCount;
	}

}
