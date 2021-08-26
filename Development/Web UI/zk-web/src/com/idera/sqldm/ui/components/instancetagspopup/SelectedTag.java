package com.idera.sqldm.ui.components.instancetagspopup;

import com.idera.core.tags.Tag;

public class SelectedTag extends Tag {

	private SelectedState selectedState = SelectedState.NOT_SELECTED;

	public SelectedState getSelectedState() {
		return selectedState;
	}

	public void setSelectedState(SelectedState selectedState) {
		this.selectedState = selectedState;
	}

	private boolean isChanged;

	public boolean isChanged() {
		return isChanged;
	}

	public void setChanged(boolean changed) {
		isChanged = changed;
	}

	public SelectedTag(String name) {
		super(name);
	}

	public SelectedTag(String name, SelectedState selectedState) {
		setName(name);
		this.selectedState = selectedState;
	}

	public SelectedTag(String name, SelectedState selectedState,
			boolean isChanged) {
		setName(name);
		this.selectedState = selectedState;
		this.isChanged = isChanged;
	}
}
