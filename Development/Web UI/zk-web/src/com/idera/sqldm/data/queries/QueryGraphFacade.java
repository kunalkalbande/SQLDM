package com.idera.sqldm.data.queries;

import java.util.List;

import com.idera.common.rest.RestException;
import com.idera.sqldm.rest.SQLDMRestClient;

public class QueryGraphFacade {

	public static List<QueryGraph> getQueryGraph(String productInstanceName,int instanceID, int viewID,
			int groupID, double timeZoneOffSet, String applicationIDs,
			String dbIDs, String userIDs, String clientIDs,
			String sqlIncludeText, String sqlExculdeText,
			String advancedFilters, String startTime, String endTime,
			int querySignatureID, int eventId) throws RestException {
		return SQLDMRestClient.getInstance().getQueryGraphBars(productInstanceName,instanceID, viewID,
				groupID, timeZoneOffSet, applicationIDs, dbIDs, userIDs,
				clientIDs, sqlIncludeText, sqlExculdeText, advancedFilters,
				startTime, endTime, querySignatureID, eventId);
	}
}
