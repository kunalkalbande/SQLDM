package com.idera.sqldm_10_3.data.databases;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

public class DatabaseCapacityUsageDetails {
	
	@JsonProperty("UTCCollectionDateTime")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date UTCCollectionDateTime;

	@JsonProperty("DataFileSizeInMb")
	private Double DataFileSizeInMb;

	@JsonProperty("UnusedDataSizeInMb")
	private Double UnusedDataSizeInMb;

	@JsonProperty("LogFileSizeInMb")
	private Double LogFileSizeInMb;

	@JsonProperty("UnusedLogSizeInMb")
	private Double UnusedLogSizeInMb;

	@JsonProperty("NoOfFiles")
	private int NoOfFiles;

	public Date getUTCCollectionDateTime() {
		return UTCCollectionDateTime;
	}

	public Double getDataFileSizeInMb() {
		return DataFileSizeInMb==null ? 0 : DataFileSizeInMb;
	}

	public Double getUnusedDataSizeInMb() {
		return UnusedDataSizeInMb==null?0:UnusedDataSizeInMb;
	}

	public Double getLogFileSizeInMb() {
		return LogFileSizeInMb==null?0:LogFileSizeInMb;
	}

	public Double getUnusedLogSizeInMb() {
		return UnusedLogSizeInMb==null?0:UnusedLogSizeInMb;
	}

	public int getNoOfFiles() {
		return NoOfFiles;
	}
}
