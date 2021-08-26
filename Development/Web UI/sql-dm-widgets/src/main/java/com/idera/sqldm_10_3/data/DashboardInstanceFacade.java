package com.idera.sqldm_10_3.data;

import com.idera.common.rest.RestException;
import com.idera.sqldm_10_3.data.instances.ServerStatus;
import com.idera.sqldm_10_3.i18n.SQLdmI18NStrings;
import com.idera.sqldm_10_3.rest.SQLDMRestClient;
import com.idera.sqldm_10_3.rest.UnauthorizedAccessException;
import com.idera.sqldm_10_3.ui.dashboard.DashboardInstanceWrapper;
import com.idera.sqldm_10_3.utils.Comparators;
import com.idera.sqldm_10_3.rest.SQLDMRestClient;
import com.idera.sqldm_10_3.ui.dashboard.DashboardInstanceWrapper;
import com.idera.sqldm_10_3.utils.Comparators;

import java.util.*;

public class DashboardInstanceFacade {

	public static ServerStatus getDashboardInstanceServerStatus(String productInstanceName, int instanceId, int numHistoryMinutes)
			throws InstanceException {
		try {
			return SQLDMRestClient.getInstance().getDashboardInstanceServerStats(productInstanceName,instanceId, numHistoryMinutes );
		} catch (RestException x) {
			throw new InstanceException(x,
					SQLdmI18NStrings.FAILED_TO_GET_DASHBOARD_INSTANCE_SERVER_STATUS);
		}
	}
	
	/*
	 * ChaitanyaTanwar DM 10.2
	 * */
	public static ServerStatus getDashboardInstanceServerStatus(String productInstanceName, int instanceId, long numHistoryMinutes, Date endTime)
			throws InstanceException {
		try {
			return SQLDMRestClient.getInstance().getDashboardInstanceServerStats(productInstanceName,instanceId, numHistoryMinutes,endTime );
		} catch (RestException x) {
			throw new InstanceException(x,
					SQLdmI18NStrings.FAILED_TO_GET_DASHBOARD_INSTANCE_SERVER_STATUS);
		}
	}

	public static List<DashboardInstance> getDashboardInstances(String productInstanceName)
			throws InstanceException {
		try {
			List<DashboardInstance> dashBoardInstances = new LinkedList<>();
			List<DashboardInstance> originalList = SQLDMRestClient.getInstance().getDashboardInstances(productInstanceName);
			for(DashboardInstance instance: originalList) {
				if(instance != null && instance.getOverview() != null && instance.getOverview().getDisplayName() != null) {
					dashBoardInstances.add(instance);
				}
			}
			Collections.sort(dashBoardInstances, DASHBOARD_INSTANCE_STATUS_STRING_COMPARATOR_DESC);
			return dashBoardInstances;
			
		} catch (RestException x) {
			throw new InstanceException(x,
					SQLdmI18NStrings.FAILED_TO_GET_DASHBOARD_INSTANCES);
		}
	}
	public static List<DashboardInstance> getDashboardInstances(String productInstanceName, String type, String value) throws InstanceException {
		try
		{
			List<DashboardInstance> dashBoardInstances = new LinkedList<>();
			List<DashboardInstance> originalList = SQLDMRestClient.getInstance().getDashboardInstances(productInstanceName,type, value);
			for(DashboardInstance instance: originalList) {
				if(instance != null && instance.getOverview() != null 
						&& instance.getOverview().getDisplayName() != null && !"".equals(instance.getOverview().getDisplayName())) {
					dashBoardInstances.add(instance);
				}
			}
			return dashBoardInstances;
		}
		catch (RestException x)
		{
			throw new InstanceException(x, SQLdmI18NStrings.FAILED_TO_GET_DASHBOARD_INSTANCES);
		}
	}
	
	public static List<ServerStatus> getInstancesSummary(String productInstanceName)
			throws InstanceException {
		try {
			return SQLDMRestClient.getInstance().getInstancesSummary(productInstanceName);
		} catch (RestException x) {
			throw new InstanceException(x,
					SQLdmI18NStrings.FAILED_TO_GET_DASHBOARD_INSTANCES);
		}
	}

	public static boolean areInstancesRegistered(String productInstanceName) {
		try {
			List<DashboardInstance> list = SQLDMRestClient.getInstance()
					.getDashboardInstances(productInstanceName);
			return ((list != null) ? !list.isEmpty() : false);
		} catch (Exception e) {
			return true;
		}
	}

	private static int compareName(DashboardInstance di1,
                                   DashboardInstance di2, boolean asc) {
		int ret = 0;

		if (di1 != null && di2 != null && di1.getOverview().getInstanceName() != null
				&& di2.getOverview().getInstanceName() != null) {

			ret = di1.getOverview().getInstanceName().toLowerCase()
					.compareTo(di2.getOverview().getInstanceName().toLowerCase());
		}
		
		return (ret * (asc ? 1 : -1));
	}
	// @author Saumyadeep
	// Friendly Begin	
	private static int compareFriendlyServerName(DashboardInstance di1,
                                                 DashboardInstance di2, boolean asc) {
		int ret = 0;

		if (di1 != null && di2 != null && di1.getOverview().getFriendlyServerName() != null
				&& di2.getOverview().getFriendlyServerName() != null) {

			ret = di1.getOverview().getFriendlyServerName().toLowerCase()
					.compareTo(di2.getOverview().getFriendlyServerName().toLowerCase());
		}
		
		return (ret * (asc ? 1 : -1));
	}
	
	private static int compareDisplayName(DashboardInstance di1,
                                          DashboardInstance di2, boolean asc) {
		int ret = 0;

		if (di1 != null && di2 != null && di1.getOverview().getDisplayName() != null
				&& di2.getOverview().getDisplayName() != null) {

			ret = di1.getOverview().getDisplayName().toLowerCase()
					.compareTo(di2.getOverview().getDisplayName().toLowerCase());
		}
		
		return (ret * (asc ? 1 : -1));
	}	
	//Friendly End;
	
	public static final Comparator<DashboardInstance> DASHBOARD_INSTANCE_STATUS_STRING_COMPARATOR_ASC = new Comparator<DashboardInstance>() {
		@Override
		public int compare(DashboardInstance di1, DashboardInstance di2) {

			return compareStatus(di1, di2, true);
		}
	};

	public static final Comparator<DashboardInstance> DASHBOARD_INSTANCE_STATUS_STRING_COMPARATOR_DESC = new Comparator<DashboardInstance>() {
		@Override
		public int compare(DashboardInstance di1, DashboardInstance di2) {

			return compareStatus(di1, di2, false);
		}
	};

	private static int compareStatus(DashboardInstance di1,
                                     DashboardInstance di2, boolean asc) {
		int ret = 0;

		if (di1 != null && di2 != null) {

			ret = di1.getServerStatus().getMaxSeverity()
					.compareTo(di2.getServerStatus().getMaxSeverity());

			// for instances with the same status, sort by name next
			if (ret == 0) {
//				 return compareName(di1, di2, true); @author Saumyadeep
				return compareDisplayName(di1, di2, true);
			}
		}

		return (ret * (asc ? 1 : -1));
	}
//
//	public static final Comparator<DashboardInstance> DASHBOARD_INSTANCE_RESPONSETIME_STRING_COMPARATOR_ASC = new Comparator<DashboardInstance>() {
//		@Override
//		public int compare(DashboardInstance di1, DashboardInstance di2) {
//
//			return compareResponseTime(di1, di2, true);
//		}
//	};
//
//	public static final Comparator<DashboardInstance> DASHBOARD_INSTANCE_RESPONSETIME_STRING_COMPARATOR_DESC = new Comparator<DashboardInstance>() {
//		@Override
//		public int compare(DashboardInstance di1, DashboardInstance di2) {
//
//			return compareResponseTime(di1, di2, false);
//		}
//	};
//
//	private static int compareResponseTime(DashboardInstance di1,
//			DashboardInstance di2, boolean asc) {
//		int ret = 0;
//
//		if (di1 != null && di2 != null) {
//
//			ret = di1.getResponseTimeSortValue().compareTo(
//					di2.getResponseTimeSortValue());
//		}
//
//		return (ret * (asc ? 1 : -1));
//	}
//
//	public static final Comparator<DashboardInstance> DASHBOARD_INSTANCE_VERSION_STRING_COMPARATOR_ASC = new Comparator<DashboardInstance>() {
//		@Override
//		public int compare(DashboardInstance di1, DashboardInstance di2) {
//
//			return compareVersion(di1, di2, true);
//		}
//	};
//
//	public static final Comparator<DashboardInstance> DASHBOARD_INSTANCE_VERSION_STRING_COMPARATOR_DESC = new Comparator<DashboardInstance>() {
//		@Override
//		public int compare(DashboardInstance di1, DashboardInstance di2) {
//
//			return compareVersion(di1, di2, false);
//		}
//	};
//
//	private static int compareVersion(DashboardInstance di1, DashboardInstance di2, boolean asc) {
//		int ret = 0;
//
//		if (di1 != null && di2 != null) {
//
//			ret = InstanceHelper.getVersionSortValue(di1.getSqlVersion())
//					.compareTo(
//							InstanceHelper.getVersionSortValue(di2
//									.getSqlVersion()));
//		}
//
//		return (ret * (asc ? 1 : -1));
//	}
//	public static final Comparator<DashboardInstance> DASHBOARD_INSTANCE_SEVERITY_COMPARATOR_ASC = new Comparator<DashboardInstance>() {
//		@Override
//		public int compare(DashboardInstance di1, DashboardInstance di2) {
//
//			return compareVersion(di1, di2, true);
//		}
//	};

	public static final Comparator<DashboardInstance> DASHBOARD_INSTANCE_SEVERITY_COMPARATOR_DESC = new Comparator<DashboardInstance>() {
		@Override
		public int compare(DashboardInstance di1, DashboardInstance di2) {

			return compareSeverity(di1, di2, false);
		}
	};

	private static int compareSeverity(DashboardInstance di1,
                                       DashboardInstance di2, boolean asc) {
		int ret = 0;
		if(di1 == null && di2 != null) {
			return (-1 * (asc ? 1 : -1));
		} else if(di1 != null && di2 == null) {
			return (1 * (asc ? 1 : -1));
		} else if(di1 == null && di2 == null) {
			return (0);
		} else {
			if (di1.getServerStatus().getMaxSeverity() != null && di2.getServerStatus().getMaxSeverity() != null) {				
				ret = di1.getServerStatus().getMaxSeverity().compareTo(
						di2.getServerStatus().getMaxSeverity());
			} 

			return (ret * (asc ? 1 : -1));
		}
	}

	public static DashboardInstance getDashboardInstance(String productInstanceName, int instanceId)
			throws InstanceException, RestException {
		try {
			return SQLDMRestClient.getInstance().getDashboardInstance(productInstanceName, instanceId);
		}catch(UnauthorizedAccessException x) {
			throw x;
		} catch (RestException x) {
			throw new InstanceException(x,
					SQLdmI18NStrings.FAILED_TO_GET_DASHBOARD_INSTANCE);
		}
	}

	public static final Comparator<DashboardInstance> DASHBOARD_INSTANCE_BLOCKED_PROCESSES_COMPARATOR_ASC = new Comparator<DashboardInstance>() {
		@Override
		public int compare(DashboardInstance di1, DashboardInstance di2) {

			return compareBlockedProcesses(di1, di2, true);
		}
	};

	public static final Comparator<DashboardInstance> DASHBOARD_INSTANCE_BLOCKED_PROCESSES_COMPARATOR_DESC = new Comparator<DashboardInstance>() {
		@Override
		public int compare(DashboardInstance di1, DashboardInstance di2) {

			return compareBlockedProcesses(di1, di2, false);
		}
	};
	
	public static final Comparator<DashboardInstanceWrapper> DASHBOARD_INSTANCE_GROUP_BLOCKED_PROCESSES_COMPARATOR_ASC = new Comparator<DashboardInstanceWrapper>() {
		@Override
		public int compare(DashboardInstanceWrapper di1, DashboardInstanceWrapper di2) {
			try{
				return di1.getBlockedProcesses().compareTo(di2.getBlockedProcesses());	
			}
			catch(Exception ex){
				return 0;
			}
		}
	};

	public static final Comparator<DashboardInstanceWrapper> DASHBOARD_INSTANCE_GROUP_BLOCKED_PROCESSES_COMPARATOR_DESC = new Comparator<DashboardInstanceWrapper>() {
		@Override
		public int compare(DashboardInstanceWrapper di1, DashboardInstanceWrapper di2) {
			try{
				return di2.getBlockedProcesses().compareTo(di1.getBlockedProcesses());	
			}
			catch(Exception ex){
				return 0;
			}
		}
	};
	
	public static final Comparator<DashboardInstance> DASHBOARD_INSTANCE_CPU_ACTIVITY_COMPARATOR_ASC = new Comparator<DashboardInstance>() {
		@Override
		public int compare(DashboardInstance o1, DashboardInstance o2) {
			return compareCpuUsage(o1, o2, true);
		}
	};
	
	public static final Comparator<DashboardInstance> DASHBOARD_INSTANCE_CPU_ACTIVITY_COMPARATOR_DESC = new Comparator<DashboardInstance>() {
		@Override
		public int compare(DashboardInstance o1, DashboardInstance o2) {
			return compareCpuUsage(o1, o2, false);
		}
	};
	
	public static final Comparator<DashboardInstanceWrapper> DASHBOARD_INSTANCE_GROUP_CPU_ACTIVITY_COMPARATOR_ASC = new Comparator<DashboardInstanceWrapper>() {
		@Override
		public int compare(DashboardInstanceWrapper o1, DashboardInstanceWrapper o2) {
			return compareCpuUsage(o1, o2, true);
		}
	};
	
	public static final Comparator<DashboardInstanceWrapper> DASHBOARD_INSTANCE_GROUP_CPU_ACTIVITY_COMPARATOR_DESC = new Comparator<DashboardInstanceWrapper>() {
		@Override
		public int compare(DashboardInstanceWrapper o1, DashboardInstanceWrapper o2) {
			return compareCpuUsage(o1, o2, false);
		}
	};
	
	private static int compareCpuUsage(DashboardInstance o1, DashboardInstance o2, boolean asc) {
		Double c1 = null;
		Double c2 = null;
		if (o1 != null && o1.getOverview() != null && o1.getOverview().getStatistics() != null) {
			c1 = o1.getOverview().getStatistics().getCpuPercentage();
		}
		
		if (o2 != null && o2.getOverview() != null && o2.getOverview().getStatistics() != null ) {
			c2 = o2.getOverview().getStatistics().getCpuPercentage();				
		}
		
		int factor = asc ? 1:-1; 
		
		if (c1 != null && c2 != null) {
			return factor*c1.compareTo(c2);
		} else if (c1 == null && c2 != null) {
			return factor*1;
		} else if (c1 != null && c2 == null) {
			return factor*(-1);
		} else {//both null
			return 0;
		}		
	}
	
	private static int compareCpuUsage(DashboardInstanceWrapper o1, DashboardInstanceWrapper o2, boolean asc) {
		Double c1 = null;
		Double c2 = null;
		if (o1 != null && o1.getCpuActivity() != null ) {
			c1 = o1.getCpuActivity();
		}
		
		if (o2 != null && o2.getCpuActivity() != null ) {
			c2 = o2.getCpuActivity();
		}
		
		int factor = asc ? 1:-1; 
		
		if (c1 != null && c2 != null) {
			return factor*c1.compareTo(c2);
		} else if (c1 == null && c2 != null) {
			return factor*1;
		} else if (c1 != null && c2 == null) {
			return factor*(-1);
		} else {//both null
			return 0;
		}		
	}
	
	public static final Comparator<DashboardInstance> DASHBOARD_INSTANCE_AVAILABLE_MEM_COMPARATOR_ASC = new Comparator<DashboardInstance>() {
		@Override
		public int compare(DashboardInstance o1, DashboardInstance o2) {
			String c1 = o1.getOverview().getOsMetricsStatistics().getAvailableBytes();
			String c2 = o2.getOverview().getOsMetricsStatistics().getAvailableBytes();
			return Comparators.compareLogicStringNumerals(c1, c2);
		}
	};
	
	public static final Comparator<DashboardInstance> DASHBOARD_INSTANCE_AVAILABLE_MEM_COMPARATOR_DESC = new Comparator<DashboardInstance>() {
		@Override
		public int compare(DashboardInstance o1, DashboardInstance o2) {
			String c1 = o1.getOverview().getOsMetricsStatistics().getAvailableBytes();
			String c2 = o2.getOverview().getOsMetricsStatistics().getAvailableBytes();
			return Comparators.compareLogicStringNumerals(c2, c1);
		}
	};

	public static final Comparator<DashboardInstanceWrapper> DASHBOARD_INSTANCE_GROUP_AVAILABLE_MEM_COMPARATOR_DESC = new Comparator<DashboardInstanceWrapper>() {
		@Override
		public int compare(DashboardInstanceWrapper o1, DashboardInstanceWrapper o2) {
			Double c1 = o1.getAvailableMemory();
			Double c2 = o2.getAvailableMemory();
			return c2.compareTo(c1);
		}
	};
	
	public static final Comparator<DashboardInstanceWrapper> DASHBOARD_INSTANCE_GROUP_AVAILABLE_MEM_COMPARATOR_ASC = new Comparator<DashboardInstanceWrapper>() {
		@Override
		public int compare(DashboardInstanceWrapper o1, DashboardInstanceWrapper o2) {
			Double c1 = o1.getAvailableMemory();
			Double c2 = o2.getAvailableMemory();
			return c1.compareTo(c2);
		}
	};
	private static int compareBlockedProcesses(DashboardInstance di1, DashboardInstance di2, boolean asc) {
		int ret = 0;
		int diStatus = checkObjectsForNull(di1, di2); 
		if(diStatus != 100) {
			return (diStatus * ((asc ? 1 : -1)));
		} else {
			int overviewStatus = checkObjectsForNull(di1.getOverview(), di2.getOverview());
			if(overviewStatus != 100) {
				return (overviewStatus * ((asc ? 1 : -1)));
			} else {
				int systemProcessesStatus = checkObjectsForNull(di1.getOverview().getSystemProcesses(), di2.getOverview().getSystemProcesses());
				if(systemProcessesStatus != 100) {
					return (systemProcessesStatus * ((asc ? 1 : -1)));
				} else {
					int blockedProcessesStatus = checkObjectsForNull(di1.getOverview().getSystemProcesses().get("blockedProcesses"), di2.getOverview().getSystemProcesses().get("blockedProcesses"));
					if(blockedProcessesStatus != 100) {
						return (blockedProcessesStatus * ((asc ? 1 : -1)));
					}else {
						ret = di1.getOverview().getSystemProcesses().get("blockedProcesses").compareTo(
								di2.getOverview().getSystemProcesses().get("blockedProcesses"));

						return (ret * (asc ? 1 : -1));						
					}
				}
			}
		}
	}
	//Returns -1 is Object 1 is null
	//Returns 1 is Object 2 is null
	//Returns 0 is both Objects are null
	//Returns 100 is both Objects are not null
	private static int checkObjectsForNull(Object o1, Object o2) {
		if(o1 == null && o2 != null) {
			return (-1);
		} else if(o1 != null && o2 == null) {
			return (1);
		} else if(o1 == null && o2 == null) {
			return (0);
		}		
		return 100;
	}
}
