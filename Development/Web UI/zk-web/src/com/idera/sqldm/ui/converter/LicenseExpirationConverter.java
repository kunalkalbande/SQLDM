package com.idera.sqldm.ui.converter;

import java.text.DateFormat;

import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

import com.idera.sqldm.data.LicenseDetails;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.i18n.I18NStrings;

public class LicenseExpirationConverter implements TypeConverter { 

	@Override
	public Object coerceToBean(Object obj, Component comp) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component comp) {
		if( obj == null || !(obj instanceof LicenseDetails) ) return "";
		
		LicenseDetails ld = (LicenseDetails)obj;

		return 	(ld.isPermanent() ? ELFunctions.getLabel(I18NStrings.LICENSE_NEVER_EXPIRES) : DateFormat.getDateInstance().format(ld.getExpiration()));
	}

}
