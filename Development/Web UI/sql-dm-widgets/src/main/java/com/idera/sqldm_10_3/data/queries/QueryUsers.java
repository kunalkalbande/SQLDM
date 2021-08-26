package com.idera.sqldm_10_3.data.queries;


import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm_10_3.data.instances.ServerStatus;

@JsonIgnoreProperties(ignoreUnknown = true)
public class QueryUsers {
	
	private static final ServerStatus DEFAULT_SERVER_STATUS = new ServerStatus();
	@JsonProperty("UserID")
	private int userId;
	
	@JsonProperty("UserName")
	private String user;
	
	private boolean checked;
	private boolean visibility;

	public QueryUsers() {	}
	
	public QueryUsers(int userId, String userName) {
		this.userId = userId;
		this.user = userName;
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

	public String getUser() {
		if(user == null || user.equals(""))
			user = "<blank>";
		return user;
	}
	
	public void setUser(String name) {
		user = name;
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
	 * @return the userId
	 */
	public int getUserId() {
		return userId;
	}

	/**
	 * @param userId the userId to set
	 */
	public void setUserId(int userId) {
		this.userId = userId;
	}
}
