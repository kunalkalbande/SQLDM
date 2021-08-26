package com.idera.sqldm.data.queries;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm.data.instances.ServerStatus;
@JsonIgnoreProperties(ignoreUnknown = true)
public class QueryStatementColumns {
	
	private static final ServerStatus DEFAULT_SERVER_STATUS = new ServerStatus();
	@JsonProperty("Column")
	private String columns;
	
	@JsonProperty("Database")
	private String database;
	
	@JsonProperty("Schema")
	private String schema;
	
	@JsonProperty("Table")
	private String table;

	public String getColumns() {
		return columns;
	}

	public void setColumns(String columns) {
		this.columns = columns;
	}

	public String getDatabase() {
		return database;
	}

	public void setDatabase(String database) {
		this.database = database;
	}

	public String getSchema() {
		return schema;
	}

	public void setSchema(String schema) {
		this.schema = schema;
	}

	public String getTable() {
		return table;
	}

	public void setTable(String table) {
		this.table = table;
	}
	
	
}
