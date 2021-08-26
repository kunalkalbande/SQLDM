package com.idera.sqldm_10_3.ui.dashboard;

import com.idera.cwf.model.Product;
import com.idera.sqldm_10_3.data.AlertCategoryWiseMaxSeverity;
import com.idera.sqldm_10_3.data.SeverityCodeToStringEnum;
import com.idera.sqldm_10_3.i18n.SQLdmI18NStrings;
import com.idera.sqldm_10_3.server.web.ELFunctions;
import com.idera.sqldm_10_3.utils.Utility;

public class DashboardInstanceWrapper {
	private int instanceId;
	private String instanceName;
	private String version;
	private String status;
	private String agentStatus;
	private String dtcStatus;
	private Double availableMemory;
	private Integer blockedProcesses;
	private Double cpuActivity;
	private Integer diskQueueLength;
	private String tag;
	private Integer severity;
	private Product product;
	private double healthIndex;
	private AlertCategoryWiseMaxSeverity alertCategoryWiseSeverity;
	boolean isSwaInstance = false;
	// @author Saumyadeep 
	// Friendly Begin 
	
	private String friendlyServerName;
	
	public String getFriendlyServerName() {
		return this.friendlyServerName;
	}
	public void setFriendlyServerName(String friendlyServerName) {
		this.friendlyServerName = friendlyServerName;
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
	
	// Friendly End -- More work to be done. Change Contrstuctor 
	
//	public DashboardInstanceWrapper(int instanceId, String instanceName, String version,
//			String status, String agentStatus, String dtcStatus,
//			Double availableMemory, Integer blockedProcesses, Double cpuActivity,
//			Integer diskQueueLength, String tag, Integer severity , Product product) {
//		super();
//		this.instanceId = instanceId;
//		this.instanceName = instanceName;
//		this.version = version;
//		this.status = status;
//		this.agentStatus = agentStatus;
//		this.dtcStatus = dtcStatus;
//		this.availableMemory = availableMemory;
//		this.blockedProcesses = blockedProcesses;
//		this.cpuActivity = cpuActivity;
//		this.diskQueueLength = diskQueueLength;
//		this.tag = tag;
//		this.severity = severity;
//		this.product = product;
//	}
	public DashboardInstanceWrapper(int instanceId, String instanceName, String version,
                                    String status, String agentStatus, String dtcStatus,
                                    Double availableMemory, Integer blockedProcesses, Double cpuActivity,
                                    Integer diskQueueLength, String tag, Integer severity , Product product, String displayName) {
		super();
		this.instanceId = instanceId;
		this.instanceName = instanceName;
		this.version = version;
		this.status = status;
		this.agentStatus = agentStatus;
		this.dtcStatus = dtcStatus;
		this.availableMemory = availableMemory;
		this.blockedProcesses = blockedProcesses;
		this.cpuActivity = cpuActivity;
		this.diskQueueLength = diskQueueLength;
		this.tag = tag;
		this.severity = severity;
		this.product = product;
		this.displayName = displayName;
	}

	public DashboardInstanceWrapper(int instanceId, String instanceName, String version,
                                    String status, String agentStatus, String dtcStatus,
                                    Double availableMemory, Integer blockedProcesses, Double cpuActivity,
                                    Integer diskQueueLength, String tag, Integer severity , Product product, String displayName,
                                    double healthIdx, AlertCategoryWiseMaxSeverity alertCategoryWiseMaxSeverity,
                                    boolean isSwaInstance) {
		super();
		this.instanceId = instanceId;
		this.instanceName = instanceName;
		this.version = version;
		this.status = status;
		this.agentStatus = agentStatus;
		this.dtcStatus = dtcStatus;
		this.availableMemory = availableMemory;
		this.blockedProcesses = blockedProcesses;
		this.cpuActivity = cpuActivity;
		this.diskQueueLength = diskQueueLength;
		this.tag = tag;
		this.severity = severity;
		this.product = product;
		this.displayName = displayName;
		this.setHealthIndex(healthIdx);
		this.alertCategoryWiseSeverity = alertCategoryWiseMaxSeverity;
		this.isSwaInstance =isSwaInstance;

	}

	public int getInstanceId() {
		return instanceId;
	}
	public void setInstanceId(int instanceId) {
		this.instanceId = instanceId;
	}

	public String getInstanceName() {
		return instanceName;
	}
	public void setInstanceName(String instanceName) {
		this.instanceName = instanceName;
	}
	public String getVersion() {
		return version;
	}
	public void setVersion(String version) {
		this.version = version;
	}
	public String getStatus() {
		return status;
	}
	public void setStatus(String status) {
		this.status = status;
	}
	public String getAgentStatus() {
		return agentStatus;
	}
	public void setAgentStatus(String agentStatus) {
		this.agentStatus = agentStatus;
	}
	public String getDtcStatus() {
		return dtcStatus;
	}
	public void setDtcStatus(String dtcStatus) {
		this.dtcStatus = dtcStatus;
	}
	public Double getAvailableMemory() {
		return availableMemory;
	}
	public void setAvailableMemory(Double availableMemory) {
		this.availableMemory = availableMemory;
	}
	public Integer getBlockedProcesses() {
		return blockedProcesses;
	}
	public void setBlockedProcesses(Integer blockedProcesses) {
		this.blockedProcesses = blockedProcesses;
	}
	public Double getCpuActivity() {
		return cpuActivity;
	}
	public void setCpuActivity(Double cpuActivity) {
		this.cpuActivity = cpuActivity;
	}
	public Integer getDiskQueueLength() {
		return diskQueueLength;
	}
	public void setDiskQueueLength(Integer diskQueueLength) {
		this.diskQueueLength = diskQueueLength;
	}
	public String getTag() {
		return tag;
	}
	public void setTag(String tag) {
		this.tag = tag;
	}
	public Integer getSeverity() {
		return severity;
	}
	public void setSeverity(Integer severity) {
		this.severity = severity;
	}
	public String getSeverityString() {
		if (severity != null) {
			if (severity == SeverityCodeToStringEnum.CRITICAL.getId()) {
				return SeverityCodeToStringEnum.CRITICAL.getStyleName();
			} else if (severity == SeverityCodeToStringEnum.WARNING.getId()) {
				return SeverityCodeToStringEnum.WARNING.getStyleName();
			} else if (severity == SeverityCodeToStringEnum.INFORMATIONAL.getId()) {
				return SeverityCodeToStringEnum.INFORMATIONAL.getStyleName();
			} else if (severity == SeverityCodeToStringEnum.OK.getId()) {
				return SeverityCodeToStringEnum.OK.getStyleName();
			} else {
				return SeverityCodeToStringEnum.CRITICAL.getStyleName();
			}
		}
		return SeverityCodeToStringEnum.CRITICAL.getStyleName(); //Where severity is not known, showing as critical
	}
	public Product getProduct() {
		return product;
	}
	public void setProduct(Product product) {
		this.product = product;
	}
	
	public AlertCategoryWiseMaxSeverity getAlertCategoryWiseSeverity()
	{
		return alertCategoryWiseSeverity;
	}
	public double getHealthIndex() {
		return healthIndex;
	}
	public void setHealthIndex(double healthIndex) {
		this.healthIndex = healthIndex;
	}
	
	public String getAlertImage(String category){
		int sevCode;
		if(category.equals("MaxSeverity")){
			/*if(this.ServerStatus == null)
				sevCode = SeverityCodeToStringEnum.OK.getId(); //TODO: remove this tempcode
			 */sevCode = this.severity;
		}
		else if((ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_CPU).equalsIgnoreCase(category))){
			sevCode = this.alertCategoryWiseSeverity.getCpu();
		}
		else if((ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_MEMORY).equalsIgnoreCase(category))){
			sevCode = this.alertCategoryWiseSeverity.getMemory();
		}
		else if(("IO").equalsIgnoreCase(category)){
			sevCode = (this.alertCategoryWiseSeverity.getIO());
		}
		else if(("Databases").equalsIgnoreCase(category)){
			sevCode = this.alertCategoryWiseSeverity.getDatabases();
		}
		else if((ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_LOGS)).equalsIgnoreCase(category)){
			sevCode = this.alertCategoryWiseSeverity.getLogs();
		}
		else if((ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_QUERIES)).equalsIgnoreCase(category)){
			sevCode = this.alertCategoryWiseSeverity.getQueries();
		}
		else if((ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_SERVICES)).equalsIgnoreCase(category)){
			sevCode = this.alertCategoryWiseSeverity.getServices();
		}
		else if((ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_SESSIONS)).equalsIgnoreCase(category)){
			sevCode = this.alertCategoryWiseSeverity.getSessions();
		}
		else if((ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_VIRTUALIZATION)).equalsIgnoreCase(category)){
			sevCode = this.alertCategoryWiseSeverity.getVirtualization();
		}
		else if((ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_OPERATIONAL)).equalsIgnoreCase(category)){
			sevCode = this.alertCategoryWiseSeverity.getOperational();
		}
		else
			sevCode = SeverityCodeToStringEnum.OK.getId();
		return Utility.getAlertImageURL(sevCode);

	}
	public boolean isSwaInstance() {
		return isSwaInstance;
	}
	public void setSwaInstance(boolean isSwaInstance) {
		this.isSwaInstance = isSwaInstance;
	}
	
}
