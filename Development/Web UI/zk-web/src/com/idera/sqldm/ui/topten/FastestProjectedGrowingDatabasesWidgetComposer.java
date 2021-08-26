package com.idera.sqldm.ui.topten;
import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.topten.FastestProjectedGrowingDatabasesWidgetInstance;
import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;


public class FastestProjectedGrowingDatabasesWidgetComposer extends TopXWidgetComposer {
	
	@Init
	public void init() {
		super.init();
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		return String.valueOf(((FastestProjectedGrowingDatabasesWidgetInstance)instance).getTotalSizeDiffernceKb());
	}
	
	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.FASTEST_PROJECTED_GROWING_DATABASES;
	}
	 
	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.DATABASES_SUMMARY;
	}
	
}
