package com.idera.sqldm.server.web;

import java.text.DateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Locale;

import com.idera.ServerVersion;
import com.idera.i18n.I18NStrings;
import com.idera.i18n.I18NUtil;
import com.idera.sqldm.server.web.WebConstants;
import com.idera.sqldm.data.SeverityCodeToStringEnum;
import com.idera.sqldm.server.web.session.SessionUtil;
import com.idera.sqldm.utils.Utility;

public class ELFunctions {

	public enum IconSize {

		SMALL("small", "16x16"),
		MEDIUM("medium", "24x24"),
		LARGE("large", "32x32"),
		XL("xl", "48x48");

		private String stringValue;
		private String pixelValue;

		IconSize(String stringValue, String pixelValue) {
			this.stringValue = stringValue;
			this.pixelValue = pixelValue;
		}

		public String getStringValue() {
			return stringValue;
		}

		public String getPixelValue() {
			return pixelValue;
		}

		static IconSize fromString(String string) {

			for( IconSize size : IconSize.values() ) {
				if( size.stringValue.equals(string) ) return size;
			}

			return null;
		}
	}

	public static String getImageURL(String iconType, String iconSize) {

		IconSize size = IconSize.fromString(iconSize);

		if( size == null ) throw new IllegalArgumentException("Invalid icon size, must be: small, medium, or large");

		StringBuilder builder = new StringBuilder(WebConstants.BASE_IMAGE_URL);
		builder.append("/").append(iconType).append("-");
		builder.append(size.getPixelValue()).append(WebConstants.DEFAULT_IMAGE_EXT);

		return builder.toString();
	}

	public static String getImageURLWithoutSize(String iconType) {
		StringBuilder builder = new StringBuilder(WebConstants.BASE_IMAGE_URL);
		builder.append("/").append(iconType).append(WebConstants.DEFAULT_IMAGE_EXT);
		return builder.toString();
	}

	public static String getLabel(String labelKey) {
		return getLabel(SessionUtil.getSelectedLocale(), labelKey);
	}

	public static String getUpperCaseLabel(String labelKey) {
		return getUpperCaseLabel(SessionUtil.getSelectedLocale(), labelKey);
	}

    public static String getLowerCaseLabel(String labelKey) {
        return getLowerCaseLabel(SessionUtil.getSelectedLocale(), labelKey);
    }

	public static String getLabelWithParams(String labelKey, Object... varargs) {
		return getLabelWithParams(SessionUtil.getSelectedLocale(), labelKey, varargs);
	}

	public static String getMessage(String msgKey) {
		return getMessage(SessionUtil.getSelectedLocale(), msgKey);
	}

	public static String getUpperCaseMessage(String msgKey) {
		return getMessage(SessionUtil.getSelectedLocale(), msgKey).toUpperCase(SessionUtil.getSelectedLocale());
	}

	public static String getUpperCaseMessageWithParams(String msgKey, Object... varargs) {
		return getMessageWithParams(SessionUtil.getSelectedLocale(), msgKey, varargs).toUpperCase(SessionUtil.getSelectedLocale());
	}

	public static String getLabel(Locale locale, String labelKey) {
		return I18NUtil.getLocalizedMessage(locale, labelKey);
	}

	public static String getUpperCaseLabel(Locale locale, String labelKey) {
		return I18NUtil.getLocalizedMessage(locale, labelKey).toUpperCase(locale);
	}

    public static String getLowerCaseLabel(Locale locale, String labelKey) {
        return I18NUtil.getLocalizedMessage(locale, labelKey).toLowerCase(locale);
    }

	public static String getLabelWithParams(Locale locale, String labelKey, Object... varargs) {
		return I18NUtil.getLocalizedMessage(locale, labelKey, varargs);
	}

	public static String getMessage(Locale locale, String msgKey) {
		return I18NUtil.getLocalizedMessage(locale, msgKey);
	}

	public static String getMessageWithParams(String msgKey, Object... varargs) {
		return I18NUtil.getLocalizedMessage(SessionUtil.getSelectedLocale(), msgKey, varargs);
	}

	public static String getMessageWithParams(Locale locale, String msgKey, Object... varargs) {
		return I18NUtil.getLocalizedMessage(locale, msgKey, varargs);
	}

	public static Object getStaticField(String name) {
		try {
			int i = name.lastIndexOf(".");
			String field = name.substring(i + 1, name.length());
			name = name.substring(0, i);
			Class<?> clz = Class.forName(name);
			Object obj = clz.getField(field).get(null);
			return obj;
		}
		catch( RuntimeException x ) {
			throw x;
		}
		catch( Exception x ) {
			throw new RuntimeException(x);
		}
	}
	public static String getSeverityString(int MaxSeverity) {

		return SeverityCodeToStringEnum.getSeverityEnumForId(MaxSeverity).getLabel();
	}
	
	public static String vendorCopyrightString() {
		return "&copy; " + ServerVersion.SERVER_VENDOR_COPYRIGHT;
	}

	public static String productHeaderString() {
		return ELFunctions.getLabel(I18NStrings.PRODUCT_NAME);
	}

	public static String browserVersionContentString()	{
		return "IE=9";
	}
	
	public static String getShortDateTime(Date date) {
		try {
			if (date == null) {
				return "";
			}
			return DateFormat.getDateInstance(DateFormat.SHORT,  Locale.getDefault()).format(date);

		} catch(Exception e) {
			throw e;
		}
	}

	public static String getLocalShortDateTime(Date date) {
		try {
			if (date == null) {
				return "";
			}
			return Utility.getLocalDateString(date);
		} catch(Exception e) {
			throw e;
		}
	}

	public static String getDefaultStringIfNull(String str) {
		if (str == null || str.length() == 0) {
			return "N/A";
		}
		return str;
	}
	
	public static String getInstanceSpecificURL(String instanceID){
		if(instanceID == null)
			return "idera://";
		return "Idera://instance?instanceid="+instanceID;
	} 
	
	public static String getAlertSpecificURL(String alertID , String instanceID , String instanceName){
		if(alertID == null  || instanceID == null || instanceName == null)
			return "idera://";
		return "Idera://alert?instancename="+ instanceName + "&alertid=" + alertID +"&instanceid="  +  instanceID;
	} 
	
	public static String buildPathRelativeToCurrentProduct(String str)
	{
		return WebUtil.buildPathRelativeToCurrentProduct(str);
	}

	public static String getColorGradientAtIndex(Double index) {
		ColorGradient cg =  new ColorGradient();
		
		//setting color spectrum
		List<ColorScale> colorRange = new ArrayList<ColorScale>();
		ColorScale cs =  new ColorScale();
		cs.setRange(0);		cs.setColor("bc2708");
		colorRange.add(cs);
		
		cs = new ColorScale();
		cs.setRange(70);	cs.setColor("bc2708");
		colorRange.add(cs);
		
		cs = new ColorScale();
		cs.setRange(85);	cs.setColor("fcc416");
		colorRange.add(cs);
		
		cs = new ColorScale();
		cs.setRange(101);	cs.setColor("499c06");
		colorRange.add(cs);
		
		cg.setSpectrum(colorRange);
		
		String bgColor = cg.getColorAt(index);
		return "#" + bgColor;
	}
}