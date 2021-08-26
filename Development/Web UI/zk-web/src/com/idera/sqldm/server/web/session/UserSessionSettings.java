package com.idera.sqldm.server.web.session;

import java.io.Serializable;
import java.util.HashMap;
import java.util.Locale;
import java.util.Map;

import com.idera.core.user.User;


public class UserSessionSettings implements Serializable {
	
	/**
	 * 
	 */
	public static final String USER_SESSION_SETTINGS_PROPERTY = "userSessionSettings";
	private static final long serialVersionUID = 1L;
	public static final String SECURITY_CONTEXT_PROPERTY = "securityContext";
	public static final String SQLDM_USER_SESSION_SETTINGS_PROPERTY = "sqldm-userSessionSettings";
	private Locale selectedLocale;
	private String activeItem = "dashboard";
	private Map<String,String> userData = new HashMap<String,String>();
	
	private User user = null;

	public Locale getSelectedLocale() {
		return selectedLocale;
	}

	public void setSelectedLocale(Locale selectedLocale) {
		this.selectedLocale = selectedLocale;
	}
	
	public User getUser() {
		return user;
	}

	public void setUser(User user) {
		this.user = user;
	}
	
	public String getActiveItem() {
		return activeItem;
	}

	public void setActiveItem(String activeItem) {
		this.activeItem = activeItem;
	}
	
	public void setUserData(String key, String value){
		userData.put(key, value);
	}
	
	public void setCurrentUserData (Map<String,String> newUserData){
		userData.putAll(newUserData);
	}
	
	public String getUserData(String key){
		return userData.get(key);
	}
}
