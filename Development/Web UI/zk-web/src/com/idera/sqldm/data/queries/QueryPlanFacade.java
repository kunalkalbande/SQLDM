package com.idera.sqldm.data.queries;

import java.util.ArrayList;

import com.idera.common.rest.RestException;
import com.idera.sqldm.rest.SQLDMRestClient;

public class QueryPlanFacade {
	public static ArrayList<QueryPlan> getQueryPlan(String productInstanceName,int instanceID, int queryID)
			throws RestException {
		return SQLDMRestClient.getInstance().getQueryPlan(productInstanceName,instanceID, queryID);
	}
}
