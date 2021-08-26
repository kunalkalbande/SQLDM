package com.idera.sqldm.ui.dashboard.instances.resources;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.HashSet;
import java.util.List;
import java.util.Set;
import java.util.concurrent.TimeUnit;

import org.apache.commons.lang.math.NumberUtils;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Grid;
import org.zkoss.zul.Paging;

import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.ResourcesFacade;
import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.data.category.resources.ResourceCategory;
import com.idera.sqldm.data.category.resources.ServerWait;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.ui.components.charts.line.IderaLineChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean;
import com.idera.sqldm.ui.preferences.HistoryPanelPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.utils.Utility;

public class ServerWaitsViewModel {
	
	enum WaitCategoryType{
		all("all"), backup("Backup"), io("I/O"), lock("Lock"), memory("Memory"), noniopage("Non-I/O Page Latch"), 
		nonpage("Non-Page Latch"), other("Other"), transactionLog("Transaction Log");
		
		private String name;
		private WaitCategoryType(String name){
			this.name = name;
		}
		
		public String getName() {
			return name;
		}

		public void setName(String name) {
			this.name = name;
		}
	}
	private final String DEFAULT_WAIT_TYPE = ELFunctions.getLabel("Labels.serverwaits.totalwaits");
	@Wire IderaLineChart serverWaitsChart1;

	private int instanceId = -1;
	private int listRowsCount, fileActivityListRowsCount;
	private int prevPageSize;
	private String waitTypelabel = DEFAULT_WAIT_TYPE;
	
	private static int DEFAULT_ROWS_COUNT = 10;
	private List<ServerWaitModel> waitItems;
	private long OFFSET_MILLIS = 0;
	@Wire Paging fileActivityListPgId;
	@Wire Grid serverWaitGrid;

	List<ServerWait> serverWaitList;
	private boolean historyEvent = false;
	
	@Init
	public void init(){
		OFFSET_MILLIS = Utility.cancelOffSetInMillis();
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
		if (dpb != null && dpb.getResoucesSWGridPageCount() != -1) {
			defaultAlertsRowCount = dpb.getResoucesSWGridPageCount();
		}
		prevPageSize = defaultAlertsRowCount;
		setListRowsCount(prevPageSize);
		fileActivityListRowsCount = prevPageSize;
		
	}
	
	@AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view){
        Selectors.wireComponents(view, this, false);
        int defaultAlertsRowCount = DEFAULT_ROWS_COUNT;
		DashboardPreferencesBean dpb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();
		if (dpb != null && dpb.getResoucesSWGridPageCount() != -1) {
			defaultAlertsRowCount = dpb.getResoucesSWGridPageCount();
		}
		prevPageSize = defaultAlertsRowCount;
		setListRowsCount(prevPageSize);
		fileActivityListRowsCount = prevPageSize;
        refreshServerWaitsModel();
        serverWaitGrid.setPaginal(fileActivityListPgId);
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
    		refreshServerWaitsModel();
    	}
	}

	protected void refreshGraphs() {
    	historyEvent = true;
	}
	
	private void refreshServerWaitsModel(){
		serverWaitsChart1.refresh();
		populateServerWaitsData();
		refreshModels();
	}
	
	private void refreshModels(){
		waitItems = new ArrayList<>();
		IderaLineChartModel chartModel = new IderaLineChartModel();
		
		Set<String> keys = new HashSet<>();
		for(ServerWait serverWait : serverWaitList){
			Double waitTime = getWaitTime(serverWait);
			//Adding offset for graphs
			long collectionTime = serverWait.getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
			chartModel.setValue(serverWait.getCategory(), collectionTime, waitTime);

			// table model
			String key = serverWait.getCategory()+serverWait.getWaitType();
			if(!keys.contains(key)){
				keys.add(key);
				Double totalWaitTime = getTotalWaitTime(serverWait);
				ServerWaitModel serverModel = new ServerWaitModel(serverWait.getCategory(), serverWait.getWaitType());
				serverModel.setWaitingTasks(serverWait.getWaitingTasks());
				serverModel.setWaitingTime(waitTime);
				serverModel.setHelp(serverWait.getHelp());
				serverModel.setDescription(serverWait.getDescription());
				serverModel.setTotalWait(totalWaitTime);
				waitItems.add(serverModel);
			}
		}
		Collections.sort(waitItems);
		serverWaitsChart1.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.ALL_WAIT_TYPES_IN_MS));
		if(serverWaitList.size()>1){
			serverWaitsChart1.setModel(chartModel);
			
			/*
			 * Author:Accolite
			 * Date : 11th Nov, 2016
			 * Drill  down Capability - SQLDM- 10.2 release
			 */
			serverWaitsChart1.getChart().setDrillable(true);
			
		}else{
			setEmptyCharts();
		}
		
		
	}
	
	private Double getWaitTime(ServerWait serverWait){
		if(serverWait.getStatistics() == null) {
			return 0d;
		}
		if(ELFunctions.getLabel("Labels.serverwaits.resourcewaits").equalsIgnoreCase(waitTypelabel)) {
			return serverWait.getStatistics().getResourceWaitInMils() == null ? 0 : Utility.round(NumberUtils.toDouble(serverWait.getStatistics().getResourceWaitInMils().getWait()), 2);
		} else if(ELFunctions.getLabel("Labels.serverwaits.signalwaits").equalsIgnoreCase(waitTypelabel)){
			return serverWait.getStatistics().getSignalWaitInMils() == null ? 0 : Utility.round(NumberUtils.toDouble(serverWait.getStatistics().getSignalWaitInMils().getWait()), 2);
		} else {
			return serverWait.getStatistics().getTotalWaitInMils() == null ? 0 : Utility.round(NumberUtils.toDouble(serverWait.getStatistics().getTotalWaitInMils().getWait()), 2);
		}
	}
	
	private Double getTotalWaitTime(ServerWait serverWait){
		if(serverWait.getStatistics() == null) {
			return 0d;
		}
		if(ELFunctions.getLabel("Labels.serverwaits.resourcewaits").equalsIgnoreCase(waitTypelabel)) {
			return serverWait.getStatistics().getResourceWaitInMils() == null ? 0 : NumberUtils.toDouble(serverWait.getStatistics().getResourceWaitInMils().getTotalWait());
		} else if(ELFunctions.getLabel("Labels.serverwaits.signalwaits").equalsIgnoreCase(waitTypelabel)){
			return serverWait.getStatistics().getSignalWaitInMils() == null ? 0 : NumberUtils.toDouble(serverWait.getStatistics().getSignalWaitInMils().getTotalWait());
		} else {
			return serverWait.getStatistics().getTotalWaitInMils() == null ? 0 : NumberUtils.toDouble(serverWait.getStatistics().getTotalWaitInMils().getTotalWait());
		}
	}
	
	private void populateServerWaitsData() {
		serverWaitList = new ArrayList<>();
		try {
			String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");

			//List<ServerWait> items = ResourcesFacade.getServerWaits(productInstanceName,instanceId, 30);
			HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
			List<ServerWait> items = null;
			if(pref.getScaleCombobox().getValue().equals("Custom")){
				Date startTime = pref.getFromDateTime();
				Date endTime = pref.getToDateTime();
				long duration  = endTime.getTime() - startTime.getTime();
				long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
				items = ResourcesFacade.getServerWaits(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
	        }
	        else{
	        	items = ResourcesFacade.getServerWaits(productInstanceName,instanceId, pref.getNumHistoryMinutes());
	        }
			if(items !=null && items.size()>0){
				serverWaitList.addAll(items);
			}else{
				setEmptyCharts();
			}
			Collections.sort(serverWaitList, new Comparator<ServerWait>() {
				@Override
				public int compare(ServerWait o1, ServerWait o2) {
					return o2.getUTCCollectionDateTime().compareTo(o1.getUTCCollectionDateTime());
				}
			});
		} catch (CategoryException e) {
			e.printStackTrace();
		}
	}
	
	private void setEmptyCharts() {
		serverWaitsChart1.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
	}
	
	@NotifyChange("listRowsCount")
	public void setListRowsCount(int listRowsCount) {
		this.listRowsCount = listRowsCount;
		BindUtils.postNotifyChange(null, null, this, "listRowsCount");
	}
	
	@Command("setFileActivityRowsCount")
	public void setFileActivityRowsCount() {
		try {
			int pageSize = this.fileActivityListRowsCount;
			setListRowsCount(this.fileActivityListRowsCount);
			PreferencesUtil.getInstance().setDashboardPreferencesInSession(null, null, null, null, -1, -1,null,-1,-1,-1,pageSize,-1,-1);
			prevPageSize = pageSize;
		} catch (Exception ex) {
			setListRowsCount(prevPageSize);
		}
	}

	
	@Command
	@NotifyChange({"waitTypelabel","waitItems"})
	public void showWaitType(){
		refreshModels();
		/*switch(waitType){
		case "total": waitTypelabel = ELFunctions.getLabel("Labels.serverwaits.totalwaits");refreshModels();break;
		case "signal" : waitTypelabel = ELFunctions.getLabel("Labels.serverwaits.signalwaits");refreshModels();break;
		case "resource" : waitTypelabel = ELFunctions.getLabel("Labels.serverwaits.resourcewaits");refreshModels();break;
		}*/
	}
	
	public String getWaitTypelabel() {
		return waitTypelabel;
	}

	public void setWaitTypelabel(String waitTypelabel) {
		this.waitTypelabel = waitTypelabel;
	}

	public List<ServerWaitModel> getWaitItems() {
		return waitItems;
	}

	public void setWaitItems(List<ServerWaitModel> waitItems) {
		this.waitItems = waitItems;
	}
	public int getListRowsCount() {
		return listRowsCount;
	}

	public int getFileActivityListRowsCount() {
		return fileActivityListRowsCount;
	}
	public void setFileActivityListRowsCount(int fileActivityListRowsCount) {
		this.fileActivityListRowsCount = fileActivityListRowsCount;
	}
	
	public List<String> getServerWaitsOptions() {
  		List<String> serverWaitsOptions = new ArrayList<>();
  		serverWaitsOptions.add(Utility.getMessage("Labels.serverwaits.totalwaits"));
  		serverWaitsOptions.add(Utility.getMessage("Labels.serverwaits.signalwaits"));
  		serverWaitsOptions.add(Utility.getMessage("Labels.serverwaits.resourcewaits"));
		return serverWaitsOptions; 
  	}
/*	private void setOffset() {
		if(Sessions.getCurrent()!=null)
		{
			OFFSET_MILLIS = new Long((Integer)Sessions.getCurrent().getAttribute(WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET));
			OFFSET_MILLIS = -OFFSET_MILLIS;
		}
	}*/
}
