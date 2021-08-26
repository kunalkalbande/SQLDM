package com.idera.sqldm.data.queries;

import java.util.List;

import com.idera.common.rest.RestException;
import com.idera.sqldm.rest.SQLDMRestClient;


public class QueryUsersFacade {

	public static List<QueryUsers> getQueryUsers(String productInstanceName, int instanceId, int startLimit, int noOfRecords)
			throws RestException {
		return SQLDMRestClient.getInstance().getQueryUsers(productInstanceName,instanceId, startLimit, noOfRecords);
	}


}
