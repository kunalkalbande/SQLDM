package com.idera.sqldm.ui.dashboard.instances.overview;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.List;
import java.util.concurrent.TimeUnit;

import org.apache.log4j.Logger;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.Div;

import com.idera.common.Utility;
import com.idera.sqldm.data.DashboardInstanceFacade;
import com.idera.sqldm.data.InstanceException;
import com.idera.sqldm.data.instances.ServerStatus;
import com.idera.sqldm.data.instances.StatisticsHistory;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.server.web.WebUtil;
import com.idera.sqldm.ui.components.charts.area.IderaAreaChart;
import com.idera.sqldm.ui.components.charts.area.IderaAreaChartModel;
import com.idera.sqldm.ui.dashboard.instances.SingleInstanceDashboardComposer;
import com.idera.sqldm.ui.preferences.HistoryPanelPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstancePreferencesBean;

public class OverViewChartComposer extends SelectorComposer<Div> {

	private static final long serialVersionUID = 1L;
	protected static final Logger log = Logger.getLogger(OverViewChartComposer.class);
	private int instanceId = 0;
	private static boolean historyEvent = false;
	
	@Wire protected IderaAreaChart responseTime;
	@Wire protected IderaAreaChart cpuActivity;
	@Wire protected IderaAreaChart diskBusy;
	@Wire protected IderaAreaChart memoryUsage;

	protected AnnotateDataBinder binder = null;
	private long OFFSET_MILLIS = 0;

    public void doAfterCompose(Div comp) throws Exception {
        super.doAfterCompose(comp);
        
        OFFSET_MILLIS = com.idera.sqldm.utils.Utility.cancelOffSetInMillis();
        Integer instanceIdParameter = Utility.getIntUrlParameter(Executions
				.getCurrent().getParameterMap(), "id");
		if (instanceIdParameter != null) {
			instanceId = instanceIdParameter;
		}
		else {
			//fallback
			Object param = Executions.getCurrent().getDesktop().getAttribute("instanceId");
			if(param != null){
				instanceId = (Integer) param;
			}
		}
		drawChart();
		EventQueue<Event> eq1 = EventQueues.lookup("historyChange1",
				EventQueues.DESKTOP, true);
		eq1.subscribe(new EventListener<Event>() {

			@Override
			public void onEvent(Event event) throws Exception {
				refreshGraphs();
			}
		});
		EventQueue<Event> eq2 = EventQueues.lookup("overviewHistory",
				EventQueues.DESKTOP, true);
		eq2.subscribe(new EventListener<Event>() {

			@Override
			public void onEvent(Event event) throws Exception {
				refreshChart();
			}
		});
    }
    
    protected void refreshChart() {
    	if(historyEvent){
    		historyEvent = false;
    		drawChart();
    	}
	}

	protected void refreshGraphs() {
    	historyEvent = true;
	}

	private void drawChart() {
    	IderaAreaChartModel rtModel = new IderaAreaChartModel();
        IderaAreaChartModel cpuModel = new IderaAreaChartModel();
        IderaAreaChartModel dbModel = new IderaAreaChartModel();
        IderaAreaChartModel muModel = new IderaAreaChartModel();
        responseTime.refresh();
		cpuActivity.refresh();
		diskBusy.refresh();
		memoryUsage.refresh();
    	String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
		ServerStatus status = null;
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		try
		{
			if(pref.getScaleCombobox().getValue().equals("Custom")){
				Date startTime = pref.getFromDateTime();
				Date endTime = pref.getToDateTime();
				long duration  = endTime.getTime() - startTime.getTime();
				long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
				status= DashboardInstanceFacade.getDashboardInstanceServerStatus(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
				
	        }
	        else{
	        	status= DashboardInstanceFacade.getDashboardInstanceServerStatus(productInstanceName,instanceId, pref.getNumHistoryMinutes());
	        }
		}
		catch (InstanceException x)
		{
			setErrorChart();
		}			
		
		if (status == null  || status.getStatisticsHistory() == null || status.getStatisticsHistory().size() <= 1) {
			setEmptyCharts();
			return;
		}

		List<StatisticsHistory> list = status.getStatisticsHistory();
		
		for (int i=list.size()-1; i>=0; i--) {
			StatisticsHistory sh = list.get(i);
			//Adding offset for graphs
			long collectionTime = sh.getUTCCollectionDateTime().getTime()+OFFSET_MILLIS;
			rtModel.setValue(ELFunctions.getMessage(SQLdmI18NStrings.RESPONSE_TIME_MS), collectionTime, sh.getReponseTimeinMilliSeconds());
			cpuModel.setValue(ELFunctions.getMessage(SQLdmI18NStrings.SQL_SERVER_CPU_ACTIVITY), collectionTime, sh.getCPUActivityPercentage());
			dbModel.setValue(ELFunctions.getMessage(SQLdmI18NStrings.DISK_BUSY), collectionTime, sh.getDiskTimePercent());
			muModel.setValue(ELFunctions.getMessage(SQLdmI18NStrings.TOTAL_OS_MEMORY_USAGE_IN_MB), collectionTime, Math.round(sh.getOSTotalPhysicalMemoryInKilobytes()/1024));
			muModel.setValue(ELFunctions.getMessage(SQLdmI18NStrings.OS_MEMORY_USAGE_IN_MB), collectionTime, sh.getOSPhysicalMemoryUsageInMB());            
		}
		responseTime.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESPONSE_TIME_MS));
		cpuActivity.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.SQL_SERVER_CPU_ACTIVITY));
		diskBusy.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.DISK_BUSY));
		memoryUsage.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.OS_MEMORY_USAGE_IN_MB));
		responseTime.setModel(rtModel);
		responseTime.getChart().setXAxisLegendSpacing(27);
		cpuActivity.setModel(cpuModel);
		
		
		
		
		if(cpuActivity.getChart()!=null){
            cpuActivity.getChart().setyAxisCustomMaxDomainValue(true);
            cpuActivity.getChart().setyAxisCustomMinDomainValue(true);
            cpuActivity.getChart().setyAxisMinDomainValue(0);
            cpuActivity.getChart().setyAxisMaxDomainValue(100);
        }
	
		diskBusy.setModel(dbModel);
		/*
		 * Author:Accolite
		 * Date : 11th Nov, 2016
		 * Drill  down Capability - SQLDM- 10.2 release
		 */
		responseTime.getChart().setDrillable(true);
		cpuActivity.getChart().setDrillable(true);
		diskBusy.getChart().setDrillable(true);
		memoryUsage.getChart().setDrillable(true);
		
		if(diskBusy.getChart()!=null){
			diskBusy.getChart().setyAxisCustomMaxDomainValue(true);
			diskBusy.getChart().setyAxisCustomMinDomainValue(true);
			diskBusy.getChart().setyAxisMinDomainValue(0);
			diskBusy.getChart().setyAxisMaxDomainValue(100);
        }	
		memoryUsage.setModel(muModel);    
		memoryUsage.getChart().setXAxisLegendSpacing(26);
		
    }
    
	private void setErrorChart() {
		responseTime.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.ERROR_OCCURRED_LOADING_CHART));
		memoryUsage.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.ERROR_OCCURRED_LOADING_CHART));
		diskBusy.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.ERROR_OCCURRED_LOADING_CHART));
		cpuActivity.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.ERROR_OCCURRED_LOADING_CHART));
		
	}

	private void setEmptyCharts() {
		responseTime.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESPONSE_TIME_MS));
		cpuActivity.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.SQL_SERVER_CPU_ACTIVITY));
		diskBusy.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.DISK_BUSY));
		memoryUsage.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.OS_MEMORY_USAGE_IN_MB));
		responseTime.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		memoryUsage.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		diskBusy.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		cpuActivity.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
	}
	
	
	/*
	 * Author:Accolite
	 * Date : 11th Nov, 2016
	 * Drill  down Capability - SQLDM- 10.2 release
	 */
	
	/*@Listen("onDrillChange=div#memoryUsageChartDiv")
	public void onDrillChange(Event evt){
		String[] time = evt.getData().toString().split("&");
		String from = time[0].substring(0,time[0].indexOf("G"));
		String to = time[1].substring(0,time[1].indexOf("G"));
		Date fromDate= null;
		Date toDate= null;
		try {
			
			//Read the date from UI
			SimpleDateFormat parseDateFormat = new SimpleDateFormat("E MMM dd yyyy HH:mm:ss ");
			//From date and Time
			fromDate=(Date)parseDateFormat.parse(from);
			//To date and time
			toDate=(Date)parseDateFormat.parse(to);
			
		} catch (Exception e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		SingleInstanceDashboardComposer siDC = new SingleInstanceDashboardComposer();
		siDC.callDrillDownChange(fromDate, toDate);
	}*/
	
	
	@Listen("onClick = div#responseTimeChartDiv")
	public void onClickResponseTimeDiv(Event evt) {
        SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
        pref.setSelectedCategory(1);
        pref.setSelectedSubCategory(1);
        PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);

        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance"+"/"+instanceId));
	}

	@Listen("onClick = div#cpuActivityChartDiv")
	public void onClickCpuActivityChartDiv(Event evt) {
        SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
        pref.setSelectedCategory(3);
        pref.setSelectedSubCategory(2);
        PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);

        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance"+"/"+instanceId));
	}

	@Listen("onClick = div#memoryUsageChartDiv")
	public void onClickMemoryUsageChartDiv(Event evt) {
        SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
        pref.setSelectedCategory(3);
        pref.setSelectedSubCategory(3);
        PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);

        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance"+"/"+instanceId));
	}

	@Listen("onClick = div#diskBusyChartDiv")
	public void onClickDiskBusyChartDiv(Event evt) {
        SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
        pref.setSelectedCategory(3);
        pref.setSelectedSubCategory(4);
        PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);

        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance"+"/"+instanceId));
	}

 }