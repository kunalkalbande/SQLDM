package com.idera.sqldm.ui.converter;

import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

public class EnabledDisabledBooleanSclassConverter implements TypeConverter {

	@Override
	public Object coerceToBean(Object obj, Component comp) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component comp) {
		if( obj == null || !(obj instanceof Boolean) ) return "";
		return (Boolean) obj ? "bold-green-text" : "bold-orange-text";
	}

}
