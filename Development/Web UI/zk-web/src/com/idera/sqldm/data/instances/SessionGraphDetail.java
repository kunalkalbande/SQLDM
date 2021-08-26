package com.idera.sqldm.data.instances;

import java.util.Date;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;
@JsonIgnoreProperties(ignoreUnknown = true)
public class SessionGraphDetail {
	@JsonProperty("UTCCollectionDateTime")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date UTCCollectionDateTime;
	
	@JsonProperty("ResponseTime")
	private Long responsetimeinmils;
	
	@JsonProperty("ActiveSessionCount")
	private long activeSessionCount;
	
	@JsonProperty("IdleSessionCount")
	private long idleSessionCount;
	
	@JsonProperty("SystemSessionCount")
	private long systemSessionCount;
	
	@JsonProperty("BlockedCount")
	private int blockedCount;

	@JsonProperty("LeadBlockers")
	private int leadBlockers;

	@JsonProperty("TotalDeadLock")
	private int totalDeadLock;

	public Date getUTCCollectionDateTime() {
		return UTCCollectionDateTime;
	}

	public Long getResponsetimeinmils() {
		return responsetimeinmils;
	}

	public long getActiveSessionCount() {
		return activeSessionCount;
	}

	public long getIdleSessionCount() {
		return idleSessionCount;
	}

	public long getSystemSessionCount() {
		return systemSessionCount;
	}

	public int getBlockedCount() {
		return blockedCount;
	}

	public int getLeadBlockers() {
		return leadBlockers;
	}

	public int getTotalDeadLock() {
		return totalDeadLock;
	}


}
