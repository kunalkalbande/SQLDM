package com.idera.sqldm_10_3.data.databases;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

public class TempDBUsageDetails {
	@JsonProperty("UTCCollectionDateTime")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date UTCCollectionDateTime;

	@JsonProperty("InternalObjectsInMB")
	private Double InternalObjectsInMB;

	@JsonProperty("MixedExtentsInMB")
	private Double MixedExtentsInMB;
	@JsonProperty("UnallocatedSpaceInMB")
	private Double UnallocatedSpaceInMB;
	@JsonProperty("UserObjectsInMB")
	private Double UserObjectsInMB;

	@JsonProperty("TempdbGAMWaitTimeMilliseconds")
	private Long TempdbGAMWaitTimeMilliseconds;
	@JsonProperty("TempdbPFSWaitTimeMilliseconds")
	private Long TempdbPFSWaitTimeMilliseconds;
	@JsonProperty("TempdbSGAMWaitTimeMilliseconds")
	private Long TempdbSGAMWaitTimeMilliseconds;
	
	@JsonProperty("VersionStoreCleanupKilobytesPerSec")
	private Double VersionStoreCleanupKilobytesPerSec;
	@JsonProperty("VersionStoreGenerationKilobytesPerSec")
	private Double VersionStoreGenerationKilobytesPerSec;
	@JsonProperty("VersionStoreInMB")
	private Double VersionStoreInMB;

	@JsonProperty("TotalSize")
	private Double TotalSize;

	public Date getUTCCollectionDateTime() {
		return UTCCollectionDateTime;
	}
	public Double getInternalObjectsInMB() {
		return InternalObjectsInMB;
	}
	public Double getMixedExtentsInMB() {
		return MixedExtentsInMB;
	}
	public Double getUnallocatedSpaceInMB() {
		return UnallocatedSpaceInMB;
	}
	public Double getUserObjectsInMB() {
		return UserObjectsInMB;
	}
	public Long getTempdbGAMWaitTimeMilliseconds() {
		return TempdbGAMWaitTimeMilliseconds;
	}
	public Long getTempdbPFSWaitTimeMilliseconds() {
		return TempdbPFSWaitTimeMilliseconds;
	}
	public Long getTempdbSGAMWaitTimeMilliseconds() {
		return TempdbSGAMWaitTimeMilliseconds;
	}
	public Double getVersionStoreCleanupKilobytesPerSec() {
		return VersionStoreCleanupKilobytesPerSec;
	}
	public Double getVersionStoreGenerationKilobytesPerSec() {
		return VersionStoreGenerationKilobytesPerSec;
	}
	public Double getVersionStoreInMB() {
		return VersionStoreInMB;
	}
	public Double getTotalSize() {
		return TotalSize;
	}

}

