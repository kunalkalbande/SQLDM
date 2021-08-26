package com.idera.sqldm.ui.dashboard;

import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.HashSet;
import java.util.LinkedHashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;
import java.util.Set;
import java.util.SortedMap;
import java.util.SortedSet;
import java.util.TreeMap;
import java.util.TreeSet;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.util.Composer;
import org.zkoss.zul.ListModel;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Popup;
import org.zkoss.zul.Window;

import com.idera.server.web.WebConstants;
import com.idera.cwf.model.Product;
import com.idera.sqldm.data.DashboardAlertsByCategoryWidget;
import com.idera.sqldm.data.DashboardAlertsByDatabaseWidget;
import com.idera.sqldm.data.DashboardAlertsByInstanceWidget;
import com.idera.sqldm.data.DashboardWidgetsFacade;
import com.idera.sqldm.data.DashboardWorstResponseTimeWidget;
import com.idera.sqldm.data.alerts.Alert;
import com.idera.sqldm.data.alerts.AlertException;
import com.idera.sqldm.data.alerts.AlertFacade;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.server.web.WebUtil;
import com.idera.sqldm.server.web.session.SessionUtil;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.utils.SQLdmConstants;
import com.idera.sqldm.utils.Utility;

public class DashboardWidgetsComposer implements Composer<Component>{

	private static final Logger log = Logger.getLogger(DashboardWidgetsComposer.class);
	
	private String displayMessageForAlertsByCategory;
	private String displayMessageForAlertsByDatabase;
	private String displayMessageForWorstResponseTime;
	private String displayMessageForAlertsByInstance;
	private String OFFSET_IN_HOURS = "0.0";
	private Integer iaCount;
	private Integer rtCount;
	private Integer daCount;
	private Integer caCount;
	
	Set<DashboardAlertsByCategoryWidget> alertsByCategorySet;
	SortedSet<DashboardAlertsByDatabaseWidget> alertsByDatabaseSet;
	SortedSet<DashboardAlertsByInstanceWidget> alertsByInstanceSet;
	SortedSet<DashboardWorstResponseTimeWidget> worstResponseTimeSet;

	Set<DashboardAlertsByCategoryWidget> alertsByCategoryMasterSet;
	SortedSet<DashboardAlertsByDatabaseWidget> alertsByDatabaseMasterSet;
	SortedSet<DashboardAlertsByInstanceWidget> alertsByInstanceMasterSet;
	SortedSet<DashboardWorstResponseTimeWidget> worstResponseTimeMasterSet;
	
	@Init
    public void init() {
		setOffSet();
		setDisplayMessageForAlertsByCategory(ELFunctions.getMessage(SQLdmI18NStrings.LOADING_DATA));
		setDisplayMessageForAlertsByDatabase(ELFunctions.getMessage(SQLdmI18NStrings.LOADING_DATA));
		setDisplayMessageForAlertsByInstance(ELFunctions.getMessage(SQLdmI18NStrings.LOADING_DATA));
		setDisplayMessageForWorstResponseTime(ELFunctions.getMessage(SQLdmI18NStrings.LOADING_DATA));
		
		setIaCount(SQLdmConstants.DASHBOARD_RHS_WIDGET_ALERT_BY_INSTANCE_DEFAULT);
		setRtCount(SQLdmConstants.DASHBOARD_RHS_WIDGET_RESPONSE_TIME_FROM_DB_DEFAULT);
		setDaCount(SQLdmConstants.DASHBOARD_RHS_WIDGET_ALERT_BY_DATABASE_DEFAULT);
		setCaCount(SQLdmConstants.DASHBOARD_RHS_WIDGET_ALERT_BY_CATEGORY_DEFAULT);
		
		Integer count = (Integer) Executions.getCurrent().getSession().getAttribute(SQLdmConstants.DASHBOARD_RHS_WIDGET_ALERT_BY_INSTANCE_STR);
		if (count != null) {
			setIaCount(count);
		}
		
		count = (Integer) Executions.getCurrent().getSession().getAttribute(SQLdmConstants.DASHBOARD_RHS_WIDGET_RESPONSE_TIME_FROM_DB_STR);
		if (count != null) {
			setRtCount(count);
		}
		
		count = (Integer) Executions.getCurrent().getSession().getAttribute(SQLdmConstants.DASHBOARD_RHS_WIDGET_ALERT_BY_CATEGORY_STR);
		if (count != null) {
			setCaCount(count);
		}
		
		count = (Integer) Executions.getCurrent().getSession().getAttribute(SQLdmConstants.DASHBOARD_RHS_WIDGET_ALERT_BY_DATABASE_STR);
		if (count != null) {
			setDaCount(count);
		}
		EventQueue<Event> eqForDashboardRefresh = EventQueues.lookup(DashboardConstants.DASHBOARD_QUEUE_NAME, EventQueues.SESSION, true);
		eqForDashboardRefresh.subscribe(new EventListener<Event>() {
			@SuppressWarnings("unchecked")
			public void onEvent(Event event) throws Exception {
				SessionUtil.getSecurityContext();
				if(DashboardConstants.DASHBOARD_REFRESH_ALERT_BY_CATEGORY_EVENT.equalsIgnoreCase(event.getName())) {
					if(event.getData() != null) {
						alertsByCategoryMasterSet = (Set<DashboardAlertsByCategoryWidget>)event.getData();
						setAlertsByCategorySet(alertsByCategoryMasterSet);
					} else {
						setAlertsByCategorySet( new TreeSet<DashboardAlertsByCategoryWidget>());
					}
				}
				if(DashboardConstants.DASHBOARD_REFRESH_ALERT_BY_DATABASE_EVENT.equalsIgnoreCase(event.getName())) {
					if(event.getData() != null) {
						alertsByDatabaseMasterSet = (SortedSet<DashboardAlertsByDatabaseWidget>)event.getData();
						setAlertsByDatabaseSet(alertsByDatabaseMasterSet);
					} else {
						setAlertsByDatabaseSet(new TreeSet<DashboardAlertsByDatabaseWidget>());
					}
				}
				if(DashboardConstants.DASHBOARD_REFRESH_ALERT_BY_INSTANCE_EVENT.equalsIgnoreCase(event.getName())) {
					if(event.getData() != null) {
						alertsByInstanceMasterSet = (SortedSet<DashboardAlertsByInstanceWidget>)event.getData();
						setAlertsByInstanceSet(alertsByInstanceMasterSet);
					} else {
						setAlertsByInstanceSet(new TreeSet<DashboardAlertsByInstanceWidget>());
					}
				}
				if(DashboardConstants.DASHBOARD_REFRESH_WORST_RESPONSE_TIME_EVENT.equalsIgnoreCase(event.getName())) {
					if(event.getData() != null) {
						worstResponseTimeMasterSet = (SortedSet<DashboardWorstResponseTimeWidget>)event.getData();
						setWorstResponseTimeSet(worstResponseTimeMasterSet); 
					} else {
						setWorstResponseTimeSet(new TreeSet<DashboardWorstResponseTimeWidget>());
					}
				}
			}
        });
		EventQueue<Event> eq = EventQueues.lookup(SQLdmConstants.DASHBOARD_UPDATE_INSTANCES_EVENT_QUEUE, EventQueues.DESKTOP, true);
        eq.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				SessionUtil.getSecurityContext();
				setAlertsByCategorySet(alertsByCategoryMasterSet);
				setAlertsByDatabaseSet(alertsByDatabaseMasterSet);
				setAlertsByInstanceSet(alertsByInstanceMasterSet);
				setWorstResponseTimeSet(worstResponseTimeMasterSet);
            }
        });
	}
    
	public String getDisplayMessageForAlertsByInstance() {
		return displayMessageForAlertsByInstance;
	}

	public void setDisplayMessageForAlertsByInstance(String displayMessageForAlertsByInstance) {
		this.displayMessageForAlertsByInstance = displayMessageForAlertsByInstance;
		BindUtils.postNotifyChange(null, null, this, "displayMessageForAlertsByInstance");
	}
	
	public String getDisplayMessageForAlertsByCategory() {
		return displayMessageForAlertsByCategory;
	}

	public void setDisplayMessageForAlertsByCategory(String displayMessageForAlertsByCategory) {
		this.displayMessageForAlertsByCategory = displayMessageForAlertsByCategory;
		BindUtils.postNotifyChange(null, null, this, "displayMessageForAlertsByCategory");
	}

	public String getDisplayMessageForAlertsByDatabase() {
		return displayMessageForAlertsByDatabase;
	}

	public void setDisplayMessageForAlertsByDatabase(String displayMessageForAlertsByDatabase) {
		this.displayMessageForAlertsByDatabase = displayMessageForAlertsByDatabase;
		BindUtils.postNotifyChange(null, null, this, "displayMessageForAlertsByDatabase");
	}

	public String getDisplayMessageForWorstResponseTime() {
		return displayMessageForWorstResponseTime;
	}

	public void setDisplayMessageForWorstResponseTime(String displayMessageForWorstResponseTime) {
		this.displayMessageForWorstResponseTime = displayMessageForWorstResponseTime;
		BindUtils.postNotifyChange(null, null, this, "displayMessageForWorstResponseTime");
	}

	@SuppressWarnings("unchecked")
	public void updateWidgetModel() {
		BindUtils.postNotifyChange(null, null, this, "alertsByCategory");
		BindUtils.postNotifyChange(null, null, this, "alertsByDatabase");
		BindUtils.postNotifyChange(null, null, this, "alertsByInstance");
		BindUtils.postNotifyChange(null, null, this, "worstResponseTime");
		SortedSet<DashboardAlertsByCategoryWidget> dabcwList = (SortedSet<DashboardAlertsByCategoryWidget>) Executions.getCurrent().getSession().getAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_WIDGET_ALERT_BY_CATEGORY_SET);
		SortedSet<DashboardAlertsByDatabaseWidget> dabdwList = (SortedSet<DashboardAlertsByDatabaseWidget>) Executions.getCurrent().getSession().getAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_WIDGET_ALERT_BY_DATABASE_SET);
		SortedSet<DashboardAlertsByInstanceWidget> dabiwList = (SortedSet<DashboardAlertsByInstanceWidget>) Executions.getCurrent().getSession().getAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_WIDGET_ALERT_BY_INSTANCE_SET);
		SortedSet<DashboardWorstResponseTimeWidget> dwrtList = (SortedSet<DashboardWorstResponseTimeWidget>) Executions.getCurrent().getSession().getAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_WIDGET_WORST_RESPONSE_TIME_FROM_DB_SET);
		if(dabcwList == null || dabcwList.size() == 0) {
			setDisplayMessageForAlertsByCategory(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_DASHBOARD_NO_ALERTS_FOR_ALERTSBYCATEGORY));
		}
		if(dabdwList == null || dabdwList.size() == 0) {
			setDisplayMessageForAlertsByDatabase(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_DASHBOARD_NO_ALERTS_FOR_ALERTSBYDATABASE));
		}
		if(dabiwList == null || dabiwList.size() == 0) {
			setDisplayMessageForAlertsByInstance(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_DASHBOARD_NO_ALERTS_FOR_ALERTSBYINSTANCE));
		}
		if(dwrtList == null || dwrtList.size() == 0) {
			setDisplayMessageForWorstResponseTime(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_DASHBOARD_NO_ALERTS_FOR_WORSTRESPONSETIME));
		}

	}

	public static Map<String,Integer> sortByValues(Map<String,Integer> map){
        List<Map.Entry<String, Integer>> entries = new LinkedList<Map.Entry<String,Integer>>(map.entrySet());
      
        Collections.sort(entries, new Comparator<Map.Entry<String,Integer>>() {

            @Override
            public int compare(Entry<String,Integer> o1, Entry<String,Integer> o2) {
                return o2.getValue().compareTo(o1.getValue());
            }
        });
      
        Map<String,Integer> sortedMap = new LinkedHashMap<String,Integer>();
        for(Map.Entry<String,Integer> entry: entries){
            sortedMap.put(entry.getKey(), entry.getValue());
        }
      
        return sortedMap;
    }
	
	public ListModel<DashboardAlertsByCategoryWidget> getAlertsByCategory() {

		SortedSet<DashboardAlertsByCategoryWidget> sortedSet = new TreeSet<DashboardAlertsByCategoryWidget>(DashboardWidgetsFacade.ALERTS_BY_CATEGORY_COMPARATOR);
		if(alertsByCategorySet != null) {
			SortedMap<String, Integer> alertsByCategoryNS = new TreeMap<String, Integer>();
			for (DashboardAlertsByCategoryWidget alertsByCategory : alertsByCategorySet) {
				Integer alertCount = alertsByCategoryNS.get(alertsByCategory.getCategory());
				Integer numAlertCount = 0;
				if (alertCount != null && alertsByCategory.getNumOfAlerts() != null) {
					numAlertCount = alertsByCategory.getNumOfAlerts() + alertCount;
				} else if (alertCount != null) {
					numAlertCount = alertCount;
				} else  if (alertsByCategory.getNumOfAlerts() != null) {
					numAlertCount = alertsByCategory.getNumOfAlerts();
				}
				
				alertsByCategoryNS.put(alertsByCategory.getCategory(), numAlertCount);
			}
			
			Map<String, Integer> sortedOnValueMap = sortByValues(alertsByCategoryNS);
			int count = 0;
			for (Entry<String, Integer> set: sortedOnValueMap.entrySet()) {
				count++;
				if(count <= caCount){
					sortedSet.add(new DashboardAlertsByCategoryWidget(set.getKey(), set.getValue()));
				}
			}
			 
		}
		return new ListModelList<DashboardAlertsByCategoryWidget>(sortedSet);
	}
	public ListModel<DashboardAlertsByDatabaseWidget> getAlertsByDatabase() {

		if(alertsByDatabaseSet == null) {
			alertsByDatabaseSet = new TreeSet<>();
		}
		return new ListModelList<DashboardAlertsByDatabaseWidget>(alertsByDatabaseSet);			
		
	}
	public ListModel<DashboardAlertsByInstanceWidget> getAlertsByInstance() {

		if(alertsByInstanceSet == null) {
			alertsByInstanceSet = new TreeSet<>();
		}
		return new ListModelList<DashboardAlertsByInstanceWidget>(alertsByInstanceSet);			
		
	}
	public ListModel<DashboardWorstResponseTimeWidget> getWorstResponseTime() {

		if(worstResponseTimeSet == null) {
			worstResponseTimeSet = new TreeSet<>();
		}
		return new ListModelList<DashboardWorstResponseTimeWidget>(worstResponseTimeSet);			
	}

	@Command("redirectToInstance")
	public void redirectToInstance(@BindingParam("instanceId") Integer instanceId, @BindingParam("tabId") Integer tabId, @BindingParam("subTabId") Integer subTabId , @BindingParam("product") Product product) {
		PreferencesUtil.getInstance().setInstanceTabSubTabs(instanceId, InstanceSubCategoriesTab.findById(tabId, subTabId));
		Executions.sendRedirect(WebUtil.buildPathRelativeToProduct(product , "singleInstance"+"/"+instanceId));
	}
	
    @Command
    public void categoryClicked(@BindingParam("category") String category){
		SessionUtil.getSecurityContext();
    	//Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");

		String productInstanceName="";
		DashboardPreferencesBean dbpb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();
		if(dbpb != null){
		productInstanceName = dbpb.getProductInstanceName();
		}
		
    	List<Alert> alertList = null;
		try {
			alertList = AlertFacade.getAllAlerts(productInstanceName, true, category , OFFSET_IN_HOURS );
		} catch (AlertException e) {
			log.error(e.getMessage(), e);
		}
    	if(alertList != null && !alertList.isEmpty()){
    		Alert alert = alertList.get(0);
    		Map<String, Object> args = new HashMap<String, Object>();
    		args.put("alert", alert);
    		args.put("alertsList", alertList);
    		Window window = (Window)Executions.createComponents(
                    "~./sqldm/com/idera/sqldm/ui/alerts/alertsView.zul", null, args);
            window.doModal();
    	}
    }
    
    public void setAlertsByCategorySet(Set<DashboardAlertsByCategoryWidget> alertsByCategory) {
		this.alertsByCategorySet = new HashSet<DashboardAlertsByCategoryWidget>();
		if(alertsByCategory != null) {
			Map<Integer, Object> filteredInstancesMapFromSession = PreferencesUtil.getInstance().getFilteredInstancesMap();
			if (filteredInstancesMapFromSession != null) {
				for (DashboardAlertsByCategoryWidget abd : alertsByCategory) {
				 	if (filteredInstancesMapFromSession.get(abd.getInstanceID()) != null) {
				 		this.alertsByCategorySet.add(abd);
					}
				}
			} else {
				this.alertsByCategorySet = alertsByCategory; 
			}
		}
		BindUtils.postNotifyChange(null, null, this, "alertsByCategory");
		if(this.alertsByCategorySet.size() == 0) {
			setDisplayMessageForAlertsByCategory(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_DASHBOARD_NO_ALERTS_FOR_ALERTSBYCATEGORY));
		}		
	}
	
	public void setAlertsByInstanceSet(
			SortedSet<DashboardAlertsByInstanceWidget> alertsByInstance) {
		this.alertsByInstanceSet = new TreeSet<DashboardAlertsByInstanceWidget>(DashboardWidgetsFacade.ALERTS_BY_INSTANCE_COMPARATOR);
		if(alertsByInstance != null) {
			Map<Integer, Object> filteredInstancesMapFromSession = PreferencesUtil.getInstance().getFilteredInstancesMap();
			int counter = 0;
			if (filteredInstancesMapFromSession != null) {
				for (DashboardAlertsByInstanceWidget abi : alertsByInstance) {
				 	if (filteredInstancesMapFromSession.get(abi.getInstanceId()) != null) {
				 		if(counter >= iaCount){
					 		break;
					 	}
					 	counter++;
						alertsByInstanceSet.add(abi);
					}
				}
			} else {
				for (DashboardAlertsByInstanceWidget abi : alertsByInstance) {
					if(counter >= iaCount){
				 		break;
				 	}
				 	counter++;
				 	alertsByInstanceSet.add(abi);
				}
			}
		}
		BindUtils.postNotifyChange(null, null, this, "alertsByInstance");
		if(alertsByInstanceSet.size() == 0) {
			setDisplayMessageForAlertsByInstance(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_DASHBOARD_NO_ALERTS_FOR_ALERTSBYINSTANCE));
		}
	}
	
	public void setAlertsByDatabaseSet(SortedSet<DashboardAlertsByDatabaseWidget> alertsByDatabase) {
		this.alertsByDatabaseSet = new TreeSet<DashboardAlertsByDatabaseWidget>(DashboardWidgetsFacade.ALERTS_BY_DATABASE_COMPARATOR);
		if(alertsByDatabase != null) {
			Map<Integer, Object> filteredInstancesMapFromSession = PreferencesUtil.getInstance().getFilteredInstancesMap();
			int counter = 0;
			if (filteredInstancesMapFromSession != null) {
				for (DashboardAlertsByDatabaseWidget abd : alertsByDatabase) {
				 	if (filteredInstancesMapFromSession.get(abd.getInstanceId()) != null) {
				 		if(counter >= daCount){
					 		break;
					 	}
					 	counter++;
				 		alertsByDatabaseSet.add(abd);
					}
				}
			} else {
				for (DashboardAlertsByDatabaseWidget abd : alertsByDatabase) {
				 		if(counter >= daCount){
					 		break;
					 	}
					 	counter++;
				 		alertsByDatabaseSet.add(abd);
				}
			}
		}
		BindUtils.postNotifyChange(null, null, this, "alertsByDatabase");
		if(alertsByDatabaseSet.size() == 0) {
			setDisplayMessageForAlertsByDatabase(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_DASHBOARD_NO_ALERTS_FOR_ALERTSBYDATABASE));
		}
	}

	public void setWorstResponseTimeSet(SortedSet<DashboardWorstResponseTimeWidget> worstResponseTime) {
		this.worstResponseTimeSet = new TreeSet<DashboardWorstResponseTimeWidget>(DashboardWidgetsFacade.LATEST_RESPONSE_COMPARATOR);
		if(worstResponseTime != null) {
			int counter = 0;
			Map<Integer, Object> instancesHashListFromSession = PreferencesUtil.getInstance().getFilteredInstancesMap();
			if (instancesHashListFromSession != null) {
				for (DashboardWorstResponseTimeWidget abd : worstResponseTime) {
					if (instancesHashListFromSession.get(abd.getInstanceId()) != null) {
						if(counter >= rtCount){
					 		break;
					 	}
					 	counter++;
					 	this.worstResponseTimeSet.add(abd);
					}
				}
			} else {
				for (DashboardWorstResponseTimeWidget abd : worstResponseTime) {
					if(counter >= rtCount){
				 		break;
				 	}
				 	counter++;
				 	this.worstResponseTimeSet.add(abd);
				}
			}
		}
		BindUtils.postNotifyChange(null, null, this, "worstResponseTime");
		if(this.worstResponseTimeSet.size() == 0) {
			setDisplayMessageForWorstResponseTime(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_DASHBOARD_NO_ALERTS_FOR_WORSTRESPONSETIME));
		}
	}

	@Command
    public void applyIAConfig(@BindingParam("popup")  Popup popup) {
		Executions.getCurrent().getSession().setAttribute(SQLdmConstants.DASHBOARD_RHS_WIDGET_ALERT_BY_INSTANCE_STR, iaCount);
		setAlertsByInstanceSet(alertsByInstanceMasterSet);
		popup.close();
	}
	
	@Command
    public void applyRTConfig(@BindingParam("popup")  Popup popup) {
		Executions.getCurrent().getSession().setAttribute(SQLdmConstants.DASHBOARD_RHS_WIDGET_RESPONSE_TIME_FROM_DB_STR, rtCount);
		setWorstResponseTimeSet(worstResponseTimeMasterSet);
		popup.close();
	}
	
	@Command
    public void applyDAConfig(@BindingParam("popup")  Popup popup) {
		Executions.getCurrent().getSession().setAttribute(SQLdmConstants.DASHBOARD_RHS_WIDGET_ALERT_BY_DATABASE_STR, daCount);
		setAlertsByDatabaseSet(alertsByDatabaseMasterSet);
		popup.close();
	}
	
	@Command
    public void applyCAConfig(@BindingParam("popup")  Popup popup) {
		Executions.getCurrent().getSession().setAttribute(SQLdmConstants.DASHBOARD_RHS_WIDGET_ALERT_BY_CATEGORY_STR, caCount);
		setAlertsByCategorySet(alertsByCategoryMasterSet);
		popup.close();
	}
	
	public Integer getIaCount() {
		return iaCount;
	}

	public void setIaCount(Integer iaCount) {
		this.iaCount = iaCount;
	}

	public Integer getRtCount() {
		return rtCount;
	}

	public void setRtCount(Integer rtCount) {
		this.rtCount = rtCount;
	}

	public Integer getDaCount() {
		return daCount;
	}

	public void setDaCount(Integer daCount) {
		this.daCount = daCount;
	}

	public Integer getCaCount() {
		return caCount;
	}

	public void setCaCount(Integer caCount) {
		this.caCount = caCount;
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
	
	@Override
	public void doAfterCompose(Component arg0) throws Exception {
		// TODO Auto-generated method stub
		
	}

}
