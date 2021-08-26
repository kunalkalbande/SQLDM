package com.idera.sqldm.ui.dashboard.instances.queries;


public class QueryColumns {

	private String width;
	private String value;
	private String labelType;
	private String template = "label";

	/*
	 * groupBy Filters 0 - Applications 1 - Database 2 - User 3 - Client 4 -
	 * Query Signature 5 - Query Statement
	 */
	public int[] groupByFilters = new int[6];

	public String getWidth() {
		return width;
	}

	public void setWidth(String width) {
		this.width = width;
	}

	public String getValue() {
		return value;
	}

	public void setValue(String value) {
		this.value = value;
	}

	public String getLabelType() {
		return labelType;
	}

	public void setLabelType(String labelType) {
		this.labelType = labelType;
	}

	public String getTemplate() {
		return template;
	}

	public void setTemplate(String template) {
		this.template = template;
	}

	public int[] getGroupByFilters() {
		return groupByFilters;
	}

	public void setGroupByFilters(int[] groupByFilters) {
		this.groupByFilters = groupByFilters;
	}

}