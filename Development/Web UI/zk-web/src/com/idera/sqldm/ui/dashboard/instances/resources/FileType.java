package com.idera.sqldm.ui.dashboard.instances.resources;

public class FileType {

	private String name;
	private boolean checked;
	public FileType(String name, boolean checked){
		this.name = name;
		this.checked = checked;
	}
	
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	public boolean getChecked() {
		return checked;
	}
	public void setChecked(boolean checked) {
		this.checked = checked;
	}
}
