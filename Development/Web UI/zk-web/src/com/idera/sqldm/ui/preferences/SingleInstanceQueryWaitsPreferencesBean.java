package com.idera.sqldm.ui.preferences;

import java.util.Date;
import java.util.List;
import java.util.Map;

import org.zkoss.zk.ui.Sessions;

import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.instances.QueryWaits;
import com.idera.sqldm.ui.dashboard.instances.queryWaits.QueryChartOptionsEnum;
import com.idera.sqldm.ui.dashboard.instances.queryWaits.QueryWaitsFilter;

public class SingleInstanceQueryWaitsPreferencesBean {

	public final static String SESSION_VARIABLE_NAME = "QueryWaitsSessionDataBean";

	private final int instanceId;
	
	private Date fromDate;
	private Date endDate;
	private Date fromTime;
	private Date endTime;
	
	private int limit;
	private String excludeSql;

	private Double offsetHours;
	private int selectedOptionForCharting;	
	private int selectedWaitOption;
	
	private List<QueryWaits> modelData;
	private Map<QueryChartOptionsEnum, QueryWaitsFilter> filters;
	List<Map<String, Map<String, Number>>> durationStackedChartData;
	List<Map<String, Map<String, Number>>> timeStackedChartData;
	List<Map<String, Integer>> idMaps;

	// It can only be initialized from PreferencesUtil
	SingleInstanceQueryWaitsPreferencesBean(int instanceId) {
		this.instanceId = instanceId;
		this.offsetHours = ((int)Sessions.getCurrent().getAttribute(WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET))/(3600*1000.0);
		this.selectedOptionForCharting = -1;
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

	public int getLimit() {
		return limit;
	}

	public void setLimit(int limit) {
		this.limit = limit;
	}

	public String getExcludeSql() {
		return excludeSql;
	}

	public void setExcludeSql(String excludeSql) {
		this.excludeSql = excludeSql;
	}

	public Double getOffsetHours() {
		return offsetHours;
	}

	public void setOffsetHours(Double offsetHours) {
		this.offsetHours = offsetHours;
	}

	public Map<QueryChartOptionsEnum, QueryWaitsFilter> getFilters() {
		return filters;
	}

	public void setFilters(Map<QueryChartOptionsEnum, QueryWaitsFilter> filters) {
		this.filters = filters;
	}

	public int getSelectedOptionForCharting() {
		return selectedOptionForCharting;
	}

	public void setSelectedOptionForCharting(int selectedOptionForCharting) {
		this.selectedOptionForCharting = selectedOptionForCharting;
	}

	public int getSelectedWaitOption() {
		return selectedWaitOption;
	}

	public void setSelectedWaitOption(int selectedWaitOption) {
		this.selectedWaitOption = selectedWaitOption;
	}

	public List<QueryWaits> getModelData() {
		return modelData;
	}

	public void setModelData(List<QueryWaits> modelData) {
		this.modelData = modelData;
	}

	public List<Map<String, Map<String, Number>>> getDurationStackedChartData() {
		return durationStackedChartData;
	}

	public void setDurationStackedChartData(
			List<Map<String, Map<String, Number>>> durationStackedChartData) {
		this.durationStackedChartData = durationStackedChartData;
	}

	public List<Map<String, Map<String, Number>>> getTimeStackedChartData() {
		return timeStackedChartData;
	}

	public void setTimeStackedChartData(
			List<Map<String, Map<String, Number>>> timeStackedChartData) {
		this.timeStackedChartData = timeStackedChartData;
	}

	public List<Map<String, Integer>> getIdMaps() {
		return idMaps;
	}

	public void setIdMaps(List<Map<String, Integer>> idMaps) {
		this.idMaps = idMaps;
	}


}
