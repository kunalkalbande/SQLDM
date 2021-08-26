package com.idera.sqldm.ui.dashboard.instances;

import java.util.Calendar;
import java.util.Date;

import org.apache.log4j.Logger;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.Selectors;
import org.zkoss.zul.Button;
import org.zkoss.zul.Include;
import org.zkoss.zul.Toolbar;

import com.idera.common.Utility;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstancePreferencesBean;
import com.idera.sqldm.utils.SQLdmConstants;

public abstract class SubCategoryViewModel {


	private Logger log = Logger.getLogger(SubCategoryViewModel.class);
	protected int instanceId = -1;
	
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view){
        Selectors.wireComponents(view, this, false);
        Integer subTabId = Utility.getIntUrlParameter(Executions.getCurrent().getParameterMap(), "subtab");
        Integer tabId = Utility.getIntUrlParameter(Executions.getCurrent().getParameterMap(), "tab");
        if (tabId == null) {
			SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
			tabId = pref.getSelectedCategory();
	    }
        if(haveSubTab()){
        	 if(subTabId == null || subTabId == 0){
             	SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
             	subTabId = pref.getSelectedSubCategory();
             }
             if(subTabId == null || subTabId == 0 || tabId == null || tabId != getInstanceCategoryTab().getTabId()) {
             	subTabId = getInstanceSubCategoryTab().getSubTabId();
             }
             toolbarBtnClicked(null, getInstanceCategoryTab().getTabId(), subTabId);
        }
        EventQueue<Event> eqTabChanged = EventQueues.lookup("tabChanged",
        		EventQueues.DESKTOP, true);
        eqTabChanged.subscribe(new EventListener<Event>() {

        	@Override
        	public void onEvent(Event event) throws Exception {
        		if(event.getName().equals("tabSelected")){
        			SingleInstancePreferencesBean pref = (SingleInstancePreferencesBean) event.getData();
        			toolbarBtnClicked(event,pref.getSelectedCategory(),pref.getSelectedSubCategory());
        		}
        	}
        });
    }
    
    public void init(@ContextParam(ContextType.VIEW) Component view){
        Integer instanceIdParameter = Utility.getIntUrlParameter(Executions.getCurrent().getParameterMap(), "id");
		if (instanceIdParameter != null) {
			instanceId = instanceIdParameter;
		}else{
			//fallback
			Object param = Executions.getCurrent().getDesktop().getAttribute("instanceId");
			if(param != null){
				instanceId = (Integer) param;
			}
		}
        
        getModelData();
    }
	
	@Command
	public void toolbarBtnClicked(Event evt, @BindingParam("tabId") Integer newTabId, @BindingParam("subTabId") Integer newSubTabId){
		InstanceSubCategoriesTab isct = InstanceSubCategoriesTab.findById(newTabId, newSubTabId);
        if(isct != null && getToolbar() !=null) {
    		for(Component comp : getToolbar().getChildren()){
    			Button btn = (Button) comp;
    			if(btn.getId().replaceAll(" ", "").equalsIgnoreCase("button".concat(newSubTabId.toString()))) {
    				btn.setClass(SQLdmConstants.SINGLE_INSTANCE_SUB_TAB_SELECT_CLASS);
    			} else {
    				btn.setClass(SQLdmConstants.SINGLE_INSTANCE_SUB_TAB_DESELECT_CLASS);
    			}
    		}
    		getContentView().setSrc(isct.getUrl());
        }
        SingleInstancePreferencesBean pref = PreferencesUtil.getInstance().getSingleInstancePreferencesInSession(instanceId);
        pref.setSelectedCategory(newTabId);
        pref.setSelectedSubCategory(newSubTabId);
        PreferencesUtil.getInstance().setSingleInstancePreferencesInSession(pref);
	}
	
	public void getModelData() {
		try {
			Executions.getCurrent().getDesktop().setAttribute(getModelName(), getModelNameData());
        } catch (Exception ce) {
        	Executions.getCurrent().getDesktop().removeAttribute("resources");
        	log.error(ce);
        }
	}
	
	public Date getStartDate() {
		Calendar startTime = Calendar.getInstance();
		startTime.setTimeInMillis(startTime.getTimeInMillis());
		startTime.add(Calendar.MONTH, -20);
		
		return startTime.getTime();
	}
	
	public Date getEndDate() {
		return Calendar.getInstance().getTime();
	}
	
	public Integer getInstanceId() {
		Integer instanceId = null;
		Integer instanceIdParameter = Utility.getIntUrlParameter(Executions.getCurrent().getParameterMap(), "id");
		if (instanceIdParameter != null) {
			instanceId = instanceIdParameter;
		} else {
			Object param = Executions.getCurrent().getDesktop().getAttribute("instanceId");
			if(param != null) {
				instanceId = (Integer) param;
			}
		}
		return instanceId;
	}
	public abstract boolean haveSubTab();
	public abstract Include getContentView();
	public abstract Toolbar getToolbar();
	
	public abstract InstanceCategoryTab getInstanceCategoryTab();
	public abstract InstanceSubCategoriesTab getInstanceSubCategoryTab();
	
	public abstract String getModelName();
	public abstract Object getModelNameData() throws Exception;
}
