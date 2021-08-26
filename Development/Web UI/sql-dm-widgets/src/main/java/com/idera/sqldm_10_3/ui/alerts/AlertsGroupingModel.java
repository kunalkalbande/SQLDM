package com.idera.sqldm_10_3.ui.alerts;

import com.idera.sqldm_10_3.data.alerts.Alert;
import com.idera.sqldm_10_3.data.alerts.Alert.AlertSeverity;
import org.zkoss.zul.GroupsModelArray;

import java.util.Comparator;
import java.util.List;

public class AlertsGroupingModel extends GroupsModelArray<Alert, String, String, Object>{
	
	public enum GroupBy{
		INSTANCE(2),
		METRIC(3), 
		SEVERITY(1),
		ACTIVE(0),
		CATEGORY(4),
		CUSTOM(6),
		PRODUCT(5);
		
		private int index;
		private GroupBy(int index){
			this.index = index;
		}
		public int getIndex() {
			return index;
		}
		public void setIndex(int index) {
			this.index = index;
		}
	}

	private static final long serialVersionUID = 1L;
	//private static final String footerString = "Total %d items";
	private boolean showGroup;
	
	public AlertsGroupingModel(List<Alert> data, Comparator<Alert> cmpr, boolean showGroup) {
        super(data.toArray(new Alert[0]), cmpr);
        this.showGroup = showGroup;
    }
	
    protected String createGroupHead(Alert[] groupdata, int index, int col) {
        String ret = "";
        AlertsComparator comp = (AlertsComparator) this._comparator;
        if (groupdata.length > 0) {
        	 if (comp.getSortBy() == AlertSortByColumns.INSTANCE) {
//        		 ret = groupdata[0].getInstanceName(); @author Saumyadeep
        		 ret = groupdata[0].getDisplayName(); 
        		 
        	 }
        	 else if(comp.getSortBy() == AlertSortByColumns.SEVERITY) {
        		 
        		 switch(groupdata[0].getSeverity()){
        		 case 1 : ret = AlertSeverity.OK.name();break;
        		 case 2 : ret = AlertSeverity.INFORMATIONAL.name();break;
        		 case 4 : ret = AlertSeverity.WARNING.name();break;
        		 case 8 : ret = AlertSeverity.CRITICAL.name();break;
        		 }
        		 //ret = groupdata[0].getSeverity();
        	 }
        	 else if(comp.getSortBy() == AlertSortByColumns.METRIC) {
        		 ret = groupdata[0].getMetric().getName();
        	 }
        	 else if(comp.getSortBy() == AlertSortByColumns.CATEGORY) {
        		 ret = groupdata[0].getMetric().getMetricCategory();
        	 }
        	 else if(comp.getSortBy() == AlertSortByColumns.PRODUCT) {
        		 ret = groupdata[0].getProduct().getInstanceName();
        	 }
			ret += " ("+groupdata.length;
			if(groupdata.length > 1){
				ret +=" items)";
			}
			else{
				ret +=" item)";
			}
        }
 
        return ret;
    }
    
    /*protected String createGroupFoot(Alert[] groupdata, int index, int col) {
        return String.format(footerString, groupdata.length);
    }
 
    public boolean hasGroupfoot(int groupIndex) {
        boolean retBool = false;
         
        if(showGroup) {
            retBool = super.hasGroupfoot(groupIndex);
        }
         
        return retBool;
    }*/
}


