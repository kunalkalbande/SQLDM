package com.idera.sqldm.ui.topten;

import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.QueryWidgetInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;


public class QueryWidgetComposer extends TopXWidgetComposer {
	
	@Init
	public void init() {
		super.init();
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		return new Integer(((QueryWidgetInstance)instance).getNumberOfQueries()).toString();
	}
	
	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.QUERY_MONITOR_EVENTS;
	}

	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.QUERIES_SUMMARY;
	}	

	@Override
	public String getEmptyMessage() {
		return ELFunctions.getMessage(SQLdmI18NStrings.TOPTEN_QUERY_DATA_NOT_AVAILABLE);
	}

}
