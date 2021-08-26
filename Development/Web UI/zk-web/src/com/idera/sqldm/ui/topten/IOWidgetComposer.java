package com.idera.sqldm.ui.topten;

import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.topten.IOWidgetInstance;
import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;


/**
 * This class is responsible for displaying widget highest I/O load  (reads and writes). 
 * @author varun
 *
 */
public class IOWidgetComposer extends TopXWidgetComposer {

	@Init
	public void init() {
		super.init();
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		return ((IOWidgetInstance)instance).getPhysicalIO();
	}
	
	
	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.IO;
	}

	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.RESOURCES_DISKVIEW;
	}
	
}
