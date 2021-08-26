package com.idera.sqldm.ui.dashboard.instances.resources;

import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.TimeUnit;

import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.A;
import org.zkoss.zul.Button;
import org.zkoss.zul.Popup;
import org.zkoss.zul.Spinner;

import com.idera.common.Utility;
import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.ResourcesFacade;
import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.data.category.resources.GraphBaseline;
import com.idera.sqldm.data.category.resources.ResourceCategory;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.ui.components.charts.IderaChartModel;
import com.idera.sqldm.ui.components.charts.area.IderaAreaChart;
import com.idera.sqldm.ui.components.charts.area.IderaAreaChartModel;
import com.idera.sqldm.ui.components.charts.line.IderaLineChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.ui.preferences.HistoryPanelPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.topten.TopXComposer;

@SuppressWarnings("rawtypes")
public class CPUUsageComposer extends TopXComposer{
	
	private static final long serialVersionUID = 7411480761432590068L;
	@Wire
	IderaLineChart sqlCPUUsage;
	@Wire IderaLineChart processorTime;
	@Wire IderaAreaChart processorQueueTime;
	
	@Wire Spinner sessionsCount;
	@Wire Button applySessionsConfig;
	@Wire Popup sessionsWidgetPopup;	

	@Wire A cpuUsage12;	
	@Wire A cpuUsage8;	
	@Wire A cpuUsage4;	
	@Wire A cpuUsage1;	
	@Wire A cpuUsage30;	

	private int instanceId;
	private String productInstanceName;
	private long OFFSET_MILLIS = 0;
	private boolean historyEvent = false;	
	@SuppressWarnings("unchecked")
	@Override
	public void doAfterCompose(Component comp) throws Exception {

        super.doAfterCompose(comp);
        OFFSET_MILLIS = com.idera.sqldm.utils.Utility.cancelOffSetInMillis();
        Integer instanceIdParameter = Utility.getIntUrlParameter(Executions.getCurrent().getParameterMap(), "id");
		if (instanceIdParameter != null) {
			instanceId = instanceIdParameter;
		}else{
			//fallback
			Object param = Executions.getCurrent().getDesktop().getAttribute("instanceId");
			if(param != null){
				instanceId = (Integer) param;
			}
		}
    	String productInstanceParameter=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
    	if(productInstanceParameter != null) {
    		productInstanceName = productInstanceParameter;
    	} else {
    		Object param = Executions.getCurrent().getDesktop().getAttribute("instance");
			if(param != null) {
				productInstanceName = (String) param;
			}
    	}

		refreshModels();
		updateDefaultValue(TopXEnum.INSTANCE_SESSIONS_CPU_ACTIVITY, sessionsCount);
		
		TopXEnum.INSTANCE_SESSIONS_CPU_ACTIVITY.setInstanceId(instanceId);
		List<TopXEnum> teList = new ArrayList<>();
		teList.add(TopXEnum.INSTANCE_SESSIONS_CPU_ACTIVITY);
		EventQueue<Event> eq = EventQueues.lookup("updateInstanceWidgets",
				EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("refreshData", null, teList));
		}
		
		EventQueue<Event> eq1 = EventQueues.lookup("historyChange1",
				EventQueues.DESKTOP, true);
		eq1.subscribe(new EventListener<Event>() {

			@Override
			public void onEvent(Event event) throws Exception {
				refreshGraphs();
			}
		});
		
		EventQueue<Event> eq2 = EventQueues.lookup("updateResourcesTab",
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
    		refreshModels();
    	}
	}

	protected void refreshGraphs() {
    	historyEvent = true;
	}
	
	private void refreshModels() throws CategoryException{
		
		Map<String, IderaChartModel> modelsMap = getModelData();
		sqlCPUUsage.refresh();
		processorTime.refresh();
		processorQueueTime.refresh();
		refreshCpuUsage(60);
		
		processorTime.setModel(modelsMap.get("PROCESSOR_TIME"));
//		processorTime.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESOURCES_PROCESSOR_TIME_IN_PERCENTAGE));
		if(processorTime.getChart()!=null){
			 processorTime.getChart().setyAxisCustomMaxDomainValue(true);
			 processorTime.getChart().setyAxisCustomMinDomainValue(true);
			 processorTime.getChart().setyAxisMinDomainValue(0);
			 processorTime.getChart().setyAxisMaxDomainValue(100);
			 
			 processorTime.getChart().setDrillable(true);
	    }
		 
		processorQueueTime.setModel(modelsMap.get("PROC_QUEUE"));
		/*
		 * Author:Accolite
		 * Date : 11th Nov, 2016
		 * Drill  down Capability - SQLDM- 10.2 release
		 */
		processorQueueTime.getChart().setDrillable(true);
//		processorQueueTime.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESOURCES_PROCESSOR_QUEUE_LENGTH));
	}
		
	
	private Map<String, IderaChartModel> getModelData() throws CategoryException {
//		String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		List<ResourceCategory> resourceCategoryList = null;
        if(pref.getScaleCombobox().getValue().equals("Custom")){
        	//resourceCategoryList = ResourcesFacade.getResources(productInstanceName,instanceId, 30);
        	Date startTime = pref.getFromDateTime();
			Date endTime = pref.getToDateTime();
			long duration  = endTime.getTime() - startTime.getTime();
			long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
        	resourceCategoryList = ResourcesFacade.getResources(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
        }
        else{
        	resourceCategoryList = ResourcesFacade.getResources(productInstanceName,instanceId, pref.getNumHistoryMinutes());
        }
		
		
		Map<String, IderaChartModel> modelsMap = new HashMap<String, IderaChartModel>();
		
		if(resourceCategoryList !=null && resourceCategoryList.size() >1){ //We need at least 2 point to plot a graph
		IderaLineChartModel procTimeModel = new IderaLineChartModel();
        IderaAreaChartModel procQueueModel = new IderaAreaChartModel();
		
		modelsMap.put("PROCESSOR_TIME", procTimeModel);
		modelsMap.put("PROC_QUEUE", procQueueModel);
		
		for(int i=resourceCategoryList.size()-1; i>=0; i--){
			ResourceCategory resource = resourceCategoryList.get(i);
			//Adding Offset for graphs
			long collectionTime = resource.getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
			
			procTimeModel.setValue("Privileged Time", collectionTime, resource.getCpu().getProcessorPrivilegedTimePercent());
			procTimeModel.setValue("User Time", collectionTime, resource.getCpu().getProcessorTime());
			
			procQueueModel.setValue("Processor Queue Length", collectionTime, resource.getCpu().getProcessorQueueLength());
		}
		}else{
			setEmptyCharts();
		}
		return modelsMap;
	}
	
	private void setEmptyCharts() {
		sqlCPUUsage.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		processorTime.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		processorQueueTime.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
	}

	private void refreshCpuUsage(int numOfMinutes) {

//		String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
		List<ResourceCategory> resourceCategoryList;
		List<GraphBaseline> baseline;
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		try {
			if(pref.getScaleCombobox().getValue().equals("Custom")){
				Date startTime = pref.getFromDateTime();
				Date endTime = pref.getToDateTime();
				long duration  = endTime.getTime() - startTime.getTime();
				long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
	        	resourceCategoryList = ResourcesFacade.getResources(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
	        	baseline = ResourcesFacade.getBaseline(productInstanceName, instanceId, 0, numHistoryMinutes,pref.getToDateTime());
	        }
	        else{
	        	resourceCategoryList = ResourcesFacade.getResources(productInstanceName,instanceId, pref.getNumHistoryMinutes());
	        	baseline = ResourcesFacade.getBaseline(productInstanceName, instanceId, 0, pref.getNumHistoryMinutes());
	        }
			//Adding Offset for graphs
			if(resourceCategoryList.size()!=0){
			long startTimeUTC  = resourceCategoryList.get(0).getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
			long endTimeUTC  = resourceCategoryList.get(resourceCategoryList.size()-1).getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
			if(resourceCategoryList !=null && resourceCategoryList.size() >1){ //We need at least 2 point to plot a graph
				IderaLineChartModel cpuUsageModel = new IderaLineChartModel();

				for(int i=resourceCategoryList.size()-1; i>=0; i--){
					ResourceCategory resource = resourceCategoryList.get(i);
					//Adding Offset for graphs
					long collectionTime = resource.getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
					cpuUsageModel.setValue("SQL Server", collectionTime, resource.getCpu().getSqlCPUUsage());
					cpuUsageModel.setValue("OS", collectionTime, resource.getCpu().getOSCPUUsage());
				}

				if(baseline != null) {
					int n = baseline.size();
					for(int i=0; i<n; i++){
						GraphBaseline baselineValue = baseline.get(i);
						//Adding Offset for graphs
						long collectionStartTime = baselineValue.getStartTime().getTime()+OFFSET_MILLIS;
						
						if(collectionStartTime >= startTimeUTC)
							cpuUsageModel.setValue("Baseline", collectionStartTime, baselineValue.getValue());
						else
							cpuUsageModel.setValue("Baseline", startTimeUTC, baselineValue.getValue());

						if(baselineValue.getEndTime() != null) {
							//Adding Offset for graphs
							long collectionEndTime = baselineValue.getEndTime().getTime()+OFFSET_MILLIS;
							if(collectionEndTime >= startTimeUTC)
								cpuUsageModel.setValue("Baseline", collectionEndTime, baselineValue.getValue());
							else
								cpuUsageModel.setValue("Baseline", startTimeUTC, baselineValue.getValue());
						} else {
							cpuUsageModel.setValue("Baseline", endTimeUTC, baselineValue.getValue());
						}
					}
				}
				
				sqlCPUUsage.setModel(cpuUsageModel);
				if(sqlCPUUsage.getChart()!=null){
					sqlCPUUsage.getChart().setyAxisCustomMaxDomainValue(true);
					sqlCPUUsage.getChart().setyAxisCustomMinDomainValue(true);
					sqlCPUUsage.getChart().setyAxisMinDomainValue(0);
					sqlCPUUsage.getChart().setyAxisMaxDomainValue(100);
					/*
					 * Author:Accolite
					 * Date : 11th Nov, 2016
					 * Drill  down Capability - SQLDM- 10.2 release
					 */
					sqlCPUUsage.getChart().setDrillable(true);
					
				}
				}

			} else {
				sqlCPUUsage.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));			
			}
		} catch (CategoryException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
	}

	@Listen("onClick = #applySessionsConfig")
	public void refreshSESWidget(){
		publishWidgetSettings(TopXEnum.INSTANCE_SESSIONS_CPU_ACTIVITY, null, sessionsCount, sessionsWidgetPopup);
	}
	
	/*@Listen("onClick = #cpuUsageDiv > a")
	public void updateGraph(Event event) {
		
		String targetName = event.getTarget().getId();
		A link = (A) event.getTarget();
		int numOfMinutes;
		
		String defaultStyle = "vertical-align: -webkit-baseline-middle;";
		
		cpuUsage12.setStyle(defaultStyle + "font-weight:normal");
		cpuUsage8.setStyle(defaultStyle + "font-weight:normal");
		cpuUsage4.setStyle(defaultStyle + "font-weight:normal");
		cpuUsage1.setStyle(defaultStyle + "font-weight:normal");
		cpuUsage30.setStyle(defaultStyle + "font-weight:normal");
		
		if(targetName.contains("12")) {
			numOfMinutes = 12 * 60;
		} else if(targetName.contains("8")) {
			numOfMinutes = 8 * 60;
		} else if(targetName.contains("4")) {
			numOfMinutes = 4 * 60;
		} else if(targetName.contains("1")) {
			numOfMinutes = 1 * 60;
		} else {
			numOfMinutes = 30;
		}

		refreshCpuUsage(numOfMinutes);
		link.setStyle(defaultStyle + "font-weight: bold");
	}*/

}
