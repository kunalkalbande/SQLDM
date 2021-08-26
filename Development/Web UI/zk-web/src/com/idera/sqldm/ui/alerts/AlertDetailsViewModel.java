package com.idera.sqldm.ui.alerts;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;

import org.apache.commons.lang.math.NumberUtils;
import org.apache.log4j.Logger;
import org.zkoss.zhtml.Messagebox;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zkplus.databind.AnnotateDataBinder;
import org.zkoss.zul.Button;
import org.zkoss.zul.Div;
import org.zkoss.zul.Image;
import org.zkoss.zul.Label;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Toolbarbutton;
import org.zkoss.zul.Window;

import com.idera.cwf.model.Product;
import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.alerts.Alert;
import com.idera.sqldm.data.alerts.GridAlert;
import com.idera.sqldm.data.alerts.Alert.AlertSeverity;
import com.idera.sqldm.data.alerts.AlertFacade;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.rest.TimedValue;
import com.idera.sqldm.rest.UnauthorizedAccessException;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.server.web.WebUtil;
import com.idera.sqldm.server.web.session.SessionUtil;
import com.idera.sqldm.ui.components.charts.line.IderaLineChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.ui.dashboard.instances.InstanceCategoryTab;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstancePreferencesBean;
import com.idera.sqldm.utils.Utility;
public class AlertDetailsViewModel extends SelectorComposer<Window> {
	
	private static final Logger log = Logger.getLogger(AlertDetailsViewModel.class);
	private String OFFSET_IN_HOURS = "0.0";
	
	enum AlertHistoryType{
		fourHours("Last 4 hours", "HH:mm", 4), oneDay("Last 24 hours", "HH:mm", 24), oneWeek("Last 7 days", "dd MMM, HH:mm", 168);
		
		private String title;
		private SimpleDateFormat format;
		private int numHours;
		
		AlertHistoryType(String title, String printFormat, int numHours){
			this.title = title;
			this.format = new SimpleDateFormat(printFormat);
			this.numHours = numHours;
		}
		public String getTitle() {
			return title;
		}

		public SimpleDateFormat getFormat() {
			return format;
		}
		public int getNumHours() {
			return numHours;
		}				

	}

	private static final long serialVersionUID = 1L;
	@Wire
	IderaLineChart alertsActivity;
	private AnnotateDataBinder binder;
	
	@Wire
    private Window alertDetailWindow;
	@Wire private Button nextBtn;
	@Wire private Button previousBtn;
	@Wire private Label statusLabel;
	@Wire private Image statusImg;
	@Wire Label metricHistoryTitle;
	@Wire Textbox alertDetail;
	@Wire Div metricsDetails;
	@Wire Toolbarbutton toolbarLaunch;
	private Alert alert;
	private Alert newAlert;
	private List<Alert> alertsList;
	private Product product;
	private int currentIndex = -1;
	
	
	@SuppressWarnings("unchecked")
	@Override
	public void doAfterCompose(Window comp) throws Exception {
		setOffSet();
        super.doAfterCompose(comp);
        try {
			alert = toGridAlert((GridAlert) Executions.getCurrent().getArg().get("alert"));
		} catch (ClassCastException e) {
			// TODO Auto-generated catch block
			alert = (Alert) Executions.getCurrent().getArg().get("alert");
		}
        alertsList = new ArrayList<>();
		try {
			alertsList.addAll(toAlertsList((List<GridAlert>) Executions.getCurrent().getArg().get("alertsList")));	
		} catch (ClassCastException e) {
			// TODO Auto-generated catch block
			alertsList.addAll((List<Alert>) Executions.getCurrent().getArg().get("alertsList"));	
		}
       // product = (Product) Executions.getCurrent().getArg().get("product");
        product = alert.getProduct();
        updateCurrentIndex();
        binder = new AnnotateDataBinder(comp);
        refreshAlertModel(alert.getAlertId());
    }
	
	
	private void setEmptyChart(){
		alertsActivity.setErrorMessage(alertsActivity.getTitle() + "\n "+Utility.getMessage(SQLdmI18NStrings.DATA_NOT_AVAILABLE));
	}
	
	private void refreshAlertModel(Long alertId){
		refreshAlert(alertId);
		binder.bindBean("alertBean", alert);
		String instanceName  = "";
		try {
			instanceName = URLEncoder.encode(alert.getInstanceName() , "UTF-8");
		} catch (UnsupportedEncodingException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		toolbarLaunch.setHref(ELFunctions.getAlertSpecificURL(alert.getAlertId()+"", alert.getInstanceId()+"", instanceName));
		binder.loadAll();
		alertDetail.setValue(alert.getDescription());
		if(NumberUtils.isNumber(alert.getStringValue()) && alert.getStringValue() != "-1"){
			metricsDetails.setVisible(true);
			refreshAlertHistory(AlertHistoryType.fourHours);
		}
		else{
			metricsDetails.setVisible(false);
		}
		
        setStatusImageSrc();
		statusLabel.setValue(getStatusTitle());
		updateNextPreviousButtons();
	}
	
	private void refreshAlert(Long alertId) {
/*		String productInstanceName = Utility.getUrlParameter(Executions
				.getCurrent().getParameterMap(), "instance");
		if (productInstanceName == null) {
			DashboardPreferencesBean dbpb = PreferencesUtil.getInstance()
					.getDashboardPreferencesInSession();
			if (dbpb != null) {
				productInstanceName = dbpb.getProductInstanceName();
			}

		}*/
		String productInstanceName = "";
		if(alert!=null && alert.getProduct()!=null)
		 productInstanceName = alert.getProduct().getInstanceName();
		try {
			this.newAlert = AlertFacade.getAlert(productInstanceName, alertId);
		} catch (UnauthorizedAccessException x) {
			Messagebox.show(ELFunctions.getMessage(SQLdmI18NStrings.EXCEPTION_OCCURRED_INVALID_INSTANCE), "Error", Messagebox.OK,
					Messagebox.ERROR, new EventListener<Event>() {
						
						@Override
						public void onEvent(Event arg0) throws Exception {
							Executions.sendRedirect("/");							
						}
					});

		} catch (Exception e) {
			log.error(e.getMessage(), e);
		}
	}

	private void setStatusImageSrc(){
		/*${idera:getImageURLWithoutSize('warning32x32')}*/
		String icon;
		switch(alert.getSeverity()){
		case 1:
			icon = "ok32x32";
			break;
		case 2:
			icon = "Information32x32";
			break;
		case 4:
			icon = "warning32x32";
			break;
		case 8:
			icon = "critical32x32";
			break;
		default : icon = "ok32x32";
		break;
		}
		statusImg.setSrc(ELFunctions.getImageURLWithoutSize(icon));
	}
	
	private String getStatusTitle(){
		switch(alert.getSeverity()){
		case 1:
			return AlertSeverity.OK.name();
			
		case 2:
			return AlertSeverity.INFORMATIONAL.name();
		case 4:
			return AlertSeverity.WARNING.name();
		case 8:
			return AlertSeverity.CRITICAL.name();
		default : return AlertSeverity.OK.name();
		}
	}
	
	private void updateCurrentIndex(){
		int i = 0;
		for(Alert alrt : alertsList){
			if(this.alert.getAlertId().equals(alrt.getAlertId())){
				currentIndex = i;
				break;
			}
			i++;
		}
	}
	
	private void refreshAlertHistory(AlertHistoryType historyFor){
		List<TimedValue> metricsHistory = new ArrayList<>();
    	String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");

		try{
			metricsHistory.addAll(AlertFacade.getMetricsHistoryForAlert(productInstanceName,alert.getAlertId(), historyFor.getNumHours() , OFFSET_IN_HOURS));
		}
		catch(Exception e){
		}
		alertsActivity.setTitle(historyFor.getTitle());
		
		if(metricsHistory.isEmpty()){
			setEmptyChart();
			return;
		}
		alertsActivity.refresh();
		try{
			IderaLineChartModel model = getAlertHistoryModel(metricsHistory, historyFor);
			alertsActivity.setModel(model);
            //Set format if it 7-days
            if(historyFor.equals(AlertHistoryType.oneWeek)){
                alertsActivity.getChart().setXAxisTickFormat("%d-%b-%y");
            }
		}
		catch(Exception e){
		}
		
	}
	
	private IderaLineChartModel getAlertHistoryModel(List<TimedValue> metricsHistory, AlertHistoryType historyFor) {
		//SimpleDateFormat printFormat = historyFor.getFormat();
		IderaLineChartModel model = new IderaLineChartModel();
		//int x_value = 0;
		if (metricsHistory.size() > 0) {
			TimedValue tv = metricsHistory.get(metricsHistory.size() - 1);
			model.setValue("Metric History", tv.getUtcUpdated().getTime() - 1, 0.0 );
		}
		for (int i = metricsHistory.size() - 1; i >= 0; i--) {
			TimedValue tv = metricsHistory.get(i);
			 //printFormat.format(alert.getUtcUpdated().getTime())
        	if(tv.getValue() == null || tv.getUtcUpdated() == null){
				continue;
			}
        	model.setValue("Metric History", tv.getUtcUpdated().getTime(), Utility.round(tv.getValue(), 2) );
        }
		return model;
	}
	
	@Listen("onClick = #fourHoursThresholds, #twentyFourHoursThresholds, #sevenDaysThresholds")
	public void refreshHistoryChart(Event e){
		Calendar startTime = Calendar.getInstance();
		startTime.setTimeInMillis(startTime.getTimeInMillis());
		AlertHistoryType historyFor;
		if("twentyFourHoursThresholds".equals(e.getTarget().getId())){
			startTime.add(Calendar.DATE, -1);
			historyFor = AlertHistoryType.oneDay;
			metricHistoryTitle.setValue(ELFunctions.getLabel("SQLdm.Labels.history.twenty-four-hours"));
		}
		else if("sevenDaysThresholds".equals(e.getTarget().getId())){
			startTime.add(Calendar.DATE, -7);
			historyFor = AlertHistoryType.oneWeek;
			metricHistoryTitle.setValue(ELFunctions.getLabel("SQLdm.Labels.history.seven-days"));
		}
		else{
			startTime.add(Calendar.HOUR_OF_DAY, -4);
			historyFor = AlertHistoryType.fourHours;
			metricHistoryTitle.setValue(ELFunctions.getLabel("SQLdm.Labels.history.four-hours"));
		}
		SessionUtil.getSecurityContext();
		refreshAlertHistory(historyFor);
	}

	@Listen("onClick = #nextBtn")
	public void getNextAlert(){
		if(currentIndex >= alertsList.size()-1){
			updateNextPreviousButtons();
			return;
		}
		SessionUtil.getSecurityContext();
		currentIndex++;
		this.alert = alertsList.get(currentIndex);//SQLDM-31301
		metricHistoryTitle.setValue(ELFunctions.getLabel("SQLdm.Labels.history.four-hours"));
		refreshAlertModel(this.alert.getAlertId());
	}
	
	@Listen("onClick = #previousBtn")
	public void getPreviousAlert(){
		if(currentIndex <= 0){
			updateNextPreviousButtons();
			return;
		}
		SessionUtil.getSecurityContext();
		currentIndex--;
		this.alert = alertsList.get(currentIndex);
		metricHistoryTitle.setValue(ELFunctions.getLabel("SQLdm.Labels.history.four-hours"));
		refreshAlertModel(this.alert.getAlertId());
	}
	
	@Listen("onClick = #instanceViewBtn")
	public void redirectToInstanceDetail(){
	     if(product==null)
	    	 Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance"+"/"+alert.getInstanceId()));
	     else
	     {
	    	 
	    	 Executions.sendRedirect(WebUtil.buildPathRelativeToProduct(product , "singleInstance"+"/"+alert.getInstanceId()));
	     }
	}
	
	@Listen("onClick = #categoryViewBtn")
	public void redirectToCategoryView(){
		InstanceCategoryTab tab = InstanceCategoryTab.findByStatus(alert.getMetric().getMetricCategory().toLowerCase());
		int categoryId = tab == null ? 0 : tab.getId();
		Integer instanceId = new Integer(alert.getInstanceId());
        SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
        pref.setSelectedCategory(categoryId);
        if(categoryId == 3){
        	pref.setSelectedSubCategory(2);
        }
        else{
        	pref.setSelectedSubCategory(1);
        }
        PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);

        Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance"+"/"+alert.getInstanceId()));
	}
	
	@Listen("onClick = #closeBtn")
	public void closeWindow(){
		alertDetailWindow.detach();
	}
	

	
	public Alert getAlert(){
		return alert;
	}
	
	private void updateNextPreviousButtons(){
		if(currentIndex <= 0){
			previousBtn.setDisabled(true);
		}
		else{
			previousBtn.setDisabled(false);
		}
		
		if(currentIndex >= alertsList.size()-1){
			nextBtn.setDisabled(true);
		}
		else{
			nextBtn.setDisabled(false);
		}
		
	}

	private void setOffSet() {
		Double offSet = null;
		if (Sessions.getCurrent() != null) {
			offSet = new Double((Integer) Sessions.getCurrent().getAttribute(
					WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET))
					/ (1000 * 60.0 * 60.0);
			offSet = -offSet;
		}
		if (offSet != null)
			OFFSET_IN_HOURS = offSet.toString();
	}
	
	private Alert toGridAlert(GridAlert gridAlert){
		Alert alert = new Alert();
		alert.alertId = gridAlert.getAlertId();
		alert.name = gridAlert.getName();
		alert.instanceName = gridAlert.getInstanceName();
		alert.databaseName = gridAlert.getDatabaseName();
		alert.instanceId = gridAlert.getInstanceId();
		alert.isActive = gridAlert.getIsActive();
		alert.utcUpdated = gridAlert.getUtcUpdated();
		switch(gridAlert.getSeverity())
        {
			case "Ok":
				alert.severity = 1;
				break;
			case "Informational":
				alert.severity = 2;
				break;
			case "Warning":
				alert.severity = 4;
				break;
			case "Critical":
				alert.severity = 8;
				break;
			default:
				alert.severity = 1;
			break;
		}
		alert.previousAlertSeverity = gridAlert.getPreviousAlertSeverity();
		alert.description = gridAlert.getDescription();
		alert.activeDuration = gridAlert.getActiveDuration();
		alert.Metric = gridAlert.getMetric();
		alert.value = gridAlert.getValue();
		alert.StringValue = gridAlert.getStringValue();
		alert.StateEvent = gridAlert.getStateEvent();
		alert.product = gridAlert.getProduct();
		
		return alert;
	}
	
	private List<Alert> toAlertsList(List<GridAlert> gridAlerts){
		List<Alert> alerts = new ArrayList<Alert>();
		
		for(GridAlert gridAlert : gridAlerts){
			alerts.add(toGridAlert(gridAlert));
		}
		
		return alerts;
	}
	
}
