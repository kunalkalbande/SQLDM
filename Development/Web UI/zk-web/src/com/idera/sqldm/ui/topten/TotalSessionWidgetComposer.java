package com.idera.sqldm.ui.topten;

import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.data.topten.TotalSessionWidgetInstance;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;


public class TotalSessionWidgetComposer extends TopXWidgetComposer {
	
	@Init
	public void init() {
		super.init();
	}
	@Override
	public String getUtilization(IWidgetInstance instance) {
		return new Integer(((TotalSessionWidgetInstance)instance).getNumberOfSessions()).toString();
	}
	
	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.NUMBER_OF_SESSIONS;
	}

	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.SESSIONS_SUMMARY;
	}	

}
