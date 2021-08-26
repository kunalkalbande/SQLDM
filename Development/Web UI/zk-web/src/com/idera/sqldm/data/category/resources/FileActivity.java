package com.idera.sqldm.data.category.resources;

import java.util.Date;

import org.apache.commons.lang.StringUtils;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

@JsonIgnoreProperties(ignoreUnknown=true)
public class FileActivity {
	@JsonProperty("DatebaseName") private String DatebaseName;
	@JsonProperty("Drive") private String Drive;
	@JsonProperty("FileName") private String FileName;
	
	@JsonProperty("FilePath") private String FilePath;
	@JsonProperty("FileType") private String FileType;
	
	@JsonProperty("UTCCollectionDateTime")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date UTCCollectionDateTime;
	@JsonProperty("statistics") private FileActivityStatistics statistics;
	public String getDatebaseName() {
		return StringUtils.trimToEmpty(DatebaseName);
	}
	public void setDatebaseName(String datebaseName) {
		DatebaseName = datebaseName;
	}
	public String getDrive() {
		return Drive;
	}
	public void setDrive(String drive) {
		Drive = drive;
	}
	public String getFileName() {
		return FileName;
	}
	public void setFileName(String fileName) {
		FileName = fileName;
	}
	public FileActivityStatistics getStatistics() {
		return statistics;
	}
	public void setStatistics(FileActivityStatistics statistics) {
		this.statistics = statistics;
	}
	public Date getUTCCollectionDateTime() {
		return UTCCollectionDateTime;
	}
	public void setUTCCollectionDateTime(Date uTCCollectionDateTime) {
		UTCCollectionDateTime = uTCCollectionDateTime;
	}
	public String getFilePath() {
		return FilePath;
	}
	public void setFilePath(String filePath) {
		FilePath = filePath;
	}
	public String getFileType() {
		return FileType;
	}
	public void setFileType(String fileType) {
		FileType = fileType;
	}
	
	
}
