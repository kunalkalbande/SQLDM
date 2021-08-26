package com.idera.sqldm.data.category.resources;

import java.util.Map;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown=true)
public class Memory {
	@JsonProperty("PageLifeExpectancyinSec") private Long PageLifeExpectancyinSec;
	@JsonProperty("SqlMemory") private Map<String, Long> SqlMemory;
	@JsonProperty("BufferCacheHitRatio") private Double BufferCacheHitRatio;
	@JsonProperty("ProcedureCacheHitRatio") private Double ProcedureCacheHitRatio;
	@JsonProperty("MemoryAreas") private Map<String, Double> MemoryAreas;
	public Long getPageLifeExpectancyinSec() {
		return PageLifeExpectancyinSec;
	}
	public void setPageLifeExpectancyinSec(Long pageLifeExpectancyinSec) {
		PageLifeExpectancyinSec = pageLifeExpectancyinSec;
	}
	public Map<String, Long> getSqlMemory() {
		return SqlMemory;
	}
	public void setSqlMemory(Map<String, Long> sqlMemory) {
		SqlMemory = sqlMemory;
	}
	public Double getBufferCacheHitRatio() {
		return BufferCacheHitRatio;
	}
	public void setBufferCacheHitRatio(Double bufferCacheHitRatio) {
		BufferCacheHitRatio = bufferCacheHitRatio;
	}
	public Double getProcedureCacheHitRatio() {
		return ProcedureCacheHitRatio;
	}
	public void setProcedureCacheHitRatio(Double procedureCacheHitRatio) {
		ProcedureCacheHitRatio = procedureCacheHitRatio;
	}
	public Map<String, Double> getMemoryAreas() {
		return MemoryAreas;
	}
	public void setMemoryAreas(Map<String, Double> memoryAreas) {
		MemoryAreas = memoryAreas;
	}
}
