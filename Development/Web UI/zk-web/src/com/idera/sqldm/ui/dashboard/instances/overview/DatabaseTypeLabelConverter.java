package com.idera.sqldm.ui.dashboard.instances.overview;


import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

import com.idera.sqldm.data.databases.InstanceDetailsDatabase;

public class DatabaseTypeLabelConverter implements TypeConverter {

	@Override
	public Object coerceToBean(Object arg0, Component arg1) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component comp) {

		if( obj == null || !(obj instanceof InstanceDetailsDatabase) ) return "";

		InstanceDetailsDatabase db = (InstanceDetailsDatabase) obj;
		
		return db.getDatabaseType();
	}
}
