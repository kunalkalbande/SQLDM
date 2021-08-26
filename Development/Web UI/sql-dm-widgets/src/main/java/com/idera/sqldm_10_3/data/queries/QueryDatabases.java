package com.idera.sqldm_10_3.data.queries;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm_10_3.data.instances.ServerStatus;

@JsonIgnoreProperties(ignoreUnknown = true)
public class QueryDatabases {

	private static final ServerStatus DEFAULT_SERVER_STATUS = new ServerStatus();

	@JsonProperty("DatabaseID")
	private int databaseId;

	@JsonProperty("DatabaseName")
	private String database;

	private boolean checked;
	private boolean visibility;

	public QueryDatabases() {
	}

	public QueryDatabases(int databaseId, String databaseName) {
		this.databaseId = databaseId;
		this.database = databaseName;
	}

	/**
	 * @return the checked
	 */
	public boolean getChecked() {
		return checked;
	}

	/**
	 * @param checked
	 *            the checked to set
	 */
	public void setChecked(boolean checked) {
		this.checked = checked;
	}

	public String getDatabase() {
		if(database == null || database.equals(""))
			database = "<blank>";
		return database;
	}

	public void setDatabase(String name) {
		database = name;
	}

	/**
	 * @return the visibility
	 */
	public boolean isVisibility() {
		return visibility;
	}

	/**
	 * @param visibility
	 *            the visibility to set
	 */
	public void setVisibility(boolean visibility) {
		this.visibility = visibility;
	}

	/**
	 * @return the databaseId
	 */
	public int getDatabaseId() {
		return databaseId;
	}

	/**
	 * @param databaseId
	 *            the databaseId to set
	 */
	public void setDatabaseId(int databaseId) {
		this.databaseId = databaseId;
	}
}
