package com.idera.sqldm.ui.topten;

import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.topten.CpuLoad;
import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;

public class CpuLoadWidgetComposer extends TopXWidgetComposer {

	@Init
	public void init() {
		super.init();
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		String utilization = "" + ((CpuLoad)instance).getCpuLoad();
		return utilization;
	}
	
	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.SQL_CPU_LOAD;
	}
	 
	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.RESOURCES_CPUVIEW;
	}

}
