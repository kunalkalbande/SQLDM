package com.idera.sqldm_10_3.data.databases;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm_10_3.utils.Utility;

@JsonIgnoreProperties(ignoreUnknown = true)
public class AvailabilityGroupDetails {
	/*@JsonProperty("DatabaseName")
	private String databaseName;*/
	@JsonProperty("DatabaseStatus")
	private String databaseStatus;
	@JsonProperty("GroupName")
	private String groupName;
	@JsonProperty("LogSendQueueSize")
	private Double logSendQueueSize;
	@JsonProperty("LogTransferRate")
	private Double logTransferRate;
	@JsonProperty("RedoQueueSize")
	private Double redoQueueSize;
	@JsonProperty("RedoTransferRate")
	private Double redoTransferRate;
	@JsonProperty("ReplicaName")
	private String replicaName;
	@JsonProperty("ReplicaRole")
	private String replicaRole;
	@JsonProperty("SyncHealth")
	private String syncHealth;
	/*public String getDatabaseName() {
		return databaseName;
	}
	public void setDatabaseName(String databaseName) {
		this.databaseName = databaseName;
	}*/
	public String getDatabaseStatus() {
		return databaseStatus;
	}
	public void setDatabaseStatus(String databaseStatus) {
		this.databaseStatus = databaseStatus;
	}
	public String getGroupName() {
		return groupName;
	}
	public void setGroupName(String groupName) {
		this.groupName = groupName;
	}
	public Double getLogSendQueueSize() {
		return Utility.round(logSendQueueSize == null ? 0d : logSendQueueSize, 2);
	}
	public void setLogSendQueueSize(Double logSendQueueSize) {
		this.logSendQueueSize = logSendQueueSize;
	}
	public Double getLogTransferRate() {
		return Utility.round(logTransferRate == null ? 0d : logTransferRate, 2);
	}
	public void setLogTransferRate(Double logTransferRate) {
		this.logTransferRate = logTransferRate;
	}
	public Double getRedoQueueSize() {
		return Utility.round(redoQueueSize == null ? 0d : redoQueueSize, 2);
	}
	public void setRedoQueueSize(Double redoQueueSize) {
		this.redoQueueSize = redoQueueSize;
	}
	public Double getRedoTransferRate() {
		return Utility.round(redoTransferRate == null ? 0d : redoTransferRate, 2);
	}
	public void setRedoTransferRate(Double redoTransferRate) {
		this.redoTransferRate = redoTransferRate;
	}
	public String getReplicaName() {
		return replicaName;
	}
	public void setReplicaName(String replicaName) {
		this.replicaName = replicaName;
	}
	public String getReplicaRole() {
		return replicaRole;
	}
	public void setReplicaRole(String replicaRole) {
		this.replicaRole = replicaRole;
	}
	public String getSyncHealth() {
		return syncHealth;
	}
	public void setSyncHealth(String syncHealth) {
		this.syncHealth = syncHealth;
	}
}
