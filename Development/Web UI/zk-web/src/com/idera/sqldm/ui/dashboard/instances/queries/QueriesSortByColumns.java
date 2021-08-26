package com.idera.sqldm.ui.dashboard.instances.queries;

public enum QueriesSortByColumns {
	APPLICATION("Application"), DATABASE("Database"), CLIENT("Client"), 
	USER("User") , DEFAULT("Default");
	
	private String columnName;
	private QueriesSortByColumns(String columnName){
		this.columnName = columnName;
	}
	public String getColumnName() {
		return columnName;
	}
	public void setColumnName(String columnName) {
		this.columnName = columnName;
	}
}
