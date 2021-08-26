package com.idera.sqldm.utils;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Collection;
import java.util.Collections;
import java.util.Date;
import java.util.List;
import java.util.Locale;
import java.util.Map;
import java.util.Set;
import java.util.TimeZone;

import org.apache.log4j.Logger;
import org.zkoss.zk.ui.Sessions;

import com.idera.server.web.WebConstants;
import com.idera.sqldm.server.web.ELFunctions;



public class Utility extends com.idera.common.Utility
{

	private static final Logger log = Logger.getLogger(Utility.class);

	public static String setOffSet(){
		
		Double offSet = null;
		if(Sessions.getCurrent()!=null)
		{
			offSet = new Double((Integer)Sessions.getCurrent().getAttribute(WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET))/(1000*60.0*60.0);
			offSet = -offSet;
		}
		if(offSet!=null)
			return offSet.toString();
		else return "0.0";
	}

	public static long cancelOffSetInMillis(){
		
		if(Sessions.getCurrent()!=null)
		{
			return -(new Long((Integer)Sessions.getCurrent().getAttribute(WebConstants.IDERA_WEB_CONSOLE_TZ_OFFSET)));
			
		}
		return 0;
	}
	
	@SuppressWarnings("rawtypes")
	public static String getUrlParameter(Map parameterMap, String parameterName) {

		String[] parameterLowerCase = (String[]) parameterMap.get(parameterName
				.toLowerCase());
		String[] parameterUpperCase = (String[]) parameterMap.get(parameterName
				.toUpperCase());

		if ((parameterLowerCase == null) && (parameterUpperCase == null)) {

			return null;

		} else {

			if (parameterLowerCase != null) {

				return parameterLowerCase[0];

			} else {

				return parameterUpperCase[0];
			}
		}
	}
	
	public static String round(String strValue, int places) {
		if(strValue == null || strValue.length() == 0) {
			log.error("This is not expected. Value came as String instead of double, value came as empty or null. Value = ("+strValue+")");
			return "0";
		}
		try {
			double value = Double.parseDouble(strValue.trim());
			return new Double(round(value, places)).toString();			
		} catch (Exception e) {
			log.error("An error has occurred while parsing double value ("+strValue+"): ", e);
			return strValue;
		}
	}
	
	public static double round(double value, int places) {
	     if (places < 0) throw new IllegalArgumentException();

	     long factor = (long) Math.pow(10, places);
	     value = value * factor;
	     long tmp = Math.round(value);
	     return (double) tmp / factor;
	}
	public static boolean wildCardMatch(String text, String pattern)
    {
        // Create the cards by splitting using a RegEx. If more speed 
        // is desired, a simpler character based splitting can be done.
		// Uses % as the wilcard
        String [] cards = pattern.split("\\%");

        // Iterate over the cards.
        for (String card : cards)
        {
            int idx = text.indexOf(card);
            
            // Card not detected in the text.
            if(idx == -1)
            {
                return false;
            }
            
            // Move ahead, towards the right of the text.
            text = text.substring(idx + card.length());
        }
        
        return true;
    }
	
	
	public static String getUtcDateString(Date date){
		SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
		sdf.setTimeZone(TimeZone.getTimeZone("UTC"));
	    return sdf.format(date);
	}

	public static String getLocalDateString(Date localDate) {
		SimpleDateFormat sdf = new SimpleDateFormat(new SimpleDateFormat().toLocalizedPattern());
		return sdf.format(localDate);
	}
	
	public static String getLocalDateStringFromUTCDate(Date utcDate) {
		return getLocalDateStringFromUTCDate(utcDate, null,null,null);
	}

	public static String getLocalDateStringFromUTCDate(Date utcDate, String dateFormat,Locale locale, TimeZone tz) {
		SimpleDateFormat sdf;
		if (dateFormat == null) {
			dateFormat = "MM/dd/yyyy hh:mm a";
		}
		if (locale != null) {
			sdf = new SimpleDateFormat(dateFormat,locale);
		}
		else {
			sdf = new SimpleDateFormat(dateFormat);				
		}

		if (tz == null){
			tz=TimeZone.getDefault();
		}
		//Removing offset 
		//int offset = tz.getOffset(utcDate.getTime());

		Date localDateFromUTCDate = new Date(
				utcDate.getTime());
		
		return sdf.format(localDateFromUTCDate);
	}

	public static Date getDate(int year, int month, int day) {
		Calendar cal = Calendar.getInstance();
		cal.set(Calendar.YEAR, year);
		cal.set(Calendar.MONTH, month);
		cal.set(Calendar.DAY_OF_MONTH, day);
		cal.set(Calendar.HOUR_OF_DAY, 0);
		cal.set(Calendar.MINUTE, 0);
		cal.set(Calendar.SECOND, 0);
		return cal.getTime();
	}
	
	public static Date getLocalDateFromUTCDate(Date utcDate) {
		if (utcDate == null) return null;
		Date localDateFromUTCDate = new Date(
				utcDate.getTime() + 
				TimeZone.getDefault().getOffset(utcDate.getTime()));
		return localDateFromUTCDate;
	}
	
	public static String getUtcDateStringFromLocalDate(Date localDate, String dateFormat) {
		Date localDateFromUTCDate = getUtcDateFromLocalDate(localDate);
		if (dateFormat != null) {
			SimpleDateFormat sdf;
			sdf = new SimpleDateFormat(dateFormat);
			return sdf.format(localDateFromUTCDate);
		}
		
		return DateFormat.getDateInstance(DateFormat.SHORT, Locale.getDefault()).format(localDateFromUTCDate);
	}
	
	public static Date getUtcDateFromLocalDate(Date localDate) {
		Date utcDateFromLocalDate = new Date(
				localDate.getTime() - 
				TimeZone.getDefault().getOffset(localDate.getTime()));
		return utcDateFromLocalDate;
	}

	// WHEN '8.' THEN '2000'
	// WHEN '9.' THEN '2005'
	// WHEN '10' THEN '2008'
	// WHEN '11' THEN '2012'
	public static String getSQLServerRelease(String version) {
		if (version == null) {
			return null;
		}
		int start = version.indexOf(".");
		if (start != -1) {
			String majorVersion = version.substring(0, version.indexOf("."));
			try {
				switch (Integer.parseInt(majorVersion)) {
					case 8:
						return "SQL Server 2000";
					case 9:
						return "SQL Server 2005";
					case 10:
						return "SQL Server 2008";
					case 11:
						String[] vers = version.split("\\.");
						if (vers.length > 3) {
							int ver = Integer.parseInt(vers[2]);
							if (ver == 3000){
								return "SQL Server 2012 SP1";							
							}
							else if (ver == 5058){
								return "SQL Server 2012 SP2";							
							}
							else if (ver == 2100){
								return "SQL Server 2012 RTM";							
							}
						}
						return "SQL Server 2012";
					case 12:
						return "SQL Server 2014";
					case 13:
						return "SQL Server 2016";
                    case 14:
                        return "SQL Server 2017";	// SQLDM-28654 (Varun Chopra) WebUI_Single instance overview: SQL server 2017 version is not displayed
					default:
						return "N/A";
				}
			} catch (Exception ex) {
				log.error("Failed to determine major version: ", ex);
			}
		}
		
		return null;
	}
	
	public static String getAlertImageURL(int severityCode) {
		String image;
		switch(severityCode){
		case 1:
			image = "ok32x32";
			break;
		case 2:
			image = "Information32x32";
			break;
		case 4:
			image = "warning32x32";
			break;
		case 8:
			image = "critical32x32";
			break;
		default : image = "ok32x32";
		break;
		}
		return ELFunctions.getImageURLWithoutSize(image);
	}

	public static Collection<? extends String> sortAsCollection(Set<String> diskList) {
		List<String> list = new ArrayList<String>(diskList);
		Collections.sort(list);
		return list;
	}

	public static Object getLocalDate(Date localDate) {
		// TODO Auto-generated method stub

		SimpleDateFormat sdf = new SimpleDateFormat(new SimpleDateFormat().toLocalizedPattern());
		Calendar calendar = Calendar.getInstance();
        TimeZone timeZone = calendar.getTimeZone();
        sdf.setTimeZone(timeZone);
		return sdf.format(localDate);
	}
}
