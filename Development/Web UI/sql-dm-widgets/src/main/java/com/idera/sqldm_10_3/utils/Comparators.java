package com.idera.sqldm_10_3.utils;

import org.apache.commons.lang.math.NumberUtils;

public class Comparators extends com.idera.common.Comparators {
	
	public static int compareLogicStringNumerals(String c1, String c2) {
		if(c1 == null){
			c1 = "0";
		}
		if(c2 == null){
			c2 = "0";
		}
		if(NumberUtils.isNumber(c1) && NumberUtils.isNumber(c2)){
			return NumberUtils.compare(Double.parseDouble(c2), Double.parseDouble(c1));
		}
		else {
			return c2.compareTo(c1);
		}
	}

}
