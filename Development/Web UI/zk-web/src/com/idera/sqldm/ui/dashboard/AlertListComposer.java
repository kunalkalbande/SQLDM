package com.idera.sqldm.ui.dashboard;

import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.StringTokenizer;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.UiException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Composer;
import org.zkoss.zkplus.databind.BindingListModelList;
import org.zkoss.zul.Grid;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Paging;
import org.zkoss.zul.Row;
import org.zkoss.zul.Window;

import com.idera.common.Utility;
import com.idera.cwf.model.Product;
import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.alerts.Alert;
import com.idera.sqldm.data.alerts.AlertException;
import com.idera.sqldm.data.alerts.AlertFacade;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.server.web.WebUtil;
import com.idera.sqldm.server.web.session.SessionUtil;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstanceAlertsPreferencesBean;
import com.idera.sqldm.utils.GridExporter;
import com.idera.sqldm.utils.SQLdmConstants;

public class AlertListComposer implements Composer<Component>{
	
	protected static final Logger log = Logger.getLogger(AlertListComposer.class);
	static final long serialVersionUID = 1L;
	private String OFFSET_IN_HOURS = "0.0";
	private static int ALERT_ROWS = 25; 
	private boolean calledFromInstance = false;
	private List<Alert> alertsList;
	private int prevPageSize;
	private int alertsListRowsCount;
	private int listRowsCount;
	private String displayMessageForAlertGrid;
	private boolean isForceLoad = false;
	public ListModelList<Alert> alertModel = new BindingListModelList<Alert>(new LinkedList<Alert>(), false);
	private final String alertsLabel = ELFunctions.getLabel(SQLdmI18NStrings.ACTIVE_ALERTS);
	public String alertModelTitle = alertsLabel.toUpperCase() + " (?)"; 
	private Integer instanceIdParameter;
	@Wire Paging instancesAlertsListPgId;
	@Wire Grid alertGrid;

	@Init
	public void init(){
		setOffSet();
		refreshAlerts();
		EventQueue<Event> eqForDashboardRefresh = EventQueues.lookup(DashboardConstants.DASHBOARD_QUEUE_NAME, EventQueues.SESSION, true);
		eqForDashboardRefresh.subscribe(new EventListener<Event>() {
 			public void onEvent(Event event) throws Exception {
 				if(DashboardConstants.DASHBOARD_REFRESH_ALERT_EVENT_NAME.equalsIgnoreCase(event.getName())) {
 					try {
 	 					log.debug("Updating Dashboard Alerts View");
 	 					refreshAlertsListForDashboard((List<Alert>)event.getData());
 					} catch (Exception e) {
 						log.error(e.getMessage(), e);
 					}
 				}
             }
         });
		
		EventQueue<Event> eq = EventQueues.lookup(SQLdmConstants.DASHBOARD_UPDATE_INSTANCES_EVENT_QUEUE, EventQueues.DESKTOP, true);
        eq.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				refreshAlertsListForDashboard(getAlerts());
            }
        });

 		setDisplayMessageForAlertGrid(ELFunctions.getMessage(SQLdmI18NStrings.LOADING_DATA));
 		String forceLoad = (String)Executions.getCurrent().getAttribute("forceLoad");
 		//System.out.println(">>> Force Load:" + forceLoad);
 		isForceLoad = false;
 		if (forceLoad != null) {
 			try {
 				isForceLoad = Boolean.parseBoolean(forceLoad);
 				if (isForceLoad) {
 					refreshAlertsList(instanceIdParameter);
 				}
 			} catch (Exception ex) {
 				log.error("An exception has occurred: " + ex);
 			}
 		}

		eq = EventQueues.lookup("updateModel",
				EventQueues.DESKTOP, true);
		eq.subscribe(new EventListener<Event>() {

			@Override
			public void onEvent(Event event) throws Exception {

				if (event.getName().equals("updateModel")) {
					refreshAlertsList(instanceIdParameter);
				}
			
			}
		});

	}
	
	public void refreshAlerts() {
		instanceIdParameter = Utility.getIntUrlParameter(Executions.getCurrent().getParameterMap(), "id");
		if (instanceIdParameter == null) {
			Object param = Executions.getCurrent().getDesktop().getAttribute("instanceId");
			if(param != null){
				instanceIdParameter = (Integer) param;
			}
		}

 		String pageCountStr = (String)Executions.getCurrent().getAttribute("pageCount");
 		if (pageCountStr != null) {
 			try {
 				ALERT_ROWS = Integer.parseInt(pageCountStr);
 				calledFromInstance = true;
 			} catch (Exception ex) {
 				log.error("An exception has occurred: " + ex);
 			}
 		}
 		
		DashboardPreferencesBean dpb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();
		int defaultAlertsRowCount = ALERT_ROWS;
		if (dpb != null && dpb.getAlertsRowCount() != -1 && calledFromInstance == false) { 
			defaultAlertsRowCount = dpb.getAlertsRowCount();
		}
		prevPageSize = defaultAlertsRowCount == 1 ? 2 : defaultAlertsRowCount;
		setAlertsListRowsCount(prevPageSize);
		setListRowsCount(prevPageSize);
	}
	public int getAlertsListRowsCount() {
		return alertsListRowsCount;
	}

	public int getListRowsCount() {
		return listRowsCount;
	}
	
	public boolean getIsForceLoad() {
		return isForceLoad;
	}
	
	public boolean getIsNotForceLoad() {
		return !isForceLoad;
	}

	public void setAlertsListRowsCount(int alertsListRowsCount) {
		this.alertsListRowsCount = alertsListRowsCount;
		BindUtils.postNotifyChange(null, null, this, "alertsListRowsCount");
	}

	public void setListRowsCount(int listRowsCount) {
		this.listRowsCount = listRowsCount;
		BindUtils.postNotifyChange(null, null, this, "listRowsCount");
	}

	@Command("setAlertsRowsCount")
	public void setAlertsRowsCount() {
		setListRowsCount(this.alertsListRowsCount);
		if(!calledFromInstance){
			PreferencesUtil.getInstance().setDashboardPreferencesInSession(null, null, null, null, -1, this.alertsListRowsCount, null);
		}
	}
	
	private void refreshAlertsList(Integer instanceId) {
		alertsList = new LinkedList<>();
       	String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
        
		try {
			if(instanceId == null){
				alertsList.addAll(AlertFacade.getActiveAlerts(productInstanceName , OFFSET_IN_HOURS));
			}
			else{
				
				SingleInstanceAlertsPreferencesBean sessionBean = PreferencesUtil
						.getInstance().getSingleInstanceAlertsPreferenceInSession(instanceId);
				alertsList.addAll(sessionBean.getModelData());

			}
		} catch(Exception ex) {
		}
		setAlertModel(alertsList);
		if(alertsList == null || alertsList.size() == 0) {
			setDisplayMessageForAlertGrid(ELFunctions.getMessage(SQLdmI18NStrings.NO_ACTIVE_ALERTS));
		}
	}

	private void refreshAlertsListForDashboard(List<Alert> alertsList) {
		this.alertsList = alertsList;
		setAlertModelAndMessage(alertsList);
	}

	public List<Alert> getAlerts(){
		return alertsList;
	}
	
	@Command("onInstanceClick")
	public void onInstanceClick(@BindingParam("instanceId") int instanceId , @BindingParam("product") Product product) {
		//Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance"+"/"+instanceId));
		Executions.sendRedirect(WebUtil.buildPathRelativeToProduct(product, "singleInstance"+"/"+instanceId));
	}
	
	@Command("onAlertClick")
	public void onAlertClick(@ContextParam(ContextType.COMPONENT) Component comp) {
		Row row = (Row) comp;
		Alert alert = null;
		if (row != null && row.getValue() != null && row.getValue() instanceof Alert) {			
			alert = (Alert) row.getValue();
		}
		Map<String, Object> args = new HashMap<String, Object>();
		args.put("alert", alert);
		args.put("alertsList", alertsList);
		SessionUtil.getSecurityContext();
		Window window = (Window)Executions.createComponents(
                "~./sqldm/com/idera/sqldm/ui/alerts/alertsView.zul", null, args);
        window.doModal();
    }
	

	@Command
	public void exportGrid(@BindingParam("alertGrid") Grid alertGrid) {

		GridExporter.exportToPdf(alertGrid, Alert.class, "getMapDashboard", "AlertsList");
		
	}

	@Command
	public void exportToExcel(@BindingParam("alertGrid") Grid alertGrid) {

		GridExporter.exportToExcel(alertGrid, Alert.class, "getMapDashboard", "AlertsList");

	}

	@Command
	public void exportToXml(@BindingParam("alertGrid") Grid alertGrid,
			@BindingParam("title") String title) {

		StringTokenizer str = new StringTokenizer(title, "(");

		GridExporter.exportToXml(alertGrid, str.nextToken(), "(" + str.nextToken(), Alert.class, "getMapDashboard", "AlertsList");

	}

	@Command
	public void onClickSelectedAlert(@BindingParam("alert") Object alert) {
		
		try {
			SessionUtil.getSecurityContext();
			Map<Object, Object> args = new HashMap<Object, Object>();
			args.put("alert", alert);
			args.put("alertsList", alertsList);
			Window window = (Window) Executions.createComponents(
					"~./sqldm/com/idera/sqldm/ui/alerts/alertsView.zul", null, args);
			window.doModal();
		} catch(UiException e) {
			
		}
	}

	public ListModelList<Alert> getAlertModel() {
		return alertModel;
	}
	
	private void setAlertModelAndMessage(List<Alert> alertsList) {
		List<Alert> subAlertsList = null;
		if (alertsList != null) {
			try {
				subAlertsList = new LinkedList<Alert>();
				Map<Integer, Object> instancesHashListFromSession = PreferencesUtil
						.getInstance().getFilteredInstancesMap();
				if (instancesHashListFromSession != null) {
					for (Alert alert : alertsList) {
						if (instancesHashListFromSession.get(alert
								.getInstanceId()) != null) {
							subAlertsList.add(alert);
						}
					}
				} else {
					subAlertsList = alertsList;
				}
				
				setAlertModel(subAlertsList);
			} catch(Exception ex) {
				log.error("Exception while setting Alerts model.", ex);
			}
		}

		if (subAlertsList == null || subAlertsList.size() == 0) {
			setDisplayMessageForAlertGrid(ELFunctions
					.getMessage(SQLdmI18NStrings.NO_ACTIVE_ALERTS));
		}
	}

	public void setAlertModel(List<Alert> alertsList) {
		this.alertModel = new ListModelList<Alert>(alertsList);
		if (alertsList != null) {
			this.alertModelTitle = alertsLabel.toUpperCase() + " ("
					+ new Integer(alertsList.size()).toString() + ")";
		} else {
			this.alertModelTitle = alertsLabel.toUpperCase();
		}
		BindUtils.postNotifyChange(null, null, this, "alertModel");
		BindUtils.postNotifyChange(null, null, this, "alertModelTitle");
	}
	public String getDisplayMessageForAlertGrid() {
		return displayMessageForAlertGrid;
	}

	public String getAlertModelTitle() {
		return alertModelTitle;
	}

	public void setDisplayMessageForAlertGrid(String displayMessageForAlertGrid) {
		this.displayMessageForAlertGrid = displayMessageForAlertGrid;
		BindUtils.postNotifyChange(null, null, this,
				"displayMessageForAlertGrid");
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view)
			throws AlertException {
		// super.doAfterCompose(view);
		Selectors.wireComponents(view, this, false);
		alertGrid.setPaginal(instancesAlertsListPgId);
	}

	@Override
	public void doAfterCompose(Component arg0) throws Exception {
		// TODO Auto-generated method stub
		
	}
	private void setOffSet(){
		Double offSet = null;
		if(Sessions.getCurrent()!=null)
		{
			offSet = new Double((Integer)Sessions.getCurrent().getAttribute(WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET))/(1000*60.0*60.0);
			offSet = -offSet; 
		}
		if(offSet!=null)
	OFFSET_IN_HOURS = offSet.toString();
	}
}
