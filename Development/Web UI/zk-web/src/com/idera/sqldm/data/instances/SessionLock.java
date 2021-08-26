package com.idera.sqldm.data.instances;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
@JsonIgnoreProperties(ignoreUnknown = true)
public class SessionLock {
	@JsonProperty("BlockedBy") private int blockedby;
	@JsonProperty("BlockedCount") private long  count;
	@JsonProperty("Blocking") private boolean blocking;
	@JsonProperty("Resource") private String resource;
	@JsonProperty("WaitTime") private Long waitTime;
	@JsonProperty("WaitType") private String waitType;
	public int getBlockedby() {
		return blockedby;
	}
	public void setBlockedby(int blockedby) {
		this.blockedby = blockedby;
	}
	public long getCount() {
		return count;
	}
	public void setCount(long count) {
		this.count = count;
	}
	public boolean isBlocking() {
		return blocking;
	}
	public void setBlocking(boolean blocking) {
		this.blocking = blocking;
	}
	public String getResource() {
		return resource;
	}
	public void setResource(String resource) {
		this.resource = resource;
	}
	public String getWaitTime() {
		return String.format("%,d", waitTime);
	}
	public void setWaitTime(Long waitTime) {
		this.waitTime = waitTime;
	}
	public String getWaitType() {
		return waitType;
	}
	public void setWaitType(String waitType) {
		this.waitType = waitType;
	}
	
}
