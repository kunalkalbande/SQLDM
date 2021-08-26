package com.idera.sqldm_10_3.ui.dashboard;

import java.util.Comparator;

public class DashboardInstanceWrapperComparator implements
Comparator<DashboardInstanceWrapper> {

	private String sortBy;
	private boolean ascending;

	public DashboardInstanceWrapperComparator(String sortBy, boolean ascending) {
		this.sortBy = sortBy;
		this.ascending = ascending;
	}

	@Override
	public int compare(DashboardInstanceWrapper o1, DashboardInstanceWrapper o2) {
		int v = 0;
		switch (sortBy){
		case "Instance" :
			v = o1.getDisplayName().compareTo(o2.getDisplayName());                
			break;
		case "Health":
			v = ((Double)o1.getHealthIndex()).compareTo((Double)o2.getHealthIndex());
			break;
		case "Alert":
			v = (o1.getSeverity()).compareTo(o2.getSeverity());
			break;
		case "CPU":
			v = (o1.getAlertCategoryWiseSeverity().getCpu()).compareTo(o2.getAlertCategoryWiseSeverity().getCpu());
			break;
		case "Memory":
			v = (o1.getAlertCategoryWiseSeverity().getMemory()).compareTo(o2.getAlertCategoryWiseSeverity().getMemory());
			break;
		case "I/O":
			v = (o1.getAlertCategoryWiseSeverity().getIO()).compareTo(o2.getAlertCategoryWiseSeverity().getIO());
			break;
		case "Databases":
			v = (o1.getAlertCategoryWiseSeverity().getDatabases()).compareTo(o2.getAlertCategoryWiseSeverity().getDatabases());
			break;
		case "Logs":
			v = (o1.getAlertCategoryWiseSeverity().getLogs()).compareTo(o2.getAlertCategoryWiseSeverity().getLogs());
			break;
		case "Queries":
			v = (o1.getAlertCategoryWiseSeverity().getQueries()).compareTo(o2.getAlertCategoryWiseSeverity().getQueries());
			break;
		case "Sessions":
			v = (o1.getAlertCategoryWiseSeverity().getSessions()).compareTo(o2.getAlertCategoryWiseSeverity().getSessions());
			break;
		case "Services":
			v = (o1.getAlertCategoryWiseSeverity().getServices()).compareTo(o2.getAlertCategoryWiseSeverity().getServices());
			break;
		case "Virtualization":
			v = (o1.getAlertCategoryWiseSeverity().getVirtualization()).compareTo(o2.getAlertCategoryWiseSeverity().getVirtualization());
			break;
		case "Operational":
			v = (o1.getAlertCategoryWiseSeverity().getOperational()).compareTo(o2.getAlertCategoryWiseSeverity().getOperational());
			break;
		case "Version":
			v = o1.getVersion().compareTo(o2.getVersion());
			break;
		case "Status":
			v = o1.getAgentStatus().compareTo(o2.getAgentStatus());
			break;
		case "DTC Status":
			v = o1.getDtcStatus().compareTo(o2.getDtcStatus());
			break;
		case "SWA" :
			v =  (o1.isSwaInstance() == true) ? 1:-1;
		default:
			break;
		}
		return ascending ? v : -v;
	}

}
