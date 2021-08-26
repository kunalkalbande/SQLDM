package com.idera.sqldm_10_3.server.web;

public interface WebConstants {

	public static final String BASE_IMAGE_URL = "~./sqldm/com/idera/sqldm/images";
	public static final String DEFAULT_IMAGE_EXT = ".png";

	public static final String STAGE_INDICATOR_IMAGE_RUNNING = "process";
	public static final String STAGE_INDICATOR_IMAGE_FINISHED = "check";
	public static final String STAGE_INDICATOR_IMAGE_TODO = "clipboard-empty";
	public static final String STAGE_INDICATOR_IMAGE_ERROR = "error";

	public static final String STAGE_INDICATOR_IMAGE_SIZE = "small";

	public static final String AJAX_INDICATOR_NEUTRAL = "neutral";
	public static final String AJAX_INDICATOR_ACTIVE = "enabled";

	public static final String DEFAULT_SECURITY_REDIRECT = "/";
	public static final String SPRING_LOGOUT_REDIRECT = "/j_spring_security_logout";

	public static final String LICENSE_EXPIRED_REDIRECT = "/licenseExpired.zul";

	public static final String DOUBLE_DASH_STRING = "--";
	public static final String X_OUT_OF_Y_FORMAT = "%d / %d";
	public static final String VALUE_PAREN_FORMAT = "%s (%s)";

	public static final String EMPTY_STRING = "";

	public static final String ZK_DATABOUND_TEMPLATE_ATTR = "zkplus.databind.TEMPLATE";

	public static final String RECOMMENDATION_REFRESH_EVENT_QUEUE	= "recommendation-refresh-event-queue";
	public static final String RECOMMENDATION_REFRESH_EVENT			= "recommendation-refresh-event";
	public static final String RECOMMENDATIONS_DISMISSED_EVENT		= "onRecommendationsDismissed";
	public static final String RECOMMENDATION_DISMISSED_EVENT		= "onRecommendationDismissed";
	public static final String RECOMMENDATIONS_REFRESHED_EVENT		= "onRecommendationsRefreshed";
	public static final String RECOMMENDATION_REFRESHED_EVENT		= "onRecommendationRefreshed";

	public static final String DASHBOARD_INSTANCE_REFRESH_EVENT_QUEUE = "dashboard-instance-refresh-event-queue";
	public static final String DASHBOARD_INSTANCE_REFRESH_EVENT = "dashboard-instance-refresh-event";
	
	public static final String DASHBOARD_LICENSE_REFRESH_EVENT_QUEUE = "dashboard-license-refresh-event-queue";
	public static final String DASHBOARD_LICENSE_REFRESH_EVENT = "dashboard-license-refresh-event";

	public static final String INSTANCE_PROPERTIES_REFRESH_EVENT_QUEUE = "instance-properties-refresh-event-queue";
	public static final String INSTANCE_PROPERTIES_REFRESH_EVENT = "instance-properties-refresh-event";

	public static final String TAG_REFRESH_EVENT_QUEUE = "tag-refresh-event-queue";
	public static final String TAG_REFRESH_EVENT = "tag-refresh-event";
	
	public static final String DASHBOARD_SIDEBAR_EVENT_QUEUE = "dashboard-sidebar-event-queue";
	public static final String AUTO_REGISTRATION_CHANGED_EVENT = "onAutoRegistrationChanged";
	public static final String LOGIN_FAILED_REDIRECT = "/login.zul?failed=true";
	
}