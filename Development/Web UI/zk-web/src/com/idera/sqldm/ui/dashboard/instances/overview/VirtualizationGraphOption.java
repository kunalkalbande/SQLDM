package com.idera.sqldm.ui.dashboard.instances.overview;

import java.util.ArrayList;
import java.util.List;

public class VirtualizationGraphOption {
	public enum VirtualizationGraphOptionEnum{
		VIRTUAL_MACHINE("Virtual Machine",1),
		HOST_MACHINE("VM Host",2);
		
		private int defaultSequence;
		private String optionName;
		
		VirtualizationGraphOptionEnum(String optionName, int defaultSequence){
			this.setOptionName(optionName);
			this.setDefaultSequence(defaultSequence);
		}
		
		
		public int getDefaultSequence() {
			return defaultSequence;
		}
		public void setDefaultSequence(int defaultSequence) {
			this.defaultSequence = defaultSequence;
		}
		public String getOptionName() {
			return optionName;
		}
		public void setOptionName(String optionName) {
			this.optionName = optionName;
		}
		
	}
	
	public static List<String> getDefaultOptionNameList() {
		// TODO Auto-generated method stub
		List<String> list = new ArrayList<String>();
		VirtualizationGraphOptionEnum[] values = VirtualizationGraphOptionEnum.values();
		for (VirtualizationGraphOptionEnum virtualGraphOption : values) {
			list.add(virtualGraphOption.getOptionName());
		}
		return list;
	}
	
	public static VirtualizationGraphOptionEnum getVirtualGraphOptionEnum(String categoryName){
		if(VirtualizationGraphOptionEnum.HOST_MACHINE.getOptionName().equalsIgnoreCase(categoryName)){
			return VirtualizationGraphOptionEnum.HOST_MACHINE;
		}
		else if(VirtualizationGraphOptionEnum.VIRTUAL_MACHINE.getOptionName().equalsIgnoreCase(categoryName)){
			return VirtualizationGraphOptionEnum.VIRTUAL_MACHINE;
		}
		else return null;
	}
}
