package com.idera.sqldm.ui.dashboard.instances.sessions;

import java.util.List;

import com.idera.sqldm.ui.components.charts.area.IderaAreaChart;
import com.idera.sqldm.ui.components.charts.area.IderaAreaChartModel;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.Div;

import com.idera.sqldm.data.instances.SessionGraphDetail;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.ui.components.charts.line.IderaLineChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.utils.SQLdmConstants;
import com.idera.sqldm.utils.Utility;

public class ChartPageComposer extends SelectorComposer<Div> {
	
	private static final long serialVersionUID = 1L;
	protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory
			.getLogger(ChartPageComposer.class);

	@Wire protected IderaAreaChart responseTimeChart;
	@Wire protected  IderaLineChart activityChart;
	@Wire protected  IderaAreaChart blockedSessionChart;
	protected AnnotateDataBinder binder = null;

	private List<SessionGraphDetail> sessionDetails;

	@Override
	public void doAfterCompose(Div comp) throws Exception {
		super.doAfterCompose(comp);
		
		/*
		 * ChaitanyaTanwar DM 10.2 History Requirement
		 * */
		EventQueue<Event> eq1 = EventQueues.lookup("updateSessionChart",
				EventQueues.DESKTOP, true);
		eq1.subscribe(new EventListener<Event>() {

			@Override
			public void onEvent(Event event) throws Exception {

				if (event.getName().equals("update")) {
					drawChart();
				}

			}
		});
		
		drawChart();
	}

	@SuppressWarnings("unchecked")
	private void drawChart() {
//		Calendar endTime = Calendar.getInstance();
//		Calendar startTime = Calendar.getInstance();
//		startTime.setTimeInMillis(startTime.getTimeInMillis());
//		startTime.add(Calendar.MINUTE, -30);
		
		IderaAreaChartModel rtModel = new IderaAreaChartModel();
		IderaLineChartModel asModel = new IderaLineChartModel();
		IderaAreaChartModel bsModel = new IderaAreaChartModel();
		responseTimeChart.refresh();
		activityChart.refresh();
		blockedSessionChart.refresh();
		
		try {
			Object instanceSessionsChartObj = Executions.getCurrent().getDesktop().getAttribute(SQLdmConstants.DASHBOARD_SCOPE_SINGLE_INSTANCE_SESSION_CHART);
			sessionDetails = (List<SessionGraphDetail>) instanceSessionsChartObj;
			// InstanceDetailsSessionFacade.getSessionChartDetail(instanceId,new Date(startTime.getTimeInMillis()), new Date(endTime.getTimeInMillis()));
			if (sessionDetails != null && sessionDetails.size() >1){
				for (int i = 0 ; i < sessionDetails.size(); i++) {
					SessionGraphDetail detail = sessionDetails.get(i);
					//Adding offset for graphs
					long collectionTime = detail.getUTCCollectionDateTime().getTime() + Utility.cancelOffSetInMillis();
					rtModel.setValue("Response Time (ms)", collectionTime,detail.getResponsetimeinmils() );
					
					asModel.setValue("Active", collectionTime, detail.getActiveSessionCount());
					asModel.setValue("Idle", collectionTime, detail.getIdleSessionCount());
					asModel.setValue("System", collectionTime, detail.getSystemSessionCount());
					
					bsModel.setValue("Blocked Sessions", collectionTime, detail.getBlockedCount());
					bsModel.setValue("Lead Blockers", collectionTime, detail.getLeadBlockers());
					bsModel.setValue("Total Deadlocks", collectionTime, detail.getTotalDeadLock());
				}
				responseTimeChart.setModel(rtModel);
				responseTimeChart.getChart().setXAxisLegendSpacing(30);
				activityChart.setModel(asModel);
				blockedSessionChart.setModel(bsModel);
				
				/*
				 * Author:Accolite
				 * Date : 11th Nov, 2016
				 * Drill  down Capability - SQLDM- 10.2 release
				 */
				responseTimeChart.getChart().setDrillable(true);
				activityChart.getChart().setDrillable(true);
				blockedSessionChart.getChart().setDrillable(true);
				
			}else{
				setEmptyCharts();
			}
			responseTimeChart.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.RESPONSE_TIME_MS));
			activityChart.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.SESSION_ACTIVITY));
			blockedSessionChart.setTitle(ELFunctions.getMessage(com.idera.sqldm.i18n.SQLdmI18NStrings.BLOCKED_SESSION));
			
		} catch (Exception x) {
			responseTimeChart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.ERROR_OCCURRED_LOADING_CHART));
			activityChart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.ERROR_OCCURRED_LOADING_CHART));
			blockedSessionChart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.ERROR_OCCURRED_LOADING_CHART));
		}
		
	}

	
	private void setEmptyCharts() {
		responseTimeChart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		activityChart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
		blockedSessionChart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
	}
	 

}
