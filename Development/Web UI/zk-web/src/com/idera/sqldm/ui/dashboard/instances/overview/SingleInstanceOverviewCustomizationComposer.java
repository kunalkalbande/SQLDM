package com.idera.sqldm.ui.dashboard.instances.overview;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.concurrent.TimeUnit;

import org.apache.commons.lang.math.NumberUtils;
import org.apache.log4j.Logger;
import org.zkoss.addon.columnchooser.ColumnVisibilityChangeEvent;
import org.zkoss.addon.columnchooser.Columnchooser;
import org.zkoss.bind.BindUtils;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Button;
import org.zkoss.zul.CategoryModel;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.Constraint;
import org.zkoss.zul.Div;
import org.zkoss.zul.Grid;
import org.zkoss.zul.Hlayout;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Popup;
import org.zkoss.zul.Row;
import org.zkoss.zul.Rows;
import org.zkoss.zul.Space;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Vlayout;

import com.idera.common.rest.RestException;
import com.idera.sqldm.data.DashboardInstance;
import com.idera.sqldm.data.DashboardInstanceFacade;
import com.idera.sqldm.data.InstanceDetailsSessionFacade;
import com.idera.sqldm.data.InstanceException;
import com.idera.sqldm.data.ResourcesFacade;
import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.data.category.resources.CPUStatDetails;
import com.idera.sqldm.data.category.resources.CustomCounterStats;
import com.idera.sqldm.data.category.resources.DiskIOPerSecondEntry;
import com.idera.sqldm.data.category.resources.FileStats;
import com.idera.sqldm.data.category.resources.GraphBaseline;
import com.idera.sqldm.data.category.resources.LockWaitData;
import com.idera.sqldm.data.category.resources.NetworkStats;
import com.idera.sqldm.data.category.resources.OSPagingData;
import com.idera.sqldm.data.category.resources.ResourceCategory;
import com.idera.sqldm.data.category.resources.ServerWait;
import com.idera.sqldm.data.databases.InstanceDetailsDatabaseFacade;
import com.idera.sqldm.data.databases.TempDBUsageDetails;
import com.idera.sqldm.data.instances.DatabaseStatsDetails;
import com.idera.sqldm.data.instances.GraphCategories;
import com.idera.sqldm.data.instances.GraphCategories.GraphCategoriesEnum;
import com.idera.sqldm.data.instances.GraphCategoriesServices;
import com.idera.sqldm.data.instances.SessionGraphDetail;
import com.idera.sqldm.data.instances.VirtualizationStats;
import com.idera.sqldm.data.instances.VirtualizationStatsDetails;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.server.web.WebUtil;
import com.idera.sqldm.ui.components.charts.IderaChart;
import com.idera.sqldm.ui.components.charts.area.IderaAreaChart;
import com.idera.sqldm.ui.components.charts.area.IderaAreaChartModel;
import com.idera.sqldm.ui.components.charts.line.IderaLineChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.ui.dashboard.instances.InstanceCategoryTab;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm.ui.dashboard.instances.overview.DBGraphOptions.DBGraphOptionEnum;
import com.idera.sqldm.ui.dashboard.instances.overview.VirtualizationGraphOption.VirtualizationGraphOptionEnum;
import com.idera.sqldm.ui.preferences.HistoryPanelPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstanceAlertsPreferencesBean;
import com.idera.sqldm.ui.preferences.SingleInstanceOverviewPreferenceBean;
import com.idera.sqldm.ui.preferences.SingleInstancePreferencesBean;
import com.idera.sqldm.utils.SQLdmConstants;
import com.idera.sqldm.utils.Utility;

@SuppressWarnings("rawtypes")
public class SingleInstanceOverviewCustomizationComposer extends SelectorComposer<Div> {
	static final long serialVersionUID = 1L;
	//private String OFFSET_IN_HOURS = "0.0";
	private final static Logger log = Logger.getLogger(SingleInstanceOverviewComposer.class);

	DashboardInstance instance;
	int instanceId;
	//private Double offsetHours;
	private long OFFSET_MILLIS = 0;
	private ListModelList<GraphCategories> graphCategoryOptions ;//= new ListModelList<GraphCategories>(GraphCategories.getDefaultList());;

	private Date fromDate,endDate,fromTime, endTime ;

	private GraphCategoriesServices graphCatServices = new GraphCategoriesServices();
	@Wire("#manageGraphPopup")	
	protected Popup manageGraphPopup;

	@Wire 
	protected Button applyButton, cancelButton, manageGraphButton;

	@Wire
	protected Vlayout categoryOptionsLayout;


	@Wire("#customChartsContainerDiv")
	protected Div customChartsContainerDiv;

	@Wire("#customChartsVLayout")
	protected Div customChartsVLayout;

	@Wire
	protected Grid categoryList;
	private String productInstanceName;

	private HashMap<String, CategoryModel> modelsMap;
	private String selectedDisk;

	/*Graphs*/
	//CPU
	private IderaLineChart sqlCpuUsage, cpuCallRateStats;

	//Memory
	private IderaLineChart sqlMemoryUsage, sqlOSPaging;
	
	//Cache
	private IderaLineChart memoryAreas;
	private IderaAreaChart pageLifeExpectancy;
	
	//Disk
	private IderaLineChart sqlPhysicalIO, diskTotalTime, diskAvgTime;
	private HashMap<String, IderaLineChartModel> diskVsTotalTimeModel, diskVsAvgTimeModel; 
	
	//Lock waits
	private IderaLineChart lockWaits;
	
	//sessions
	private List<SessionGraphDetail> sessionDetails;
	private  IderaAreaChart blockedSessionChart;
	
	//server wait
	private List<ServerWait> serverWaitList;
	private IderaLineChart serverWaitsChart1;
	private final String DEFAULT_WAIT_TYPE = ELFunctions.getLabel("Labels.serverwaits.totalwaits");
	private String waitTypelabel = DEFAULT_WAIT_TYPE;
	//tempDB
	private IderaLineChart tempdbUsedByTime, tempDbPagesWaitTime;
	
	//custom counters
	private IderaLineChart customCountersStat;
	
	//network counters
	private IderaLineChart networkStats;
	
	//file activity
	private IderaLineChart fileStats;

	//Database
	private IderaLineChart databaseCharts;
	protected String selectDBGraph;	

	//Virtualization 
	private IderaLineChart virtualizationMemChart,virtualizationReadWriteChart;
	protected String selectVirtualGraph;
	VirtualizationStatsDetails virtualizationStatData;
	
	@Wire("#columnchooser")
	private Columnchooser columnchooser;
	private String selectedFilter;
	private Combobox sb1;
	
	private SingleInstanceOverviewPreferenceBean overPrefBean;
	private ArrayList<String> selectedFileComponent;


	@Override
	public void doAfterCompose(Div comp) throws Exception { 
		super.doAfterCompose(comp);
		//setOffSet();
		modelsMap = new HashMap<String, CategoryModel>();
		OFFSET_MILLIS = Utility.cancelOffSetInMillis();
		Integer instanceIdParameter = Utility.getIntUrlParameter(Executions.getCurrent().getParameterMap(), "id");
		productInstanceName = Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
		if (instanceIdParameter != null) {
			instanceId = instanceIdParameter;
		} else {
			// fallback
			Object param = Executions.getCurrent().getDesktop().getAttribute("instanceId");
			if (param != null) {

				instanceId = (Integer) param;
			}
		}
		Executions.getCurrent().getDesktop().setAttribute("instanceId", instanceId);

		try {
			instance = DashboardInstanceFacade.getDashboardInstance(productInstanceName, instanceId);
		} catch (InstanceException x) {
			log.error(x.getMessage(), x);
		} catch (RestException e) {
			e.printStackTrace();
		}

		if (instance == null || instance.getOverview() == null) {
			Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("dashboard"));
			return;
		}

		overPrefBean = PreferencesUtil.getInstance().getOverviewPreferenceInSession(instanceId);
		
		HistoryPanelPreferencesBean prefHistory = PreferencesUtil.getInstance()
				.getHistoryPanelPreferencesInSession();
		
		if(overPrefBean.getCustomizedGraphSettings()==null){
			graphCategoryOptions =  new ListModelList<GraphCategories>(graphCatServices.getDefaultList());
			overPrefBean.setCustomizedGraphSettings(graphCategoryOptions);
			PreferencesUtil.getInstance().setOverviewPreferenceInSession(overPrefBean);
		}else {
			graphCategoryOptions = overPrefBean.getCustomizedGraphSettings();
		}
		categoryList.setModel(graphCategoryOptions);
		
		
		EventQueue<Event> eq1 = EventQueues.lookup("historyChange1", EventQueues.DESKTOP, true);
		eq1.subscribe(new EventListener<Event>() {

			@Override
			public void onEvent(Event event) throws Exception {
				refreshOnEventChange();
			}
		});

		if (prefHistory.getFromDate() != null) {
			setHistoryPanelValues(prefHistory);
		} else {
			//	updateViewUsingSession();
			//	refreshView(true);
		}
		refreshView();
	}
	
	public void launchToSessions() throws IOException, RestException{
		SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
		pref.setSelectedCategory(InstanceCategoryTab.SESSIONS.getId());
		pref.setSelectedSubCategory(2);
        PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);

        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance"+"/"+instanceId));
	}

	@SuppressWarnings("unchecked")
	private void refreshModels() throws Exception{
		try{
			categoryList.setModel(graphCategoryOptions);
			//flag to stop repeated calling of resource model API
			boolean resourceModelAPI = false;
			for(int i=0;i<graphCategoryOptions.size();i++){
				GraphCategories gc = graphCategoryOptions.get(i);

				if(gc.getCategory().getCategoryName().equalsIgnoreCase(GraphCategoriesEnum.SESSIONS.getCategoryName()) && gc.isVisible()){
					getSessionsModelData();
				}
				else if(gc.getCategory().getCategoryName().equalsIgnoreCase(GraphCategoriesEnum.SERVER_WAITS.getCategoryName()) && gc.isVisible()){
					getServerWaitModelData();
				}
				else if((gc.getCategory().getCategoryName().equalsIgnoreCase(GraphCategoriesEnum.CPU.getCategoryName())
						||gc.getCategory().getCategoryName().equalsIgnoreCase(GraphCategoriesEnum.MEMORY.getCategoryName())
						||gc.getCategory().getCategoryName().equalsIgnoreCase(GraphCategoriesEnum.CACHE.getCategoryName())
						||gc.getCategory().getCategoryName().equalsIgnoreCase(GraphCategoriesEnum.DISK.getCategoryName()) 
						)
						&& gc.isVisible() && !resourceModelAPI){
					getResourceDataModel();
					resourceModelAPI=true;
				}
				else if(gc.getCategory().getCategoryName().equalsIgnoreCase(GraphCategoriesEnum.LOCK_WAITS.getCategoryName()) && gc.isVisible()){
					getLockWaitsModelData();
				}
				else if(gc.getCategory().getCategoryName().equalsIgnoreCase(GraphCategoriesEnum.TEMPDB.getCategoryName()) && gc.isVisible()){
					getTempDBDataModel();
				}
				else if(gc.getCategory().getCategoryName().equalsIgnoreCase(GraphCategoriesEnum.DATABASES.getCategoryName()) && gc.isVisible()){
					getDBDataModel();
				}
				else if(gc.getCategory().getCategoryName().equalsIgnoreCase(GraphCategoriesEnum.CUSTOM_COUNTERS.getCategoryName()) && gc.isVisible()){
					getCustomCounterStats();
				}
				else if(gc.getCategory().getCategoryName().equalsIgnoreCase(GraphCategoriesEnum.NETWORK.getCategoryName()) && gc.isVisible()){
					getNetworkDataModel();
				}
				else if(gc.getCategory().getCategoryName().equalsIgnoreCase(GraphCategoriesEnum.FILE_ACTIVITY.getCategoryName()) && gc.isVisible()){
					getFileStatsDataModel();
				}
				else if(gc.getCategory().getCategoryName().equalsIgnoreCase(GraphCategoriesEnum.VIRTUALIZATION.getCategoryName()) && gc.isVisible()){
					getVirtualizationDataModel();
				}
				
				//Additional api calls for few categories, hence not in else part
				if(gc.getCategory().getCategoryName().equalsIgnoreCase(GraphCategoriesEnum.CPU.getCategoryName()) && gc.isVisible()){
					getCPUCallRatesDataModel();
				}
				if(gc.getCategory().getCategoryName().equalsIgnoreCase(GraphCategoriesEnum.MEMORY.getCategoryName()) && gc.isVisible()){
					getOSPagingModelData();
				}
				
			}
		}
		catch (CategoryException e) {
			e.printStackTrace();
		}
	}
	/**
	 *Author:Accolite
	 * Date : 21th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release 
	 */
	private void getVirtualizationDataModel(){
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		
		try {
			if(pref.getScaleCombobox().getValue().equals("Custom")){
				Date startTime = pref.getFromDateTime();
				Date endTime = pref.getToDateTime();
				long duration  = endTime.getTime() - startTime.getTime();
				long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
				virtualizationStatData = ResourcesFacade.getVirtualizationStatDetails(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
			}
			else{
				virtualizationStatData = ResourcesFacade.getVirtualizationStatDetails(productInstanceName,instanceId, pref.getNumHistoryMinutes());
			}
			
			if(virtualizationStatData !=null){
				List<VirtualizationStats> vsList = virtualizationStatData.getVirtualizationStats();
				if(vsList!=null && vsList.size()>1){
					IderaLineChartModel VMAvailableMemoryModel = new IderaLineChartModel();
					IderaLineChartModel hostAvailableMemoryModel = new IderaLineChartModel();
					IderaLineChartModel VMActiveBaloonedConsumeGrantedModel = new IderaLineChartModel();
					IderaLineChartModel hostActiveBaloonedConsumeGrantedModel = new IderaLineChartModel();
					IderaLineChartModel VMreadWriteModel = new IderaLineChartModel();
					IderaLineChartModel hostreadWriteModel = new IderaLineChartModel();
					for(int i=vsList.size()-1;i>0;i--){
						VirtualizationStats vs = vsList.get(i);
						long collectionTime = vs.getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
						
						//if type is hyperV only Available memory is shown
						if(virtualizationStatData.getType().equalsIgnoreCase("HyperV")){
							VMAvailableMemoryModel.setValue("VM Available Memory", collectionTime, vs.getVMAvailableByte());
							hostAvailableMemoryModel.setValue("Host Available Memory", collectionTime, vs.getESXAvailableMemBytes());
						}else{
							//VM details
							VMActiveBaloonedConsumeGrantedModel.setValue("Active", collectionTime, vs.getVMMemActiveMB());
							VMActiveBaloonedConsumeGrantedModel.setValue("Ballooned", collectionTime, vs.getVMMemBaloonedMB());
							VMActiveBaloonedConsumeGrantedModel.setValue("Consumed", collectionTime, vs.getVMMemConsumedMB());
							VMActiveBaloonedConsumeGrantedModel.setValue("Granted", collectionTime, vs.getVMMemGrantedMB());
							//ESX is equal to host
							hostActiveBaloonedConsumeGrantedModel.setValue("Active", collectionTime, vs.getESXMemActiveMB());
							hostActiveBaloonedConsumeGrantedModel.setValue("Ballooned", collectionTime, vs.getESXMemBaloonedMB());
							hostActiveBaloonedConsumeGrantedModel.setValue("Consumed", collectionTime, vs.getESXMemConsumedMB());
							hostActiveBaloonedConsumeGrantedModel.setValue("Granted", collectionTime, vs.getESXMemGrantedMB());
						}
						VMreadWriteModel.setValue("Disk read", collectionTime, vs.getVMDiskRead());
						VMreadWriteModel.setValue("Disk Write", collectionTime, vs.getVMDiskWrite());
						hostreadWriteModel.setValue("Disk Read", collectionTime, vs.getESXDiskRead());
						hostreadWriteModel.setValue("Disk Write", collectionTime, vs.getESXDiskWrite());
					}
					
					modelsMap.put(CustomizationConstants.VM_AVAILABLE_MEMORY_MODEL , VMAvailableMemoryModel);
					modelsMap.put(CustomizationConstants.HOST_AVAILABLE_MEMORY_MODEL , hostAvailableMemoryModel);
					modelsMap.put(CustomizationConstants.VM_ACTIVE_BALOONED_CONSUME_GRANTED_MODEL , VMActiveBaloonedConsumeGrantedModel);
					modelsMap.put(CustomizationConstants.HOST_ACTIVE_BALOONED_CONSUME_GRANTED_MODEL , hostActiveBaloonedConsumeGrantedModel);
					modelsMap.put(CustomizationConstants.VM_READ_WRITE_MODEL , VMreadWriteModel);
					modelsMap.put(CustomizationConstants.HOST_READ_WRITE_MODEL , hostreadWriteModel);
					
					
				}
				else
					setEmptyVirtualiationModel();
			}
			else{
				setEmptyVirtualiationModel();
			}
		}
		catch(CategoryException e){
			log.error(e.getMessage());
		}
	}
	
	private void setEmptyVirtualiationModel() {
		modelsMap.put(CustomizationConstants.VM_AVAILABLE_MEMORY_MODEL , null);
		modelsMap.put(CustomizationConstants.HOST_AVAILABLE_MEMORY_MODEL , null);
		modelsMap.put(CustomizationConstants.VM_ACTIVE_BALOONED_CONSUME_GRANTED_MODEL , null);
		modelsMap.put(CustomizationConstants.HOST_ACTIVE_BALOONED_CONSUME_GRANTED_MODEL , null);
		modelsMap.put(CustomizationConstants.VM_READ_WRITE_MODEL , null);
		modelsMap.put(CustomizationConstants.HOST_READ_WRITE_MODEL , null);
	}

	private void getDBDataModel() {
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
			List<DatabaseStatsDetails> dbStatDataList;
			try {
				if(pref.getScaleCombobox().getValue().equals("Custom")){
					Date startTime = pref.getFromDateTime();
					Date endTime = pref.getToDateTime();
					long duration  = endTime.getTime() - startTime.getTime();
					long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
					dbStatDataList = ResourcesFacade.getDBStatDetails(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
				}
				else{
					dbStatDataList = ResourcesFacade.getDBStatDetails(productInstanceName,instanceId, pref.getNumHistoryMinutes());
				}

			if(dbStatDataList !=null){

				if(dbStatDataList.size() >1){ //We need at least 2 point to plot a graph
					Executions.getCurrent().getDesktop().setAttribute(CustomizationConstants.DB_STAT_DATA_LIST,dbStatDataList);
					
					DBStatFilterDataComposer.createFilterMapData(instanceId);

				}else{
					Executions.getCurrent().getDesktop().setAttribute(CustomizationConstants.DB_STAT_MODEL_NAME,null);
				}
			} else{
				Executions.getCurrent().getDesktop().setAttribute(CustomizationConstants.DB_STAT_MODEL_NAME,null);
			}
		}
		catch(CategoryException e){
			
		}
	}
	
	/*private void getDBDataModel() {

		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		List<DatabaseStatsDetails> dbStatDataList;
		try {
			if(pref.getScaleCombobox().getValue().equals("Custom")){
				Date startTime = pref.getFromDateTime();
				Date endTime = pref.getToDateTime();
				long duration  = endTime.getTime() - startTime.getTime();
				long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
				dbStatDataList = ResourcesFacade.getDBStatDetails(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
			}
			else{
				dbStatDataList = ResourcesFacade.getDBStatDetails(productInstanceName,instanceId, pref.getNumHistoryMinutes());
			}

			if(dbStatDataList.size()!=0){

//				long startTimeUTC  = dbStatDataList.get(0).getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
	//			long endTimeUTC  = dbStatDataList.get(dbStatDataList.size()-1).getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
				if(dbStatDataList !=null && dbStatDataList.size() >1){ //We need at least 2 point to plot a graph
					//CPU Call Rate
					IderaLineChartModel transDBModel = new IderaLineChartModel();
					IderaLineChartModel LogFlushesDBModel = new IderaLineChartModel();
					IderaLineChartModel readsDBModel = new IderaLineChartModel();
					IderaLineChartModel writesDBModel = new IderaLineChartModel();
					IderaLineChartModel IOStallDBModel = new IderaLineChartModel();

					for(int i=dbStatDataList.size()-1; i>=0; i--){
						DatabaseStatsDetails dBCurrDetails = dbStatDataList.get(i);
						//Adding Offset for graphs
						long collectionTime = dBCurrDetails.getUTCCollectionDateTime().getTime()+ OFFSET_MILLIS;
						transDBModel.setValue(dBCurrDetails.getDatabaseName(), collectionTime, dBCurrDetails.getTransactionsPerSec());
						LogFlushesDBModel.setValue(dBCurrDetails.getDatabaseName(), collectionTime, dBCurrDetails.getLogFlushesPerSec());
						readsDBModel.setValue(dBCurrDetails.getDatabaseName(), collectionTime, dBCurrDetails.getReadsPerSec());
						writesDBModel.setValue(dBCurrDetails.getDatabaseName(), collectionTime, dBCurrDetails.getWritesPerSec());
						IOStallDBModel.setValue(dBCurrDetails.getDatabaseName(), collectionTime, dBCurrDetails.getIOStallPerSec());
					}
					modelsMap.put(CustomizationConstants.TRANS_DB_MODEL , transDBModel);
					modelsMap.put(CustomizationConstants.LOG_FLUSHES_MODEL, LogFlushesDBModel);
					modelsMap.put(CustomizationConstants.READS_DB_MODEL , readsDBModel);
					modelsMap.put(CustomizationConstants.WRITES_DB_MODEL , writesDBModel);
					modelsMap.put(CustomizationConstants.IO_STALL_DB_MODEL , IOStallDBModel);

				}else{
					databaseCharts.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
				}
			}
			else{
				modelsMap.put(CustomizationConstants.TRANS_DB_MODEL , null);
				modelsMap.put(CustomizationConstants.LOG_FLUSHES_MODEL, null);
				modelsMap.put(CustomizationConstants.READS_DB_MODEL , null);
				modelsMap.put(CustomizationConstants.WRITES_DB_MODEL , null);
				modelsMap.put(CustomizationConstants.IO_STALL_DB_MODEL , null);
			}
		}
		catch(CategoryException e){
			log.error(e.getMessage());
		}

	}
*/	
	private void getFileStatsDataModel() {
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		List<FileStats> fileStats;
		try {
			if(pref.getScaleCombobox().getValue().equals("Custom")){
				Date startTime = pref.getFromDateTime();
				Date endTime = pref.getToDateTime();
				long duration  = endTime.getTime() - startTime.getTime();
				long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
				fileStats = ResourcesFacade.getFileStats(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
			}
			else{
				fileStats = ResourcesFacade.getFileStats(productInstanceName,instanceId, pref.getNumHistoryMinutes());
			}

			if(fileStats !=null){

				if(fileStats.size() >1){ //We need at least 2 point to plot a graph
					Executions.getCurrent().getDesktop().setAttribute("fileStats",fileStats);
					SingleInstanceOverviewPreferenceBean overviewPref = PreferencesUtil.getInstance().getOverviewPreferenceInSession(instanceId);
					List<String> selectedComponents = overviewPref.getSelectedFileComponent();
					selectedFileComponent = new ArrayList<String>();
					if(selectedComponents == null){
						selectedFileComponent.add("AllFiles");
						selectedFileComponent.add("");
					}
					else{
						for(int i = 0; i < selectedComponents.size();i++){
							selectedFileComponent.add(selectedComponents.get(i));
						}
					}
					overviewPref.setSelectedFileComponent(selectedFileComponent);
					PreferencesUtil.getInstance().setOverviewPreferenceInSession(overviewPref);
					FileFilterDataComposer.createFilterMapData(instanceId);

				}else{
					Executions.getCurrent().getDesktop().setAttribute("fileStatsLineChartModels",null);
				}
			} else{
				Executions.getCurrent().getDesktop().setAttribute("fileStatsLineChartModels",null);
			}
		}
		catch(CategoryException e){
			
		}
	}

	private void getNetworkDataModel() {
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		List<NetworkStats> networkStats;
		try {
			if(pref.getScaleCombobox().getValue().equals("Custom")){
				Date startTime = pref.getFromDateTime();
				Date endTime = pref.getToDateTime();
				long duration  = endTime.getTime() - startTime.getTime();
				long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
				networkStats = ResourcesFacade.getNetworkStats(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
			}
			else{
				networkStats = ResourcesFacade.getNetworkStats(productInstanceName,instanceId, pref.getNumHistoryMinutes());
			}

			if(networkStats.size()!=0){

				if(networkStats !=null && networkStats.size() >1){ //We need at least 2 point to plot a graph
					IderaLineChartModel networkDataModel = new IderaLineChartModel();
					for(int i=networkStats.size()-1; i>=0; i--){
						NetworkStats dataObj = networkStats.get(i);
						//Adding Offset for graphs
						long collectionTime = dataObj.getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
						networkDataModel.setValue("Packets Sent", collectionTime, dataObj.getPacketsSentPerSec());
						networkDataModel.setValue("Packets Received", collectionTime, dataObj.getPacketsRecievedPerSec());
					}
					modelsMap.put(CustomizationConstants.NETWORK_MODEL , networkDataModel);

				}
			}else{
				modelsMap.put(CustomizationConstants.NETWORK_MODEL , null);
			}
		}
		catch(CategoryException e){
		}
	}

	private void getCustomCounterStats() {
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		List<CustomCounterStats> customCounterStats;
		try {
			if(pref.getScaleCombobox().getValue().equals("Custom")){
				Date startTime = pref.getFromDateTime();
				Date endTime = pref.getToDateTime();
				long duration  = endTime.getTime() - startTime.getTime();
				long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
				customCounterStats = ResourcesFacade.getCustomCounterStats(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
			}
			else{
				customCounterStats = ResourcesFacade.getCustomCounterStats(productInstanceName,instanceId, pref.getNumHistoryMinutes());
			}

			if(customCounterStats.size()!=0){

				if(customCounterStats !=null && customCounterStats.size() >1){ //We need at least 2 point to plot a graph
					IderaLineChartModel customCounterStatsModel = new IderaLineChartModel();
					for(int i=customCounterStats.size()-1; i>=0; i--){
						CustomCounterStats dataObj = customCounterStats.get(i);
						//Adding Offset for graphs
						long collectionTime = dataObj.getUTCCollectionDateTime().getTime() + OFFSET_MILLIS;
						customCounterStatsModel.setValue(dataObj.getName(), collectionTime, dataObj.getValue());
					}
					modelsMap.put(CustomizationConstants.CUSTOM_COUNTERS_MODEL , customCounterStatsModel);
				}
			}
			else{
				modelsMap.put(CustomizationConstants.CUSTOM_COUNTERS_MODEL , null);
			}
		}
		catch(CategoryException e){
		}
	}

	private void getServerWaitModelData() {
		serverWaitList = new ArrayList<>();
		serverWaitsChart1 = new IderaLineChart();
		try {
			String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");

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
				serverWaitList = new ArrayList<ServerWait>();
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


	private void getSessionsModelData() {
		try {
			String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
			HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
			if(pref.getScaleCombobox().getValue().equals("Custom")){
				Date startTime = pref.getFromDateTime();
				Date endTime = pref.getToDateTime();
				long duration  = endTime.getTime() - startTime.getTime();
				long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
				Executions.getCurrent().getDesktop().setAttribute(
						SQLdmConstants.DASHBOARD_SCOPE_SINGLE_INSTANCE_SESSION_CHART, 
						InstanceDetailsSessionFacade.getSessionChartDetail(productInstanceName,instanceId,numHistoryMinutes,pref.getToDateTime()));
			}
			else{
				Executions.getCurrent().getDesktop().setAttribute(
						SQLdmConstants.DASHBOARD_SCOPE_SINGLE_INSTANCE_SESSION_CHART, 
						InstanceDetailsSessionFacade.getSessionChartDetail(productInstanceName,instanceId,pref.getNumHistoryMinutes()));
			}
	    } catch (Exception ce) {
        	Executions.getCurrent().getDesktop().removeAttribute("resources");
        	log.error(ce);
	    }
	}


	private void getCPUCallRatesDataModel() throws CategoryException {
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		List<CPUStatDetails> cpuStatsDataList;
		Date startTime, endTime;
		try {
			if(pref.getScaleCombobox().getValue().equals("Custom")){
				startTime = pref.getFromDateTime();
				endTime = pref.getToDateTime();
				long duration  = endTime.getTime() - startTime.getTime();
				long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
				cpuStatsDataList = ResourcesFacade.getCpuStatDetails(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
			}
			else{
				/*startTime = pref.getFromDateTime();
				endTime = pref.getToDateTime();*/
				cpuStatsDataList = ResourcesFacade.getCpuStatDetails(productInstanceName,instanceId, pref.getNumHistoryMinutes());
			}

			if(cpuStatsDataList.size()!=0){

				/*long utcStartPt = (Utility.getUtcDateFromLocalDate(startTime)).getTime()+OFFSET_MILLIS;
				long utcEndtPt = (Utility.getUtcDateFromLocalDate(endTime)).getTime()+OFFSET_MILLIS;*/
				if(cpuStatsDataList !=null && cpuStatsDataList.size() >1){ //We need at least 2 point to plot a graph
					//CPU Call Rate
					IderaLineChartModel cpuCallRateModel = new IderaLineChartModel();

					for(int i=cpuStatsDataList.size()-1; i>=0; i--){
						CPUStatDetails resource = cpuStatsDataList.get(i);
						//Adding Offset for graphs
						long collectionTime = resource.getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
						cpuCallRateModel.setValue("Compiles", collectionTime, resource.getSqlCompPerSec());
						cpuCallRateModel.setValue("Re-Compiles", collectionTime, resource.getSqlRecompPerSec());
						cpuCallRateModel.setValue("Batches", collectionTime, resource.getBatchesPerSec());
						cpuCallRateModel.setValue("Transactions", collectionTime, resource.getTransPerSec());

					}
					modelsMap.put(CustomizationConstants.CPU_CALL_RATES_MODEL , cpuCallRateModel);

				}/*else{
					cpuCallRateStats.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
				}*/
			}
			else{
				modelsMap.put(CustomizationConstants.CPU_CALL_RATES_MODEL , null);
			}
		}
		catch(CategoryException e){
			throw e;
		}
	}

	private void getResourceDataModel() throws CategoryException {
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
			IderaLineChartModel cpuUsageModel = new IderaLineChartModel();
			//Memory
			IderaLineChartModel memoryUsageModel = new IderaLineChartModel();
			//IderaLineChartModel memoryAreasModel = new IderaLineChartModel();

			//Cache
			IderaAreaChartModel pageLifeModel = new IderaAreaChartModel();
			IderaLineChartModel memoryAreasModel = new IderaLineChartModel();
			//	IderaLineChartModel cacheHitModel = new IderaLineChartModel();
			//Disk
			IderaLineChartModel physicalIOModel = new IderaLineChartModel();
			
			diskVsTotalTimeModel =  new HashMap<String, IderaLineChartModel>();
			diskVsAvgTimeModel  =  new HashMap<String, IderaLineChartModel>();

			//Adding Offset for graphs
			if(resourceCategoryList.size() != 0){

				long startTimeUTC  = resourceCategoryList.get(0).getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
				long endTimeUTC  = resourceCategoryList.get(resourceCategoryList.size()-1).getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
				if(resourceCategoryList !=null && resourceCategoryList.size() >1){ //We need at least 2 point to plot a graph
					//CPU
					
					for(int i=resourceCategoryList.size()-1; i>=0; i--){
						ResourceCategory resource = resourceCategoryList.get(i);
						//Adding Offset for graphs
						long collectionTime = resource.getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;

						cpuUsageModel.setValue("SQL Server", collectionTime, resource.getCpu().getSqlCPUUsage());
						cpuUsageModel.setValue("OS", collectionTime, resource.getCpu().getOSCPUUsage());
						
						memoryAreasModel.setValue("Procedure Cache", collectionTime, Math.round(resource.getMemory().getMemoryAreas().get("ProcedureCacheSizeInKB")/1024*100.0)/100.0);
						//memoryAreasModel.setValue("Connections", collectionTime, Math.round(resource.getMemory().getMemoryAreas().get("ConnectionsMemoryInKB")/1024*100.0)/100.0);
						//memoryAreasModel.setValue("Locks", collectionTime, Math.round(resource.getMemory().getMemoryAreas().get("LocksMemoryInKB")/1024*100.0)/100.0);
						memoryAreasModel.setValue("Database", collectionTime, Math.round(resource.getMemory().getMemoryAreas().get("DatabaseMemoryInKB")/1024*100.0)/100.0);
						memoryAreasModel.setValue("Free", collectionTime, Math.round(resource.getMemory().getMemoryAreas().get("FreePagesInKilobytes")/1024*100.0)/100.0);
						memoryAreasModel.setValue("Other", collectionTime, Math.round(resource.getMemory().getMemoryAreas().get("Others")/1024*100.0)/100.0);
						

						memoryUsageModel.setValue("SQL Used", collectionTime, Math.round(resource.getMemory().getSqlMemory().get("UsedMemoryInKB")/1024*100.0)/100.0);
						memoryUsageModel.setValue("SQL Allocated", collectionTime, Math.round(resource.getMemory().getSqlMemory().get("AllocatedMemoryInKB")/1024*100.0)/100.0);
						memoryUsageModel.setValue("Total Used", collectionTime, Math.round(resource.getMemory().getSqlMemory().get("TotalMemoryInKB")/1024*100.0)/100.0);

						pageLifeModel.setValue("Page Life Expectancy", collectionTime, resource.getMemory().getPageLifeExpectancyinSec());

						/*cacheHitModel.setValue("Buffer Cache", collectionTime, resource.getMemory().getBufferCacheHitRatio());
						cacheHitModel.setValue("Procedure Cache", collectionTime, resource.getMemory().getProcedureCacheHitRatio());
*/
						physicalIOModel.setValue("Checkpoint Writes", collectionTime, resource.getDisk().getSqlPhysicalIO().get("CheckPointWrites"));
						physicalIOModel.setValue("Lazy Writer Writes", collectionTime, resource.getDisk().getSqlPhysicalIO().get("LazyWriterWrites"));
						physicalIOModel.setValue("Read Ahead Reads", collectionTime, resource.getDisk().getSqlPhysicalIO().get("ReadAheadPages"));
						physicalIOModel.setValue("Page Reads", collectionTime, resource.getDisk().getSqlPhysicalIO().get("PageReads"));
						physicalIOModel.setValue("Page Writes", collectionTime, resource.getDisk().getSqlPhysicalIO().get("PageWrites"));
					    List<DiskIOPerSecondEntry> reads = resource.getDisk().getDiskReadsPerSecond();
					    for (DiskIOPerSecondEntry read : reads) {
					    	IderaLineChartModel model =diskVsTotalTimeModel.get(read.getKey());
					    	if (model == null){
					    		model = new IderaLineChartModel();
					    	} 
					    	model.setValue("Reads", collectionTime, read.getValue());
					    	diskVsTotalTimeModel.put(read.getKey(), model);
					    }
					    List<DiskIOPerSecondEntry> writes = resource.getDisk().getDiskWritesPerSecond();
					    for (DiskIOPerSecondEntry write : writes) {
					    	IderaLineChartModel model = diskVsTotalTimeModel.get(write.getKey());
					    	if (model == null){
					    		model = new IderaLineChartModel();
					    	} 
					    	model.setValue("Writes", collectionTime, write.getValue());
				    		diskVsTotalTimeModel.put(write.getKey(), model);
					    }
					    List<DiskIOPerSecondEntry> avgDiskPerReads = resource.getDisk().getAvgDiskMsPerRead();
					    for (DiskIOPerSecondEntry avgDiskPerRead : avgDiskPerReads) {
					    	IderaLineChartModel model = diskVsAvgTimeModel.get(avgDiskPerRead.getKey());
					    	if (model == null){
					    		model = new IderaLineChartModel();
					    	} 
					    	model.setValue("Per Read", collectionTime, avgDiskPerRead.getValue());
					    	diskVsAvgTimeModel.put(avgDiskPerRead.getKey(), model);
					    }
					    List<DiskIOPerSecondEntry> avgDiskMsPerWrites = resource.getDisk().getAvgDiskMsPerWrite();
					    for (DiskIOPerSecondEntry avgDiskMsPerWrite : avgDiskMsPerWrites) {
					    	IderaLineChartModel model = diskVsAvgTimeModel.get(avgDiskMsPerWrite.getKey());
					    	if (model == null){
					    		model = new IderaLineChartModel();
					    	} 
					    	model.setValue("Per Write", collectionTime, avgDiskMsPerWrite.getValue());
					    	diskVsAvgTimeModel.put(avgDiskMsPerWrite.getKey(), model);
					    }
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
							} else 
								cpuUsageModel.setValue("Baseline", endTimeUTC, baselineValue.getValue());
						}
					}	//
					
					modelsMap.put(CustomizationConstants.SQL_CPU_MODEL , cpuUsageModel);
					modelsMap.put(CustomizationConstants.SQL_CPU_MEMORY_USAGE_MODEL, memoryUsageModel);
					modelsMap.put(CustomizationConstants.CACHE_MEMORY_AREAS_MODEL, memoryAreasModel);
					modelsMap.put(CustomizationConstants.CACHE_PAGE_LIFE_MODEL, pageLifeModel);
				//	modelsMap.put(CustomizationConstants.CACHE_HIT_MODEL, cacheHitModel);
					modelsMap.put(CustomizationConstants.DISK_PHYSICAL_IO_MODEL, physicalIOModel);
					if(diskVsTotalTimeModel.keySet()!=null){
						selectedDisk = diskVsTotalTimeModel.keySet().iterator().next();
						modelsMap.put(CustomizationConstants.DISK_TOTAL_RW_MODEL, diskVsTotalTimeModel.get(selectedDisk));
					}

				}
			}
			else{
				setEmptyResourcesModels();
			}

		} catch (CategoryException e) {
			// TODO Auto-generated catch block
			log.error(e.getMessage());
		}
	}
	
	private void setEmptyResourcesModels() {
		modelsMap.remove(CustomizationConstants.SQL_CPU_MODEL);
		modelsMap.remove(CustomizationConstants.SQL_CPU_MEMORY_USAGE_MODEL);
		modelsMap.remove(CustomizationConstants.CACHE_MEMORY_AREAS_MODEL);
		modelsMap.remove(CustomizationConstants.CACHE_PAGE_LIFE_MODEL);
	//	modelsMap.put(CustomizationConstants.CACHE_HIT_MODEL, cacheHitModel);
		modelsMap.remove(CustomizationConstants.DISK_PHYSICAL_IO_MODEL);
		modelsMap.remove(CustomizationConstants.DISK_TOTAL_RW_MODEL);
		
	}

	private void getOSPagingModelData() throws CategoryException {
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		List<OSPagingData> osPagingDataList;
		try {
			if(pref.getScaleCombobox().getValue().equals("Custom")){
				Date startTime = pref.getFromDateTime();
				Date endTime = pref.getToDateTime();
				long duration  = endTime.getTime() - startTime.getTime();
				long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
				osPagingDataList = ResourcesFacade.getOSPagingDetails(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
			}
			else{
				osPagingDataList = ResourcesFacade.getOSPagingDetails(productInstanceName,instanceId, pref.getNumHistoryMinutes());
			}

			if(osPagingDataList != null ){
				if(osPagingDataList.size()!=0){

					//if(osPagingDataList !=null && osPagingDataList.size() >1){ //We need at least 2 point to plot a graph
					//CPU Call Rate
					IderaLineChartModel osPagingModel = new IderaLineChartModel();

					for(int i=osPagingDataList.size()-1; i>=0; i--){
						OSPagingData resource = osPagingDataList.get(i);
						//Adding Offset for graphs
						long collectionTime = resource.getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
						osPagingModel.setValue("Pages", collectionTime, resource.getPagesPerSec());
					}
					modelsMap.put(CustomizationConstants.MEMORY_OS_PAGING_MODEL, osPagingModel);

				}
				else{
					modelsMap.put(CustomizationConstants.MEMORY_OS_PAGING_MODEL, null);
				}
			}
			else{
				modelsMap.put(CustomizationConstants.MEMORY_OS_PAGING_MODEL, null);
			}
		}
		catch(CategoryException e){
			throw e;
		}
	}

	private void getLockWaitsModelData() throws CategoryException {
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		List<LockWaitData> lockWaitsDataList;
		try {
			if(pref.getScaleCombobox().getValue().equals("Custom")){
				Date startTime = pref.getFromDateTime();
				Date endTime = pref.getToDateTime();
				long duration  = endTime.getTime() - startTime.getTime();
				long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
				lockWaitsDataList = ResourcesFacade.getLockWaitsDetails(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
			}
			else{
				lockWaitsDataList = ResourcesFacade.getLockWaitsDetails(productInstanceName,instanceId, pref.getNumHistoryMinutes());
			}

			if(lockWaitsDataList.size()!=0){

				if(lockWaitsDataList !=null && lockWaitsDataList.size() >1){ //We need at least 2 point to plot a graph
					IderaLineChartModel lockWaitsModel = new IderaLineChartModel();

					for(int i=lockWaitsDataList.size()-1; i>=0; i--){
						LockWaitData dataObj = lockWaitsDataList.get(i);
						//Adding Offset for graphs
						if(dataObj == null)
							continue;
						long collectionTime = dataObj.getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
						lockWaitsModel.setValue("Database", collectionTime, dataObj.getDatabase());
						lockWaitsModel.setValue("HoBT", collectionTime, dataObj.getHoBT());
						lockWaitsModel.setValue("Object", collectionTime, dataObj.getObject());
						lockWaitsModel.setValue("Extent", collectionTime, dataObj.GetExtent());
						lockWaitsModel.setValue("Key/RID", collectionTime, (dataObj.getKey()+dataObj.getRID()));
						lockWaitsModel.setValue("Page", collectionTime, dataObj.getPage());
						lockWaitsModel.setValue("File", collectionTime, dataObj.getFile());
						lockWaitsModel.setValue("Metadata", collectionTime, dataObj.getMetadata());
					}
					modelsMap.put(CustomizationConstants.LOCK_WAITS_MODEL , lockWaitsModel);
				}/*else{
					if(lockWaits!=null)
					lockWaits.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
				}*/
			}
			else{
				modelsMap.put(CustomizationConstants.LOCK_WAITS_MODEL , null);
			}
		}
		catch(CategoryException e){
			throw e;
		}
	}

	private void getTempDBDataModel() throws Exception {
		List<TempDBUsageDetails> tempDbModel;
		IderaLineChartModel pagesModel = new IderaLineChartModel();
		IderaLineChartModel model = new IderaLineChartModel();
		try{
			HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
	        if(pref.getScaleCombobox().getValue().equals("Custom")){
	        	Date startTime = pref.getFromDateTime();
				Date endTime = pref.getToDateTime();
				long duration  = endTime.getTime() - startTime.getTime();
				long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
				tempDbModel = InstanceDetailsDatabaseFacade.getTempDBUsageDetails(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
	        }
	        else{
	        	tempDbModel = InstanceDetailsDatabaseFacade.getTempDBUsageDetails(productInstanceName,instanceId, pref.getNumHistoryMinutes());
	        }

			if (tempDbModel != null & tempDbModel.size() >0){

				for (TempDBUsageDetails usg : tempDbModel) {
					//Adding offset for graphs
					if(usg==null)
						continue;
					long collectionTime = usg.getUTCCollectionDateTime().getTime() + OFFSET_MILLIS;
					model.setValue("Internal Objects",collectionTime, Utility.round(usg.getInternalObjectsInMB(), 2));
					model.setValue("Unallocated Space",collectionTime, Utility.round(usg.getUnallocatedSpaceInMB(), 2));
					model.setValue("User Objects", collectionTime, Utility.round(usg.getUserObjectsInMB(), 2));
					model.setValue("Mixed Extents", collectionTime, Utility.round(usg.getMixedExtentsInMB(), 2));
					model.setValue("Version Store", collectionTime, Utility.round(usg.getVersionStoreInMB(), 2));

					pagesModel.setValue("PFS Page", collectionTime, usg.getTempdbPFSWaitTimeMilliseconds());
					pagesModel.setValue("GAM Page", collectionTime, usg.getTempdbGAMWaitTimeMilliseconds());
					pagesModel.setValue("SGAM Page", collectionTime, usg.getTempdbSGAMWaitTimeMilliseconds());
				}
				modelsMap.put(CustomizationConstants.TEMPDB_USAGE_MODEL, model);
				modelsMap.put(CustomizationConstants.TEMPDB_PAGING_MODEL, pagesModel);
			}else{
				modelsMap.put(CustomizationConstants.TEMPDB_USAGE_MODEL, null);
				modelsMap.put(CustomizationConstants.TEMPDB_PAGING_MODEL, null);
			}	

		} catch (Exception x) {
			throw x;
		}
	}

	public void setHistoryPanelValues(HistoryPanelPreferencesBean prefHistory) {

		setEndDate(prefHistory.getToDate());
		setEndTime(prefHistory.getToTime());
		setFromDate(prefHistory.getFromDate());
		setFromTime(prefHistory.getFromTime());

		//refreshView();

	}
	/**todo: is it really required??
	 * Updates the values stored in session
	 */ //TODO: check this
	private void updateSession() {

		SingleInstanceAlertsPreferencesBean sessionBean = PreferencesUtil.getInstance()
				.getSingleInstanceAlertsPreferenceInSession(instanceId);

		//sessionBean.setModelData(modelData);
		sessionBean.setFromDate(fromDate);
		sessionBean.setFromTime(fromTime);
		sessionBean.setEndDate(endDate);
		sessionBean.setEndTime(endTime);
		//sessionBean.setCategoryOptions(categoryOptions);
		PreferencesUtil.getInstance().setSingleInstanceAlertsPreferenceInSession(sessionBean);
		overPrefBean.setCustomizedGraphSettings(graphCategoryOptions);
		PreferencesUtil.getInstance().setOverviewPreferenceInSession(overPrefBean);
		

	}
	/**
	 * Refresh view using API call if value of sessionFlag is false or using
	 * model data stored in session if this flag is true
	 * 
	 * @param sessionsFlag
	 */
	private void refreshView() {
		try{
			refreshModels();
			updateGraphs();
		}
		catch(CategoryException e){
			log.error(e.getMessage());
		}
		catch(Exception e){
			e.printStackTrace();
		}
	}

	private void updateGraphs() {
		//detach all graphs from container
		List<Component> layoutChildren = new ArrayList<>(customChartsVLayout.getChildren());
		for (Component child : layoutChildren) {
			child.detach();
		}

		// Draw customized graphs
		int vlayoutHeight = 0;
		int numCategories = graphCategoryOptions.getSize();
		for (int i = 0; i < numCategories; i++) {

			GraphCategories categoryDetails = graphCategoryOptions.getElementAt(i);
			GraphCategoriesEnum gCat = categoryDetails.getCategory();
			String categoryName = gCat.getCategoryName();
			vlayoutHeight += createGraphAccToCategory(categoryName,categoryDetails.isVisible());
		}

		customChartsVLayout.setHeight(vlayoutHeight + "px");
		//customChartsContainerDiv.setHeight((vlayoutHeight )+ "px");
		Clients.resize(customChartsContainerDiv);
		BindUtils.postNotifyChange(null, null, this, "graphCategoryOptions");		
	}
	
	private int createGraphAccToCategory(String categoryName, boolean isGraphVisible) {
		if(GraphCategoriesEnum.CPU.getCategoryName().equalsIgnoreCase(categoryName)){
			if(isGraphVisible){
				customChartsVLayout.appendChild(drawCPUGraph());
			}
			else{
				SetGraphInvisible(sqlCpuUsage);
				SetGraphInvisible(cpuCallRateStats);
			}
		}
		else if(GraphCategoriesEnum.CACHE.getCategoryName().equalsIgnoreCase(categoryName)){
			if(isGraphVisible){
				customChartsVLayout.appendChild(drawCacheGraph());
			}
			else{
				SetGraphInvisible(pageLifeExpectancy);
				SetGraphInvisible(memoryAreas);
			}
		}
		else if(GraphCategoriesEnum.MEMORY.getCategoryName().equalsIgnoreCase(categoryName)){
			if(isGraphVisible){
				customChartsVLayout.appendChild(drawMemoryGraph());
			}
			else{
				SetGraphInvisible(sqlMemoryUsage);
				SetGraphInvisible(sqlOSPaging);
			}
		}
		else if(GraphCategoriesEnum.TEMPDB.getCategoryName().equalsIgnoreCase(categoryName)){
			if(isGraphVisible){
				customChartsVLayout.appendChild(drawTempDbGraph());
			}
			else{
				SetGraphInvisible(tempdbUsedByTime);
				SetGraphInvisible(tempDbPagesWaitTime);
			}
		}
		else if(GraphCategoriesEnum.DISK.getCategoryName().equalsIgnoreCase(categoryName)){
			if(isGraphVisible){

				customChartsVLayout.appendChild(drawDiskGraph());
			}
			else{
				SetGraphInvisible(sqlPhysicalIO);
				SetGraphInvisible(diskTotalTime);
				SetGraphInvisible(diskAvgTime);
			}
		}
		else if(GraphCategoriesEnum.SESSIONS.getCategoryName().equalsIgnoreCase(categoryName)){
			if(isGraphVisible){
				customChartsVLayout.appendChild(drawSessionsGraph());
			}
			else{
				SetGraphInvisible(blockedSessionChart);
			}
		}
		else if(GraphCategoriesEnum.SERVER_WAITS.getCategoryName().equalsIgnoreCase(categoryName)){
			if(isGraphVisible){
				customChartsVLayout.appendChild(drawServerWaitGraph());
			}
			else{
				SetGraphInvisible(serverWaitsChart1);
			}
		}
		else if(GraphCategoriesEnum.DATABASES.getCategoryName().equalsIgnoreCase(categoryName)){
			if(isGraphVisible){
				customChartsVLayout.appendChild(drawDatabasesGraphs());
			}
			else{
				SetGraphInvisible(databaseCharts);
			}
		}
		else if(GraphCategoriesEnum.LOCK_WAITS.getCategoryName().equalsIgnoreCase(categoryName)){
			if(isGraphVisible){
				customChartsVLayout.appendChild(drawLockWaitsGraph());
			}
			else{
				SetGraphInvisible(lockWaits);
			}
		}
		else if(GraphCategoriesEnum.CUSTOM_COUNTERS.getCategoryName().equalsIgnoreCase(categoryName)){
			if(isGraphVisible){
				customChartsVLayout.appendChild(drawCustomCounterGraph());
			}
			else{
				SetGraphInvisible(customCountersStat);
				
			}
		}
		else if(GraphCategoriesEnum.NETWORK.getCategoryName().equalsIgnoreCase(categoryName)){
			if(isGraphVisible){
				customChartsVLayout.appendChild(drawNetworkStatsGraph());
			}
			else{
				SetGraphInvisible(networkStats);
			}
		}
		else if(GraphCategoriesEnum.FILE_ACTIVITY.getCategoryName().equalsIgnoreCase(categoryName)){
			if(isGraphVisible){
				customChartsVLayout.appendChild(drawFileStatsGraph());
			}
			else{
				SetGraphInvisible(fileStats);
			}
		}
		else if(GraphCategoriesEnum.VIRTUALIZATION.getCategoryName().equalsIgnoreCase(categoryName)){
			if(isGraphVisible){
				customChartsVLayout.appendChild(drawVirtualizationGraph());
			}
			else{
				SetGraphInvisible(virtualizationMemChart);
				SetGraphInvisible(virtualizationReadWriteChart);
			}
		}
		return 700;
	}
	/**
	 * Author:Accolite
	 * Date : 21th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release
	 * @return Div
	 */
	private Div drawVirtualizationGraph(){
		Div virtualDiv = new Div();
		virtualDiv.setSclass(CustomizationConstants.GRAPH_BORDER);
		//virtualDiv.setStyle("padding-left: 55px;");
		Vlayout vlayout = new Vlayout();
		Hlayout headerLayout = new Hlayout();
		headerLayout.appendChild(getGraphLabel(CustomizationConstants.VIRTUALIZATION_TITLE));
		headerLayout.setStyle("padding-right: 4px");
		Div graphDiv =  new Div();
		graphDiv.setSclass(CustomizationConstants.GRAPH_BORDER);
		graphDiv.setStyle("padding-left: 55px;");
		Div memGraphDiv =  new Div();
		Div readWriteGraphDiv =  new Div();
		Vlayout vLay  =  new Vlayout();
		Space space = new Space();
		space.setSpacing("30px");
		final Combobox virtualcb =  new Combobox();
		virtualizationMemChart = getIderaLineChart();
		virtualizationMemChart.getChart().setDrillable(true);
		
		virtualizationReadWriteChart = getIderaLineChart();
		virtualizationReadWriteChart.getChart().setDrillable(true);
		
		final ListModelList<String> model = new ListModelList<String>(VirtualizationGraphOption.getDefaultOptionNameList());
		//virtualcb.setModel(model);
		for(int i = 0; i<model.size();i++){
			virtualcb.appendItem(model.get(i));
		}
		virtualcb.setSelectedIndex(0); virtualcb.setReadonly(true);
		
		if(model.getSelection() != null)
			selectVirtualGraph = virtualcb.getSelectedItem().getLabel();
		else
			selectVirtualGraph =VirtualizationGraphOptionEnum.VIRTUAL_MACHINE.getOptionName();
		
		virtualcb.addEventListener("onChange", (new EventListener<Event>(){
			public void onEvent(Event event) throws Exception {
				Set<String> strList = model.getSelection();
				selectVirtualGraph = virtualcb.getSelectedItem().getLabel();
				redrawVirtualizationGraph(selectVirtualGraph);
            }
        }));
		
		headerLayout.appendChild(virtualcb);
		vlayout.appendChild(headerLayout);
		redrawVirtualizationGraph(selectVirtualGraph);
		memGraphDiv.appendChild(virtualizationMemChart);
		readWriteGraphDiv.appendChild(virtualizationReadWriteChart);
		graphDiv.appendChild(memGraphDiv);
		graphDiv.appendChild(readWriteGraphDiv);
		vlayout.appendChild(graphDiv);
		virtualDiv.appendChild(vlayout);
		
		return virtualDiv;
	}
	
	/**
	 * Author:Accolite
	 * Date : 21th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release
	 * @param selectedGraph
	 */
	private void redrawVirtualizationGraph(String selectedGraph){
		VirtualizationGraphOptionEnum modelEnum = VirtualizationGraphOption.getVirtualGraphOptionEnum(selectedGraph);
		String modelName = modelEnum.getOptionName();
		boolean isMemChartNoData = false;
		boolean isVirRDChartNull = false;
		if(virtualizationStatData!=null 
				&& (CustomizationConstants.VIRTUALIZATION_GRAPH_TYPE).equalsIgnoreCase(virtualizationStatData.getType())){
			if(VirtualizationGraphOptionEnum.VIRTUAL_MACHINE.getOptionName().equalsIgnoreCase(modelName) 
					&& modelsMap.get(CustomizationConstants.VM_AVAILABLE_MEMORY_MODEL)!=null){
				CategoryModel model = modelsMap.get(CustomizationConstants.VM_AVAILABLE_MEMORY_MODEL);
				if(model!=null)
					virtualizationMemChart.setModel(model);
				else{
					isMemChartNoData = true;
					//virtualizationMemChart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
				}
			}
			else  if(VirtualizationGraphOptionEnum.HOST_MACHINE.getOptionName().equalsIgnoreCase(modelName) 
					&& modelsMap.get(CustomizationConstants.HOST_AVAILABLE_MEMORY_MODEL)!=null){
				CategoryModel model = modelsMap.get(CustomizationConstants.HOST_AVAILABLE_MEMORY_MODEL);
				if(model!=null)
					virtualizationMemChart.setModel(model);
				else {
					isMemChartNoData = true;
					//virtualizationMemChart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
				}
			}
			else{
				isMemChartNoData = true;
				//virtualizationMemChart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
			}	
		}
		else if(virtualizationStatData!=null 
				&& !((CustomizationConstants.VIRTUALIZATION_GRAPH_TYPE).equalsIgnoreCase(virtualizationStatData.getType()))){
			if(VirtualizationGraphOptionEnum.VIRTUAL_MACHINE.getOptionName().equalsIgnoreCase(modelName) 
					&& modelsMap.get(CustomizationConstants.VM_ACTIVE_BALOONED_CONSUME_GRANTED_MODEL)!=null){
				CategoryModel model = modelsMap.get(CustomizationConstants.VM_ACTIVE_BALOONED_CONSUME_GRANTED_MODEL);
				
				if(model!=null){
					virtualizationMemChart.setModel(model);
				}
				else
					isMemChartNoData =true;
			}
			else  if(VirtualizationGraphOptionEnum.HOST_MACHINE.getOptionName().equalsIgnoreCase(modelName) 
					&& modelsMap.get(CustomizationConstants.HOST_ACTIVE_BALOONED_CONSUME_GRANTED_MODEL)!=null){
				CategoryModel model= modelsMap.get(CustomizationConstants.HOST_ACTIVE_BALOONED_CONSUME_GRANTED_MODEL);
				virtualizationMemChart.setModel(model);
			}
			else{
				isMemChartNoData = true;
				//virtualizationMemChart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
			}
		}
		
		if(VirtualizationGraphOptionEnum.VIRTUAL_MACHINE.getOptionName().equalsIgnoreCase(modelName) 
				&& modelsMap.get(CustomizationConstants.VM_READ_WRITE_MODEL)!=null){
			CategoryModel model =  modelsMap.get(CustomizationConstants.VM_READ_WRITE_MODEL);
			
			if(model!=null)
				virtualizationReadWriteChart.setModel(model);
			else
				isVirRDChartNull = true;
		}
		else if(VirtualizationGraphOptionEnum.HOST_MACHINE.getOptionName().equalsIgnoreCase(modelName) 
				&& modelsMap.get(CustomizationConstants.HOST_READ_WRITE_MODEL)!=null){
			CategoryModel model= modelsMap.get(CustomizationConstants.HOST_READ_WRITE_MODEL);
			if(model!=null)
				virtualizationReadWriteChart.setModel(model);
			else
				isVirRDChartNull = true;
		}
		else{
			isVirRDChartNull = true;
		}
		if(isMemChartNoData){
			virtualizationMemChart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		}
		else
			setXAxisDomain(virtualizationMemChart);
		if(isVirRDChartNull){
			virtualizationReadWriteChart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		}
		else
			setXAxisDomain(virtualizationReadWriteChart);
	}
	private Div drawDatabasesGraphs() {
		Div dbDiv = new Div();
		dbDiv.setSclass(CustomizationConstants.GRAPH_BORDER);
		Div graphDiv =  new Div();
		graphDiv.setHeight(CustomizationConstants.GRAPH_CONTAINER_DIV_HEIGHT);
		//Vlayout vlayout = new Vlayout();
		Hlayout headerLayout = new Hlayout();
		headerLayout.appendChild(getGraphLabel(CustomizationConstants.DATABASE_TITLE));
		Space space = new Space();
		space.setSpacing("30px");
		headerLayout.appendChild(space);
		final Combobox sb_Database =  new Combobox();
	
		databaseCharts =  getIderaLineChart();
	
		final ListModelList<String> model = new ListModelList<String>(DBGraphOptions.getDefaultOptionNameList());
		//sb.setModel(model); 
		sb_Database.setAutocomplete(true);  sb_Database.setReadonly(true);
		for(int i = 0; i<model.size();i++){
			sb_Database.appendItem(model.get(i));
		}
		sb_Database.setSelectedIndex(0);
		
		if(model.getSelection() != null)
			selectDBGraph = sb_Database.getSelectedItem().getLabel();
		else
			selectDBGraph =DBGraphOptionEnum.TRANS_PER_SEC.getOptionName();

		sb_Database.addEventListener("onChange", (new EventListener<Event>(){
			public void onEvent(Event event) throws Exception {
				selectDBGraph= sb_Database.getSelectedItem().getLabel();
				redrawDBGraph(selectDBGraph);
            }
        }));
		
		headerLayout.appendChild(sb_Database);
		dbDiv.appendChild(headerLayout);
		//Div grDiv =  new Div();
		redrawDBGraph(selectDBGraph);
		graphDiv.appendChild(databaseCharts);
		graphDiv.setStyle("padding-left: 55px;");
		//redirection disabled for database on overview screen
		/*grDiv.addEventListener("onClick", new EventListener<Event>() {
			@Override
			public void onEvent(Event arg0) throws Exception {
				launchToDatabases(InstanceSubCategoriesTab.DATABASES_SUMMARY.getSubTabId());
			}
		});
*/		dbDiv.appendChild(graphDiv);
		
		//dbDiv.appendChild(vlayout);
		return dbDiv;
	}

	/*private void redrawDBGraph(String selectedGraph){
		DBGraphOptionEnum modelEnum = DBGraphOptions.getDBGraphOptionEnum(selectedGraph);
		String modelName = modelEnum.getModelName();
		
		if(modelsMap.get(modelName)!=null){
			databaseCharts.setModel(modelsMap.get(modelName));
			databaseCharts.getChart().setyAxisCustomMaxDomainValue(true);
			databaseCharts.getChart().setyAxisCustomMinDomainValue(true);
			databaseCharts.getChart().setDrillable(true);
		}
		else
		{
			databaseCharts.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		}	
	}*/
	
	@SuppressWarnings("unchecked")
	private void redrawDBGraph(String selectedGraph){
		DBGraphOptionEnum modelEnum = DBGraphOptions.getDBGraphOptionEnum(selectedGraph);
		String modelName = modelEnum.getModelName();
		
		Map<String,IderaLineChartModel> dbStatsLineChartModels = (Map<String, IderaLineChartModel>) 
				Executions.getCurrent().getDesktop().getAttribute(CustomizationConstants.DB_STAT_MODEL_NAME);
		databaseCharts.refresh();
		if(dbStatsLineChartModels !=null && dbStatsLineChartModels.get(modelName)!=null){
			databaseCharts.setModel(dbStatsLineChartModels.get(modelName));
			databaseCharts.getChart().setDrillable(true);
			setXAxisDomain(databaseCharts);
		}
		else
		{
			databaseCharts.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		}	
	}
	
	@SuppressWarnings("unchecked")
	private Component drawFileStatsGraph() {
		Map<String,IderaLineChartModel> fileStatsLineChartModels = (Map<String, IderaLineChartModel>) 
				Executions.getCurrent().getDesktop().getAttribute("fileStatsLineChartModels");
		//SingleInstanceOverviewPreferenceBean overviewPref = PreferencesUtil.getInstance().getOverviewPreferenceInSession(instanceId);
		Div lwDiv = new Div();
		lwDiv.setSclass(CustomizationConstants.GRAPH_BORDER);
		lwDiv.appendChild(getGraphLabel(CustomizationConstants.FILE_ACTIVITY_TITLE));
		Div graphDiv =  new Div();
		graphDiv.setHeight(CustomizationConstants.GRAPH_CONTAINER_DIV_HEIGHT);
		fileStats = getIderaLineChart();
		if (fileStatsLineChartModels!=null){
			IderaLineChartModel model = fileStatsLineChartModels.get(overPrefBean.getSelectedPerSecIndex());
			if(model != null ) {
				fileStats.setModel(model);
				fileStats.getChart().setDrillable(true);
				setXAxisDomain(fileStats);
			}else{
				fileStats.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
				//return lwDiv;

			}
		}
		else{
			fileStats.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
			//return lwDiv;
		}
		
		graphDiv.appendChild(fileStats);
		graphDiv.setStyle("padding-left: 55px");
		//First combo box
		final Combobox sb =  new Combobox();
		sb.setWidth("120px"); sb.setReadonly(true);
		sb.setId("fileStatDropDown");
		sb.appendItem("Reads/sec");
		sb.appendItem("Writes/sec");
		sb.appendItem("Transfers/sec");
		
		//second combo box
		sb1 =  new Combobox(); sb1.setWidth("220px"); sb1.setReadonly(true);
		sb1.setId("chooseFileStatDropDown");
		//AddComboItems(sb1);
		sb1.appendItem("All Files");
		sb1.appendItem("Choose Databases..");
		sb1.appendItem("Choose Disks..");
		sb1.appendItem("Choose Files..");
		sb.setSelectedIndex(overPrefBean.getSelectedPerSecIndex());
		sb1.setSelectedIndex(overPrefBean.getSelectedComponentIndex());
		/*if(fileStatsLineChartModels == null || fileStatsLineChartModels.isEmpty()){
			sb.setDisabled(true);
			sb1.setDisabled(true);
		}*/
		sb1.addEventListener("onSelect", (new EventListener<Event>(){
			public void onEvent(Event event) throws Exception {
				//SingleInstanceOverviewPreferenceBean overviewPref = PreferencesUtil.getInstance().getOverviewPreferenceInSession(instanceId);
				int i = sb1.getSelectedIndex();
				List<String> hiddenColumns = new ArrayList<String>();
				boolean openChooserWindow = true;
				switch(i){
				case 0 :
					openChooserWindow = false;
					break;
				case 1 :
					hiddenColumns = FileFilterDataComposer.getAllDatabases();
					overPrefBean.setSelectedComponentIndex(1);
					selectedFilter = "DatabaseName";
					break;
				case 2 :
					hiddenColumns = FileFilterDataComposer.getAllDrives();
					overPrefBean.setSelectedComponentIndex(2);
					selectedFilter = "Drive";
					break;
				case 3 :
					hiddenColumns = FileFilterDataComposer.getAllFilePaths();
					overPrefBean.setSelectedComponentIndex(3);
					selectedFilter = "FileName"; 
					sb1.setText("All files selected");
					break;
				}
				List<String> visibleColumns = new ArrayList<String>();
				PreferencesUtil.getInstance().setOverviewPreferenceInSession(overPrefBean);
				List<String> selectedComponents = overPrefBean.getSelectedFileComponent();
				List<String> hiddenColumns1 =new ArrayList<String>();
				for(int k = 0 ; k < hiddenColumns.size(); k++){
					if(selectedComponents.contains(hiddenColumns.get(k))){
						visibleColumns.add(hiddenColumns.get(k));
					}
					else{
						hiddenColumns1.add(hiddenColumns.get(k));
					}
				}					

				
				columnchooser.setVisibleColumns(visibleColumns);
				columnchooser.setHiddenColumns(hiddenColumns1);
				if(openChooserWindow){
					columnchooser.open(sb1,"end_before");
				}
				else{
					selectedFileComponent = new ArrayList<String>();
					selectedFileComponent.add("AllFiles");
					selectedFileComponent.add("");
					overPrefBean.setSelectedFileComponent(selectedFileComponent);
					PreferencesUtil.getInstance().setOverviewPreferenceInSession(overPrefBean);
					sb1.setText("All files selected");
					FileFilterDataComposer.createFilterMapData(instanceId);
					refreshFileStatsGraph();
				}
			}
		}));
		sb.addEventListener("onChange", (new EventListener<Event>(){
			public void onEvent(Event event) throws Exception {
				int i = sb.getSelectedIndex();
				SingleInstanceOverviewPreferenceBean overviewPref = PreferencesUtil.getInstance().getOverviewPreferenceInSession(instanceId);
				overviewPref.setSelectedPerSecIndex(i);
				PreferencesUtil.getInstance().setOverviewPreferenceInSession(overviewPref);
				Map<String,IderaLineChartModel> fileStatsLineChartModels = (Map<String, IderaLineChartModel>) 
						Executions.getCurrent().getDesktop().getAttribute("fileStatsLineChartModels");
				/** incase of no data available, no action required  */
				if(fileStatsLineChartModels == null)
					return;
				IderaLineChartModel model = new IderaLineChartModel();
				switch(i){
				case 0 :
					model = fileStatsLineChartModels.get(0);
					fileStats.setModel(model);
					break;
				case 1 :
					model = fileStatsLineChartModels.get(1);
					fileStats.setModel(model);
					break;
				case 2 :
					model = fileStatsLineChartModels.get(2);
					fileStats.setModel(model);
					break;
				}
			}
		}));
		Space space = new Space();
		space.setSpacing("30px");
		Label topLabel = new Label("Top");
		topLabel.setStyle("font-size:16px");
		Label chooseLabel = new Label();
		chooseLabel.setValue("  ");
		lwDiv.appendChild(space);
		lwDiv.appendChild(topLabel);
		lwDiv.appendChild(sb);
		lwDiv.appendChild(chooseLabel);
		lwDiv.appendChild(sb1);
		lwDiv.appendChild(graphDiv);
		return lwDiv;
	}
	
	private String getFileSelectedLabel() {
		// TODO Auto-generated method stub
		if(selectedFileComponent == null || selectedFileComponent.size()==0)
			return null;
		switch (selectedFileComponent.get(0)) {
		case "AllFiles":
			return "All files selected";
		case "Drive":
			if(selectedFileComponent.size()>2)
				return "Multiple Disks Selected";
			else
				return "Disk "+ selectedFileComponent.get(1);
		case "DatabaseName":
			if(selectedFileComponent.size()>2)
				return "Multiple Databases Selected";
			else
				return "Database "+ selectedFileComponent.get(1);
		case "FileName":
			if(selectedFileComponent.size()>2)
				return "Multiple Files selected";
			else if(selectedFileComponent.size() <2){
				selectedFileComponent = new ArrayList<String>();
				selectedFileComponent.add("AllFiles");
				selectedFileComponent.add("");
				return "All files selected";
				//return "No Files Selected";
			}
			else
				return "File "+ selectedFileComponent.get(1);
		default:
			break;
		}
		return null;
	}

	

	@Listen("onColumnVisibilityChange=#columnchooser")
	public void doColumnVisibilityChange(ColumnVisibilityChangeEvent event) {
		List<String> hiddenColumns = new ArrayList<String>();
		List<String> visibleColumns = new ArrayList<String>();
		visibleColumns = event.getVisibleColumns();
		hiddenColumns = event.getHiddenColumns();
		selectedFileComponent= new ArrayList<String>();
		SingleInstanceOverviewPreferenceBean overviewPref = PreferencesUtil.getInstance().getOverviewPreferenceInSession(instanceId);
		selectedFileComponent.add(selectedFilter);
		for(int i =0;i<visibleColumns.size();i++){
			selectedFileComponent.add(visibleColumns.get(i));
		}
		
		sb1.setText(getFileSelectedLabel());
		overviewPref.setSelectedFileComponent(selectedFileComponent);
		PreferencesUtil.getInstance().setOverviewPreferenceInSession(overviewPref);
		Map<String,IderaLineChartModel> fileStatsLineChartModels = (Map<String, IderaLineChartModel>) 
				Executions.getCurrent().getDesktop().getAttribute("fileStatsLineChartModels");
		if(fileStatsLineChartModels!=null)
			FileFilterDataComposer.createFilterMapData(instanceId);
		refreshFileStatsGraph();
	}

	private void refreshFileStatsGraph() {
		Map<String,IderaLineChartModel> fileStatsLineChartModels = (Map<String, IderaLineChartModel>) 
				Executions.getCurrent().getDesktop().getAttribute("fileStatsLineChartModels");
		SingleInstanceOverviewPreferenceBean overviewPref = PreferencesUtil.getInstance().getOverviewPreferenceInSession(instanceId);
		IderaLineChartModel model = new IderaLineChartModel();
		if(fileStatsLineChartModels!=null)
			model = fileStatsLineChartModels.get(overviewPref.getSelectedPerSecIndex());
		fileStats.setModel(model);
	}

	private Component drawNetworkStatsGraph() {
		Div lwDiv = new Div();
		lwDiv.setSclass(CustomizationConstants.GRAPH_BORDER);
		lwDiv.appendChild(getGraphLabel(CustomizationConstants.NETWORK_TITLE));
		Div graphDiv =  new Div();
		graphDiv.setStyle("cursor:pointer");
		graphDiv.setHeight(CustomizationConstants.GRAPH_CONTAINER_DIV_HEIGHT);
		networkStats = getIderaLineChart();
		//networkStats.refresh();
		CategoryModel model = modelsMap.get(CustomizationConstants.NETWORK_MODEL);
		if(model != null){
			networkStats.setModel(model);
			//lockWaits.setTitle("");
			networkStats.getChart().setDrillable(true);
			setXAxisDomain(networkStats);
		}
		else{
			networkStats.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		}
		graphDiv.appendChild(networkStats);
		graphDiv.setStyle("padding-left: 55px;");
		lwDiv.appendChild(graphDiv);
		return lwDiv;
	}

	private Component drawCustomCounterGraph() {
		Div lwDiv = new Div();		
		lwDiv.setSclass(CustomizationConstants.GRAPH_BORDER);
		lwDiv.appendChild(getGraphLabel(CustomizationConstants.CUSTOM_COUNTERS_TITLE));
		Div graphDiv =  new Div();
		graphDiv.setHeight(CustomizationConstants.GRAPH_CONTAINER_DIV_HEIGHT);
		customCountersStat = getIderaLineChart();
		//customCountersStat.refresh();
		CategoryModel model = modelsMap.get(CustomizationConstants.CUSTOM_COUNTERS_MODEL);
		if(model != null){
			customCountersStat.setModel(model);
			customCountersStat.getChart().setDrillable(true);
			setXAxisDomain(customCountersStat);
		}
		else{
			customCountersStat.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		}
		graphDiv.appendChild(customCountersStat);
		graphDiv.setStyle("padding-left: 55px;");
		lwDiv.appendChild(graphDiv);
		return lwDiv;
	}

	private Component drawServerWaitGraph() {
		Div serverWaitDiv = new Div();
		serverWaitDiv.setSclass(CustomizationConstants.GRAPH_BORDER);
		serverWaitDiv.appendChild(getGraphLabel(CustomizationConstants.SERVER_WAITS_TITLE));
		Div graphDiv =  new Div();
		serverWaitsChart1 = getIderaLineChart();
		//serverWaitsChart1.refresh();
		IderaLineChartModel chartModel = new IderaLineChartModel();
		
		//Set<String> keys = new HashSet<>();
		for(ServerWait serverWait : serverWaitList){
			Double waitTime = getWaitTime(serverWait);
			//Adding offset for graphs
			long collectionTime = serverWait.getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
			chartModel.setValue(serverWait.getCategory(), collectionTime, waitTime);
		}
		serverWaitsChart1.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.ALL_WAIT_TYPES_IN_MS));
		if(serverWaitList.size()>1){
			serverWaitsChart1.setModel(chartModel);
			serverWaitsChart1.getChart().setDrillable(true);
			setXAxisDomain(serverWaitsChart1);
			
		}else{
			serverWaitsChart1.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		}
		graphDiv.appendChild(serverWaitsChart1);
		graphDiv.setStyle("padding-left: 55px;cursor:pointer");
		graphDiv.addEventListener("onClick", new EventListener<Event>() {
			@Override
			public void onEvent(Event arg0) throws Exception {
				launchToResources(InstanceSubCategoriesTab.RESOURCES_SERVERWAITS.getSubTabId());
			}
		});
		serverWaitDiv.appendChild(graphDiv);
		return serverWaitDiv;
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


	private Component drawSessionsGraph() {
		Div sessionsDiv = new Div();
		sessionsDiv.setSclass(CustomizationConstants.GRAPH_BORDER);
		sessionsDiv.appendChild(getGraphLabel(CustomizationConstants.BLOCKED_SESSION_TITLE));
		Div graphDiv =  new Div();
		blockedSessionChart = getIderaAreaChart();
		//blockedSessionChart.refresh();
		IderaAreaChartModel bsModel = new IderaAreaChartModel();
		
		try {
			Object instanceSessionsChartObj = Executions.getCurrent().getDesktop().getAttribute(SQLdmConstants.DASHBOARD_SCOPE_SINGLE_INSTANCE_SESSION_CHART);
			sessionDetails = (List<SessionGraphDetail>) instanceSessionsChartObj;
			if (sessionDetails != null && sessionDetails.size() >1){
				for (int i = 0 ; i < sessionDetails.size(); i++) {
					SessionGraphDetail detail = sessionDetails.get(i);
					//Adding offset for graphs
					long collectionTime = detail.getUTCCollectionDateTime().getTime() + Utility.cancelOffSetInMillis();
					bsModel.setValue("Blocked Sessions", collectionTime, detail.getBlockedCount());
					bsModel.setValue("Lead Blockers", collectionTime, detail.getLeadBlockers());
					bsModel.setValue("Total Deadlocks", collectionTime, detail.getTotalDeadLock());
					bsModel.setValue("Active", collectionTime, detail.getActiveSessionCount());
				}
				blockedSessionChart.setModel(bsModel);
			/*	blockedSessionChart.getChart().setXAxisLegendSpacing(new Integer(35));
				blockedSessionChart.getChart().setXAxisTickCount(4);
				*/blockedSessionChart.getChart().setDrillable(true);
				setXAxisDomain(blockedSessionChart);
				
			}else{
				blockedSessionChart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
			}
			
		} catch (Exception x) {
			blockedSessionChart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.ERROR_OCCURRED_LOADING_CHART));
		}
		graphDiv.appendChild(blockedSessionChart);
		graphDiv.setStyle("padding-left: 55px;cursor:pointer");
		sessionsDiv.appendChild(graphDiv);
		sessionsDiv.addEventListener("onClick", new EventListener<Event>() {

			@Override
			public void onEvent(Event arg0) throws Exception {
				launchToSessions();
			}
		});
		return sessionsDiv;
	}


	private void SetGraphInvisible(IderaChart lineChart) {
		if(lineChart!=null){
			lineChart.setVisible(false);
		}
	}

	private Div drawLockWaitsGraph() {
		Div lwDiv = new Div();
		lwDiv.setSclass(CustomizationConstants.GRAPH_BORDER);
		Vlayout vLay = new Vlayout();
		Hlayout hLay = new Hlayout();
		Div lableDiv = new Div();
		lableDiv.appendChild(getGraphLabel(CustomizationConstants.LOCK_WAITS_TITLE));
		vLay.appendChild(lableDiv);
		Div graphDiv =  new Div();
		//graphDiv.appendChild(vLay);
		lockWaits = getIderaLineChart();
		//lockWaits.refresh();
		CategoryModel model = modelsMap.get(CustomizationConstants.LOCK_WAITS_MODEL);
		if(model != null){
			lockWaits.setModel(model);
			lockWaits.getChart().setDrillable(true);
			setXAxisDomain(lockWaits);
		}
		else{
			lockWaits.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		}
		graphDiv.appendChild(lockWaits);
		//graphDiv.setStyle("padding-left: 55px;");
		hLay.appendChild(graphDiv);
		vLay.appendChild(hLay);
		lwDiv.appendChild(vLay);
		return lwDiv;

	}

	private Div drawDiskGraph() {
		Div diskDiv = new Div();
		diskDiv.setSclass(CustomizationConstants.GRAPH_BORDER);
		Vlayout vlayout = new Vlayout();
		Hlayout headerLayout = new Hlayout();
		headerLayout.appendChild(getGraphLabel(CustomizationConstants.DISK_TITLE));
		Space space = new Space();
		space.setSpacing("30px");
		headerLayout.appendChild(space);
		final Combobox sb =  new Combobox();
		sb.setReadonly(true);
		Div graphDiv =  new Div();
		//graphDiv.setStyle("cursor:pointer");
		Div div1 =  new Div();
		Div div2 =  new Div();
		Div div3 = new Div();
		div1.setStyle("padding-left: 55px;");
		div2.setStyle("padding-left: 55px;");
		div3.setStyle("padding-left: 55px;cursor:pointer");
		diskTotalTime = getIderaLineChart();
		diskAvgTime = getIderaLineChart();
		if(diskVsTotalTimeModel!= null){
			Set<String> diskList = diskVsTotalTimeModel.keySet();

			final ListModelList<String> model = new ListModelList<String>(Utility.sortAsCollection(diskList));
			//sb.setModel(model);
			for(int i = 0; i<model.size();i++){
				   sb.appendItem(model.get(i));
				  }
			if(model.size()>0){
				sb.setSelectedIndex(0);
				selectedDisk = sb.getSelectedItem().getLabel();
			}
			sb.addEventListener("onChange", (new EventListener<Event>(){
				public void onEvent(Event event) throws Exception {
					selectedDisk= sb.getSelectedItem().getLabel();
					//selectedDisk = strList.iterator().next();
					redrawDiskGraphs(selectedDisk);
				}
			}));

			headerLayout.appendChild(sb);
			vlayout.appendChild(headerLayout);

			redrawDiskGraphs(selectedDisk);
		}
		div1.appendChild(diskTotalTime);
		div2.appendChild(diskAvgTime);

		sqlPhysicalIO = getIderaLineChart();
		CategoryModel model = modelsMap.get(CustomizationConstants.DISK_PHYSICAL_IO_MODEL);
		if(model != null){
			sqlPhysicalIO.setModel(model);
			sqlPhysicalIO.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESOURCES_SQL_SERVER_PHYSICAL_IO));
			sqlPhysicalIO.getChart().setDrillable(true);
			setXAxisDomain(sqlPhysicalIO);
		}
		else{
			sqlPhysicalIO.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		}
		div3.appendChild(sqlPhysicalIO);
		
		graphDiv.appendChild(div1); graphDiv.appendChild(div2); graphDiv.appendChild(div3);
		div3.addEventListener("onClick", new EventListener<Event>() {
			@Override
			public void onEvent(Event arg0) throws Exception {
				launchToResources(InstanceSubCategoriesTab.RESOURCES_DISKVIEW.getSubTabId());
			}
		});
		vlayout.appendChild(graphDiv);

		diskDiv.appendChild(vlayout);
		return diskDiv;
	}

	private void redrawDiskGraphs(String selectedDisk) {
		// TODO Auto-generated method stub
		if(diskVsTotalTimeModel.get(selectedDisk)!=null){

			diskTotalTime.setModel(diskVsTotalTimeModel.get(selectedDisk));
			diskTotalTime.setTitle(CustomizationConstants.DISK_TOTAL_TIME_GRAPH_NAME);
			diskTotalTime.getChart().setDrillable(true);
			setXAxisDomain(diskTotalTime);
			
			diskAvgTime.setModel(diskVsAvgTimeModel.get(selectedDisk));
			diskAvgTime.setTitle(CustomizationConstants.DISK_AVG_TIME_GRAPH_NAME);
			diskAvgTime.getChart().setDrillable(true);
			setXAxisDomain(diskAvgTime);
		}
		else
		{
			diskTotalTime.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
			diskAvgTime.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		}	
	}

	private Div drawCacheGraph() {
		Div cachediv = new Div();
		cachediv.setSclass(CustomizationConstants.GRAPH_BORDER);
		Vlayout vLay = new Vlayout();
		Div lableDiv = new Div();
		lableDiv.appendChild(getGraphLabel(CustomizationConstants.CACHE_TITLE));
		vLay.appendChild(lableDiv);
		cachediv.appendChild(vLay);
		Vlayout hLayout = new Vlayout();
		Div div1 = new Div();
		div1.setHeight(CustomizationConstants.GRAPH_CONTAINER_DIV_HEIGHT);
		Div div2 = new Div();
		div2.setHeight(CustomizationConstants.GRAPH_CONTAINER_DIV_HEIGHT);
		pageLifeExpectancy = getIderaAreaChart();
		//pageLifeExpectancy.refresh();
	
		pageLifeExpectancy.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESOURCES_PAGE_LIFE_EXPECTANCY));
		pageLifeExpectancy.getChart().setDrillable(true);
		if(pageLifeExpectancy.getChart() != null && (modelsMap.get(CustomizationConstants.CACHE_PAGE_LIFE_MODEL) != null)){
			pageLifeExpectancy.setModel(modelsMap.get(CustomizationConstants.CACHE_PAGE_LIFE_MODEL));
			pageLifeExpectancy.getChart().setXAxisLegendSpacing(new Integer(35));
			pageLifeExpectancy.getChart().setDrillable(true);
			setXAxisDomain(pageLifeExpectancy);
	/*		pageLifeExpectancy.getChart().setXAxisTickCount(4);*/
		}
		else{
			pageLifeExpectancy.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
			
		}

		memoryAreas = getIderaLineChart();
		//memoryAreas.refresh();
		CategoryModel model = modelsMap.get(CustomizationConstants.CACHE_MEMORY_AREAS_MODEL);
		
		if(model != null){
			memoryAreas.setModel(model);
			memoryAreas.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESOURCES_MEMEORY_AREAS_IN_MB));
			memoryAreas.getChart().setDrillable(true);
			memoryAreas.getChart().setXAxisLegendSpacing(new Integer(35));
			setXAxisDomain(memoryAreas);
		}
		else{
			memoryAreas.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		}
		
		div1.appendChild(pageLifeExpectancy);
		div2.appendChild(memoryAreas);
		div1.setStyle("padding-left: 55px;");
		div2.setStyle("padding-left: 55px;");
		hLayout.appendChild(div1);
		hLayout.appendChild(div2);
		hLayout.setStyle("cursor:pointer");
		cachediv.appendChild(hLayout);
		cachediv.addEventListener("onClick", new EventListener<Event>() {
			@Override
			public void onEvent(Event arg0) throws Exception {
				launchToResources(InstanceSubCategoriesTab.RESOURCES_MEMORYVIEW.getSubTabId());
			}
		});
		return cachediv;
	}

	private Div drawMemoryGraph() {
		Div memoryDiv =  new Div();
		memoryDiv.setSclass(CustomizationConstants.GRAPH_BORDER);
		Vlayout graphVlayout = new Vlayout();
		//graphVlayout.setStyle("cursor:pointer");
		Vlayout vLay = new Vlayout();
		Div lableDiv = new Div();
		lableDiv.appendChild(getGraphLabel(CustomizationConstants.MEMORY_TITLE));
		vLay.appendChild(lableDiv);
		memoryDiv.appendChild(vLay);
		Div div1 = new Div();
		Div div2 = new Div();
		sqlMemoryUsage = getIderaLineChart();
		//sqlMemoryUsage.refresh();
		CategoryModel model = modelsMap.get(CustomizationConstants.SQL_CPU_MEMORY_USAGE_MODEL);
		if(model!=null){
			sqlMemoryUsage.setModel(model);
			sqlMemoryUsage.getChart().setDrillable(true);
			setXAxisDomain(sqlMemoryUsage);
		}
		else
			sqlMemoryUsage.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		sqlMemoryUsage.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESOURCES_SQL_MEMORY_USAGE_IN_MB));
	
		div1.appendChild(sqlMemoryUsage);
		
		sqlOSPaging = getIderaLineChart();
		//sqlOSPaging.refresh();
		CategoryModel model2 = modelsMap.get(CustomizationConstants.MEMORY_OS_PAGING_MODEL);
		if(model2!=null){
			sqlOSPaging.setModel(model2);
			sqlOSPaging.getChart().setDrillable(true);
			setXAxisDomain(sqlOSPaging);
		}
		else{
			sqlOSPaging.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		}
	
		sqlOSPaging.setTitle(CustomizationConstants.MEMORY_PAGING_GRAPH_NAME);
	
		div2.appendChild(sqlOSPaging);
		div1.setStyle("padding-left: 55px;cursor:pointer");
		div2.setStyle("padding-left: 55px;");
		graphVlayout.appendChild(div1);
		graphVlayout.appendChild(div2);
		memoryDiv.appendChild(graphVlayout);
		div1.addEventListener("onClick", new EventListener<Event>() {
			@Override
			public void onEvent(Event arg0) throws Exception {
				launchToResources(InstanceSubCategoriesTab.RESOURCES_MEMORYVIEW.getSubTabId());
			}
		});
		return memoryDiv;

	}
	
	private Div drawTempDbGraph() {
		Div tempDbGraph =  new Div();
		tempDbGraph.setSclass(CustomizationConstants.GRAPH_BORDER);
		Vlayout vlayout = new Vlayout();
		//vlayout.setStyle("cursor:pointer");
		vlayout.appendChild(getGraphLabel(CustomizationConstants.TEMPDB_TITLE));
		Div div1 = new Div();
		Div div2 = new Div();
		
		tempdbUsedByTime = getIderaLineChart();
		//tempdbUsedByTime.refresh();
		CategoryModel model = modelsMap.get(CustomizationConstants.TEMPDB_USAGE_MODEL);
		if(model!=null){
			tempdbUsedByTime.setModel(model); 	
			tempdbUsedByTime.getChart().setDrillable(true);
			setXAxisDomain(tempdbUsedByTime);
		} else{
			tempdbUsedByTime.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		}
		
		tempdbUsedByTime.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.TEMP_DB_BY_TIME_MB));
		
		div1.appendChild(tempdbUsedByTime);

		tempDbPagesWaitTime = getIderaLineChart();
		//tempDbPagesWaitTime.refresh();
		CategoryModel model2= modelsMap.get(CustomizationConstants.TEMPDB_PAGING_MODEL);
		if(model2!=null){
			tempDbPagesWaitTime.setModel(model2);
			tempDbPagesWaitTime.getChart().setDrillable(true);
			setXAxisDomain(tempDbPagesWaitTime);
		}
		else{
			tempDbPagesWaitTime.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		}
		
		tempDbPagesWaitTime.setTitle(CustomizationConstants.TEMPDB_PAGING_GRAPH_NAME);
		
		div2.appendChild(tempDbPagesWaitTime);
		div1.setStyle("padding-left: 55px;cursor:pointer;");
		div2.setStyle("padding-left: 55px;");
		vlayout.appendChild(div1);
		vlayout.appendChild(div2);
		tempDbGraph.appendChild(vlayout);
		div1.addEventListener("onClick", new EventListener<Event>() {
			@Override
			public void onEvent(Event arg0) throws Exception {
				launchToDatabases(InstanceSubCategoriesTab.DATABASES_TEMPDB.getSubTabId());
			}
		});
		return tempDbGraph;

	}
	
	private Div drawCPUGraph() {
		Div cpuDiv = new Div();
		cpuDiv.setSclass(CustomizationConstants.GRAPH_BORDER);
		Vlayout hLay = new Vlayout();
		Div lableDiv = new Div();
		lableDiv.appendChild(getGraphLabel(CustomizationConstants.CPU_TITLE));
		hLay.appendChild(lableDiv);
		cpuDiv.appendChild(hLay);
		Div graphDiv =  new Div();
		graphDiv.setSclass(CustomizationConstants.GRAPH_BORDER);
		Div sqlCpuGraphDiv =  new Div();
		Div callRatesGraphDiv =  new Div();
		Vlayout vLay  =  new Vlayout();
		sqlCpuUsage = getIderaLineChart();
		sqlCpuUsage.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESOURCES_CPU_USAGE_PERCENTAGE));
		//sqlCpuUsage.refresh();
		if(sqlCpuUsage.getChart()!=null && modelsMap.get(CustomizationConstants.SQL_CPU_MODEL) != null){
			sqlCpuUsage.setModel(modelsMap.get(CustomizationConstants.SQL_CPU_MODEL));
			sqlCpuUsage.getChart().setyAxisCustomMaxDomainValue(true);
			sqlCpuUsage.getChart().setyAxisCustomMinDomainValue(true);
			sqlCpuUsage.getChart().setyAxisMinDomainValue(0);
			sqlCpuUsage.getChart().setyAxisMaxDomainValue(100);
			sqlCpuUsage.getChart().setDrillable(true);
			setXAxisDomain(sqlCpuUsage);
		}
		else {
			sqlCpuUsage.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));			
		}

		cpuCallRateStats = getIderaLineChart();
		//cpuCallRateStats.refresh();
		cpuCallRateStats.setTitle(CustomizationConstants.CPU_CALL_RATES_GRAPH_NAME);
		cpuCallRateStats.setModel(modelsMap.get(CustomizationConstants.CPU_CALL_RATES_MODEL));
		if(cpuCallRateStats.getChart()!=null && modelsMap.get(CustomizationConstants.CPU_CALL_RATES_MODEL) != null){
			cpuCallRateStats.getChart().setyAxisCustomMaxDomainValue(true);
			cpuCallRateStats.getChart().setyAxisCustomMinDomainValue(true);
			cpuCallRateStats.getChart().setyAxisMinDomainValue(0);
			cpuCallRateStats.getChart().setDrillable(true);
			setXAxisDomain(cpuCallRateStats);
		}
		else {
			cpuCallRateStats.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));			
		}
		sqlCpuGraphDiv.appendChild(sqlCpuUsage);
		sqlCpuGraphDiv.setStyle("cursor:pointer;");
		vLay.appendChild(sqlCpuGraphDiv);
		//cpuDiv.appendChild(sqlCpuUsage);
		callRatesGraphDiv.appendChild(cpuCallRateStats);
		vLay.appendChild(callRatesGraphDiv);
		graphDiv.appendChild(vLay);
		graphDiv.setStyle("padding-left: 55px;");
		cpuDiv.appendChild(graphDiv);
		sqlCpuGraphDiv.addEventListener("onClick", new EventListener<Event>() {

			@Override
			public void onEvent(Event arg0) throws Exception {
				launchToResources(InstanceSubCategoriesTab.RESOURCES_CPUVIEW.getSubTabId());
			}
		});
		return cpuDiv;
	}

	public Date getFromDate() {
		return fromDate;
	}

	public void setFromDate(Date fromDate) {
		this.fromDate = fromDate;
		BindUtils.postNotifyChange(null, null, this, "fromDate");
	}

	public Date getEndDate() {
		return endDate;
	}

	public void setEndDate(Date endDate) {
		this.endDate = endDate;
		BindUtils.postNotifyChange(null, null, this, "endDate");
	}

	public Date getFromTime() {
		return fromTime;
	}

	public void setFromTime(Date fromTime) {
		this.fromTime = fromTime;
		BindUtils.postNotifyChange(null, null, this, "fromTime");
	}

	public Date getEndTime() {
		return endTime;
	}

	public void setEndTime(Date endTime) {
		this.endTime = endTime;
		BindUtils.postNotifyChange(null, null, this, "endTime");
	}

	public ListModelList<GraphCategories> getGraphCategoryOptions() {
		return graphCategoryOptions;
	}

	public void setGraphCategoryOptions(ListModelList<GraphCategories> categoryOptions) {
		this.graphCategoryOptions = categoryOptions;
		BindUtils.postNotifyChange(null, null, this, "graphCategoryOptions");
	}
	public Constraint getOrderConstraint() {

		Constraint ctt = new Constraint() {
			public void validate(Component comp, Object value)
					throws WrongValueException {

				if (value == null)
					throw new WrongValueException(comp,
							"Enter a valid number between 1 and "+ graphCategoryOptions.size());
				try {
					int orderValue = Integer.parseInt((String)value);
					if(orderValue < 1 || orderValue > graphCategoryOptions.size())
						throw new WrongValueException(comp,
								"Enter a valid number between 1 and "+graphCategoryOptions.size());

				} catch(Exception e) {
					log.error("[getOrderConstraint]: " +e.getMessage());
					throw new WrongValueException(comp,
							"Enter a valid number between 1 and "+graphCategoryOptions.size());
				}
			}
		};
		return ctt;
	}

	@Listen("onClick = #applyButton")
	public void filterByCategory() {

		int numCategories = graphCategoryOptions.getSize();
		ListModelList<GraphCategories> tmpGrapgCateOptions = new ListModelList<GraphCategories>() ;//= graphCategoryOptions;

		//graphCategoryOptions.clear();
		Rows rows = categoryList.getRows();
		Iterator<Component> rowItr = rows.getChildren().iterator();
		while(rowItr.hasNext()){
			Component rowComp = rowItr.next();
			if(rowComp instanceof Row){
				Row row = (Row)rowComp;
				List<Component> cols =  row.getChildren();
				Textbox  orderComp = (Textbox)cols.get(0);
				Checkbox isVis = (Checkbox)cols.get(1);
				Label lbl = (Label)cols.get(2);
				GraphCategories gc = setNewCustomizedValues(lbl.getValue(),orderComp.getValue(),isVis.isChecked());
				if(gc!=null){
					tmpGrapgCateOptions.add(gc);
				}
			}
		}
		graphCategoryOptions = tmpGrapgCateOptions;

		boolean isOrderingValid = isOrderingValid();
		if (!isOrderingValid) {
			for (int i = 0; i < numCategories; i++) {
				GraphCategories graphCategoryOption = graphCategoryOptions
						.getElementAt(i);
				
				graphCategoryOption.setTempPosition(graphCategoryOption.getPosition());
			}
			Messagebox.show(
					"Invalid Ordering! Order should have distinct values between 1 and "
							+ numCategories + ".", "Error", Messagebox.OK,
					Messagebox.ERROR, null);
		}

		graphCategoryOptions.sort(new Comparator<GraphCategories>() {

			@Override
			public int compare(GraphCategories o1,
					GraphCategories o2) {
				return o1.getTempPosition() - o2.getTempPosition();
			}
		}, true);

		updateSession();
		refreshView();
		manageGraphPopup.close();
	
	}

	private GraphCategories setNewCustomizedValues(String graphName, String newOrder,
			boolean isChecked) {
		GraphCategories newCat = null;// = new GraphCategories();
		Boolean validNumericFlag = true;
		for(int i=0; i<graphCategoryOptions.size(); i++){
			GraphCategories graphCate = graphCategoryOptions.get(i);
			if(graphCate.getCategory().getCategoryName().equalsIgnoreCase(graphName)){
				newCat = new GraphCategories(graphCate.getCategory());
				newCat.setPosition(graphCate.getPosition());
				try{
					newCat.setTempPosition(Integer.valueOf(newOrder));
				}catch(Exception x){
					validNumericFlag = false ;
					newCat.setTempPosition(graphCate.getPosition());
				}

				newCat.setVisible(isChecked);
				break;
			}
		}
		if(!validNumericFlag){
			Messagebox.show(
					"Invalid Character! Please enter a valid numeric value", "Error", Messagebox.OK,
							Messagebox.ERROR, null);
		}
		return newCat;
	}

	@Listen("onClick = #cancelButton")
	public void cancelFiltering() {
		BindUtils.postNotifyChange(null, null, this, "graphCategoryOptions");
		manageGraphPopup.close();
	}

	/**
	 * Validates the ordering selected by user
	 * @return
	 */
	private boolean isOrderingValid() {

		int numCategories = graphCategoryOptions.getSize();
		boolean validatePosition[] = new boolean[numCategories];

		for (int i = 0; i < numCategories; i++) 
			validatePosition[i] = false;

		for (int i = 0; i < numCategories; i++) {
			GraphCategories categoryOption = graphCategoryOptions
					.getElementAt(i);
			int tempPosition = categoryOption.getTempPosition();
			if (tempPosition <= 0 || tempPosition > numCategories
					|| validatePosition[tempPosition - 1])
				return false;

			validatePosition[categoryOption.getTempPosition() - 1] = true;
		}
		return true;
	}
	
	/**
	 * Combines from date and from time values into a single date object
	 * 
	 * @return
	 */
	private Date getFromDateTime() {
		Calendar fromCalendar = Calendar.getInstance();
		fromCalendar.setTime(fromDate);

		Calendar timeCal = Calendar.getInstance();
		timeCal.setTime(fromTime);

		fromCalendar.set(Calendar.HOUR_OF_DAY,
				timeCal.get(Calendar.HOUR_OF_DAY));
		fromCalendar.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));

		return fromCalendar.getTime();
	}

	/**
	 * Combines end date and end time values into a single date object
	 * 
	 * @return
	 */
	private Date getEndDateTime() {
		Calendar endCalendar = Calendar.getInstance();
		endCalendar.setTime(endDate);

		Calendar timeCal = Calendar.getInstance();
		timeCal.setTime(endTime);

		endCalendar
				.set(Calendar.HOUR_OF_DAY, timeCal.get(Calendar.HOUR_OF_DAY));
		endCalendar.set(Calendar.MINUTE, timeCal.get(Calendar.MINUTE));

		return endCalendar.getTime();
	}


	private void refreshOnEventChange() {
		refreshView(); 
		
	}
	
	public void launchToResources(int subCategoryTabViewId) throws IOException, RestException{
		SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
        pref.setSelectedCategory(InstanceCategoryTab.RESOURCES.getId());
        pref.setSelectedSubCategory(subCategoryTabViewId);
        PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);
        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance"+"/"+instanceId));
	}
	
	private void launchToDatabases(int subTabId)  throws IOException, RestException{
		SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
        pref.setSelectedCategory(InstanceCategoryTab.DATABASES.getId());
        pref.setSelectedSubCategory(subTabId);
        PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);
        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance"+"/"+instanceId));
	}
	
	
	
	private IderaLineChart getIderaLineChart(){
		IderaLineChart lineChart = new IderaLineChart();
		lineChart.setWidth(CustomizationConstants.GRAPH_WIDTH);
		lineChart.setHeight(CustomizationConstants.GRAPH_HEIGHT);
		lineChart.setLeftMargin(CustomizationConstants.GRAPH_LEFT_MARGIN);
		lineChart.setTopMargin(CustomizationConstants.GRAPH_TOP_MARGIN);
		lineChart.setBottomMargin(CustomizationConstants.GRAPH_BOTTOM_MARGIN);
		lineChart.setRightMargin(CustomizationConstants.GRAPH_RIGHT_MARGIN);
		lineChart.getChart().setPlotMarginRight(CustomizationConstants.GRAPH_RIGHT_PADDING);
		return lineChart;
	}
	
	private IderaAreaChart getIderaAreaChart(){
		IderaAreaChart areaChart = new IderaAreaChart();
		areaChart.setWidth(CustomizationConstants.GRAPH_WIDTH);
		areaChart.setHeight(CustomizationConstants.GRAPH_HEIGHT);
		areaChart.setLeftMargin(CustomizationConstants.GRAPH_LEFT_MARGIN);
		areaChart.setTopMargin(CustomizationConstants.GRAPH_TOP_MARGIN);
		areaChart.setBottomMargin(CustomizationConstants.GRAPH_BOTTOM_MARGIN);
		areaChart.setRightMargin(CustomizationConstants.GRAPH_RIGHT_MARGIN);
		areaChart.getChart().setPlotMarginRight(CustomizationConstants.GRAPH_RIGHT_PADDING);
		return areaChart;
	}
	
	private  void setXAxisDomain(IderaChart chart){
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		Date fromDate = pref.getFromDateTime();
		Date toDate= pref.getToDateTime();
		chart.getChart().setxAxisCustomMinDomainValue(true);
		chart.getChart().setxAxisCustomMaxDomainValue(true);
		chart.getChart().setxAxisMinDomainValue(fromDate);
		chart.getChart().setxAxisMaxDomainValue(toDate);
	}
	
	private Label getGraphLabel(String title){
		Label lbl = new Label(title);
		lbl.setStyle(CustomizationConstants.GRAPH_LABEL_STYLE);
		return lbl;
	}
}
