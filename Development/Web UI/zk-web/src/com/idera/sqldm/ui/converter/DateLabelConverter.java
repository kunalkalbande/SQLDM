package com.idera.sqldm.ui.converter;

import java.util.Calendar;
import java.util.Date;
import java.util.TimeZone;

import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

import com.idera.sqldm.utils.Utility;

public class DateLabelConverter implements TypeConverter {

	//private static final Date DEFAULT_API_DATE = Utility.getDate(1900, 0, 1);
	
	@Override
	public Object coerceToBean(Object arg0, Component arg1) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component comp) {
		if( obj == null || !(obj instanceof Date) ) return "N/A";
		
		Date date = (Date)obj;
		Calendar cal = Calendar.getInstance();
		cal.setTime(date);
		if (cal.get(Calendar.YEAR) <= 1900) { //Changed to <= (Less Than or Equal To) because the current default year got from the REST API is 1899
			return "N/A";
		}
		
		return Utility.getLocalDate(date);
	}

}
