package com.idera.sqldm_10_3.server.web.session;

import com.idera.core.user.User;
import com.idera.i18n.I18NStrings;
import com.idera.sqldm_10_3.server.web.ELFunctions;
import com.idera.sqldm_10_3.server.web.ELFunctions;
import org.springframework.security.context.SecurityContext;
import org.springframework.security.context.SecurityContextHolder;
import org.zkoss.zk.ui.Session;
import org.zkoss.zk.ui.Sessions;

import java.util.Locale;
import java.util.Map;

public class SessionUtil {
	
	protected static final mazz.i18n.Logger logger = mazz.i18n.LoggerFactory.getLogger(SessionUtil.class);

	public static SecurityContext getSecurityContext() {
		SecurityContext context = SecurityContextHolder.getContext();
		Session session = Sessions.getCurrent();
		if (session != null) {
			if (context != null && context.getAuthentication() != null) {
				session.setAttribute(UserSessionSettings.SECURITY_CONTEXT_PROPERTY,context);
			}
			else {
				SecurityContext c = (SecurityContext) session.getAttribute(UserSessionSettings.SECURITY_CONTEXT_PROPERTY);	
				if (c != null && c.getAuthentication() != null) {
					SecurityContextHolder.getContext().setAuthentication(c.getAuthentication());
				}
			}
		}
		return context;
	}

	public static void setSessionVariable(String key, Object value) {
		Session session = Sessions.getCurrent();
		if (session != null) {
				session.setAttribute(key,value);
		}
	}

	public static Object getSessionVariable(String key) {
		Session session = Sessions.getCurrent();
		Object value = null;
		if (session != null) {
				 value = session.getAttribute(key);
		}
		return value;
	}
	
	public static UserSessionSettings getUserSessionSettings() {
		Session session = Sessions.getCurrent();
		UserSessionSettings settings = (UserSessionSettings) session.getAttribute(UserSessionSettings.USER_SESSION_SETTINGS_PROPERTY);
		
		if (SecurityContextHolder.getContext().getAuthentication() != null) {
			Object principal = SecurityContextHolder.getContext().getAuthentication().getPrincipal();
			
			User user = null;
	
			if( principal instanceof User ) {
				user = (User) principal;
			}
			if( settings == null || settings.getUser() == null ) {
				settings = createUserSessionSettings(user);
			}
			else if( !settings.getUser().equals(user) ) {
				settings.setUser(user);
			}
		}
		return settings;

	}
	
	public static UserSessionSettings createUserSessionSettings(User user) {

		UserSessionSettings settings = new UserSessionSettings();
		settings.setUser(user);
		settings.setSelectedLocale(getSelectedLocale());

		Session session = Sessions.getCurrent();
		session.setAttribute(UserSessionSettings.USER_SESSION_SETTINGS_PROPERTY, settings);

		return settings;
	}
	
	public static Boolean isLoggedIn() {
		User user = getCurrentUser();
		return user != null && user.isEnabled();
	}

	public static User getCurrentUser() {
		User user = getUserSessionSettings().getUser();
		return user;
	}

	public static String getCurrentUsername() {
		User user = getCurrentUser();
		return user == null ? "" : user.getUsername();
	}
	
	public static String getWelcomeUserString() {
		User user = getCurrentUser();
		return user == null ? "" : ELFunctions.getLabelWithParams(I18NStrings.WELCOME_BACK_USER, user.getUsername());
	}
	
	public static Locale getSelectedLocale(){
		return Locale.ENGLISH;
	}
	
	public static String getDataForCurrentUser(String key){
		return getUserSessionSettings().getUserData(key);
	}
	
	public static void setCurrentUserData(String key, String value){
		getUserSessionSettings().setUserData(key, value);
	}
	
	public static void setCurrentUserData (Map<String, String> allUserData){
		getUserSessionSettings().setCurrentUserData (allUserData);
	}
	
	public static boolean userDataContains(String key){
		return getUserSessionSettings().getUserData(key) == null ? false : true;
	}
	

	public static void clearSecurityContext() {
		Session session = Sessions.getCurrent();
		session.removeAttribute(UserSessionSettings.SECURITY_CONTEXT_PROPERTY);
		
	}
	

}
