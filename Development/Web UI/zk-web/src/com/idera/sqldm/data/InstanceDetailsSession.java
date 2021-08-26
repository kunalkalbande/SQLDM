package com.idera.sqldm.data;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.LinkedHashMap;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm.data.instances.SessionConnection;
import com.idera.sqldm.data.instances.SessionLock;
import com.idera.sqldm.data.instances.SessionUsage;
import com.idera.sqldm.i18n.SQLdmI18NStrings;

@JsonIgnoreProperties(ignoreUnknown = true)
public class InstanceDetailsSession {
	
	@JsonProperty("connection") 
	private SessionConnection connection;
		
	@JsonProperty("lockInformation") 
	private SessionLock lock;
	
	@JsonProperty("usage") 
	private SessionUsage usage;
	
	@JsonProperty("tempDBUsage") 
	private SessionTempDbUsage tempDBUsage;

	public SessionConnection getConnection() {
		return connection;
	}

	public void setConnection(SessionConnection connection) {
		this.connection = connection;
	}

	public SessionLock getLock() {
		return lock;
	}

	public void setLock(SessionLock lock) {
		this.lock = lock;
	}

	public SessionUsage getUsage() {
		return usage;
	}

	public void setUsage(SessionUsage usage) {
		this.usage = usage;
	}

	public SessionTempDbUsage getTempDBUsage() {
		return tempDBUsage;
	}

	public void setTempDBUsage(SessionTempDbUsage tempDBUsage) {
		this.tempDBUsage = tempDBUsage;
	}
	
	public static HashMap<String, ArrayList<String>> getMap() {
		
		LinkedHashMap<String, ArrayList<String>> map = new LinkedHashMap<>();
		
		ArrayList<String> columnsList = new ArrayList<>();
		
		columnsList.add("getConnection");		
		columnsList.add("getId");		
		map.put(SQLdmI18NStrings.SESSION_ID, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getConnection");		
		columnsList.add("getHost");		
		map.put(com.idera.i18n.I18NStrings.HOST, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getConnection");		
		columnsList.add("getDatabase");		
		map.put(SQLdmI18NStrings.DATABASE, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getConnection");		
		columnsList.add("getStatus");		
		map.put(SQLdmI18NStrings.STATUS, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getUsage");		
		columnsList.add("getOpenTransactions");		
		map.put(SQLdmI18NStrings.SESSION_OPEN_TRANSACTION, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getConnection");		
		columnsList.add("getCommand");		
		map.put(SQLdmI18NStrings.SESSION_COMMAND, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getConnection");		
		columnsList.add("getApplication");		
		map.put(SQLdmI18NStrings.SESSION_APPLICATION, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getLock");		
		columnsList.add("getWaitTime");		
		map.put(SQLdmI18NStrings.SESSION_WAITTIME, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getLock");		
		columnsList.add("getWaitType");		
		map.put(SQLdmI18NStrings.SESSION_WAITTYPE, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getLock");		
		columnsList.add("getResource");		
		map.put(SQLdmI18NStrings.SESSION_RESOURCE, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getUsage");		
		columnsList.add("getCpu");		
		map.put(SQLdmI18NStrings.SESSION_CPU, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getUsage");		
		columnsList.add("getCpuDelta");		
		map.put(SQLdmI18NStrings.SESSION_CPUDELTA, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getUsage");		
		columnsList.add("getIo");		
		map.put(SQLdmI18NStrings.SESSION_IO, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getUsage");		
		columnsList.add("getMemoryUsage");		
		map.put(SQLdmI18NStrings.SESSION_MEMORY_USAGE, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getUsage");		
		columnsList.add("getLoginTime");		
		map.put(SQLdmI18NStrings.SESSION_LOGIN_TIME, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getUsage");		
		columnsList.add("getLastBatch");		
		map.put(SQLdmI18NStrings.SESSION_LAST_BATCH, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getLock");		
		columnsList.add("getBlockedby");		
		map.put(SQLdmI18NStrings.SESSION_BLOCKEDBY, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getLock");		
		columnsList.add("isBlocking");		
		map.put(SQLdmI18NStrings.SESSION_BLOCKING, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getLock");		
		columnsList.add("getCount");		
		map.put(SQLdmI18NStrings.SESSION_COUNT, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getConnection");		
		columnsList.add("getAddress");		
		map.put(SQLdmI18NStrings.SESSION_ADDRESS, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getConnection");		
		columnsList.add("getNetLibrary");		
		map.put(SQLdmI18NStrings.SESSION_LIBRARY, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getTempDBUsage");		
		columnsList.add("getVersionStoreElapsedSeconds");
		map.put(SQLdmI18NStrings.SESSION_VERSION_STRORE_SEC, columnsList);
		
		return map;
	}

	
}
