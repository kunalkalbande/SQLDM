package com.idera.sqldm_10_3.data.instances;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class SessionConnection {
	@JsonProperty("Application") private String application;
	@JsonProperty("Command") private String command;
	@JsonProperty("Database") private String database;
	@JsonProperty("Host") private String host;
	@JsonProperty("Id") private Long id;
	@JsonProperty("NetworkAddress") private String address;
	@JsonProperty("NetLibrary") private String netLibrary;
	@JsonProperty("Status") private String status;	
	@JsonProperty("IsUserSession") private Boolean IsUserSession;	
	@JsonProperty("IsSystemSession") private Boolean IsSystemSession;
	
	public String getNetLibrary() {
		return netLibrary;
	}
	public void setNetLibrary(String netLibrary) {
		this.netLibrary = netLibrary;
	}
	public String getApplication() {
		return application;
	}
	public void setApplication(String application) {
		this.application = application;
	}
	public String getCommand() {
		return command;
	}
	public void setCommand(String command) {
		this.command = command;
	}
	public String getDatabase() {
		return database;
	}
	public void setDatabase(String database) {
		this.database = database;
	}
	public String getHost() {
		return host;
	}
	public void setHost(String host) {
		this.host = host;
	}
	public Long getId() {
		return id;
	}
	public void setId(Long id) {
		this.id = id;
	}
	public String getAddress() {
		return address;
	}
	public void setAddress(String address) {
		this.address = address;
	}
	public String getStatus() {
		return status;
	}
	public void setStatus(String status) {
		this.status = status;
	}
	public Boolean getIsUserSession() {
		return IsUserSession;
	}
	public Boolean getIsSystemSession() {
		return IsSystemSession;
	}

	
}
