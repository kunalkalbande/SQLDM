package com.idera.sqldm_10_3.data.databases;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.RecoveryModel;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;
import org.apache.commons.lang.StringUtils;

import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown = true)
public class Database {

	@JsonProperty("ID")
	private int id;
	public int getId() {
		return id;
	}

	@JsonProperty("Name")
	private String name;
	public String getName() {
		return StringUtils.trimToEmpty(name);
	}

	@JsonProperty("InstanceID")
	private int instanceId;
	public int getInstanceId() {
		return instanceId;
	}

	@JsonProperty("InstanceName")
	private String instanceName;
	public String getInstanceName() {
		return instanceName;
	}

	// @author Saumyadeep 
	// Friendly Begin
	@JsonProperty("FriendlyServerName")
	private String friendlyServerName;
	public String getFriendlyServerName() {
		return this.friendlyServerName;
	}

	private String displayName;
	
	public String getDisplayName() {
		if(this.getFriendlyServerName()!= null)	
			return this.getFriendlyServerName();
		else
			return this.getInstanceName();
	}
	// Friendly End
	
	
	@JsonProperty("SqlID")
	private int sqlId;
	public int getSqlId() {
		return sqlId;
	}

	@JsonProperty("State")
	private int state;
	public int getState() {
		return state;
	}

	@JsonProperty("Owner")
	private String owner;
	public String getOwner() {
		return owner;
	}

	@JsonProperty("PageCount")
	private long pageCount;
	public long getPageCount() {
		return pageCount;
	}

	@JsonProperty("SystemDatabase")
	private boolean systemDatabase;
	public boolean isSystemDatabase() {
		return systemDatabase;
	}

	@JsonProperty("LastUpdate")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date lastUpdate;
	public Date getLastUpdate() {
		return lastUpdate;
	}

	@JsonProperty("Removed")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date removed;
	public Date getRemoved() {
		return removed;
	}

	@JsonProperty("Availability")
	private int availability;
	public int getAvailability() {
		return availability;
	}

	@JsonProperty("AvailabilityDate")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date availabilityDate;
	public Date getAvailabilityDate() {
		return availabilityDate;
	}

	@JsonProperty("SizeMB")
	private double sizeMB;
	public double getSizeMB() {
		return sizeMB;
	}

	@JsonProperty("TransactionsPerSec")
	private long transactionsPerSec;
	public long getTransactionsPerSec() {
		return transactionsPerSec;
	}

	@JsonProperty("Enabled")
	private boolean enabled;
	public boolean getEnabled() {
		return enabled;
	}

	@JsonProperty("CompatibilityLevel")
	private byte compatibilityLevel;
	public byte getCompatibilityLevel() {
		return compatibilityLevel;
	}

	@JsonProperty("CompatibilityLevelDescription")
	private String compatibilityLevelDescription;
	public String getCompatibilityLevelDescription() {
		return compatibilityLevelDescription;
	}

	@JsonProperty("CreateDate")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date createDate;
	public Date getCreateDate() {
		return createDate;
	}
	@JsonProperty("LastDbccCheckdb")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date lastDbccCheckdb;
	public Date getLastDbccCheckdb() {
		return lastDbccCheckdb;
	}
	@JsonProperty("LastBackup")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date lastBackup;
	public Date getLastBackup() {
		return lastBackup;
	}
	@JsonProperty("LastFullBackup")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date lastFullBackup;
	public Date getLastFullBackup() {
		return lastFullBackup;
	}
	@JsonProperty("LastDiffBackup")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date lastDiffBackup;
	public Date getLastDiffBackup() {
		return lastDiffBackup;
	}
	@JsonProperty("LastLogBackup")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date lastLogBackup;
	public Date getLastLogBackup() {
		return lastLogBackup;
	}

	@JsonProperty("AvailabilityDescription")
	private String availabilityDescription;
	public String getAvailabilityDescription() {
		return availabilityDescription;
	}
	
	@JsonProperty("RecoveryModel")
	private byte recoveryModel;
	public RecoveryModel getRecoveryModel() {
		return RecoveryModel.getRecoveryModel(recoveryModel);
	}
}
