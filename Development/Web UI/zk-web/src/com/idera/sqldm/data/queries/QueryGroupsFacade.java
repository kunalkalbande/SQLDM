package com.idera.sqldm.data.queries;

import java.util.List;

import com.idera.common.rest.RestException;
import com.idera.sqldm.rest.SQLDMRestClient;

public class QueryGroupsFacade {

	public static List<QueryGroups> getQueryGroups(String productInstanceName) throws RestException {
		return SQLDMRestClient.getInstance() .getQueryGroups(productInstanceName);
	}

}
