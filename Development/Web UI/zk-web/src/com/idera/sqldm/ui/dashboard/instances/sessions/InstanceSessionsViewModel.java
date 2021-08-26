package com.idera.sqldm.ui.dashboard.instances.sessions;

import java.util.Date;
import java.util.concurrent.TimeUnit;

import org.apache.log4j.Logger;
import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.BindingParam;
import org.zkoss.bind.annotation.Command;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.EventQueue;
import org.zkoss.zk.ui.event.EventQueues;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Include;
import org.zkoss.zul.Tab;
import org.zkoss.zul.Toolbar;

import com.idera.sqldm.data.InstanceDetailsSessionFacade;
import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.ui.dashboard.instances.InstanceCategoryTab;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm.ui.dashboard.instances.SubCategoryViewModel;
import com.idera.sqldm.ui.preferences.HistoryPanelPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.ui.preferences.SingleInstancePreferencesBean;
import com.idera.sqldm.utils.SQLdmConstants;
import com.idera.sqldm.utils.Utility;

public class InstanceSessionsViewModel extends SubCategoryViewModel{

	private Logger log = Logger.getLogger(InstanceSessionsViewModel.class);
	
	@Wire private Toolbar sessionsTb;
	@Wire private Include contentView;

	private boolean historyEvent = false;

	@AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view){
		super.afterCompose(view);
		EventQueue<Event> eq1 = EventQueues.lookup("historyChange1",
				EventQueues.DESKTOP, true);
		eq1.subscribe(new EventListener<Event>() {

			@Override
			public void onEvent(Event event) throws Exception {
				refreshGraphs();
			}
		});
		EventQueue<Event> eq2 = EventQueues.lookup("updateSessionsTab",
				EventQueues.DESKTOP, true);
		eq2.subscribe(new EventListener<Event>() {

			@Override
			public void onEvent(Event event) throws Exception {
				refreshChart();
			}
		});
	}
	
	@Init
    public void init(@ContextParam(ContextType.VIEW) Component view){
		super.init(view);
	}
	
	protected void refreshChart() throws CategoryException {
    	if(historyEvent  ){
    		historyEvent  = false;
    		updateWithRange();
    	}
	}

	protected void refreshGraphs() {
    	historyEvent = true;
	}
	
	@Override
	public boolean haveSubTab(){
		return true;
	}
	
	@Override
	public Include getContentView() {
		return contentView;
	}

	@Override
	public Toolbar getToolbar() {
		return sessionsTb;
	}

	@Override
	public InstanceCategoryTab getInstanceCategoryTab() {
		return InstanceCategoryTab.SESSIONS;
	}

	@Override
	public InstanceSubCategoriesTab getInstanceSubCategoryTab() {
		return InstanceSubCategoriesTab.SESSIONS_SUMMARY;
	}
	
	@Override
	public String getModelName() {
		return SQLdmConstants.DASHBOARD_SCOPE_SINGLE_INSTANCE_SESSION;
	}
	
	@Override
	public Object getModelNameData() throws Exception {
		String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
		return InstanceDetailsSessionFacade.getInstanceDetailsSession(productInstanceName,getInstanceId());	
	}
	
	public void updateWithRange() {
		try {
			String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
			HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
			if(pref.getScaleCombobox().getValue().equals("Custom")){
				Date startTime = pref.getFromDateTime();
				Date endTime = pref.getToDateTime();
				long duration  = endTime.getTime() - startTime.getTime();
				long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
				Executions.getCurrent().getDesktop().setAttribute(
						SQLdmConstants.DASHBOARD_SCOPE_SINGLE_INSTANCE_SESSION_CHART, 
						InstanceDetailsSessionFacade.getSessionChartDetail(productInstanceName,getInstanceId(),numHistoryMinutes,pref.getToDateTime()));
			}
			else{
				Executions.getCurrent().getDesktop().setAttribute(
						SQLdmConstants.DASHBOARD_SCOPE_SINGLE_INSTANCE_SESSION_CHART, 
						InstanceDetailsSessionFacade.getSessionChartDetail(productInstanceName,getInstanceId(),pref.getNumHistoryMinutes()));
			}
			updateCharts();
	    } catch (Exception ce) {
        	Executions.getCurrent().getDesktop().removeAttribute("resources");
        	log.error(ce);
        }
	}
	
	
	
	@Command("drillDownUpdate")
	public void graphDrillDown(@BindingParam("fromDate") String fromDate,@BindingParam("fromTime") String fromTime){
		
	}
	
	public void updateCharts(){
		EventQueue<Event> eq = EventQueues.lookup("updateSessionChart",
				EventQueues.DESKTOP, false);
		if (eq != null) {
			eq.publish(new Event("update", null, null));
		}
	}
	
	public void getModelData() {
		try {
			String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
			Executions.getCurrent().getDesktop().setAttribute(getModelName(), getModelNameData());
			HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
			if(pref.getScaleCombobox().getValue().equals("Custom")){
				/*Executions.getCurrent().getDesktop().setAttribute(
						SQLdmConstants.DASHBOARD_SCOPE_SINGLE_INSTANCE_SESSION_CHART, 
						InstanceDetailsSessionFacade.getSessionChartDetail(productInstanceName,getInstanceId(),30));*/
				Date startTime = pref.getFromDateTime();
				Date endTime = pref.getToDateTime();
				long duration  = endTime.getTime() - startTime.getTime();
				long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
				Executions.getCurrent().getDesktop().setAttribute(
						SQLdmConstants.DASHBOARD_SCOPE_SINGLE_INSTANCE_SESSION_CHART, 
						InstanceDetailsSessionFacade.getSessionChartDetail(productInstanceName,getInstanceId(),numHistoryMinutes,pref.getToDateTime()));
			}
			else{
				Executions.getCurrent().getDesktop().setAttribute(
						SQLdmConstants.DASHBOARD_SCOPE_SINGLE_INSTANCE_SESSION_CHART, 
						InstanceDetailsSessionFacade.getSessionChartDetail(productInstanceName,getInstanceId(),pref.getNumHistoryMinutes()));
			}
	    } catch (Exception ce) {
        	Executions.getCurrent().getDesktop().removeAttribute("resources");
        	log.error(ce);
        }
	}

}
