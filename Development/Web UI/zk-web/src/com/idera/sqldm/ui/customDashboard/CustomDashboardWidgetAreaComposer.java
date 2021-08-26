package com.idera.sqldm.ui.customDashboard;

import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;

import org.apache.log4j.Logger;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Session;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.WrongValueException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.event.InputEvent;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zkmax.zul.Chosenbox;
import org.zkoss.zkmax.zul.Portalchildren;
import org.zkoss.zkmax.zul.Portallayout;
import org.zkoss.zul.Button;
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Combobox;
import org.zkoss.zul.Datebox;
import org.zkoss.zul.Div;
import org.zkoss.zul.Label;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Panel;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

import com.idera.common.rest.RestException;
import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.customdashboard.CustomDashboard;
import com.idera.sqldm.data.customdashboard.CustomDashboardFacade;
import com.idera.sqldm.data.customdashboard.CustomDashboardWidget;
import com.idera.sqldm.data.customdashboard.CustomDashboardWidgetData;
import com.idera.sqldm.data.customdashboard.Types;
import com.idera.sqldm.io.executor.ParallelExecutions;
import com.idera.sqldm.io.executor.ParallelExecutionsPDU;
import com.idera.sqldm.server.web.session.UserSessionSettings;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.utils.SQLdmConstants;
import com.idera.sqldm.utils.Utility;

public class CustomDashboardWidgetAreaComposer extends SelectorComposer<Component>{
	
	private static final long serialVersionUID = 1L;
	private final Logger log = Logger.getLogger(CustomDashboardWidgetAreaComposer.class);
	@Wire private Label customDashboardNameLbl;
	@Wire private Combobox liveselectbox;
	@Wire private Div displayDBDiv,editDBDiv,customDashboardWidgetsDiv;
	@Wire private Portallayout layout;
	@Wire private Button editBtn,removeBtn,saveBtn,cancelBtn,addWidgetBtn,copyBtn;
	@Wire private Textbox customDashboardNametxtBox;
	@Wire private Datebox startDatebox,endDatebox;
	@Wire private Chosenbox dbtagChosenbox;
	@Wire private Checkbox dbIsDefaultcb;
	private String productInstanceName = null;
	private CustomDashboard customDashboard;
	private List<CustomDashboardWidget> dashboardWidgets;
	private Map<Integer,String> widgetIdMap = new HashMap<>();
	private Portalchildren pc;
	private Date startTime;
	private Date endTime;
	private String OFFSET_IN_HOURS = "0.0";
	private int OFFSET_IN_MILISECS = 0;
	
	public enum LiveFilterLabel {
		SELECTONE("Select One"), MINUTE("15 min"), ONEHOUR(
				"1 hr"), FOURHOUR("4 hr"),TWELVEHOUR("12 hr"),
				ONEDAY("1 day"),SEVENDAY("7 days")
				,ONEMONTH("1 month");

		private String label;

		private LiveFilterLabel(String label) {
			this.label = label;
		}

		private String getLabel() {
			return this.label;
		}
	};
	
	@Override
	public void doAfterCompose(Component comp) throws Exception {
		super.doAfterCompose(comp);
		
		DashboardPreferencesBean dbpb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();

		if(dbpb != null){
		productInstanceName = dbpb.getProductInstanceName();
		}
		setOffSet();
		setFromEndTime();
		setConstraintString();
		initializeWidgetTypes();
		initializeLiveCombobox();
		
			displayDBDiv.setVisible(true);
			editDBDiv.setVisible(false);
			dbtagChosenbox.setModel(new ListModelList<>());
		
		EventQueue<Event> eqForCustomDashboard = EventQueues.lookup(
				"customdashboard", EventQueues.SESSION,
				true);
		eqForCustomDashboard.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if ("onCreateCustomDashboard".equalsIgnoreCase(event.getName())) {
					clearCustomDashboardWidgets();
					customDashboard = (CustomDashboard)event.getData();
					setDashboardDetails();
					editCustomDashboard();
				}
				if ("onDashboardLinkClick".equalsIgnoreCase(event.getName())) {
					customDashboard = (CustomDashboard)event.getData();
					setDashboardDetails();
					addAllCustomDashboardWidgets();
					backToCustomDashboard();
				}
				if("customDashboardWidgetAdded".equalsIgnoreCase(event.getName())) {
					addAllCustomDashboardWidgets();
					editCustomDashboard();
				}
				if("onWidgetRemove".equalsIgnoreCase(event.getName())) {
					resize();
				}
			}
		});
		EventQueue<Event> eq = EventQueues.lookup("deleteDashboard",
				EventQueues.DESKTOP, true);
		eq.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if (event.getName().equals("removeCustom")) {
					deleteCustomDashboard();
					resize();
				}
			}
		});
		
	}
	
	private void resize() {
		Clients.resize(layout);
		Clients.resize(customDashboardWidgetsDiv);
		Clients.resize(customDashboardWidgetsDiv.getRoot());
	}
	
	public void setConstraintString() {
		Calendar  cal = Calendar.getInstance();
		cal.add(Calendar.MILLISECOND,OFFSET_IN_MILISECS);
		String day = "" +cal.get(Calendar.DAY_OF_MONTH);
		String month = "" + (cal.get(Calendar.MONTH) + 1);
		if(day.length() < 2)
			day = "0"+day;
		if(month.length() < 2)
			month = "0"+month;
		String constr = "no empty, before "+ cal.get(Calendar.YEAR) + month + day+": Date in future is not allowed.";
		startDatebox.setConstraint(constr);
		endDatebox.setConstraint(constr);
	}
	
	private void initializeLiveCombobox() {
		List<String> l = new LinkedList<>();
		
		for(LiveFilterLabel label : LiveFilterLabel.values()){
			l.add(label.getLabel());
		}
		
		ListModelList<String> model = new ListModelList<>(l);
		model.addToSelection(model.get(1));
		liveselectbox.setModel(model);
	}
	
	@Listen("onChange = #liveselectbox")
	public void onSelectboxChange() {
		Calendar c = Calendar.getInstance();
		c.add(Calendar.MILLISECOND, OFFSET_IN_MILISECS);
		endTime = c.getTime();
		
		String selectedTimeframe = liveselectbox.getSelectedItem().getLabel();
		
		if(LiveFilterLabel.MINUTE.getLabel().equals(selectedTimeframe)) {
			c.add(Calendar.MINUTE, -15);
			startTime = c.getTime();
			updateWidget();
		}
		
		if(LiveFilterLabel.ONEHOUR.getLabel().equals(selectedTimeframe)) {
			c.add(Calendar.HOUR_OF_DAY, -1);
			startTime = c.getTime();
			updateWidget();
		}
		
		if(LiveFilterLabel.FOURHOUR.getLabel().equals(selectedTimeframe)) {
			c.add(Calendar.HOUR_OF_DAY, -4);
			startTime = c.getTime();
			updateWidget();
		}
		
		if(LiveFilterLabel.TWELVEHOUR.getLabel().equals(selectedTimeframe)) {
			c.add(Calendar.HOUR_OF_DAY, -12);
			startTime = c.getTime();
			updateWidget();
		}
		
		if(LiveFilterLabel.ONEDAY.getLabel().equals(selectedTimeframe)) {
			c.add(Calendar.DAY_OF_MONTH, -1);
			startTime = c.getTime();
			updateWidget();
		}
		
		if(LiveFilterLabel.SEVENDAY.getLabel().equals(selectedTimeframe)) {
			c.add(Calendar.DAY_OF_MONTH, -7);
			startTime = c.getTime();
			updateWidget();
		}
		
		if(LiveFilterLabel.ONEMONTH.getLabel().equals(selectedTimeframe)) {
			c.add(Calendar.MONTH, -1);
			startTime = c.getTime();
			updateWidget();
		}
	}
	
	private void updateWidget(){
		EventQueue<Event> eqForSelectedTimeFrame = EventQueues.lookup(
				"customdashboard", EventQueues.SESSION,
				true);
		Map<String,Date> dateMap = new HashMap<String, Date>();
		startDatebox.setValue(startTime);
		endDatebox.setValue(endTime);
		dateMap.put("startTime", startTime);
		dateMap.put("endTime", endTime);
		eqForSelectedTimeFrame.publish(new Event("onDateTimeChange", null, dateMap));
		loadWidgetsData();
	}
	
	@Listen("onChange = #startDatebox,#endDatebox")
	public void onDateboxChange() {
		
		Date startDt,endDt;
		
		Calendar today = Calendar.getInstance();
		startDt = today.getTime();
		endDt = today.getTime();
		today.add(Calendar.MILLISECOND, OFFSET_IN_MILISECS);
		today.set(Calendar.HOUR_OF_DAY, 0);
		today.set(Calendar.MINUTE, 0);
		today.set(Calendar.SECOND, 0);
		today.set(Calendar.MILLISECOND, 0);
		
		Calendar startDate = Calendar.getInstance();
		startDate.setTime(startDatebox.getValue());
		startDate.set(Calendar.HOUR_OF_DAY, 0);
		startDate.set(Calendar.MINUTE, 0);
		startDate.set(Calendar.SECOND, 0);
		startDate.set(Calendar.MILLISECOND, 0);
		
		Calendar endDate = Calendar.getInstance();
		endDate.setTime(endDatebox.getValue());
		endDate.set(Calendar.HOUR_OF_DAY, 0);
		endDate.set(Calendar.MINUTE, 0);
		endDate.set(Calendar.SECOND, 0);
		endDate.set(Calendar.MILLISECOND, 0);

		Calendar cal = Calendar.getInstance();
		cal.add(Calendar.MILLISECOND, OFFSET_IN_MILISECS);
		cal.set(Calendar.SECOND, 0);
		cal.set(Calendar.MILLISECOND, 0);
		
		if(today.getTime().equals(endDate.getTime())) {
			endDt = cal.getTime();
		} else {
			endDate.set(Calendar.HOUR_OF_DAY, 23);
			endDate.set(Calendar.MINUTE, 59);
			endDt = endDate.getTime();
		}
		
		if(today.getTime().equals(startDate.getTime())) {
			cal.set(Calendar.HOUR_OF_DAY, 0);
			cal.set(Calendar.MINUTE, 0);
			startDt = cal.getTime();
		} else {
			startDt = startDate.getTime();
		}
		
		//int i = startDatebox.getValue().compareTo(endDatebox.getValue());
		int i = startDt.compareTo(endDt);
		
		EventQueue<Event> eqForSelectedTimeFrame = EventQueues.lookup(
				"customdashboard", EventQueues.SESSION,
				true);
		Map<String,Date> dateMap = new HashMap<String, Date>();
		
		if( i > 0) {
			Messagebox.show("Start date can't be greater than End date", 
				    "Error", Messagebox.OK ,
				    Messagebox.ERROR,
				    new org.zkoss.zk.ui.event.EventListener<Event>(){
	            public void onEvent(Event e){
	                if("onOK".equals(e.getName())){
	                	startDatebox.setValue(startTime);
	                	endDatebox.setValue(endTime);
	                }
	            }
	        }
				    );
			return;
		}
		//startTime = startDatebox.getValue();
		//endTime = endDatebox.getValue();
		startTime = startDt;
		endTime = endDt;
		dateMap.put("startTime", startTime);
		dateMap.put("endTime", endTime);
		eqForSelectedTimeFrame.publish(new Event("onDateTimeChange", null, dateMap));
		liveselectbox.setSelectedIndex(0);
		loadWidgetsData();
	}
	
	private void setFromEndTime() {
		Calendar c = Calendar.getInstance();
		c.add(Calendar.MILLISECOND, OFFSET_IN_MILISECS);
		//c.set(Calendar.HOUR_OF_DAY, 0);
		//c.set(Calendar.MINUTE, 0);
		//c.set(Calendar.SECOND, 0);
		endTime = c.getTime();
		endDatebox.setValue(endTime);
		//c.add(Calendar.DATE, -3);
		//c.set(Calendar.HOUR_OF_DAY, 0);
		//c.set(Calendar.MINUTE, -15);
		//c.set(Calendar.SECOND, 0);
		c.add(Calendar.MINUTE, -15);
		startTime = c.getTime();
		startDatebox.setValue(startTime);
	}
	
	private void clearCustomDashboardWidgets() {
		if(layout.hasFellow("allWidgets"))
			pc.detach();
	}
	
	private void setDashboardDetails() {
		if(customDashboard != null) {
			customDashboardNameLbl.setValue(customDashboard.getCustomDashboardName());
			customDashboardNametxtBox.setValue(customDashboard.getCustomDashboardName());
		} else {
			customDashboardNameLbl.setValue("");
			customDashboardNametxtBox.setValue("");
		}
		
		if(customDashboard != null && customDashboard.getIsDefaultOnUI())
			dbIsDefaultcb.setChecked(true);
		else
			dbIsDefaultcb.setChecked(false);
		
		dbtagChosenbox.clearSelection();
		if(customDashboard != null && customDashboard.getTags() != null){
			ListModelList<String> model= new ListModelList<>(customDashboard.getTags());
			dbtagChosenbox.setModel(model);
			dbtagChosenbox.setSelectedObjects(model);
		}
	}
	
	private void addAllCustomDashboardWidgets() {
		clearCustomDashboardWidgets();
		
		if(widgetIdMap == null || widgetIdMap.size() <= 0){
			log.info("seems getAllWidgetType call was failed ....");
			return;
		}
		
		try {
			dashboardWidgets = CustomDashboardFacade.getCustomDashboardWidgets(productInstanceName,customDashboard.getCustomDashboardId());
		}catch(Exception e){
			e.printStackTrace();
		}
		
		if(dashboardWidgets != null) {
			pc = new Portalchildren();
			pc.setId("allWidgets");
			for(CustomDashboardWidget widget: dashboardWidgets) {
				String url = widgetIdMap.get(widget.getWidgetTypeID()).toLowerCase().replaceAll("\\s+|-","")+"widget.zul";
				url = "~./sqldm/com/idera/sqldm/ui/customDashboard/" + url ;
				HashMap<String, Object> map = new HashMap<>();
				map.put("widgetId", widget.getWidgetID());
				map.put("widgetName", widget.getWidgetName());
				map.put("match", widget.getMatch());
				map.put("metricId", widget.getMetricID());
				map.put("widgetTypeId", widget.getWidgetTypeID());
				map.put("tags", widget.getTagId());
				map.put("serverIds", widget.getSqlServerId());
				map.put("customDashboardId", widget.getRelatedCustomDashboardID());
				map.put("productInstanceName", productInstanceName);
				map.put("startTime", Utility.getUtcDateString(startTime));
				map.put("endTime", Utility.getUtcDateString(endTime));
				Panel p = (Panel)Executions.createComponents(
						url,null,map);
				pc.appendChild(p);
			}
			layout.appendChild(pc);
			loadWidgetsData();
			resize();
		}
	}
	
	
	private void initializeWidgetTypes() {
		try {
			List<Types> widgetTypes = CustomDashboardFacade.getAllWidgetTypes(productInstanceName);
			
			for(Types type: widgetTypes){
				widgetIdMap.put(type.getId(), type.getValue());
			}
			
		} catch (RestException e) {
			e.printStackTrace();
		}
	}
	
	@Listen("onClick = #cancelBtn")
	public void backToCustomDashboard(){
		displayDBDiv.setVisible(true);
		editDBDiv.setVisible(false);
		EventQueue<Event> eqForCustomDashboard = EventQueues.lookup(
				"customdashboard", EventQueues.SESSION,
				true);
		eqForCustomDashboard.publish(new Event("displayCustomDashboard"));
	}
	
	@Listen("onClick = #copyBtn")
	public void copyCustomDashboard() {
		if(customDashboard == null || customDashboard.getCustomDashboardName() == null){
			return;
		}
		try {
			if(CustomDashboardFacade.copyCustomDashboard(productInstanceName, customDashboard.getCustomDashboardId())){
				EventQueue<Event> eqForCustomDashboard = EventQueues.lookup(
						"customdashboard", EventQueues.SESSION,
						true);
				eqForCustomDashboard.publish(new Event("onCopyCustomDashboard"));
			}
		} catch(Exception e) {
			log.error("error while making copy of customdashboard ...");
			e.printStackTrace();
		}
	}
	
	@Listen("onClick = #editBtn")
	public void editCustomDashboard(){
		if(customDashboard == null || customDashboard.getCustomDashboardName() == null){
			return;
		}
		displayDBDiv.setVisible(false);
		editDBDiv.setVisible(true);
		resize();
		EventQueue<Event> eqForCustomDashboard = EventQueues.lookup(
				"customdashboard", EventQueues.SESSION,
				true);
		eqForCustomDashboard.publish(new Event("editCustomDashboard"));
	}
	
	@Listen("onClick = #removeBtn")
	public void removeCustomDashboard(){
		if(customDashboard == null || customDashboard.getCustomDashboardName() == null){
			return;
		}
		Map<String, String> args = new HashMap<String, String>();
		args.put("obj1", customDashboard.getCustomDashboardName());
		 Window window = (Window) Executions.createComponents("~./sqldm/com/idera/sqldm/ui/customDashboard/idera-dialog.zul", null, args);    
		 window.doModal(); 
		/*Messagebox.show("Dashboard "+customDashboard.getCustomDashboardName()+" will be removed permanently. Do you want to continue ?", 
			    "Confirmation", Messagebox.OK | Messagebox.CANCEL,
			    Messagebox.QUESTION,
			        new org.zkoss.zk.ui.event.EventListener<Event>(){
			            public void onEvent(Event e){
			                if("onOK".equals(e.getName())){
			                	deleteCustomDashboard();
			                	resize();
			                }
			            }
			        }
			    );*/
	}
	
	@Listen("onSearch = #dbtagChosenbox")
	public void addCustomDashboardTags(InputEvent evt) {
		ListModelList<Object> model =  (ListModelList<Object>) dbtagChosenbox.getModel();
		model.add(evt.getValue());
		model.addToSelection(evt.getValue());
		dbtagChosenbox.setModel(model);
	}
	
	@Listen("onClick = #saveBtn")
	public void saveCustomDashboard() {
		clearErrorMsg();
		validateInput();
		String dashboardName = customDashboardNametxtBox.getValue();
		List<String> dbTags = new LinkedList<>();
		for(Object obj: dbtagChosenbox.getSelectedObjects()) {
			dbTags.add((String)obj);
		}
		
		Session session = Sessions.getCurrent();
		com.idera.server.web.session.UserSessionSettings settings = (com.idera.server.web.session.UserSessionSettings)session.getAttribute(UserSessionSettings.USER_SESSION_SETTINGS_PROPERTY);
		
		if(settings.getUser() != null ) {
			try {
				CustomDashboardFacade.updateCustomDashboard(productInstanceName, customDashboard.getCustomDashboardId(),
						dashboardName, dbIsDefaultcb.isChecked(), settings.getUser().getSID(), dbTags);
				
				customDashboard.setCustomDashboardName(dashboardName);
				customDashboard.setIsDefaultOnUI(dbIsDefaultcb.isChecked());
				customDashboard.setTags(dbTags);
				setDashboardDetails();
				
				EventQueue<Event> eqForCustomDashboard = EventQueues.lookup(
						"customdashboard", EventQueues.SESSION,
						true);
				eqForCustomDashboard.publish(new Event("onUpdateCustomDashboard",null,customDashboard));
				backToCustomDashboard();
				
			} catch (RestException e) {
				log.error("error while saving customdashboard ...");
				e.printStackTrace();
			}
		}
		
	}
	
	private void validateInput() {
		if(customDashboardNametxtBox.getValue() == null || customDashboardNametxtBox.getValue().trim().equals("")) {
			throw new WrongValueException(customDashboardNametxtBox, "Please enter customdashboard name.");
		}
	}
	
	private void clearErrorMsg() {
		customDashboardNametxtBox.clearErrorMessage();
	}
	
	private void deleteCustomDashboard() {
		try {
			CustomDashboardFacade.deleteCustomDashboard(productInstanceName,customDashboard.getCustomDashboardId() );
			clearCustomDashboardWidgets();
			customDashboard = new CustomDashboard();
			setDashboardDetails();
			EventQueue<Event> eqForCustomDashboard = EventQueues.lookup(
					"customdashboard", EventQueues.SESSION,
					true);
			eqForCustomDashboard.publish(new Event("onRemoveCustomDashboard",null,customDashboard));
		} catch (RestException e) {
			e.printStackTrace();
		}
	}
	
	@Listen("onClick = #addWidgetBtn")
	public void addWidget(){
		if(customDashboard == null || customDashboard.getCustomDashboardName() == null){
			return;
		}
		HashMap<String, Object> map = new HashMap<>();
		map.put("customDashboardId", customDashboard.getCustomDashboardId());
		Window widgetsModal = (Window)Executions.createComponents(
                "~./sqldm/com/idera/sqldm/ui/customDashboard/customWidgets.zul",null,map);
		widgetsModal.doModal();
	}
	
	public void loadWidgetsData() {
		final EventQueue<Event> eq = EventQueues.lookup(SQLdmConstants.WIDGET_PARALLEL_LOAD_DATA_EVENT_QUEUE, EventQueues.SESSION, true);
		
		if(eq != null) {
			ParallelExecutions pe = new ParallelExecutions();
			
			for (final CustomDashboardWidget widget :dashboardWidgets) {
				final String eventName = ""+widget.getWidgetID();
				
				ParallelExecutionsPDU pePDU = new ParallelExecutionsPDU() {
					@Override
					public void taskComplete(PDUBean returnObject) {}
					
					@Override
					public PDUBean task() {
						List<CustomDashboardWidgetData> instancesHistoryList = new LinkedList<>(); 
						try {
							instancesHistoryList = CustomDashboardFacade.getWidgetData(productInstanceName, customDashboard.getCustomDashboardId()
									, widget.getWidgetID(), startTime, endTime,OFFSET_IN_HOURS);
						} catch (Exception e) {
							log.error(e.getMessage(), e);
						}
						eq.publish(new Event(eventName, null, instancesHistoryList));
						return null;
					}
				};
				pe.addToCallable(pePDU);
			}
			try {
				pe.invokeAll();
			} catch (Exception e) {
				e.printStackTrace();
			}
		}
	}
	
	private void setOffSet() {
	OFFSET_IN_MILISECS = (int)(-Utility.cancelOffSetInMillis());
	OFFSET_IN_HOURS = Utility.setOffSet();
	}
}
