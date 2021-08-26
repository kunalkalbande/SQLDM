package com.idera.sqldm.ui.alerts;

import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Caption;
import org.zkoss.zul.Groupbox;
import org.zkoss.zul.Messagebox;
import org.zkoss.zul.Radio;
import org.zkoss.zul.Radiogroup;
import org.zkoss.zul.Window;

import com.idera.sqldm.data.alerts.AlertException;
import com.idera.sqldm.server.web.session.SessionUtil;
import com.idera.sqldm.ui.alerts.AlertsGroupingModel.GroupBy;
import com.idera.sqldm.ui.preferences.AlertsPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;

public class AlertsViewModel extends SelectorComposer<Window> {

	static final long serialVersionUID = 1L;

	@Wire private Radiogroup radioGroups;
	@Wire private Groupbox byStatusBox;
	@Wire private Caption currentViewCaption;
	private String OPEN_TRUE ="open-true";
	private String OPEN_FALSE = "open-false";
	private GroupBy grpBy = GroupBy.ACTIVE;
	
	@Override
	public void doAfterCompose(Window comp) throws Exception {
        super.doAfterCompose(comp);
        AlertsPreferencesBean sessionBean = PreferencesUtil.getInstance().getAlertsPreferencesInSession();
		if(sessionBean.getSelectedGroupBy() != null){
			//update selected filters if available in session.
			grpBy = sessionBean.getSelectedGroupBy();
			radioGroups.setSelectedIndex(grpBy.getIndex());
		}
		EventQueue<Event> eq1 = EventQueues.lookup("changeFilter", EventQueues.DESKTOP, true);
        eq1.subscribe(new EventListener<Event>() {
			public void onEvent(Event event) throws Exception {
				SessionUtil.getSecurityContext();
            	AlertFilter filter = (AlertFilter)event.getData();
            	if(filter.getInstanceId()!=null){
            	//If active radio button NOT selected OR filter value is not equal to ALL then set current view to Custom
            	if((!filter.isActive()&&radioGroups.getSelectedIndex()!=GroupBy.PRODUCT.getIndex()) || filter.getInstanceId() != -1 || 
            			filter.getMetricId() != -1 || filter.getSeverity() != -1 || filter.getProductId() != -1 ){
            		grpBy = GroupBy.CUSTOM;
            		radioGroups.setSelectedIndex(grpBy.getIndex());
        		}
            	}
            	else{
            		Messagebox.show("Connection to server is not established.");
            	}
     
            }
        });
    }
	
	@Listen("onCheck = #radioGroups")
    public void changeAlertGroup() throws AlertException {
        Radio selectedItem = radioGroups.getSelectedItem();
        if("activeAlertsGrp".equals(selectedItem.getId())){
        	grpBy = GroupBy.ACTIVE;
        }
        else if("severityGrp".equals(selectedItem.getId())){
        	grpBy = GroupBy.SEVERITY;
        }
        else if("serversGrp".equals(selectedItem.getId())){
        	grpBy = GroupBy.INSTANCE;
        }
        else if("metricGrp".equals(selectedItem.getId())){
        	grpBy = GroupBy.METRIC;
        }
        else if("categoryGrp".equals(selectedItem.getId())){
        	grpBy = GroupBy.CATEGORY;
        }
        else if("custom".equals(selectedItem.getId())){
        	grpBy = GroupBy.CUSTOM;
        }
        else if ("sqldmRepo".equals(selectedItem.getId())){
        	grpBy = GroupBy.PRODUCT;
        }
        	
        EventQueue<Event> eq = EventQueues.lookup("changeGroup", EventQueues.DESKTOP, true);
        if(eq != null){
        	eq.publish(new Event("onCheck", radioGroups, grpBy));
        }
    }
	@Listen("onOpen = #byStatusBox")
	public void onOpenTagsGroupBox(Event evt) {
		Groupbox tmp = (Groupbox) evt.getTarget();
		currentViewCaption.setSclass(tmp.isOpen() ? OPEN_TRUE : OPEN_FALSE);
	}
}
