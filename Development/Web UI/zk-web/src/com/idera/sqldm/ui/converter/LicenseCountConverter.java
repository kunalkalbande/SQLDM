package com.idera.sqldm.ui.converter;

import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

import com.idera.sqldm.data.LicenseDetails;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.i18n.I18NStrings;

public class LicenseCountConverter implements TypeConverter { 

	@Override
	public Object coerceToBean(Object obj, Component comp) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component comp) {
		if( obj == null || !(obj instanceof LicenseDetails) ) return "";
		
		LicenseDetails ld = (LicenseDetails)obj;
		
		return (ld.getLicensedServers() == -1 ? ELFunctions.getLabel(I18NStrings.LICENSE_UNLIMITED_SERVERS) : ((Integer)ld.getLicensedServers()).toString());
	}

}
