package com.idera.sqldm.data;

import java.util.Comparator;
import java.util.List;

import com.idera.common.rest.RestException;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.rest.SQLDMRestClient;

public class DashboardWidgetsFacade {
	public static List<DashboardAlertsByCategoryWidget> getNumAlertsByCategory(String productInstanceName)
			throws InstanceException {
		try {
			return SQLDMRestClient.getInstance().getNumAlertsByCategory(productInstanceName,true);
		} catch (RestException x) {
			throw new InstanceException(x,
					SQLdmI18NStrings.FAILED_TO_GET_ALERTS_BY_CATEGORY);
		}
	}
	public static List<DashboardAlertsByDatabaseWidget> getNumAlertsByDatabase(String productInstanceName)
			throws InstanceException {
		try {
			return SQLDMRestClient.getInstance().getNumAlertsByDatabase(productInstanceName);
		} catch (RestException x) {
			throw new InstanceException(x,
					SQLdmI18NStrings.FAILED_TO_GET_ALERTS_BY_DATABASE);
		}
	}
	
	public static List<DashboardAlertsByInstanceWidget> getNumAlertsByInstance(String productInstanceName)
			throws InstanceException {
		try {
			return SQLDMRestClient.getInstance().getNumAlertsByInstance(productInstanceName);
		} catch (RestException x) {
			throw new InstanceException(x,
					SQLdmI18NStrings.FAILED_TO_GET_ALERTS_BY_DATABASE);
		}
	}

	public static List<DashboardWorstResponseTimeWidget> getLatestResponseTime(String productInstanceName , String offSet)
			throws InstanceException {
		try {
			return SQLDMRestClient.getInstance().getLatestResponseTime(productInstanceName , offSet);
		} catch (RestException x) {
			throw new InstanceException(x,
					SQLdmI18NStrings.FAILED_TO_GET_ALERTS_BY_DATABASE);
		}
	}
	public static final Comparator<DashboardWorstResponseTimeWidget> LATEST_RESPONSE_COMPARATOR = new Comparator<DashboardWorstResponseTimeWidget>() {
		@Override
	    public int compare(DashboardWorstResponseTimeWidget o1, DashboardWorstResponseTimeWidget o2) {
	    	int result = o2.getResponseTimeMillis().compareTo(o1.getResponseTimeMillis());
	    	if (result == 0) {
//	    		return o1.getInstanceName().compareTo(o2.getInstanceName()); @author Saumyadeep
	    		return o1.getDisplayName().compareTo(o2.getDisplayName());
	    	}
	    	return result;
	    }
	};
	public static final Comparator<DashboardAlertsByCategoryWidget> ALERTS_BY_CATEGORY_COMPARATOR = new Comparator<DashboardAlertsByCategoryWidget>() {
		@Override
	    public int compare(DashboardAlertsByCategoryWidget o1, DashboardAlertsByCategoryWidget o2) {
	    	int result = o2.getNumOfAlerts().compareTo(o1.getNumOfAlerts());
	    	if (result == 0) {
	    		return o1.getCategory().compareTo(o2.getCategory());
	    	}
	    	return result;
	    }
	};
	public static final Comparator<DashboardAlertsByDatabaseWidget> ALERTS_BY_DATABASE_COMPARATOR = new Comparator<DashboardAlertsByDatabaseWidget>() {
		@Override
	    public int compare(DashboardAlertsByDatabaseWidget o1, DashboardAlertsByDatabaseWidget o2) {
	    	int result = o2.getNumOfAlerts().compareTo(o1.getNumOfAlerts());
	    	if (result == 0) {
	    		result = o1.getDatabaseName().compareTo(o2.getDatabaseName());
	    		if (result == 0) {
//	    			result = o1.getInstanceName().compareTo(o2.getInstanceName()); @author Saumyadeep
	    			result = o1.getDisplayName().compareTo(o2.getDisplayName());
	    			if (result == 0) {
	    				return o1.getInstanceId().compareTo(o2.getInstanceId());
	    			}
	    		}
	    	}
	    	return result;
	    }
	};
	
	public static final Comparator<DashboardAlertsByInstanceWidget> ALERTS_BY_INSTANCE_COMPARATOR = new Comparator<DashboardAlertsByInstanceWidget>() {
		@Override
	    public int compare(DashboardAlertsByInstanceWidget o1, DashboardAlertsByInstanceWidget o2) {
	    	int result = o2.getNumOfAlerts().compareTo(o1.getNumOfAlerts());
	    	if (result == 0) {
//	    		result = o1.getInstanceName().compareTo(o2.getInstanceName()); @author Saumyadeep
	    		result = o1.getDisplayName().compareTo(o2.getDisplayName());
	    		if (result == 0) {
	    			return o1.getInstanceId().compareTo(o2.getInstanceId());
	    		}
	    	}
	    	return result;
	    }
	};
	
}

