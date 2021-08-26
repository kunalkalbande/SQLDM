package com.idera.sqldm.ui.converter;

import java.util.Collection;

import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

public class NotEmptyCollectionBooleanConverter implements TypeConverter {

	@Override
	public Object coerceToBean(Object arg0, Component arg1) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component comp) {
		if( obj == null || !(obj instanceof Collection<?>) ) return false;
		return !((Collection<?>) obj).isEmpty();
	}

}
