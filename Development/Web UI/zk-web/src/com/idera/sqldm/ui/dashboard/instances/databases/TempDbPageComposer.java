package com.idera.sqldm.ui.dashboard.instances.databases;

import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.TimeUnit;

import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zkplus.databind.BindingListModelList;
import org.zkoss.zul.Div;
import org.zkoss.zul.ListModelList;

import com.idera.common.rest.RestException;
import com.idera.sqldm.data.ResourcesFacade;
import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.data.category.resources.ResourceCategory;
import com.idera.sqldm.data.databases.InstanceDetailsDatabase;
import com.idera.sqldm.data.databases.InstanceDetailsDatabaseFacade;
import com.idera.sqldm.data.databases.TempDBUsageDetails;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.ui.components.charts.bar.IderaBarChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.ui.preferences.HistoryPanelPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.utils.Utility;

public class TempDbPageComposer extends SelectorComposer<Div> {
	private static final long serialVersionUID = 1L;
	protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory.getLogger(TempDbPageComposer.class);

	private int instanceId = 0;

	@Wire protected IderaBarChart tempdbUsedByFile;


	@Wire protected  IderaLineChart tempdbUsedByTime;

	@Wire protected  IderaLineChart versionStoreChart;

	protected AnnotateDataBinder binder = null;

	public ListModelList<InstanceDetailsDatabase> databasesModel = new BindingListModelList<InstanceDetailsDatabase>(new ArrayList<InstanceDetailsDatabase>(), false);
	private List<TempDBUsageDetails> tempDBUsageDetails;
	private long OFFSET_MILLIS = 0;
	private boolean historyEvent = false;
	@Override
	public void doAfterCompose(Div comp) throws Exception {
		super.doAfterCompose(comp);
		OFFSET_MILLIS = Utility.cancelOffSetInMillis();
		Integer instanceIdParameter = Utility.getIntUrlParameter(Executions.getCurrent().getParameterMap(), "id");
		if (instanceIdParameter != null) {
			instanceId = instanceIdParameter;
		}
		else{
			//fallback
			Object param = Executions.getCurrent().getDesktop().getAttribute("instanceId");
			if(param != null){
				instanceId = (Integer) param;
			}
		}
    	String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");

		drawChart(productInstanceName);
		EventQueue<Event> eq1 = EventQueues.lookup("historyChange1",
				EventQueues.DESKTOP, true);
		eq1.subscribe(new EventListener<Event>() {

			@Override
			public void onEvent(Event event) throws Exception {
				refreshGraphs();
			}
		});
		
		EventQueue<Event> eq2 = EventQueues.lookup("updateDatabaseTab",
				EventQueues.DESKTOP, true);
		eq2.subscribe(new EventListener<Event>() {

			@Override
			public void onEvent(Event event) throws Exception {
				refreshChart();
			}
		});
	}
	
	protected void refreshChart() throws CategoryException {
    	if(historyEvent){
    		historyEvent  = false;
    		tempDBUsageDetails = null;
    		String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
    		drawChart(productInstanceName);
    	}
	}

	protected void refreshGraphs() {
    	historyEvent = true;
	}

	protected void drawChart(String productInstanceName) {
		drawFileChart(productInstanceName);
		drawTempDbByTimeChart(productInstanceName);
	}

	protected void drawFileChart(String productInstanceName) {
		try {
			Map<String, Double> chartData =	loadTempDbByFileData(productInstanceName);
			tempdbUsedByFile.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.TEMP_DB_BY_FILE_MB));
			if ((chartData != null) && (!chartData.isEmpty())) {
				tempdbUsedByFile.setShowZeroValues(true);
				tempdbUsedByFile.setFitToHeight(false);
				tempdbUsedByFile.setOrient("vertical");
				tempdbUsedByFile.setAnimateBars(true);
				tempdbUsedByFile.setAnimationLength(500);
				tempdbUsedByFile.setAnimationDelay(100);
				tempdbUsedByFile.setChartData(chartData);
				tempdbUsedByFile.setVisible(true);
			} else {

				setEmptyBarCharts();
			}

		} catch (RestException x) {
			tempdbUsedByFile.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.ERROR_OCCURRED_LOADING_CHART));
		}

	}

	private Map<String, Double> loadTempDbByFileData(String productInstanceName) throws RestException {
		if (tempDBUsageDetails == null) {
			HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
	        if(pref.getScaleCombobox().getValue().equals("Custom")){
	        	Date startTime = pref.getFromDateTime();
				Date endTime = pref.getToDateTime();
				long duration  = endTime.getTime() - startTime.getTime();
				long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
	        	tempDBUsageDetails = InstanceDetailsDatabaseFacade.getTempDBUsageDetails(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
	        }
	        else{
	        	tempDBUsageDetails = InstanceDetailsDatabaseFacade.getTempDBUsageDetails(productInstanceName,instanceId, pref.getNumHistoryMinutes());
	        }
		}

		Map<String, Double> chartData = new HashMap<String, Double>();
		if (tempDBUsageDetails != null & tempDBUsageDetails.size() >0){
			TempDBUsageDetails usg = tempDBUsageDetails.get(0);
			chartData.put("Internal Objects", Utility.round(usg.getInternalObjectsInMB(), 2));
			chartData.put("Unallocated Space", Utility.round(usg.getUnallocatedSpaceInMB(), 2));
			chartData.put("User Objects", Utility.round(usg.getUserObjectsInMB(), 2));
			chartData.put("Mixed Extents", Utility.round(usg.getMixedExtentsInMB(), 2));
			chartData.put("Version Store", Utility.round(usg.getVersionStoreInMB(), 2));
		}
		return chartData;
	}

	protected void drawTempDbByTimeChart(String productInstanceName) {
		IderaLineChartModel model = new IderaLineChartModel();
		IderaLineChartModel versionStoreModel = new IderaLineChartModel();
		tempdbUsedByTime.refresh();
		versionStoreChart.refresh();
		try {
			if (tempDBUsageDetails == null) {
				HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		        if(pref.getScaleCombobox().getValue().equals("Custom")){
		        	Date startTime = pref.getFromDateTime();
					Date endTime = pref.getToDateTime();
					long duration  = endTime.getTime() - startTime.getTime();
					long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
		        	tempDBUsageDetails = InstanceDetailsDatabaseFacade.getTempDBUsageDetails(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
		        }
		        else{
		        	tempDBUsageDetails = InstanceDetailsDatabaseFacade.getTempDBUsageDetails(productInstanceName,instanceId, pref.getNumHistoryMinutes());
		        }
			}
			if (tempDBUsageDetails != null & tempDBUsageDetails.size() >1){

				for (TempDBUsageDetails usg : tempDBUsageDetails) {
					//Adding offset for graphs
					long collectionTime = usg.getUTCCollectionDateTime().getTime() + OFFSET_MILLIS;
					model.setValue("Internal Objects",collectionTime, Utility.round(usg.getInternalObjectsInMB(), 2));
					model.setValue("Unallocated Space",collectionTime, Utility.round(usg.getUnallocatedSpaceInMB(), 2));
					model.setValue("User Objects", collectionTime, Utility.round(usg.getUserObjectsInMB(), 2));
					model.setValue("Mixed Extents", collectionTime, Utility.round(usg.getMixedExtentsInMB(), 2));
					model.setValue("Version Store", collectionTime, Utility.round(usg.getVersionStoreInMB(), 2));

					versionStoreModel.setValue("Version Cleanup Rate",collectionTime, Utility.round(usg.getVersionStoreCleanupKilobytesPerSec(), 2));
					versionStoreModel.setValue("Version Generation Rate",collectionTime, Utility.round(usg.getVersionStoreGenerationKilobytesPerSec(), 2));

				}
				tempdbUsedByTime.setModel(model); 				
				versionStoreChart.setModel(versionStoreModel);
				/*
				 * Author:Accolite
				 * Date : 11th Nov, 2016
				 * Drill  down Capability - SQLDM- 10.2 release
				 */
				tempdbUsedByTime.getChart().setDrillable(true);
				versionStoreChart.getChart().setDrillable(true);
				
				
			}else{

				setEmptyLineCharts();
			}	
			tempdbUsedByTime.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.TEMP_DB_BY_TIME_MB));
			versionStoreChart.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.VERSION_STORE_KILOBYTE_PER_SEC));
		} catch (Exception x) {
			tempdbUsedByTime.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.ERROR_OCCURRED_LOADING_CHART));
		}
	}

	private void setEmptyLineCharts() {
		tempdbUsedByTime.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		versionStoreChart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
	}

	private void setEmptyBarCharts() {
		tempdbUsedByFile.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));

	}


}

