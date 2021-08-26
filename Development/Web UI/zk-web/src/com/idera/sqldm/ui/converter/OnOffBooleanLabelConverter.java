package com.idera.sqldm.ui.converter;

import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

import com.idera.sqldm.server.web.ELFunctions;
import com.idera.i18n.I18NStrings;

public class OnOffBooleanLabelConverter implements TypeConverter {

	@Override
	public Object coerceToBean(Object arg0, Component arg1) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component comp) {
		if( obj == null || !(obj instanceof Boolean) ) return "";
		return (Boolean) obj ? ELFunctions.getLabel(I18NStrings.ON) : ELFunctions.getLabel(I18NStrings.OFF);
	}

}
