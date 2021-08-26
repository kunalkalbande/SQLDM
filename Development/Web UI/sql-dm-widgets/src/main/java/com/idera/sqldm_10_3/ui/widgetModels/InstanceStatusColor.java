package com.idera.sqldm_10_3.ui.widgetModels;

public class InstanceStatusColor {

	private String type;
	
	private int count;
	
	public InstanceStatusColor(){
		
	}
	
	public InstanceStatusColor(String type, int count){
		this.type=type;
		this.count=count;
	}

	public String getType() {
		return type;
	}

	public void setType(String type) {
		this.type = type;
	}

	public int getCount() {
		return count;
	}

	public void setCount(int count) {
		this.count = count;
	}
}
