package com.idera.sqldm.data;

import java.util.Date;
import java.util.List;

import com.idera.common.rest.RestException;
import com.idera.sqldm.data.instances.SessionGraphDetail;
import com.idera.sqldm.rest.SQLDMRestClient;

public class InstanceDetailsSessionFacade {

	public static List<InstanceDetailsSession> getInstanceDetailsSession(String productInstanceName, int instanceId)
			throws RestException {
		return SQLDMRestClient.getInstance().getInstanceDetailsSession(productInstanceName, instanceId);
	}

	public static List<SessionGraphDetail> getSessionChartDetail(String productInstanceName, int instanceId, int numHistoryMinutes)throws RestException {
		return SQLDMRestClient.getInstance().getInstanceDetailsSessionGraph(productInstanceName, instanceId, numHistoryMinutes);
	}
	
	/*
	 * ChaitanyaTanwar DM 10.2
	 * */
	public static List<SessionGraphDetail> getSessionChartDetail(String productInstanceName, int instanceId, long numHistoryMinutes, Date endTime)throws RestException {
		return SQLDMRestClient.getInstance().getInstanceDetailsSessionGraph(productInstanceName, instanceId, numHistoryMinutes, endTime);
	}
}
