package com.idera.sqldm_10_3.data.databases;


import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown = true)
public class DatabaseFile {

	@JsonProperty("ID")
	private int id;
	public int getId() {
		return id;
	}

	@JsonProperty("Name")
	private String name;
	public String getName() {
		return name;
	}

	@JsonProperty("DatabaseID")
	private int databaseID;
	public int getDatabaseID() {
		return databaseID;
	}

	@JsonProperty("SqlFileID")
	private int sqlFileID;
	public int getSqlFileID() {
		return sqlFileID;
	}

	@JsonProperty("SqlFileType")
	private int sqlFileType;
	public int getSqlFileType() {
		return sqlFileType;
	}

	@JsonProperty("PhysicalName")
	private String physicalName;
	public String getPhysicalName() {
		return physicalName;
	}

	@JsonProperty("PageCount")
	private long pageCount;
	public long getPageCount() {
		return pageCount;
	}

	@JsonProperty("LastUpdate")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date lastUpdate;
	public Date getLastUpdate() {
		return lastUpdate;
	}
	
	@JsonProperty("Found")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date found;
	public Date getFound() {
		return found;
	}
	
	@JsonProperty("Removed")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date removed;
	public Date getRemoved() {
		return removed;
	}
	
	@JsonProperty("SizeMB")
	private double sizeMB;
	public double getSizeMB() {
		return sizeMB;
	}

	@JsonProperty("SqlFileTypeDescription")
	private String SqlFileTypeDescription;
	public String getSqlFileTypeDescription() {
		return SqlFileTypeDescription;
	}

	@JsonProperty("State")
	private byte State;
	public byte getState() {
		return State;
	}

	@JsonProperty("MaxSizeMB")
	private double maxSizeMB;
	public double getMaxSizeMB() {
		return maxSizeMB;
	}

	@JsonProperty("GrowthMB")
	private double growthMB;
	public double getGrowthMB() {
		return growthMB;
	}

	@JsonProperty("GrowthPercent")
	private int growthPercent;
	public int getGrowthPercent() {
		return growthPercent;
	}

	@JsonProperty("IsPercentGrowth")
	private boolean isPercentGrowth;
	public boolean getIsPercentGrowth() {
		return isPercentGrowth;
	}

	@JsonProperty("IsAutoGrow")
	private boolean isAutoGrow;
	public boolean getIsAutoGrow() {
		return isAutoGrow;
	}




}
