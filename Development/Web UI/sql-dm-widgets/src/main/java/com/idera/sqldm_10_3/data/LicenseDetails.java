package com.idera.sqldm_10_3.data;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown = true)
public class LicenseDetails {

	@JsonProperty("Expiration")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date expiration;
	public Date getExpiration() {
		return expiration;
	}

	@JsonProperty("IsPermanent")
	private boolean isPermanent;
	public boolean isPermanent() {
		return isPermanent;
	}

	@JsonProperty("IsTrial")
	private boolean isTrial;
	public boolean isTrial() {
		return isTrial;
	}
	
	@JsonProperty("LicensedServers")
	private int licensedServers;
	public int getLicensedServers() {
		return licensedServers;
	}
	
	@JsonProperty("CheckedKeyCount")
	private int CheckedKeyCount;
	public int getCheckedKeyCount() {
		return CheckedKeyCount;
	}
	
	@JsonProperty("MonitoredServers")
	private int monitoredServers;
	public int getMonitoredServers() {
		return monitoredServers;
	}
	
	@JsonProperty("Repository")
	private String repository;
	public String getRepository() {
		return repository;
	}

	//Enum Status { OK, Expired, CountExceeded, NoValidKeys }
	@JsonProperty("Status")
	private String status;
	public String getStatus() {
		return status;
	}

	public long getDaysToExpire() {
		long diff=expiration.getTime()-System.currentTimeMillis();
		return  diff / (24 * 60 * 60 * 1000);
	}
}
