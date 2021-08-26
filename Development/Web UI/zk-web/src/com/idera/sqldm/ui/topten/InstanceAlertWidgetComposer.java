package com.idera.sqldm.ui.topten;

import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.InstanceAlertWidgetInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;

public class InstanceAlertWidgetComposer extends TopXWidgetComposer {

	@Init
	public void init() {
		super.init();
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		return ""+((InstanceAlertWidgetInstance)instance).getNumberOfAlerts();
	}
	
	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.INSTANCE_ALERT;
	}
	 
	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		// TODO Auto-generated method stub
		return InstanceSubCategoriesTab.OVERVIEW_SUMMARY;
	}

}
