package com.idera.sqldm.ui.converter;

import java.text.DateFormat;
import java.util.Date;

import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

public class ShortDateTimeConverter implements TypeConverter {

	@Override
	public Object coerceToBean(Object obj, Component comp) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component comp) {
		if( obj == null || !(obj instanceof Date) ) return "";
		
		Date date = (Date)obj;

		return 	DateFormat.getDateTimeInstance( DateFormat.SHORT, DateFormat.SHORT).format(date);
	}
}
