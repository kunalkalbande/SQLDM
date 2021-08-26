package com.idera.sqldm.ui.converter;

import java.io.Serializable;
import java.math.RoundingMode;
import java.text.DecimalFormat;
import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.server.web.session.SessionUtil;

public class ByteSizeFormatter implements Serializable {

	static final long serialVersionUID = 1L;

	protected static final String numberDisplayFormat = "###,###.#";

	protected static final Pattern parsePattern = Pattern.compile("^(\\d+(?:\\.\\d+)?)\\s+(\\S+)$");

	public static ByteUnit getDisplayUnit(Integer bytes) {
		if( bytes == null ) bytes = 0;
		return getDisplayUnit(Double.valueOf(bytes));
	}

	public static ByteUnit getDisplayUnit(Long bytes) {
		if( bytes == null ) bytes = 0L;
		return getDisplayUnit(Double.valueOf(bytes));
	}

	public static ByteUnit getDisplayUnit(Double bytes) {

		if( bytes == null ) bytes = 0D;

		ByteUnit displayUnit = ByteUnit.BYTES;

		for( ByteUnit unit : ByteUnit.values() ) {
			if( Math.abs(bytes) >= unit.getNumberOfBytes() ) {
				displayUnit = unit;
			}
			else {
				break;
			}
		}

		return displayUnit;
	}

	public static Double getDisplayBytes(Integer bytes) {
		if( bytes == null ) bytes = 0;
		return getDisplayBytes(bytes.doubleValue());
	}

	public static Double getDisplayBytes(Long bytes) {
		if( bytes == null ) bytes = 0L;
		return getDisplayBytes(bytes.doubleValue());
	}

	public static Double getDisplayBytes(Double bytes) {
		if( bytes == null ) bytes = 0D;
		ByteUnit unit = getDisplayUnit(bytes);
		return bytes.doubleValue() / unit.getNumberOfBytes().doubleValue();
	}

	public static String getFriendlyString(Integer bytes) {
		if( bytes == null ) bytes = 0;
		return getFriendlyString(Double.valueOf(bytes), SessionUtil.getSelectedLocale());
	}
	public static String getFriendlyString(Integer bytes, Locale locale) {
		if( bytes == null ) bytes = 0;
		if( locale == null ) locale = SessionUtil.getSelectedLocale();
		return getFriendlyString(Double.valueOf(bytes), locale);
	}

	public static String getFriendlyString(Long bytes) {
		if( bytes == null ) bytes = 0L;
		return getFriendlyString(Double.valueOf(bytes), SessionUtil.getSelectedLocale());
	}

	public static String getFriendlyString(Double bytes) {
		if( bytes == null ) bytes = 0D;
		return getFriendlyString(bytes, SessionUtil.getSelectedLocale());
	}

	public static String getFriendlyString(Long bytes, Locale locale) {
		if( bytes == null ) bytes = 0L;
		if( locale == null ) locale = SessionUtil.getSelectedLocale();
		return getFriendlyString(Double.valueOf(bytes), locale);
	}

	public static String getFriendlyString(Double bytes, Locale locale) {
		if( bytes == null ) bytes = 0D;
		if( locale == null ) locale = SessionUtil.getSelectedLocale();
		ByteUnit unit = getDisplayUnit(bytes);
		return getFriendlyString(bytes, unit, locale);
	}

	public static String getFriendlyString(Double bytes, ByteUnit unit, Locale locale) {

		if( bytes == null ) bytes = 0D;
		if( unit == null ) unit = ByteUnit.BYTES;
		if( locale == null ) locale = SessionUtil.getSelectedLocale();

		Double value = bytes.doubleValue() / unit.getNumberOfBytes().doubleValue();

		NumberFormat f = NumberFormat.getInstance(locale);
		f.setRoundingMode(RoundingMode.HALF_UP);
		if (f instanceof DecimalFormat) {
			((DecimalFormat) f).applyPattern(numberDisplayFormat);
		}

		StringBuffer sb = new StringBuffer(f.format(value));
		sb.append(" ");
		sb.append(ELFunctions.getLabel(locale, unit.getLabel()));
		return sb.toString();
	}

	public static List<String> getCommonFriendlyStringLong(List<Long> bytesList, ByteUnit floor, Locale locale) {

		if( floor == null ) floor = ByteUnit.BYTES;
		if( locale == null ) locale = SessionUtil.getSelectedLocale();

		List<Double> doubleValues = new ArrayList<Double>();
		if (bytesList == null) return getCommonFriendlyStringDouble(doubleValues, floor, locale);

		for (Long val : bytesList) {
			doubleValues.add(Double.valueOf(val));
		}
		return getCommonFriendlyStringDouble(doubleValues, floor, locale);
	}

	/**
	 * Return a ordered list of friendly strings where all values have the same ByteUnit
	 * ByteUnit is determined by the smallest common denominator.
	 * floor is the smallest ByteUnit allowed
	 */
	public static List<String> getCommonFriendlyStringDouble(List<Double> bytesList, ByteUnit floor, Locale locale) {
		List<String> friendlyStrings = new ArrayList<String>();

		if (bytesList == null || bytesList.isEmpty()) return friendlyStrings;
		if( floor == null ) floor = ByteUnit.BYTES;
		if( locale == null ) locale = SessionUtil.getSelectedLocale();

		ByteUnit lowestCommonDenominator = null;
		for (Double val : bytesList) {

			if( val == null ) continue;

			ByteUnit bu = getDisplayUnit(val);

			if (lowestCommonDenominator == null) {
				lowestCommonDenominator = bu;
				continue;
			}

			if (floor != null && bu.getNumberOfBytes() < floor.getNumberOfBytes()) {
				lowestCommonDenominator = floor;
				continue;
			}

			lowestCommonDenominator = bu;
		}

		for (Double bytes : bytesList) {
			friendlyStrings.add(getFriendlyString(bytes, lowestCommonDenominator, locale));
		}

		return friendlyStrings;
	}

	public static Double getBytes(String formattedString) {

		if( formattedString == null ) return 0D;

		Matcher matcher = parsePattern.matcher(formattedString);

		if( matcher.matches() ) {

			Double value  = Double.valueOf(matcher.group(1));
			ByteUnit unit = ByteUnit.fromLabel(matcher.group(2));

			if( unit == null ) {
				throw new IllegalArgumentException("Invalid units specified");
			}

			return value * unit.getNumberOfBytes();
		}
		else {
			throw new IllegalArgumentException("Invalid friendly byte string specified");
		}

	}
}
