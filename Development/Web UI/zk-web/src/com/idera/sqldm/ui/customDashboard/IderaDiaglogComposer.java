package com.idera.sqldm.ui.customDashboard;

import org.zkoss.zhtml.Button;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Label;
import org.zkoss.zul.Window;

import com.idera.sqldm.data.customdashboard.CustomDashboard;


public class IderaDiaglogComposer extends SelectorComposer<Component>{
	@Wire private Button okbutton;
	
	@Wire private Button cancelbutton;
	
	@Wire private Label confirmationMessage;
	//@Wire private Label confirmationMessage2;
	//@Wire private Label confirmationMessage3;
	
	CustomDashboardWidgetAreaComposer customDashboardWidgetAreaComposer;
	private CustomDashboard customDashboard;
	
	@Override
	public void doAfterCompose(Component comp) throws Exception {
		super.doAfterCompose(comp);
		//confirmationMessage1.setValue("Dashboard ");
		String str="";
		if(Executions.getCurrent().getArg().get("obj1")!=null)
			 str = "\""+Executions.getCurrent().getArg().get("obj1").toString()+"\" ";
		
		confirmationMessage.setValue("Dashboard "+str+"will be removed permanently. Do you want to continue ?");
		//confirmationMessage3.setValue("will be removed permanently. Do you want to continue ?");
	}
	
	@Listen("onClick = #okbutton")
	public void removeCustomDashboard() {  
		EventQueue<Event> eq = EventQueues.lookup("deleteDashboard", EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("removeCustom"));
		}
		cancelremoveCustomDashboard();
		}
	
	@Listen("onClick = #cancelbutton")
	public void cancelremoveCustomDashboard() {  
		if(confirmationMessage.getParent().getParent().getParent()!=null){
		Window win=(Window)confirmationMessage.getParent().getParent().getParent();
		win.detach();
		}
		}

}
