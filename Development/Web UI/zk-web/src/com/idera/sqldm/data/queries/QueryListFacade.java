package com.idera.sqldm.data.queries;

import java.util.List;

import com.idera.common.rest.RestException;
import com.idera.sqldm.rest.SQLDMRestClient;

public class QueryListFacade {

	public static List<QueryApplicationDetails> getQueryApplications(String productInstanceName,
			int instanceID, int viewID, int groupID, double timeZoneOffSet,
			String applicationIDs, String dbIDs, String userIDs,
			String clientIDs, String sqlIncludeText, String sqlExculdeText,
			String advancedFilters, String startTime, String endTime,
			int startIndex, int rowCount, String sortBy, String sortOrder,
			int sqlSignatureId, int eventId) throws RestException {
		return SQLDMRestClient.getInstance().getList(productInstanceName,instanceID, viewID, groupID,
				timeZoneOffSet, applicationIDs, dbIDs, userIDs, clientIDs,
				sqlIncludeText, sqlExculdeText, advancedFilters, startTime,
				endTime, startIndex, rowCount, sortBy, sortOrder,
				sqlSignatureId, eventId);
	}
}
