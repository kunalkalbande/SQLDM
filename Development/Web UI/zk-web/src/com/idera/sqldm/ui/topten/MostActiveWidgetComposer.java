package com.idera.sqldm.ui.topten;

import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.MostActiveConnection;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;


public class MostActiveWidgetComposer extends TopXWidgetComposer {
protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory.getLogger(MostActiveWidgetComposer.class);

	@Init
	public void init() {
		super.init();
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		return new Integer(((MostActiveConnection)instance).getConnections()).toString();
	}
	
	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.MOST_ACTIVE_CONNECTIONS;
	}
	 
	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.SESSIONS_SUMMARY ;
	}	
	
}
