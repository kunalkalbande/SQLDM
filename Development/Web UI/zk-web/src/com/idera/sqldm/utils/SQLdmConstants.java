package com.idera.sqldm.utils;

import java.util.regex.Pattern;

import com.idera.GlobalConstants;
import com.idera.sqldm.rest.SQLDMRestClient;

public interface SQLdmConstants {
	
	public static final int CHART_X_AXIS_TICKS_COUNT = 3; 
	public static final String SINGLE_INSTANCE_SUB_TAB_DESELECT_CLASS = "gray-text-transparent-button";
	public static final String SINGLE_INSTANCE_SUB_TAB_SELECT_CLASS = "white-text-dark-gray-button";
	
	public static final String DASHBOARD_SCOPE_ALL_DASHBOARD_INTANCES_LIST = "all_instances";
	public static final String DASHBOARD_SCOPE_DASHBOARD_INSTANCES_LIST  = "m_instances";
	public static final String DASHBOARD_SCOPE_DASHBOARD_ALERTS_LIST  = "dashboard_alerts";
	public static final String DASHBOARD_SCOPE_DASHBOARD_TAGS  = "tags";
	public static final String DASHBOARD_SCOPE_DASHBOARD_SEVERITIES  = "severities";
	public static final String DASHBOARD_SCOPE_DASHBOARD_SEARCH  = "search";
	
	public static final String DASHBOARD_SCOPE_DASHBOARD_WIDGET_ALERT_BY_CATEGORY_SET  = "alertsByCategorySet";
	public static final String DASHBOARD_SCOPE_DASHBOARD_WIDGET_ALERT_BY_DATABASE_SET  = "alertsByDatabaseSet";
	public static final String DASHBOARD_SCOPE_DASHBOARD_WIDGET_ALERT_BY_INSTANCE_SET  = "alertsByInstanceSet";
	public static final String DASHBOARD_SCOPE_DASHBOARD_WIDGET_WORST_RESPONSE_TIME_FROM_DB_SET  = "worstResponseTimeFromDB";
	
	public static final String DASHBOARD_SCOPE_SINGLE_INSTANCE_SESSION  = "SingleInstanceSessionModel";
	public static final String DASHBOARD_SCOPE_SINGLE_INSTANCE_SESSION_CHART  = "SingleInstanceSessionChartModel";
	public static final String DASHBOARD_SCOPE_SINGLE_INSTANCE_RESOURCE = "SingleInstanceResourceModel";
	
	public static final String DASHBOARD_UPDATE_INSTANCES_EVENT_QUEUE  = "updateInstances";
	
	public static final String TOPX_PARALLEL_LOAD_DATA_EVENT_QUEUE  = "topXDataParallelLoadQueue";
	public static final String TOPX_PARALLEL_LOAD_DATA_EVENT  = "topXDataParallelLoad_";
	
	public static final Integer DASHBOARD_RHS_WIDGET_ALERT_BY_INSTANCE_DEFAULT = 5;
	public static final Integer DASHBOARD_RHS_WIDGET_ALERT_BY_DATABASE_DEFAULT = 5;
	public static final Integer DASHBOARD_RHS_WIDGET_ALERT_BY_CATEGORY_DEFAULT = 6;
	public static final Integer DASHBOARD_RHS_WIDGET_RESPONSE_TIME_FROM_DB_DEFAULT = 5;
	
	public static final String DASHBOARD_RHS_WIDGET_ALERT_BY_INSTANCE_STR = "alertsByInstanceDefaultCount";
	public static final String DASHBOARD_RHS_WIDGET_ALERT_BY_DATABASE_STR = "alertsByDBDefaultCount";
	public static final String DASHBOARD_RHS_WIDGET_ALERT_BY_CATEGORY_STR = "alertsByCategoryDefaultCount";
	public static final String DASHBOARD_RHS_WIDGET_RESPONSE_TIME_FROM_DB_STR = "responseTimeFromDBDefaultCount";
	// See: http://www.regular-expressions.info/email.html
	public static final Pattern EMAIL_ADDRESS_PATTERN = Pattern.compile("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$", Pattern.CASE_INSENSITIVE);
	
	// Tag name patterns.
	// leading/trailing spaces in a tag name are ignored.
	// tag name begins with [a-zA-Z], subsequent chars can be any combo of  a-zA-Z0-9-_':()#!.@\s (\s: embedded space)
	// tag name cannot exceed 20 chars
	public static final String SINGLE_TAG_PATTERN = "(\\s*[a-zA-Z0-9][a-zA-Z0-9-_':()#!.@\\s]{0,19}\\s*)";

	// multiple tag names can be specified, they are separated by ;, this constraint is only valid for add
	public static final String MULTI_TAG_SEPERATOR = ";";
	public static final String MULTI_TAG_PATTERN = "(\\s*[a-zA-Z0-9][a-zA-Z0-9-_':()#!.@\\s]{0,19}\\s*)(;((\\s*)|(\\s*[a-zA-Z0-9][a-zA-Z0-9-_':()#!.@\\s]{0,19}\\s*)))*";
	public static final String BLANK_PATTERN = "^\\s*$";
	
	// Used to set "Unlimited" counts for things like max # of agents, add-ons, etc...
	public static final int UNLIMITED_COUNT_DESIGNATOR = GlobalConstants.UNLIMITED_COUNT_DESIGNATOR;
	
	// Login user parameter.
	public static final String LOGIN_USER_PARAMETER = "user";

	// Cookie Stuff
	public static final String IDERA_SQL_DM_COOKIE_NAME = "idera-sqldm";
	public static final String IDERA_SQL_DM_COOKIE_USERNAME_KEY = "idera-sqldm-username";
	public static final String IDERA_COOKIE_SELECTED_LOCALE_KEY = "idera-sqldm-selectedLocale";
	public static final String IDERA_COOKIE_KEY_VALUE_DELIMITER = "=";
	public static final String IDERA_COOKIE_VALUES_DELIMITER = ":";
	public static final Integer COOKIE_MAX_AGE = 365 * 24 * 60 * 60;
	//TODO temporary fix

	public static final String PRODUCT_VERSION = "11.1.0.0";//chagnes from 9.0 to 9.1 by Gaurav Karwal

	//public static final String PRODUCT_VERSION = SQLDMRestClient.getInstance().getVersion();
	public static final String PRODUCT_NAME = "SQLdm";
	public static final String XSLT_PATH = "conf/QueryPlanTransforms.xslt";

	public static final String WIDGET_PARALLEL_LOAD_DATA_EVENT_QUEUE = "widgetParallelLoadDataEventQueue";
}
