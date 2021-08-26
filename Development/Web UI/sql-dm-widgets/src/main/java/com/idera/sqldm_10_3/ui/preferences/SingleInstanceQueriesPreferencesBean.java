package com.idera.sqldm_10_3.ui.preferences;

import com.idera.sqldm_10_3.data.queries.*;
import com.idera.sqldm_10_3.ui.dashboard.instances.queries.QueryFilter;
import com.idera.sqldm_10_3.data.queries.*;
import com.idera.sqldm_10_3.ui.dashboard.instances.queries.QueryFilter;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public class SingleInstanceQueriesPreferencesBean {

	public final static String SESSION_VARIABLE_NAME = "QueriesSessionDataBean";

	private final int instanceId;

	QueryFilter queryFilter;

	private List<QueryApplication> checkedApplications;
	private List<QueryDatabases> checkedDatabases;
	private List<QueryClients> checkedClients;
	private List<QueryUsers> checkedUsers;

	List<ViewMetrics> viewMetricOptions;
	List<QueryGroups> queryGroupOptions;

	private int pageCount;

	private SingleInstanceQueriesPreferencesBean defaultSession;
	private SingleInstanceQueriesPreferencesBean querySignatureSession;
	
	private int set;
	private int drill;
	
	private HashMap<String, String> queryIds;

	// It can only be initialized from PreferncesUtil
	SingleInstanceQueriesPreferencesBean(int instanceId) {
		this.instanceId = instanceId;
		init();
	}
	
	public void init() {
		pageCount = -1;
		defaultSession = null;
		querySignatureSession = null;
	}

	public int getInstanceId() {
		return instanceId;
	}

	public QueryFilter getQueryFilter() {
		return queryFilter;
	}

	public void setQueryFilter(QueryFilter queryFilter) {
		this.queryFilter = queryFilter;
	}

	public List<QueryApplication> getCheckedApplications() {
		return checkedApplications;
	}

	public void setCheckedApplications(
			List<QueryApplication> checkedApplications) {
		this.checkedApplications = checkedApplications;
	}

	public List<QueryDatabases> getCheckedDatabases() {
		return checkedDatabases;
	}

	public void setCheckedDatabases(List<QueryDatabases> checkedDatabases) {
		this.checkedDatabases = checkedDatabases;
	}

	public List<QueryClients> getCheckedClients() {
		return checkedClients;
	}

	public void setCheckedClients(List<QueryClients> checkedClients) {
		this.checkedClients = checkedClients;
	}

	public List<QueryUsers> getCheckedUsers() {
		return checkedUsers;
	}

	public void setCheckedUsers(List<QueryUsers> checkedUsers) {
		this.checkedUsers = checkedUsers;
	}

	public int getPageCount() {
		return pageCount;
	}

	public void setPageCount(int pageCount) {
		this.pageCount = pageCount;
	}

	public List<ViewMetrics> getViewMetricOptions() {
		return viewMetricOptions;
	}

	public void setViewMetricOptions(List<ViewMetrics> viewMetricOptions) {
		this.viewMetricOptions = viewMetricOptions;
	}

	public List<QueryGroups> getQueryGroupOptions() {
		return queryGroupOptions;
	}

	public void setQueryGroupOptions(List<QueryGroups> queryGroupOptions) {
		this.queryGroupOptions = queryGroupOptions;
	}

	public SingleInstanceQueriesPreferencesBean getDefaultSession() {
		return defaultSession;
	}

	public SingleInstanceQueriesPreferencesBean getQuerySignatureSession() {
		return querySignatureSession;
	}

	public void setQuerySignatureSession(
			SingleInstanceQueriesPreferencesBean querySignatureSession) {
		if (querySignatureSession != null) {
			try {
				this.querySignatureSession = new SingleInstanceQueriesPreferencesBean(querySignatureSession.getInstanceId());
				this.querySignatureSession.queryFilter = (QueryFilter) querySignatureSession.queryFilter
						.clone();
				this.querySignatureSession.checkedApplications = (List<QueryApplication>) ((ArrayList<QueryApplication>) querySignatureSession.checkedApplications)
						.clone();
				this.querySignatureSession.checkedDatabases = (List<QueryDatabases>) ((ArrayList<QueryDatabases>) querySignatureSession.checkedDatabases)
						.clone();
				this.querySignatureSession.checkedUsers = (List<QueryUsers>) ((ArrayList<QueryUsers>) querySignatureSession.checkedUsers)
						.clone();
				this.querySignatureSession.checkedClients = (List<QueryClients>) ((ArrayList<QueryClients>) querySignatureSession.checkedClients)
						.clone();
				this.querySignatureSession.pageCount = querySignatureSession.pageCount;
				this.querySignatureSession.viewMetricOptions = querySignatureSession.viewMetricOptions;
				this.querySignatureSession.queryGroupOptions = querySignatureSession.queryGroupOptions;
			} catch (CloneNotSupportedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		} else
			this.querySignatureSession = null;

	}

	public void setDefaultSession(
			SingleInstanceQueriesPreferencesBean defaultSession) {
		if (defaultSession != null) {
			try {
				this.defaultSession = new SingleInstanceQueriesPreferencesBean(defaultSession.getInstanceId());
				this.defaultSession.queryFilter = (QueryFilter) defaultSession.queryFilter
						.clone();
				this.defaultSession.checkedApplications = (List<QueryApplication>) ((ArrayList<QueryApplication>) defaultSession.checkedApplications)
						.clone();
				this.defaultSession.checkedDatabases = (List<QueryDatabases>) ((ArrayList<QueryDatabases>) defaultSession.checkedDatabases)
						.clone();
				this.defaultSession.checkedUsers = (List<QueryUsers>) ((ArrayList<QueryUsers>) defaultSession.checkedUsers)
						.clone();
				this.defaultSession.checkedClients = (List<QueryClients>) ((ArrayList<QueryClients>) defaultSession.checkedClients)
						.clone();
				this.defaultSession.pageCount = defaultSession.pageCount;
				this.defaultSession.viewMetricOptions = defaultSession.viewMetricOptions;
				this.defaultSession.queryGroupOptions = defaultSession.queryGroupOptions;
			} catch (CloneNotSupportedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}

		} else
			this.defaultSession = null;
	}

	public int getSet() {
		return set;
	}

	public void setSet(int set) {
		this.set = set;
	}

	public int getDrill() {
		return drill;
	}

	public void setDrill(int drill) {
		this.drill = drill;
	}

	public HashMap<String, String> getQueryIds() {
		return queryIds;
	}

	public void setQueryIds(HashMap<String, String> queryIds) {
		this.queryIds = queryIds;
	}

}
