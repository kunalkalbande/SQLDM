package com.idera.sqldm_10_3.data.instances;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown=true)
public class VirtualizationStats {
	
	@JsonProperty("ESXAvailableMemBytes") 
	private Long ESXAvailableMemBytes;
	
	@JsonProperty("ESXDiskRead") 
	private Long ESXDiskRead;
	
	@JsonProperty("ESXDiskWrite") 
	private Long ESXDiskWrite;
	
	@JsonProperty("ESXMemActiveMB")
	private Long ESXMemActiveMB;
	
	@JsonProperty("ESXMemBaloonedMB")
	private Long ESXMemBaloonedMB;
	
	@JsonProperty("ESXMemConsumedMB")
	private Long ESXMemConsumedMB;
	
	@JsonProperty("ESXMemGrantedMB")
	private Long ESXMemGrantedMB;
	
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	@JsonProperty("UTCCollectionDateTime")
	private Date UTCCollectionDateTime;
	
	@JsonProperty("VMAvailableByte")
	private Long VMAvailableByte;
	
	@JsonProperty("VMDiskRead")
	private Long VMDiskRead;
	
	@JsonProperty("VMDiskWrite")
	private Long VMDiskWrite;
	
	@JsonProperty("VMMemActiveMB")
	private Long VMMemActiveMB;
	
	@JsonProperty("VMMemBaloonedMB")
	private Long VMMemBaloonedMB;
	
	@JsonProperty("VMMemConsumedMB")
	private Long VMMemConsumedMB;
	
	@JsonProperty("VMMemGrantedMB")
	private Long VMMemGrantedMB;
	
	public Long getESXDiskRead() {
		return ESXDiskRead;
	}
	public void setESXDiskRead(Long eSXDiskRead) {
		ESXDiskRead = eSXDiskRead;
	}
	public Long getESXDiskWrite() {
		return ESXDiskWrite;
	}
	public void setESXDiskWrite(Long eSXDiskWrite) {
		ESXDiskWrite = eSXDiskWrite;
	}
	public Long getVMDiskRead() {
		return VMDiskRead;
	}
	public void setVMDiskRead(Long vMDiskRead) {
		VMDiskRead = vMDiskRead;
	}
	public Long getVMDiskWrite() {
		return VMDiskWrite;
	}
	public void setVMDiskWrite(Long vMDiskWrite) {
		VMDiskWrite = vMDiskWrite;
	}
	public Long getESXAvailableMemBytes() {
		return ESXAvailableMemBytes;
	}
	public void setESXAvailableMemBytes(Long eSXAvailableMemBytes) {
		ESXAvailableMemBytes = eSXAvailableMemBytes;
	}
	
	public Long getESXMemActiveMB() {
		return ESXMemActiveMB;
	}
	public void setESXMemActiveMB(Long eSXMemActiveMB) {
		ESXMemActiveMB = eSXMemActiveMB;
	}
	public Long getESXMemBaloonedMB() {
		return ESXMemBaloonedMB;
	}
	public void setESXMemBaloonedMB(Long eSXMemBaloonedMB) {
		ESXMemBaloonedMB = eSXMemBaloonedMB;
	}
	public Long getESXMemConsumedMB() {
		return ESXMemConsumedMB;
	}
	public void setESXMemConsumedMB(Long eSXMemConsumedMB) {
		ESXMemConsumedMB = eSXMemConsumedMB;
	}
	public Long getESXMemGrantedMB() {
		return ESXMemGrantedMB;
	}
	public void setESXMemGrantedMB(Long eSXMemGrantedMB) {
		ESXMemGrantedMB = eSXMemGrantedMB;
	}
	public Date getUTCCollectionDateTime() {
		return UTCCollectionDateTime;
	}
	public void setUTCCollectionDateTime(Date uTCCollectionDateTime) {
		UTCCollectionDateTime = uTCCollectionDateTime;
	}
	public Long getVMAvailableByte() {
		return VMAvailableByte;
	}
	public void setVMAvailableByte(Long vMAvailableByte) {
		VMAvailableByte = vMAvailableByte;
	}
	
	public Long getVMMemActiveMB() {
		return VMMemActiveMB;
	}
	public void setVMMemActiveMB(Long vMMemActiveMB) {
		VMMemActiveMB = vMMemActiveMB;
	}
	public Long getVMMemBaloonedMB() {
		return VMMemBaloonedMB;
	}
	public void setVMMemBaloonedMB(Long vMMemBaloonedMB) {
		VMMemBaloonedMB = vMMemBaloonedMB;
	}
	public Long getVMMemConsumedMB() {
		return VMMemConsumedMB;
	}
	public void setVMMemConsumedMB(Long vMMemConsumedMB) {
		VMMemConsumedMB = vMMemConsumedMB;
	}
	public Long getVMMemGrantedMB() {
		return VMMemGrantedMB;
	}
	public void setVMMemGrantedMB(Long vMMemGrantedMB) {
		VMMemGrantedMB = vMMemGrantedMB;
	}
}
