package com.idera.sqldm_10_3.data.category.resources;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown=true)
public class ServerWait {
	@JsonProperty("Category") private String Category;
	@JsonProperty("WaitType") private String WaitType;
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	@JsonProperty("UTCCollectionDateTime") 
	private Date UTCCollectionDateTime;
	@JsonProperty("WaitingTasks") private Double WaitingTasks;
	@JsonProperty("statistics") private ServerWaitStatistics statistics;
	@JsonProperty("Description") private String description;
	@JsonProperty("HelpLink") private String help;
	
	public String getCategory() {
		return Category;
	}
	public void setCategory(String category) {
		Category = category;
	}
	public String getWaitType() {
		return WaitType;
	}
	public void setWaitType(String waitType) {
		WaitType = waitType;
	}
	public Date getUTCCollectionDateTime() {
		return UTCCollectionDateTime;
	}
	public void setUTCCollectionDateTime(Date uTCCollectionDateTime) {
		UTCCollectionDateTime = uTCCollectionDateTime;
	}
	public Double getWaitingTasks() {
		return WaitingTasks;
	}
	public void setWaitingTasks(Double waitingTasks) {
		WaitingTasks = waitingTasks;
	}
	public ServerWaitStatistics getStatistics() {
		return statistics;
	}
	public void setStatistics(ServerWaitStatistics statistics) {
		this.statistics = statistics;
	}
	public String getDescription() {
		return description;
	}
	public void setDescription(String description) {
		this.description = description;
	}
	public String getHelp() {
		return help;
	}
	public void setHelp(String help) {
		this.help = help;
	}
}
