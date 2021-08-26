package com.idera.sqldm.ui.topten;

import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.topten.BlockedSession;
import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;

public class BlockedSessionWidgetComposer extends TopXWidgetComposer {
	
	@Override
	public String getUtilization(IWidgetInstance instance) {
		return new Integer(((BlockedSession)instance).getNumberOfBlockedProcess()).toString();
	}

	@Init
	public void init() {
		super.init();
	}

	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.BLOCKED_SESSIONS;
	}
	 
	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		return InstanceSubCategoriesTab.SESSIONS_OVERVIEW;
	}

}
