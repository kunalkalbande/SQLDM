package com.idera.sqldm.ui.dashboard.instances.queries;

import java.util.Comparator;
import java.util.List;

import com.idera.common.rest.RestException;
import com.idera.sqldm.data.InstanceException;
import com.idera.sqldm.data.instances.Query;
import com.idera.sqldm.data.instances.QueryWaits;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.rest.SQLDMRestClient;

public class SingleInstanceQueriesFacade {

	public static List<Query> getQueryInstance(String productInstanceName,int instanceId)	throws InstanceException {
		try {
			return SQLDMRestClient.getInstance().getQueryInstance(productInstanceName,instanceId, 240, -1);
		} catch (RestException x) {
			throw new InstanceException(x,
					SQLdmI18NStrings.FAILED_TO_GET_DASHBOARD_INSTANCE);
		}
	}
	
	public static List<QueryWaits> getQueryWaitsInstances(String productInstanceName, int instanceId, String offset, String fromDateTime, String endDateTime, int waitTypeId,int waitCategoryId, int sqlStatementId, int applicationId, int databaseId, int hostId, int sessionId, int loginId)	throws InstanceException {
		try {
			return SQLDMRestClient.getInstance().getQueryWaitsInstance(productInstanceName, instanceId, offset, fromDateTime, endDateTime, waitTypeId, waitCategoryId, sqlStatementId, applicationId, databaseId, hostId, sessionId, loginId);
		} catch (RestException x) {
			throw new InstanceException(x,
					SQLdmI18NStrings.FAILED_TO_GET_DASHBOARD_INSTANCE);
		}
	}
	
	
	/**
	 * Comparator which will sort the list based on the database size in desc order.
	 */
	public static final Comparator<Query> QUERY_NAME_COMPARATOR_ASC = new Comparator<Query>() {

		@Override
		public int compare(Query o1, Query o2) {
			
			return compareQueryName(o1,o2,false);
		}

	};
	/**
	 * Comparator which will sort the list based on the database size in desc order.
	 */
	public static final Comparator<Query> QUERY_NAME_COMPARATOR_DESC = new Comparator<Query>() {

		@Override
		public int compare(Query o1, Query o2) {
			
			return compareQueryName(o1,o2,true);
		}

	};
	
	
	
	private static int  compareQueryName(Query o1,Query o2, boolean desc) {
			
			int ret = 0;
			
			if(o1!=null && o2!=null){
			ret = o1.getQueryName().toLowerCase().compareTo(o2.getQueryName().toLowerCase());
			}
			return (ret  * (desc ? -1 : 1));
		
	}
	
	public static List<QueryWaits> getOverviewQueryWaitsInstances(String productInstanceName, int instanceId, String offset, String fromDateTime, String endDateTime)	throws InstanceException {
		try {
			return SQLDMRestClient.getInstance().getOverviewQueryWaitsInstance(productInstanceName, instanceId, offset, fromDateTime, endDateTime);
		} catch (RestException x) {
			throw new InstanceException(x,
					SQLdmI18NStrings.FAILED_TO_GET_DASHBOARD_INSTANCE);
		}
	}
}
