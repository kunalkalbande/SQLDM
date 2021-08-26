package com.idera.sqldm.ui.components.filtering;

import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

public class FilterCountConverter implements TypeConverter {

	public Object coerceToBean(Object arg0, Component arg1) {
		// TODO Auto-generated method stub
		return null;
	}

	public Object coerceToUi(Object value, Component comp) {

		if (value == null || !(value instanceof Integer))
			return "";

		Integer intValue = (Integer) value;

		if (intValue <= 0) {
			return "";
		} else {
			return intValue.toString();
		}
	}

}
