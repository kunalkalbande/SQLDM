package com.idera.sqldm.ui.topten;

import java.util.Comparator;

import org.zkoss.bind.annotation.Init;

import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.data.topten.WaitWidgetInstance;
import com.idera.sqldm.ui.dashboard.instances.InstanceSubCategoriesTab;


/**
 * This is the widget for Top X waits by instances.
 * @author Accolite
 *
 */
public class WaitWidgetComposer extends TopXWidgetComposer {

	@Init
	public void init() {
		super.init();
	}

	@Override
	public String getUtilization(IWidgetInstance instance) {
		return new Double(((WaitWidgetInstance)instance).getWaitTime()).toString();
	}
	
	@Override
	public TopXEnum getTopXEnum() {
		return TopXEnum.WAITS;
	}

	public static final Comparator<WaitWidgetInstance> WAIT_WIDGET_INSTANCE_WAIT_TIME_DESC = new Comparator<WaitWidgetInstance>() {

		@Override
		public int compare(WaitWidgetInstance o1, WaitWidgetInstance o2) {
			
			return compareWaitTime(o1,o2,true);
		}

	};
	
	private static int  compareWaitTime(WaitWidgetInstance o1,WaitWidgetInstance o2, boolean desc) {
			
			long ret = 0;
			
			ret = (long)(o1.getWaitTime() - o2.getWaitTime());
			
			return (int)(ret  * (desc ? -1 : 1));
		
	}

	@Override
	public InstanceSubCategoriesTab getInstanceSubCategory() {
		// TODO Auto-generated method stub
		return InstanceSubCategoriesTab.RESOURCES_SERVERWAITS ;
	}

}
