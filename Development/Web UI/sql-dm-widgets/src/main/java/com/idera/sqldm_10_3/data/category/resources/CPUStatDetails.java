package com.idera.sqldm_10_3.data.category.resources;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown=true)
public class CPUStatDetails {
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	@JsonProperty("UTCDateTime") private Date UTCCollectionDateTime;
	@JsonProperty("TransactionsPerSec") private Double transPerSec;
	@JsonProperty("SQLRecompilationsPerSec") private Double sqlRecompPerSec;
	@JsonProperty("SQLCompilationsPerSec") private Double sqlCompPerSec;
	@JsonProperty("BatchesPerSec") private Double BatchesPerSec;
	public Date getUTCCollectionDateTime() {
		return UTCCollectionDateTime;
	}
	public void setUTCCollectionDateTime(Date uTCCollectionDateTime) {
		UTCCollectionDateTime = uTCCollectionDateTime;
	}
	public Double getTransPerSec() {
		return transPerSec;
	}
	public void setTransPerSec(Double transPerSec) {
		this.transPerSec = transPerSec;
	}
	public Double getSqlRecompPerSec() {
		return sqlRecompPerSec;
	}
	public void setSqlRecompPerSec(Double sqlRecompPerSec) {
		this.sqlRecompPerSec = sqlRecompPerSec;
	}
	public Double getSqlCompPerSec() {
		return sqlCompPerSec;
	}
	public void setSqlCompPerSec(Double sqlCompPerSec) {
		this.sqlCompPerSec = sqlCompPerSec;
	}
	public Double getBatchesPerSec() {
		return BatchesPerSec;
	}
	public void setBatchesPerSec(Double d) {
		BatchesPerSec = d;
	}
	
	
}
