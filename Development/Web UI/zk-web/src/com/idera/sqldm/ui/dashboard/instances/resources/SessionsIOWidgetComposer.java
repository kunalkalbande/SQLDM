package com.idera.sqldm.ui.dashboard.instances.resources;

import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.category.resources.TopSessionsIOActivity;
import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;
import com.idera.sqldm.ui.topten.TopXWidgetComposer;


public class SessionsIOWidgetComposer  extends TopXWidgetComposer {
protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory.getLogger(SessionsIOWidgetComposer.class);
	
	@Init
	public void init() {
		super.init();
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		return ((TopSessionsIOActivity)instance).getPhysicalIO().toString();
	}
	
	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.INSTANCE_SESSIONS_IO_ACTIVITY;
	}
	 
	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.SESSIONS_SUMMARY ;
	}	

}
