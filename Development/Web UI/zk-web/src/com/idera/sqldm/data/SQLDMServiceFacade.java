package com.idera.sqldm.data;

import com.idera.common.rest.RestException;
import com.idera.common.rest.ServiceStatusResponse;
import com.idera.sqldm.rest.SQLDMRestClient;

public class SQLDMServiceFacade {

	public static ServiceStatusResponse getServiceStatus(String productInstanceName)
			throws SQLDMException {

		try {
			return SQLDMRestClient.getInstance().getServiceStatus(productInstanceName);

		} catch (RestException x) {

			throw new SQLDMException(x);
		}
	}

	public static String getVersion(String productInstanceName) throws SQLDMException {

		try {
			return SQLDMRestClient.getInstance().getVersion(productInstanceName);

		} catch (RestException x) {

			throw new SQLDMException(x);
		}
	}

}
