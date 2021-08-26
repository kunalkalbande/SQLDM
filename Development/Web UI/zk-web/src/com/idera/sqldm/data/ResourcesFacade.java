package com.idera.sqldm.data;

import java.util.Date;
import java.util.List;

import com.idera.common.rest.RestException;
import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.data.category.resources.CPUStatDetails;
import com.idera.sqldm.data.category.resources.CustomCounterStats;
import com.idera.sqldm.data.category.resources.FileActivity;
import com.idera.sqldm.data.category.resources.FileDrive;
import com.idera.sqldm.data.category.resources.FileStats;
import com.idera.sqldm.data.category.resources.GraphBaseline;
import com.idera.sqldm.data.category.resources.LockWaitData;
import com.idera.sqldm.data.category.resources.NetworkStats;
import com.idera.sqldm.data.category.resources.OSPagingData;
import com.idera.sqldm.data.category.resources.ResourceCategory;
import com.idera.sqldm.data.category.resources.ServerWait;
import com.idera.sqldm.data.instances.DatabaseStatsDetails;
import com.idera.sqldm.data.instances.VirtualizationStatsDetails;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.rest.SQLDMRestClient;

public class ResourcesFacade {        	

	public static List<ResourceCategory> getResources(String productInstanceName,Integer instanceId, int numHistoryMinutes) throws CategoryException{
		try {
			return SQLDMRestClient.getInstance().getResources(productInstanceName, instanceId, numHistoryMinutes);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_RESOURCES);
		}
	}
	
	/*
	 * ChaitanyaTanwar DM 10.2
	 * */
	public static List<ResourceCategory> getResources(String productInstanceName,Integer instanceId, long numHistoryMinutes, Date endTime) throws CategoryException{
		try {
			return SQLDMRestClient.getInstance().getResources(productInstanceName, instanceId, numHistoryMinutes,endTime);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_RESOURCES);
		}
	}
	
	public static List<FileDrive> getFileDrives(String productInstanceName, Integer instanceId) throws CategoryException{
		try {
			return SQLDMRestClient.getInstance().getFileDrives(productInstanceName, instanceId);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FILE_DRIVES);
		}
	}
	
	public static List<FileActivity> getFileActivity(String productInstanceName, Integer instanceId, int numHistoryMinutes) throws CategoryException{
		try {
			return SQLDMRestClient.getInstance().getFileActivity(productInstanceName, instanceId, numHistoryMinutes);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_FETCHING_FILE_ACTIVITY);
		}
	}
	
	public static List<ServerWait> getServerWaits(String productInstanceName, Integer instanceId, int numHistoryMinutes) throws CategoryException{
		try {
			return SQLDMRestClient.getInstance().getServerWaits(productInstanceName, instanceId, numHistoryMinutes);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_FETCHING_SERVER_WAIT);
		}
	}
	
	/*
	 * ChaitanyaTanwar DM 10.2
	 * */
	public static List<ServerWait> getServerWaits(String productInstanceName, Integer instanceId, long numHistoryMinutes, Date endTime) throws CategoryException{
		try {
			return SQLDMRestClient.getInstance().getServerWaits(productInstanceName, instanceId, numHistoryMinutes,endTime);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_FETCHING_SERVER_WAIT);
		}
	}
	
	public static List<GraphBaseline> getBaseline(String productInstanceName, Integer instanceId, int metricId, int numHistoryMinutes) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getBaseline(productInstanceName, instanceId, metricId, numHistoryMinutes);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_RESOURCES);
		}		
	}
	/*
	 * ChaitanyaTanwar DM 10.2
	 * */
	public static List<GraphBaseline> getBaseline(String productInstanceName, Integer instanceId, int metricId, long numHistoryMinutes, Date endTime) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getBaseline(productInstanceName, instanceId, metricId, numHistoryMinutes,endTime);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_RESOURCES);
		}		
	}
	/*
	 * Api calls for overview screen customization
	 * SQLDM 10.2
	 */
	public static List<CPUStatDetails> getCpuStatDetails(String productInstanceName, Integer instanceId, long numHistoryMinutes, Date endTime) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getCPUStats(productInstanceName, instanceId, numHistoryMinutes, endTime);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_CPU_STATS);
		}		
	}
	public static List<CPUStatDetails> getCpuStatDetails(String productInstanceName, Integer instanceId, long numHistoryMinutes) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getCPUStats(productInstanceName, instanceId, numHistoryMinutes);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_CPU_STATS);
		}		
	}

	public static List<OSPagingData> getOSPagingDetails(
			String productInstanceName, int instanceId, int numHistoryMinutes) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getOSPagingStats(productInstanceName, instanceId, numHistoryMinutes);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_OSPAGING_STATS);
		}	
	}

	public static List<OSPagingData> getOSPagingDetails(
			String productInstanceName, int instanceId, long numHistoryMinutes,
			Date toDateTime) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getOSPagingStats(productInstanceName, instanceId, numHistoryMinutes, toDateTime);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_OSPAGING_STATS);
		}
	}

	public static List<LockWaitData> getLockWaitsDetails(
			String productInstanceName, int instanceId, long numHistoryMinutes) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getLockWaitsData(productInstanceName, instanceId, numHistoryMinutes);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_LOCK_WAITS_STATS);
		}
	}
	
	public static List<LockWaitData> getLockWaitsDetails(
			String productInstanceName, int instanceId, long numHistoryMinutes,
			Date toDateTime) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getLockWaitsData(productInstanceName, instanceId, numHistoryMinutes, toDateTime);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_LOCK_WAITS_STATS);
		}
	}

	public static List<DatabaseStatsDetails> getDBStatDetails(
			String productInstanceName, int instanceId, long numHistoryMinutes,
			Date toDateTime) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getDBStatData(productInstanceName, instanceId, numHistoryMinutes, toDateTime);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_DB_STATS);
		}
	}
	
	public static List<DatabaseStatsDetails> getDBStatDetails(
			String productInstanceName, int instanceId, long numHistoryMinutes) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getDBStatData(productInstanceName, instanceId, numHistoryMinutes);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_DB_STATS);
		}
	}
	
	/*
	 * Author:Accolite
	 * Date : 20th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release
	 */

	public static List<CustomCounterStats> getCustomCounterStats(
			String productInstanceName, int instanceId, long numHistoryMinutes) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getCustomCounterStats(productInstanceName, instanceId, numHistoryMinutes);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_CUSTOM_COUNTERS);
		}
	}
	
	/*
	 * Author:Accolite
	 * Date : 20th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release
	 */

	public static List<CustomCounterStats> getCustomCounterStats(
			String productInstanceName, int instanceId, long numHistoryMinutes,
			Date toDateTime) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getCustomCounterStats(productInstanceName, instanceId, numHistoryMinutes, toDateTime);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_CUSTOM_COUNTERS);
		}
	}
	
	/*
	 * Author:Accolite
	 * Date : 21th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release
	 */

	public static List<NetworkStats> getNetworkStats(
			String productInstanceName, int instanceId, long numHistoryMinutes) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getNetworkStats(productInstanceName, instanceId, numHistoryMinutes);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_NETWORK_STATS);
		}
	}
	
	/*
	 * Author:Accolite
	 * Date : 21th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release
	 */

	public static List<NetworkStats> getNetworkStats(
			String productInstanceName, int instanceId, long numHistoryMinutes,
			Date toDateTime) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getNetworkStats(productInstanceName, instanceId, numHistoryMinutes, toDateTime);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_NETWORK_STATS);
		}
	}
	
	/*
	 * Author:Accolite
	 * Date : 21th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release
	 */

	public static List<FileStats> getFileStats(
			String productInstanceName, int instanceId, long numHistoryMinutes) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getFileStats(productInstanceName, instanceId, numHistoryMinutes);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_FILE_STATS);
		}
	}
	
	/*
	 * Author:Accolite
	 * Date : 21th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release
	 */

	public static List<FileStats> getFileStats(
			String productInstanceName, int instanceId, long numHistoryMinutes,
			Date toDateTime) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getFileStats(productInstanceName, instanceId, numHistoryMinutes, toDateTime);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_FILE_STATS);
		}
	}
	/*
	 * Author:Accolite
	 * Date : 21th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release
	 * @param productInstanceName
	 * @param instanceId
	 * @param numHistoryMinutes
	 * @return
	 * @throws CategoryException
	 */
	
	public static VirtualizationStatsDetails getVirtualizationStatDetails(
			String productInstanceName, int instanceId, long numHistoryMinutes) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getVirtualizationStatDetails(productInstanceName, instanceId, numHistoryMinutes);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_VIRTUALIZATION_STATS);
		}
	}
	
	/*
	 * Author:Accolite
	 * Date : 21th Dec, 2016
	 * Overview Graph Customization - SQLDM- 10.2 release
	 * @param productInstanceName
	 * @param instanceId
	 * @param numHistoryMinutes
	 * @param toDateTime
	 * @return
	 * @throws CategoryException
	 */
		public static VirtualizationStatsDetails getVirtualizationStatDetails(
			String productInstanceName, int instanceId, long numHistoryMinutes,	Date toDateTime) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getVirtualizationStatDetails(productInstanceName, instanceId, numHistoryMinutes,toDateTime);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_FETCHING_VIRTUALIZATION_STATS);
		}
	}
}
