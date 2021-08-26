package com.idera.sqldm.data;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.common.DatabaseStatus;
import com.idera.common.InstanceStatus;
import com.idera.common.Status;
import com.idera.sqldm.data.SQLDMQueryFacade.GroupBy;

public class SQLDMQueryResult {

	private Long databaseCount = null;
	private Long instanceCount = null;
	private Long totalDataSize = null;
	private Long totalDatabaseSize = null;
	private Long totalLogSize = null;
	private Long totalTransactions = null;
	private String name = null;
	private Integer id = null;
	private Long dataSize = null;
	private Long databaseSize = null;
	private Integer instanceID = null;
	private String instanceName = null;
	private Long logSize = null;
	private String sqlVersion = null;
	private Long transactionsPerSec = null;
	private Integer responseTime = null;
	private Long transactions = null;
	private boolean drillable = false;
	private GroupBy mainEntry;
	private Integer availability = null;
	private boolean enabled = true;
	private boolean hasDetails = false;
	private Integer instanceState;
	
	// @author Saumyadeep 
	// Friendly Begin

	private String friendlyServerName;

	public void setFriendlyServerName(String friendlyServerName) {
		this.friendlyServerName = friendlyServerName;
	}
	
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

	public void setDisplayName(String displayName) {
		this.displayName = displayName;
	}
	// Friendly End
	
	public boolean hasDetails() {
		return hasDetails;
	}
	public void setHasDetails(boolean hasDetails) {
		this.hasDetails = hasDetails;
	}
	public boolean isEnabled() {
		return enabled;
	}
	public void setEnabled(boolean enabled) {
		this.enabled = enabled;
	}
	public void setAvailability(Integer availability) {
		this.availability = availability;
	}
	public Integer getAvailability() {
		return availability;
	}
	public void setAvailability(int availability) {
		this.availability = availability;
	}
	public GroupBy getMainEntry() {
		return mainEntry;
	}
	public void setMainEntry(GroupBy mainEntry) {
		this.mainEntry = mainEntry;
	}
	public boolean isDrillable() {
		return drillable;
	}
	public void setDrillable(boolean drillable) {
		this.drillable = drillable;
	}
	public Long getDatabaseCount() {
		if ( isEnabled() || databaseCount == null)
			return databaseCount;
		else
			return -1L;
	}
	public void setDatabaseCount(Long databaseCount) {
		this.databaseCount = databaseCount;
	}
	public Long getInstanceCount() {
		if ( isEnabled() || instanceCount == null)
			return instanceCount;
		else
			return -1L;
	}
	public void setInstanceCount(Long instanceCount) {
		this.instanceCount = instanceCount;
	}
	public Long getTotalDataSize() {
		if ( isEnabled() || totalDataSize == null)
			return totalDataSize;
		else
			return -1L;
	}
	public void setTotalDataSize(Long totalDataSize) {
		this.totalDataSize = totalDataSize;
	}
	public Long getTotalDatabaseSize() {
		if ( isEnabled() || totalDatabaseSize == null)
			return totalDatabaseSize;
		else
			return -1L;
	}
	public void setTotalDatabaseSize(Long totalDatabaseSize) {
		this.totalDatabaseSize = totalDatabaseSize;
	}
	public Long getTotalLogSize() {
		if ( isEnabled() || totalLogSize == null)
			return totalLogSize;
		else
			return -1L;
	}
	public void setTotalLogSize(Long totalLogSize) {
		this.totalLogSize = totalLogSize;
	}
	public Long getTotalTransactions() {
		if ( isEnabled() || totalTransactions == null)
			return totalTransactions;
		else
			return -1L;
	}
	public void setTotalTransactions(Long totalTransactions) {
		this.totalTransactions = totalTransactions;
	}
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	public Integer getId() {
		return id;
	}
	public void setId(Integer id) {
		this.id = id;
	}
	public Long getDataSize() {
		if ( isEnabled() || dataSize == null)
			return dataSize;
		else
			return -1L;
	}
	public void setDataSize(Long dataSize) {
		this.dataSize = dataSize;
	}
	public Long getDatabaseSize() {
		if ( isEnabled() || databaseSize == null)
			return databaseSize;
		else
			return -1L;
	}
	public void setDatabaseSize(Long databaseSize) {
		this.databaseSize = databaseSize;
	}
	public Integer getInstanceID() {
		return instanceID;
	}
	public void setInstanceID(Integer instanceID) {
		this.instanceID = instanceID;
	}
	public String getInstanceName() {
		return instanceName;
	}
	public void setInstanceName(String instanceName) {
		this.instanceName = instanceName;
	}
	public Long getLogSize() {
		if ( isEnabled() || logSize == null)
			return logSize;
		else
			return -1L;
	}
	public void setLogSize(Long logSize) {
		this.logSize = logSize;
	}
	public String getSqlVersion() {
		return sqlVersion;
	}
	public void setSqlVersion(String sqlVersion) {
		this.sqlVersion = sqlVersion;
	}
	
	public Long getTransactionsPerSec() {
		if ( isEnabled() || transactionsPerSec == null)
			return transactionsPerSec;
		else
			return -1L;
	}
	public void setTransactionsPerSec(Long transactionsPerSec) {
		this.transactionsPerSec = transactionsPerSec;
	}
	public Integer getResponseTime() {
		if ( isEnabled() || responseTime == null)
			return responseTime;
		else
			return -1;
	}
	public void setResponseTime(Integer responseTime) {
		this.responseTime = responseTime;
	}
	public Long getTransactions() {
		if ( isEnabled() || transactions == null)
			return transactions;
		else
			return -1L;
	}
	public void setTransactions(Long transactions) {
		this.transactions = transactions;
	}
	
	public Integer getInstanceState()
	{
		return instanceState;
	}
	
	public void setInstanceState(Integer instanceState)
	{
		this.instanceState = instanceState;
	}
		
	public Status getStatus() {
		
		if (GroupBy.INSTANCE.equals(getMainEntry())){
			return InstanceStatus.getInstanceStatus(true, isEnabled(), getInstanceState());	
		}
		else if (GroupBy.DATABASE.equals(getMainEntry())){
			return DatabaseStatus.getDatabaseStatus(isEnabled(), getAvailability());
		}
		
		return null;
	}
}