package com.idera.sqldm.ui.topten;

import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.LargestDatabaseBySizeInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;

public class LargestDatabaseBySizeWidgetComposer extends TopXWidgetComposer {
	
	@Init
	public void init() {
		super.init();
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		return new Double(((LargestDatabaseBySizeInstance)instance).getFileSize()).toString();
	}
	
	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.LARGEST_DATABASES_BY_SIZE;
	}
	 
	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.DATABASES_SUMMARY;
	}	

}
