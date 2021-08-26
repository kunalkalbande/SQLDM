package com.idera.sqldm.ui.topten;

import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.TempDBUtilizationInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;



/**
 * This widget is responsible for displaying most tempdb space utilization. 
 * @author Accolite
 *
 */
public class TempDBUtilizationWidgetComposer extends TopXWidgetComposer {
	@Init
	public void init() {
		super.init();
	}


	@Override
	public String getUtilization(IWidgetInstance instance) {
		return new Double(((TempDBUtilizationInstance)instance).getSpaceUtilization()).toString();
	}
	
	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.TEMPDB_UTILIZATION;
	}
	 
	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.DATABASES_TEMPDB;
	}	

}
