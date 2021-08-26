package com.idera.sqldm.data.queries;

import java.util.List;

import com.idera.common.rest.RestException;
import com.idera.sqldm.rest.SQLDMRestClient;

public class ViewMetricsFacade {

	public static List<ViewMetrics> getQueryViewMetrics(String productInstanceName) throws RestException {
		return SQLDMRestClient.getInstance().getQueryViewMetrics(productInstanceName);
	}

}
