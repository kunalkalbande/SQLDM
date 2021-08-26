package com.idera.sqldm_10_3.ui.dashboard.instances.resources;

import com.idera.sqldm_10_3.data.category.resources.FileDrive;
import com.idera.sqldm_10_3.data.databases.InstanceDetailsDatabase;

import java.util.HashSet;
import java.util.Set;

public class FileActivityFilters implements Cloneable{
	final static String DEFAULT_SORT_BY = "databaseName";
	final static String DEFAULT_SORT_DIRECTION = "ascending";
	
	private String databaseBoxValue;
	private String driveBoxValue;
	private Set<String> fileTypes = new HashSet<>();
	private String filePathLike;
	private String fileNameLike;
	private String pickedSortByName = DEFAULT_SORT_BY;
	private String pickedSortDirection = DEFAULT_SORT_DIRECTION;
	private Set<InstanceDetailsDatabase> selectedDatabases;
	private Set<FileDrive> selectedDrives;
	
	public String getDatabaseBoxValue() {
		return databaseBoxValue;
	}
	public void setDatabaseBoxValue(String databaseBoxValue) {
		this.databaseBoxValue = databaseBoxValue;
	}
	public String getDriveBoxValue() {
		return driveBoxValue;
	}
	public void setDriveBoxValue(String driveBoxValue) {
		this.driveBoxValue = driveBoxValue;
	}
	public Set<String> getFileTypes() {
		return fileTypes;
	}
	public void setFileTypes(Set<String> fileTypes) {
		this.fileTypes = fileTypes;
	}
	public String getFilePathLike() {
		return filePathLike;
	}
	public void setFilePathLike(String filePathLike) {
		this.filePathLike = filePathLike;
	}
	public String getFileNameLike() {
		return fileNameLike;
	}
	public void setFileNameLike(String fileNameLike) {
		this.fileNameLike = fileNameLike;
	}
	public String getPickedSortByName() {
		return pickedSortByName;
	}
	public void setPickedSortByName(String pickedSortByName) {
		this.pickedSortByName = pickedSortByName;
	}
	public String getPickedSortDirection() {
		return pickedSortDirection;
	}
	public void setPickedSortDirection(String pickedSortDirection) {
		this.pickedSortDirection = pickedSortDirection;
	}
	public Set<InstanceDetailsDatabase> getSelectedDatabases() {
		return selectedDatabases;
	}
	public void setSelectedDatabases(Set<InstanceDetailsDatabase> selectedDatabases) {
		this.selectedDatabases = selectedDatabases;
	}
	public Set<FileDrive> getSelectedDrives() {
		return selectedDrives;
	}
	public void setSelectedDrives(Set<FileDrive> selectedDrives) {
		this.selectedDrives = selectedDrives;
	}
	
	@Override
	protected Object clone() throws CloneNotSupportedException {
		return super.clone();
	}

}
