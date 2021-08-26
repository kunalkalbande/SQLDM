package com.idera.sqldm_10_3.data.queries;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm_10_3.data.instances.ServerStatus;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown = true)
public class QueryApplicationDetails {

	private static final ServerStatus DEFAULT_SERVER_STATUS = new ServerStatus();
	@JsonProperty("Application")
	private String applicationName;
	@JsonProperty("ApplicationID")
	private String applicationID;

	private QueryApplication application =  new QueryApplication();
	
	@JsonProperty("UserName")
	private String userName;
	@JsonProperty("UserID")
	private String userID;
	
	private QueryUsers user = new QueryUsers();
	
	@JsonProperty("Client")
	private String clientName;
	@JsonProperty("ClientID")
	private String clientID;
	
	private QueryClients client = new QueryClients();
	
	@JsonProperty("DatabaseName")
	private String databaseName;
	@JsonProperty("DatabaseID")
	private String databaseID;
	
	private QueryDatabases database = new QueryDatabases();
	
	@JsonProperty("Occurrences")
	private int occurrences;
	
	@JsonProperty("TotalDuration")
	private long totalDuration;
	
	@JsonProperty("AvgDuration")
	private double avgDuration;
	
	@JsonProperty("TotalCPUTime")
	private long totalCPUTime;
	
	@JsonProperty("AvgCPUTime")
	private double avgCPUTime;
	
	//TODO change it to AvgCPUPerSec
	@JsonProperty("AvgCPUTimePerSec")
	private double avgCPUPerSec;
	
	@JsonProperty("TotalReads")
	private long totalReads;
	
	@JsonProperty("AvgReads")
	private double avgReads;
	
	@JsonProperty("TotalWrites")
	private long totalWrites;
	
	@JsonProperty("AvgWrites")
	private double avgWrites;
	
	@JsonProperty("TotalIO")
	private long totalIO;
	
	@JsonProperty("AvgIO")
	private double avgIO;
	
	//TODO confirm from Rob
	/*	@JsonProperty("AvgIO")*/
	private double avgIOPerSec;
	
	@JsonProperty("TotalWaitTime")
	private long totalWaitTime;
	
	@JsonProperty("AvgWaitTime")
	private double avgWaitTime;
	
	@JsonProperty("MostRecentCompletion")
	private String mostRecentCompletion;
	
	@JsonProperty("TotalBlockingTime")
	private long totalBlockingTime;
	
	@JsonProperty("AvgBlockingTime")
	private double avgBlockingTime;
	
	@JsonProperty("TotalDeadlocks")
	private long totalDeadlocks;
	
	@JsonProperty("AvgDeadlocks")
	private double avgDeadlocks;
	
	//TODO Rob @JsonProperty("")
	private double cpuAsPercentOfList;
	
	//TODO Rob @JsonProperty("")
	private double readsAsPercentOfList;
	
	@JsonProperty("SignatureSQLText")
	private String signatureSQLText;
	
	@JsonProperty("SQLSignatureID")
	private int sqlSignatureID;
	
	@JsonProperty("EventType")
	private int eventType;
	
	@JsonProperty("KeepDetailedHistory")
	private boolean keepDetailedHistoryFlag;

	@JsonProperty("Aggregated")
	private boolean aggregated;
	
	@JsonProperty("QueryName")
	private String queryName;
	
	@JsonProperty("StatementSQLText")
	private String sqlText;
	
	@JsonProperty("SQLStatementID")
	private int sqlStatementID;

	@JsonProperty("QueryStatisticsID")
	private int queryStatisticsID;

	@JsonProperty("Spid")
	private String spid;

	@JsonProperty("StartTime")
	private String startTime;
	
	private String queryNum;

	public boolean getAggregated() {
		return aggregated;
	}
	
	public void setAggregated(boolean aggregated) {
		this.aggregated = aggregated;
	}
	
	public QueryApplication getApplication() {
		application.setApplication(applicationName);
		application.setApplicationId(Integer.parseInt(applicationID));
		return application;
	}

	public void setApplication(QueryApplication application) {
		this.application = application;
	}

	public QueryUsers getUser() {
		user.setUser(userName);
		user.setUserId(Integer.parseInt(userID));
		return user;
	}

	public void setUser(QueryUsers user) {
		this.user = user;
	}

	public QueryClients getClient() {
		client.setClient(clientName);
		client.setClientId(Integer.parseInt(clientID));
		return client;
	}

	public void setClient(QueryClients client) {
		this.client = client;
	}

	public QueryDatabases getDatabase() {
		database.setDatabase(databaseName);
		database.setDatabaseId(Integer.parseInt(databaseID));
		return database;
	}

	public void setDatabase(QueryDatabases database) {
		this.database = database;
	}

	public int getOccurrences() {
		return occurrences;
	}

	public void setOccurrences(int occurrences) {
		this.occurrences = occurrences;
	}

	public long getTotalDuration() {
		return totalDuration;
	}

	public void setTotalDuration(long totalDuration) {
		this.totalDuration = totalDuration;
	}

	public double getAvgDuration() {
		return avgDuration;
	}

	public void setAvgDuration(double avgDuration) {
		this.avgDuration = avgDuration;
	}

	public long getTotalCPUTime() {
		return totalCPUTime;
	}

	public void setTotalCPUTime(long totalCPUTime) {
		this.totalCPUTime = totalCPUTime;
	}

	public double getAvgCPUTime() {
		return avgCPUTime;
	}

	public void setAvgCPUTime(double avgCPUTime) {
		this.avgCPUTime = avgCPUTime;
	}

	public double getAvgCPUPerSec() {
		return avgCPUPerSec;
	}

	public void setAvgCPUPerSec(double avgCPUPerSec) {
		this.avgCPUPerSec = avgCPUPerSec;
	}

	public long getTotalReads() {
		return totalReads;
	}

	public void setTotalReads(long totalReads) {
		this.totalReads = totalReads;
	}

	public double getAvgReads() {
		return avgReads;
	}

	public void setAvgReads(double avgReads) {
		this.avgReads = avgReads;
	}

	public long getTotalWrites() {
		return totalWrites;
	}

	public void setTotalWrites(long totalWrites) {
		this.totalWrites = totalWrites;
	}

	public double getAvgWrites() {
		return avgWrites;
	}

	public void setAvgWrites(double avgWrites) {
		this.avgWrites = avgWrites;
	}

	public long getTotalIO() {
		return totalIO;
	}

	public void setTotalIO(long totalIO) {
		this.totalIO = totalIO;
	}

	public double getAvgIO() {
		return avgIO;
	}

	public void setAvgIO(double avgIO) {
		this.avgIO = avgIO;
	}

	public double getAvgIOPerSec() {
		return avgIOPerSec;
	}

	public void setAvgIOPerSec(double avgIOPerSec) {
		this.avgIOPerSec = avgIOPerSec;
	}

	public long getTotalWaitTime() {
		return totalWaitTime;
	}

	public void setTotalWaitTime(long totalWaitTime) {
		this.totalWaitTime = totalWaitTime;
	}

	public double getAvgWaitTime() {
		return avgWaitTime;
	}

	public void setAvgWaitTime(double avgWaitTime) {
		this.avgWaitTime = avgWaitTime;
	}

	public String getMostRecentCompletion() {
		SimpleDateFormat inputFormat = new SimpleDateFormat("dd-MM-yyyy HH:mm:ss");
		SimpleDateFormat outputFormat = new SimpleDateFormat("MM/dd/yy hh:mm aa");
		
		try {
			return outputFormat.format(inputFormat.parse(mostRecentCompletion));
		} catch (ParseException e) {
			e.printStackTrace();
		}
		
		return (new Date()).toString();
	}

	public void setMostRecentCompletion(String mostRecentCompletion) {
		this.mostRecentCompletion = mostRecentCompletion;
	}

	public long getTotalBlockingTime() {
		return totalBlockingTime;
	}

	public void setTotalBlockingTime(long totalBlockingTime) {
		this.totalBlockingTime = totalBlockingTime;
	}

	public double getAvgBlockingTime() {
		return avgBlockingTime;
	}

	public void setAvgBlockingTime(double avgBlockingTime) {
		this.avgBlockingTime = avgBlockingTime;
	}

	public double getTotalDeadlocks() {
		return totalDeadlocks;
	}

	public void setTotalDeadlocks(long totalDeadlocks) {
		this.totalDeadlocks = totalDeadlocks;
	}

	public double getAvgDeadlocks() {
		return avgDeadlocks;
	}

	public void setAvgDeadlocks(double avgDeadlocks) {
		this.avgDeadlocks = avgDeadlocks;
	}

	public double getCpuAsPercentOfList() {
		return cpuAsPercentOfList;
	}

	public void setCpuAsPercentOfList(double cpuAsPercentOfList) {
		this.cpuAsPercentOfList = cpuAsPercentOfList;
	}

	public double getReadsAsPercentOfList() {
		return readsAsPercentOfList;
	}

	public void setReadsAsPercentOfList(double readsAsPercentOfList) {
		this.readsAsPercentOfList = readsAsPercentOfList;
	}

	public String getSignatureSQLText() {
		return signatureSQLText;
	}

	public void setSignatureSQLText(String signatureSQLText) {
		this.signatureSQLText = signatureSQLText;
	}

	public String getEventType() {
		return QuerySignatureTypeEnum.values()[eventType].getSignatureType();
	}

	public int getEventId() {
		return eventType;
	}

	public void setEventType(int eventType) {
		this.eventType = eventType;
	}

	public boolean isKeepDetailedHistoryFlag() {
		return keepDetailedHistoryFlag;
	}

	public void setKeepDetailedHistoryFlag(boolean keepDetailedHistoryFlag) {
		this.keepDetailedHistoryFlag = keepDetailedHistoryFlag;
	}

	public String getQueryName() {
		return queryName;
	}

	public void setQueryName(String queryName) {
		this.queryName = queryName;
	}

	public String getSqlText() {
		return sqlText;
	}

	public void setSqlText(String sqlText) {
		this.sqlText = sqlText;
	}

	public String getSpid() {
		return spid;
	}

	public void setSpid(String spid) {
		this.spid = spid;
	}

	public String getStartTime() {

		SimpleDateFormat inputFormat = new SimpleDateFormat("dd-MM-yyyy HH:mm:ss");
		SimpleDateFormat outputFormat = new SimpleDateFormat("MM/dd/yy hh:mm aa");
		
		try {
			return outputFormat.format(inputFormat.parse(startTime));
		} catch (ParseException e) {
			e.printStackTrace();
		}
		
		return (new Date()).toString();

	}

	public void setStartTime(String startTime) {
		this.startTime = startTime;
	}

	public static ServerStatus getDefaultServerStatus() {
		return DEFAULT_SERVER_STATUS;
	}

	public String getApplicationName() {
		return applicationName;
	}

	public void setApplicationName(String applicationName) {
		this.applicationName = applicationName;
	}

	public String getApplicationID() {
		return applicationID;
	}

	public void setApplicationID(String applicationID) {
		this.applicationID = applicationID;
	}

	public String getUserName() {
		return userName;
	}

	public void setUserName(String userName) {
		this.userName = userName;
	}

	public String getUserID() {
		return userID;
	}

	public void setUserID(String userID) {
		this.userID = userID;
	}

	public String getClientName() {
		return clientName;
	}

	public void setClientName(String clientName) {
		this.clientName = clientName;
	}

	public String getClientID() {
		return clientID;
	}

	public void setClientID(String clientID) {
		this.clientID = clientID;
	}

	public String getDatabaseName() {
		return databaseName;
	}

	public void setDatabaseName(String databaseName) {
		this.databaseName = databaseName;
	}

	public String getDatabaseID() {
		return databaseID;
	}

	public void setDatabaseID(String databaseID) {
		this.databaseID = databaseID;
	}

	public int getSqlSignatureID() {
		return sqlSignatureID;
	}

	public void setSqlSignatureID(int sqlSignatureID) {
		this.sqlSignatureID = sqlSignatureID;
	}

	public int getSqlStatementID() {
		return sqlStatementID;
	}

	public void setSqlStatementID(int sqlStatementID) {
		this.sqlStatementID = sqlStatementID;
	}

	public int getQueryStatisticsID() {
		return queryStatisticsID;
	}

	public void setQueryStatisticsID(int queryStatisticsID) {
		this.queryStatisticsID = queryStatisticsID;
	}

	public String getQueryNum() {
		return queryNum;
	}

	public void setQueryNum(String queryNum) {
		this.queryNum = queryNum;
	}

}
