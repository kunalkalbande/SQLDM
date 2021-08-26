package com.idera.sqldm_10_3.data;

import com.idera.common.rest.RestException;
import com.idera.sqldm_10_3.data.category.CategoryException;
import com.idera.sqldm_10_3.i18n.SQLdmI18NStrings;
import com.idera.sqldm_10_3.rest.SQLDMRestClient;
import com.idera.sqldm_10_3.data.category.CategoryException;
import org.zkoss.json.JSONArray;

import java.util.List;

/*
 * Author:Accolite
 * Date : 15th Nov, 2016
 * History Panel - SQLDM- 10.2 release
 * Load and save the user settings from previous session
 */
public class UserSettingFacade {

	public static String saveUserSettings(String productInstanceName, JSONArray settings) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().saveUserSessionSettings(productInstanceName, settings);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_SAVE_USER_SETTINGS);
		}
		
	}
	
	public static List<UserSettings> getUserSettings(String productInstanceName) throws CategoryException {
		try {
			return SQLDMRestClient.getInstance().getUserSessionSettings(productInstanceName);
		} catch (RestException e) {
			throw new CategoryException(e, SQLdmI18NStrings.ERROR_OCCURRED_SAVE_USER_SETTINGS);
		}
		
	}

}
