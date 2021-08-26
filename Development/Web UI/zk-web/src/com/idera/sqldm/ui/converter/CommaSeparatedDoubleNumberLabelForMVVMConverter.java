package com.idera.sqldm.ui.converter;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

import com.idera.sqldm.utils.Utility;

public class CommaSeparatedDoubleNumberLabelForMVVMConverter implements Converter {

	private Logger log = Logger.getLogger(CommaSeparatedDoubleNumberLabelForMVVMConverter.class);
	
	@Override
	public Object coerceToBean(Object arg0, Component arg1, BindContext ctx) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component comp, BindContext ctx) {
		if( obj == null ) return "";
		if( obj instanceof String ) {
			try {
				obj = Utility.round(Double.parseDouble((String)obj), 2);
			} catch (Exception ex) {
				log.error(ex.getMessage(), ex);
				return "";
			}
		} else if( !(obj instanceof Number) ) return "";
		return String.format("%,f", (Number) obj);
	}

}
