package com.idera.sqldm.ui.dashboard;

import java.util.Comparator;
import java.util.List;

import org.zkoss.zul.GroupsModelArray;

/*
 * An array implementation of GroupsModel. This implementation takes a list of elements that are not grouped yet, and a comparator that will be used to group them. The c supports regroup array to groups depends on Comparator and GroupComparator. For immutable content (no re-grouping allowed), please use SimpleGroupsModel instead.

Generics:

D
The class of each data
H
The class of each group header
F
The class of each group footer
E
The class of each selection. It is the common base class of D, H, F. In other words, D, H and F must extend from E.
 * 
 * 
 * 
 * */
public class InstanceGroupModel extends GroupsModelArray<DashboardInstanceWrapper, Object, Object, Object>{

	private static final long serialVersionUID = 1L;
	private boolean showGroup;
	public InstanceGroupModel(List<DashboardInstanceWrapper> data, Comparator<DashboardInstanceWrapper> cmpr, boolean showGroup) {
        super(data.toArray(new DashboardInstanceWrapper[0]), cmpr);
        this.showGroup = showGroup;
    }
	
    protected String createGroupHead(DashboardInstanceWrapper[] groupdata, int index, int col) {
        String ret = "";
        if (groupdata.length > 0) {
        	 if (this._comparator instanceof InstanceSeverityComparator) {
        		 ret = groupdata[0].getSeverityString();
        	 }
        	 else if(this._comparator instanceof InstanceTagComparator){
        		 ret = groupdata[0].getTag();
        	 }
        	 else if(this._comparator instanceof InstanceRepoComparator){
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

}
