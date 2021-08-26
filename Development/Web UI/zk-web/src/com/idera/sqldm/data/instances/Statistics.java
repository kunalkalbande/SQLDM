package com.idera.sqldm.data.instances;

import org.apache.commons.lang.StringUtils;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm.utils.Utility;

public class Statistics {
	@JsonProperty("checkpointPages") private String checkpointPages;
	@JsonProperty("totalConnections") private String totalConnections;
	@JsonProperty("packetsSent") private String packetsSent;
	@JsonProperty("ReplicationSubscribed") private String ReplicationSubscribed;
	@JsonProperty("BufferCacheHitRatio") private String BufferCacheHitRatio;
	@JsonProperty("cpuBusyDelta") private String cpuBusyDelta;
	@JsonProperty("sqlCompilations") private String sqlCompilations;
	@JsonProperty("pageSplits") private String pageSplits;
	@JsonProperty("diskErrors") private String diskErrors;
	@JsonProperty("workfilesCreated") private String workfilesCreated;
	@JsonProperty("diskRead") private String diskRead;
	@JsonProperty("WorktablesCreated") private String WorktablesCreated;
	@JsonProperty("batchRequests") private String batchRequests;
	@JsonProperty("pageReads") private String pageReads;
	@JsonProperty("TempDBSizePercent") private String TempDBSizePercent;
	@JsonProperty("diskWrite") private String diskWrite;
	@JsonProperty("TempDBSize") private String TempDBSize;
	@JsonProperty("ReplicationUnsubscribed") private String ReplicationUnsubscribed;
	@JsonProperty("pageWrites") private String pageWrites;
	@JsonProperty("pageLookups") private String pageLookups;
	@JsonProperty("lockWaits") private String lockWaits;
	@JsonProperty("sqlRecompilations") private String sqlRecompilations;
	@JsonProperty("fullScans") private String fullScans;
	@JsonProperty("idleTimeDelta") private String idleTimeDelta;
	@JsonProperty("OldestOpenTransactionsInMinutes") private String OldestOpenTransactionsInMinutes;
	@JsonProperty("idlePercentage") private String idlePercentage;
	@JsonProperty("lazyWrites") private String lazyWrites;
	@JsonProperty("pageLifeExpectancy") private String pageLifeExpectancy;
	@JsonProperty("CacheHitRatio") private String CacheHitRatio;
	@JsonProperty("cpuPercentage") private Double cpuPercentage;
	@JsonProperty("timeTicks") private String timeTicks;
	@JsonProperty("logFlushes") private String logFlushes;
	@JsonProperty("ioPercentage") private String ioPercentage;
	@JsonProperty("ReplicationUndistributed") private String ReplicationUndistributed;
	@JsonProperty("packetsReceived") private String packetsReceived;
	@JsonProperty("packetErrors") private String packetErrors;
	@JsonProperty("ReplicationLatencyInSeconds") private String ReplicationLatencyInSeconds;
	@JsonProperty("ioTimeDelta") private String ioTimeDelta;
	@JsonProperty("tableLockEscalations") private String tableLockEscalations;
	@JsonProperty("readaheadPages") private String readaheadPages;
	@JsonProperty("SqlMemoryUsed") private String SqlMemoryUsed;
	@JsonProperty("PageLifeExpectancySeconds") private Integer PageLifeExpectancySeconds;
	public String getCheckpointPages() {
		return checkpointPages;
	}
	public String getTotalConnections() {
		return totalConnections;
	}
	public String getPacketsSent() {
		return packetsSent;
	}
	public String getReplicationSubscribed() {
		return ReplicationSubscribed;
	}
	public String getBufferCacheHitRatio() {
		return BufferCacheHitRatio;
	}
	public String getCpuBusyDelta() {
		return cpuBusyDelta;
	}
	public String getSqlCompilations() {
		return sqlCompilations;
	}
	public String getPageSplits() {
		return pageSplits;
	}
	public String getDiskErrors() {
		return diskErrors;
	}
	public String getWorkfilesCreated() {
		return workfilesCreated;
	}
	public String getDiskRead() {
		return diskRead;
	}
	public String getWorktablesCreated() {
		return WorktablesCreated;
	}
	public String getBatchRequests() {
		return batchRequests;
	}
	public String getPageReads() {
		if (StringUtils.isEmpty(pageReads)) {
			return "-";
		}
		return pageReads;
	}
	public String getTempDBSizePercent() {
		return TempDBSizePercent;
	}
	public String getDiskWrite() {
		return diskWrite;
	}
	public String getTempDBSize() {
		return TempDBSize;
	}
	public String getReplicationUnsubscribed() {
		return ReplicationUnsubscribed;
	}
	public String getPageWrites() {
		if (StringUtils.isEmpty(pageWrites)) {
			return "-";
		}
		return pageWrites;
	}
	public String getPageLookups() {
		return pageLookups;
	}
	public String getLockWaits() {
		return lockWaits;
	}
	public String getSqlRecompilations() {
		return sqlRecompilations;
	}
	public String getFullScans() {
		return fullScans;
	}
	public String getIdleTimeDelta() {
		return idleTimeDelta;
	}
	public String getOldestOpenTransactionsInMinutes() {
		return OldestOpenTransactionsInMinutes;
	}
	public String getIdlePercentage() {
		return idlePercentage;
	}
	public String getLazyWrites() {
		return lazyWrites;
	}
	public String getPageLifeExpectancy() {
		return pageLifeExpectancy;
	}
	public String getCacheHitRatio() {
		return CacheHitRatio;
	}
	public Double getCpuPercentage() {
		return getCpuPercentage(2);
	}
	public Double getCpuPercentage(int places) {
		if (cpuPercentage == null) {
			return null;
		}
		return Utility.round(cpuPercentage, places);
	}
	public String getCpuPercentageString() {
		Double d = getCpuPercentage(0);
		if (d == null) {
			return "N/A";
		}
		return d.intValue() + " %";
	}
	public String getTimeTicks() {
		return timeTicks;
	}
	public String getLogFlushes() {
		return logFlushes;
	}
	public String getIoPercentage() {
		return ioPercentage;
	}
	public String getReplicationUndistributed() {
		return ReplicationUndistributed;
	}
	public String getPacketsReceived() {
		return packetsReceived;
	}
	public String getPacketErrors() {
		return packetErrors;
	}
	public String getReplicationLatencyInSeconds() {
		return ReplicationLatencyInSeconds;
	}
	public String getIoTimeDelta() {
		return ioTimeDelta;
	}
	public String getTableLockEscalations() {
		return tableLockEscalations;
	}
	public String getReadaheadPages() {
		return readaheadPages;
	}
	public String getSqlMemoryUsed() {
		return SqlMemoryUsed;
	}
	public void setSqlMemoryUsed(String sqlMemoryUsed) {
		SqlMemoryUsed = sqlMemoryUsed;
	}
	public Integer getPageLifeExpectancySeconds() {
		return PageLifeExpectancySeconds;
	}
	public void setPageLifeExpectancySeconds(Integer pageLifeExpectancySeconds) {
		PageLifeExpectancySeconds = pageLifeExpectancySeconds;
	}
	/*TODO: Computed Property Need to be after every contract change*/
	/*Computed Properties*/
	public String getDiskIO() {
		
		String read = this.getDiskRead();
		if(read == null ||  "".equals(read)) {
			read = "-";
		}
		String write = this.getDiskWrite();
		if(write == null ||  "".equals(write)) {
			write = "-";
		}
		return read.concat("/").concat(write);
	}
	
}
