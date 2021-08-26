package com.idera.sqldm.data;

import com.fasterxml.jackson.annotation.JsonProperty;

public class SessionTempDbSpaceUsed {
	@JsonProperty("SessionInterval") private String sessionInterval;
	@JsonProperty("SessionUser") private String sessionUser;
	@JsonProperty("TaskInterval") private String taskInterval;
	@JsonProperty("TaskUser") private String taskUser;
	public String getSessionInterval() {
		return sessionInterval;
	}
	public void setSessionInterval(String sessionInterval) {
		this.sessionInterval = sessionInterval;
	}
	public String getSessionUser() {
		return sessionUser;
	}
	public void setSessionUser(String sessionUser) {
		this.sessionUser = sessionUser;
	}
	public String getTaskInterval() {
		return taskInterval;
	}
	public void setTaskInterval(String taskInterval) {
		this.taskInterval = taskInterval;
	}
	public String getTaskUser() {
		return taskUser;
	}
	public void setTaskUser(String taskUser) {
		this.taskUser = taskUser;
	}
}
