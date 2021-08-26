package com.idera.sqldm.data.category.resources;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown=true)
public class FileDrive {

	@JsonProperty("DriveID") private Integer DriveID;
	@JsonProperty("DriveName") private String DriveName;
	public Integer getDriveID() {
		return DriveID;
	}
	public void setDriveID(Integer driveID) {
		DriveID = driveID;
	}
	public String getDriveName() {
		return DriveName;
	}
	public void setDriveName(String driveName) {
		DriveName = driveName;
	}
}
