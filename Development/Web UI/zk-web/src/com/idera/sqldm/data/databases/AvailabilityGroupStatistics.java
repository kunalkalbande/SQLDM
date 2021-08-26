package com.idera.sqldm.data.databases;

import java.util.Date;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.sqldm.utils.Utility;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

@JsonIgnoreProperties(ignoreUnknown = true)
public class AvailabilityGroupStatistics {

	@JsonProperty("LogSendQueueSize") private Double logSendQueueSize;
	@JsonProperty("LogTransferRate") private Double logTransferRate;
	@JsonProperty("RedoQueueSize") private Double redoQueueSize;
	
	@JsonProperty("RedoTransferRate") private Double redoTransferRate;
	//@JsonProperty("DatabaseName") private String databaseName;
	@JsonProperty("GroupName")
	private String groupName;
	@JsonProperty("ReplicaName")
	private String replicaName;
	
	@JsonProperty("UTCCollectionDateTime")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date UTCCollectionDateTime;

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

	/*public String getDatabaseName() {
		return databaseName;
	}

	public void setDatabaseName(String databaseName) {
		this.databaseName = databaseName;
	}*/

	public Date getUTCCollectionDateTime() {
		return UTCCollectionDateTime;
	}

	public void setUTCCollectionDateTime(Date uTCCollectionDateTime) {
		UTCCollectionDateTime = uTCCollectionDateTime;
	}

	public String getGroupName() {
		return groupName;
	}

	public void setGroupName(String groupName) {
		this.groupName = groupName;
	}

	public String getReplicaName() {
		return replicaName;
	}

	public void setReplicaName(String replicaName) {
		this.replicaName = replicaName;
	}
		
}


