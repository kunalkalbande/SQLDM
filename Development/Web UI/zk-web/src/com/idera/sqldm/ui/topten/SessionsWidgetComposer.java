package com.idera.sqldm.ui.topten;

import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.Sessions;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;


public class SessionsWidgetComposer  extends TopXWidgetComposer {
protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory.getLogger(SessionsWidgetComposer.class);
	
	@Init
	public void init() {
		super.init();
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		return ((Sessions)instance).getSessionCPUUsage().toString();
	}
	
	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.SESSIONS;
	}
	 
	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.SESSIONS_SUMMARY ;
	}	

}
