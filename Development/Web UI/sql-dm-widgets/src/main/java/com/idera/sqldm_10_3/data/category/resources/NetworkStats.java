package com.idera.sqldm_10_3.data.category.resources;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

public class NetworkStats {

	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	@JsonProperty("UTCDateTime") private Date UTCCollectionDateTime;
	@JsonProperty("PacketsRecievedPerSec") private Integer packetsRecievedPerSec;
	@JsonProperty("PacketsSentPerSec") private Integer packetsSentPerSec;
	public Date getUTCCollectionDateTime() {
		return UTCCollectionDateTime;
	}
	public void setUTCCollectionDateTime(Date uTCCollectionDateTime) {
		UTCCollectionDateTime = uTCCollectionDateTime;
	}
	public Integer getPacketsRecievedPerSec() {
		return packetsRecievedPerSec;
	}
	public void setPacketsRecievedPerSec(Integer packetsRecievedPerSec) {
		this.packetsRecievedPerSec = packetsRecievedPerSec;
	}
	public Integer getPacketsSentPerSec() {
		return packetsSentPerSec;
	}
	public void setPacketsSentPerSec(Integer packetsSentPerSec) {
		this.packetsSentPerSec = packetsSentPerSec;
	}
	
}
