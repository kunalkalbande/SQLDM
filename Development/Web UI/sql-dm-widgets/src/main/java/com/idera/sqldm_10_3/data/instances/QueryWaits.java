package com.idera.sqldm_10_3.data.instances;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;

import java.util.Date;

@JsonIgnoreProperties(ignoreUnknown = true)
public class QueryWaits {

	@JsonProperty("ApplicationID")
	private Integer ApplicationID;

	@JsonProperty("ApplicationName")
	private String ApplicationName;

	@JsonProperty("DatabaseID")
	private Integer DatabaseID;

	@JsonProperty("DatabaseName")
	private String DatabaseName;

	@JsonProperty("HostID")
	private Integer HostID;

	@JsonProperty("HostName")
	private String HostName;

	@JsonProperty("LoginID")
	private Integer LoginID;

	@JsonProperty("LoginName")
	private String LoginName;

	@JsonProperty("SQLStatementID")
	private Integer SQLStatementID;

	@JsonProperty("SQLStatement")
	private String SQLStatement;

	@JsonProperty("SessionID")
	private Integer SessionID;

	@JsonProperty("StatementUTCStartTime")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date StatementUTCStartTime;

	@JsonProperty("WaitCategoryID")
	private Integer WaitCategoryID;

	@JsonProperty("WaitCategory")
	private String WaitCategory;

	@JsonProperty("WaitType")
	private String WaitType;

	@JsonProperty("WaitTypeID")
	private Integer WaitTypeID;

	@JsonProperty("WaitDurationPerSecond")
	private Double WaitDurationPerSecond;

	public String getDatabaseName() {
		if(DatabaseName ==null){
			DatabaseName="";
		}
		return DatabaseName;
	}

	public String getApplicationName() {
		if(ApplicationName == null)
			{ApplicationName = "";}
		return ApplicationName;
	}

	public Integer getSessionID() {
		return SessionID;
	}

	public String getWaitType() {
		if(WaitType == null)
			WaitType = "";
		return WaitType;
	}

	public String getLoginName() {
		if(LoginName == null)
			LoginName = "";
		return LoginName;
	}

	public String getSQLStatement() {
		if(SQLStatement==null){
			SQLStatement = "";
		}
		return SQLStatement;
	}

	public Double getWaitDurationPerSecond() {
		return WaitDurationPerSecond;
	}

	public String getWaitCategory() {
		if(WaitCategory==null)
			WaitCategory="";
		return WaitCategory;
	}

	public String getHostName() {
		if(HostName == null)
			HostName = "";
		return HostName;
	}

	public Date getStatementUTCStartTime() {
		return StatementUTCStartTime;
	}

	public void setDatabaseName(String databaseName) {
		DatabaseName = databaseName;
	}

	public void setApplicationName(String applicationName) {
		ApplicationName = applicationName;
	}

	public void setStatementUTCStartTime(Date statementUTCStartTime) {
		StatementUTCStartTime = statementUTCStartTime;
	}

	public void setSessionID(Integer sessionID) {
		SessionID = sessionID;
	}

	public void setWaitType(String waitType) {
		WaitType = waitType;
	}

	public void setLoginName(String loginName) {
		LoginName = loginName;
	}

	public void setSQLStatement(String sQLStatement) {
		SQLStatement = sQLStatement;
	}

	public void setWaitDurationPerSecond(Double waitDurationPerSecond) {
		WaitDurationPerSecond = waitDurationPerSecond;
	}

	public void setWaitCategory(String waitCategory) {
		WaitCategory = waitCategory;
	}

	public void setHostName(String hostName) {
		HostName = hostName;
	}

	public Integer getApplicationID() {
		return ApplicationID;
	}

	public void setApplicationID(Integer applicationID) {
		ApplicationID = applicationID;
	}

	public Integer getDatabaseID() {
		return DatabaseID;
	}

	public void setDatabaseID(Integer databaseID) {
		DatabaseID = databaseID;
	}

	public Integer getHostID() {
		return HostID;
	}

	public void setHostID(Integer hostID) {
		HostID = hostID;
	}

	public Integer getLoginID() {
		return LoginID;
	}

	public void setLoginID(Integer loginID) {
		LoginID = loginID;
	}

	public Integer getSQLStatementID() {
		return SQLStatementID;
	}

	public void setSQLStatementID(Integer sQLStatementID) {
		SQLStatementID = sQLStatementID;
	}

	public Integer getWaitCategoryID() {
		return WaitCategoryID;
	}

	public void setWaitCategoryID(Integer waitCategoryID) {
		WaitCategoryID = waitCategoryID;
	}

	public Integer getWaitTypeID() {
		return WaitTypeID;
	}

	public void setWaitTypeID(Integer waitTypeID) {
		WaitTypeID = waitTypeID;
	}

}