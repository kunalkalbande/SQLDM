package com.idera.sqldm.ui.topten;

import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.TopDatabaseByAlertWidgetInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;

public class TopDatabaseByAlertWidgetComposer extends TopXWidgetComposer {

	@Init
	public void init() {
		super.init();
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		return new Long(((TopDatabaseByAlertWidgetInstance)instance).getNumberOfAlerts()).toString();
	}
	
	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.DATABASE_WITH_MOST_ALERTS;
	}

	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.DATABASES_SUMMARY;
	}	

}
