package com.idera.sqldm.ui.topten;

import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.TopDatabaseByActivityWidgetInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;


/**
 * This class is responsible for getting Top X databases by activity size across all my instances 
 * @author Varun
 *
 */
public class TopDatabaseByActivityWidgetComposer extends TopXWidgetComposer {

	@Init
	public void init() {
		super.init();
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		return new Double(((TopDatabaseByActivityWidgetInstance)instance).getTransactionValue()).toString();
	}
	
	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.TOP_DATABASES_BY_ACTIVITY;
	}

	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.DATABASES_SUMMARY;
	}	

}
