package com.idera.sqldm.data.instances;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm.data.AlertCategoryWiseMaxSeverity;

public class Instance {
	@JsonProperty("ServerStatus") private ServerStatus ServerStatus;
	@JsonProperty("Overview") private Overview Overview;
	@JsonProperty("AlertCategoryWiseMaxSeverity") 
	private AlertCategoryWiseMaxSeverity AlertCategoryWiseMaxSeverity;
	
	public ServerStatus getServerStatus() {
		return ServerStatus;
	}
	public Overview getOverview() {
		return Overview;
	}
	public AlertCategoryWiseMaxSeverity getAlertCategoryWiseMaxSeverity() {
		return AlertCategoryWiseMaxSeverity;
	}
}
