package com.idera.sqldm.data.category.resources;

import java.util.Date;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

public class LockWaitData {
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	@JsonProperty("UTCDateTime") private Date UTCCollectionDateTime;
	@JsonProperty("AllocUnit") private Double AllocUnit;
	@JsonProperty("Applicataion") private Double Application;
	@JsonProperty("Database") private Double Database;
	@JsonProperty("Extent") private Double Extent;
	@JsonProperty("File") private Double File;
	@JsonProperty("HoBT") private Double HoBT;
	@JsonProperty("Key") private Double Key;
	@JsonProperty("Latch") private Double Latch;
	@JsonProperty("Metadata") private Double Metadata;
	@JsonProperty("Object") private Double Object;
	@JsonProperty("Page") private Double Page;
	@JsonProperty("RID") private Double RID;
	@JsonProperty("Table") private Double Table;
	public Date getUTCCollectionDateTime() {
		return UTCCollectionDateTime;
	}
	public void setUTCCollectionDateTime(Date uTCCollectionDateTime) {
		UTCCollectionDateTime = uTCCollectionDateTime;
	}
	public Double getAllocUnit() {
		return AllocUnit;
	}
	public void setAllocUnit(Double allocUnit) {
		AllocUnit = allocUnit;
	}
	public Double getApplication() {
		return Application;
	}
	public void setApplication(Double application) {
		Application = application;
	}
	public Double getDatabase() {
		return Database;
	}
	public void setDatabase(Double database) {
		Database = database;
	}
	public Double GetExtent() {
		return Extent;
	}
	public void setExtent(Double extent) {
		Extent = extent;
	}
	public Double getFile() {
		return File;
	}
	public void setFile(Double file) {
		File = file;
	}
	public Double getHoBT() {
		return HoBT;
	}
	public void setHoBT(Double hoBT) {
		HoBT = hoBT;
	}
	public Double getKey() {
		return Key;
	}
	public void setKey(Double key) {
		Key = key;
	}
	public Double getLatch() {
		return Latch;
	}
	public void setLatch(Double latch) {
		Latch = latch;
	}
	public Double getMetadata() {
		return Metadata;
	}
	public void setMetadata(Double metadata) {
		Metadata = metadata;
	}
	public Double getObject() {
		return Object;
	}
	public void setObject(Double object) {
		Object = object;
	}
	public Double getPage() {
		return Page;
	}
	public void setPage(Double page) {
		Page = page;
	}
	public Double getRID() {
		return RID;
	}
	public void setRID(Double rID) {
		RID = rID;
	}
	public Double getTable() {
		return Table;
	}
	public void setTable(Double table) {
		Table = table;
	}
	

}
