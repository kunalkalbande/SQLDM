package com.idera.sqldm_10_3.data.alerts;

import com.idera.common.rest.RestException;
import com.idera.sqldm_10_3.i18n.SQLdmI18NStrings;
import com.idera.sqldm_10_3.rest.SQLDMRestClient;
import com.idera.sqldm_10_3.rest.TimedValue;
import com.idera.sqldm_10_3.rest.UnauthorizedAccessException;
import com.idera.sqldm_10_3.rest.SQLDMRestClient;

import java.util.Date;
import java.util.List;

public class AlertFacade {
	
	
	public static List<Alert> getAllAlerts(String productInstanceName, boolean isActive, Date startTime, Date endTime, Integer metricId, Integer instanceId, Integer severity, int limit,
                                           String orderBy, String orderType , String offSet) throws AlertException{
		try {
			List<Alert> list = SQLDMRestClient.getInstance().getAlerts(productInstanceName, isActive, startTime, endTime, instanceId, metricId, severity, null, limit, orderBy, orderType, null , offSet);
			//Collections.sort(list, ALERTS_COMPARATOR);
			return list;
		} catch (RestException e) {
			throw new AlertException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_ALERTS);
		}
	}
	
	public static List<Alert> getAllAlerts(String productInstanceName, boolean isActive, Integer metricId, Integer instanceId, Integer severity, int limit, String orderBy, String orderType , String offSet) throws AlertException{
		try {
			List<Alert> list = SQLDMRestClient.getInstance().getAlerts(productInstanceName, isActive, null, null, instanceId, metricId, severity, null, limit, orderBy, orderType, null , offSet);
			//Collections.sort(list, ALERTS_COMPARATOR);
			return list;
		} catch (RestException e) {
			throw new AlertException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_ALERTS);
		}
	}
	
	public static List<Alert> getAllAlerts(String productInstanceName, boolean isActive, Integer instanceId, String category , String offSet) throws AlertException{
		try {
			List<Alert> list = SQLDMRestClient.getInstance().getAlerts(productInstanceName, isActive, null, null, instanceId, null, null, category , offSet);
			//Collections.sort(list, ALERTS_COMPARATOR);
			return list;
		} catch (RestException e) {
			throw new AlertException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_ALERTS);
		}
	}
	
	public static List<Alert> getAllAlerts(String productInstanceName, boolean isActive, String category , String offSet) throws AlertException{
		try {
			List<Alert> list = SQLDMRestClient.getInstance().getAlerts(productInstanceName, isActive, null, null, null, null, null, category , offSet);
			//Collections.sort(list, ALERTS_COMPARATOR);
			return list;
		} catch (RestException e) {
			throw new AlertException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_ALERTS);
		}
	}
	
	public static List<Alert> getAbridgeActiveAlerts(String productInstanceName , String offSet) throws AlertException{
		try {
			List<Alert> list = SQLDMRestClient.getInstance().getAbridgeAlerts(productInstanceName, true, null, null, null, null, null, null, 1500 , offSet);
			//Collections.sort(list, ALERTS_COMPARATOR);
			return list;
		} catch (RestException e) {
			throw new AlertException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_ALERTS);
		}
	}
		
	public static List<Alert> getActiveAlerts(String productInstanceName , String offSet) throws AlertException{
		try {
			List<Alert> list = SQLDMRestClient.getInstance().getAlerts(productInstanceName, true, null, null, null, null, null, null , offSet);
			//Collections.sort(list, ALERTS_COMPARATOR);
			return list;
		} catch (RestException e) {
			throw new AlertException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_ALERTS);
		}
	}
	
	
	public static Alert getAlert(String productInstanceName, Long alertId) throws AlertException, UnauthorizedAccessException{
		try {
			return SQLDMRestClient.getInstance().getAlert(productInstanceName, alertId);
		} catch (UnauthorizedAccessException x) {
			throw x;
		} catch (RestException e) {
			throw new AlertException(e, SQLdmI18NStrings.ERROR_OCCURRED_SINGLE_ALERT);
		}
	}
	
	public static List<TimedValue> getMetricsHistoryForAlert(String productInstanceName, Long alertId, Integer numHistoryHours , String offSet) throws AlertException{
		try {
			return SQLDMRestClient.getInstance().getMetricsHistoryForAlert(productInstanceName, alertId, numHistoryHours , offSet);
		} catch (RestException e) {
			throw new AlertException(e, SQLdmI18NStrings.ERROR_FETCHING_ALERT_HISTORY);
		}
	}
	
	public static List<Metrics> getAllMetrics(String productInstanceName , String offSet) throws AlertException{
		try {
			return SQLDMRestClient.getInstance().getMetrics(productInstanceName , offSet);
		} catch (RestException e) {
			throw new AlertException(e, SQLdmI18NStrings.ERROR_FETCHING_ALL_METRICS);
		}
	}
	
	public List<AlertMetaData> getAllAlertMetaData(String productInstanceName) throws AlertException {

		try {
			return SQLDMRestClient.getInstance().getAlertMetaData(productInstanceName, AlertMetrics.NULL, false);
		}
		catch (RestException e) {
			throw new AlertException(e, com.idera.i18n.I18NStrings.FAILED_TO_RETRIEVE_ALERT_METADATA);
		}

	}
	
}

