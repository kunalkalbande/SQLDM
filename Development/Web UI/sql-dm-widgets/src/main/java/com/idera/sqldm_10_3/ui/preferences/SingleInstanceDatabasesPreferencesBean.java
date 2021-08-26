package com.idera.sqldm_10_3.ui.preferences;

import com.idera.sqldm_10_3.data.databases.InstanceDetailsDatabase;

import java.util.List;

public class SingleInstanceDatabasesPreferencesBean {

	public final static String SESSION_VARIABLE_NAME = "DatabasesSessionDataBean";
	
	private final int instanceId;
	
	private boolean isFirstLoad = true;
	
	private String filterBy = "Data Megabytes";
	
	private List<InstanceDetailsDatabase> diList;
	// It can only be initialized from PreferncesUtil
	SingleInstanceDatabasesPreferencesBean(int instanceId) {
		this.instanceId = instanceId;
	}

	public boolean isFirstLoad() {
		return isFirstLoad;
	}

	public void setFirstLoad(boolean isFirstLoad) {
		this.isFirstLoad = isFirstLoad;
	}

	public String getFilterBy() {
		return filterBy;
	}

	public void setFilterBy(String filterBy) {
		this.filterBy = filterBy;
	}

	public int getInstanceId() {
		return instanceId;
	}

	public List<InstanceDetailsDatabase> getDiList() {
		return diList;
	}

	public void setDiList(List<InstanceDetailsDatabase> diList) {
		this.diList = diList;
	}
}
