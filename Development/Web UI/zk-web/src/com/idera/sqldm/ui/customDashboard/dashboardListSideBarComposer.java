package com.idera.sqldm.ui.customDashboard;

import java.util.ArrayList;
import java.util.List;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindUtils;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Session;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zul.ListModel;
import org.zkoss.zul.ListModelList;
import org.zkoss.zul.Window;

import com.idera.common.rest.RestException;
import com.idera.sqldm.data.customdashboard.CustomDashboard;
import com.idera.sqldm.data.customdashboard.CustomDashboardFacade;
import com.idera.sqldm.server.web.session.UserSessionSettings;
import com.idera.sqldm.utils.Utility;


public class dashboardListSideBarComposer {

	private final Logger log = Logger.getLogger(dashboardListSideBarComposer.class);
	private ListModel<CustomDashboard> dashboardListModel = new ListModelList<>();
	private List<CustomDashboard> customdashboards = new ArrayList<CustomDashboard>();
	
	@Init
	public void init() throws RestException {
		
		refreshDashboardModel();
		
		EventQueue<Event> eqForCustomDashboard = EventQueues.lookup(
				"customdashboard", EventQueues.SESSION,
				true);
		eqForCustomDashboard.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				if ("onCreateCustomDashboard".equalsIgnoreCase(event.getName())) {
					refreshDashboardModel();
				}
				if ("onRemoveCustomDashboard".equalsIgnoreCase(event.getName())) {
					refreshDashboardModel();
					displaySelectedDashboard(selectDefaultDashboard());
				}
				if ("onUpdateCustomDashboard".equalsIgnoreCase(event.getName())) {
					updateDashboard((CustomDashboard)event.getData());
				}
				if("onCopyCustomDashboard".equalsIgnoreCase(event.getName())) {
					refreshDashboardModel();
				}
			}
		});
		
		eqForCustomDashboard.publish(new Event("onDashboardLinkClick", null, selectDefaultDashboard()));
	}
	
	public void updateDashboard(CustomDashboard db) {
		if(db == null) {
			return;
		}
		for(CustomDashboard tempDb : customdashboards) {
			if(tempDb.equals(db)) {
				tempDb.setCustomDashboardName(db.getCustomDashboardName());
				tempDb.setIsDefaultOnUI(db.getIsDefaultOnUI());
				tempDb.setTags(db.getTags());
			}
		}
		setDashboardListModel(customdashboards);
	}
	
	public CustomDashboard selectDefaultDashboard() {
		if(customdashboards != null && customdashboards.size() > 0) {
			for(CustomDashboard db:customdashboards) {
				if(db.getIsDefaultOnUI())
					return db;
			}
		}
		
		return new CustomDashboard();
	}
	
	public void refreshDashboardModel() {
		String productInstanceName = Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
		Session session = Sessions.getCurrent();
		com.idera.server.web.session.UserSessionSettings settings = (com.idera.server.web.session.UserSessionSettings)session.getAttribute(UserSessionSettings.USER_SESSION_SETTINGS_PROPERTY);
		if(settings.getUser() != null ) {
			try {
				customdashboards = CustomDashboardFacade.getAllCustomDashboards(productInstanceName, settings.getUser().getSID());
			} catch (RestException e) {
				log.error("error while getting customdashboards ...");
				e.printStackTrace();
			}
		}
		
		if( customdashboards!= null) {
			setDashboardListModel(customdashboards);
		}
	}
	
	public void setDashboardListModel(List<CustomDashboard> model){
		if(model == null)
			return;
		this.dashboardListModel = new ListModelList<>(model);
		BindUtils.postNotifyChange(null, null, this, "dashboardListModel");
	}
	
	public ListModel<CustomDashboard> getDashboardListModel(){
		return dashboardListModel;
	}
	
	@Command
	public void displaySelectedDashboard(@BindingParam("dashBoardname") CustomDashboard selectedDashboard){
		EventQueue<Event> eq = EventQueues.lookup("customdashboard", EventQueues.SESSION, true);
		
		if(eq != null){
			eq.publish(new Event("onDashboardLinkClick", null, selectedDashboard));
		}
	}
	
	@Command
	public void createDashboard() {
		Window window = (Window)Executions.createComponents(
                "~./sqldm/com/idera/sqldm/ui/customDashboard/createDashboard.zul",null,null);
        window.doModal();
	}
	
}
