package com.idera.sqldm.data;

import org.apache.commons.lang.builder.EqualsBuilder;

public class Filter {

	private FilterTypes type;

	public FilterTypes getType() {
		return type;
	}

	public void setType(FilterTypes type) {
		this.type = type;
	}

	private String value;

	public String getValue() {
		return value;
	}

	public void setValue(String value) {
		this.value = value;
	}

	private int count;

	public int getCount() {
		return count;
	}

	public void setCount(int count) {
		this.count = count;
	}

	private boolean selected;

	public boolean isSelected() {
		return selected;
	}

	public void setSelected(boolean selected) {
		this.selected = selected;
	}

	public Filter(FilterTypes type, String value) {
		new Filter(type, value, false);
	}

	public Filter(FilterTypes type, String value, boolean selected) {
		this.type = type;
		this.value = value;
		this.selected = selected;
	}

	public Filter(FilterTypes type, String value, boolean selected, int count) {
		this.type = type;
		this.value = value;
		this.selected = selected;
		this.count = count;
	}

	// This private default constructor is required to support JSON
	// deserialization. It is not meant to be used by class consumers.
	@SuppressWarnings("unused")
	private Filter() {
	}

	@Override
	public boolean equals(Object obj) {
		if (obj == null)
			return false;
		if (obj == this)
			return true;
		if (obj.getClass() != getClass()) {
			return false;
		}
		Filter filter = (Filter) obj;
		return new EqualsBuilder().append(this.type, filter.getType())
				.append(this.value, filter.getValue()).isEquals();
	}

}
