package com.idera.sqldm_10_3.ui.alerts;

import com.idera.sqldm_10_3.data.alerts.Alert;
import com.idera.sqldm_10_3.data.alerts.Metrics;
import org.zkoss.zul.GroupComparator;

import java.util.Comparator;

public class AlertsComparator implements Comparator<Alert>, GroupComparator<Alert>{

	private AlertSortByColumns sortBy;
	private boolean ascending;
	public AlertsComparator(AlertSortByColumns sortBy, boolean ascending){
		this.sortBy = sortBy;
		this.ascending = ascending;
	}
	
	@Override
	public int compare(Alert o1, Alert o2) {
		int v = 0;
        switch (sortBy) {
            case INSTANCE:
//            	v = o1.getInstanceName().compareTo(o2.getInstanceName()); @author Saumyadeep
                v = o1.getDisplayName().compareTo(o2.getDisplayName());                
                break;
            case PRODUCT:
                v = o1.getProduct().getInstanceName().compareTo(o2.getProduct().getInstanceName());
                break;
            case CATEGORY:
            	Metrics m1 = o1.getMetric();
            	Metrics m2 = o2.getMetric();
            	if(m1!=null && m2 !=null && m1.getName() !=null && m2.getName() !=null){	
            		v = m1.getMetricCategory().compareTo(m2.getMetricCategory());
            	}
                break;
            case SEVERITY:
            	v = o1.getSeverity().compareTo(o2.getSeverity());
            	break;
            case CURRENTDATE:
            	v = o1.getUtcUpdated().compareTo(o2.getUtcUpdated());
            	break;
            case DATABASE:
            	if(o1.getDatabaseName() == null || o2.getDatabaseName() == null){
            		v = 0;
            	}
            	else{
            		v = o1.getDatabaseName().compareTo(o2.getDatabaseName());
            	}
            	break;
            case METRIC:
            	 m1 = o1.getMetric();
            	 m2 = o2.getMetric();
            	
            	if(m1!=null && m2 !=null && m1.getName() !=null && m2.getName() !=null){
            		
            		v = m1.getName().compareTo(m2.getName());
            	}
                break;
        }
        return ascending ? v: -v;
	}
	
	@Override
	public int compareGroup(Alert o1, Alert o2) {
		int v = 0;
        switch (sortBy) {
            case INSTANCE:
//            	v = o1.getInstanceName().compareTo(o2.getInstanceName()); @author Saumyadeep
                v = o1.getDisplayName().compareTo(o2.getDisplayName());
                break;
            case PRODUCT:
                v = o1.getProduct().getInstanceName().compareTo(o2.getProduct().getInstanceName());
                break;
            case CATEGORY:
            	Metrics m1 = o1.getMetric();
            	Metrics m2 = o2.getMetric();
            	if(m1!=null && m2 !=null && m1.getName() !=null && m2.getName() !=null){	
            		v = m1.getMetricCategory().compareTo(m2.getMetricCategory());
            	}
                break;
            case SEVERITY:
            	v = o1.getSeverity().compareTo(o2.getSeverity());
            	break;
            case CURRENTDATE:
            	v = o1.getUtcUpdated().compareTo(o2.getUtcUpdated());
            	break;
            case DATABASE:
            	if(o1.getDatabaseName() == null || o2.getDatabaseName() == null){
            		v = 0;
            	}
            	else{
            		v = o1.getDatabaseName().compareTo(o2.getDatabaseName());
            	}
            	break;
            case METRIC:
            	
            	 m1 = o1.getMetric();
            	 m2 = o2.getMetric();
            	
            	if(m1!=null && m2 !=null && m1.getName() !=null && m2.getName() !=null){
            		
            		v = m1.getName().compareTo(m2.getName());
            	}
            	
                break;
        }
        return ascending ? v: -v;
	}

	public AlertSortByColumns getSortBy() {
		return sortBy;
	}

	public void setSortBy(AlertSortByColumns sortBy) {
		this.sortBy = sortBy;
	}

	public boolean isAscending() {
		return ascending;
	}

	public void setAscending(boolean ascending) {
		this.ascending = ascending;
	}
}
