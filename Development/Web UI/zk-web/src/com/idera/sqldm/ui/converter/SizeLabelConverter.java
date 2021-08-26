package com.idera.sqldm.ui.converter;

import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

import com.idera.i18n.I18NStrings;
import com.idera.sqldm.server.web.ELFunctions;

public class SizeLabelConverter implements TypeConverter {

	@Override
	public Object coerceToBean(Object arg0, Component arg1) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component comp) {

		if( obj == null || !(obj instanceof Number)) return ELFunctions.getLabel(I18NStrings.N_A);

		Double size = (Double) obj;
		
		if ( size >= 0 ) {
			//System.out.println("obj>0: " + obj);
			if ( size > 1000 ) {
				return String.format( "%,.0f", size);
			}
			else {
				return String.format( "%,.2f", size);
			}
		}
			
		else {			
			return ELFunctions.getLabel(I18NStrings.N_A);
		}
	}
}
