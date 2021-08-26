package com.idera.sqldm.ui.converter;

import java.util.IllegalFormatConversionException;

import org.apache.log4j.Logger;
import org.zkoss.bind.BindContext;
import org.zkoss.bind.Converter;
import org.zkoss.zk.ui.Component;

import com.idera.sqldm.utils.Utility;

public class CommaSeparatedNumberLabelForMVVMConverter implements Converter {

	private Logger log = Logger.getLogger(CommaSeparatedNumberLabelForMVVMConverter.class);
	
	@Override
	public Object coerceToBean(Object arg0, Component arg1, BindContext ctx) {
		// TODO Auto-generated method stub
		return null;
	}

//	 TODO - 
//		revisit this logic and separate TopX code to point to CommaSeparatedDoubleNumberLabelForMVVMConverter when there is a double value
//		For that APIs also needs a change as at present some of them are returning String value.
	@Override
	public Object coerceToUi(Object obj, Component comp, BindContext ctx) {
		return formatNumber(obj);
	}
	
	public static String formatNumber(Object obj) {
		try {
			if( obj == null ) return "N/A";
			if( obj instanceof String ) {
				try {
					obj = Long.parseLong((String)obj);
					return String.format("%,d", (Number) obj);
				} catch (Exception ex) {
					//log.error(ex);
					try {
						obj = Utility.round(Double.parseDouble((String)obj), 2);
						return String.format("%,.2f", (Number) obj);
					} catch (Exception ex1) {
						//log.error(ex1);
					}
					return "";
				}
			} else if( !(obj instanceof Number) ) return "";
			return String.format("%,d", (Number) obj);
		} catch (IllegalFormatConversionException efce ) {
			// log.debug(obj); log.debug(comp);
			//log.error("Double value: " + obj, efce);
			return String.format("%,.2f", (Number) obj);
		}
	}

}
