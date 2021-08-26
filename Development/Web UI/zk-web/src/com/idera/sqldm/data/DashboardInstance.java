package com.idera.sqldm.data;


import java.util.ArrayList;
import java.util.HashMap;
import java.util.LinkedHashMap;

import org.apache.log4j.Logger;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.cwf.model.Product;
import com.idera.sqldm.data.instances.Overview;
import com.idera.sqldm.data.instances.ServerStatus;
import com.idera.sqldm.helpers.InstanceHelper;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.utils.Utility;

@JsonIgnoreProperties(ignoreUnknown = true)
public class DashboardInstance {
	
	private static final ServerStatus DEFAULT_SERVER_STATUS = new ServerStatus();
	
	@JsonProperty("ServerStatus") 
	private ServerStatus ServerStatus;
	
	@JsonProperty("Overview") 
	private Overview Overview;

	@JsonProperty("AlertCategoryWiseMaxSeverity") 
	private AlertCategoryWiseMaxSeverity AlertCategoryWiseMaxSeverity;

	private Boolean isInstanceOpenInHeatMap;
	
	//Used in Idera Dashboard widget
	private Product product;
	
	private boolean isSWAInstance = false;
	
	private int swaID;
	
	private String productName;
	private final Logger log = Logger.getLogger(DashboardInstance.class);
	
	public ServerStatus getServerStatus() {
		if(ServerStatus == null) {
			DEFAULT_SERVER_STATUS.setNull(true);
			return DEFAULT_SERVER_STATUS;
		}
		return ServerStatus;
	}
	public Overview getOverview() {
		return Overview;
	}
		
	public String getServerEditionString(){
		String edition = Utility.getSQLServerRelease(getOverview().getProductVersion());
		if (edition == null) {
			return "N/A";
		}
		return edition;
	}
	public String getDiskIOString(){
		if(getOverview() != null && getOverview().getStatistics() != null){
			if(getOverview().getStatistics().getPageReads().equals("0") && getOverview().getStatistics().getPageWrites().equals("0"))
				return "N/A";
			return new StringBuilder().append(getOverview().getStatistics().getPageReads()).append("/").append(getOverview().getStatistics().getPageWrites()).toString();
		}
		return "-/-";
	}
	
	public Integer getUserSessions(){
		if(getOverview() != null && getOverview().getSystemProcesses() != null){
			return getOverview().getSystemProcesses().get("currentUserProcesses");
		}
		return 0;
	}
	
	public Double getMemoryUsage() {
		return getMemoryUsage(2);
	}
	public Double getMemoryUsage(int precision) {
		if(getOverview() != null && getOverview().getTargetServerMemory() != null && getOverview().getTargetServerMemory() != 0d){
			return Utility.round(getOverview().getTotalServerMemory()/getOverview().getTargetServerMemory()*100d, precision);
		}
		return null;
	}
	
	public String getMemoryUsageString() {
		Double memoryUsage = getMemoryUsage(0);
		if (memoryUsage != null) {
			return memoryUsage.intValue() + " %";
		}
		return "N/A";
	}

	
	public String getSessionsCatImage(){
		return InstanceHelper.getCategoryIcons(getServerStatus().getCategories(), "Sessions")+"Thumbnail";
	}
	public String getQueriesCatImage(){
		return InstanceHelper.getCategoryIcons(getServerStatus().getCategories(), "Queries")+"Thumbnail";
	}
	public String getResourcesCatImage(){
		return InstanceHelper.getCategoryIcons(getServerStatus().getCategories(), "Resources")+"Thumbnail";
	}
	public String getDatabasesCatImage(){
		return InstanceHelper.getCategoryIcons(getServerStatus().getCategories(), "Databases")+"Thumbnail";
	}
	public String getServicesCatImage(){
		return InstanceHelper.getCategoryIcons(getServerStatus().getCategories(), "Services")+"Thumbnail";
	}
	public String getLogsCatImage(){
		return InstanceHelper.getCategoryIcons(getServerStatus().getCategories(), "Logs")+"Thumbnail";
	}
	
	public Integer getSessionsMaxSeverity(){
		return InstanceHelper.getCategoryMaxSeverity(getServerStatus().getCategories(), "Sessions");
	}
	public Integer getQueriesMaxSeverity(){
		return InstanceHelper.getCategoryMaxSeverity(getServerStatus().getCategories(), "Queries");
	}
	public Integer getResourcesMaxSeverity(){
		return InstanceHelper.getCategoryMaxSeverity(getServerStatus().getCategories(), "Resources");
	}
	public Integer getDatabasesMaxSeverity(){
		return InstanceHelper.getCategoryMaxSeverity(getServerStatus().getCategories(), "Databases");
	}
	public Integer getServicesMaxSeverity(){
		return InstanceHelper.getCategoryMaxSeverity(getServerStatus().getCategories(), "Services");
	}
	public Integer getLogsMaxSeverity(){
		return InstanceHelper.getCategoryMaxSeverity(getServerStatus().getCategories(), "Logs");
	}

	public Boolean getIsInstanceOpenInHeatMap() {
		return isInstanceOpenInHeatMap;
	}
	public void setIsInstanceOpenInHeatMap(Boolean isInstanceOpenINHeatMap) {
		this.isInstanceOpenInHeatMap = isInstanceOpenINHeatMap;
	}
	
		public String getInstanceName() {
		if(getServerStatus() !=null && getServerStatus().getInstanceName()!=null)
			return getServerStatus().getInstanceName();
		else
			return getOverview().getInstanceName();
	}
	
	// @author Saumyadeep 
	
	
	public String getFriendlyServerName() {
		if(getServerStatus() !=null && getServerStatus().getFriendlyServerName()!=null)
			return getServerStatus().getFriendlyServerName();
		else
			return getOverview().getFriendlyServerName();	
	}
	
	public String getDisplayName() {
		if(getServerStatus() !=null && getServerStatus().getDisplayName()!=null)
			return getServerStatus().getDisplayName();
			
			else
				return getOverview().getDisplayName();
	}
	
	// End
	
	public String getProductVersion() {
		return getOverview().getProductVersion();
	}
	
	public String getSqlServiceStatus() {
		return getOverview().getSqlServiceStatus();
	}
	
	public String getAgentServiceStatus() {
		return getOverview().getAgentServiceStatus();
	}
	public Double getAvailableMemoryInMB(){
		return getOverview().getOsMetricsStatistics().getAvailableMemoryInMB();
	}
	public Integer getBlockedSessions() {
		return getOverview().getSystemProcesses().get("blockedProcesses");
	}
	
	public Double getCpuPercentage() {
		return getOverview().getStatistics().getCpuPercentage();
	}
	public static HashMap<String, ArrayList<String>> getMap() {
		
		LinkedHashMap<String, ArrayList<String>> map = new LinkedHashMap<>();
		ArrayList<String> columnsList = new ArrayList<>();
//		columnsList.add("getInstanceName"); @author Saumyadeep
		columnsList.add("getDisplayName");
		
		map.put(SQLdmI18NStrings.INSTANCE, columnsList); 
		
		addAlertCategoriesColumns(map);
		columnsList = new ArrayList<>();
		columnsList.add("getProductVersion");		
		map.put(SQLdmI18NStrings.INSTANCE_DASHBOARD_VERSION, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getSqlServiceStatus");		
		map.put(SQLdmI18NStrings.INSTANCE_DASHBOARD_STATUS, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getAgentServiceStatus");		
		map.put(SQLdmI18NStrings.AGENT_STATUS, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getAvailableMemoryInMB");		
		map.put(SQLdmI18NStrings.AVAILABLE_MEMORY, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getBlockedSessions");		
		map.put(SQLdmI18NStrings.BLOCKED_SESSIONS, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getCpuPercentage");		
		map.put(SQLdmI18NStrings.CPU_ACTIVITY, columnsList);
		
		return map;
	}
	private static void addAlertCategoriesColumns(LinkedHashMap<String, ArrayList<String>> map) {
		ArrayList<String> columnsList ;
	
		columnsList = new ArrayList<>();
		columnsList.add("getHealthIndex");		
		map.put(SQLdmI18NStrings.HEALTH_INDEX, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getMaxSeverity");		
		map.put(SQLdmI18NStrings.INSTANCE_AlERT, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getCpuMaxAlert");		
		map.put(SQLdmI18NStrings.INSTANCE_CPU, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getMemoryMaxAlert");		
		map.put(SQLdmI18NStrings.INSTANCE_MEMORY, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getIOMaxAlert");		
		map.put(SQLdmI18NStrings.INSTANCE_IO, columnsList);	
		
		columnsList = new ArrayList<>();
		columnsList.add("getDatabasesMaxAlert");		
		map.put(SQLdmI18NStrings.FILTER_DATABASES, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getLogsMaxAlert");		
		map.put(SQLdmI18NStrings.INSTANCE_LOGS, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getQueryMaxAlert");		
		map.put(SQLdmI18NStrings.INSTANCE_QUERIES, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getServicesMaxAlert");		
		map.put(SQLdmI18NStrings.INSTANCE_SERVICES, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getSessionsMaxAlert");		
		map.put(SQLdmI18NStrings.INSTANCE_SESSIONS, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getVirtualizationMaxAlert");		
		map.put(SQLdmI18NStrings.INSTANCE_VIRTUALIZATION, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getOperationalMaxAlert");		
		map.put(SQLdmI18NStrings.INSTANCE_OPERATIONAL, columnsList);
	}
	public Product getProduct() {
		return product;
	}
	public void setProduct(Product product) {
		this.product = product;
	}
	public String getProductName() {
		return productName;
	}
	public void setProductName(String productName) {
		this.productName = productName;
	}
	public boolean getIsSWAInstance() {
		return isSWAInstance;
	}
	public void setIsSWAInstance(boolean isSWAInstance) {
		this.isSWAInstance = isSWAInstance;
	}
	public int getSwaID() {
		return swaID;
	}
	public void setSwaID(int swaID) {
		this.swaID = swaID;
	}
	
	public AlertCategoryWiseMaxSeverity getAlertCategoryWiseMaxSeverity() {
		return AlertCategoryWiseMaxSeverity;
	}

	public String getAlertImage(String category) {
		int sevCode;
		try{
		if(category.equals("MaxSeverity")){
			sevCode = getMaxSeverity();
			//if(this.ServerStatus == null )
				//sevCode = SeverityCodeToStringEnum.OK.getId(); 
		}
		else if((ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_CPU).equalsIgnoreCase(category))){
			sevCode = getCpuMaxAlert();
		}
		else if(ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_MEMORY).equals(category)){
			sevCode = getMemoryMaxAlert();
		}
		else if(("IO").equals(category)){
			sevCode = getIOMaxAlert();
		}
		else if(("Databases").equals(category)){
			sevCode = getDatabasesMaxAlert();
		}
		else if((ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_LOGS)).equals(category)){
			sevCode = getLogsMaxAlert();
		}
		else if((ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_QUERIES)).equals(category)){
			sevCode = getQueryMaxAlert();
		}
		else if((ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_SERVICES)).equals(category)){
			sevCode = getServicesMaxAlert();
		}
		else if((ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_SESSIONS)).equals(category)){
			sevCode = getSessionsMaxAlert();
		}
		else if((ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_VIRTUALIZATION)).equals(category)){
			sevCode = getVirtualizationMaxAlert();
		}
		else if((ELFunctions.getMessage(SQLdmI18NStrings.INSTANCE_OPERATIONAL)).equals(category)){
			sevCode =getOperationalMaxAlert();
		}
		else
			sevCode = SeverityCodeToStringEnum.OK.getId();
		return Utility.getAlertImageURL(sevCode);	
		}catch(Exception ex){
			log.debug("Exception occured during fetching alert type: " + ex);
		}
		return "";

	}
	
	public Double getHealthIndex() {
		return getServerStatus().getHealthIndex();
	}
	public Integer getMaxSeverity() {
		if(ServerStatus == null){
			return getAlertCategoryWiseMaxSeverity().computeMaxSeverity();
		}
		return getServerStatus().getMaxSeverity();
	}
	
	public Integer getCpuMaxAlert() {
		return getAlertCategoryWiseMaxSeverity().getCpu();
	}
	
	public Integer getDatabasesMaxAlert() {
		return getAlertCategoryWiseMaxSeverity().getDatabases();
	}
	public Integer getIOMaxAlert() {
		return getAlertCategoryWiseMaxSeverity().getIO();
	}
	public Integer getLogsMaxAlert() {
		return getAlertCategoryWiseMaxSeverity().getLogs();
	}
	public Integer getMemoryMaxAlert() {
		return getAlertCategoryWiseMaxSeverity().getMemory();
	}
	public Integer getOperationalMaxAlert() {
		return getAlertCategoryWiseMaxSeverity().getOperational();
	}
	public Integer getQueryMaxAlert() {
		return getAlertCategoryWiseMaxSeverity().getQueries();
	}
	public Integer getServicesMaxAlert() {
		return getAlertCategoryWiseMaxSeverity().getServices();
	}
	public Integer getSessionsMaxAlert() {
		return getAlertCategoryWiseMaxSeverity().getSessions();
	}
	public Integer getVirtualizationMaxAlert() {
		return getAlertCategoryWiseMaxSeverity().getVirtualization();
	}
}
