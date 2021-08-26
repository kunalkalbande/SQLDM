package com.idera.sqldm.server.web.session;

import java.util.Arrays;
import java.util.List;

/**
 * Preferences keys format 
 *         <b>ProductName-PageName-PreferenceKey-Key</b>
 * Sample key:
 *         sql-elements-dashboard-health-check-dismiss-ok-key
 *
 */
public interface UserPreferences {

	// SHOW HEALTH CHECK RECOMMENDATIONS PREFERENCES
	public static final String SQL_ELEMENTS_DASHBOARD_HEALTH_CHECK_DISMISS_KEY = "sql-elements-dashboard-health-check-dismiss-key";

	public static final String SQL_ELEMENTS_INSTANCE_HEALTH_CHECK_DISMISS_KEY = "sql-elements-instance-health-check-dismiss-key";

	public static final String SQL_ELEMENTS_ACTION_ITEM_HEALTH_CHECK_DISMISS_KEY = "sql-elements-action-item-health-check-dismiss-key";

	
	// COMMON PREFERENCES START
	public static final String SQL_ELEMENTS_COMMON_ITEMS_PER_PAGE_KEY = "sql-elements-common-items-per-page-key";
	public static final String SQL_ELEMENTS_COMMON_ITEMS_PER_PAGE_DEFAULT = "10";
	// COMMON PREFERENCES END
	

	// EXPLORER PREFERENCES START
	public static final String SQL_ELEMENTS_EXPLORER_MAX_NUMBER_OF_RESULT_SET_KEY = "sql-elements-explorer-max-number-of-result-set-key";
	public static final String SQL_ELEMENTS_EXPLORER_MAX_NUMBER_OF_RESULT_SET_DEFAULT = "100";
	
	public static final String SQL_ELEMENTS_EXPLORER_ITEMS_PER_PAGE_KEY = "sql-elements-explorer-items-per-page-key";
	public static final String SQL_ELEMENTS_EXPLORER_ITEMS_PER_PAGE_DEFAULT = "7";
	// EXPLORER PREFERENCES END	

	// Dashboard instance preferences
	public static final String SQL_ELEMENTS_INSTANCE_GRID_ITEMS_PER_PAGE_KEY = "sql-elements-instance-grid-items-per-page-key";
	public static final String SQL_ELEMENTS_INSTANCE_GRID_ITEMS_PER_PAGE_DEFAULT = "20";

	// Instance view preferences
	public static final String SQL_ELEMENTS_INSTANCE_VIEW_LIST_PAGE_SIZE_KEY = "sql-elements-instance-view-list-page-size-key";
	public static final String SQL_ELEMENTS_INSTANCE_VIEW_LIST_PAGE_SIZE_DEFAULT = "500";
	
	// History Panel Preferences
	public static final String SQLDM_FRESH_LOGIN ="sqldm_fresh-login";	
	public static List<String> SQLDM_USER_PREFERENCE_LIST = Arrays.asList("sqldm_history-panel_start-date", "sqldm_history-panel_end-date", "sqldm_history-panel_scale","Grid_View_Columns_User_Preference");
}