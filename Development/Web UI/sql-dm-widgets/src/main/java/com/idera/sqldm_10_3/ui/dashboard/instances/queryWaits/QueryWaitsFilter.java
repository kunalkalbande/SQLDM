package com.idera.sqldm_10_3.ui.dashboard.instances.queryWaits;


public class QueryWaitsFilter implements Cloneable {
	static QueryWaitsFilter DEFAULT_FILTER = new QueryWaitsFilter();
	
	int selectedId;
	String filterName;
	
	public QueryWaitsFilter() {
		
	}

	@Override
	public Object clone() throws CloneNotSupportedException {
		return super.clone();
	}

	public int getSelectedId() {
		return selectedId;
	}

	public void setSelectedId(int selectedId) {
		this.selectedId = selectedId;
	}

	public String getFilterName() {
		return filterName;
	}

	public void setFilterName(String filterName) {
		this.filterName = filterName;
	}


}
