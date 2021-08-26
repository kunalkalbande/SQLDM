package com.idera.sqldm_10_3.data.category.resources;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

public class FileStats {
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	@JsonProperty("UTCCollectionDateTime") private Date UTCCollectionDateTime;
	@JsonProperty("DatebaseName") private String datebaseName;
	@JsonProperty("Drive") private String drive;
	@JsonProperty("FileName") private String fileName;
	@JsonProperty("FilePath") private String filePath;
	@JsonProperty("FileType") private String fileType;
	@JsonProperty("statistics") private FileStatsStatistics statistics;
	public Date getUTCCollectionDateTime() {
		return UTCCollectionDateTime;
	}
	public void setUTCCollectionDateTime(Date uTCCollectionDateTime) {
		UTCCollectionDateTime = uTCCollectionDateTime;
	}
	public String getDatebaseName() {
		return datebaseName;
	}
	public void setDatebaseName(String datebaseName) {
		this.datebaseName = datebaseName;
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
	public String getFilePath() {
		return filePath;
	}
	public void setFilePath(String filePath) {
		this.filePath = filePath;
	}
	public String getFileType() {
		return fileType;
	}
	public void setFileType(String fileType) {
		this.fileType = fileType;
	}
	public FileStatsStatistics getStatistics() {
		return statistics;
	}
	public void setStatistics(FileStatsStatistics statistics) {
		this.statistics = statistics;
	}	
}
