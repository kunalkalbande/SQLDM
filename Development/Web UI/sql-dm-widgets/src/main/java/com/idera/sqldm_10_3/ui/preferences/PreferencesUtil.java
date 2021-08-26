package com.idera.sqldm_10_3.ui.preferences;

import com.idera.sqldm_10_3.ui.alerts.AlertFilter;
import com.idera.sqldm_10_3.ui.alerts.AlertsGroupingModel.GroupBy;
import com.idera.sqldm_10_3.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm_10_3.ui.dashboard.instances.resources.FileActivityFilters;
import com.idera.sqldm_10_3.ui.preferences.DashboardPreferencesBean.DashboardCategoryGroup;
import com.idera.sqldm_10_3.ui.preferences.DashboardPreferencesBean.DashboardGroupByInstances;
import com.idera.sqldm_10_3.ui.preferences.DashboardPreferencesBean.DashboardInstanceView;
import org.zkoss.zk.ui.Executions;

import java.util.List;
import java.util.Map;

public class PreferencesUtil {
	
	private static final PreferencesUtil pu = new PreferencesUtil();
	
	private PreferencesUtil() {
	}
	
	public static final PreferencesUtil getInstance() {
		return pu;
	}
	
	public void setDashboardPreferencesInSession(String productInstanceName){
		DashboardPreferencesBean dsdb = getDashboardPreferencesInSession();
		
		if(productInstanceName != null && !productInstanceName.equals("")){
			dsdb.setProductInstanceName(productInstanceName);
			Executions.getCurrent().getSession().setAttribute(DashboardPreferencesBean.SESSION_VARIABLE__NAME, dsdb);
		}
	}
	
	public void setDashboardPreferencesInSession(String productInstanceName, String selectedSqldmTab){
		DashboardPreferencesBean dsdb = getDashboardPreferencesInSession();
		
		if(productInstanceName != null && !productInstanceName.equals("")){
			dsdb.setProductInstanceName(productInstanceName);
			dsdb.setSelectedSqldmTab(selectedSqldmTab);
			Executions.getCurrent().getSession().setAttribute(DashboardPreferencesBean.SESSION_VARIABLE__NAME, dsdb);
		}
	}
	
	public void setDashboardPreferencesInSession(DashboardCategoryGroup dcg, String tagValue, DashboardInstanceView div,
                                                 DashboardGroupByInstances gii, int instanceListRowsCount, int alertsListRowsCount, String searchText){
		this.setDashboardPreferencesInSession(dcg, tagValue, div, gii,instanceListRowsCount ,alertsListRowsCount, searchText, -1,-1,-1,-1,-1,-1);
	}
	public void setDashboardPreferencesInSession(DashboardCategoryGroup dcg, String tagValue, DashboardInstanceView div,
                                                 DashboardGroupByInstances gii, int instanceListRowsCount, int alertsListRowsCount, String searchText, List<String> defaultColumnList,
                                                 List<String> columnList) {
		this.setDashboardPreferencesInSession(dcg, tagValue, div, gii,instanceListRowsCount ,alertsListRowsCount, searchText, -1,-1,-1,-1,-1,-1);
		DashboardPreferencesBean dsdb = (DashboardPreferencesBean) Executions.getCurrent().getSession().getAttribute(DashboardPreferencesBean.SESSION_VARIABLE__NAME);
		if (dsdb == null) {
			dsdb = new DashboardPreferencesBean();
		}
		dsdb.setInstanceListViewVisibleColumns(defaultColumnList);
		dsdb.setInstanceListViewHiddenColumns(columnList);
		Executions.getCurrent().getSession().setAttribute(DashboardPreferencesBean.SESSION_VARIABLE__NAME, dsdb);
	}
	
	public void setDashboardPreferencesInSession(DashboardCategoryGroup dcg, String tagValue, DashboardInstanceView div,
                                                 DashboardGroupByInstances gii, int instanceListRowsCount, int alertsListRowsCount, String searchText,
                                                 int overviewDBGridPageCount, int sessionsGridPageCount, int queriesGridPageCount, int resoucesSWGridPageCount,
                                                 int databaseSummaryGridpageCount, int avaGroupGridpageCount) {
    	DashboardPreferencesBean dsdb = getDashboardPreferencesInSession();
		if (dcg != null && tagValue != null) {
			dcg.setName(tagValue);
			dsdb.setLeftCategoryGroup(dcg);
		}
		if (div != null) {
			dsdb.setInstancesView(div);
		}
		if (gii != null) {
			dsdb.setGroupByInstances(gii);
		}
		if (instanceListRowsCount != -1) {
			dsdb.setInstanceRowCount(instanceListRowsCount);
		}
		if (alertsListRowsCount != -1) {
			dsdb.setAlertsRowCount(alertsListRowsCount);
		}
		if (overviewDBGridPageCount != -1) {
			dsdb.setOverviewDBGridPageCount(overviewDBGridPageCount);
		}
		if (sessionsGridPageCount != -1) {
			dsdb.setSessionsGridPageCount(sessionsGridPageCount);
		}
		if (queriesGridPageCount != -1) {
			dsdb.setQueriesGridPageCount(queriesGridPageCount);
		}
		if (resoucesSWGridPageCount != -1) {
			dsdb.setResoucesSWGridPageCount(resoucesSWGridPageCount);
		}
		if (databaseSummaryGridpageCount != -1) {
			dsdb.setDatabaseSummaryGridpageCount(databaseSummaryGridpageCount);
		}
		if (avaGroupGridpageCount != -1) {
			dsdb.setAvaGroupGridpageCount(avaGroupGridpageCount); 
		}
		if(searchText != null) {
			dsdb.setSearchText(searchText);
		}
		
		Executions.getCurrent().getSession().setAttribute(DashboardPreferencesBean.SESSION_VARIABLE__NAME, dsdb);
    }
	
	public void setFilteredInstancesMap(Map<Integer, Object> filteredInstancesMap) {
		DashboardPreferencesBean dsdb = (DashboardPreferencesBean) Executions.getCurrent().getSession().getAttribute(DashboardPreferencesBean.SESSION_VARIABLE__NAME);
		if (dsdb == null) {
			dsdb = new DashboardPreferencesBean();
		}
		dsdb.setFilteredInstancesMap(filteredInstancesMap);
    }
	
	public Map<Integer, Object> getFilteredInstancesMap() {
		DashboardPreferencesBean dsdb = (DashboardPreferencesBean) Executions.getCurrent().getSession().getAttribute(DashboardPreferencesBean.SESSION_VARIABLE__NAME);
		if (dsdb != null) {
			return dsdb.getFilteredInstancesMap();
		}
		return null;
    }
    

	public void addOpenedInstanceDashboardHeatMapViewInSession(Integer instanceId) {
    	DashboardPreferencesBean dsdb = getDashboardPreferencesInSession();
    	dsdb.addHeatMapOpenInstance(instanceId);
		Executions.getCurrent().getSession().setAttribute(DashboardPreferencesBean.SESSION_VARIABLE__NAME, dsdb);
    }
	public void removeClosedInstanceDashboardHeatMapViewInSession(Integer instanceId) {
    	DashboardPreferencesBean dsdb = getDashboardPreferencesInSession();
    	dsdb.removeHeatMapOpenInstance(instanceId);
		Executions.getCurrent().getSession().setAttribute(DashboardPreferencesBean.SESSION_VARIABLE__NAME, dsdb);
    }

	public DashboardPreferencesBean getDashboardPreferencesInSession() {
    	DashboardPreferencesBean dsdb = (DashboardPreferencesBean) Executions.getCurrent().getSession().getAttribute(DashboardPreferencesBean.SESSION_VARIABLE__NAME);
		if (dsdb == null) {
			dsdb = new DashboardPreferencesBean();
		}
		return dsdb;
    }

	
	public SingleInstancePreferencesBean getSingleInstancePreferencesInSession(int instanceId) {
		SingleInstancePreferencesBean dsdb = (SingleInstancePreferencesBean) Executions.getCurrent().getSession().getAttribute(SingleInstancePreferencesBean.SESSION_VARIABLE__NAME);
		if (dsdb == null || dsdb.getInstanceId() != instanceId) {
			dsdb = new SingleInstancePreferencesBean(instanceId);
		}
		return dsdb;
    }
	
	public void setSingleInstancePreferencesInSession(int instanceId, FileActivityFilters fileActivityFilters) {
		SingleInstancePreferencesBean dsdb = getSingleInstancePreferencesInSession(instanceId);
		if (fileActivityFilters != null) {
			dsdb.setFileActivityFilters(fileActivityFilters);
		}
		Executions.getCurrent().getSession().setAttribute(SingleInstancePreferencesBean.SESSION_VARIABLE__NAME, dsdb);
    }
	
	public void setSingleInstancePreferencesInSession(SingleInstancePreferencesBean bean) {
		Executions.getCurrent().getSession().setAttribute(SingleInstancePreferencesBean.SESSION_VARIABLE__NAME, bean);
    }
	
	/*
	 * Chaitanya Tanwar DM 10.2
	 * */
	public HistoryPanelPreferencesBean getHistoryPanelPreferencesInSession() {
		HistoryPanelPreferencesBean dsdb = (HistoryPanelPreferencesBean) Executions.getCurrent().getSession().getAttribute(HistoryPanelPreferencesBean.SESSION_VARIABLE_NAME);
		if (dsdb == null) {
			dsdb = new HistoryPanelPreferencesBean();
		}
		return dsdb;
    }
	
	/*
	 * Chaitanya Tanwar DM 10.2
	 * */
	public void setHistoryPanelPreferencesInSession(HistoryPanelPreferencesBean bean) {
		Executions.getCurrent().getSession().setAttribute(HistoryPanelPreferencesBean.SESSION_VARIABLE_NAME, bean);
    }
	
	public AlertsPreferencesBean getAlertsPreferencesInSession(){
		AlertsPreferencesBean dsdb = (AlertsPreferencesBean) Executions.getCurrent().getSession().getAttribute(AlertsPreferencesBean.SESSION_VARIABLE__NAME);
		if (dsdb == null) {
			dsdb = new AlertsPreferencesBean();
		}
		return dsdb;
	}
	
	public void setAlertsPreferencesInSession(AlertFilter filters, GroupBy grpBy, Integer pageCount) {
		AlertsPreferencesBean dsdb = getAlertsPreferencesInSession();
		if(filters != null) {
			dsdb.setCustomFilter(filters);
		}
		if(grpBy != null) {
			dsdb.setSelectedGroupBy(grpBy);
		}
		if(pageCount != -1) {
			dsdb.setPageCount(pageCount);
		}
		Executions.getCurrent().getSession().setAttribute(AlertsPreferencesBean.SESSION_VARIABLE__NAME, dsdb);
    }
	
	public void setAlertsPreferencesInSession(AlertsPreferencesBean bean) {
		Executions.getCurrent().getSession().setAttribute(AlertsPreferencesBean.SESSION_VARIABLE__NAME, bean);
    }
	
	public void setInstanceTabSubTabs(Integer instanceId, InstanceSubCategoriesTab isct) {
		SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
		pref.setSelectedCategory(0);
        pref.setSelectedSubCategory(1);
		if (isct != null) {
            pref.setSelectedCategory(isct.getInstanceCategory().getTabId());
            pref.setSelectedSubCategory(isct.getSubTabId());
		}
        PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);
	}
	
	public SingleInstanceQueriesPreferencesBean getQueriesPreferenceInSession(int instanceId) {

		SingleInstanceQueriesPreferencesBean dsdb = (SingleInstanceQueriesPreferencesBean) Executions.getCurrent().getSession().getAttribute(SingleInstanceQueriesPreferencesBean.SESSION_VARIABLE_NAME);
		if (dsdb == null || dsdb.getInstanceId() != instanceId) {
			dsdb = new SingleInstanceQueriesPreferencesBean(instanceId);
		}
		return dsdb;
		
	}
	
	public void setQueriesPreferenceInSession(SingleInstanceQueriesPreferencesBean bean) {
		Executions.getCurrent().getSession().setAttribute(SingleInstanceQueriesPreferencesBean.SESSION_VARIABLE_NAME, bean);
    }
	
	public SingleInstanceDatabasesPreferencesBean getDatabasesPreferenceInSession(int instanceId) {

		SingleInstanceDatabasesPreferencesBean dsdb = (SingleInstanceDatabasesPreferencesBean) Executions.getCurrent().getSession().getAttribute(SingleInstanceDatabasesPreferencesBean.SESSION_VARIABLE_NAME);
		if (dsdb == null || dsdb.getInstanceId() != instanceId) {
			dsdb = new SingleInstanceDatabasesPreferencesBean(instanceId);
		}
		return dsdb;
		
	}
	
	public void setDatabasesPreferenceInSession(SingleInstanceDatabasesPreferencesBean bean) {
		Executions.getCurrent().getSession().setAttribute(SingleInstanceDatabasesPreferencesBean.SESSION_VARIABLE_NAME, bean);
    }
	
	
	public SingleInstanceQueryWaitsPreferencesBean getQueryWaitsPreferenceInSession(int instanceId) {

		SingleInstanceQueryWaitsPreferencesBean dsdb = (SingleInstanceQueryWaitsPreferencesBean) Executions.getCurrent().getSession().getAttribute(SingleInstanceQueryWaitsPreferencesBean.SESSION_VARIABLE_NAME + "_" + instanceId);
		if (dsdb == null || dsdb.getInstanceId() != instanceId) {
			dsdb = new SingleInstanceQueryWaitsPreferencesBean(instanceId);
		}
		return dsdb;
		
	}
	
	public void setQueryWaitsPreferenceInSession(SingleInstanceQueryWaitsPreferencesBean bean) {
		Executions.getCurrent().getSession().setAttribute(SingleInstanceQueryWaitsPreferencesBean.SESSION_VARIABLE_NAME + "_" + bean.getInstanceId(), bean);
    }
	
	public SingleInstanceAlertsPreferencesBean getSingleInstanceAlertsPreferenceInSession(int instanceId) {

		SingleInstanceAlertsPreferencesBean dsdb = (SingleInstanceAlertsPreferencesBean) Executions.getCurrent().getSession().getAttribute(SingleInstanceAlertsPreferencesBean.SESSION_VARIABLE_NAME + "_" + instanceId);
		if (dsdb == null || dsdb.getInstanceId() != instanceId) {
			dsdb = new SingleInstanceAlertsPreferencesBean(instanceId);
		}
		return dsdb;
		
	}
	
	public void setSingleInstanceAlertsPreferenceInSession(SingleInstanceAlertsPreferencesBean bean) {
		Executions.getCurrent().getSession().setAttribute(SingleInstanceAlertsPreferencesBean.SESSION_VARIABLE_NAME + "_" + bean.getInstanceId(), bean);
    }
	
	public SingleInstanceOverviewPreferenceBean getOverviewPreferenceInSession(int instanceId) {

		SingleInstanceOverviewPreferenceBean overBean = (SingleInstanceOverviewPreferenceBean) Executions.getCurrent().getSession().getAttribute(SingleInstanceOverviewPreferenceBean.SESSION_VARIABLE_NAME + "_" + instanceId);
		if (overBean == null || overBean.getInstanceId() != instanceId) {
			overBean = new SingleInstanceOverviewPreferenceBean(instanceId);
		}
		return overBean;
		
	}
	
	public void setOverviewPreferenceInSession(SingleInstanceOverviewPreferenceBean bean) {
		Executions.getCurrent().getSession().setAttribute(SingleInstanceOverviewPreferenceBean.SESSION_VARIABLE_NAME + "_" + bean.getInstanceId(), bean);
    }
}
