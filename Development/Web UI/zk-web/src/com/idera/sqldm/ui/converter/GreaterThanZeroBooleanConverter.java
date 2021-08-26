package com.idera.sqldm.ui.converter;

import java.math.BigDecimal;
import java.math.BigInteger;
import java.util.concurrent.atomic.AtomicInteger;
import java.util.concurrent.atomic.AtomicLong;

import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

public class GreaterThanZeroBooleanConverter implements TypeConverter {

	@Override
	public Object coerceToBean(Object arg0, Component arg1) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component comp) {

		if( obj == null ) return false;

		if (Short.TYPE.equals(obj.getClass()) || obj instanceof Short ) {
			return ((Short) obj) > 0;
		}
		else if (Byte.TYPE.equals(obj.getClass()) || obj instanceof Byte ) {
			return ((Byte) obj) > 0;
		}
		else if (Integer.TYPE.equals(obj.getClass()) || obj instanceof Integer ) {
			return ((Integer) obj) > 0;
		}
		else if (Long.TYPE.equals(obj.getClass()) || obj instanceof Long ) {
			return ((Long) obj) > 0L;
		}
		else if (Float.TYPE.equals(obj.getClass()) || obj instanceof Float ) {
			return ((Float) obj) > 0F;
		}
		else if (Double.TYPE.equals(obj.getClass()) || obj instanceof Double ) {
			return ((Double) obj) > 0D;
		}
		else if ( obj instanceof AtomicInteger ) {
			return ((AtomicInteger) obj).get() > 0;
		}
		else if ( obj instanceof AtomicLong ) {
			return ((AtomicLong) obj).get() > 0L;
		}
		else if( obj instanceof BigDecimal ) {
			return ((BigDecimal) obj).compareTo(BigDecimal.ZERO) > 0;
		}
		else if( obj instanceof BigInteger ) {
			return ((BigInteger) obj).compareTo(BigInteger.ZERO) > 0;
		}

		return false;
	}

}
