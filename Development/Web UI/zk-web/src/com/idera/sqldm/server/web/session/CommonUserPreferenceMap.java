package com.idera.sqldm.server.web.session;

import java.util.HashMap;
import java.util.Map;

public abstract class CommonUserPreferenceMap {
	
	public static final Map<String, String> COMMON_PREFERENCE_MAP = new HashMap<String,String>();
	static { 
		COMMON_PREFERENCE_MAP.put(UserPreferences.SQL_ELEMENTS_COMMON_ITEMS_PER_PAGE_KEY, UserPreferences.SQL_ELEMENTS_COMMON_ITEMS_PER_PAGE_DEFAULT);
	}

}
