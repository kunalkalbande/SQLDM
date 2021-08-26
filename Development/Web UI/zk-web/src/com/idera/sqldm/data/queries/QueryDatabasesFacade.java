package com.idera.sqldm.data.queries;

import java.util.List;

import com.idera.common.rest.RestException;
import com.idera.sqldm.rest.SQLDMRestClient;

public class QueryDatabasesFacade {

	public static List<QueryDatabases> getQueryDatabases(String productInstanceName,int instanceId,
			int startLimit, int noOfRecords) throws RestException {
		return SQLDMRestClient.getInstance().getQueryDatabases(productInstanceName,instanceId, startLimit,
				noOfRecords);
	}

}
