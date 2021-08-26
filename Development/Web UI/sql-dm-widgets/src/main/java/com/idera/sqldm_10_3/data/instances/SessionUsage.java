package com.idera.sqldm_10_3.data.instances;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown = true)
public class SessionUsage {
	@JsonProperty("Cpu") private Long cpu;
	@JsonProperty("CpuDelta") private Long cpuDelta;
	@JsonProperty("LastActivity") 
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date lastBatch;
	@JsonProperty("LoginTime") 
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date loginTime;
	@JsonProperty("Memory") private String memoryUsage;
	@JsonProperty("OpenTransactions") private Long openTransactions;
	@JsonProperty("PhysicalIO") private Long io;
	
	public Long getCpu() {
		return cpu;
	}
	public void setCpu(Long cpu) {
		this.cpu = cpu;
	}
	public Long getCpuDelta() {
		return cpuDelta;
	}
	public void setCpuDelta(Long cpuDelta) {
		this.cpuDelta = cpuDelta;
	}
	public Date getLastBatch() {
		return lastBatch;
	}
	public void setLastBatch(Date lastBatch) {
		this.lastBatch = lastBatch;
	}
	public Date getLoginTime() {
		return loginTime;
	}
	public void setLoginTime(Date loginTime) {
		this.loginTime = loginTime;
	}
	public String getMemoryUsage() {
		return memoryUsage;
	}
	public void setMemoryUsage(String memoryUsage) {
		this.memoryUsage = memoryUsage;
	}
	public long getOpenTransactions() {
		return openTransactions;
	}
	public void setOpenTransactions(Long openTransactions) {
		this.openTransactions = openTransactions;
	}
	public Long getIo() {
		return io;
	}
	public void setIo(Long io) {
		this.io = io;
	}
	
}
