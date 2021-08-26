package com.idera.sqldm_10_3.ui.dashboard;


public class InstancesColumns {

	private String value;
	private String width;
	private Boolean isMandatoryColumn;
	private String template;

	public String getWidth() {
		return width;
	}

	public void setWidth(String width) {
		this.width = width;
	}

	public String getTemplate() {
		return template;
	}

	public void setTemplate(String labelType) {
		this.template = labelType;
	}

	public Boolean getDefaultFlag() {
		return isMandatoryColumn;
	}

	public void setDefaultFlag(boolean flag) {
		this.isMandatoryColumn = flag;
	}

	public InstancesColumns(String name, String width, boolean isMandatory, String labelType) {
		this.value = name;
		this.width = width;
		this.isMandatoryColumn = isMandatory;
		this.template = labelType;
	}

	public String getValue() {
		return value;
	}

	public void setValue(String name) {
		this.value = name;
	}
}