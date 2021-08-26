package com.idera.sqldm.ui.dashboard;

import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.HashSet;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.SortedMap;
import java.util.SortedSet;
import java.util.TreeMap;
import java.util.TreeSet;

import org.apache.http.auth.Credentials;
import org.apache.log4j.Logger;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Desktop;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.util.Composer;

import com.idera.server.web.WebConstants;
import com.idera.common.rest.ICSTokenCredentials;
import com.idera.sqldm.data.DashboardAlertsByCategoryWidget;
import com.idera.sqldm.data.DashboardAlertsByDatabaseWidget;
import com.idera.sqldm.data.DashboardAlertsByInstanceWidget;
import com.idera.sqldm.data.DashboardInstance;
import com.idera.sqldm.data.DashboardInstanceFacade;
import com.idera.sqldm.data.DashboardWidgetsFacade;
import com.idera.sqldm.data.DashboardWorstResponseTimeWidget;
import com.idera.sqldm.data.InstanceException;
import com.idera.sqldm.data.SeverityCodeToStringEnum;
import com.idera.sqldm.data.alerts.Alert;
import com.idera.sqldm.data.alerts.AlertFacade;
import com.idera.sqldm.io.executor.ParallelExecutions;
import com.idera.sqldm.io.executor.ParallelExecutionsPDU;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.utils.SQLdmConstants;

public class DashboardComposer implements Composer<Component> {

	static final long serialVersionUID = 1L;
	
	private final Logger log = Logger.getLogger(DashboardComposer.class);
	//protected List<DashboardInstance> m_dashboardInstances;
	protected Map<String, DashboardBean> tags;
	protected Map<String, DashboardBean> severities;
    private EventQueue<Event> eq;
    private String OFFSET_IN_HOURS = "0.0";
    private boolean setFromNav;
//	protected String[] severityList = {"All", SeverityCodeToStringEnum.CRITICAL.getUiLabel(), SeverityCodeToStringEnum.WARNING.getUiLabel(), SeverityCodeToStringEnum.OK.getUiLabel(), SeverityCodeToStringEnum.INFORMATIONAL.getUiLabel()};
    
	@Override
	public void doAfterCompose(Component view) throws Exception {
		//init();
        //Selectors.wireComponents(view, this, false);
	
		ICSTokenCredentials curr = null;
		try {
			curr = (ICSTokenCredentials)com.idera.server.web.session.SessionUtil.getCurrentUserCredentials();
			if (curr  != null) {
				com.idera.sqldm.server.web.session.SessionUtil.setSessionVariable("SqldmUserSetting", curr);
				log.debug("Setting the Credentials " + curr.getUserPrincipal());
			}
		}
		catch(Exception e) {
			log.error("Exception in Dashboard composer" , e);
		}
		
		//updateDashboard(true);
    }

	@Init
	public void init() {
		setOffSet();
		log.debug("Initializing Dashboard Composer");
		eq = EventQueues.lookup(DashboardConstants.DASHBOARD_QUEUE_NAME, EventQueues.SESSION, true);
		eq.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				log.debug(event.getName() + " Subscribing to "+DashboardConstants.DASHBOARD_QUEUE_NAME );
				if(DashboardConstants.DASHBOARD_MANUAL_REFRESH_EVENT_NAME.equalsIgnoreCase(event.getName())) {
					updateDashboard(true);
				}				
            }
        });
		log.debug("Calling updateDashboard from init");
		updateDashboard (false);
		
		//Subscribing to Change Product Filter
		EventQueue<Event> productQueue = EventQueues.lookup("changeProduct",
				EventQueues.DESKTOP, true);
		productQueue.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if(event.getName().equals("productChanged")){
					updateDashboard(false);
				}
			}
		});
		//initialising All instances list in Session
		List<DashboardInstance> allDashboardInstances = new ArrayList<DashboardInstance>();
		try {
			allDashboardInstances = DashboardInstanceFacade.getDashboardInstances("All");
		} catch (InstanceException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}		
		setAllInstances(allDashboardInstances, Executions.getCurrent().getDesktop());
	}

	protected Map<String, DashboardBean> getDefaultLeftNavigationSeveritiesValues() {
		Map<String, DashboardBean> tSeverities = new HashMap<String, DashboardBean>();
		for (SeverityCodeToStringEnum sctse : SeverityCodeToStringEnum.getLeftNavigationStatusList()) {
			tSeverities.put(sctse.getUiLabel(), new DashboardBean(sctse.getId(), null, sctse.getUiLabel(), sctse));
		}
		return tSeverities;
	}
	
	private void setMInstances(List<DashboardInstance> dashboardInstances, Desktop desktop) {
		desktop.setAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_INSTANCES_LIST, dashboardInstances);
	}
	
	private void setAllInstances(List<DashboardInstance> allDashboardInstances , Desktop desktop){
		desktop.setAttribute(SQLdmConstants.DASHBOARD_SCOPE_ALL_DASHBOARD_INTANCES_LIST, allDashboardInstances);
	}
	
	protected boolean reloadDashboardInstances(List<DashboardInstance> dashboardInstances, Desktop desktop) {
		generateLeftNavigationModel(dashboardInstances, desktop);
		// setMInstances();
		return true;
	}

	protected void generateLeftNavigationModel(List<DashboardInstance> instances, Desktop desktop) {
		Map<String, DashboardBean> tSeverities = getDefaultLeftNavigationSeveritiesValues();
		SortedMap<String, DashboardBean> tTags = new TreeMap<>();
		List<String> instanceTags ;
		int maxSeverity = 0;
		if (instances != null) {
			for (DashboardInstance instance : instances) {
				if (instance.getServerStatus() != null || instance.getOverview() != null) {
					if(instance.getServerStatus().getTags()!=null){
						instanceTags = instance.getServerStatus().getTags();
					}
					else{
						instanceTags = instance.getOverview().getTags();
					}
					
				if (instance.getServerStatus() != null && instance.getServerStatus().getMaxSeverity() != null) {
					Integer severity = instance.getServerStatus().getMaxSeverity();
					if (instanceTags != null && instanceTags.size() > 0) {
						for (String tag : instanceTags) {
							DashboardBean db = tTags.get(tag);
							if (db == null) {
								db = new DashboardBean(SeverityCodeToStringEnum.NULL.getId(), null, tag, null);
								tTags.put(tag, db);
							}
							if (severity > db.getMaxSeverity()) {
								db.setMaxSeverity(severity);
							}
							db.addInstance(instance);
						}
					}
					// String label = ELFunctions.getSeverityString(instance.getServerStatus().getMaxSeverity());
					SeverityCodeToStringEnum sctse = SeverityCodeToStringEnum.getSeverityEnumForId(instance.getServerStatus().getMaxSeverity());
					DashboardBean db = tSeverities.get(sctse.getUiLabel());
					db.addInstance(instance);
					if (severity > maxSeverity) {
						maxSeverity = severity;
					}
				}
				}else {
					// TODO log error
					log.error((new Date()).toString() + "No Server Status received.");
				}
			}
		}
		DashboardBean dbForAll = tSeverities.get(SeverityCodeToStringEnum.ALL.getUiLabel());
		if (dbForAll != null) {
			dbForAll.setInstances(instances);
			dbForAll.setMaxSeverity(maxSeverity);
		}
		desktop.setAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_TAGS, tTags);
		desktop.setAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_SEVERITIES, tSeverities);
		//notifyChildrenForDataRefresh();
	}
	
	@SuppressWarnings("unchecked")
	@Command("updateDashboard")
	public void updateDashboard (@BindingParam("forceLoad") final Boolean forceLoad){
		log.debug("In Dashboard Composer forceLoad: " + forceLoad);
		final Desktop desktop = Executions.getCurrent().getDesktop(); 
		ParallelExecutions pe = new ParallelExecutions();
		final EventQueue<Event> eq = EventQueues.lookup(DashboardConstants.DASHBOARD_QUEUE_NAME, EventQueues.SESSION, true);
		DashboardPreferencesBean dbpb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();
		String productInstanceNameFromNav = null;
		if(dbpb != null){
		productInstanceNameFromNav = dbpb.getProductInstanceName();
		}
		//Setting the product instance name in navigation bar  
/*		String _productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
    	if( _productInstanceName != null)
    		PreferencesUtil.getInstance().setDashboardPreferencesInSession(_productInstanceName);
    	else{
    		dbpb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();
    		if(dbpb != null){
    		_productInstanceName = dbpb.getProductInstanceName();
    		}
    	}*/
    //	log.debug("In updateDashboard method , product instance name : " + _productInstanceName);
    	final String productInstanceName = productInstanceNameFromNav ;
    	/*if(setFromNav){
    	productInstanceName = productInst= anceNameFromNav;
    	}
    	else
    	productInstanceName = _productInstanceName;*/
    	//final String productInstanceName = productInstanceNameFromNav;
    	if (productInstanceName == null) {
    		return;
    	}
		//System.out.println("Loading Data........");
		ParallelExecutionsPDU pePDU1 = new ParallelExecutionsPDU() {
			@Override
			public PDUBean task() {
				try {
					log.debug("In Alerts By Category Widget task");
					Set<DashboardAlertsByCategoryWidget> alertsByCategorySet = new HashSet<>();
					log.debug("In Alerts By Category Widget task , after making getNumAlertsByCategory API call");
					try {
						alertsByCategorySet.addAll(DashboardWidgetsFacade.getNumAlertsByCategory(productInstanceName));
					} catch(Exception e) {
						log.error("Exception in getting alertByCategory from server");
						log.error(e.getMessage(), e);
					}
					if(alertsByCategorySet != null) {
				        if(eq != null) {
				            eq.publish(new Event(DashboardConstants.DASHBOARD_REFRESH_ALERT_BY_CATEGORY_EVENT, null, alertsByCategorySet));
				            log.debug("Published event "+DashboardConstants.DASHBOARD_REFRESH_ALERT_BY_CATEGORY_EVENT);
				        }														
					}
				} catch (Exception e) {
					log.error("Exception in settings alertByCategory taskComplete");
					log.error(e.getMessage(), e); //throw new RuntimeException(e);
				}
				return getPduBean();
			}
			@Override
			public void taskComplete(PDUBean pduBean) {}
		};
		pe.addToCallable(pePDU1);

		ParallelExecutionsPDU pePDU2 = new ParallelExecutionsPDU() {
			@Override
			public PDUBean task() {
				try {
					log.debug("In Alerts By DB Widget task");
					SortedSet<DashboardAlertsByDatabaseWidget> alertsByDatabaseSet = new TreeSet<>(DashboardWidgetsFacade.ALERTS_BY_DATABASE_COMPARATOR);
					try {
						alertsByDatabaseSet.addAll(DashboardWidgetsFacade.getNumAlertsByDatabase(productInstanceName));
						log.debug("In Alerts By DB Widget task , after making getNumAlertsByDatabase API call");
					} catch (Exception e) {
						log.error("Exception in getting alertsByDatabase from server");
						log.error(e.getMessage(), e);
					}
					if(alertsByDatabaseSet != null) {
						if(eq != null) {
				            eq.publish(new Event(DashboardConstants.DASHBOARD_REFRESH_ALERT_BY_DATABASE_EVENT, null, alertsByDatabaseSet));
				        }					
					}
					
				} catch (Exception e) {
					log.error("Exception in settings alertsByDatabase taskComplete");
					log.error(e.getMessage(), e); //throw new RuntimeException(e);
				}
				return getPduBean();
			}
			@Override
			public void taskComplete(PDUBean pduBean) {}
		};
		pe.addToCallable(pePDU2);
		
		ParallelExecutionsPDU pePDU3 = new ParallelExecutionsPDU() {
			@Override
			public PDUBean task() {
				try {
					log.debug("In Alerts By Response Time Widget task");
					SortedSet<DashboardWorstResponseTimeWidget> responseTimeSet = new TreeSet<>(DashboardWidgetsFacade.LATEST_RESPONSE_COMPARATOR);
					try {
						responseTimeSet.addAll(DashboardWidgetsFacade.getLatestResponseTime(productInstanceName , OFFSET_IN_HOURS));
						log.debug("In Alerts By Response Time task , after making getLatestResponseTime API call");
					} catch (Exception e) {
						log.error("Exception in getting worstResponseTime from server");
						log.error(e.getMessage(), e);
					}
					
					if(responseTimeSet != null) {
						if(eq != null) {
				            eq.publish(new Event(DashboardConstants.DASHBOARD_REFRESH_WORST_RESPONSE_TIME_EVENT, null, responseTimeSet));
				        }					
					}
				} catch (Exception e) {
					log.error("Exception in settings worstResponseTime taskComplete");
					log.error(e.getMessage(), e); //throw new RuntimeException(e);
				}
				return null;
			}
			@Override
			public void taskComplete(PDUBean pduBean) {}
		};
		pe.addToCallable(pePDU3);
		
		ParallelExecutionsPDU pePDU4 = new ParallelExecutionsPDU() {
			@Override
			public PDUBean task() {
				try {
					log.debug("In Instances List task");
					List<DashboardInstance> responses = null;
					log.debug("In Instances List task , after making getDashboardInstances API call");
					try{
						responses = DashboardInstanceFacade.getDashboardInstances(productInstanceName);
					} catch (Exception e) {
						log.error("Exception in retrieving the Instances from server");
						log.error(e.getMessage(), e);
					}
					log.debug("Getting dashboard instances "+responses.size());
					List<DashboardInstance> dashboardInstances = new LinkedList<DashboardInstance>();
					if(responses != null) {
						dashboardInstances = responses;
					}
					setMInstances(dashboardInstances, desktop);
					reloadDashboardInstances((List<DashboardInstance>) desktop.getAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_INSTANCES_LIST), desktop);
			        if(eq != null) {
			            eq.publish(new Event(DashboardConstants.DASHBOARD_REFRESH_EVENT_NAME));
			        }
				} catch (Exception e) {
					log.error("Exception in settings dashboardInstance taskComplete");
					log.error(e.getMessage(), e); //throw new RuntimeException(e);
				}
				return null;
			}
			@Override
			public void taskComplete(PDUBean pduBean) {}
		};
		pe.addToCallable(pePDU4);
		
		ParallelExecutionsPDU pePDU5 = new ParallelExecutionsPDU() {
			@Override
			public PDUBean task() {
				try {
					List<Alert> responses = null;
					try {
						log.debug("In Alerts List task");
						responses = AlertFacade.getAbridgeActiveAlerts(productInstanceName , OFFSET_IN_HOURS);
						log.debug("In Alerts List task , after making getAbridgeActiveAlerts API call");
					} catch (Exception e) {
						log.error("Exception getting alerts from server");
						log.error(e.getMessage(), e);						
					}
					List<Alert> dashboardAlerts = new LinkedList<Alert>();
					if(responses != null) {
						dashboardAlerts.addAll(responses);
					}
			        if(eq != null) {
			            eq.publish(new Event(DashboardConstants.DASHBOARD_REFRESH_ALERT_EVENT_NAME, null, dashboardAlerts));
			        }
				} catch (Exception ex) {
					log.error("Exception in settings alerts taskComplete");
					log.error(ex.getMessage(), ex);
				}
				return null;
			}
			@Override
			public void taskComplete(PDUBean pduBean) {}
		};
		pe.addToCallable(pePDU5);
		
		ParallelExecutionsPDU pePDU6 = new ParallelExecutionsPDU() {
			@Override
			public PDUBean task() {
				try {
					log.debug("In Alerts By Instances Widget task");
					SortedSet<DashboardAlertsByInstanceWidget> alertsByInstanceSet = new TreeSet<>(DashboardWidgetsFacade.ALERTS_BY_INSTANCE_COMPARATOR);
					try {
						alertsByInstanceSet.addAll(DashboardWidgetsFacade.getNumAlertsByInstance(productInstanceName));
						log.debug("In Alerts By DB Widget task , after making getNumAlertsByInstance API call");
					} catch (Exception e) {
						log.error("Exception in getting alertsByInstance from server");
						log.error(e.getMessage(), e);
					}
					if(alertsByInstanceSet != null) {
						if(eq != null) {
				            eq.publish(new Event(DashboardConstants.DASHBOARD_REFRESH_ALERT_BY_INSTANCE_EVENT, null, alertsByInstanceSet));
				        }					
					}
					
				} catch (Exception e) {
					log.error("Exception in settings alertsByInstance taskComplete");
					log.error(e.getMessage(), e); //throw new RuntimeException(e);
				}
				return getPduBean();
			}
			@Override
			public void taskComplete(PDUBean pduBean) {}
		};
		pe.addToCallable(pePDU6);
		

		try {
			pe.invokeAll();
		} catch (Exception e) {
			log.error(e.getMessage(), e);
		}
	}
	
	protected void setAlerts(List<Alert> dashboardAlerts) {
		Executions.getCurrent().getSession().setAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_ALERTS_LIST, dashboardAlerts);
	}

	public Map<String, DashboardBean> getTags() {
		return tags;
	}

	public Map<String, DashboardBean> getSeverities() {
		return severities;
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