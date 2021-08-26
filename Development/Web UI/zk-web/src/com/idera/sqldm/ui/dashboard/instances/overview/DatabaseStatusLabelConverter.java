package com.idera.sqldm.ui.dashboard.instances.overview;


import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

import com.idera.sqldm.data.databases.InstanceDetailsDatabase;


public class DatabaseStatusLabelConverter implements TypeConverter {

	@Override
	public Object coerceToBean(Object arg0, Component arg1) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component comp) {

		if( obj == null || !(obj instanceof InstanceDetailsDatabase) ) return "";

		InstanceDetailsDatabase db = (InstanceDetailsDatabase) obj;
		
		String key;
		if ( db.getIsInstanceEnabled()==null || ! db.getIsInstanceEnabled() || db.getCurrentDatabaseStatus()==null )
			key = "Disabled";
		else
			key = db.getCurrentDatabaseStatus();

		return key;
	}
}
