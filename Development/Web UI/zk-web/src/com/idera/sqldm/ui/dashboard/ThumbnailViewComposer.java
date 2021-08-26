package com.idera.sqldm.ui.dashboard;

import java.io.IOException;
import java.util.List;

import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Div;
import org.zkoss.zul.Image;

import com.idera.common.rest.RestException;
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
import com.idera.sqldm.ui.components.charts.line.IderaLineChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.utils.Utility;

//new thumbnailViewComposer @Author:prakash 
public class ThumbnailViewComposer extends SelectorComposer<Component> {

	private static final long serialVersionUID = 9200286224241425901L;

	@Wire
	private IderaLineChart cpuUsage;
	@Wire
	private IderaLineChart ioActivity;
	@Wire
	private IderaLineChart memoryUsage;
	@Wire
	private IderaLineChart responseTime;
	@Wire
	private Div cpuDiv;
	@Wire
	private Div memoryDiv;
	@Wire
	private Div ioDiv;
	@Wire
	private Div responseTimeChartDiv;
	
	private int instanceId;
	private String productInstanceName;
	private boolean restChartErrorDiv, rtChartErrorDiv;
	private long OFFSET_MILLIS = 0;
	
	@Override
	public void doAfterCompose(Component comp) throws Exception {
		super.doAfterCompose(comp);
		OFFSET_MILLIS = Utility.cancelOffSetInMillis();
		String s = Executions.getCurrent().getParameter("id");
		Integer instanceIdParameter = Integer.parseInt(Executions.getCurrent().getParameter("id"));
		productInstanceName = Executions.getCurrent().getParameter("productName");
	/*	Integer productId = Utility.getIntUrlParameter(Executions.getCurrent()
				.getParameterMap(), "productId");*/
		if (instanceIdParameter != null) {
			instanceId = instanceIdParameter;
		} else {
			Object param = Executions.getCurrent().getDesktop()
					.getAttribute("instanceId");
			if (param != null) {
				instanceId = (Integer) param;
			}
		}

		refreshModels();
	}

	private void refreshModels() throws CategoryException {
		
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
			chart.getErrorLabel().setStyle("font-size:10px;");
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
				//Adding offset for graphs
				long collectionTime = sh.getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
				rtModel.setValue(ELFunctions
						.getMessage(SQLdmI18NStrings.RESPONSE_TIME_MS),
						collectionTime, sh.getReponseTimeinMilliSeconds());

			}
			responseTime.setModel(rtModel);
			rtChartErrorDiv = true;
			
		} else {
			setRTEmptyCharts();
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