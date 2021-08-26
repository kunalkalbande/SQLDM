package com.idera.sqldm.data.queries;

import java.util.List;

import com.idera.common.rest.RestException;
import com.idera.sqldm.rest.SQLDMRestClient;

public class QueryClientsFacade {

	public static List<QueryClients> getQueryClients(String productInstanceName, int instanceId,
			int startLimit, int noOfRecords) throws RestException {
		return SQLDMRestClient.getInstance().getQueryClients(productInstanceName,instanceId, startLimit,
				noOfRecords);
	}

}
