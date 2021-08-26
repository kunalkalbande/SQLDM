package com.idera.sqldm.ui.preferences;

import com.idera.sqldm.ui.dashboard.instances.resources.FileActivityFilters;

public class SingleInstancePreferencesBean {
	
	public final static String SESSION_VARIABLE__NAME = "SingleInstanceSessionDataBean";
	
	private final int instanceId;
	private FileActivityFilters fileActivityFilters;
	//just to make sure that 0 is also important here rather -1 mark it.
	private int selectedCategory = 0;
	private int selectedSubCategory = 1;
	
	//It can only be initialized from PreferncesUtil
	SingleInstancePreferencesBean(int instanceId){
		this.instanceId = instanceId;
	}
	public int getInstanceId() {
		return instanceId;
	}
	/*public void setInstanceId(int instanceId) {
		this.instanceId = instanceId;
	}*/
	public FileActivityFilters getFileActivityFilters() {
		return fileActivityFilters;
	}
	public void setFileActivityFilters(FileActivityFilters fileActivityFilters) {
		this.fileActivityFilters = fileActivityFilters;
	}
	public int getSelectedCategory() {
		return selectedCategory;
	}
	public void setSelectedCategory(int selectedCategory) {
		this.selectedCategory = selectedCategory;
	}
	public int getSelectedSubCategory() {
		return selectedSubCategory;
	}
	public void setSelectedSubCategory(int selectedSubCategory) {
		this.selectedSubCategory = selectedSubCategory;
	}
}
