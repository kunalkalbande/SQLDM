package com.idera.sqldm.data.queries;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm.data.instances.ServerStatus;
@JsonIgnoreProperties(ignoreUnknown = true)
public class QueryPlan {
	
	private static final ServerStatus DEFAULT_SERVER_STATUS = new ServerStatus();
	@JsonProperty("PlanXML")
	private String planMXL;
	
	@JsonProperty("InstanceID")
	private int instanceID;
	
	@JsonProperty("PlanID")
	private int planID;
	
	@JsonProperty("SQLStatementID")
	private int sqlStatementID;

	@JsonProperty("QueryColumns")
	List<QueryStatementColumns> queryColumns;
	
	@JsonProperty("IsActualPlan")
	boolean isActual;	
	
	public String getPlanMXL() {
		return planMXL;
	}

	public void setPlanMXL(String planMXL) {
		this.planMXL = planMXL;
	}

	public int getInstanceID() {
		return instanceID;
	}

	public void setInstanceID(int instanceID) {
		this.instanceID = instanceID;
	}

	public int getPlanID() {
		return planID;
	}

	public void setPlanID(int planID) {
		this.planID = planID;
	}

	public int getSqlStatementID() {
		return sqlStatementID;
	}

	public void setSqlStatementID(int sqlStatementID) {
		this.sqlStatementID = sqlStatementID;
	}

	public List<QueryStatementColumns> getQueryColumns() {
		return queryColumns;
	}

	public void setQueryColumns(List<QueryStatementColumns> queryColumns) {
		this.queryColumns = queryColumns;
	}

	public boolean isActual() {
		return isActual;
	}

	public void setActual(boolean isActual) {
		this.isActual = isActual;
	}

	
}
