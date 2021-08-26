package com.idera.sqldm.ui.customDashboard.widgets;

public enum CustomDashboardWidgetSizeEnum {

	small("350px","142px"),medium("350px","300px"),large("716px","300px");
	
	private String width;
	private String height;
	
	CustomDashboardWidgetSizeEnum(String width, String height) {
		this.width = width;
		this.height = height;
	}
	
	public String getWidth() {
		return this.width;
	}
	
	public String getHeight() {
		return this.height;
	}
}
