package com.idera.sqldm.data.customdashboard;

import java.util.Date;
import java.util.List;

import com.idera.common.rest.RestException;
import com.idera.sqldm.rest.SQLDMRestClient;

public class CustomDashboardFacade {

	public static CustomDashboard createCustomDashboard(String productInstanceName, String customDashboardName,boolean isDefault, String userSID)
			throws RestException {
		return SQLDMRestClient.getInstance().createCustomDashboard(productInstanceName,customDashboardName,isDefault,userSID);
	}
	
	public static CustomDashboard updateCustomDashboard(String productInstanceName, Integer customDashboardId,
			String customDashboardName, boolean isDefault,String userSID, List<String> tags)
			throws RestException {
		return SQLDMRestClient.getInstance().updateCustomDashboard(productInstanceName,customDashboardId,
				customDashboardName,isDefault,userSID,tags);
	}
	
	public static CustomDashboard deleteCustomDashboard(String productInstanceName, Integer customDashboardId)
			throws RestException {
		return SQLDMRestClient.getInstance().deleteCustomDashboard(productInstanceName, customDashboardId);
	}
	
	public static CustomDashboardWidget updateCustomdashboardWidget(String productInstanceName,
			Integer customDashboardId,Integer widgetId,String widgetName, int widgetTypeId,int metricId,
			int match,List<Integer> tagId,List<Integer> sourceServerIds)
			throws RestException {
		return SQLDMRestClient.getInstance().updateCustomdashboardWidget(productInstanceName, customDashboardId,
				widgetId,widgetName,widgetTypeId,metricId,match,tagId,sourceServerIds
				);
	}
	
	public static CustomDashboardWidget createCustomDashboardWidget(String productInstanceName,
			Integer customDashboardId,String widgetName, int widgetTypeId,int metricId,
			int match,List<Integer> tagId,List<Integer> sourceServerIds)
			throws RestException {
		return SQLDMRestClient.getInstance().createCustomDashboardWidget(productInstanceName, customDashboardId,
				widgetName,widgetTypeId,metricId,match,tagId,sourceServerIds
				);
	}
	
	public static List<CustomDashboardWidget> getCustomDashboardWidgets(String productInstanceName, Integer customDashboardId)
			throws RestException {
		return SQLDMRestClient.getInstance().getCustomDashboardWidgets(productInstanceName, customDashboardId);
	}
		
	public static List<CustomDashboard> getAllCustomDashboards(String productInstanceName, String userSID)
			throws RestException {
		return SQLDMRestClient.getInstance().getAllCustomDashboards(productInstanceName, userSID);
	}
	
	public static List<Types> getAllMatchTypes(String productInstanceName)
			throws RestException {
		return SQLDMRestClient.getInstance().getAllMatchTypes(productInstanceName);
	}
	
	public static List<Types> getAllWidgetTypes(String productInstanceName)
			throws RestException {
		return SQLDMRestClient.getInstance().getAllWidgetTypes(productInstanceName);
	}
	
	public static List<CustomDashboardWidgetData> getWidgetData(String productInstanceName, Integer customDashboardId,
			Integer widgetId, Date startTime, Date endTime, String tzo) throws RestException {
		return SQLDMRestClient.getInstance().getWidgetData(productInstanceName, customDashboardId, widgetId, startTime, endTime,tzo);
	}
	
	public static boolean deleteCustomDashboardWidget(String productInstanceName,Integer customDashboardId,
			Integer widgetId) throws RestException {
		return SQLDMRestClient.getInstance().deleteCustomDashboardWidget(productInstanceName, customDashboardId, widgetId);
	}
	
	public static boolean copyCustomDashboard(String productInstanceName,Integer customDashboardId) throws RestException {
		return SQLDMRestClient.getInstance().copyCustomDashboard(productInstanceName, customDashboardId);
	}
}
