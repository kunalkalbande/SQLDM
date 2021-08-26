package com.idera.sqldm.ui.dashboard;

import java.io.IOException;
import java.text.DecimalFormat;
import java.util.LinkedList;
import java.util.List;

import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.A;
import org.zkoss.zul.Div;
import org.zkoss.zul.Image;
import org.zkoss.zul.Label;
import org.zkoss.zul.Span;
import org.zkoss.zul.Vlayout;

import com.idera.common.rest.RestException;
import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.DashboardInstance;
import com.idera.sqldm.data.DashboardInstanceFacade;
import com.idera.sqldm.data.InstanceException;
import com.idera.sqldm.data.ResourcesFacade;
import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.data.category.resources.ResourceCategory;
import com.idera.sqldm.data.instances.ServerStatus;
import com.idera.sqldm.data.instances.StatisticsHistory;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.rest.SQLDMRestClient;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.server.web.WebUtil;
import com.idera.sqldm.ui.components.charts.line.IderaLineChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.utils.Utility;

public class ServerDetailsPopupComposer extends SelectorComposer<Component>{
	private static final long serialVersionUID = 9200286224241425901L;
	@Wire private IderaLineChart cpuUsage;
	@Wire private IderaLineChart memoryUsage;
	@Wire private IderaLineChart ioActivity;
	@Wire private IderaLineChart responseTime;
	@Wire private Label instanceName,healthIdx;
	@Wire private Span tagLabel;
	@Wire private Vlayout serverHeatMapHeader;
	@Wire private A detailsBtn;
	@Wire private Div tags;
	@Wire private Image launchSWAHeatMap;
	private int instanceId;
	private int swaID;
	private String swaInstanceName;
	private String productInstanceName;
	private boolean restChartErrorDiv, rtChartErrorDiv;
	private long OFFSET_MILLIS = 0;
	
	@Override
	public void doAfterCompose(Component comp) throws Exception {
		super.doAfterCompose(comp);
		OFFSET_MILLIS = Utility.cancelOffSetInMillis();
		Integer instanceIdParameter = Utility.getIntUrlParameter(Executions.getCurrent().getParameterMap(), "id");
		String instanceNameParameter = Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "name");
		swaInstanceName = instanceNameParameter;
		String healthIndexParameter = Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "idx");
		String statusParameter = Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "state");
		productInstanceName = Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
		swaID = Integer.parseInt(Executions.getCurrent().getParameter("swaID"));
		if(swaID==0){
			launchSWAHeatMap.setVisible(false);
		}
		if (instanceIdParameter != null) {
			instanceId = instanceIdParameter;
		} else {
			Object param = Executions.getCurrent().getDesktop().getAttribute("instanceId");
			if(param != null){
				instanceId = (Integer) param;
			}
		}
		
		
		detailsBtn.setHref(WebUtil.buildPathRelativeToCurrentProduct("singleInstance"+"/"+instanceId));
		serverHeatMapHeader.removeAttribute("class");
		serverHeatMapHeader.setClass("serverHeatMapHeader "+"state_"+statusParameter);
		instanceName.setValue(instanceNameParameter);
		healthIdx.setValue((new DecimalFormat("#00.00").format(Double.parseDouble(healthIndexParameter)*100))+"%");
		//healthIdx.setValue(healthIndexParameter+"%");
        refreshModels();
        
	}
	
	@Listen("onClick = #launchSWAHeatMap")
	public void launchSWA() throws IOException, RestException{
		try{
			Executions.sendRedirect(SQLDMRestClient.getInstance().getSWAurl(swaID,swaInstanceName));
		}		
		catch(Exception e){
			Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("/home"));
		}
	}
	
	
	private void refreshModels() throws CategoryException{
		
		setModelData();
		
		setChartProperties(cpuUsage);
		setChartProperties(memoryUsage);
		setChartProperties(ioActivity);
		setChartProperties(responseTime);
		
		if(restChartErrorDiv){
			cpuUsage.getErrorDiv().setVisible(false);
			memoryUsage.getErrorDiv().setVisible(false);
			ioActivity.getErrorDiv().setVisible(false);
		}
		
		if(rtChartErrorDiv){
			responseTime.getErrorDiv().setVisible(false);
		}

	}
	
	private void setChartProperties(IderaLineChart chart){
		if(chart != null && chart.getChart() != null) {
			chart.getChart().setShowSeriesLabels(false);
			chart.getChart().setXAxisTickCount(0);
			chart.getChart().setYAxisTickCount(0);
			chart.getTopDiv().setVisible(false);
			chart.getInnerSpace().setVisible(false);
			chart.getContainerDiv().setStyle(
					"padding-left:0px;padding-right:0px");
			chart.getChart().setHeight("100%");
			chart.getChart().setWidth("100%");
			chart.getChart().setDrawXAxis(false);
			chart.getChart().setDrawYAxis(false);
		}
	}
	
	public void setModelData() throws CategoryException {
		
		List<ResourceCategory> resourceCategoryList = ResourcesFacade.getResources(productInstanceName,instanceId, 60);
		
		IderaLineChartModel cpuUsageModel = new IderaLineChartModel();
		IderaLineChartModel sqlMemoryUsageModel = new IderaLineChartModel();
		IderaLineChartModel physicalIOModel = new IderaLineChartModel();
		
		if(resourceCategoryList !=null && resourceCategoryList.size() >1){
			for(int i=resourceCategoryList.size()-1; i>=0; i--) {
				ResourceCategory resource = resourceCategoryList.get(i);
				//Adding offset for graphs
				long collectionTime = resource.getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
				cpuUsageModel.setValue("SQL Server", collectionTime, resource.getCpu().getSqlCPUUsage());
				sqlMemoryUsageModel.setValue("Total Used", collectionTime, Math.round(
						resource.getMemory().getSqlMemory().get("TotalMemoryInKB")/1024*100.0)/100.0);
				physicalIOModel.setValue("Page Reads", collectionTime, resource
						.getDisk().getSqlPhysicalIO().get("PageReads"));
				physicalIOModel
						.setValue("Page Writes", collectionTime, resource
								.getDisk().getSqlPhysicalIO().get("PageWrites"));
			}
			ioActivity.setModel(physicalIOModel);
			memoryUsage.setModel(sqlMemoryUsageModel);
			cpuUsage.setModel(cpuUsageModel);
			restChartErrorDiv = true;
			
		} else {
			setEmptyCharts();
		}
		/* for responsetime linechart */
		
		IderaLineChartModel rtModel = new IderaLineChartModel();
		ServerStatus status = null;
		
		try {
			status = DashboardInstanceFacade.getDashboardInstanceServerStatus(
					productInstanceName, instanceId, 60);
		} catch (InstanceException x) {
			setErrorChart();
		}
		
		if (status != null && status.getStatisticsHistory() != null
				&& status.getStatisticsHistory().size() > 1){
			List<StatisticsHistory> list = status.getStatisticsHistory();
			for (int i = list.size() - 1; i >= 0; i--) {
				StatisticsHistory sh = list.get(i);
				long collectionTime = sh.getUTCCollectionDateTime().getTime();
				//Adding offset for graphs
				long collectionTimeGraph = sh.getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
				rtModel.setValue(ELFunctions
						.getMessage(SQLdmI18NStrings.RESPONSE_TIME_MS),
						collectionTimeGraph, sh.getReponseTimeinMilliSeconds());

			}
			responseTime.setModel(rtModel);
			rtChartErrorDiv = true;
			
		} else {
			setRTEmptyCharts();
		}
		
		/* for tags*/
		DashboardInstance db =  null;
		try {
			db = DashboardInstanceFacade.getDashboardInstance(productInstanceName, instanceId);
		} catch (InstanceException e) {
			e.printStackTrace();
		} catch (RestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		/*if(db != null && db.getServerStatus() != null ){
			healthIdx.setValue((db.getServerStatus().getHealthIndex())+"%");
		}*/
		
		List<String> tagsList = null;
		if(db != null && 
				((db.getServerStatus() != null && db.getServerStatus().getTags()!=null &&  db.getServerStatus().getTags().size() >= 1) 
						|| (db.getOverview()!=null && db.getOverview().getTags() !=null && db.getOverview().getTags().size() >= 1))){
			if(db.getServerStatus().getTags()!=null){
				tagsList = db.getServerStatus().getTags();
			}
			else {
				tagsList = db.getOverview().getTags();
			}
		} else {
			tagsList = new LinkedList<String>();
		}
		appendTags(tagsList);
	}
	
	private void appendTags(List<String> tagsList){

		if(!tagsList.isEmpty()){
			for(String s : tagsList ){
	        	Label l = new Label();
	        	l.setValue(s);
	        	l.setClass("tagName");
	        	tags.appendChild(l);
	        }
		} else {
			tagLabel.setVisible(false);
			tags.setVisible(false);
		}
	}
	
	private void setEmptyCharts(){
		ioActivity.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		cpuUsage.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		memoryUsage.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
	}
	
	private void setRTEmptyCharts() {
		responseTime.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
	}
	
	
	private void setErrorChart() {
		responseTime.setErrorMessage(ELFunctions
				.getMessage(SQLdmI18NStrings.ERROR_OCCURRED_LOADING_CHART));
	}

	
}
