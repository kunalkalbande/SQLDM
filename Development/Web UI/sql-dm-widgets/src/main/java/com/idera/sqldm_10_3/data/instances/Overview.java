package com.idera.sqldm_10_3.data.instances;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;
import org.apache.commons.lang.StringUtils;

import java.util.*;

public class Overview {
	
	@JsonProperty ("InstanceName") private String InstanceName;	
	@JsonProperty ("SQLServerId") private Integer SQLServerId;
	@JsonProperty("Tags") private List<String> Tags;

	@JsonProperty("osMetricsStatistics") private OsMetricsStatistics osMetricsStatistics;
	@JsonProperty("targetServerMemory") private Double targetServerMemory;
	@JsonProperty("processorCount") private Integer processorCount;
	@JsonProperty("physicalMemory") private Object physicalMemory;
	@JsonProperty("agentServiceStatus") private String agentServiceStatus;
	@JsonProperty("realServerName") private String realServerName;
	@JsonProperty("loginHasAdministratorRights") private Boolean loginHasAdministratorRights;
	@JsonProperty("statistics") private Statistics statistics;
	@JsonProperty("serverHostName") private String serverHostName;
	@JsonProperty("systemProcesses") private Map<String, Integer> systemProcesses;
	@JsonProperty("TempDBStatistics") private Map<String, Object> TempDBStatistics;
	@JsonProperty("procedureCacheSize") private String procedureCacheSize;
	@JsonProperty("isClustered") private Boolean isClustered;
	@JsonProperty("fullTextServiceStatus") private String fullTextServiceStatus;
	@JsonProperty("TempDBSummary") private TempDBSummary TempDBSummary;
	@JsonProperty("databaseSummary") private DatabaseSummary databaseSummary;
	@JsonProperty("responseTime") private Integer responseTime;
	@JsonProperty("ProductEdition") private Object ProductEdition;
	@JsonProperty("maxConnections") private Integer maxConnections;
	@JsonProperty("processorsUsed") private Integer processorsUsed;
	@JsonProperty("runningSince") 
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date runningSince;
	@JsonProperty("sqlServiceStatus") private String sqlServiceStatus;
	@JsonProperty("sqlActiveDirectoryHelperServiceStatus") private String sqlActiveDirectoryHelperServiceStatus;
	@JsonProperty("sqlBrowserServiceStatus") private String sqlBrowserServiceStatus;
	@JsonProperty("totalServerMemory") private Double totalServerMemory;
	@JsonProperty("sqlServerEdition") private String sqlServerEdition;
	@JsonProperty("dtcServiceStatus") private String dtcServiceStatus;
	@JsonProperty("processorType") private Object processorType;
	@JsonProperty("procedureCachePercentageUsed") private Float procedureCachePercentageUsed;
	@JsonProperty("clusterNodeName") private Object clusterNodeName;
	@JsonProperty("windowsVersion") private String windowsVersion;
	@JsonProperty("language") private Object language;
	@JsonProperty("ProductVersion") private String ProductVersion;
	@JsonProperty("maintenanceModeEnabled") private boolean maintenanceModeEnabled;
	
	// @author Saumyadeep 
	// Friendly Begin
	@JsonProperty("FriendlyServerName")
	private String friendlyServerName;

	public void setFriendlyServerName(String friendlyServerName) {
		this.friendlyServerName = friendlyServerName;
	}
	
	public String getFriendlyServerName() {
		return this.friendlyServerName;

	}
	
	public List<String> getTags() {
		return Tags;
	}

	public void setTags(List<String> tags) {
		Tags = tags;
	}

	private String displayName;
	
	public String getDisplayName() {
		if(this.getFriendlyServerName()!= null && this.getFriendlyServerName() != "" )	
			return this.getFriendlyServerName();
		else
			return this.getInstanceName();
	}

	public void setDisplayName(String displayName) {
		this.displayName = displayName;
	}
	
	// Friendly End
	
	
	public String getInstanceName() {
		return InstanceName;
	}
	public Integer getSQLServerId() {
		return SQLServerId;
	}
	public OsMetricsStatistics getOsMetricsStatistics() {
		return osMetricsStatistics;
	}
	public Double getTargetServerMemory() {
		return targetServerMemory;
	}
	public Integer getProcessorCount() {
		return processorCount;
	}
	public Object getPhysicalMemory() {
		return physicalMemory;
	}
	public String getAgentServiceStatus() {
		String uiLabelFromCode;
		if(agentServiceStatus == null) {
			return agentServiceStatus;
		}
		Status status = Status.findByCode(agentServiceStatus.toLowerCase());  
		if(status == null) {
			uiLabelFromCode = agentServiceStatus;
		} else {
			uiLabelFromCode = status.getUiLabel();
		}
		return uiLabelFromCode;
	}
	public String getRealServerName() {
		return realServerName;
	}
	public Boolean getLoginHasAdministratorRights() {
		return loginHasAdministratorRights;
	}
	public Statistics getStatistics() {
		return statistics;
	}
	public String getServerHostName() {
		return serverHostName;
	}
	public Map<String, Integer> getSystemProcesses() {
		return systemProcesses;
	}
	public Map<String, Object> getTempDBStatistics() {
		return TempDBStatistics;
	}
	public String getProcedureCacheSize() {
		return procedureCacheSize;
	}
	public Boolean getIsClustered() {
		if (isClustered == null) {
			return false;
		} else {
			return isClustered;
		}
	}
	public String getFullTextServiceStatus() {
		return fullTextServiceStatus;
	}
	public TempDBSummary getTempDBSummary() {
		return TempDBSummary;
	}
	public DatabaseSummary getDatabaseSummary() {
		return databaseSummary;
	}
	public Integer getResponseTime() {
		return responseTime;
	}
	public String getResponseTimeString() {
		if (getResponseTime() != null) {
			return getResponseTime().toString() + " ms";
		}
		return "N/A";
	}
	public Object getProductEdition() {
		return ProductEdition;
	}
	public Integer getMaxConnections() {
		return maxConnections;
	}
	public Integer getProcessorsUsed() {
		return processorsUsed;
	}
	public Date getRunningSince() {
		return runningSince;
	}
	public String getSqlServiceStatus() {
		String uiLabelFromCode;
		if(sqlServiceStatus == null) {
			return sqlServiceStatus;
		}
		Status status = Status.findByCode(sqlServiceStatus.toLowerCase());  
		if(status == null) {
			uiLabelFromCode = sqlServiceStatus;
		} else {
			uiLabelFromCode = status.getUiLabel();
		}
		return uiLabelFromCode;
	}
	public Double getTotalServerMemory() {
		return totalServerMemory;
	}
	public String getSqlServerEdition() {
		if(sqlServerEdition == null) {
			sqlServerEdition = "N/A";
		}
		return sqlServerEdition;
	}
	public String getDtcServiceStatus() {
		String uiLabelFromCode;
		if(dtcServiceStatus == null) {
			return dtcServiceStatus;
		}
		Status status = Status.findByCode(dtcServiceStatus.toLowerCase());  
		if(status == null) {
			uiLabelFromCode = dtcServiceStatus;
		} else {
			uiLabelFromCode = status.getUiLabel();
		}
		return uiLabelFromCode;
	}
	public Object getProcessorType() {
		return processorType;
	}
	public Float getProcedureCachePercentageUsed() {
		return procedureCachePercentageUsed;
	}
	public Object getClusterNodeName() {
		return clusterNodeName;
	}
	public String getWindowsVersion() {
		return windowsVersion;
	}
	public Object getLanguage() {
		return language;
	}
	public String getProductVersion() {
		return ProductVersion;
	}
	public String getProductVersionString() {
		if (StringUtils.isNotEmpty(ProductVersion) && !"N/A".equals(ProductVersion)) {
			return "Build " + ProductVersion;
		}
		return ProductVersion;
	}
	private static enum Status {
		UNABLETOMONITOR("UnableToMonitor", "Unable To Monitor"),
		RUNNING("Running", "Running");
		String code;
		String uiLabel;
		private Status(String code, String uiLabel) {
			this.code = code;
			this.uiLabel = uiLabel;
		}
		public String getCode() {
			return code;
		}
		public String getUiLabel() {
			return uiLabel;
		}
		private static final Map<String, Status> lookupByCode = new HashMap<>();

		static {
			for(Status option : EnumSet.allOf(Status.class)) {
				lookupByCode.put(option.getCode().toLowerCase(), option);
			}
		}
		public static Status findByCode(String code) { 
			return lookupByCode.get(code); 
		}	
	}
	public String getSqlActiveDirectoryHelperServiceStatus() {
		return sqlActiveDirectoryHelperServiceStatus;
	}
	public void setSqlActiveDirectoryHelperServiceStatus(
			String sqlActiveDirectoryHelperServiceStatus) {
		this.sqlActiveDirectoryHelperServiceStatus = sqlActiveDirectoryHelperServiceStatus;
	}
	public String getSqlBrowserServiceStatus() {
		return sqlBrowserServiceStatus;
	}
	public void setSqlBrowserServiceStatus(String sqlBrowserServiceStatus) {
		this.sqlBrowserServiceStatus = sqlBrowserServiceStatus;
	}
}
