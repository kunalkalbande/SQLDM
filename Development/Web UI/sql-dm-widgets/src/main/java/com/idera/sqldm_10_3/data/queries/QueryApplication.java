package com.idera.sqldm_10_3.data.queries;


import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm_10_3.data.instances.ServerStatus;

@JsonIgnoreProperties(ignoreUnknown = true)
public class QueryApplication {
	
	private static final ServerStatus DEFAULT_SERVER_STATUS = new ServerStatus();
	
	@JsonProperty("ApplicationNameID")
	private int applicationId;

	@JsonProperty("ApplicationName")
	private String application;
	
	private boolean checked;
	private boolean visibility;

	public QueryApplication() {	}
	
	public QueryApplication(int applicationId, String applicationName) {
		this.applicationId = applicationId;
		this.application = applicationName;
	}
	
	/**
	 * @return the checked
	 */
	public boolean getChecked() {
		return checked;
	}

	/**
	 * @param checked the checked to set
	 */
	public void setChecked(boolean checked) {
		this.checked = checked;
	}

	public String getApplication() {
		if(application == null || application.equals(""))
			application = "<blank>";
		return application;
	}
	
	public void setApplication(String name) {
		application = name;
	}

	/**
	 * @return the visibility
	 */
	public boolean isVisibility() {
		return visibility;
	}

	/**
	 * @param visibility the visibility to set
	 */
	public void setVisibility(boolean visibility) {
		this.visibility = visibility;
	}

	public int getApplicationId() {
		return applicationId;
	}

	public void setApplicationId(int applicationId) {
		this.applicationId = applicationId;
	}

}
