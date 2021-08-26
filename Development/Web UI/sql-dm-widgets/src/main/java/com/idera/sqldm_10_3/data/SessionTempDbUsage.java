package com.idera.sqldm_10_3.data;

import com.fasterxml.jackson.annotation.JsonProperty;

public class SessionTempDbUsage {

	@JsonProperty("VersionStoreElapsedSeconds") private Long versionStoreElapsedSeconds;
	@JsonProperty("spaceUsed") private SessionTempDbSpaceUsed spaceUsed;
	public Long getVersionStoreElapsedSeconds() {
		return versionStoreElapsedSeconds;
	}
	public void setVersionStoreElapsedSeconds(Long versionStoreElapsedSeconds) {
		this.versionStoreElapsedSeconds = versionStoreElapsedSeconds;
	}
	public SessionTempDbSpaceUsed getSpaceUsed() {
		return spaceUsed;
	}
	public void setSpaceUsed(SessionTempDbSpaceUsed spaceUsed) {
		this.spaceUsed = spaceUsed;
	}

}
