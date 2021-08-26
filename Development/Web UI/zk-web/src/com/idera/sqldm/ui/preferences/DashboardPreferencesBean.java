package com.idera.sqldm.ui.preferences;

import java.util.Map;

import java.util.LinkedList;
import java.util.List;

import com.idera.sqldm.ui.dashboard.DashboardInstanceViewComposer.FilterType;


public class DashboardPreferencesBean extends IPreferencesBean {

    public final static String SESSION_VARIABLE__NAME = "DashboardSessionDataBean";

    private DashboardCategoryGroup leftCategoryGroup;
    private DashboardInstanceView instancesView;
    private DashboardGroupByInstances groupByInstances;
    private Map<Integer, Object> filteredInstancesMap;
    private List<Integer> heatMapOpenInstances;
    private int instanceRowCount = -1;
    private int alertsRowCount = -1;
    private String searchText;
    //Single instances page count for each grid - Keeping it in this bean rather than single instance preferences bean because
    //the counts are independent of instanceid

    private int overviewDBGridPageCount = -1;
    private int sessionsGridPageCount = -1;
    private int queriesGridPageCount = -1;
    private int resoucesSWGridPageCount = -1;
    private int databaseSummaryGridpageCount = -1;
    private int avaGroupGridpageCount = -1;

    private String productInstanceName = null;

    private String selectedSqldmTab = null;

    private List<String> listViewVisibleColumns;

    private List<String> listViewHiddenColumns;

    public DashboardPreferencesBean() {
    }

    public String getProductInstanceName() {
        return this.productInstanceName;
    }

    public void setProductInstanceName(String productInstanceName) {
        this.productInstanceName = productInstanceName;
    }

    public void setLeftCategoryGroup(DashboardCategoryGroup leftCategoryGroup) {
        this.leftCategoryGroup = leftCategoryGroup;
    }

    public DashboardCategoryGroup getLeftCategoryGroup() {
        return leftCategoryGroup;
    }

    public void setInstancesView(DashboardInstanceView instancesView) {
        this.instancesView = instancesView;
    }

    public DashboardInstanceView getInstancesView() {
        return instancesView;
    }

    public DashboardGroupByInstances getGroupByInstances() {
        return groupByInstances;
    }

    public void setGroupByInstances(DashboardGroupByInstances groupByInstances) {
        this.groupByInstances = groupByInstances;
    }

    public int getInstanceRowCount() {
        return instanceRowCount;
    }

    public void setInstanceRowCount(int instanceRowCount) {
        this.instanceRowCount = instanceRowCount;
    }

    public int getAlertsRowCount() {
        return alertsRowCount;
    }

    public void setAlertsRowCount(int alertsRowCount) {
        this.alertsRowCount = alertsRowCount;
    }

    public List<Integer> getHeatMapOpenInstances() {
        return heatMapOpenInstances;
    }

    public void setHeatMapOpenInstances(List<Integer> heatMapOpenInstances) {
        this.heatMapOpenInstances = heatMapOpenInstances;
    }

    public void addHeatMapOpenInstance(Integer instanceId) {
        if (this.heatMapOpenInstances == null) {
            this.heatMapOpenInstances = new LinkedList<>();
        }
        this.heatMapOpenInstances.add(instanceId);
    }

    public void removeHeatMapOpenInstance(Integer instanceId) {
        if (this.heatMapOpenInstances == null) {
            return;
        }
        this.heatMapOpenInstances.remove(instanceId);
    }

    public String getSearchText() {
        return searchText;
    }

    public void setSearchText(String searchText) {
        this.searchText = searchText;
    }

    private boolean leftPanelStatus = true;

    public void setLeftPanelStatus(boolean leftPanelStatus) {
        this.leftPanelStatus = leftPanelStatus;
    }

    public boolean isLeftPanelStatus() {
        return leftPanelStatus;
    }

    private boolean rightPanelStatus = true;

    public boolean isRightPanelStatus() {
        return rightPanelStatus;
    }

    public void setRightPanelStatus(boolean rightPanelStatus) {
        this.rightPanelStatus = rightPanelStatus;
    }

    private String instancePanelHeight = "300px";

    public String getInstancePanelHeight() {
        return instancePanelHeight;
    }

    public void setInstancePanelHeight(String instancePanelHeight) {
        this.instancePanelHeight = instancePanelHeight;
    }

    public static enum DashboardGroupByInstances {
        SEVERITY("severity", "Severity"),
        GROUPBY("groupBy", "-Group By-"),
        TAGS("tags", "Tags"),
        SQLDM_REPO("sqldmrepo", "SQLdmRepo");

        private String type;
        private String groupBy;

        private DashboardGroupByInstances(String type, String groupBy) {
            this.type = type;
            this.groupBy = groupBy;
        }

        public String getType() {
            return type;
        }

        public String getGroupBy() {
            return groupBy;
        }

    }

    public static enum DashboardCategoryGroup {
        SEVERITY("severity", FilterType.STATUS),
        TAGS("tags", FilterType.TAG);

        private String type;
        private FilterType filterType;
        private String name;

        private DashboardCategoryGroup(String type, FilterType filterType) {
            this.type = type;
            this.filterType = filterType;
        }

        public String getType() {
            return type;
        }

        public FilterType getFilterType() {
            return filterType;
        }

        public void setFilterType(FilterType filterType) {
            this.filterType = filterType;
        }

        public String getName() {
            return name;
        }

        public void setName(String name) {
            this.name = name;
        }
    }

    public static enum DashboardInstanceView {
        LIST("list", 1),
        THUMBNAIL("thumbnail", 2),
        HEATMAP("heatmap", 3);

        private final String view;
        private final int code;

        private DashboardInstanceView(String view, int code) {
            this.view = view;
            this.code = code;
        }

        public String getView() {
            return view;
        }

        public int getCode() {
            return code;
        }
    }

    public void setFilteredInstancesMap(Map<Integer, Object> instancesHashList) {
        this.filteredInstancesMap = instancesHashList;
    }

    public Map<Integer, Object> getFilteredInstancesMap() {
        return filteredInstancesMap;
    }

    public int getOverviewDBGridPageCount() {
        return overviewDBGridPageCount;
    }

    public void setOverviewDBGridPageCount(int overviewDBGridPageCount) {
        this.overviewDBGridPageCount = overviewDBGridPageCount;
    }

    public int getSessionsGridPageCount() {
        return sessionsGridPageCount;
    }

    public void setSessionsGridPageCount(int sessionsGridPageCount) {
        this.sessionsGridPageCount = sessionsGridPageCount;
    }

    public int getQueriesGridPageCount() {
        return queriesGridPageCount;
    }

    public void setQueriesGridPageCount(int queriesGridPageCount) {
        this.queriesGridPageCount = queriesGridPageCount;
    }

    public int getResoucesSWGridPageCount() {
        return resoucesSWGridPageCount;
    }

    public void setResoucesSWGridPageCount(int resoucesSWGridPageCount) {
        this.resoucesSWGridPageCount = resoucesSWGridPageCount;
    }

    public int getDatabaseSummaryGridpageCount() {
        return databaseSummaryGridpageCount;
    }

    public void setDatabaseSummaryGridpageCount(int databaseSummaryGridpageCount) {
        this.databaseSummaryGridpageCount = databaseSummaryGridpageCount;
    }

    public int getAvaGroupGridpageCount() {
        return avaGroupGridpageCount;
    }

    public void setAvaGroupGridpageCount(int avaGroupGridpageCount) {
        this.avaGroupGridpageCount = avaGroupGridpageCount;
    }

    public String getSelectedSqldmTab() {
        return selectedSqldmTab;
    }

    public void setSelectedSqldmTab(String selectedSqldmTab) {
        this.selectedSqldmTab = selectedSqldmTab;
    }

    public void setInstanceListViewVisibleColumns(List<String> defaultColumnList) {
        this.listViewVisibleColumns = defaultColumnList;
    }

    public void setInstanceListViewHiddenColumns(List<String> hiddenList) {
        this.listViewHiddenColumns = hiddenList;
    }

    public List<String> getInstanceListViewVisibleColumns() {
        return this.listViewVisibleColumns;
    }

    public List<String> getInstanceListViewHiddenColumns() {
        return this.listViewHiddenColumns;
    }

}
