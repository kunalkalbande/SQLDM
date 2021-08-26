package com.idera.sqldm.ui.alerts;

public class CustomComboBoxModel {

	private Integer id;
	private String value;
	public CustomComboBoxModel(Integer id, String value) {
		this.id = id;
		this.value = value;
	}
	public Integer getId() {
		return id;
	}
	public void setId(Integer id) {
		this.id = id;
	}
	public String getValue() {
		return value;
	}
	public void setValue(String value) {
		this.value = value;
	}
}
