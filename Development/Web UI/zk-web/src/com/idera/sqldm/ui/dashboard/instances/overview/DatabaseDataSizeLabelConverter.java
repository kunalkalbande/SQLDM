package com.idera.sqldm.ui.dashboard.instances.overview;

import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

import com.idera.i18n.I18NStrings;
import com.idera.sqldm.data.databases.InstanceDetailsDatabase;
import com.idera.sqldm.server.web.ELFunctions;

public class DatabaseDataSizeLabelConverter implements TypeConverter {

	@Override
	public Object coerceToBean(Object arg0, Component arg1) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component comp) {

		if( obj == null || !(obj instanceof InstanceDetailsDatabase) ) return ELFunctions.getLabel( I18NStrings.N_A );;

		InstanceDetailsDatabase db = (InstanceDetailsDatabase) obj;
		
		if (db.getIsInstanceEnabled() == null || ! db.getIsInstanceEnabled() || db.getCurrentDataSizeInMb() == null)
			return ELFunctions.getLabel( I18NStrings.N_A );
		else {
			if ( db.getCurrentDataSizeInMb() > 1000 )
				return String.format( "%,.0f", db.getCurrentDataSizeInMb());
			else
				return String.format( "%,.1f", db.getCurrentDataSizeInMb());
		}
	}
}
