package com.idera.sqldm_10_3.data.queries;


import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm_10_3.data.instances.ServerStatus;

@JsonIgnoreProperties(ignoreUnknown = true)
public class QueryClients {
	
	private static final ServerStatus DEFAULT_SERVER_STATUS = new ServerStatus();

	@JsonProperty("ClientID")
	private int clientId;
	
	@JsonProperty("ClientName")
	private String client;

	private boolean checked;
	private boolean visibility;

	public QueryClients() {	}
	
	public QueryClients(int clientId, String clientName) {
		this.clientId = clientId;
		this.client = clientName;
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

	public String getClient() {
		if(client == null || client.equals(""))
			client = "<blank>";
		return client;
	}
	
	public void setClient(String name) {
		client = name;
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

	/**
	 * @return the clientId
	 */
	public int getClientId() {
		return clientId;
	}

	/**
	 * @param clientId the clientId to set
	 */
	public void setClientId(int clientId) {
		this.clientId = clientId;
	}

}
