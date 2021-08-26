package com.idera.sqldm.data.instances;

import com.fasterxml.jackson.annotation.JsonProperty;

public class TempDBSummary {
	@JsonProperty("versionStoreMegabytes") private Object versionStoreMegabytes;
	@JsonProperty("filesize") private Object filesize;
	@JsonProperty("unallocatedSpaceMegabytes") private Object unallocatedSpaceMegabytes;
	@JsonProperty("mixedExtentsMegabytes") private Object mixedExtentsMegabytes;
	@JsonProperty("userObjectsMegabytes") private Object userObjectsMegabytes;
	@JsonProperty("internalObjectsMegabytes") private Object internalObjectsMegabytes;
	public Object getVersionStoreMegabytes() {
		return versionStoreMegabytes;
	}
	public Object getFilesize() {
		return filesize;
	}
	public Object getUnallocatedSpaceMegabytes() {
		return unallocatedSpaceMegabytes;
	}
	public Object getMixedExtentsMegabytes() {
		return mixedExtentsMegabytes;
	}
	public Object getUserObjectsMegabytes() {
		return userObjectsMegabytes;
	}
	public Object getInternalObjectsMegabytes() {
		return internalObjectsMegabytes;
	}
	
	
}
