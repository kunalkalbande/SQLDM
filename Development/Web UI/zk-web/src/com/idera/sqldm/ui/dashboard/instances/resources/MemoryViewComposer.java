package com.idera.sqldm.ui.dashboard.instances.resources;

import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.TimeUnit;

import com.idera.server.web.WebConstants;
import com.idera.sqldm.ui.components.charts.IderaChartModel;
import com.idera.sqldm.ui.components.charts.area.IderaAreaChart;
import com.idera.sqldm.ui.components.charts.area.IderaAreaChartModel;

import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Wire;

import com.idera.common.Utility;
import com.idera.sqldm.data.ResourcesFacade;
import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.data.category.resources.ResourceCategory;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.ui.components.charts.line.IderaLineChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.ui.preferences.HistoryPanelPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;

@SuppressWarnings("rawtypes")
public class MemoryViewComposer extends SelectorComposer{

	private static final long serialVersionUID = 9200286224241425901L;
	@Wire private IderaLineChart sqlMemoryUsage;
	@Wire private IderaLineChart memoryAreas;
	@Wire private IderaAreaChart pageLifeExpectancy;
	@Wire private IderaLineChart cacheHitRatios;
	private int instanceId;
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
		} else {
			Object param = Executions.getCurrent().getDesktop().getAttribute("instanceId");
			if(param != null){
				instanceId = (Integer) param;
			}
		}
		
        refreshModels();
        
        /*
         * ChaitanyaTanwar DM 10.2 History Panel
         * */
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
    	if(historyEvent ){
    		historyEvent  = false;
    		refreshModels();
    	}
	}

	protected void refreshGraphs() {
    	historyEvent = true;
	}
	
	private void refreshModels() throws CategoryException{
		sqlMemoryUsage.refresh();
		memoryAreas.refresh();
		pageLifeExpectancy.refresh();
		cacheHitRatios.refresh();
		Map<String, IderaChartModel> modelsMap = getModelData();
		sqlMemoryUsage.setModel(modelsMap.get("MEMORY_USAGE"));
		sqlMemoryUsage.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESOURCES_SQL_MEMORY_USAGE_IN_MB));
		
		
		/*
		 * Author:Accolite
		 * Date : 11th Nov, 2016
		 * Drill  down Capability - SQLDM- 10.2 release
		 */
		sqlMemoryUsage.getChart().setDrillable(true);
		memoryAreas.getChart().setDrillable(true);
		
		
		
		memoryAreas.setModel(modelsMap.get("MEMORY_AREAS"));
		memoryAreas.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESOURCES_MEMEORY_AREAS_IN_MB));
		
		pageLifeExpectancy.setModel(modelsMap.get("PAGE_LIFE"));
		pageLifeExpectancy.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESOURCES_PAGE_LIFE_EXPECTANCY));
        if(pageLifeExpectancy.getChart() != null){
            pageLifeExpectancy.getChart().setXAxisLegendSpacing(new Integer(35));
            //Drilldown Capability - SQLDM- 10.2 release
            pageLifeExpectancy.getChart().setDrillable(true);
        }
		
		cacheHitRatios.setModel(modelsMap.get("CACHE_HIT"));
		cacheHitRatios.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESOURCES_CACHE_HIT_RATIOS_PERCENT));

        if(cacheHitRatios.getChart() != null){
            cacheHitRatios.getChart().setyAxisCustomMinDomainValue(true);
            cacheHitRatios.getChart().setyAxisMinDomainValue(0);
            cacheHitRatios.getChart().setyAxisCustomMaxDomainValue(true);
            cacheHitRatios.getChart().setyAxisMaxDomainValue(100);
            //Drilldown Capability - SQLDM- 10.2 release
            cacheHitRatios.getChart().setDrillable(true);
        }
	}
	
	private Map<String, IderaChartModel> getModelData() throws CategoryException {
		String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		List<ResourceCategory> resourceCategoryList = null;
		if(pref.getScaleCombobox().getValue().equals("Custom")){
			Date startTime = pref.getFromDateTime();
			Date endTime = pref.getToDateTime();
			long duration  = endTime.getTime() - startTime.getTime();
			long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
        	//resourceCategoryList = ResourcesFacade.getResources(productInstanceName,instanceId, 30);
        	resourceCategoryList = ResourcesFacade.getResources(productInstanceName,instanceId, numHistoryMinutes, pref.getToDateTime());
        }
        else{
        	resourceCategoryList = ResourcesFacade.getResources(productInstanceName,instanceId, pref.getNumHistoryMinutes());
        }
		
		
		Map<String, IderaChartModel> modelsMap = new HashMap<String, IderaChartModel>();
		
		if(resourceCategoryList !=null && resourceCategoryList.size() >1){
		IderaLineChartModel memoryUsageModel = new IderaLineChartModel();
		IderaLineChartModel memoryAreasModel = new IderaLineChartModel();
		IderaAreaChartModel pageLifeModel = new IderaAreaChartModel();
		IderaLineChartModel cacheHitModel = new IderaLineChartModel();
		
		modelsMap.put("MEMORY_USAGE", memoryUsageModel);
		modelsMap.put("MEMORY_AREAS", memoryAreasModel);
		modelsMap.put("PAGE_LIFE", pageLifeModel);
		modelsMap.put("CACHE_HIT", cacheHitModel);
		
		for(int i=resourceCategoryList.size()-1; i>=0; i--){
			ResourceCategory resource = resourceCategoryList.get(i);
			//Adding Offset for graphs
			long collectionTime = resource.getUTCCollectionDateTime().getTime() + OFFSET_MILLIS;
		
			memoryAreasModel.setValue("Procedure Cache", collectionTime, Math.round(resource.getMemory().getMemoryAreas().get("ProcedureCacheSizeInKB")/1024*100.0)/100.0);
			memoryAreasModel.setValue("Connections", collectionTime, Math.round(resource.getMemory().getMemoryAreas().get("ConnectionsMemoryInKB")/1024*100.0)/100.0);
			memoryAreasModel.setValue("Locks", collectionTime, Math.round(resource.getMemory().getMemoryAreas().get("LocksMemoryInKB")/1024*100.0)/100.0);
			memoryAreasModel.setValue("Database", collectionTime, Math.round(resource.getMemory().getMemoryAreas().get("DatabaseMemoryInKB")/1024*100.0)/100.0);
			
			memoryUsageModel.setValue("SQL Used", collectionTime, Math.round(resource.getMemory().getSqlMemory().get("UsedMemoryInKB")/1024*100.0)/100.0);
			memoryUsageModel.setValue("SQL Allocated", collectionTime, Math.round(resource.getMemory().getSqlMemory().get("AllocatedMemoryInKB")/1024*100.0)/100.0);
			memoryUsageModel.setValue("Total Used", collectionTime, Math.round(resource.getMemory().getSqlMemory().get("TotalMemoryInKB")/1024*100.0)/100.0);
			
			pageLifeModel.setValue("Page Life Expectancy", collectionTime, resource.getMemory().getPageLifeExpectancyinSec());
			
			cacheHitModel.setValue("Buffer Cache", collectionTime, resource.getMemory().getBufferCacheHitRatio());
			cacheHitModel.setValue("Procedure Cache", collectionTime, resource.getMemory().getProcedureCacheHitRatio());
		}
		}else{
			setEmptyCharts();
		}
		return modelsMap;
	}
	
	private void setEmptyCharts() {
		sqlMemoryUsage.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		memoryAreas.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		pageLifeExpectancy.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		cacheHitRatios.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
	}
	
}
