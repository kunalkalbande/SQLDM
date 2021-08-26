
package com.idera.sqldm.ui.dashboard;

import java.io.IOException;
import java.io.Serializable;
import java.io.UnsupportedEncodingException;
import java.text.ParseException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.StringTokenizer;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.apache.commons.lang.StringUtils;
import org.apache.http.auth.Credentials;
import org.apache.log4j.Logger;
import org.springframework.security.context.SecurityContext;
import org.zkoss.addon.columnchooser.Columnchooser;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.*;
import org.zkoss.json.JSONArray;
import org.zkoss.json.JSONObject;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Desktop;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Session;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.UiException;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Div;
import org.zkoss.zul.Grid;
import org.zkoss.zul.GroupComparator;
import org.zkoss.zul.Groupbox;
import org.zkoss.zul.Include;
import org.zkoss.zul.ListModel;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Paging;
import org.zkoss.zul.Window;
import org.zkoss.zul.event.PagingEvent;
import org.zkoss.zk.ui.util.Clients;

import com.idera.common.rest.CoreRestClient;
import com.idera.common.rest.ICSTokenCredentials;
import com.idera.common.rest.RestException;
import com.idera.cwf.model.Product;
import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.DashboardInstance;
import com.idera.sqldm.data.DashboardInstanceFacade;
import com.idera.sqldm.data.SeverityCodeToStringEnum;
import com.idera.sqldm.data.UserSettingFacade;
import com.idera.sqldm.data.alerts.Alert;
import com.idera.sqldm.data.alerts.AlertCategoriesDetails;
import com.idera.sqldm.data.alerts.AlertCategoriesDetails.AlertCategoriesEnum;
import com.idera.sqldm.data.alerts.AlertException;
import com.idera.sqldm.data.alerts.AlertFacade;
import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.rest.SQLDMRestClient;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.server.web.WebUtil;
import com.idera.sqldm.server.web.session.SessionUtil;
import com.idera.sqldm.server.web.session.UserPreferences;
import com.idera.sqldm.server.web.session.UserSessionSettings;
import com.idera.sqldm.ui.dashboard.instances.InstanceCategoryTab;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean.DashboardCategoryGroup;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean.DashboardGroupByInstances;
import com.idera.sqldm.ui.preferences.DashboardPreferencesBean.DashboardInstanceView;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstanceAlertsPreferencesBean;
import com.idera.sqldm.ui.preferences.SingleInstancePreferencesBean;
import com.idera.sqldm.utils.GridExporter;
import com.idera.sqldm.utils.SQLdmConstants;
import com.idera.sqldm.utils.Utility;
import org.springframework.security.context.SecurityContextHolder;

import javax.xml.bind.DatatypeConverter;

@SuppressWarnings("rawtypes")
public class DashboardInstanceViewComposer extends DashboardComposer{

	private final Logger log = Logger.getLogger(DashboardInstanceViewComposer.class);
	private String OFFSET_IN_HOURS = "0.0";
	private final int INSTANCES_COUNT = 10;
	private String titleLabel, criticalInstanceCountLabel, warningInstanceCountLabel, maintenanceInstanceCountLabel, okInstanceCountLabel;
	private Boolean isThumbnailVisible = true, isListVisible, isHeatMapVisible;
	private Boolean isListEmpty;
	private int listRowsCount = INSTANCES_COUNT;
	private int instanceListRowsCount = INSTANCES_COUNT;
	private ListModel<DashboardInstance> instancesModel;
	public static final String TAG_NOT_FOUND_LABEL = "Untagged";
	private InstanceGroupModel instanceGroupModel;
	private boolean isInstancesLoaded = false;
	private List<DashboardInstance> m_dashboardInstances = new LinkedList<DashboardInstance>();
	private List<DashboardInstance> selectedLeftNavigationFilteredInstances;
	private int maxSeverity;
	private static final String HEATMAPID_PREFIX = "groupbox-heatmap-";
	private String displayMessageForInstancesList;
	EventQueue<Event> eqForManualReload;
	private String selectedCategoryForSorting = DashboardGroupByInstances.GROUPBY
			.getGroupBy();
	private String searchText;
	private Comparator blockedProcessAscComparator,
	blockedProcessDescComparator;
	private Comparator cpuActivityAscComparator, cpuActivityDescComparator;
	private Comparator availMemAscComparator, availMemDescComparator;
	private String selectedFilterId;
	@Wire
	Paging instancesListPgId;
	@Wire
	Grid instanceListGrid;
	@Wire
	Paging instancesGroupPgId;
	@Wire
	Grid instanceGroup;
	@Wire
	Div instancesThumbnailLayoutNew;

	@Wire
	Columnchooser columnChooser;
	
	private HashMap<String, InstancesColumns> instancesColumn = new HashMap<String, InstancesColumns>();

	// available column list
	private List<String> columnList = new ArrayList<String>(); 
	// Displayed column list
	private List<String> defaultColumnList = new ArrayList<String>(); 
	private List<String> dbOrder;
	

	@AfterCompose
	public void afterCompose(@ContextParam(ContextType.VIEW) Component view)
			throws Exception {
		Selectors.wireComponents(view, this, true);
		if (instanceListGrid != null && instancesListPgId != null) {
			instanceListGrid.setPaginal(instancesListPgId);
		}
		if (instanceGroup != null && instancesGroupPgId != null) {
			instanceGroup.setPaginal(instancesGroupPgId);
		}

		EventQueue<Event> eq = EventQueues.lookup("navigateToHeatmap",
				EventQueues.DESKTOP, true);
		eq.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if (event.getName().equals("navigate")) {
					showHeatMapView();
				}
			}
		});
		loadUserSettings();
	}

	private void loadUserSettings() {
		Session session = Sessions.getCurrent();
		UserSessionSettings settings = (UserSessionSettings)session.getAttribute(UserSessionSettings.SQLDM_USER_SESSION_SETTINGS_PROPERTY);
		List<String> visibleColClone = new ArrayList<>();
		List<String> hiddenColClone = new ArrayList<>();
		List<String> hiddenColumns = new ArrayList<>();
		try{
			if(settings.getUserData(UserPreferences.SQLDM_FRESH_LOGIN).equals("")){
				String selectedColumnStr=settings.getUserData("Grid_View_Columns_User_Preference");
				List<String> orderedColumns = getOrderedColumnList(false);
				for(int i = 0 ; i<orderedColumns.size() ; i++){
					String col = orderedColumns.get(i);
					if(!(selectedColumnStr.contains(col))){
						hiddenColumns.add(col);
					}
					if(selectedColumnStr.contains(col)){
						visibleColClone.add(col);
					} else if(instancesColumn!=null && instancesColumn.get(col).getDefaultFlag()){
						visibleColClone.add(col);
					}
					if(hiddenColumns.contains(col) && !instancesColumn.get(col).getDefaultFlag()){
						hiddenColClone.add(col);
					}
				}
				
				if(defaultColumnList!=null){
					defaultColumnList.clear();
					defaultColumnList = visibleColClone;
				}
				if(columnList!=null){
					columnList.clear();
					columnList = hiddenColClone;
				}
				PreferencesUtil.getInstance().setDashboardPreferencesInSession(null, null, DashboardInstanceView.LIST, null, -1, -1, null, defaultColumnList, columnList);
			}
		}
		catch(Exception e){
			
		}
	}

	@DependsOn({ "selectedCategoryForSorting", "searchText" })
	public InstanceGroupModel getInstanceGroupModel() {
		if (selectedCategoryForSorting != null
				&& !"".equals(selectedCategoryForSorting)
				&& !DashboardGroupByInstances.GROUPBY.getGroupBy()
				.equalsIgnoreCase(selectedCategoryForSorting)) {
			groupInstances(selectedCategoryForSorting);
		}
		return instanceGroupModel;
	}

	@NotifyChange
	public void setInstanceGroupModel(InstanceGroupModel instanceGroupModel) {
		this.instanceGroupModel = instanceGroupModel;
	}

	@DependsOn({ "selectedCategoryForSorting" })
	public Boolean getShowGroups() {
		return isShowGroups();
	}

	private Boolean isShowGroups() {
		switch (selectedCategoryForSorting.toUpperCase()) {
		case "TAGS": {
			PreferencesUtil.getInstance().setDashboardPreferencesInSession(
					null, null, null, DashboardGroupByInstances.TAGS, -1, -1,
					null);
			break;
		}
		case "SEVERITY": {
			PreferencesUtil.getInstance().setDashboardPreferencesInSession(
					null, null, null, DashboardGroupByInstances.SEVERITY, -1,
					-1, null);
			break;
		}
		case "SQLDMREPO":{
			PreferencesUtil.getInstance().setDashboardPreferencesInSession(
					null, null, null, DashboardGroupByInstances.SQLDM_REPO, -1,
					-1, null);
			break;
		}
		default:
			PreferencesUtil.getInstance().setDashboardPreferencesInSession(
					null, null, null, DashboardGroupByInstances.GROUPBY, -1,
					-1, null);
		}

		if (DashboardGroupByInstances.GROUPBY.getGroupBy().equalsIgnoreCase(
				selectedCategoryForSorting)) {
			return false;
		}

		return true;
	}

	public String getSearchText() {
		return searchText;
	}

	public int getMaxSeverity() {
		return maxSeverity;
	}

	@NotifyChange
	public void setMaxSeverity(int maxSeverity) {
		this.maxSeverity = maxSeverity;
		BindUtils.postNotifyChange(null, null, this, "maxSeverity");
		BindUtils.postNotifyChange(null, null, this, "maxSeverityImage");
	}

	@NotifyChange
	public void setSearchText(String searchText) {
		this.searchText = searchText;
		BindUtils.postNotifyChange(null, null, this, "searchText");
	}

	public Boolean getIsThumbnailVisible() {
		return isThumbnailVisible;
	}

	@NotifyChange("isThumbnailVisible")
	public void setIsThumbnailVisible(Boolean isThumbnailVisible) {
		this.isThumbnailVisible = isThumbnailVisible;
		BindUtils.postNotifyChange(null, null, this, "isThumbnailVisible");
	}

	public int getInstanceListRowsCount() {
		return instanceListRowsCount;
	}

	public void setInstanceListRowsCount(int instanceListRowsCount) {
		this.instanceListRowsCount = instanceListRowsCount;
		BindUtils.postNotifyChange(null, null, this, "instanceListRowsCount");
	}

	public int getListRowsCount() {
		return listRowsCount;
	}

	@NotifyChange("listRowsCount")
	public void setListRowsCount(int listRowsCount) {
		this.listRowsCount = listRowsCount;
		BindUtils.postNotifyChange(null, null, this, "listRowsCount");
		//loadInstances(false);
	}

	@Command("setInstancesListRowsCount")
	public void setInstancesListRowsCount() {
		setListRowsCount(this.instanceListRowsCount);
		log.debug("setting pageCount as " + this.instanceListRowsCount);
		PreferencesUtil.getInstance().setDashboardPreferencesInSession(null,
				null, null, null, this.instanceListRowsCount, -1, null);
	}

	/*-----------Auth:Rishabh Mishra---------------*/

	@Command 
	public void onClickgear() {

		try {
			SessionUtil.getSecurityContext();
			Map<Object, Object> args = new HashMap<Object, Object>();
			Window window = (Window) Executions.createComponents("~./sqldm/com/idera/sqldm/ui/dashboard/gearView.zul", null, args);
			window.doModal();
		} catch(UiException e) {

		}
	}


	public Boolean getIsListVisible() {
		return isListVisible;
	}
	@DependsOn({"selectedCategoryForSorting"})
	public Boolean getIsListAndGroupVisible() {
		return getIsListVisible() && !isShowGroups();
	}

	public String CredentialsToAuthHeader(Credentials creds) throws UnsupportedEncodingException {
		return "Basic " + DatatypeConverter
				.printBase64Binary((creds.getUserPrincipal().getName() + ":" + creds.getPassword().toString()).getBytes("UTF-8"));
	}

	@GlobalCommand("triggerInstanceListGridLoad")
	public void triggerInstanceListGridLoad() {
		String header = null;
		try {
			ICSTokenCredentials curr = null;
			try {
				SecurityContext sc = SecurityContextHolder.getContext();
				curr = (ICSTokenCredentials)com.idera.server.web.session.SessionUtil.getCurrentUserCredentials();
				if (curr != null) {
					com.idera.sqldm.server.web.session.SessionUtil.setSessionVariable("SqldmUserSetting", curr);
					log.debug("Setting the Credentials " + curr.getUserPrincipal());
				}
			} catch (Exception e) {
				log.error("Error while setting the Credentials "+e.getMessage());
			}
			if (curr == null) {
				curr = (ICSTokenCredentials) com.idera.sqldm.server.web.session.SessionUtil.getSessionVariable("SqldmUserSetting");
				log.debug("Got from session " + curr);
			}
			if(curr instanceof ICSTokenCredentials) {
				header = ((ICSTokenCredentials)curr).getToken();
			}else {
				header = CredentialsToAuthHeader(curr);
			}
		} catch (Exception e) {
			log.error("Error in Getting header "+e.getMessage());
		}
		String cwfLocation="";
		try {
			cwfLocation = CoreRestClient.getInstance().getAllProducts().get(0).getLocation().toLowerCase();
		}catch(Exception e) {
		}
		Product currentdm = WebUtil.getCurrentProduct();
		boolean isLocalProduct=false;
		if(currentdm.getRestUrl().toLowerCase().contains(cwfLocation)){
			isLocalProduct=true;
		}
		String restUrl = currentdm.getRestUrl()+ "InstancesByName";
		if(isListVisible) {
			String event="Ext.fireEvent('loadInstancesList', \""+ restUrl + "\", \"" + header + "\", \""+isLocalProduct+"\")";
			Clients.evalJavaScript(event);
		}
	}

	@NotifyChange("isListVisible")
	public void setIsListVisible(Boolean isListVisible) {
		this.isListVisible = isListVisible;
		BindUtils.postNotifyChange(null, null, this, "isListVisible");
		BindUtils.postNotifyChange(null, null, this, "isListAndGroupVisible");
		BindUtils.postGlobalCommand(null, null, "triggerInstanceListGridLoad", null);
	}

	public Boolean getIsHeatMapVisible() {
		return isHeatMapVisible;
	}


	@Command("heatMapViewOpen")
	public void heatMapViewOpen(
			@ContextParam(ContextType.COMPONENT) Component component, @BindingParam("instanceId") Integer Id ,  @BindingParam("instance") DashboardInstance instance, @BindingParam("swaID") Integer swaID, @BindingParam("instanceName") String instanceName) {
		Groupbox gb = (Groupbox) component;
		Integer instanceId = null;
		try {
			String id = gb.getId();
			instanceId = Integer.parseInt(id.replace(HEATMAPID_PREFIX, ""));
		} catch (Exception e) {
			log.error(e);
		}
		if (gb.isOpen()) {
			populateThumbnailCharts(Id , instance,instanceName, swaID );
			if (instanceId != null) {
				PreferencesUtil.getInstance()
				.addOpenedInstanceDashboardHeatMapViewInSession(
						instanceId);
			}
			((Div) ((Div) ((Groupbox) component).getParent()).getParent())
			.setStyle("height:367px;");
		} else {
			if (instanceId != null) {
				PreferencesUtil.getInstance()
				.removeClosedInstanceDashboardHeatMapViewInSession(
						instanceId);
			}
			((Div) ((Div) ((Groupbox) component).getParent()).getParent())
			.setStyle("height:49px;");

		}
	}

	private void populateThumbnailCharts(Integer instanceId , DashboardInstance instance, String instanceName, Integer swaID) {
		if(instanceId == null){
			return;
		}
		Include in = null;
		if(instancesThumbnailLayoutNew.hasFellow("includeGraph_"+ instance.getProduct().getProductId() +"_"+instanceId)){
			in = (Include) instancesThumbnailLayoutNew.getFellow("includeGraph_"+ instance.getProduct().getProductId()+ "_"+instanceId);
			in.setSrc("~./sqldm/com/idera/sqldm/ui/dashboard/thumbnailView.zul?id="+instanceId +"&productId="+instance.getProduct().getProductId() + 
					"&productName="+instance.getProduct().getInstanceName() + "&swaID="+swaID + "&instanceName="+instanceName);
			//	in.setSrc("~./sqldm/com/idera/sqldm/ui/dashboard/thumbnailView.zul?id="+instanceId);

		}
	}

	@NotifyChange("isHeatMapVisible")
	public void setIsHeatMapVisible(Boolean isHeatMapVisible) {
		this.isHeatMapVisible = isHeatMapVisible;
		BindUtils.postNotifyChange(null, null, this, "isHeatMapVisible");
	}

	public String getTitleLabel() {
		return titleLabel;
	}

	public String getCriticalInstanceCountLabel() {
		return criticalInstanceCountLabel;
	}

	public String getWarningInstanceCountLabel() {
		return warningInstanceCountLabel;
	}

	public String getMaintenanceInstanceCountLabel() {
		return maintenanceInstanceCountLabel;
	}
	public String getOkInstanceCountLabel() {
		return okInstanceCountLabel;
	}

	public static enum FilterType {
		STATUS("Status"), TAG("TagId"),SQLDMREPO("SQLdmRepo"),SEARCH("Search");
		private String fieldLabel;

		FilterType(String fieldLabel) {
			this.fieldLabel = fieldLabel;
		}

		public String getFieldLabel() {
			return fieldLabel;
		}

	}

	@Init
	public void init() {
		setOffSet();
		setInstancesLoaded(false);
		setupDashboardFromPreferences();
		// super.init();
		setDisplayMessageForInstancesList(ELFunctions
				.getMessage(SQLdmI18NStrings.LOADING_DATA));
		instancesModel = new ListModelList<DashboardInstance>(
				new LinkedList<DashboardInstance>(), false);
		instanceGroupModel = new InstanceGroupModel(
				new LinkedList<DashboardInstanceWrapper>(),
				new InstanceSeverityComparator(), false);
		EventQueue<Event> eq = EventQueues.lookup(
				SQLdmConstants.DASHBOARD_UPDATE_INSTANCES_EVENT_QUEUE,
				EventQueues.DESKTOP, true);
		eq.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				leftNavigationListener(event, event.getName());
			}
		});

		EventQueue<Event> eqForDashboardRefresh = EventQueues.lookup(DashboardConstants.DASHBOARD_QUEUE_NAME, EventQueues.SESSION, true);
		eqForDashboardRefresh.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if(DashboardConstants.DASHBOARD_REFRESH_EVENT_NAME.equalsIgnoreCase(event.getName())) {
					loadInstances(true);
				}
			}
		});
		//loadInstances(true);

		blockedProcessAscComparator = DashboardInstanceFacade.DASHBOARD_INSTANCE_BLOCKED_PROCESSES_COMPARATOR_ASC;
		blockedProcessDescComparator = DashboardInstanceFacade.DASHBOARD_INSTANCE_BLOCKED_PROCESSES_COMPARATOR_DESC;
		cpuActivityAscComparator = DashboardInstanceFacade.DASHBOARD_INSTANCE_CPU_ACTIVITY_COMPARATOR_ASC;
		cpuActivityDescComparator = DashboardInstanceFacade.DASHBOARD_INSTANCE_CPU_ACTIVITY_COMPARATOR_DESC;
		availMemAscComparator = DashboardInstanceFacade.DASHBOARD_INSTANCE_AVAILABLE_MEM_COMPARATOR_ASC;
		availMemDescComparator = DashboardInstanceFacade.DASHBOARD_INSTANCE_AVAILABLE_MEM_COMPARATOR_DESC;
		//getSortingCategoryList();

		addColumnsToList();
		setVisibleAndHiddenColumnsList();
	}
	private void setupDashboardFromPreferences() {
		/*FilterType ft = FilterType.STATUS;
        String name = "All";*/
		int code = -1;
		int defaultInstanceRowCount = INSTANCES_COUNT;
		String groupBy = DashboardGroupByInstances.GROUPBY.getGroupBy();
		DashboardPreferencesBean dsdb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();
		if (dsdb != null) {
			/*if (dsdb.getLeftCategoryGroup() != null) {
        		ft = dsdb.getLeftCategoryGroup().getFilterType();
        	}
        	if (dsdb.getLeftCategoryGroup() != null) {
        		name = dsdb.getLeftCategoryGroup().getName();
        	}*/
			if (dsdb.getGroupByInstances() != null) {
				groupBy = dsdb.getGroupByInstances().getGroupBy();
			}
			if (dsdb.getInstancesView() != null) {
				code = dsdb.getInstancesView().getCode();
			}
			if (dsdb.getInstanceRowCount() != -1) {
				defaultInstanceRowCount = dsdb.getInstanceRowCount();
			}
			/*log.debug(" Ft : " + ft.getFieldLabel() );
        	log.debug(" name : " + name );*/
			log.debug(" groupBy : " + groupBy );
			log.debug(" code : " + code );
			log.debug(" defaultInstanceRowCount : " + defaultInstanceRowCount );
		}
		defaultInstanceRowCount = defaultInstanceRowCount==1 ? 2 : defaultInstanceRowCount;
		setListRowsCount(defaultInstanceRowCount);
		setInstanceListRowsCount(defaultInstanceRowCount);
		switch(code) {
		case 1:
			showListView();
			setSelectedCategoryForSorting(groupBy);
			break;
		case 3:
			setSelectedCategoryForSorting(DashboardGroupByInstances.GROUPBY.getGroupBy());
			showHeatMapView();
			break;
		case -1:
		case 2:
		default:
			setSelectedCategoryForSorting(DashboardGroupByInstances.GROUPBY.getGroupBy());
			showThumbnailView();
		}    	
	}
	private void loadInstances(boolean reload) {
		FilterType ft = FilterType.STATUS;
		String name = SeverityCodeToStringEnum.ALL.getUiLabel();
		/*int code = -1;
        int defaultInstanceRowCount = INSTANCES_COUNT;
        String groupBy = DashboardGroupByInstances.GROUPBY.getGroupBy();*/
		DashboardPreferencesBean dsdb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();
		if (dsdb != null) {
			if (dsdb.getLeftCategoryGroup() != null) {
				ft = dsdb.getLeftCategoryGroup().getFilterType();
			}
			if (dsdb.getLeftCategoryGroup() != null) {
				name = dsdb.getLeftCategoryGroup().getName();
			}
			if(dsdb.getSearchText() != null) {
				setSearchText(dsdb.getSearchText());
			} else {
				setSearchText("");
			}
			/*if (dsdb.getGroupByInstances() != null) {
        		groupBy = dsdb.getGroupByInstances().getGroupBy();
        	}
        	if (dsdb.getInstancesView() != null) {
        		code = dsdb.getInstancesView().getCode();
        	}
        	if (dsdb.getInstanceRowCount() != -1) {
        		defaultInstanceRowCount = dsdb.getInstanceRowCount();
        	}*/

			log.debug(" Ft : " + ft.getFieldLabel() );
			log.debug(" name : " + name );
			/*log.debug(" groupBy : " + groupBy );
        	log.debug(" code : " + code );
        	log.debug(" defaultInstanceRowCount : " + defaultInstanceRowCount );*/
		}
		filterInstances(ft, name);
		if(searchText != null && !"".equals(searchText)) {
			searchInstances();
		}
		/*if (reload) {
			setListRowsCount(defaultInstanceRowCount);
			setInstanceListRowsCount(defaultInstanceRowCount);
		}*/

		/*switch(code) {
        	case 2:
        		setSelectedCategoryForSorting(DashboardGroupByInstances.GROUPBY.getGroupBy());
        		showThumbnailView();
        		break;
    		case 3:
    			setSelectedCategoryForSorting(DashboardGroupByInstances.GROUPBY.getGroupBy());
    			showHeatMapView();
        		break;
        	case -1:
        	case 1:
        	default:
    			showListView();
    			setSelectedCategoryForSorting(groupBy);
        }*/
	}

	@Command
	public void showListView() {
		EventQueue<Event> eqForDashboardRefresh = EventQueues.lookup(DashboardConstants.DASHBOARD_QUEUE_NAME, EventQueues.SESSION, true);
		if(eqForDashboardRefresh != null){
			eqForDashboardRefresh.publish(new Event("ClosePopup",null,"closePopup"));
		}
		PreferencesUtil.getInstance().setDashboardPreferencesInSession(null, null, DashboardInstanceView.LIST, null, -1, -1, null);
		updateViews(true, false, false);
	}


	@Command
	public void showThumbnailView() {
		EventQueue<Event> eqForDashboardRefresh = EventQueues.lookup(DashboardConstants.DASHBOARD_QUEUE_NAME, EventQueues.SESSION, true);
		if(eqForDashboardRefresh != null){
			eqForDashboardRefresh.publish(new Event("ClosePopup",null,"closePopup"));
		}
		PreferencesUtil.getInstance().setDashboardPreferencesInSession(null, null, DashboardInstanceView.THUMBNAIL, null, -1, -1, null);
		updateViews(false, true, false);
	}

	@Command
	public void categoryClicked(@BindingParam("category") String category, @BindingParam("instanceid") Integer instanceId,@BindingParam("goTo") String goTo) {

		List<Alert> alertList = null;
		String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
		try {
			alertList = AlertFacade.getAllAlerts(productInstanceName, true, instanceId, category , OFFSET_IN_HOURS);
		} catch (AlertException e) {
			log.error(e.getMessage(), e);
		}
		if ((goTo == null || goTo.equals("")) && (alertList != null && alertList.size() >= 1)) {
			return;
		}
		if (goTo.equals("alerts")) {
			if(alertList != null && !alertList.isEmpty()){
				Alert alert = alertList.get(0);
				Map<String, Object> args = new HashMap<String, Object>();
				args.put("alert", alert);
				args.put("alertsList", alertList);
				Window window = (Window)Executions.createComponents(
						"~./sqldm/com/idera/sqldm/ui/alerts/alertsView.zul", null, args);
				window.doModal();
			}else{
				InstanceCategoryTab tab = InstanceCategoryTab.findByStatus(category.toLowerCase());
				int categoryId = tab == null ? 0 : tab.getId();
				SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
				pref.setSelectedCategory(categoryId);
				if(categoryId == 3){
					pref.setSelectedSubCategory(2);
				}
				else{
					pref.setSelectedSubCategory(1);
				}
				PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);

				Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance"+"/"+instanceId));
			}
		}
		else  {
			InstanceCategoryTab tab = InstanceCategoryTab.findByStatus(category.toLowerCase());
			int categoryId = tab == null ? 0 : tab.getId();
			SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
			pref.setSelectedCategory(categoryId);
			if(categoryId == 3){
				pref.setSelectedSubCategory(2);
			}
			else{
				pref.setSelectedSubCategory(1);
			}
			PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);
			Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance"+"/"+instanceId));
		}
	}

	@Command
	public void showHeatMapView() {
		PreferencesUtil.getInstance().setDashboardPreferencesInSession(null, null, DashboardInstanceView.HEATMAP, null, -1, -1, null);
		updateViews(false, false, true);
	}


	@Command
	public void exportGrid(@BindingParam("instanceListGrid") Grid instanceListGrid) {

		GridExporter.exportToPdf(instanceListGrid, DashboardInstance.class, "getMap", "InstanceList");

	}

	@Command
	public void exportToExcel(@BindingParam("instanceListGrid") Grid instanceListGrid) {

		GridExporter.exportToExcel(instanceListGrid, DashboardInstance.class, "getMap", "InstanceList");

	}

	@Command
	public void exportToXml(@BindingParam("instanceListGrid") Grid instanceListGrid,
			@BindingParam("title") String title) {

		StringTokenizer str = new StringTokenizer(title, "(");

		GridExporter.exportToXml(instanceListGrid, str.nextToken(), "(" + str.nextToken(), DashboardInstance.class, "getMap", "InstanceList");

	}


	private void updateViews(Boolean isListVisible, Boolean isThumbnailVisible, Boolean isHeatMapVisible) {
		this.setIsListVisible(isListVisible);
		this.setIsThumbnailVisible(isThumbnailVisible);
		this.setIsHeatMapVisible(isHeatMapVisible);
	}

	private void leftNavigationListener(Event event, String eventName) {
		if("onStatusLinkClick".equals(eventName)){
			setSearchText("");
			String severity = (String) event.getData();
			PreferencesUtil.getInstance().setDashboardPreferencesInSession(DashboardCategoryGroup.SEVERITY, severity, null, null, -1, -1, "");
			filterInstances(FilterType.STATUS, severity);
		} else if("onTagsLinkClick".equals(eventName)){
			setSearchText("");
			//updateListTitle("Development", m_dashboardInstances);
			String tag = (String) event.getData();
			PreferencesUtil.getInstance().setDashboardPreferencesInSession(DashboardCategoryGroup.TAGS, tag, null, null, -1, -1, "");
			filterInstances(FilterType.TAG, tag);        	
			//String value = (String)event.getData();			// will be used by the filterInstances method to filter according to the requested value
			// for now assuming the ids of the links will be mapped to the db values of these fields
			// can add a new attribute and use that if ids are not feasible
		} else if("onSearch".equals(eventName)){
			PreferencesUtil.getInstance().setDashboardPreferencesInSession(null, null, null, null, -1, -1, searchText);
		}

	}

	private void refreshInstances(String type, List<DashboardInstance> instances) {
		updateInstancesViewModel(instances);
		updateListTitle(type, instances);
		BindUtils.postNotifyChange(null, null, this, "instanceGroupModel");
	}


	@NotifyChange("instancesModel")
	private void updateInstancesViewModel(List<DashboardInstance> instances) {
		setInstancesLoaded(true);
		//System.out.println("Size of list is :" + instances.size() + m_dashboardInstances.size());
		if(instances == null || instances.size() == 0) {
			m_dashboardInstances = new LinkedList<>();
			setIsListEmpty(true);
			setDisplayMessageForInstancesList(ELFunctions.getMessage(SQLdmI18NStrings.NO_INSTANCES_REGISTERED_FOR_SELECTED_FILTER));
		} else {
			m_dashboardInstances = instances;
			setIsListEmpty(false);
		}

		BindUtils.postNotifyChange(null, null, this, "instancesModel");
	}

	public boolean getIsInstancesLoaded() {
		return isInstancesLoaded;
	}

	public void setInstancesLoaded(boolean isInstancesLoaded) {
		this.isInstancesLoaded = isInstancesLoaded;
		BindUtils.postNotifyChange(null, null, this, "isInstancesLoaded");
		BindUtils.postNotifyChange(null, null, this, "maxSeverityImage");
	}

	public Boolean getIsListEmpty() {
		return isListEmpty;
	}

	public void setIsListEmpty(Boolean isListEmpty) {
		this.isListEmpty = isListEmpty;
		BindUtils.postNotifyChange(null, null, this, "isListEmpty");
	}

	public List<String> getSortingCategoryList() {
		//	List<Product> productsList = new ArrayList<Product>();
		List<String> categoriesForSorting = new ArrayList<>();


		categoriesForSorting.add(DashboardGroupByInstances.GROUPBY.getGroupBy());
		categoriesForSorting.add("Severity");
		categoriesForSorting.add("Tags");
		categoriesForSorting.add("SQLdmRepo");
		/*  		try {
			productsList = CoreRestClient.getInstance().getProducts();
			for (Product product : productsList) {
				System.out.println(product.getName());
				categoriesForSorting.add(product.getName());
			}
		} catch (RestException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}*/
		return categoriesForSorting; 
	}

	public String getSelectedCategoryForSorting() {
		return selectedCategoryForSorting;
	}


	@NotifyChange("instancesModel")
	@Command("instancesListPaginateCommand")		
	public void instancesListPaginate (@ContextParam(ContextType.TRIGGER_EVENT) PagingEvent event) {
		int pgno = event.getActivePage();
		int ofs = pgno * event.getPageable().getPageSize();

		List<DashboardInstance> list = new LinkedList<DashboardInstance>();
		if (instancesModel != null) {
			for (int i=ofs; i<= ofs + event.getPageable().getPageSize() - 1 && i<instancesModel.getSize(); i++) {
				list.add(instancesModel.getElementAt(i));
			}
		}
		instancesModel = new ListModelList<DashboardInstance>(list);
		BindUtils.postNotifyChange(null, null, this, "titleLabel");
	}

	@NotifyChange("selectedCategoryForSorting")
	public void setSelectedCategoryForSorting(String selectedCategoryForSorting) {
		this.selectedCategoryForSorting = selectedCategoryForSorting;
		BindUtils.postNotifyChange(null, null, this, "selectedCategoryForSorting");
		String event="Ext.fireEvent('groupByDropdownChange', \""+ selectedCategoryForSorting + "\")";
		Clients.evalJavaScript(event);
	}

	@DependsOn({"selectedCategoryForSorting"})
	public ListModel<DashboardInstance> getInstancesModel() throws RestException{
		//updateUISorting();
		//sortInstances(m_dashboardInstances, DashboardInstanceFacade.DASHBOARD_INSTANCE_SEVERITY_COMPARATOR_DESC);
		Collections.sort(m_dashboardInstances , DashboardInstanceFacade.DASHBOARD_INSTANCE_SEVERITY_COMPARATOR_DESC);
		return new ListModelList<DashboardInstance>(m_dashboardInstances);
	}


	@Command("navigateToSingleInstance")
	public void onClickInstance(@BindingParam("product")Product product , @BindingParam("instanceID")Integer instanceID){
		Executions.sendRedirect(WebUtil.buildPathRelativeToProduct(product, "singleInstance"+"/"+instanceID));
	}
	
	@Command("launchDeskTopConsole")
	public void onClickInstance(@BindingParam("instanceID")Integer instanceID)throws IOException, RestException{
		Executions.sendRedirect("Idera://instance?instanceid="+instanceID);
	}

	@Command("navigateToInstanceAlertView")
	public void onClickInstanceAlert(@BindingParam("product")Product product , @BindingParam("instanceID")Integer instanceID, @BindingParam("category")String category){
		int categoryId = InstanceCategoryTab.ALERTS.getId();
		
		setTabForInstanceView(instanceID, categoryId);
		setCategoryVisibiltyForAlertTab(instanceID,category);
        
		/* redirecting to alerts page*/
		Executions.sendRedirect(WebUtil.buildPathRelativeToProduct(product, "singleInstance"+"/"+instanceID ));
	}
	
    private void setCategoryVisibiltyForAlertTab(Integer instanceID,
			String category) {
    	if(category.equals(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_AlERT)))
    		return;
    	  /* setting the alert category filter according to selected category */
        SingleInstanceAlertsPreferencesBean sessionBean = PreferencesUtil
				.getInstance().getSingleInstanceAlertsPreferenceInSession(
						instanceID);
        ListModelList<AlertCategoriesDetails> categoryOptions = sessionBean.getCategoryOptions();
        if(categoryOptions == null) {
        	categoryOptions = new ListModelList<>(AlertCategoriesEnum.getDefaultList());
        }
        
        for (AlertCategoriesDetails alertCategoriesDetails : categoryOptions) {
        	if(alertCategoriesDetails.getCategory().getCategoryName().equalsIgnoreCase(category)){
        		alertCategoriesDetails.setVisible(true);
        		continue;
        	}/*else if(alertCategoriesDetails.getCategory() == AlertCategoriesDetails.AlertCategoriesEnum.QUERIES  
        			&& category.equals(ELFunctions.getMessage(SQLdmI18NStrings.QUERY))){
        		alertCategoriesDetails.setVisible(true);
        		continue;
        	}*/
        	alertCategoriesDetails.setVisible(false);
		}
        sessionBean.setCategoryOptions(categoryOptions);
		PreferencesUtil.getInstance().setSingleInstanceAlertsPreferenceInSession(sessionBean);
		
	}

	private void setTabForInstanceView(Integer instanceID, int categoryId) {
    	   SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceID);
    	        pref.setSelectedCategory(categoryId);
    	        pref.setSelectedSubCategory(1);
    	        PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);
	}
	/*
	 * @Author ChaitanyaTanwar
	 * */
	@Command("navigateToSWAInstance")
	public void onClickSWAInstance(@BindingParam("instanceName")String instanceName , @BindingParam("swaID")Integer instanceID) throws RestException{
		try{
			int swaID = instanceID;
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

	@Command("openSingleInstance")
	public void onClickSelectedAlertInstance(@BindingParam("category") String category, @BindingParam("instanceid") Integer instanceId , @BindingParam("productName") String productName) {
		System.out.println("PRODUCT NAME  : " + productName);
		InstanceCategoryTab tab = InstanceCategoryTab.findByStatus(category.toLowerCase());
		int categoryId = tab == null ? 0 : tab.getId();
		SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
		pref.setSelectedCategory(categoryId);
		pref.setSelectedSubCategory(1);
		PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);

		//Executions.sendRedirect("~./sqldm/singleInstance/"+instanceId);
		Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance"+"/"+instanceId));
		//WebUtil.buildPathRelativeToCurrentProduct("singleInstance")+"/<id>"
	}

	@NotifyChange({"instancesModel", "instanceGroupModel"})
	@Command("searchInstances")
	public void searchInstances(){

		String filter = null;
		List<DashboardInstance> searchedInstances = null;

		if(searchText.length() > 0){
			searchedInstances = new LinkedList<>();
			for (DashboardInstance instance : selectedLeftNavigationFilteredInstances) {
				if (instance != null && instance.getOverview() != null
						&& instance.getOverview().getInstanceName() != null) {
					String instanceName = instance.getOverview()
							.getInstanceName();
					if (searchText.indexOf("%") == -1
							&& searchText.equalsIgnoreCase(instanceName)) {
						searchedInstances.add(instance);
					} else if (searchText.indexOf("%") != -1
							&& Utility.wildCardMatch(
									instanceName.toLowerCase(),
									searchText.toLowerCase())) {
						searchedInstances.add(instance);
					}
				}
			}

			filter = "Searched";
			// searchedInstances = searchedInstances;
		} else {
			filter = selectedFilterId;
			searchedInstances = selectedLeftNavigationFilteredInstances;
		}

		refreshInstances(filter, searchedInstances);
		EventQueue<Event> eq = EventQueues.lookup(SQLdmConstants.DASHBOARD_UPDATE_INSTANCES_EVENT_QUEUE, EventQueues.DESKTOP, true);
		if(eq != null) {
			Map<Integer, Object> map = new HashMap<Integer, Object>();
			final Desktop desktop = Executions.getCurrent().getDesktop();
			desktop.setAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_SEARCH, searchedInstances);
			for (DashboardInstance instance : searchedInstances) {
				map.put(instance.getOverview().getSQLServerId(), true);
			}

			PreferencesUtil.getInstance().setFilteredInstancesMap(map);
			eq.publish(new Event("onSearch", null, ""));
		}
	}

	@Command
	public void heatmapBtnClicked(@BindingParam("instanceid") Integer instanceId){
		Executions.sendRedirect(WebUtil.buildPathRelativeToCurrentProduct("singleInstance"+"/"+instanceId));
	}

	/*public List<DashboardInstance> getFilteredInstances() {
		return filteredInstances;
	}*/

	@NotifyChange("titleLabel, subTitleLabel")
	private void updateListTitle(String type,
			List<DashboardInstance> instanceList) {

		String title = "0 Instances";
		String tcriticalInstanceCountLabel = "0";
		String twarningInstanceCountLabel = "0";
		String tmaintenanceInstanceCountLabel = "0";
		String tokInstanceCountLabel = "0";

		if (instanceList != null) {
			title = new StringBuffer()
			.append(StringUtils.capitalize(type))
			.append((instanceList.size() == 0 || instanceList.size() > 1) ? " Instances ( "
					: " Instance ( ").append(instanceList.size())
					.append(" )").toString();
			int critical = 0;
			int warning = 0;
			int informational = 0;
			int ok = 0;
			for (DashboardInstance di : instanceList) {
				if(di.getServerStatus() !=null){
					if (di.getServerStatus().getMaxSeverity() != null) {
						switch((int)di.getServerStatus().getMaxSeverity()) {
						case 8:
							critical++;
							break;
						case 4:
							warning++;
							break;
						case 2:
							informational++;
							break;
						case 1:
							ok++;
							break;
						case 0:
							ok++; //Ashu: 0 comes when there is no alerts for that instance. Such instances should also be treated as ok..
							break;
						}						
					} else {
						critical++; //When maxseverity is not known, setting it to critical
					}
				}
			}
			tcriticalInstanceCountLabel = String.valueOf(critical);
			twarningInstanceCountLabel = String.valueOf(warning);
			tmaintenanceInstanceCountLabel = String.valueOf(informational);
			tokInstanceCountLabel = String.valueOf(ok);
		}
		titleLabel = title.toUpperCase();
		criticalInstanceCountLabel = tcriticalInstanceCountLabel;
		warningInstanceCountLabel = twarningInstanceCountLabel;
		maintenanceInstanceCountLabel = tmaintenanceInstanceCountLabel;
		okInstanceCountLabel = tokInstanceCountLabel;

		BindUtils.postNotifyChange(null, null, this, "titleLabel");
		BindUtils.postNotifyChange(null, null, this,
				"criticalInstanceCountLabel");
		BindUtils.postNotifyChange(null, null, this,
				"warningInstanceCountLabel");
		BindUtils.postNotifyChange(null, null, this,
				"maintenanceInstanceCountLabel");
		BindUtils.postNotifyChange(null, null, this, "okInstanceCountLabel");
	}

	public static enum AlertSeverity {
		OK(1, "OK"),
		MAINTENANCE(2, "MAINTENANCE"), // Todo = Is it informational or maintenance ? 
		WARNING(4, "WARNING"),
		CRITICAL(8, "CRITICAL");

		private int id;
		private String value;

		private AlertSeverity(int id, String value) {
			this.id = id;
			this.value = value;
		}

		public int getId() {
			return id;
		}

		public String getValue() {
			return value;
		}

		public static boolean isEqualsAS(String value, int id) {
			AlertSeverity as = getAlertSeverityEnum(value);
			if (as != null) {
				return as.getId() == id;
			}

			return false;
		}

		public static AlertSeverity getAlertSeverityEnum(String value) {
			for (AlertSeverity as : values()) {
				if (as.getValue().equalsIgnoreCase(value)) {
					return as;
				}
			}

			return null;
		}
	}

	@SuppressWarnings("unchecked")
	private void groupInstances(String type) {

		//	List<DashboardInstance> instancesList = new ArrayList<DashboardInstance>();

		List<DashboardInstanceWrapper> tInstancesForGroup = new LinkedList<DashboardInstanceWrapper>();
		//Fetching instances list from Session
		if(m_dashboardInstances == null){
			m_dashboardInstances = (LinkedList<DashboardInstance>) (Executions
					.getCurrent().getDesktop()
					.getAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_INSTANCES_LIST));
			//instancesList = m_dashboardInstances;
		}

		if(type.equalsIgnoreCase("SQLdmRepo"))
		{
			LinkedList<DashboardInstance> tempList =(LinkedList<DashboardInstance>) Executions
					.getCurrent().getDesktop()
					.getAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_INSTANCES_LIST);
			if(tempList != null)
				m_dashboardInstances = tempList;
		}

		for (DashboardInstance instance : m_dashboardInstances) {
			Double availableMemory = 0d;
			Double cpuPercentage = 0d;
			try {
				if (instance == null
						|| instance.getOverview() == null
						|| instance.getOverview().getOsMetricsStatistics() == null
						|| instance.getOverview().getOsMetricsStatistics()
						.getAvailableBytes() == null) {
					availableMemory = 0d;
				} else {
					availableMemory = (Math.round(Double.parseDouble(instance
							.getOverview().getOsMetricsStatistics()
							.getAvailableBytes()) / 1024 * 100.0)) / 100.0;
				}
			} catch (Exception e) {
				log.error(e.getMessage(), e);
			}
			try {
				if (instance == null
						|| instance.getOverview() == null
						|| instance.getOverview().getStatistics() == null
						|| instance.getOverview().getStatistics()
						.getCpuPercentage() == null) {
					cpuPercentage = 0d;
				} else {
					cpuPercentage = instance.getOverview().getStatistics()
							.getCpuPercentage();
				}
			} catch (Exception e) {
				log.error(e.getMessage(), e);
			}

			if (instance.getServerStatus() != null) {
				switch (type.toUpperCase()) {
				case "TAGS":
					// PreferencesUtil.getInstance().setDashboardPreferencesInSession(null,
					// null, null, DashboardGroupByInstances.TAGS, -1, -1);
					List<String> instanceTags = instance.getServerStatus()
					.getTags();
					/**
					 * @author Accolite
					 * fix for SQLDM-27448
					 * Date: 03.01.2017
					 * Reason: when tags are not available in serverStatus, fetching it from overview
					 */
					if (instanceTags == null)
						instanceTags = instance.getOverview().getTags();
					
					if (instanceTags != null && instanceTags.size() > 0) {
						for (String tag : instanceTags) {
							tInstancesForGroup
							.add(new DashboardInstanceWrapper(instance
									.getOverview().getSQLServerId(),
									instance.getOverview()
									.getDisplayName(),
									instance.getOverview()
									.getProductVersion(),
									instance.getOverview()
									.getSqlServiceStatus(),
									instance.getOverview()
									.getAgentServiceStatus(),
									instance.getOverview()
									.getDtcServiceStatus(),
									availableMemory, instance
									.getOverview()
									.getSystemProcesses()
									.get("blockedProcesses"),
									cpuPercentage, instance
									.getOverview()
									.getOsMetricsStatistics()
									.getProcessorQueueLength(),
									tag, instance.getServerStatus()
									.getMaxSeverity(), instance.getProduct(),
									instance.getOverview()
											.getDisplayName(),
											instance.getServerStatus().getHealthIndex(),
											instance.getAlertCategoryWiseMaxSeverity(),
											instance.getIsSWAInstance())
							
									); // @author Saumyadeep - Added displayName field
						}
					} else {
						tInstancesForGroup.add(new DashboardInstanceWrapper(
								instance.getOverview().getSQLServerId(),
								instance.getOverview().getDisplayName(),
								instance.getOverview().getProductVersion(),
								instance.getOverview().getSqlServiceStatus(),
								instance.getOverview().getAgentServiceStatus(),
								instance.getOverview().getDtcServiceStatus(),
								availableMemory, instance.getOverview()
								.getSystemProcesses()
								.get("blockedProcesses"),
								cpuPercentage, instance.getOverview()
								.getOsMetricsStatistics()
								.getProcessorQueueLength(),
								TAG_NOT_FOUND_LABEL, instance.getServerStatus()
								.getMaxSeverity() , instance.getProduct(),
								instance.getOverview()
										.getDisplayName(), instance.getServerStatus().getHealthIndex(),
										instance.getAlertCategoryWiseMaxSeverity(),
										instance.getIsSWAInstance())); // @author Saumyadeep - Added displayName field
					}
					break;
				case "SQLDMREPO":
					tInstancesForGroup.add(new DashboardInstanceWrapper(
							instance.getOverview().getSQLServerId(), instance
							.getOverview().getDisplayName(), instance
							.getOverview().getProductVersion(),
							instance.getOverview().getSqlServiceStatus(),
							instance.getOverview().getAgentServiceStatus(),
							instance.getOverview().getDtcServiceStatus(),
							availableMemory, instance.getOverview()
							.getSystemProcesses()
							.get("blockedProcesses"), cpuPercentage,
							instance.getOverview().getOsMetricsStatistics()
							.getProcessorQueueLength(), null, instance
							.getServerStatus().getMaxSeverity() , instance.getProduct(),
									instance.getOverview().getDisplayName(),
									instance.getServerStatus().getHealthIndex(),
									instance.getAlertCategoryWiseMaxSeverity(),
									instance.getIsSWAInstance())); // @author Saumyadeep - Added displayName field
					break;
				case "SEVERITY":
					// PreferencesUtil.getInstance().setDashboardPreferencesInSession(null,
					// null, null, DashboardGroupByInstances.SEVERITY, -1, -1);
					tInstancesForGroup.add(new DashboardInstanceWrapper(
							instance.getOverview().getSQLServerId(), instance
							.getOverview().getDisplayName(), instance
							.getOverview().getProductVersion(),
							instance.getOverview().getSqlServiceStatus(),
							instance.getOverview().getAgentServiceStatus(),
							instance.getOverview().getDtcServiceStatus(),
							availableMemory, instance.getOverview()
							.getSystemProcesses()
							.get("blockedProcesses"), cpuPercentage,
							instance.getOverview().getOsMetricsStatistics()
							.getProcessorQueueLength(), null, instance
							.getServerStatus().getMaxSeverity() , instance.getProduct(),
									instance.getOverview().getDisplayName(),
									instance.getServerStatus().getHealthIndex(),instance.getAlertCategoryWiseMaxSeverity(),
									instance.getIsSWAInstance())); // @author Saumyadeep - Added displayName field
				}
			} else {
				log.error("No Server Status received.");
			}
		}

		switch(type.toUpperCase()){
		case "TAGS" : 
			instanceGroupModel =  new InstanceGroupModel(tInstancesForGroup, new InstanceTagComparator(), true);
			break;
		case "SEVERITY" :
			instanceGroupModel =  new InstanceGroupModel(tInstancesForGroup, new InstanceSeverityComparator() , true);
			break;
		case "SQLDMREPO" :
			instanceGroupModel =  new InstanceGroupModel(tInstancesForGroup,new InstanceRepoComparator(), true);
			break;
		}

		//	instanceGroupModel =  new InstanceGroupModel(tInstancesForGroup, "TAGS".equalsIgnoreCase(type) ? new InstanceTagComparator() : new InstanceSeverityComparator() , true);
		//instancesList = m_dashboardInstances;
	}
	@SuppressWarnings("unchecked")
	private void filterInstances(FilterType filterType, String filterId) {
		Map<String, DashboardBean> map = null;
		try {
			switch (filterType) {
			case STATUS:
				map = (Map<String, DashboardBean>) (Executions.getCurrent()
						.getDesktop()
						.getAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_SEVERITIES));
				break;

			case TAG:
				map = (Map<String, DashboardBean>) Executions
				.getCurrent()
				.getDesktop()
				.getAttribute(
						SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_TAGS);
				break;
			}
			if (map == null || map.get(filterId) == null) {
				filterType.fieldLabel = "Status";
				filterId = "ALL";
				map = (Map<String, DashboardBean>) (Executions.getCurrent()
						.getDesktop()
						.getAttribute(SQLdmConstants.DASHBOARD_SCOPE_DASHBOARD_SEVERITIES));
			}
			List<DashboardInstance> tList = map.get(filterId).getInstances();
			setMaxSeverity(map.get(filterId).getMaxSeverity());
			if (tList != null) {
				// m_dashboardInstances = tList;
				selectedLeftNavigationFilteredInstances = new LinkedList<>(
						tList);
				DashboardPreferencesBean dsdb = PreferencesUtil.getInstance()
						.getDashboardPreferencesInSession();
				List<Integer> heatMapOpenInstances = null;
				if (dsdb != null) {
					heatMapOpenInstances = dsdb.getHeatMapOpenInstances();
				}
				for (DashboardInstance instance : tList) {
					if (heatMapOpenInstances != null) {
						if (heatMapOpenInstances.contains(instance
								.getOverview().getSQLServerId())) {
							instance.setIsInstanceOpenInHeatMap(true);
						} else {
							instance.setIsInstanceOpenInHeatMap(false);
						}
					}
				}
			} else {
				// m_dashboardInstances = new LinkedList<>();
				selectedLeftNavigationFilteredInstances = new LinkedList<>();
			}

			this.selectedFilterId = filterId;
			refreshInstances(filterId, selectedLeftNavigationFilteredInstances);
		} catch (Exception ex) {
			log.error(ex.getMessage(), ex);
		}
	}
	public String getDisplayMessageForInstancesList() {
		return displayMessageForInstancesList;
	}

	public void setDisplayMessageForInstancesList(
			String displayMessageForInstancesList) {
		this.displayMessageForInstancesList = displayMessageForInstancesList;
		BindUtils.postNotifyChange(null, null, this,
				"displayMessageForInstancesList");
	}

	@DependsOn("maxSeverity")
	public String getMaxSeverityImage() {
		String severityString = null;
		if (getIsInstancesLoaded()) {
			if (maxSeverity == SeverityCodeToStringEnum.CRITICAL.getId()) {
				severityString = SeverityCodeToStringEnum.CRITICAL.getLabel()
						.toLowerCase();
			} else if (maxSeverity == SeverityCodeToStringEnum.WARNING.getId()) {
				severityString = SeverityCodeToStringEnum.WARNING.getLabel()
						.toLowerCase();
			} else if (maxSeverity == SeverityCodeToStringEnum.INFORMATIONAL
					.getId()) {
				severityString = SeverityCodeToStringEnum.INFORMATIONAL
						.getLabel().toLowerCase();
			} else if (maxSeverity == SeverityCodeToStringEnum.OK.getId()) {
				severityString = SeverityCodeToStringEnum.OK.getLabel()
						.toLowerCase();
			}
		} else {
			severityString = SeverityCodeToStringEnum.LOADING.getLabel()
					.toLowerCase();
		}
		if (severityString != null) {
			return ELFunctions.getImageURLWithoutSize(severityString
					.concat("32x32"));
		}

		return null;
	}

	public Comparator getBlockedProcessAscComparator() {
		return blockedProcessAscComparator;
	}

	public Comparator getBlockedProcessDescComparator() {
		return blockedProcessDescComparator;
	}

	public Comparator getCpuActivityAscComparator() {
		return cpuActivityAscComparator;
	}

	public Comparator getCpuActivityDescComparator() {
		return cpuActivityDescComparator;
	}

	public Comparator getAvailMemAscComparator() {
		return availMemAscComparator;
	}

	public Comparator getAvailMemDescComparator() {
		return availMemDescComparator;
	}

	@Command("updateDashboardManually")
	public void updateDashboard (){
		/*try {
			reloadDashboardInstances(DashboardInstanceFacade.getDashboardInstances(), Executions.getCurrent().getDesktop());
		} catch (InstanceException e) {
			log.error(e);
		}*/
		eqForManualReload = EventQueues.lookup(
				DashboardConstants.DASHBOARD_QUEUE_NAME, EventQueues.SESSION,
				true);
		if (eqForManualReload != null) {
			eqForManualReload.publish(new Event(
					DashboardConstants.DASHBOARD_MANUAL_REFRESH_EVENT_NAME));
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

	/**
	 * pop up for column chooser
	 * 
	 * @param ref
	 * @param columnChooser
	 */
	@Command
	public void openDefaultColumnChooser(@BindingParam("ref") Component ref,
			@BindingParam("ref2") Columnchooser columnChooser) {
		columnChooser.open(ref, "after_end");//try after_pointer
	}

	/**
	 * Changes the visible and hidden list according to the columns chosen
	 * through column chooser
	 * 
	 * @param visibleColumns
	 *            - list of the visible columns
	 * @param hiddenColumns
	 *            - list of hidden columns
	 * 
	 */
	@Command
	@NotifyChange({ "defaultColumnList", "columnList", "instancesModel" }) 
	public void doColumnVisibilityChange(@BindingParam("visibleColumns") List<String> visibleColumns,
			@BindingParam("hiddenColumns") List<String> hiddenColumns) {
		List<String> visibleColClone = new ArrayList<>();
		List<String> hiddenColClone = new ArrayList<>();
		List<String> orderedColumns = getOrderedColumnList(false);
		for(int i = 0 ; i<orderedColumns.size() ; i++){
			String col = orderedColumns.get(i);
			if(visibleColumns.contains(col)){
				visibleColClone.add(col);
			} else if(instancesColumn!=null && instancesColumn.get(col).getDefaultFlag()){
				visibleColClone.add(col);
			}
			if(hiddenColumns.contains(col) && !instancesColumn.get(col).getDefaultFlag()){
				hiddenColClone.add(col);
			}
		}
		if(defaultColumnList!=null){
			defaultColumnList.clear();
			defaultColumnList = visibleColClone;
		}
		if(columnList!=null){
			columnList.clear();
			columnList = hiddenColClone;
		}
		//Order the column on Grid
		
		try {
			PreferencesUtil.getInstance().setDashboardPreferencesInSession(null, null, DashboardInstanceView.LIST, null, -1, -1, null, defaultColumnList, columnList);
			String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
			saveUserSettings(productInstanceName, PreferencesUtil.getInstance().getDashboardPreferencesInSession());
			BindUtils.postNotifyChange(null, null, this, "columnList");
			BindUtils.postNotifyChange(null, null, this, "defaultColumnList");
		} catch (CategoryException e) {
			e.printStackTrace();
		} catch (ParseException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	
		
		
	}
	public List<String> getColumnList() {
		return columnList;
	}

	public void setColumnList(List<String> columnList) {
		this.columnList = columnList;
	}
	
	public List<String> getDefaultColumnList() {
		return defaultColumnList;
	}

	public void setDefaultColumnList(List<String> defaultColumnList) {
		this.defaultColumnList = defaultColumnList;
	}

	private void addColumnsToList() {
	    instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_SWA) ,new InstancesColumns("swaStatus", "42px", false, "swaImage"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE), new InstancesColumns("InstanceName",  "120px", true, "name"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.HEALTH_INDEX), new InstancesColumns("healthIndex",  "75px", false, "healthIndex"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_AlERT), new InstancesColumns("MaxSeverity",  "75px", false, "alertImage"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_CPU), new InstancesColumns("Cpu",  "75px", false, "alertImage"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_MEMORY), new InstancesColumns("Memory",  "75px", false, "alertImage"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_IO), new InstancesColumns("IO",  "75px", false, "alertImage"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.FILTER_DATABASES), new InstancesColumns("Databases",  "75px", false, "alertImage"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_LOGS), new InstancesColumns("Logs",  "75px", false, "alertImage"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_QUERIES), new InstancesColumns("Queries",  "75px", false, "alertImage"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_SERVICES), new InstancesColumns("Services",  "75px", false, "alertImage"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_SESSIONS), new InstancesColumns("Sessions",  "75px", false, "alertImage"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_VIRTUALIZATION), new InstancesColumns("Virtualization",  "75px", false,"alertImage"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_OPERATIONAL), new InstancesColumns("Operational",  "75px", false, "alertImage"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_DASHBOARD_VERSION), new InstancesColumns("ProductVersion",  "70px", false, "versionlabel"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_DASHBOARD_STATUS), new InstancesColumns("status",  "70px", false, "statusLabel"));		
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.AGENT_STATUS), new InstancesColumns("agentServiceStatus",  "80px", false, "agentLabel"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.DTC_STATUS), new InstancesColumns("dtcServiceStatus",  "80px", false, "dtcLabel"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.AVAILABLE_MEMORY), new InstancesColumns("AvailableBytes",  "115px", false, "availLabel"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.BLOCKED_SESSIONS), new InstancesColumns("blockedProcesses",  "115px", false, "blockedLabel"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.CPU_ACTIVITY), new InstancesColumns("cpuPercentage",  "90px", false, "cpuActLabel"));
		instancesColumn.put(ELFunctions.getMessage(SQLdmI18NStrings.DISK_QUEUE_LIST), new InstancesColumns("diskQueueLength",  "115px", false, "diskQueueLabel"));
	}
	
	@NotifyChange
	@DependsOn("filter")
	public void setVisibleAndHiddenColumnsList() {
		DashboardPreferencesBean dsdb = PreferencesUtil.getInstance().getDashboardPreferencesInSession();
		if(dsdb != null){
			List<String> visible = dsdb.getInstanceListViewVisibleColumns();
			if(visible!=null && !(visible.isEmpty())){
				defaultColumnList = visible;
				columnList = dsdb.getInstanceListViewHiddenColumns();
			}
		}
		if(defaultColumnList.isEmpty()){
			List<String> hidden = new ArrayList<String>();
			defaultColumnList = getOrderedColumnList(true);
		}
		//columnList = hidden;
		BindUtils.postNotifyChange(null, null, this, "columnList");
		BindUtils.postNotifyChange(null, null, this, "defaultColumnList");
	}

	private List<String> getOrderedColumnList(boolean isInitialization) {
		dbOrder = new ArrayList<>();
		// Default
		dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_SWA));
		dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE));
		dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.HEALTH_INDEX));
		// Available
		dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_AlERT));
		dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_CPU));
		dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_MEMORY));
		dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_IO));
		dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.FILTER_DATABASES));
		dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_LOGS));
		dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_QUERIES));
		if(!isInitialization){
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_SERVICES));
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_SESSIONS));
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_VIRTUALIZATION));
			dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_OPERATIONAL));
		}
		else{
			columnList.add(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_SERVICES));
			columnList.add(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_SESSIONS));
			columnList.add(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_VIRTUALIZATION));
			columnList.add(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_OPERATIONAL));
		}
		dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_DASHBOARD_VERSION));
		dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_DASHBOARD_STATUS));
		dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AGENT_STATUS));
		dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.DTC_STATUS));
		dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.AVAILABLE_MEMORY));
		dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.BLOCKED_SESSIONS));
		dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.CPU_ACTIVITY));
		dbOrder.add(ELFunctions.getMessage(SQLdmI18NStrings.DISK_QUEUE_LIST));
		return dbOrder;
	}

	public HashMap<String, InstancesColumns> getInstancesColumn() {
		return instancesColumn;
	}

	public void setInstancesColumn(HashMap<String, InstancesColumns> instancesColumn) {
		this.instancesColumn = instancesColumn;
	}
	
	public Comparator<DashboardInstance> chooseAscComparator(
			String compareColumn) {
		if(compareColumn.equals(ELFunctions.getMessage(SQLdmI18NStrings.AVAILABLE_MEMORY)))
			return new DashboardInstanceFacade().DASHBOARD_INSTANCE_AVAILABLE_MEM_COMPARATOR_ASC;
		else if(compareColumn.equals(ELFunctions.getMessage(SQLdmI18NStrings.BLOCKED_PROCESSES)))
			return new DashboardInstanceFacade().DASHBOARD_INSTANCE_BLOCKED_PROCESSES_COMPARATOR_ASC;
		else if(compareColumn.equals(ELFunctions.getMessage(SQLdmI18NStrings.CPU_ACTIVITY)))
			return new DashboardInstanceFacade().DASHBOARD_INSTANCE_CPU_ACTIVITY_COMPARATOR_ASC;
		return new DashboardInstanceComparator(compareColumn,true);
	}

	public Comparator<DashboardInstance> chooseDescComparator(
			String compareColumn) {
		if(compareColumn.equals(ELFunctions.getMessage(SQLdmI18NStrings.AVAILABLE_MEMORY)))
			return new DashboardInstanceFacade().DASHBOARD_INSTANCE_AVAILABLE_MEM_COMPARATOR_DESC;
		else if(compareColumn.equals(ELFunctions.getMessage(SQLdmI18NStrings.BLOCKED_PROCESSES)))
			return new DashboardInstanceFacade().DASHBOARD_INSTANCE_BLOCKED_PROCESSES_COMPARATOR_DESC;
		else if(compareColumn.equals(ELFunctions.getMessage(SQLdmI18NStrings.CPU_ACTIVITY)))
			return new DashboardInstanceFacade().DASHBOARD_INSTANCE_CPU_ACTIVITY_COMPARATOR_DESC;
		return new DashboardInstanceComparator(compareColumn,false);
	}
	
	public Comparator<DashboardInstanceWrapper> chooseAscGroupComparator(
			String compareColumn) {
		if(compareColumn.equals(ELFunctions.getMessage(SQLdmI18NStrings.AVAILABLE_MEMORY)))
			return new DashboardInstanceFacade().DASHBOARD_INSTANCE_GROUP_AVAILABLE_MEM_COMPARATOR_ASC;
		else if(compareColumn.equals(ELFunctions.getMessage(SQLdmI18NStrings.BLOCKED_PROCESSES)))
			return new DashboardInstanceFacade().DASHBOARD_INSTANCE_GROUP_BLOCKED_PROCESSES_COMPARATOR_ASC;
		else if(compareColumn.equals(ELFunctions.getMessage(SQLdmI18NStrings.CPU_ACTIVITY)))
			return new DashboardInstanceFacade().DASHBOARD_INSTANCE_GROUP_CPU_ACTIVITY_COMPARATOR_ASC;
		return new DashboardInstanceWrapperComparator(compareColumn,true);
	}

	public Comparator<DashboardInstanceWrapper> chooseDescGroupComparator(
			String compareColumn) {
		if(compareColumn.equals(ELFunctions.getMessage(SQLdmI18NStrings.AVAILABLE_MEMORY)))
			return new DashboardInstanceFacade().DASHBOARD_INSTANCE_GROUP_AVAILABLE_MEM_COMPARATOR_DESC;
		else if(compareColumn.equals(ELFunctions.getMessage(SQLdmI18NStrings.BLOCKED_PROCESSES)))
			return new DashboardInstanceFacade().DASHBOARD_INSTANCE_GROUP_BLOCKED_PROCESSES_COMPARATOR_DESC;
		else if(compareColumn.equals(ELFunctions.getMessage(SQLdmI18NStrings.CPU_ACTIVITY)))
			return new DashboardInstanceFacade().DASHBOARD_INSTANCE_GROUP_CPU_ACTIVITY_COMPARATOR_DESC;
		return new DashboardInstanceWrapperComparator(compareColumn,false);
	}
	public String getHealthIndexBgColor(DashboardInstance instance) {

		String bgColor;
		Double healthIndex = instance.getServerStatus().getHealthIndex();
		if((SeverityCodeToStringEnum.INFORMATIONAL.getStyleName()).equals(instance.getServerStatus().getSeverityString())){
			bgColor = "background-color: #94979f";
		} else {
			bgColor = getBgColorByHealthIndex(healthIndex);
		}
		return  bgColor;
	}
	public String getBgColorByHealthIndex(Double healthIndex){
		String bgColor;
		if ((healthIndex/100) >1) 
			bgColor = "#94979f";
		else
			bgColor = ELFunctions.getColorGradientAtIndex(healthIndex);
		return "background-color: " + bgColor;
	}
	private void saveUserSettings(String productInstanceName, DashboardPreferencesBean dsdb) throws CategoryException, ParseException{
		JSONArray settings = new JSONArray();
		JSONObject elementSetting = new JSONObject();
		String concatStr="";
		if(dsdb != null){
			List<String> visible = dsdb.getInstanceListViewVisibleColumns();
			//converting the List of Visible columns into Comma separated Sting value
			for(int i=0;i<visible.size();i++){
				if(i==visible.size()-1){
					concatStr+=visible.get(i);
				}else{
					concatStr+=visible.get(i)+",";
				}
				
			}
			if(visible!=null && !(visible.isEmpty())){
				elementSetting.put("Key", "Grid_View_Columns_User_Preference");
				elementSetting.put("Value", concatStr);
			}
			settings.add(elementSetting);
		}
		UserSettingFacade.saveUserSettings(productInstanceName,settings);
	}
}

class InstanceRepoComparator implements
Comparator<DashboardInstanceWrapper>,
GroupComparator<DashboardInstanceWrapper>, Serializable {
	private static final long serialVersionUID = 1L;

	public int compare(DashboardInstanceWrapper o1, DashboardInstanceWrapper o2) {
		return o2.getProduct().getInstanceName().compareTo(o1.getProduct().getInstanceName());
	}

	public int compareGroup(DashboardInstanceWrapper o1,
			DashboardInstanceWrapper o2) {
		return o2.getProduct().getInstanceName().compareTo(o1.getProduct().getInstanceName());
	}
}


class InstanceSeverityComparator implements
Comparator<DashboardInstanceWrapper>,
GroupComparator<DashboardInstanceWrapper>, Serializable {
	private static final long serialVersionUID = 1L;

	public int compare(DashboardInstanceWrapper o1, DashboardInstanceWrapper o2) {
		return o2.getSeverity().compareTo(o1.getSeverity());
	}

	public int compareGroup(DashboardInstanceWrapper o1,
			DashboardInstanceWrapper o2) {
		return o2.getSeverity().compareTo(o1.getSeverity());
	}
}

class InstanceTagComparator implements Comparator<DashboardInstanceWrapper>,
GroupComparator<DashboardInstanceWrapper>, Serializable {
	private static final long serialVersionUID = 1L;

	public int compare(DashboardInstanceWrapper o1, DashboardInstanceWrapper o2) {
		return o1.getTag().compareTo(o2.getTag());
	}

	public int compareGroup(DashboardInstanceWrapper o1,
			DashboardInstanceWrapper o2) {
		if (o1.getTag() == null && o2.getTag() == null) {
			return 0;
		} else if (o1.getTag() == null) {
			return -1;
		} else if (o2.getTag() == null) {
			return 1;
		}
		return o1.getTag().compareTo(o2.getTag());
	}

	
}
