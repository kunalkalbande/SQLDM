package com.idera.sqldm.ui.customDashboard;

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
import org.zkoss.zul.Checkbox;
import org.zkoss.zul.Textbox;
import org.zkoss.zul.Window;

import com.idera.common.rest.RestException;
import com.idera.sqldm.data.customdashboard.CustomDashboard;
import com.idera.sqldm.data.customdashboard.CustomDashboardFacade;
import com.idera.sqldm.server.web.session.UserSessionSettings;
import com.idera.sqldm.utils.Utility;

public class CreateDashboardComposer extends SelectorComposer<Window>{
	
	private static final long serialVersionUID = 1L;
	private String productInstanceName;
	@Wire private Textbox dashboardNametb;
	@Wire private Checkbox isDefaultcb;
	
	@Override
	public void doAfterCompose(Window comp) throws Exception {
		super.doAfterCompose(comp);
		
		productInstanceName = Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");

	}

	@Listen("onClick = #cancelDBbtn")
	public void closeTheWindow(){
		getSelf().detach();
	}
	
	@Listen("onClick = #saveDBBtn")
	public void saveDashboard() throws RestException{
		clearMessage();
		validateInput();
		CustomDashboard customDashboard = null;
		Session session = Sessions.getCurrent();
		com.idera.server.web.session.UserSessionSettings settings = (com.idera.server.web.session.UserSessionSettings)session.getAttribute(UserSessionSettings.USER_SESSION_SETTINGS_PROPERTY);
		if(settings.getUser() != null ) {
		
		try {
			customDashboard = CustomDashboardFacade.createCustomDashboard(productInstanceName, dashboardNametb.getValue(), isDefaultcb.isChecked(), settings.getUser().getSID());
		} catch (RestException e) {
			e.printStackTrace();
		}
		}
		
		if(customDashboard == null) {
			throw new WrongValueException(dashboardNametb, "Duplicate customdashboard name is not allowed. Please retry.");
		}
		
		EventQueue<Event> eq = EventQueues.lookup("customdashboard", EventQueues.SESSION, true);
		
		if(eq != null){
			eq.publish(new Event("onCreateCustomDashboard", null, customDashboard));
		}
		getSelf().detach();
	}
	
	private void validateInput() {
		if(dashboardNametb.getValue() == null || dashboardNametb.getValue().trim().equals("")) {
			throw new WrongValueException(dashboardNametb, "Please enter customdashboard name.");
		}
	}
	
	private void clearMessage() {
		dashboardNametb.clearErrorMessage();
	}
}
