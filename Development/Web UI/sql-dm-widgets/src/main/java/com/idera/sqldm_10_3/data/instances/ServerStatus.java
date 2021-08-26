package com.idera.sqldm_10_3.data.instances;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm_10_3.data.SeverityCodeToStringEnum;
import com.idera.sqldm_10_3.utils.Utility;

import java.util.List;

public class ServerStatus {
	private static final String ZERO_ALERT = "0";
	@JsonProperty("UtilizedSpaceInKilobytes") private Object UtilizedSpaceInKilobytes;
	@JsonProperty("MostRecentSQLVersion") private Object MostRecentSQLVersion;
	@JsonProperty("LastRunIOActivityPercentage") private Object LastRunIOActivityPercentage;
	@JsonProperty("SQLServerId") private Integer SQLServerId;
	@JsonProperty("InstanceEdition") private String InstanceEdition;
	@JsonProperty("LastRunSqlMemoryUsedInKilobytes") private Object LastRunSqlMemoryUsedInKilobytes;
	@JsonProperty("AvgDiskTimePercent") private Object AvgDiskTimePercent;
	@JsonProperty("AvgReponseTimeinMilliSeconds") private Object AvgReponseTimeinMilliSeconds;
	@JsonProperty("AvgOSAvailableMemoryInKilobytes") private Object AvgOSAvailableMemoryInKilobytes;
	@JsonProperty("Categories") private List<CategoriesEntry> Categories;
	@JsonProperty("BlockedSessionCount") private Object BlockedSessionCount;
	@JsonProperty("Tags") private List<String> Tags;
	@JsonProperty("MaxSeverity") private Integer MaxSeverity;
	@JsonProperty("Databases") private List<Object> Databases;
	@JsonProperty("LastRunSqlMemoryAllocatedInKilobytes") private Object LastRunSqlMemoryAllocatedInKilobytes;
	@JsonProperty("AvgCPUActivityPercentage") private Object AvgCPUActivityPercentage;
	@JsonProperty("AvgOSTotalPhysicalMemoryInKilobytes") private Object AvgOSTotalPhysicalMemoryInKilobytes;
	@JsonProperty("StatisticsHistory") private List<StatisticsHistory> StatisticsHistory;
	@JsonProperty("DiskUtilizationPercentage") private Object DiskUtilizationPercentage;
	@JsonProperty("IsActive") private Boolean IsActive;
	@JsonProperty("NumOfQueries") private Object NumOfQueries;
	@JsonProperty("LastRunCPUActivityPercentage") private Object LastRunCPUActivityPercentage;
	@JsonProperty("LastRunActiveUserSessions") private Object LastRunActiveUserSessions;
	@JsonProperty("InstanceName") private String InstanceName;
	@JsonProperty("activeCriticalAlertCount") private String activeCriticalAlertCount;
	@JsonProperty("activeInfoAlertCount") private String activeInfoAlertCount;
	@JsonProperty("activeWarningAlertCount") private String activeWarningAlertCount;
	@JsonProperty("HealthIndex") private double healthIndex;
	
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
	
	private boolean isNull ;

	public boolean isNull() {
		return isNull;
	}
	public void setNull(boolean isNull) {
		this.isNull = isNull;
	}
	public Object getUtilizedSpaceInKilobytes() {
		return UtilizedSpaceInKilobytes;
	}
	public Object getMostRecentSQLVersion() {
		return MostRecentSQLVersion;
	}
	public Object getLastRunIOActivityPercentage() {
		return LastRunIOActivityPercentage;
	}
	public Integer getSQLServerId() {
		return SQLServerId;
	}
	public String getInstanceEdition() {
		return InstanceEdition;
	}
	public Object getLastRunSqlMemoryUsedInKilobytes() {
		return LastRunSqlMemoryUsedInKilobytes;
	}
	public Object getAvgDiskTimePercent() {
		return AvgDiskTimePercent;
	}
	public Object getAvgReponseTimeinMilliSeconds() {
		return AvgReponseTimeinMilliSeconds;
	}
	public Object getAvgOSAvailableMemoryInKilobytes() {
		return AvgOSAvailableMemoryInKilobytes;
	}
	public List<CategoriesEntry> getCategories() {
		return Categories;
	}
	public Object getBlockedSessionCount() {
		return BlockedSessionCount;
	}
	public List<String> getTags() {
		return Tags;
	}
	public Integer getMaxSeverity() {
		if (MaxSeverity == null) {
			return SeverityCodeToStringEnum.OK.getId();
		}
		return MaxSeverity;
	}
	public List<Object> getDatabases() {
		return Databases;
	}
	public Object getLastRunSqlMemoryAllocatedInKilobytes() {
		return LastRunSqlMemoryAllocatedInKilobytes;
	}
	public Object getAvgCPUActivityPercentage() {
		return AvgCPUActivityPercentage;
	}
	public Object getAvgOSTotalPhysicalMemoryInKilobytes() {
		return AvgOSTotalPhysicalMemoryInKilobytes;
	}
	public List<StatisticsHistory> getStatisticsHistory() {
		return StatisticsHistory;
	}
	public Object getDiskUtilizationPercentage() {
		return DiskUtilizationPercentage;
	}
	public Boolean getIsActive() {
		return IsActive;
	}
	public Object getNumOfQueries() {
		return NumOfQueries;
	}
	public Object getLastRunCPUActivityPercentage() {
		return LastRunCPUActivityPercentage;
	}
	public Object getLastRunActiveUserSessions() {
		return LastRunActiveUserSessions;
	}
	public String getInstanceName() {
		return InstanceName;
	}
	
	public String getActiveCriticalAlertCount() {
		if(activeCriticalAlertCount ==null){
			return ZERO_ALERT;
		}
		return activeCriticalAlertCount;
	}
	public String getActiveInfoAlertCount() {
		if(activeInfoAlertCount ==null){
			return ZERO_ALERT;
		}
		return activeInfoAlertCount;
	}
	public String getActiveWarningAlertCount() {
		if(activeWarningAlertCount ==null){
			return ZERO_ALERT;
		}
		return activeWarningAlertCount;
	}
	
	
	/*Start: Should be included manually whenever contract changes*/
	public Double getMemoryUsedByAllcocatedPercentage() {
		try {
			return Utility.round((Double)LastRunSqlMemoryUsedInKilobytes/(Double)LastRunSqlMemoryAllocatedInKilobytes*100, 2);
		} catch (Exception e) {
			//log error
		}
		return 0d;
	}
	
	public String getSeverityString() {
		if (MaxSeverity != null) {
			if (MaxSeverity == SeverityCodeToStringEnum.CRITICAL.getId()) {
				return SeverityCodeToStringEnum.CRITICAL.getStyleName();
			} else if (MaxSeverity == SeverityCodeToStringEnum.WARNING.getId()) {
				return SeverityCodeToStringEnum.WARNING.getStyleName();
			} else if (MaxSeverity == SeverityCodeToStringEnum.INFORMATIONAL.getId()) {
				return SeverityCodeToStringEnum.INFORMATIONAL.getStyleName();
			} else if (MaxSeverity == SeverityCodeToStringEnum.OK.getId()) {
				return SeverityCodeToStringEnum.OK.getStyleName();
			} else {
				return SeverityCodeToStringEnum.CRITICAL.getStyleName();
			}
		}
		return SeverityCodeToStringEnum.CRITICAL.getStyleName(); //Where severity is not known, showing as critical
	 }
	public double getHealthIndex() {
		//round to two decimal places
		return Math.round(healthIndex*100.0)/100.;
	}
	public void setHealthIndex(double healthIndex) {
		this.healthIndex = healthIndex;
	}
	
	/*End: Should be included manually whenever contract changes*/
}
