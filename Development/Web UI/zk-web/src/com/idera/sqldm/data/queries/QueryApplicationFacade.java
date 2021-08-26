package com.idera.sqldm.data.queries;

import java.util.List;

import com.idera.common.rest.RestException;
import com.idera.sqldm.rest.SQLDMRestClient;

public class QueryApplicationFacade {

	public static List<QueryApplication> getQueryApplications(String productInstanceName, int instanceId,
			int startLimit, int noOfRecords) throws RestException {
		return SQLDMRestClient.getInstance().getQueryApplications(productInstanceName,instanceId,
				startLimit, noOfRecords);
	}

}
