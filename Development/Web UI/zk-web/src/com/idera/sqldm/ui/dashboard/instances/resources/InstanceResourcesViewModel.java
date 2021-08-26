package com.idera.sqldm.ui.dashboard.instances.resources;

import java.util.Date;
import java.util.List;
import java.util.concurrent.TimeUnit;

import org.zkoss.bind.annotation.AfterCompose;
import org.zkoss.bind.annotation.ContextParam;
import org.zkoss.bind.annotation.ContextType;
import org.zkoss.bind.annotation.Init;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Include;
import org.zkoss.zul.Toolbar;

import com.idera.sqldm.data.ResourcesFacade;
import com.idera.sqldm.data.category.resources.ResourceCategory;
import com.idera.sqldm.ui.dashboard.instances.InstanceCategoryTab;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm.ui.dashboard.instances.SubCategoryViewModel;
import com.idera.sqldm.ui.preferences.HistoryPanelPreferencesBean;
import com.idera.sqldm.ui.preferences.PreferencesUtil;
import com.idera.sqldm.utils.SQLdmConstants;
import com.idera.sqldm.utils.Utility;

public class InstanceResourcesViewModel extends SubCategoryViewModel {
	
	//private Logger log = Logger.getLogger(InstanceResourcesViewModel.class);
	@Wire private Include contentView;
	@Wire private Toolbar resourcesTb;
	
	@AfterCompose
    public void afterCompose(@ContextParam(ContextType.VIEW) Component view){
		super.afterCompose(view);
	}
	
	@Init
    public void init(@ContextParam(ContextType.VIEW) Component view){
		super.init(view);
	}
	

	@Override
	public Include getContentView() {
		return contentView;
	}

	@Override
	public Toolbar getToolbar() {
		return resourcesTb;
	}
	@Override
	public boolean haveSubTab(){
		return true;
	}
	
	@Override
	public InstanceCategoryTab getInstanceCategoryTab() {
		return InstanceCategoryTab.RESOURCES;
	}

	@Override
	public InstanceSubCategoriesTab getInstanceSubCategoryTab() {
		return InstanceSubCategoriesTab.RESOURCES_CPUVIEW;
	}
	
	@Override
	public String getModelName() {
		return SQLdmConstants.DASHBOARD_SCOPE_SINGLE_INSTANCE_RESOURCE;
	}
	
	@Override
	public Object getModelNameData() throws Exception {
    	String productInstanceName=Utility.getUrlParameter(Executions.getCurrent().getParameterMap(), "instance");
    	HistoryPanelPreferencesBean pref = PreferencesUtil.getInstance().getHistoryPanelPreferencesInSession();
    	List<ResourceCategory> resourceCategoryList;
		if(pref.getScaleCombobox().getValue().equals("Custom")){
			Date startTime = pref.getFromDateTime();
			Date endTime = pref.getToDateTime();
			long duration  = endTime.getTime() - startTime.getTime();
			long numHistoryMinutes = TimeUnit.MILLISECONDS.toMinutes(duration);
        	resourceCategoryList = ResourcesFacade.getResources(productInstanceName,instanceId, numHistoryMinutes,pref.getToDateTime());
        }
        else{
        	resourceCategoryList = ResourcesFacade.getResources(productInstanceName,instanceId, pref.getNumHistoryMinutes());
        }
		return resourceCategoryList;
	}
}
