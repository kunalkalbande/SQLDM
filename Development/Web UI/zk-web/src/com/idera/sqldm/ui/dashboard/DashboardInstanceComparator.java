package com.idera.sqldm.ui.dashboard;

import java.util.Comparator;

import com.idera.sqldm.data.DashboardInstance;

public class DashboardInstanceComparator implements
Comparator<DashboardInstance> {

	private String sortBy;
	private boolean ascending;

	public DashboardInstanceComparator(String sortBy, boolean ascending) {
		this.sortBy = sortBy;
		this.ascending = ascending;
	}

	@Override
	public int compare(DashboardInstance o1, DashboardInstance o2) {
		int v = 0;
		switch (sortBy){
		case "Instance" :
			v = o1.getDisplayName().compareTo(o2.getDisplayName());                
			break;
		case "Health":
			v = ((Double)o1.getServerStatus().getHealthIndex()).compareTo((Double)o2.getServerStatus().getHealthIndex());
			break;
		case "Alert":
			v = (o1.getServerStatus().getMaxSeverity()).compareTo(o2.getServerStatus().getMaxSeverity());
			break;
		case "CPU":
			v = (o1.getAlertCategoryWiseMaxSeverity().getCpu()).compareTo(o2.getAlertCategoryWiseMaxSeverity().getCpu());
			break;
		case "Memory":
			v = (o1.getAlertCategoryWiseMaxSeverity().getMemory()).compareTo(o2.getAlertCategoryWiseMaxSeverity().getMemory());
			break;
		case "I/O":
			v = (o1.getAlertCategoryWiseMaxSeverity().getIO()).compareTo(o2.getAlertCategoryWiseMaxSeverity().getIO());
			break;
		case "Databases":
			v = (o1.getAlertCategoryWiseMaxSeverity().getDatabases()).compareTo(o2.getAlertCategoryWiseMaxSeverity().getDatabases());
			break;
		case "Logs":
			v = (o1.getAlertCategoryWiseMaxSeverity().getLogs()).compareTo(o2.getAlertCategoryWiseMaxSeverity().getLogs());
			break;
		case "Queries":
			v = (o1.getAlertCategoryWiseMaxSeverity().getQueries()).compareTo(o2.getAlertCategoryWiseMaxSeverity().getQueries());
			break;
		case "Sessions":
			v = (o1.getAlertCategoryWiseMaxSeverity().getSessions()).compareTo(o2.getAlertCategoryWiseMaxSeverity().getSessions());
			break;
		case "Services":
			v = (o1.getAlertCategoryWiseMaxSeverity().getServices()).compareTo(o2.getAlertCategoryWiseMaxSeverity().getServices());
			break;
		case "Virtualization":
			v = (o1.getAlertCategoryWiseMaxSeverity().getVirtualization()).compareTo(o2.getAlertCategoryWiseMaxSeverity().getVirtualization());
			break;
		case "Operational":
			v = (o1.getAlertCategoryWiseMaxSeverity().getOperational()).compareTo(o2.getAlertCategoryWiseMaxSeverity().getOperational());
			break;
		case "Version":
			v = o1.getOverview().getProductVersion().compareTo(o2.getOverview().getProductVersion());
			break;
		case "Status":
			v = o1.getOverview().getSqlServiceStatus().compareTo(o2.getOverview().getSqlServiceStatus());
			break;
		case "DTC Status":
			v = o1.getOverview().getDtcServiceStatus().compareTo(o2.getOverview().getDtcServiceStatus());
			break;
		case "SWA" :
			v =  (o1.getIsSWAInstance() == true) ? 1:-1;
		default:
			break;
		}
		return ascending ? v : -v;
	}

}
