package com.idera.sqldm.ui.dashboard.instances.databases;

import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.TimeUnit;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Grid;
import org.zkoss.zul.Paging;

import com.idera.common.Utility;
import com.idera.common.rest.RestException;
import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.data.databases.AvailabilityGroupDetails;
import com.idera.sqldm.data.databases.AvailabilityGroupDetailsFacade;
import com.idera.sqldm.data.databases.AvailabilityGroupStatistics;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.ui.components.charts.line.IderaLineChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean;
import com.idera.sqldm.ui.preferences.HistoryPanelPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;

public class AvailabilityGrpsViewModel {
	private static final Logger LOGGER = Logger.getLogger(AvailabilityGrpsViewModel.class);
	private int instanceId = -1;
	@Wire IderaLineChart queueSize; 
	@Wire IderaLineChart transferRate;
	
	private List<AvailabilityGroupDetails> availabilityGroups;
	//private List<AvailabilityGroupStatistics> statistics;
	Map<String, List<AvailabilityGroupStatistics>> statisticsCache;
	private int availablityGroupsListRowsCount;
	private int listRowsCount;
	private int prevPageSize;
	private static int DEFAULT_ROWS_COUNT = 10;
	private long OFFSET_MILLIS = 0;
	@Wire Paging availabilityGroupListPgId;
	@Wire Grid availabilityGrpGrid;
	private boolean historyEvent = false;

	@Init
	public void init(){
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
         
        int defaultAlertsRowCount = DEFAULT_ROWS_COUNT;
		DashboardPreferencesBean dpb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();
		if (dpb != null && dpb.getAvaGroupGridpageCount() != -1) {
			defaultAlertsRowCount = dpb.getAvaGroupGridpageCount();
		}
		prevPageSize = defaultAlertsRowCount;
		setListRowsCount(prevPageSize);
		availablityGroupsListRowsCount = prevPageSize;
		
	}
	
	@AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view){
        Selectors.wireComponents(view, this, false);
        availabilityGrpGrid.setPaginal(availabilityGroupListPgId);
        int defaultAlertsRowCount = DEFAULT_ROWS_COUNT;
		DashboardPreferencesBean dpb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();
		if (dpb != null && dpb.getAvaGroupGridpageCount() != -1) {
			defaultAlertsRowCount = dpb.getAvaGroupGridpageCount();
		}
		prevPageSize = defaultAlertsRowCount;
		setListRowsCount(prevPageSize);
		availablityGroupsListRowsCount = prevPageSize;
    	String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");

        refreshAvailabilityGrps(productInstanceName);
        populateStatistics(productInstanceName);
        if(availabilityGroups == null || availabilityGroups.size() == 0){
        	setEmptyCharts();
        }
        else{
        	refreshChartModel(availabilityGroups.get(0));
        }
        
        /*
    	 * Author:Accolite
    	 * Date : 12th Dec, 2016
    	 * History Panel - SQLDM- 10.2 release
    	 * Adding History Panel to Database
    	 */
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
	
	/*
	 * Author:Accolite
	 * Date : 12th Dec, 2016
	 * History Panel - SQLDM- 10.2 release
	 * Adding History Panel to Database
	 */
	protected void refreshChart() throws CategoryException {
    	if(historyEvent){
    		historyEvent  = false;
    		String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
    		refreshAvailabilityGrps(productInstanceName);
            populateStatistics(productInstanceName);
            if(availabilityGroups == null || availabilityGroups.size() == 0){
            	setEmptyCharts();
            }
            else{
            	refreshChartModel(availabilityGroups.get(0));
            }
    	}
	}
	protected void refreshGraphs() {
    	historyEvent = true;
	}
	
	private void refreshAvailabilityGrps(String productInstanceName){
		availabilityGroups = new ArrayList<>();
		try{
			HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
	        if(pref.getScaleCombobox().getValue().equals("Custom")){
	        	Date startTime = pref.getFromDateTime();
				Date endTime = pref.getToDateTime();
				long duration  = endTime.getTime() - startTime.getTime();
				long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
				availabilityGroups.addAll(AvailabilityGroupDetailsFacade.getAvailabilityGroupDetails(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime()));
	        }
	        else{
	        	availabilityGroups.addAll(AvailabilityGroupDetailsFacade.getAvailabilityGroupDetails(productInstanceName,instanceId, pref.getNumHistoryMinutes()));
	        }
	        
	        updateAvailabiltyGroupViewModel();
			//availabilityGroups.addAll(AvailabilityGroupDetailsFacade.getAvailabilityGroupDetails(productInstanceName,instanceId));
		}
		catch(RestException e){
			LOGGER.error(e);
		}
	}
	
	private void populateStatistics(String productInstanceName){
		List<AvailabilityGroupStatistics> statistics = new ArrayList<>();
		try {
			HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
	        if(pref.getScaleCombobox().getValue().equals("Custom")){
	        	Date startTime = pref.getFromDateTime();
				Date endTime = pref.getToDateTime();
				long duration  = endTime.getTime() - startTime.getTime();
				long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
	        	statistics.addAll(AvailabilityGroupDetailsFacade.getAvailabilityGroupStatistics(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime()));
	        }
	        else{
	        	statistics.addAll(AvailabilityGroupDetailsFacade.getAvailabilityGroupStatistics(productInstanceName,instanceId, pref.getNumHistoryMinutes()));
	        }
		} catch (RestException e) {
			LOGGER.error(e);
		}
		statisticsCache = new HashMap<>();
		for(AvailabilityGroupStatistics stat : statistics){
			String key = getCacheKey(stat.getGroupName(), stat.getReplicaName());
			if(!statisticsCache.containsKey(key)){
				statisticsCache.put(key, new ArrayList<AvailabilityGroupStatistics>());
			}
			statisticsCache.get(key).add(stat);
		}
	}
	
	private String getCacheKey(String groupName, String replicaName){
		return groupName+replicaName;
	}
	
	@Command
	public void refreshChartModel(@BindingParam("avg") AvailabilityGroupDetails avg){
		if(avg == null){
			return;
		}
		queueSize.refresh();
		transferRate.refresh();
		String key = getCacheKey(avg.getGroupName(), avg.getReplicaName());
		queueSize.setTitle(avg.getGroupName() + " " + avg.getReplicaName() + " "+
				ELFunctions.getMessage(SQLdmI18NStrings.QUEUE_SIZE));
		//Set the queue  chart drillable attribute to true.
		queueSize.getChart().setDrillable(true);
		
		transferRate.setTitle(avg.getGroupName() + " " + avg.getReplicaName() + " "+
				ELFunctions.getMessage(SQLdmI18NStrings.TRANSFER_RATE_PERCENTAGE));
		//Set the transfer chart drillable attribute to true.
		transferRate.getChart().setDrillable(true);
		if(statisticsCache != null && statisticsCache.containsKey(key)){
			Map<String, IderaLineChartModel> modelsMap = getModelData(key);
			queueSize.setModel(modelsMap.get("QUEUE_SIZE"));
			transferRate.setModel(modelsMap.get("TRANSFER_RATE"));
		}
		else{
			setEmptyCharts();
		}
	}
	
	private Map<String, IderaLineChartModel> getModelData(String cacheKey){
		Map<String, IderaLineChartModel> modelsMap = new HashMap<String, IderaLineChartModel>();
		IderaLineChartModel queueSizeModel = new IderaLineChartModel();
		IderaLineChartModel transferRateModel = new IderaLineChartModel();
		List<AvailabilityGroupStatistics> statistics = statisticsCache.get(cacheKey);
		
		modelsMap.put("QUEUE_SIZE", queueSizeModel);
		modelsMap.put("TRANSFER_RATE", transferRateModel);
		
		for(AvailabilityGroupStatistics stat : statistics){
			//String collectionTime = printFormat.format(Utility.getLocalDateFromUTCDate(resource.getUTCCollectionDateTime()));
			long collectionTime = stat.getUTCCollectionDateTime().getTime()  +OFFSET_MILLIS;
			queueSizeModel.setValue(ELFunctions.getMessage(SQLdmI18NStrings.LOG_SEND_Q), collectionTime, stat.getLogSendQueueSize());
			queueSizeModel.setValue(ELFunctions.getMessage(SQLdmI18NStrings.REDO_Q), collectionTime, stat.getRedoQueueSize());
			
			transferRateModel.setValue(ELFunctions.getMessage(SQLdmI18NStrings.AG_LOGRATE), collectionTime, stat.getLogTransferRate());
			transferRateModel.setValue(ELFunctions.getMessage(SQLdmI18NStrings.AG_REDORATE), collectionTime, stat.getRedoTransferRate());
		}
		return modelsMap;
	}

	@NotifyChange("listRowsCount")
	public void setListRowsCount(int listRowsCount) {
		this.listRowsCount = listRowsCount;
		BindUtils.postNotifyChange(null, null, this, "listRowsCount");
	}
	
	@Command("setAvailablityGroupsRowsCount")
	public void setAvailablityGroupsRowsCount() {
		try {
			int pageSize = this.availablityGroupsListRowsCount;
			setListRowsCount(this.availablityGroupsListRowsCount);
			PreferencesUtil.getInstance().setDashboardPreferencesInSession(null, null, null, null, -1, -1, null, -1,-1,-1,-1,-1,pageSize);
			prevPageSize = pageSize;
		} catch (Exception ex) {
			setListRowsCount(prevPageSize);
		}
	}
	public int getAvailablityGroupsListRowsCount() {
		return availablityGroupsListRowsCount;
	}

	public void setAvailablityGroupsListRowsCount(int availablityGroupsListRowsCount) {
		this.availablityGroupsListRowsCount = availablityGroupsListRowsCount;
	}

	public int getListRowsCount() {
		return listRowsCount;
	}

	public List<AvailabilityGroupDetails> getAvailabilityGroups() {
		return availabilityGroups;
	}

	public void setAvailabilityGroups(
			List<AvailabilityGroupDetails> availabilityGroups) {
		this.availabilityGroups = availabilityGroups;
	}
	private void setEmptyCharts() {
		queueSize.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE_SENTENCE_CASE));
		transferRate.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE_SENTENCE_CASE));
	}
	
	@NotifyChange("availabilityGroups")
	private void updateAvailabiltyGroupViewModel(/*List<AvailabilityGroupDetails> availabilityGroups*/) {
		BindUtils.postNotifyChange(null, null, this, "availabilityGroups");
	}
}
