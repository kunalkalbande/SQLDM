package com.idera.sqldm.ui.alerts;

import java.util.Calendar;
import java.util.Comparator;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

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
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.GenericForwardComposer;
import org.zkoss.zul.Grid;
import org.zkoss.zul.Intbox;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Paging;
import org.zkoss.zul.Window;

import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.alerts.Alert;
import com.idera.sqldm.data.alerts.AlertException;
import com.idera.sqldm.data.alerts.AlertFacade;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.server.web.WebUtil;
import com.idera.sqldm.server.web.session.SessionUtil;
import com.idera.sqldm.ui.alerts.AlertsGroupingModel.GroupBy;
import com.idera.sqldm.ui.preferences.AlertsPreferencesBean;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.utils.GridExporter;
import com.idera.sqldm.utils.Utility;

public class AlertsGroupViewModel extends GenericForwardComposer<Component>{
	
	private static final String DESC_ORDERTYPE = "desc";
	private static final String ASC_ORDERTYPE = "asc";
	private Logger log = Logger.getLogger(AlertsGroupViewModel.class);

	private boolean showGroup = false;
	private List<Alert> alertsList;
	private AlertsGroupingModel alertGroupingModel;
	private GroupBy groupBy = GroupBy.ACTIVE;//default is active
	private int alertsListRowsCount;
	private int listRowsCount;
	
	private int defaultAlertsRowCount = 10;
	private int prevPageSize;
	
	//Initialize with default filter
	private AlertFilter selectedFilters = new AlertFilter(true);
	private String orderBy = AlertSortByColumns.CURRENTDATE.getColumnName();
	private String orderType = DESC_ORDERTYPE;
	private String emptyGridMessage;
	private boolean showWarningMsg = false;
	ListModelList alertsModel;
	
	//Sorting is manged internally at services API so asc and desc not needed for now.
	Comparator<Alert> instanceComparator = new AlertsComparator(AlertSortByColumns.INSTANCE, true);
	Comparator<Alert> severityComparator = new AlertsComparator(AlertSortByColumns.SEVERITY, true);
	Comparator<Alert> databaseComparator = new AlertsComparator(AlertSortByColumns.DATABASE, true);
	Comparator<Alert> categoryComparator = new AlertsComparator(AlertSortByColumns.CATEGORY, true);
	Comparator<Alert> dateComparator = new AlertsComparator(AlertSortByColumns.CURRENTDATE, true);
	Comparator<Alert> metricComparator = new AlertsComparator(AlertSortByColumns.METRIC, true);
	Comparator<Alert> repoComparator = new AlertsComparator(AlertSortByColumns.PRODUCT, true);
	@Wire Paging alertsListPgId;
	@Wire Grid alertsTabGrid1;
	@Wire private Intbox alertListRowsBox;
	private String OFFSET_IN_HOURS = "0.0";
	private Integer offSetInSecs = 0;
	
	@Init
	public void init(){
		setEmptyGridMessage(ELFunctions.getMessage(SQLdmI18NStrings.LOADING_DATA));
		setOffSet();
		setOffSetInSecs();
	}
	
	@AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view) throws AlertException{
		Selectors.wireComponents(view, this, false);

	//	alertsTabGrid1.setPaginal(alertsListPgId);
		AlertsPreferencesBean sessionBean = PreferencesUtil.getInstance().getAlertsPreferencesInSession();
		if(sessionBean.getCustomFilter() != null){
			//update selected filters if available in session.
			selectedFilters = sessionBean.getCustomFilter();
		}
		if(sessionBean.getSelectedGroupBy() != null){
			groupBy = sessionBean.getSelectedGroupBy();
			if(groupBy != GroupBy.ACTIVE && groupBy != GroupBy.CUSTOM){
				this.showGroup = true;
			}
		}
		
		Integer pageCount = defaultAlertsRowCount;
		if(sessionBean.getPageCount() != null && sessionBean.getPageCount() != -1){
			pageCount = sessionBean.getPageCount();
		}
		
		setListRowsCount(pageCount);
		prevPageSize = pageCount;
		setAlertsListRowsCount(pageCount);
		alertListRowsBox.setValue(pageCount);
		setAlertListRowsCount();
		alertsTabGrid1.setPaginal(alertsListPgId);
		
		
		EventQueue<Event> eq = EventQueues.lookup("changeGroup", EventQueues.DESKTOP, true);
        eq.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				SessionUtil.getSecurityContext();
				GroupBy selectedGrp = (GroupBy)event.getData();
				onApplyGroups(selectedGrp);
            }
		});
        EventQueue<Event> eq1 = EventQueues.lookup("changeFilter", EventQueues.DESKTOP, true);
        eq1.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				SessionUtil.getSecurityContext();
            	AlertFilter filter = (AlertFilter)event.getData();
            	onApplyFilters(filter);
            }
        });
        
		EventQueue<Event> eq2 = EventQueues.lookup("exportData",
				EventQueues.DESKTOP, true);
		eq2.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if (event.getName().equals("exportToPdf")) {
					
					GridExporter.exportToPdf(alertsTabGrid1, Alert.class, "getMap", "AlertsList");

				}
				else if (event.getName().equals("exportToExcel")) {

					GridExporter.exportToExcel(alertsTabGrid1, Alert.class, "getMap", "AlertsList");

				}
				else if (event.getName().equals("exportToXml")) {

					GridExporter.exportToXml(alertsTabGrid1, "Alerts", null, Alert.class, "getMap", "AlertsList");

				}
			
			}
		});
		
        refreshAlertList();
	}

	private void refreshAlertList() throws AlertException{
    	String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");

    	DashboardPreferencesBean dbpb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();
		if(dbpb != null){
		productInstanceName = dbpb.getProductInstanceName();
		}
    	
		String currentOrderBy = orderBy;
		String currentOrderType = orderType;
		setShowWarningMsg(false);
		if(this.showGroup){
			//TODO if sorting logic needed for groups as well check here to change the logic.
			//For groups no server side sorting is maintaining after load so just use the default ones.
			currentOrderBy = AlertSortByColumns.CURRENTDATE.getColumnName();
			currentOrderType = DESC_ORDERTYPE;
		}
		else{
			alertsModel = new ListModelList(){
				 public void sort(Comparator cmpr, boolean ascending) {
					 AlertsComparator comp = (AlertsComparator) cmpr;
					 orderBy = comp.getSortBy().getColumnName();
					 if(ascending){
						 orderType = ASC_ORDERTYPE;
					 }
					 else{
						 orderType = DESC_ORDERTYPE;
					 }
	                try {
						refreshAlertList();
					} catch (AlertException e) {
						e.printStackTrace();
					}
	               fireEvent(org.zkoss.zul.event.ListDataEvent.CONTENTS_CHANGED, -1, -1);
	           }
			};
		}
		productInstanceName = AlertFilter.productMapping.get(selectedFilters.getProductId());
		if((this.groupBy != GroupBy.CUSTOM && this.groupBy != GroupBy.PRODUCT) || (this.groupBy == GroupBy.PRODUCT && selectedFilters.isActive())){
			try{
				alertsList = AlertFacade.getAllAlerts(productInstanceName,true, null , null, null, -1, currentOrderBy, currentOrderType , OFFSET_IN_HOURS);
			} catch(Exception e){
				log.error("Exception while getting all alerts list");
				log.error(e.getMessage(),e);
			}
			if(alertsList == null || alertsList.size() == 0){
				setEmptyGridMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_ACTIVE_ALERTS));
			}
		}
		else{
			Integer metricId = (-1 == selectedFilters.getMetricId()) ? null: selectedFilters.getMetricId();
			Integer instanceId = (-1 == selectedFilters.getInstanceId()) ? null: selectedFilters.getInstanceId();
			Integer severity = (-1 == selectedFilters.getSeverity()) ? null: selectedFilters.getSeverity();
			
			if(selectedFilters.isActive()){
				try{
					alertsList = AlertFacade.getAllAlerts(productInstanceName,true, metricId, instanceId, severity, -1, currentOrderBy, currentOrderType , OFFSET_IN_HOURS);
				} catch(Exception e){
					log.error("Exception while getting all alerts list");
					log.error(e.getMessage(),e);
				}
				 
			}
			else{
				Date todate = selectedFilters.getToDateTime();
				Calendar cal = Calendar.getInstance();
				//Adding offset to get Current time 
				cal.add(Calendar.SECOND, offSetInSecs);
				if(todate == null || (todate != null && cal.getTime().before(todate))){
					//if future date change it to current date
					todate = cal.getTime();
					//Removing this hack as services are fixed now 
					//cal.add(Calendar.MINUTE, -2);//TODO hack to avoid exception from services. move time 2 mins back.Remove once services fix
					
					
				}
				Date fromDate = selectedFilters.getFromDateTime();
				if(fromDate == null){
					cal.setTime(todate);
					//If From date is null , return alerts for 3 days past toDate
					cal.add(Calendar.DATE, -3);
					fromDate = cal.getTime();
				}
				try{
					alertsList = AlertFacade.getAllAlerts(productInstanceName, false, fromDate, todate, metricId, instanceId, severity, 1501, currentOrderBy, currentOrderType , OFFSET_IN_HOURS);
				} catch(Exception e){
					log.error("Exception while getting all alerts list");
					log.error(e.getMessage(),e);
				}
				
				if(alertsList != null && alertsList.size() == 1501){
					alertsList.remove(1500);
					setShowWarningMsg(true);
				}
			}
			if(alertsList == null || alertsList.size() == 0){
				setEmptyGridMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_ALERTS));
			}
		}
		if(this.showGroup){
			alertGroupingModel = new AlertsGroupingModel(alertsList, getComparator(groupBy), this.showGroup);
		}
		else{
			if(alertsModel != null) {
				alertsModel.addAll(alertsList);
			}
			BindUtils.postNotifyChange(null, null, this, "alertsModel");
		}
	}
	
	private void onApplyFilters(AlertFilter filters) throws AlertException{
		AlertsPreferencesBean sessionBean = PreferencesUtil.getInstance().getAlertsPreferencesInSession();
		if(sessionBean.getCustomFilter() != null){
			//update selected filters if available in session.
			selectedFilters = sessionBean.getCustomFilter();
		}
		else{
			selectedFilters = new AlertFilter(true);
		}
		if(selectedFilters.getInstanceId()!=null){
		if((!selectedFilters.isActive()&&this.groupBy!=GroupBy.PRODUCT)|| selectedFilters.getInstanceId() != -1 || 
				selectedFilters.getMetricId() != -1 || selectedFilters.getSeverity() != -1 ||selectedFilters.getProductId() != -1){
			this.groupBy = GroupBy.CUSTOM;
			this.showGroup = false;
		}
		PreferencesUtil.getInstance().setAlertsPreferencesInSession(selectedFilters, this.groupBy, -1);
		refreshAlertList();
		if(this.showGroup){
			BindUtils.postNotifyChange(null, null, this, "alertGroupingModel");
		}
		BindUtils.postNotifyChange(null, null, this, "showGroup");
		}
		else{
			Messagebox.show("Connection to server is not established.");
		}
	}
	
	private void onApplyGroups(GroupBy grpBy) throws AlertException {
		AlertsPreferencesBean sessionBean = PreferencesUtil.getInstance().getAlertsPreferencesInSession();
		if(sessionBean.getCustomFilter() != null){
			//update selected filters if available in session.
			selectedFilters = sessionBean.getCustomFilter();
		}
		if(grpBy == GroupBy.ACTIVE || grpBy == GroupBy.CUSTOM){
			this.showGroup = false;
		}
		else{
			this.showGroup = true;
		}
		if(grpBy != GroupBy.CUSTOM && grpBy != GroupBy.PRODUCT){
			selectedFilters.setActive(true);
		}else if(grpBy==GroupBy.PRODUCT){
			selectedFilters.setActive(sessionBean.getCustomFilter().isActive());
		}
		else{
			selectedFilters.setActive(false);
		}
		this.groupBy = grpBy;
		sessionBean.setSelectedGroupBy(grpBy);
		PreferencesUtil.getInstance().setAlertsPreferencesInSession(sessionBean);
		refreshAlertList();
		if(this.showGroup){
			BindUtils.postNotifyChange(null, null, this, "alertGroupingModel");
		}
		BindUtils.postNotifyChange(null, null, this, "showGroup");
	}
	
	private Comparator<Alert> getComparator(GroupBy grpBy){
		switch(grpBy){
    	case INSTANCE :
    		return instanceComparator;
    	case SEVERITY :
    		return severityComparator;
    	case METRIC :
    		return metricComparator;
    	case CATEGORY :
    		return categoryComparator;
    	case PRODUCT : 
    		return repoComparator;
    	default : return instanceComparator;
    	}
	}
	
	@Command("setAlertsRowsCount")
	public void setAlertsRowsCount() {
		setAlertListRowsCount();
	}
	
	@Listen("onOK = #alertListRowsBox")
	public void setAlertListRowsCount() {
		try {
			int pageSize = alertListRowsBox.getValue();
			alertsListPgId.setPageSize(pageSize);
			PreferencesUtil.getInstance().setAlertsPreferencesInSession(null, null, this.alertsListRowsCount);
			log.debug("setting pageCount as " + this.alertsListRowsCount);prevPageSize = pageSize;
		} catch (Exception ex) {
			log.error("Invalid value provided for alert row configuration. Row count provided:" + alertListRowsBox.getValue());
			alertsListPgId.setPageSize(prevPageSize);
		}
	}
	
	
	@Command
	public void onClickSelectedAlertInstance(@BindingParam("instanceid") String instanceId) {
		SessionUtil.getSecurityContext();
		Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance"+"/"+instanceId));
	}
	
	@Command
	public void onClickSelectedAlert(@BindingParam("alert") Object alert) {
		SessionUtil.getSecurityContext();
		Map<Object, Object> args = new HashMap<Object, Object>();
		args.put("alert", alert);
		args.put("alertsList", alertsList);
		Window window = (Window)Executions.createComponents(
                "~./sqldm/com/idera/sqldm/ui/alerts/alertsView.zul", null, args);
        window.doModal();
    }
	
	public int getAlertsListRowsCount() {
		return alertsListRowsCount;
	}

	public int getListRowsCount() {
		return listRowsCount;
	}

	public void setAlertsListRowsCount(int instanceListRowsCount) {
		this.alertsListRowsCount = instanceListRowsCount;
		BindUtils.postNotifyChange(null, null, this, "instanceListRowsCount");
	}

	public void setListRowsCount(int listRowsCount) {
		this.listRowsCount = listRowsCount;
		BindUtils.postNotifyChange(null, null, this, "listRowsCount");
	}
	
	public AlertsGroupingModel getAlertGroupingModel() {
		return alertGroupingModel;
	}

	public void setAlertGroupingModel(AlertsGroupingModel alertGroupingModel) {
		this.alertGroupingModel = alertGroupingModel;
	}

	public boolean isShowGroup() {
		return showGroup;
	}

	public void setShowGroup(boolean showGroup) {
		this.showGroup = showGroup;
	}
	
	public List<Alert> getAlertsList() {
		return alertsList;
	}

	public void setAlertsList(List<Alert> alertsList) {
		this.alertsList = alertsList;
	}

	public ListModelList getAlertsModel() {
		return alertsModel;
	}

	public void setAlertsModel(ListModelList alertsModel) {
		this.alertsModel = alertsModel;
	}

	public Comparator<Alert> getInstanceComparator() {
		return instanceComparator;
	}

	public void setInstanceComparator(Comparator<Alert> instanceComparator) {
		this.instanceComparator = instanceComparator;
	}

	public Comparator<Alert> getSeverityComparator() {
		return severityComparator;
	}

	public Comparator<Alert> getDatabaseComparator() {
		return databaseComparator;
	}

	public Comparator<Alert> getCategoryComparator() {
		return categoryComparator;
	}

	public Comparator<Alert> getDateComparator() {
		return dateComparator;
	}

	public String getEmptyGridMessage() {
		return emptyGridMessage;
	}

	public void setEmptyGridMessage(String emptyGridMessage) {
		this.emptyGridMessage = emptyGridMessage;
		BindUtils.postNotifyChange(null, null, this, "emptyGridMessage");
	}

	public boolean isShowWarningMsg() {
		return showWarningMsg;
	}

	public void setShowWarningMsg(boolean showWarningMsg) {
		this.showWarningMsg = showWarningMsg;
		BindUtils.postNotifyChange(null, null, this, "showWarningMsg");
	}
	
	
	private void setOffSetInSecs(){
		if (Sessions.getCurrent() != null) {
			offSetInSecs = (Integer) Sessions.getCurrent().getAttribute(
					WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET);
			offSetInSecs/=1000;
		}
		
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