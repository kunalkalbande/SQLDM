package com.idera.sqldm.ui.dashboard;

import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.SortedMap;
import java.util.SortedSet;
import java.util.TreeSet;

import com.idera.sqldm.ui.preferences.DashboardPreferencesBean;
import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.bind.annotation.NotifyChange;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zk.ui.util.Clients;
import org.zkoss.zul.Caption;
import org.zkoss.zul.Grid;
import org.zkoss.zul.Groupbox;
import org.zkoss.zul.ListModel;
import org.zkoss.zul.ListModelList;

import com.idera.common.Utility;
import com.idera.sqldm.data.DashboardAlertsByInstanceWidget;
import com.idera.sqldm.data.DashboardInstance;
import com.idera.sqldm.data.DashboardSeverityGroup;
import com.idera.sqldm.data.DashboardWidgetsFacade;
import com.idera.sqldm.data.SeverityCodeToStringEnum;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.utils.SQLdmConstants;

@SuppressWarnings("rawtypes")
public class LeftNavigationComposer extends DashboardComposer {

	private final Logger log = Logger.getLogger(LeftNavigationComposer.class);

	private EventQueue<Event> eq;
	@Wire
	Grid partialByStatusGrid, partialByGroupGrid,
			partialByCriticalInstancesGrid;
	@Wire
	Groupbox byStatusBox, byGroupBox, byCriticalInstancesBox;
	@Wire
	Caption byStatusCaption, byGroupCaption, byCriticalInstancesCaption;
	private ListModel<DashboardBean> tagsModel;
	private ListModel<DashboardBean> severityModel;
	private ListModel<CriticalInstance> criticalInstancesModel;
	private static String OPEN_TRUE = "open-true";
	private static String OPEN_FALSE = "open-false";
	private String displayMessageForTags;
	private String displayMessageForSeverity;
	private String displayMessageForCriticalInstances;
	List<DashboardSeverityGroup> severityGroup;
	Map<String, Integer> severityInstancesMap;

    private String filterId = "";

    public String getFilterId() {
        return filterId;
    }

    public void setFilterId(String filterId) {
        this.filterId = filterId;
    }


    public boolean checked(String name){
        return  filterId == name;
    }
    private String productInstanceName;

	@Init
	public void init() {
		setDisplayMessageForTags(ELFunctions
				.getMessage(SQLdmI18NStrings.LOADING_DATA));
		setDisplayMessageForSeverity(ELFunctions
				.getMessage(SQLdmI18NStrings.LOADING_DATA));
		setDisplayMessageForCriticalInstances(ELFunctions
				.getMessage(SQLdmI18NStrings.LOADING_DATA));
		EventQueue<Event> eqForDashboardRefresh = EventQueues.lookup(
				DashboardConstants.DASHBOARD_QUEUE_NAME, EventQueues.SESSION,
				true);
		eqForDashboardRefresh.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if (DashboardConstants.DASHBOARD_REFRESH_EVENT_NAME
						.equalsIgnoreCase(event.getName())) {
					generateLeftNavigationModel();
				}
			}
		});
		// generateLeftNavigationModel();
		generateLeftNavigationSeveritiesModel1();
	}

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view) {
    	productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
        DashboardPreferencesBean dsdb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();
        if (dsdb != null && dsdb.getLeftCategoryGroup() != null) {
            filterId = dsdb.getLeftCategoryGroup().getName();
        }
		Selectors.wireComponents(view, this, false);
	}

	public void generateLeftNavigationModel() {
		generateLeftNavigationSeveritiesModel();
		generateLeftNavigationTagsModel();
		generateLeftNavigationCrititcalInstances();
	}

	protected void generateLeftNavigationCrititcalInstances() {
		SortedSet<DashboardAlertsByInstanceWidget> dabiwList = new TreeSet<DashboardAlertsByInstanceWidget>(DashboardWidgetsFacade.ALERTS_BY_INSTANCE_COMPARATOR);
		try {
			dabiwList.addAll(DashboardWidgetsFacade.getNumAlertsByInstance(productInstanceName));
		} catch (Exception e) {
			log.error("Exception in getting alertsByInstance from server");
			log.error(e.getMessage(), e);
		}
		if(dabiwList == null || dabiwList.size() == 0) {
			setDisplayMessageForCriticalInstances(ELFunctions
					.getMessage(SQLdmI18NStrings.NO_CRITICAL_INSTANCES_FOUND));
			return;
		}
		
		List<CriticalInstance> criticalList = new LinkedList<>();
		int count = 0;
		for(DashboardAlertsByInstanceWidget di:dabiwList ){
			count++;
			if (count > 10){
				break;
			}
			//CriticalInstance criticalInstanceObj = new CriticalInstance(di.getInstanceId(), di.getInstanceName()); @author Saumyadeep
			CriticalInstance criticalInstanceObj = new CriticalInstance(di.getInstanceId(), di.getInstanceName(),di.getFriendlyServerName(),di.getDisplayName());
			criticalList.add(criticalInstanceObj);
		}
		if(criticalList.isEmpty()){
			setDisplayMessageForCriticalInstances(ELFunctions
					.getMessage(SQLdmI18NStrings.NO_CRITICAL_INSTANCES_FOUND));
		}
		setCriticalInstancesModel(criticalList);

	}
	
	protected void generateLeftNavigationSeveritiesModel1() {
		List<DashboardBean> list = new LinkedList<DashboardBean>();
		for (SeverityCodeToStringEnum sctse : SeverityCodeToStringEnum
				.getLeftNavigationStatusList()) {
			DashboardBean db = new DashboardBean(-1, sctse.getUiLabel());
			list.add(db);
		}
		setSeverityModel(list);
	}

	protected void generateLeftNavigationSeveritiesModel() {
		List<DashboardBean> list = new LinkedList<DashboardBean>();
		Map<String, DashboardBean> map = (HashMap<String, DashboardBean>) (Executions
				.getCurrent().getDesktop()
				.getAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_SEVERITIES));
		if (map != null && map.size() != 0) {
			for (SeverityCodeToStringEnum sctse : SeverityCodeToStringEnum
					.getLeftNavigationStatusList()) {
				DashboardBean db = map.get(sctse.getUiLabel());
				Integer severity = -1;
				if (db.getInstances() != null && db.getInstances().size() > 0) {
					severity = db.getMaxSeverity();
				}
				DashboardBean db1 = new DashboardBean(severity,
						db.getInstances(), db.getName(), db.getSctse());
				list.add(db1);
			}
		} else {
			setDisplayMessageForSeverity(ELFunctions
					.getMessage(SQLdmI18NStrings.NO_DATA_FOUND));
		}
		setSeverityModel(list);
	}

	protected void generateLeftNavigationTagsModel() {
		SortedMap<String, DashboardBean> map = (SortedMap<String, DashboardBean>) (Executions
				.getCurrent().getDesktop()
				.getAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_TAGS));
		if (map == null || map.isEmpty()) {
			setDisplayMessageForTags(ELFunctions
					.getMessage(SQLdmI18NStrings.NO_TAGS_FOUND));
			setTagsModel(new LinkedList<DashboardBean>());
		} else {
			setTagsModel(map.values());
		}
	}

	public ListModel<DashboardBean> getTagsModel() {
		return tagsModel;
	}
	@NotifyChange
	public void setTagsModel(Collection<DashboardBean> tagsModel) {
		this.tagsModel = new ListModelList<>(tagsModel);
		BindUtils.postNotifyChange(null, null, this, "tagsModel");
	}

	public ListModel<CriticalInstance> getCriticalInstancesModel() {
		return criticalInstancesModel;
	}

	public ListModel<DashboardBean> getSeverityModel() {
		return severityModel;
	}

	@NotifyChange
	public void setSeverityModel(List<DashboardBean> severityModel) {
		this.severityModel = new ListModelList<>(severityModel);
		BindUtils.postNotifyChange(null, null, this, "severityModel");
	}

	@NotifyChange
	public void setCriticalInstancesModel(
			List<CriticalInstance> criticalList) {
		this.criticalInstancesModel = new ListModelList<>(criticalList);
		BindUtils.postNotifyChange(null, null, this, "criticalInstancesModel");
	}
    @Command("filterInstancesByStatus")
    public void registerClickByStatus(@BindingParam("name") String name) {
		// for now assuming the ids of the links will be mapped to the db values of these fields
		// can add a new attribute and use that if ids are not feasible
    	eq = EventQueues.lookup(SQLdmConstants.DASHBOARD_UPDATE_INSTANCES_EVENT_QUEUE, EventQueues.DESKTOP, true);
			log.debug(name + " registerClickByStatus EVENT GENERATED " +eq);

    	if(eq != null) {
    		Map<Integer, Object> map = new HashMap<Integer, Object>();
    		Map<String, DashboardBean> tMap = (Map<String, DashboardBean>) Executions.getCurrent().getDesktop().getAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_SEVERITIES);
    		
    		if (tMap != null && tMap.get(name).getInstances() != null) {
				for (DashboardInstance instance : tMap.get(name).getInstances()) {
					map.put(instance.getOverview().getSQLServerId(), true);
				}

				PreferencesUtil.getInstance().setFilteredInstancesMap(map);
	            eq.publish(new Event("onStatusLinkClick", null, name));
    		}
    	}
    	int filter=-1;//OK Severity Code
    	if("CRITICAL".equalsIgnoreCase(name)) {
    		filter=8;
    	}else if("WARNING".equalsIgnoreCase(name)) {
    		filter=4;
    	}else if("MAINT. MODE".equalsIgnoreCase(name)) {
    		filter=2;
    	}
    	else if("OK".equalsIgnoreCase(name)) {
    		filter=1;
    	}
    	
    	String event="Ext.fireEvent('SeverityFilterApplied', \""+ filter + "\")";
		Clients.evalJavaScript(event);
    }
    @Command("filterInstancesByTags")
    public void registerClickByTags(@BindingParam("name") String name) {
		// for now assuming the ids of the links will be mapped to the db values of these fields
		// can add a new attribute and use that if ids are not feasible
        eq = EventQueues.lookup(SQLdmConstants.DASHBOARD_UPDATE_INSTANCES_EVENT_QUEUE, EventQueues.DESKTOP, true);
        if(eq != null) {
        	Map<Integer, Object> map = new HashMap<Integer, Object>();
        	Map<String, DashboardBean> tMap = (Map<String, DashboardBean>) Executions.getCurrent().getDesktop().getAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_TAGS);
        	if (tMap.get(name).getInstances() != null) {
	        	for (DashboardInstance instance : tMap.get(name).getInstances()) {
					map.put(instance.getOverview().getSQLServerId(), true);
				}
        	}
			PreferencesUtil.getInstance().setFilteredInstancesMap(map);
			
        	eq.publish(new Event("onTagsLinkClick", null, name));
        }
    }
    public static class CriticalInstance {
    	private int instanceId;
    	private String name;
		
    	// @author Saumyadeep 
    	// Friendly Begin
    	
    	private String friendlyServerName;

    	public void setFriendlyServerName(String friendlyServerName) {
    		this.friendlyServerName = friendlyServerName;
    	}
    	
    	public String getFriendlyServerName() {
    		return this.friendlyServerName;
    	}

    	private String displayName;
    	
    	public String getDisplayName() {
    		if(this.getFriendlyServerName()!= null)	
    			return this.getFriendlyServerName();
    		else
    			return this.getName();
    	}

    	public void setDisplayName(String displayName) {
    		this.displayName = displayName;
    	}

    	// Friendly End
    	
    	/*public CriticalInstance(int instanceId, String name) {
			super();
			this.instanceId = instanceId;
			this.name = name;
		} @author Saumyadeep */
    	
    	public CriticalInstance(int instanceId, String name,String friendlyServerName,String displayName) {
			super();
			this.instanceId = instanceId;
			this.name = name;
			this.friendlyServerName = friendlyServerName;
			this.displayName = displayName;
			
		}
    	
		
    	public int getInstanceId() {
			return instanceId;
		}
    	
		public String getName() {
			return name;
		}    	
    }
    @Command("onOpenGroupBox")
	public void onOpenStatusGroupBox(@ContextParam(ContextType.COMPONENT)Component comp) {
		Groupbox tmp = (Groupbox) comp;
		//onOpenTagsGroupBox(tmp);
		tmp.getCaption().setClass(tmp.isOpen() ? OPEN_TRUE : OPEN_FALSE);
	}
	public String getDisplayMessageForTags() {
		return displayMessageForTags;
	}
	public void setDisplayMessageForTags(String displayMessageForTags) {
		this.displayMessageForTags = displayMessageForTags;
		BindUtils.postNotifyChange(null, null, this, "displayMessageForTags");
	}
	public String getDisplayMessageForSeverity() {
		return displayMessageForSeverity;
	}
	public void setDisplayMessageForSeverity(String displayMessageForSeverity) {
		this.displayMessageForSeverity = displayMessageForSeverity;
		BindUtils.postNotifyChange(null, null, this, "displayMessageForSeverity");
	}
	public String getDisplayMessageForCriticalInstances() {
		return displayMessageForCriticalInstances;
	}
	public void setDisplayMessageForCriticalInstances(String displayMessageForCriticalInstances) {
		this.displayMessageForCriticalInstances = displayMessageForCriticalInstances;
		BindUtils.postNotifyChange(null, null, this, "displayMessageForCriticalInstances");
	}

}
