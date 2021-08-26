package com.idera.sqldm.data.queries;

import java.util.ArrayList;
import java.util.List;

import com.idera.sqldm.data.SQLDMQueryResult;

public class SQLDMQueryResultContainer {

	protected List<SQLDMQueryResult> queryResultList = new ArrayList<SQLDMQueryResult>();
	protected int totalNumberOfEntries;
	
	public List<SQLDMQueryResult> getQueryResultList() {
		return queryResultList;
	}
	public void setQueryResultList(List<SQLDMQueryResult> queryResultList) {
		this.queryResultList = queryResultList;
	}
	public int getTotalNumberOfEntries() {
		return totalNumberOfEntries;
	}
	public void setTotalNumberOfEntries(int totalNumberOfEntries) {
		this.totalNumberOfEntries = totalNumberOfEntries;
	}
}
