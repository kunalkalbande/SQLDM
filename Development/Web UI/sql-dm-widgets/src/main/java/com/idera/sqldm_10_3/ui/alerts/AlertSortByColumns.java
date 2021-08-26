package com.idera.sqldm_10_3.ui.alerts;

public enum AlertSortByColumns {
	SEVERITY("Severity"), DATABASE("DatabaseName"), CURRENTDATE("UTCOccurrenceDateTime"), 
	INSTANCE("ServerName"), CATEGORY("Category"), METRIC("Metric") , PRODUCT("ProductName");
	
	private String columnName;
	private AlertSortByColumns(String columnName){
		this.columnName = columnName;
	}
	public String getColumnName() {
		return columnName;
	}
	public void setColumnName(String columnName) {
		this.columnName = columnName;
	}
}
