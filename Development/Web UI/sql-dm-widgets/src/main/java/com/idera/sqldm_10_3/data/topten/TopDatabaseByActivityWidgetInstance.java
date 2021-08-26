package com.idera.sqldm_10_3.data.topten;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm_10_3.utils.Utility;

@JsonIgnoreProperties(ignoreUnknown=true)
public class TopDatabaseByActivityWidgetInstance extends IWidgetInstance {
	@JsonProperty("DatabaseName") private String databaseName;
	@JsonProperty("TransactionPerSec")private double transactionValue;
	@JsonProperty("Severity")	private int severity;
	public String getDatabaseName() {
		return databaseName;
	}
	public void setDatabaseName(String databaseName) {
		this.databaseName = databaseName;
	}
	public double getTransactionValue() {
		return Utility.round(transactionValue, 2);
	}
	public void setTransactionValue(double transactionValue) {
		this.transactionValue = transactionValue;
	}
	
	public int getSeverity() {
		return severity;
	}
	public void setSeverity(int severity) {
		this.severity = severity;
	}
}
