package com.idera.sqldm.ui.topten;

import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.ResponseTime;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;


public class ResponseTimeWidgetComposer extends TopXWidgetComposer {

	@Override
	public String getUtilization(IWidgetInstance instance) {
		return new Integer(((ResponseTime)instance).getResponseTime()).toString();
	}

	@Init
	public void init() {
		super.init();
	}
	
	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.RESPONSE_TIME_WIDGET;
	}

	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.SESSIONS_OVERVIEW ;
	}
	
}