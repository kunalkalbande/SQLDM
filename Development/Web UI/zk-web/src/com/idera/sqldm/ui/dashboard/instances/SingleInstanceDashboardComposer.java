package com.idera.sqldm.ui.dashboard.instances;


import java.io.IOException;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.List;

import org.apache.log4j.Logger;
import org.zkoss.json.JSONArray;
import org.zkoss.json.JSONObject;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Session;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.Datebox;
import org.zkoss.zul.Div;
import org.zkoss.zul.Groupbox;
import org.zkoss.zul.Image;
import org.zkoss.zul.Include;
import org.zkoss.zul.Label;
import org.zkoss.zul.Radio;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Timebox;

import com.idera.common.Utility;
import com.idera.common.rest.CoreRestClient;
import com.idera.common.rest.RestException;
import com.idera.cwf.model.Product;
import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.DashboardInstance;
import com.idera.sqldm.data.DashboardInstanceFacade;
import com.idera.sqldm.data.InstanceException;
import com.idera.sqldm.data.UserSettingFacade;
import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.rest.SQLDMRestClient;
import com.idera.sqldm.rest.UnauthorizedAccessException;
import com.idera.sqldm.server.web.WebUtil;
import com.idera.sqldm.server.web.session.UserPreferences;
import com.idera.sqldm.server.web.session.UserSessionSettings;
import com.idera.sqldm.ui.dashboard.instances.queryWaits.QueryChartOptionsEnum;
import com.idera.sqldm.ui.preferences.HistoryPanelPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstancePreferencesBean;
//import com.idera.ccl.IderaDropdownList;



public class SingleInstanceDashboardComposer extends SelectorComposer<Div> {
	private static final Logger log = Logger.getLogger(SingleInstanceDashboardComposer.class); 
	public static final String INSTANCE_CATEGORY_LOAD_EVENT_QUEUE = "instance-category-load-event-queue";
	static final long serialVersionUID = 1L;
	DashboardInstance instance;
	int instanceId;
	@Wire
	protected Label lblInstanceName;

	
	//@Wire
	//protected Image imgInstanceStatus;

	/*@Wire
	protected Tabbox categoryTab;*/
	@Wire private Groupbox resources;
	@Wire private Groupbox queries;
	@Wire private Groupbox querieswait;
	@Wire private Groupbox sessions;
	@Wire private Groupbox databases;
	@Wire private Groupbox overview;
	@Wire private Groupbox alerts;
	@Wire private Image launchSWA;
	@Wire private Label swaInstance;
	@Wire private Datebox fromDateBoxComponent;
	@Wire private Datebox toDateBoxComponent;
	@Wire private Timebox fromTimeBoxComponent;
	@Wire private Timebox toTimeBoxComponent;
	@Wire private Combobox scaleCombobox;
	@Wire private Div historyPanel;
	@Wire private Radio overviewInstanceName,sessionsOverview,sessionsSessionGraph,queriesTab1,querieswaitWaits,querieswaitWaitCategory,
	querieswaitStatements,querieswaitApplication,querieswaitDatabases,querieswaitClients,querieswaitSessions,querieswaitUsers,resourcesCpu,
	resourcesMemory,resourcesDisk,resourcesServerWaits,databasesSummary,databasesTempdb,databasesAvailabilityGroups,alertsAlerts;
	@Wire private Div contentAreaDiv;
	@Wire private Radiogroup tabs;
	private Double offsetHours;
	private String tabName;
	@Wire private Label divInstanceName;
	private Integer tabId = 0;
	

	@Override
	public void doAfterCompose(Div comp) throws Exception {
		super.doAfterCompose(comp);

		String instanceName = Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "name");
		String domainName = Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "domainname");
		String instanceNameDomain = Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instancenameswa");
		Integer instanceIdParameter = Utility.getIntUrlParameter(Executions.getCurrent().getParameterMap(), "id");
		int flag = 0;
		if(domainName!=null && instanceNameDomain != null){
			instanceName=domainName+"\\"+instanceNameDomain;
		}
		if (instanceIdParameter != null) {
			instanceId = instanceIdParameter;
		} else {
			if(instanceName != null){
				try	
				{
					String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
					List<DashboardInstance> instanceFiltered= DashboardInstanceFacade.getDashboardInstances(productInstanceName, "InstanceName",instanceName);
					for(int i = 0; i < instanceFiltered.size();i++){
						DashboardInstance checkInstance = instanceFiltered.get(i);
						if(checkInstance.getOverview().getInstanceName().equals(instanceName)){
							instance = instanceFiltered.get(i);
							break;
						}
					}
					flag =1;
				} catch (InstanceException x) {
					log.error("Exception while pulling single instance detail, (instance id="+instanceId+"): ", x);
				}
			}else{
				//fallback
				Object param = Executions.getCurrent().getDesktop().getAttribute("instanceId");
				if(param != null){
					instanceId = (Integer) param;
				}
			}
		}
		String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");

		try
		{
			if(flag==0){
				instance= DashboardInstanceFacade.getDashboardInstance(productInstanceName, instanceId);
			}
		} catch (InstanceException x) {
			log.error("Exception while pulling single instance detail, (instance id="+instanceId+"): ", x);
		} catch (UnauthorizedAccessException x) {
			Executions.sendRedirect("/");
			return;
		}
		if(flag==0){
			Executions.getCurrent().getDesktop().setAttribute("instanceId", instanceId);
		}
		else if(flag==1){
			Executions.getCurrent().getDesktop().setAttribute("instanceId", instance.getOverview().getSQLServerId());
		}
		Executions.getCurrent().getDesktop().setAttribute("instance", productInstanceName);

		if (instance == null || instance.getOverview() == null) {
			Executions.sendRedirect("/");
			return;
		}
		if((!instance.getIsSWAInstance())){
			launchSWA.setVisible(false);
			swaInstance.setVisible(false);
		}
		else{
			launchSWA.setVisible(true);
			swaInstance.setVisible(true);
		}
		

		//		Executions.getCurrent().getDesktop().setAttribute("instanceName", instance.getOverview().getInstanceName());
		Executions.getCurrent().getDesktop().setAttribute("instanceName", instance.getOverview().getDisplayName());

		//		lblInstanceName.setValue(instance.getOverview().getInstanceName());
		lblInstanceName.setValue(instance.getOverview().getDisplayName());
		divInstanceName.setValue(lblInstanceName.getValue());
		//		if (instance.getServerStatus() != null && instance.getServerStatus().getMaxSeverity() != null) {
		//			SeverityCodeToStringEnum sctse = SeverityCodeToStringEnum.getSeverityEnumForId(instance.getServerStatus().getMaxSeverity());
		//			imgInstanceStatus.setSrc(ELFunctions.getImageURLWithoutSize(sctse.getServerCat16PixIcon()));
		//		}
		tabId = Utility.getIntUrlParameter(Executions.getCurrent().getParameterMap(), "tab");
		/*SingleInstancePreferencesBean pref = null;
		if (tabId == null || tabId < 0 || tabId > 6) {
			if(flag==0){
				pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
			}
			else if(flag==1){
				pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instance.getOverview().getSQLServerId());
			}
			tabId = pref.getSelectedCategory();
		}*/
		loadUserSettings();
		setDateTime(null);
		onClickRadioButton(null);
		/*BindUtils.postGlobalCommand(INSTANCE_CATEGORY_LOAD_EVENT_QUEUE, EventQueues.DESKTOP, tabName, null);
		categoryTab.setSelectedIndex(tabId);*/
	}
	
	
	
	/*
	 * @author ChaitanyaTanwar DM 10.2
	 * */
	@Listen("onChange = #scaleCombobox;onChange = #fromDateBoxComponent;onChange = #toDateBoxComponent;onChange = #fromTimeBoxComponent;onChange = #toTimeBoxComponent")
	public void setDateTime(Event evt) throws RestException, IOException, CategoryException, ParseException {
		Calendar c = Calendar.getInstance();
		Double offSet = null;
		HistoryPanelPreferencesBean pref1 = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		
		/**
		 * Required for SQLDM-27428 : saving time/date before change of history pref
		 */
		HistoryPanelPreferencesBean prefBeforeChange =  new HistoryPanelPreferencesBean();
		copyDateTimeRanges(pref1,prefBeforeChange);
				
		String range = null;
		if(pref1.getScaleCombobox()==null){
			range = "1 Hour";
			disableTimeRangeComponent(true);
		}
		else if(!pref1.getScaleCombobox().getValue().equals("Custom")){
			disableTimeRangeComponent(true);
			range = pref1.getScaleCombobox().getValue();
		}
		else{
			
			disableTimeRangeComponent(false);
			try{
				toDateBoxComponent.setValue(pref1.getTodate().getValue());
				toTimeBoxComponent.setValue(pref1.getTotime().getValue());
				fromDateBoxComponent.setValue(pref1.getFromdate().getValue());
				fromTimeBoxComponent.setValue(pref1.getFromtime().getValue());
			}catch(WrongValueException wex){ // case added for cases like "deleting the date from custom date/time and reloading the page"
				resetDateTimeComponent(prefBeforeChange);
			}
			catch (NullPointerException ex){
				log.error("Exception Occured during Resetting start and end Date/Time : Exception - " + ex.getMessage() );
				resetDateTimeComponent(prefBeforeChange);
			}
			range = pref1.getScaleCombobox().getValue();
		}
		scaleCombobox.setValue(range);
		pref1.setScaleCombobox(scaleCombobox);
		if(Sessions.getCurrent()!=null)
		{
			offSet = new Double((Integer)Sessions.getCurrent().getAttribute(WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET))/(1000*60.0*60.0);
			offSet = -offSet;
		}
		if(offSet!=null)
			offsetHours = Double.parseDouble(offSet.toString());
		else
			offsetHours = Double.parseDouble("0.0");
		c.setTime(new Date(c.getTimeInMillis()
				- (long) (offsetHours * 60 * 60 * 1000)));

		if(!range.equals("Custom")){
			toDateBoxComponent.setValue(new Date());
			toTimeBoxComponent.setValue(c.getTime());
			switch(range){
			case "4 Weeks" :
				c.set(Calendar.DATE, c.get(Calendar.DATE) - 28);
				pref1.setNumHistoryMinutes(40320);
				break;
			case "5 Days" :
				c.set(Calendar.DATE, c.get(Calendar.DATE) - 5);
				pref1.setNumHistoryMinutes(7200);
				break;
			case "1 Day" :
				c.set(Calendar.HOUR, c.get(Calendar.HOUR) - 24);
				pref1.setNumHistoryMinutes(1140);
				break;
			case "8 Hours" :
				c.set(Calendar.HOUR, c.get(Calendar.HOUR) - 8);
				pref1.setNumHistoryMinutes(480);
				break;
			case "4 Hours" :
				c.set(Calendar.HOUR, c.get(Calendar.HOUR) - 4);
				pref1.setNumHistoryMinutes(240);
				break;
			case "1 Hour" :
				c.set(Calendar.HOUR, c.get(Calendar.HOUR) - 1);
				pref1.setNumHistoryMinutes(60);
				break;
			case "15 Minutes" :
				c.set(Calendar.MINUTE, c.get(Calendar.MINUTE) - 15);
				pref1.setNumHistoryMinutes(15);
				break;
			}
			fromDateBoxComponent.setValue(c.getTime());
			fromTimeBoxComponent.setValue(c.getTime());			
		}
		
		pref1.setFromdate(fromDateBoxComponent);
		pref1.setFromDate(fromDateBoxComponent.getValue());
		pref1.setFromtime(fromTimeBoxComponent);
		pref1.setFromTime(fromTimeBoxComponent.getValue());
		pref1.setTodate(toDateBoxComponent);
		pref1.setToDate(toDateBoxComponent.getValue());
		pref1.setTotime(toTimeBoxComponent);
		pref1.setToTime(toTimeBoxComponent.getValue());
		try{
			if(evt!=null){
				checkDateTimeConstraints(pref1,evt, prefBeforeChange);
				String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
				saveUserSettings(productInstanceName,pref1);
			}
		}
		catch(WrongValueException wex){ 
			resetDateTimeComponent(prefBeforeChange);
		}	
		PreferencesUtil.getInstance().setHistoryPanelPreferencesInSession(pref1);
		if(evt !=null){
			if(evt.getTarget().getId().equals("scaleCombobox") && range.equals("Custom")){
					return;
			}
			else{
				EventQueue<Event> eq = EventQueues.lookup("historyChange1",
						EventQueues.DESKTOP, false);
				if (eq != null && !range.equals("Custom")) {
					eq.publish(new Event("historyRange", null, null));
				}
				else {
					if(eq != null)
						eq.publish(new Event("historyTime", null, null));
				}
				launchGraphChangesForSelectedTab();
			}
		}
			
	}
	
	/*
	 * Author:Accolite
	 * Date : 15th Nov, 2016
	 * History Panel - SQLDM- 10.2 release
	 * Every change in the history panel will be saved in DB
	 */
	private void saveUserSettings(String productInstanceName, HistoryPanelPreferencesBean pref) throws CategoryException, ParseException {
		JSONArray settings = new JSONArray();
		JSONObject elementSetting = new JSONObject();
		elementSetting.put("Key",UserPreferences.SQLDM_USER_PREFERENCE_LIST.get(0));
		elementSetting.put("Value", pref.getFromDateTime().toString());
		settings.add(elementSetting);
		elementSetting = new JSONObject();
		elementSetting.put("Key",UserPreferences.SQLDM_USER_PREFERENCE_LIST.get(1));
		elementSetting.put("Value", pref.getToDateTime().toString());
		settings.add(elementSetting);
		elementSetting = new JSONObject();
		elementSetting.put("Key",UserPreferences.SQLDM_USER_PREFERENCE_LIST.get(2));
		elementSetting.put("Value", pref.getScaleCombobox().getValue());
		settings.add(elementSetting);
		UserSettingFacade.saveUserSettings(productInstanceName,settings);
	}


	/*
	 * Author:Accolite
	 * Date : 15th Nov, 2016
	 * History Panel - SQLDM- 10.2 release
	 * Load the user history panel settings only when it is called first time after every logic. If condition checking the dirty key-value which is
	 * changed before leaving the if case
	 */
	private void loadUserSettings() throws CategoryException, ParseException {
		Session session = Sessions.getCurrent();
		UserSessionSettings settings = (UserSessionSettings)session.getAttribute(UserSessionSettings.SQLDM_USER_SESSION_SETTINGS_PROPERTY);
		HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		SimpleDateFormat parseDateFormat = new SimpleDateFormat("E MMM dd HH:mm:ss yyyy");
		try{
			if(settings.getUserData(UserPreferences.SQLDM_FRESH_LOGIN).equals("")){
				Date fromDateSetting = parseDateFormat.parse(settings.getUserData(UserPreferences.SQLDM_USER_PREFERENCE_LIST.get(0)).replace(" GMT", ""));
				Date toDateSetting = parseDateFormat.parse(settings.getUserData(UserPreferences.SQLDM_USER_PREFERENCE_LIST.get(1)).replace(" GMT", ""));
				pref.setFromDate(fromDateSetting);
				pref.setToDate(toDateSetting);
				Combobox scaleComboboxSetting = new Combobox();
				pref.setFromdate(new Datebox(fromDateSetting));
				pref.setTodate(new Datebox(toDateSetting));
				pref.setFromtime(new Timebox(fromDateSetting));
				pref.setTotime(new Timebox(toDateSetting));
				scaleComboboxSetting.setValue(settings.getUserData(UserPreferences.SQLDM_USER_PREFERENCE_LIST.get(2)));
				pref.setScaleCombobox(scaleComboboxSetting);
				PreferencesUtil.getInstance().setHistoryPanelPreferencesInSession(pref);
				settings.setUserData(UserPreferences.SQLDM_FRESH_LOGIN, "Logged In");
				session.setAttribute(UserSessionSettings.SQLDM_USER_SESSION_SETTINGS_PROPERTY, settings);
			}
		}
		catch(NullPointerException npe){
			log.info("History settings not available for this user");
		}
		
		
	}



	private void launchGraphChangesForSelectedTab() throws RestException, IOException {
		if(tabName.equals("resources"))
			updateResourcesTab();
		else if(tabName.equals("overview"))
			updateOverviewTab();
		else if(tabName.equals("sessions"))
			updateSessionsTab();
		else if(tabName.equals("alerts"))
			updateAlertsTab();		
		else if(tabName.equals("databases"))
			updateDatabaseTab();
	}



	private void checkDateTimeConstraints(HistoryPanelPreferencesBean pref, Event evt, HistoryPanelPreferencesBean prefBeforeChange) {
		String componentId = evt.getTarget().getId();
		if(!componentId.equals("scaleCombobox")){
			try{
				Date startTime = pref.getFromDateTime();
				Date endTime = pref.getToDateTime();
				if(pref.getToDateTime().after(new Date(new Date().getTime() - (long) (offsetHours * 60 * 60 * 1000))) ||
						pref.getFromDateTime().after(new Date(new Date().getTime() - (long) (offsetHours * 60 * 60 * 1000)))){
					copyDateTimeRanges(prefBeforeChange, pref);
					resetDateTimeComponent(prefBeforeChange);
					throw new WrongValueException(evt.getTarget(),"Future date and time is not allowed. Resetting it to previous value.");
				}
				long MILLS_IN_YEAR = 1000L * 60 * 60 * 24 * 366;
				long duration  = endTime.getTime() - startTime.getTime();
				if(duration < 0){
					copyDateTimeRanges(prefBeforeChange, pref);
					resetDateTimeComponent(prefBeforeChange);
					if(evt.getTarget().getId().equals("fromdate") || evt.getTarget().getId().equals("fromtime"))
						throw new WrongValueException(evt.getTarget(),"Start time should be less than end time. Resetting it to previous value.");
					else{
						throw new WrongValueException(evt.getTarget(),"End time should be greater than start time. Resetting it to previous value.");
					}
				}
				if(duration > MILLS_IN_YEAR){
					copyDateTimeRanges(prefBeforeChange, pref);
					resetDateTimeComponent(prefBeforeChange);
					throw new WrongValueException(evt.getTarget(),"Invalid Range! Maximum of 1 year range can be selected. Resetting it to previous value.");
				}
				
			}
			catch(NullPointerException e){
				throw new WrongValueException(evt.getTarget(),"This field can't be empty.");
			}
		}
		
	}
	
	/*
	 * @author ChaitanyaTanwar
	 * */
	@Listen("onClick = #launchSWA")
	public void launchSWA() throws IOException, RestException{
		try{
			int swaID = instance.getSwaID();
			String instanceName = instance.getOverview().getInstanceName();
			boolean checkSWAUninstalled = true;
			List<Product> products = CoreRestClient.getInstance().getAllProducts();
			for(int noOfProducts = 0 ; noOfProducts < products.size(); noOfProducts++){
				Product product = products.get(noOfProducts);
				if (product.getProductNameWithoutInstanceName().equalsIgnoreCase("SQLWorkloadAnalysis")) {
					if(swaID == product.getProductId()){
						checkSWAUninstalled = false;
						break;
					}
				}
			}
			if(checkSWAUninstalled)
				Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("/home"));			
			else
				Executions.sendRedirect(SQLDMRestClient.getInstance().getSWAurl(swaID,instanceName));
		}
		catch(Exception e){
			log.error("Error In Launching SWA : " + e.getMessage());
			Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("/home"));
		}
	}
	@Listen("onCheck = #queriesTab1,#querieswaitWaits,#querieswaitWaitCategory,#querieswaitStatements,#querieswaitApplication,#querieswaitDatabases,#querieswaitClients,#querieswaitSessions,#querieswaitUsers,#databasesSummary")
	public void hideHistoryPanel() throws IOException, RestException{
		/**
		 * SQLDM-27469
		 * disabling redirection when date/time boxes are empty
		 */
		if(dateTimeBoxIsEmpty())
			return;
		historyPanel.setVisible(false);
	}
	@Listen("onCheck = #overviewInstanceName,#sessionsOverview,#sessionsSessionGraph,#resourcesCpu,#resourcesMemory,#resourcesDisk,#resourcesServerWaits,#alertsAlerts,#databasesAvailabilityGroups,#databasesTempdb")
	public void showHistoryPanel() throws IOException, RestException{
		/**
		 * SQLDM-27469
		 * disabling redirection when date/time boxes are empty
		 */
		if(dateTimeBoxIsEmpty())
			return;
		historyPanel.setVisible(true);
	}
	@Listen("onClick = #overview")
	public void updateOverviewTab() throws IOException, RestException{
		EventQueue<Event> eq = EventQueues.lookup("overviewHistory",
				EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("historyPanelChange", null, null));
		}
	}
	@Listen("onClick = #resources")
	public void updateResourcesTab() throws IOException, RestException{
		EventQueue<Event> eq = EventQueues.lookup("updateResourcesTab",
				EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("historyPanelChange", null, null));
		}
	}
	@Listen("onClick = #sessions")
	public void updateSessionsTab() throws IOException, RestException{
		EventQueue<Event> eq = EventQueues.lookup("updateSessionsTab",
				EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("historyPanelChange", null, null));
		}
	}
	@Listen("onClick = #alerts")
	public void updateAlertsTab() throws IOException, RestException{
		EventQueue<Event> eq = EventQueues.lookup("updateAlertsTab",
				EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("historyPanelChange", null, null));
		}
	}
	@Listen("onClick = #databases")
	public void updateDatabaseTab() throws IOException, RestException{
		EventQueue<Event> eq = EventQueues.lookup("updateDatabaseTab",
				EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("historyPanelChange", null, null));
		}
	}	
	
	/*@Subscribe(INSTANCE_CATEGORY_LOAD_EVENT_QUEUE)
	public void showCategoryTab(Event event) {
		Tab tab = null;
		if(event instanceof GlobalCommandEvent) {
			String command = ((GlobalCommandEvent)event).getCommand();
			if(InstanceCategoryTab.OVERVIEW.getStatus().equals(command)){
				tab = overview;
			}
			else if(InstanceCategoryTab.SESSIONS.getStatus().equals(command)){
				tab = sessions;
			} 
			else if (InstanceCategoryTab.RESOURCES.getStatus().equals(command)) { 
				tab = resources;
			}
			else if (InstanceCategoryTab.DATABASES.getStatus().equals(command)) { 
				tab = databases;
			}
			else if (InstanceCategoryTab.QUERIES.getStatus().equals(command)) { 
				tab = queries;
			}
			//else if (InstanceCategoryTab.QUERY_WAITS.getStatus().equals(command)) { 
			//	tab = queryWaits;
			//}
			else if (InstanceCategoryTab.ALERTS.getStatus().equals(command)) { 
				tab = alerts;
			}
			if(tab != null){
				Events.sendEvent("onSelect", tab, null);
				tab.setSelected(true);
			}
		}
	}*/
	
	public void callDrillDownChange(Date fromDate,Date toDate) throws CategoryException, ParseException{
		//checking if from date is after to date and swapping it appropriately
		
		if(fromDate.after(toDate)){
			Date temp = toDate;
			toDate=fromDate;
			fromDate=temp;
		}
		
		SimpleDateFormat printTimeFormat = new SimpleDateFormat("HH:mm:ss");
		
		String fromTime = printTimeFormat.format(fromDate);
		String toTime = printTimeFormat.format(toDate);
		HistoryPanelPreferencesBean pref1 = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
		try{
			if(toDate.getTime() - fromDate.getTime() >= 60*1000){//checking the difference of toDate and fromDate should be greater than 1 minute
				//updating Datebox object with the drill down from and to date
				fromDateBoxComponent.setValue(fromDate);
				fromTimeBoxComponent.setValue(fromDate);	
				toDateBoxComponent.setValue(toDate);
				toTimeBoxComponent.setValue(toDate);
				
				//Assigning from and to date/time to DateBox and TimeBox
					
				   pref1.setFromdate(fromDateBoxComponent);
				   pref1.setFromDate(fromDateBoxComponent.getValue());
				   pref1.setFromtime(fromTimeBoxComponent);
				   pref1.setFromTime(fromTimeBoxComponent.getValue());
				   pref1.setTodate(toDateBoxComponent);
				   pref1.setToDate(toDateBoxComponent.getValue());
				   pref1.setTotime(toTimeBoxComponent);
				   pref1.setToTime(toTimeBoxComponent.getValue());
				   pref1.getScaleCombobox().setValue("Custom");
				   disableTimeRangeComponent(false);
			}
			PreferencesUtil.getInstance().setHistoryPanelPreferencesInSession(pref1);
			String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
			saveUserSettings(productInstanceName,pref1);
			EventQueue<Event> eq = EventQueues.lookup("historyChange1",EventQueues.DESKTOP, false);
			if(eq != null){
				eq.publish(new Event("historyTime", null, null));
			}
			launchGraphChangesForSelectedTab();
			
		} catch (RestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
	}
	
	@Listen("onDrillChange=div#historyPanel")
	public void onDrillChange(Event evt) throws CategoryException, ParseException{
		
		String[] time = evt.getData().toString().split("&");
		String from = time[0];
		String to = time[1];
		Date fromDate= null;
		Date toDate= null;
		if(time[0].contains("UTC")){
			
			from = time[0].replaceAll("UTC\\++[0-9]*", " ");
			to = time[1].replaceAll("UTC\\++[0-9]*", " ");
			SimpleDateFormat parseDateFormat = new SimpleDateFormat("E MMM dd HH:mm:ss yyyy");
			SimpleDateFormat sdf = new SimpleDateFormat("E MMM dd yyyy HH:mm:ss ");
			fromDate=(Date)parseDateFormat.parse(from);
			toDate=(Date)parseDateFormat.parse(to);
			/*fromDate=sdf.format(fromDate);
			toDate=sdf.format(toDate);
			swaDate = (Date)parseDateFormat.parse(dateSWA);
			key = sdf.format(swaDate);*/
		}
		else{
			
			// replaced "GMT" from the String to parse the date
			from = time[0].replace("GMT", "");
			to = time[1].replace("GMT", "");
			SimpleDateFormat parseDateFormat = new SimpleDateFormat("E MMM dd yyyy HH:mm:ss ");
			//From date and Time
			fromDate=(Date)parseDateFormat.parse(from);
			//To date and time
			toDate=(Date)parseDateFormat.parse(to);
		}
		callDrillDownChange(fromDate, toDate);
		
	}
	
	public void disableTimeRangeComponent(Boolean flag){
			fromDateBoxComponent.setDisabled(flag);
			toDateBoxComponent.setDisabled(flag);
			fromTimeBoxComponent.setDisabled(flag);
			toTimeBoxComponent.setDisabled(flag);
	}
	
	@Listen("onCheck=#tabs")
	public void onClickRadioButton(Event evt){
		SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
		overviewInstanceName.setLabel(instance.getOverview().getDisplayName());
		overviewInstanceName.setTooltip(overviewInstanceName.getLabel());
		Integer queryWaitTabNo = -1;
		String subTabName;
		InstanceLeftPanelSubCategoryTab isct;
		/**
		 * SQLDM-27469
		 * disabling redirection when date/time boxes are empty
		 */
		if(dateTimeBoxIsEmpty())
			return;
		
		if(tabId==null){
			isct = InstanceLeftPanelSubCategoryTab.findById(pref.getSelectedCategory(), pref.getSelectedSubCategory());
		}
		else{
			isct = InstanceLeftPanelSubCategoryTab.findById(tabId, 1);
		}
		if(evt == null){
			subTabName = isct.getStatus();
		}else{
			subTabName = evt.getTarget().getId();
		}		
		String existingTabId = "";
		if(!contentAreaDiv.getChildren().isEmpty()){
			existingTabId = contentAreaDiv.getFirstChild().getId();
		}
		boolean loadTab = true;
		switch(subTabName){
		case "overviewInstanceName" :
			tabName = "overview";
			if(!overviewInstanceName.isChecked()){
				overviewInstanceName.setChecked(true);
				overview.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				loadTab = false;
			}
			pref.setSelectedCategory(InstanceCategoryTab.OVERVIEW.getId());
			pref.setSelectedSubCategory(1);
			break;
		case "sessionsOverview" :
			tabName = "sessions";
			if(!sessionsOverview.isChecked()){
				sessionsOverview.setChecked(true);
				sessions.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				loadTab = false;
			}
			pref.setSelectedCategory(InstanceCategoryTab.SESSIONS.getId());
			pref.setSelectedSubCategory(1);
			break;
		case "sessionsSessionGraph" :
			tabName = "sessions";
			if(!sessionsSessionGraph.isChecked()){
				sessionsSessionGraph.setChecked(true);
				sessions.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				loadTab = false;
			}
			pref.setSelectedCategory(InstanceCategoryTab.SESSIONS.getId());
			pref.setSelectedSubCategory(2);
			break;
		case "queriesTab1" :
			tabName = "queries";
			if(!queriesTab1.isChecked()){
				queriesTab1.setChecked(true);
				queries.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				loadTab = false;
			}
			pref.setSelectedCategory(InstanceCategoryTab.QUERIES.getId());
			pref.setSelectedSubCategory(1);
			break;
		case "querieswaitWaits" :
			tabName = "querieswait";
			if(!querieswaitWaits.isChecked()){
				querieswaitWaits.setChecked(true);
				querieswait.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				loadTab = false;
			}
			queryWaitTabNo = QueryChartOptionsEnum.WAITS.getTabId();
			pref.setSelectedCategory(InstanceCategoryTab.QUERY_WAITS.getId());
			pref.setSelectedSubCategory(0);
			break;
		case "querieswaitWaitCategory" :
			tabName = "querieswait";
			if(!querieswaitWaitCategory.isChecked()){
				querieswaitWaitCategory.setChecked(true);
				querieswait.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				loadTab = false;
			}
			queryWaitTabNo = QueryChartOptionsEnum.WAIT_CATEGORY.getTabId();
			pref.setSelectedCategory(InstanceCategoryTab.QUERY_WAITS.getId());
			pref.setSelectedSubCategory(queryWaitTabNo);
			break;
		case "querieswaitStatements" :
			tabName = "querieswait";
			if(!querieswaitStatements.isChecked()){
				querieswaitStatements.setChecked(true);
				querieswait.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				loadTab = false;
			}
			queryWaitTabNo = QueryChartOptionsEnum.STATEMENTS.getTabId();
			pref.setSelectedCategory(InstanceCategoryTab.QUERY_WAITS.getId());
			pref.setSelectedSubCategory(queryWaitTabNo);
			break;
		case "querieswaitApplication" :
			tabName = "querieswait";
			if(!querieswaitApplication.isChecked()){
				querieswaitApplication.setChecked(true);
				querieswait.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				loadTab = false;
			}
			queryWaitTabNo = QueryChartOptionsEnum.APPLICATION.getTabId();
			pref.setSelectedCategory(InstanceCategoryTab.QUERY_WAITS.getId());
			pref.setSelectedSubCategory(queryWaitTabNo);
			break;
		case "querieswaitDatabases" :
			tabName = "querieswait";
			if(!querieswaitDatabases.isChecked()){
				querieswaitDatabases.setChecked(true);
				querieswait.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				loadTab = false;
			}
			queryWaitTabNo = QueryChartOptionsEnum.DATABASES.getTabId();
			pref.setSelectedCategory(InstanceCategoryTab.QUERY_WAITS.getId());
			pref.setSelectedSubCategory(queryWaitTabNo);
			break;
		case "querieswaitClients" :
			tabName = "querieswait";
			if(!querieswaitClients.isChecked()){
				querieswaitClients.setChecked(true);
				querieswait.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				loadTab = false;
			}
			queryWaitTabNo = QueryChartOptionsEnum.CLIENTS.getTabId();
			pref.setSelectedCategory(InstanceCategoryTab.QUERY_WAITS.getId());
			pref.setSelectedSubCategory(queryWaitTabNo);
			break;
		case "querieswaitSessions" :
			tabName = "querieswait";
			if(!querieswaitSessions.isChecked()){
				querieswaitSessions.setChecked(true);
				querieswait.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				loadTab = false;
			}
			queryWaitTabNo = QueryChartOptionsEnum.SESSIONS.getTabId();
			pref.setSelectedCategory(InstanceCategoryTab.QUERY_WAITS.getId());
			pref.setSelectedSubCategory(queryWaitTabNo);
			break;
		case "querieswaitUsers" :
			tabName = "querieswait";
			if(!querieswaitUsers.isChecked()){
				querieswaitUsers.setChecked(true);
				querieswait.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				loadTab = false;
			}
			queryWaitTabNo = QueryChartOptionsEnum.USERS.getTabId();
			pref.setSelectedCategory(InstanceCategoryTab.QUERY_WAITS.getId());
			pref.setSelectedSubCategory(queryWaitTabNo);
			break;
		case "resourcesCpu" :
			tabName = "resources";
			if(!resourcesCpu.isChecked()){
				resourcesCpu.setChecked(true);
				resources.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				//loadTab = false;
			}
			pref.setSelectedCategory(InstanceCategoryTab.RESOURCES.getId());
			pref.setSelectedSubCategory(2);
			break;
		case "resourcesMemory" :
			tabName = "resources";
			if(!resourcesMemory.isChecked()){
				resourcesMemory.setChecked(true);
				resources.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				loadTab = false;
			}
			pref.setSelectedCategory(InstanceCategoryTab.RESOURCES.getId());
			pref.setSelectedSubCategory(3);
			break;
		case "resourcesDisk" :
			tabName = "resources";
			if(!resourcesDisk.isChecked()){
				resourcesDisk.setChecked(true);
				resources.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				//loadTab = false;
			}
			pref.setSelectedCategory(InstanceCategoryTab.RESOURCES.getId());
			pref.setSelectedSubCategory(4);
			break;
		case "resourcesServerWaits" :
			tabName = "resources";
			if(!resourcesServerWaits.isChecked()){
				resourcesServerWaits.setChecked(true);
				resources.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				//loadTab = false;
			}
			pref.setSelectedCategory(InstanceCategoryTab.RESOURCES.getId());
			pref.setSelectedSubCategory(6);
			break;
		case "databasesSummary" :
			tabName = "databases";
			if(!databasesSummary.isChecked()){
				databasesSummary.setChecked(true);
				databases.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				loadTab = false;
			}
			pref.setSelectedCategory(InstanceCategoryTab.DATABASES.getId());
			pref.setSelectedSubCategory(1);
			break;
		case "databasesTempdb" :
			tabName = "databases";
			if(databasesTempdb.isChecked()){
				databasesTempdb.setChecked(true);
				databases.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				//loadTab = false;
			}
			pref.setSelectedCategory(InstanceCategoryTab.DATABASES.getId());
			pref.setSelectedSubCategory(2);
			break;
		case "databasesAvailabilityGroups" :
			tabName = "databases";
			if(!databasesAvailabilityGroups.isChecked()){
				databasesAvailabilityGroups.setChecked(true);
				databases.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				//loadTab = false;
			}
			pref.setSelectedCategory(InstanceCategoryTab.DATABASES.getId());
			pref.setSelectedSubCategory(3);
			break;
		case "alertsAlerts" :
			tabName = "alerts";
			if(alertsAlerts.isChecked()){
				alertsAlerts.setChecked(true);
				alerts.setOpen(true);
			}
			if(existingTabId.equals(tabName+"Include")){
				loadTab = false;
			}
			pref.setSelectedCategory(InstanceCategoryTab.ALERTS.getId());
			pref.setSelectedSubCategory(1);
			break;
		
		}
		Sessions.getCurrent().setAttribute("isOverviewTab", tabName);
		PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);
		InstanceLeftPanelSubCategoryTab ilpsct = InstanceLeftPanelSubCategoryTab.findById(pref.getSelectedCategory(), pref.getSelectedSubCategory());
		if(loadTab){
			/*if((tabName+"Include").equals("querieswaitInclude")){
				SingleInstanceQueryWaitsPreferencesBean sessionBean = PreferencesUtil.getInstance().getQueryWaitsPreferenceInSession(instanceId);
				sessionBean.setSelectedOptionForCharting(queryWaitTabNo);
				PreferencesUtil.getInstance().setQueryWaitsPreferenceInSession(sessionBean);
			}*/
			Include in = new Include();
			in.setSrc(ilpsct.getUrl());
			in.setId(tabName+"Include");
			if(contentAreaDiv.getChildren().isEmpty()){
				contentAreaDiv.appendChild(in);
			}
			else{
				contentAreaDiv.getFirstChild().detach();
				contentAreaDiv.appendChild(in);
			}
		}else{
			EventQueue<Event> eq = EventQueues.lookup("tabChanged",
					EventQueues.DESKTOP, false);
			if (eq != null) {
				if(queryWaitTabNo < 0){
					eq.publish(new Event("tabSelected", null, pref));
				}
				else{
					eq.publish(new Event("querywaitTabSelected", null, queryWaitTabNo));
				}
			}
		}
	}

	private boolean dateTimeBoxIsEmpty() {
		if(fromDateBoxComponent.getValue() ==null || fromTimeBoxComponent.getValue() == null || toDateBoxComponent.getValue() == null
				|| toTimeBoxComponent.getValue() == null){
			return true;}
		return false;
	}
	
	/**
	 * Required for SQLDM-27428 : preserving the previous History Panel data before change in time/date box.
	 * @param fromHistPref
	 * @param toHistPref
	 */
	private void copyDateTimeRanges(HistoryPanelPreferencesBean fromHistPref, HistoryPanelPreferencesBean toHistPref) {
		toHistPref.setFromDate(fromHistPref.getFromDate());
		toHistPref.setToDate(fromHistPref.getToDate());
		toHistPref.setToTime(fromHistPref.getToTime());
		toHistPref.setFromTime(fromHistPref.getFromTime());
		
	}
	
	/**
	 * Required for SQLDM-27428: resetting Date/Time component base on time in pref parameter.
	 * @param pref
	 */
	private void resetDateTimeComponent(HistoryPanelPreferencesBean pref) {
		//System.out.println("resetting to "+ pref.getFromDate() + " " + pref.getFromTime() + ":"+ pref.getToDate() + " " + pref.getToTime());
		toDateBoxComponent.setValue(pref.getToDate());
		toTimeBoxComponent.setValue(pref.getToTime());
		fromDateBoxComponent.setValue(pref.getFromDate());
		fromTimeBoxComponent.setValue(pref.getFromTime());	
	}





}
