package com.idera.sqldm.ui.converter;

import java.util.HashMap;
import java.util.Map;

import com.idera.sqldm.server.web.ELFunctions;
import com.idera.i18n.I18NStrings;

public enum ByteUnit {

	BYTES(0, I18NStrings.BYTES),
	KB(10, I18NStrings.KB),
	MB(20, I18NStrings.MB),
	GB(30, I18NStrings.GB),
	TB(40, I18NStrings.TB),
	PB(50, I18NStrings.PB);
	
	private static Map<String, ByteUnit> labelMap = new HashMap<String, ByteUnit>();
	
	static { 
		for( ByteUnit unit : ByteUnit.values() ) {
			labelMap.put(unit.getLabel(), unit);
		}
	}
	
	private String label;
	private double numberOfBytes; 
	
	private ByteUnit(int exp, String label) {
		this.label          = label;
		this.numberOfBytes  = Math.pow(2, exp);
	}
	
	public String getLabel() {
		return label;
	}
	
	public Double getNumberOfBytes() {
		return numberOfBytes;
	}
	
	public static ByteUnit fromLabel(String label) {
		
		ByteUnit selectedUnit = null;
		
		for( ByteUnit unit : ByteUnit.values() ) { 
			
			String formattedLabel = ELFunctions.getLabel(unit.getLabel());
			
			if( formattedLabel.equals(label) ) {
				selectedUnit = unit;
				break;
			}			
		}
		
		return selectedUnit;
	}
	
}
