package com.idera.sqldm.ui.dashboard.instances.resources;

import org.apache.commons.lang.StringUtils;

import com.idera.sqldm.ui.dashboard.instances.resources.FileActivityViewModel.FileTypes;

public class FileActivityModel implements Comparable<FileActivityModel>{

	private String databaseName;
	private String drive;
	private String fileName;
	private String fileType;
	private String filePath;
	private Double readsPerSec;
	private Double writesPerSec;
	
	public FileActivityModel() {
	}
	
	public FileActivityModel(String database, String drive, String fileName, String type){
		this.databaseName = database;
		this.drive = drive;
		this.fileName = fileName;
		this.fileType = type;
	}
	
	public String getDatabaseName() {
		return databaseName;
	}
	public void setDatabaseName(String databaseName) {
		this.databaseName = databaseName;
	}
	public String getDrive() {
		return drive;
	}
	public void setDrive(String drive) {
		this.drive = drive;
	}
	public String getFileName() {
		return fileName;
	}
	public void setFileName(String fileName) {
		this.fileName = fileName;
	}
	public String getFileType() {
		if(StringUtils.isEmpty(fileType)){
			if(fileName.endsWith("_Data") || fileName.endsWith("_data") || fileName.endsWith("_dat")){
				return FileTypes.data.getName();
			}
			else if(fileName.endsWith("_Log") || fileName.endsWith("_log") || fileName.contains("_log_")
					|| fileName.contains("_Log_")){
				return FileTypes.log.getName();
			}
			else{
				return FileTypes.other.getName();
			}
		}
		return fileType;
	}
	public void setFileType(String fileType) {
		this.fileType = fileType;
	}
	
	public String getFilePath() {
		return filePath;
	}

	public void setFilePath(String filePath) {
		this.filePath = filePath;
	}

	public Double getReadsPerSec() {
		return readsPerSec;
	}

	public void setReadsPerSec(Double readsPerSec) {
		this.readsPerSec = readsPerSec;
	}

	public Double getWritesPerSec() {
		return writesPerSec;
	}

	public void setWritesPerSec(Double writesPerSec) {
		this.writesPerSec = writesPerSec;
	}

	@Override
	public boolean equals(Object obj) {
		if (obj == this) return true;
		if (obj == null || (obj.getClass() != this.getClass())) return false;
		 
		 FileActivityModel other = (FileActivityModel) obj;
		 //StringUtils.
		return (databaseName == other.databaseName 
                || (databaseName != null && databaseName.equals(other.getDatabaseName())))
           && (drive == other.drive 
                   || (drive != null && drive.equals(other.getDrive())))
           && (fileName == other.fileName 
                   || (fileName != null && fileName.equals(other.getFileName())))
           && (fileType == other.fileType 
                   || (fileType != null && fileType.equals(other.getFileType())));
	}
	
	@Override
	public int hashCode() {
		final int prime = 31;
        int result = 1;
        result = prime * result
                + ((databaseName == null) ? 0 : databaseName.hashCode());
        result = prime * result
                + ((drive == null) ? 0 : drive.hashCode());
        result = prime * result
                + ((fileName == null) ? 0 : fileName.hashCode());
        result = prime * result
                + ((fileType == null) ? 0 : fileType.hashCode());
        return result;
	}

	@Override
	public int compareTo(FileActivityModel o) {
		return this.databaseName.compareTo(o.databaseName);
	}

}
