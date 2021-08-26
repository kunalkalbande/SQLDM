package com.idera.sqldm.ui.topten;

import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.MemoryUsageWidgetInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;

public class MemoryUsageWidgetComposer extends TopXWidgetComposer {
	
	@Init
	public void init() {
		super.init();
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		return new Double(((MemoryUsageWidgetInstance)instance).getMemoryUsage()).toString();
	}
	
	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.SQL_MEMORY_USAGE;
	}
	 
	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.RESOURCES_MEMORYVIEW;
	}	

}
