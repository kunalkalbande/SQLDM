package com.idera.sqldm.data.databases;

import java.util.Collection;
import java.util.Date;
import java.util.List;

import com.idera.common.rest.RestException;
import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.data.category.resources.ResourceCategory;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.rest.SQLDMRestClient;

public class AvailabilityGroupDetailsFacade {
	/**
	 * Author:Accolite
	 * Date : 20th Jan, 2017
	 * Database Availability Group- 10.2 release
	 * @param productInstanceName
	 * @param instanceId
	 * @param numHistoryMinutes
	 * @return
	 * @throws RestException
	 */
	public static List<AvailabilityGroupDetails> getAvailabilityGroupDetails(String productInstanceName,int instanceId, int numHistoryMinutes)
			throws RestException {
		return SQLDMRestClient.getInstance().getAvailabilityGroupDetails(productInstanceName,instanceId,numHistoryMinutes);
	}
	
	/**
	 * Author:Accolite
	 * Date : 20th Jan, 2017
	 * Database Availability Group- 10.2 release
	 * @param productInstanceName
	 * @param instanceId
	 * @param numHistoryMinutes
	 * @param endTime
	 * @return
	 * @throws RestException
	 */
	public static List<AvailabilityGroupDetails> getAvailabilityGroupDetails(String productInstanceName,int instanceId, long numHistoryMinutes, Date endDate)
			throws RestException {
		return SQLDMRestClient.getInstance().getAvailabilityGroupDetails(productInstanceName,instanceId, numHistoryMinutes,endDate);
	}
	
	public static List<AvailabilityGroupStatistics> getAvailabilityGroupStatistics(String productInstanceName,int instanceId, int numHistoryMinutes)
			throws RestException {
		return SQLDMRestClient.getInstance().getAvailabilityGroupStatistics(productInstanceName,instanceId, numHistoryMinutes);
	}
	
	/*
	 * Author:Accolite
	 * Date : 12th Dec, 2016
	 * History Panel - SQLDM- 10.2 release
	 * Adding History Panel to Database
	 */
	public static List<AvailabilityGroupStatistics> getAvailabilityGroupStatistics(String productInstanceName,Integer instanceId, long numHistoryMinutes, Date endTime)
			throws RestException{
		return SQLDMRestClient.getInstance().getAvailabilityGroupStatistics(productInstanceName,instanceId, numHistoryMinutes,endTime);
	}
}
