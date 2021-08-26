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
import com.idera.sqldm.data.category.resources.DiskIOPerSecondEntry;
import com.idera.sqldm.data.category.resources.ResourceCategory;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.ui.components.charts.line.IderaLineChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.ui.preferences.HistoryPanelPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.topten.TopXComposer;

@SuppressWarnings("rawtypes")
public class DiskViewComposer extends TopXComposer {
	
	private static final long serialVersionUID = -6898380788070752972L;
	@Wire IderaLineChart diskReads;
	@Wire IderaLineChart diskWrites;
	@Wire IderaLineChart diskTransfers;
	@Wire IderaLineChart sqlPhysicalIO;
	
	@Wire Spinner sessionsCpuCount;
	@Wire Button applySessionsCPUConfig;
	@Wire Popup sessionsCPUWidgetPopup;	

	@Wire A Reads12;	
	@Wire A Reads8;	
	@Wire A Reads4;	
	@Wire A Reads1;	
	@Wire A Reads30;	

	@Wire A Writes12;	
	@Wire A Writes8;	
	@Wire A Writes4;	
	@Wire A Writes1;	
	@Wire A Writes30;	

	@Wire A Transfers12;	
	@Wire A Transfers8;	
	@Wire A Transfers4;	
	@Wire A Transfers1;	
	@Wire A Transfers30;	

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
		} else {
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
		updateDefaultValue(TopXEnum.INSTANCE_SESSIONS_IO_ACTIVITY, sessionsCpuCount);

		TopXEnum.INSTANCE_SESSIONS_IO_ACTIVITY.setInstanceId(instanceId);
		List<TopXEnum> teList = new ArrayList<>();
		teList.add(TopXEnum.INSTANCE_SESSIONS_IO_ACTIVITY);
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
		diskReads.refresh();
		diskWrites.refresh();
		diskTransfers.refresh();
		sqlPhysicalIO.refresh();
		Map<String, IderaLineChartModel> modelsMap = getModelData(productInstanceName);
		sqlPhysicalIO.setModel(modelsMap.get("PHYSICAL_IO"));
		/*
		 * Author:Accolite
		 * Date : 11th Nov, 2016
		 * Drill  down Capability - SQLDM- 10.2 release
		 */
		sqlPhysicalIO.getChart().setDrillable(true);
		
//		sqlPhysicalIO.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESOURCES_SQL_SERVER_PHYSICAL_IO));

		int numOfMinutes = 60;		
		/*List<ResourceCategory> resourceCategoryList;
		resourceCategoryList= ResourcesFacade.getResources(productInstanceName, instanceId, numOfMinutes);*/
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		List<ResourceCategory> resourceCategoryList = null;
		if(pref.getScaleCombobox().getValue().equals("Custom")){
			Date startTime = pref.getFromDateTime();
			Date endTime = pref.getToDateTime();
			long duration  = endTime.getTime() - startTime.getTime();
			long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
        	//resourceCategoryList = ResourcesFacade.getResources(productInstanceName,instanceId, numOfMinutes);
        	resourceCategoryList = ResourcesFacade.getResources(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
        }
        else{
        	resourceCategoryList = ResourcesFacade.getResources(productInstanceName,instanceId, pref.getNumHistoryMinutes());
        }
		refreshDiskReads(resourceCategoryList, numOfMinutes);
		refreshDiskWrites(resourceCategoryList, numOfMinutes);
		refreshDiskTransfers(resourceCategoryList, numOfMinutes);

/*		diskReads.setModel(modelsMap.get("DISK_READS"));
//		diskReads.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESOURCES_DISK_READS_PER_SECOND));
		
		diskWrites.setModel(modelsMap.get("DISK_WRITES"));
//		diskWrites.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESOURCES_DISK_WRITES_PER_SECOND));
		
		diskTransfers.setModel(modelsMap.get("DISK_TRANSFERS"));
*/		
	}
	
	private Map<String, IderaLineChartModel> getModelData(String productInstanceName) throws CategoryException {
		/*Calendar startCal = Calendar.getInstance();
		startCal.set(2012, 9, 15);*/
		//List<ResourceCategory> resourceCategoryList = ResourcesFacade.getResources(productInstanceName, instanceId, 30);
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		List<ResourceCategory> resourceCategoryList = null;
		if(pref.getScaleCombobox().getValue().equals("Custom")){
			Date startTime = pref.getFromDateTime();
			Date endTime = pref.getToDateTime();
			long duration  = endTime.getTime() - startTime.getTime();
			long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
        	//resourceCategoryList = ResourcesFacade.getResources(productInstanceName,instanceId, 30);
        	resourceCategoryList = ResourcesFacade.getResources(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
        }
        else{
        	resourceCategoryList = ResourcesFacade.getResources(productInstanceName,instanceId, pref.getNumHistoryMinutes());
        }
		Map<String, IderaLineChartModel> modelsMap = new HashMap<String, IderaLineChartModel>();
		
		if(resourceCategoryList !=null && resourceCategoryList.size() >1){
			IderaLineChartModel physicalIOModel = new IderaLineChartModel();

			modelsMap.put("PHYSICAL_IO", physicalIOModel);

			for(int i=resourceCategoryList.size()-1; i>=0; i--){
				ResourceCategory resource = resourceCategoryList.get(i);
				//Adding Offset for graphs
				long collectionTime = resource.getUTCCollectionDateTime().getTime() +OFFSET_MILLIS;
				physicalIOModel.setValue("Checkpoint Writes", collectionTime, resource.getDisk().getSqlPhysicalIO().get("CheckPointWrites"));
				physicalIOModel.setValue("Lazy Writer Writes", collectionTime, resource.getDisk().getSqlPhysicalIO().get("LazyWriterWrites"));
				physicalIOModel.setValue("Read Ahead Reads", collectionTime, resource.getDisk().getSqlPhysicalIO().get("ReadAheadPages"));
				physicalIOModel.setValue("Page Reads", collectionTime, resource.getDisk().getSqlPhysicalIO().get("PageReads"));
				physicalIOModel.setValue("Page Writes", collectionTime, resource.getDisk().getSqlPhysicalIO().get("PageWrites"));
			}
		}else{
			
			setEmptyCharts();
		}
		return modelsMap;
	}
	
	private void setEmptyCharts() {
		diskReads.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		diskWrites.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		diskTransfers.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		sqlPhysicalIO.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		
	}
	
	private void refreshDiskReads(List<ResourceCategory> resourceCategoryList, int numOfMinutes) {
		
		try {
			
			if(resourceCategoryList == null)
				resourceCategoryList = ResourcesFacade.getResources(productInstanceName, instanceId, numOfMinutes);

			if(resourceCategoryList !=null && resourceCategoryList.size() >1){
				IderaLineChartModel diskReadModel = new IderaLineChartModel();

				for(int i=resourceCategoryList.size()-1; i>=0; i--){
					ResourceCategory resource = resourceCategoryList.get(i);
					//Adding Offset for graphs
					long collectionTime = resource.getUTCCollectionDateTime().getTime() + OFFSET_MILLIS;
					for(DiskIOPerSecondEntry reads :resource.getDisk().getDiskReadsPerSecond()){
						diskReadModel.setValue(reads.getKey(), collectionTime, reads.getValue());
					}
				}

				diskReads.setModel(diskReadModel);
				/*
				 * Author:Accolite
				 * Date : 11th Nov, 2016
				 * Drill  down Capability - SQLDM- 10.2 release
				 */
				diskReads.getChart().setDrillable(true);
			} else {
				diskReads.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
			}
		} catch (CategoryException e) {
			e.printStackTrace();
		}
	}

	private void refreshDiskWrites(List<ResourceCategory> resourceCategoryList, int numOfMinutes) {

		try {
			if(resourceCategoryList == null)
				resourceCategoryList = ResourcesFacade.getResources(productInstanceName, instanceId, numOfMinutes);

			if(resourceCategoryList !=null && resourceCategoryList.size() >1){
				IderaLineChartModel diskWriteModel = new IderaLineChartModel();

				for(int i=resourceCategoryList.size()-1; i>=0; i--){
					ResourceCategory resource = resourceCategoryList.get(i);
					//Adding Offset for graphs
					long collectionTime = resource.getUTCCollectionDateTime().getTime() + OFFSET_MILLIS;
					for(DiskIOPerSecondEntry writes :resource.getDisk().getDiskWritesPerSecond()){
						diskWriteModel.setValue(writes.getKey(), collectionTime, writes.getValue());
					}
				}

				diskWrites.setModel(diskWriteModel);
				/*
				 * Author:Accolite
				 * Date : 11th Nov, 2016
				 * Drill  down Capability - SQLDM- 10.2 release
				 */
				diskWrites.getChart().setDrillable(true);
			} else {
				diskWrites.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
			}
		} catch (CategoryException e) {
			e.printStackTrace();
		}
	}

	private void refreshDiskTransfers(List<ResourceCategory> resourceCategoryList, int numOfMinutes) {

		try {
			if(resourceCategoryList == null)
				resourceCategoryList = ResourcesFacade.getResources(productInstanceName, instanceId, numOfMinutes);

			if(resourceCategoryList !=null && resourceCategoryList.size() >1){
				IderaLineChartModel diskTransfersModel = new IderaLineChartModel();

				for(int i=resourceCategoryList.size()-1; i>=0; i--){
					ResourceCategory resource = resourceCategoryList.get(i);
					//Adding Offset for graphs
					long collectionTime = resource.getUTCCollectionDateTime().getTime() + OFFSET_MILLIS;
					for(DiskIOPerSecondEntry transfers :resource.getDisk().getDiskTransfersPerSecond()){
						diskTransfersModel.setValue(transfers.getKey(), collectionTime, transfers.getValue());
					}
				}
				diskTransfers.setModel(diskTransfersModel);
				/*
				 * Author:Accolite
				 * Date : 11th Nov, 2016
				 * Drill  down Capability - SQLDM- 10.2 release
				 */
				diskTransfers.getChart().setDrillable(true);
			}else{
				diskTransfers.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
			}
		} catch (CategoryException e) {
			e.printStackTrace();
		}
	}

	@Listen("onClick = #applySessionsCPUConfig")
	public void refreshSESWidget(){
		publishWidgetSettings(TopXEnum.INSTANCE_SESSIONS_IO_ACTIVITY, null, sessionsCpuCount, sessionsCPUWidgetPopup);
	}
	
	/*@Listen("onClick = #diskReadsDiv > a, #diskWritesDiv > a, #diskTransfersDiv > a")
	public void updateGraph(Event event) {
		String widgetType = event.getTarget().getParent().getId();
		
		String targetName = event.getTarget().getId();
		A link = (A) event.getTarget();
		int numOfMinutes;

		String defaultStyle = "vertical-align: -webkit-baseline-middle;";
		
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
		
		if(widgetType.equals("diskReadsDiv")) {
			
			Reads12.setStyle(defaultStyle + "font-weight:normal");
			Reads8.setStyle(defaultStyle + "font-weight:normal");
			Reads4.setStyle(defaultStyle + "font-weight:normal");
			Reads1.setStyle(defaultStyle + "font-weight:normal");
			Reads30.setStyle(defaultStyle + "font-weight:normal");
			
			refreshDiskReads(null, numOfMinutes);
			
		} else if(widgetType.equals("diskWritesDiv")) {
			
			Writes12.setStyle(defaultStyle + "font-weight:normal");
			Writes8.setStyle(defaultStyle + "font-weight:normal");
			Writes4.setStyle(defaultStyle + "font-weight:normal");
			Writes1.setStyle(defaultStyle + "font-weight:normal");
			Writes30.setStyle(defaultStyle + "font-weight:normal");
			
			refreshDiskWrites(null, numOfMinutes);
		
		} else if(widgetType.equals("diskTransfersDiv")) {
		
			Transfers12.setStyle(defaultStyle + "font-weight:normal");
			Transfers8.setStyle(defaultStyle + "font-weight:normal");
			Transfers4.setStyle(defaultStyle + "font-weight:normal");
			Transfers1.setStyle(defaultStyle + "font-weight:normal");
			Transfers30.setStyle(defaultStyle + "font-weight:normal");
			
			refreshDiskTransfers(null, numOfMinutes);
		
		}  
		  
		link.setStyle(defaultStyle + "font-weight: bold");

	}*/
	
}
