package com.idera.sqldm.data;

import java.util.Comparator;
import java.util.List;

import com.idera.common.rest.RestException;
import com.idera.i18n.I18NStrings;
import com.idera.sqldm.data.topten.IWidgetInstance;
import com.idera.sqldm.data.topten.LongestRunningQueryInstance;
import com.idera.sqldm.data.topten.TopDatabaseByAlertWidgetInstance;
import com.idera.sqldm.data.topten.TopXEnum;
import com.idera.sqldm.rest.SQLDMRestClient;
import com.idera.sqldm.utils.Comparators;

public class TopXWidgetFacade {
	
	public static List<IWidgetInstance> getInstances(String productInstanceName,TopXEnum topXEnum, int limit, int days) throws InstanceException {
		try {
			return SQLDMRestClient.getInstance().getTopXInstances(productInstanceName,topXEnum, limit, days);
		} catch (RestException x) {
			throw new InstanceException(x, I18NStrings.FAILED_TO_FETCH_INSTANCES);
		}
	}
	
	public static Comparator<TopDatabaseByAlertWidgetInstance> NUMBER_OF_ALERTS = new Comparator<TopDatabaseByAlertWidgetInstance>() {

		@Override
		public int compare(TopDatabaseByAlertWidgetInstance arg0,	TopDatabaseByAlertWidgetInstance arg1) {
			
			return (int)(arg0.getNumberOfAlerts() - arg1.getNumberOfAlerts())*(-1);
		}
	};
	
	public static Comparator<LongestRunningQueryInstance> CPU_TIME_ASC = new Comparator<LongestRunningQueryInstance>() {

		@Override
		public int compare(LongestRunningQueryInstance widget1,	LongestRunningQueryInstance widget2) {
			
			return Comparators.compareLogicStringNumerals(widget2.getCpuTime(), widget1.getCpuTime());
		}
	};
	
	public static Comparator<LongestRunningQueryInstance> CPU_TIME_DESC = new Comparator<LongestRunningQueryInstance>() {

		@Override
		public int compare(LongestRunningQueryInstance widget1,	LongestRunningQueryInstance widget2) {
			
			return Comparators.compareLogicStringNumerals(widget1.getCpuTime(), widget2.getCpuTime());
		}
	};
	
	public static Comparator<LongestRunningQueryInstance> QUERY_EXEC_TIME_ASC = new Comparator<LongestRunningQueryInstance>() {

		@Override
		public int compare(LongestRunningQueryInstance widget1,	LongestRunningQueryInstance widget2) {
			
			return Comparators.compareLogicStringNumerals(widget2.getQueryExecTimeInMs(), widget1.getQueryExecTimeInMs());
		}
	};
	
	public static Comparator<LongestRunningQueryInstance> QUERY_EXEC_TIME_DESC = new Comparator<LongestRunningQueryInstance>() {

		@Override
		public int compare(LongestRunningQueryInstance widget1,	LongestRunningQueryInstance widget2) {
			
			return Comparators.compareLogicStringNumerals(widget1.getQueryExecTimeInMs(), widget2.getQueryExecTimeInMs());
		}
	};

}
