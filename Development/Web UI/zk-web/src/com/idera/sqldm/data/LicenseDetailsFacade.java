package com.idera.sqldm.data;

import com.idera.common.rest.RestException;
import com.idera.sqldm.rest.SQLDMRestClient;

public class LicenseDetailsFacade {
	public static LicenseDetails getLicenseDetails(String productInstanceName)
			throws RestException {

		return SQLDMRestClient.getInstance().getLicenseDetails(productInstanceName);
	}

}
