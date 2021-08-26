package com.idera.sqldm.ui.converter;

import java.util.Locale;

import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

import com.idera.sqldm.server.web.session.SessionUtil;

public class ByteSizeConverter implements TypeConverter {

	public static String getString(Object object) {
		return getString(object, SessionUtil.getSelectedLocale());
	}

	public static String getString(Object object, Locale locale) {

		if( object == null ) return "";

		if( object instanceof Long ) {
			Long o = (Long) object;
			return ByteSizeFormatter.getFriendlyString(o, locale);
		}
		else if( object instanceof Integer ) {
			Long o = ((Integer) object).longValue();
			return ByteSizeFormatter.getFriendlyString(o, locale);
		}
		else if( object instanceof Double ) {
			Double o = (Double) object;
			return ByteSizeFormatter.getFriendlyString(o, locale);
		}
		else {
			return "";
		}

	}

	@Override
	public Object coerceToBean(Object arg0, Component arg1) {
		return null;
	}

	@Override
	public Object coerceToUi(Object object, Component arg1) {
		if( object == null ) return "";
		return getString(object, SessionUtil.getSelectedLocale());
	}

}
