package com.idera.sqldm_10_3.data.category.resources;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

public class OSPagingData {
	
	
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	@JsonProperty("UTCDateTime") private Date UTCCollectionDateTime;
	@JsonProperty("PagesPerSec") private Double pagesPerSec;
	public Date getUTCCollectionDateTime() {
		return UTCCollectionDateTime;
	}
	public void setUTCCollectionDateTime(Date uTCCollectionDateTime) {
		UTCCollectionDateTime = uTCCollectionDateTime;
	}
	public Double getPagesPerSec() {
		return pagesPerSec;
	}
	public void setPagesPerSec(Double pagesPerSec) {
		this.pagesPerSec = pagesPerSec;
	}
	
}
