package com.idera.sqldm.ui.preferences;

import java.util.List;
import java.util.Map;

import org.zkoss.zul.ListModelList;

import com.idera.sqldm.data.instances.GraphCategories;
import com.idera.sqldm.data.instances.QueryWaits;

public class SingleInstanceOverviewPreferenceBean {

	public final static String SESSION_VARIABLE_NAME = "OverviewSessionDataBean";
	
	private final int instanceId;
	
	private boolean isFirstLoad = true;
	
	ListModelList<GraphCategories> manageGraphCategoriesSetting;
	private List<String> selectedFileComponent;
	private int selectedComponentIndex = 0;
	private int selectedPerSecIndex = 0;
	List<QueryWaits> queryWaitsModelData;
	List<Map<String, Map<String, Number>>> queryWaitsTimeStackedChartData;
	 	
	public int getSelectedComponentIndex() {
		return selectedComponentIndex;
	}

	public void setSelectedComponentIndex(int selectedComponentIndex) {
		this.selectedComponentIndex = selectedComponentIndex;
	}

	public int getSelectedPerSecIndex() {
		return selectedPerSecIndex;
	}

	public void setSelectedPerSecIndex(int selectedPerSecIndex) {
		this.selectedPerSecIndex = selectedPerSecIndex;
	}

	public List<String> getSelectedFileComponent() {
		return selectedFileComponent;
	}

	public void setSelectedFileComponent(List<String> selectedFileComponent) {
		this.selectedFileComponent = selectedFileComponent;
	}

	SingleInstanceOverviewPreferenceBean(int instanceId) {
		this.instanceId = instanceId;
	}

	public boolean isFirstLoad() {
		return isFirstLoad;
	}

	public void setFirstLoad(boolean isFirstLoad) {
		this.isFirstLoad = isFirstLoad;
	}

	public void setCustomizedGraphSettings(
			ListModelList<GraphCategories> graphCategoryOptions) {
		manageGraphCategoriesSetting =  graphCategoryOptions;
	}
	
	public ListModelList<GraphCategories> getCustomizedGraphSettings() {
		return manageGraphCategoriesSetting ;
	}
	public int getInstanceId() {
		return instanceId;
	}
	public int setInstanceId(int instanceId) {
		return instanceId;
	}

	public List<QueryWaits> getQueryWaitsModelData() {
		return queryWaitsModelData;
	}

	public void setQueryWaitsModelData(List<QueryWaits> queryWaitsModelData) {
		this.queryWaitsModelData = queryWaitsModelData;
	}

	public List<Map<String, Map<String, Number>>> getTimeStackedQueryChartData() {
		return queryWaitsTimeStackedChartData;
	}

	public void setTimeStackedQueryChartData(
			List<Map<String, Map<String, Number>>> timeStackedQueryChartData) {
		this.queryWaitsTimeStackedChartData = timeStackedQueryChartData;
	}

	public List<Map<String, Map<String, Number>>> getTimeStackedChartData() {
		return queryWaitsTimeStackedChartData;
	}

	public void setTimeStackedChartData(
			List<Map<String, Map<String, Number>>> timeStackedChartData) {
		this.queryWaitsTimeStackedChartData = timeStackedChartData;
	}

}
