package com.idera.sqldm.ui.converter;

import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

import com.idera.sqldm.data.LicenseDetails;

public class LicenseStatusConverter implements TypeConverter { 

	@Override
	public Object coerceToBean(Object obj, Component comp) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component comp) {
		if( obj == null || !(obj instanceof LicenseDetails) ) return "";
		
		LicenseDetails ld = (LicenseDetails)obj;

		// server enum is { OK, Expired, CountExceeded, NoValidKeys }
		String status = ld.getStatus();
		return status;
	}

}
