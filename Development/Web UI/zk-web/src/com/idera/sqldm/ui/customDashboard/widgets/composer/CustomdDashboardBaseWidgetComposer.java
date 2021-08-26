package com.idera.sqldm.ui.customDashboard.widgets.composer;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.TimeZone;

import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Window;

import com.idera.sqldm.data.customdashboard.CustomDashboardFacade;
import com.idera.sqldm.data.customdashboard.CustomDashboardWidgetData;
import com.idera.sqldm.ui.customDashboard.widgets.CustomDashboardWidget;
import com.idera.sqldm.utils.SQLdmConstants;
import com.idera.sqldm.utils.Utility;

public abstract class CustomdDashboardBaseWidgetComposer 
	extends SelectorComposer<CustomDashboardWidget> {
	
	private static final long serialVersionUID = 11212121212L;
	protected CustomDashboardWidget widgetPanel;
	protected String eventName;
	protected List<CustomDashboardWidgetData> metricValuesforInstance;
	protected Date startTime, endTime;
	protected Map<String, Object> arg = null;
	protected int[] ss = new int[]{2062260, 16744206, 2924588, 14034728, 9725885, 9197131, 14907330, 8355711, 12369186, 1556175},
					hs = new int[]{3244733, 7057110, 10406625, 13032431, 15095053, 16616764, 16625259, 16634018, 3253076, 7652470, 10607003, 13101504, 7695281, 10394312, 12369372, 14342891, 6513507, 9868950, 12434877, 14277081};
	
	@SuppressWarnings("unchecked")
	public void doAfterCompose(CustomDashboardWidget customDashboardWidget)
		    throws Exception {
		  super.doAfterCompose(customDashboardWidget);
		  arg = (HashMap<String,Object>)Executions.getCurrent().getArg();
		  eventName = ((Integer)arg.get("widgetId")).toString();
		  subscribeToEvents();
		  SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
		  sdf.setTimeZone(TimeZone.getTimeZone("UTC"));
		  startTime = sdf.parse((String) arg.get("startTime"));
		  endTime = sdf.parse((String) arg.get("endTime"));
		  Clients.showBusy(this.getSelf().getPanelchildren(), "Loading Data ...");
	}
	
	public void subscribeToEvents() {
		
		EventQueue<Event> eqForSelectedDashboard = EventQueues.lookup(
				"customdashboard", EventQueues.SESSION,
				true);
		eqForSelectedDashboard.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if ("editCustomDashboard".equalsIgnoreCase(event.getName())) {
					onEditCustomDashBoard();
				}
				if ("displayCustomDashboard".equalsIgnoreCase(event.getName())) {
					onDisplayCustomDashBoard();
				}
				if ("onDateTimeChange".equalsIgnoreCase(event.getName())) {
					updateDateTime(event.getData());
				}
			}
		});
		
		EventQueue<Event> eqForDataLoad = EventQueues.lookup(
				SQLdmConstants.WIDGET_PARALLEL_LOAD_DATA_EVENT_QUEUE, EventQueues.SESSION,
				true);
		eqForDataLoad.subscribe(new EventListener<Event>() {
			@SuppressWarnings("unchecked")
			@Override
			public void onEvent(Event event) throws Exception {
				if(eventName.equals(event.getName())){
					metricValuesforInstance = (List<CustomDashboardWidgetData>) event.getData();
					setWidgetData();
					Clients.clearBusy(CustomdDashboardBaseWidgetComposer.this.getSelf().getPanelchildren());
				}
			}
			
		});
	}
	
	@SuppressWarnings("unchecked")
	protected void updateDateTime(Object map){
		Map<String,Date> dateMap = (HashMap<String,Date>)map;
		startTime = dateMap.get("startTime");
		endTime = dateMap.get("endTime");
	}
	
	protected void removeWidget() {
		try{
			if(CustomDashboardFacade.deleteCustomDashboardWidget((String)arg.get("productInstanceName"), 
					(Integer)arg.get("customDashboardId"), (Integer)arg.get("widgetId"))){
				getSelf().detach();
				EventQueue<Event> eqForCustomDashboard = EventQueues.lookup(
						"customdashboard", EventQueues.SESSION,
						true);
				eqForCustomDashboard.publish(new Event("onWidgetRemove"));
			}
		} catch( Exception e) {
			e.printStackTrace();
		}
	}
	
	protected void displayDetails() {
		arg.put("startTime", Utility.getUtcDateString(startTime));
		arg.put("endTime", Utility.getUtcDateString(endTime));
		
		Window widgetsModal = (Window)Executions.createComponents(
                "~./sqldm/com/idera/sqldm/ui/customDashboard/widgetDetails.zul",null,arg);
		widgetsModal.doModal();
	}
	
	protected void editWidget() {
		Window widgetsModal = (Window)Executions.createComponents(
                "~./sqldm/com/idera/sqldm/ui/customDashboard/customWidgets.zul",null,arg);
		widgetsModal.doModal();
	}
	
	protected abstract void setWidgetData();
	protected abstract void onEditCustomDashBoard();
	protected abstract void onDisplayCustomDashBoard();
	protected abstract void setWidgetConfig(CustomDashboardWidget comp);
	protected abstract void setEmptyMessage();
}
