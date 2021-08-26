package com.idera.sqldm.ui.topten;

import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.topten.DiskSpaceWidgetInstance;
import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;

public class DiskSpaceWidgetComposer extends TopXWidgetComposer {
	
	@Override
	public String getUtilization(IWidgetInstance instance) {
		return ((DiskSpaceWidgetInstance)instance).getDiskSpaceUtilization();
	}

	@Init
	public void init() {
		super.init();
	}

	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.DISK_SPACE;
	}
	 
	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.RESOURCES_DISKVIEW ;
	}

}
