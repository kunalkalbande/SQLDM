package com.idera.sqldm.server.web;

import java.util.Calendar;
import java.util.Map;
import java.util.TreeMap;

import com.idera.sqldm.server.web.session.SessionUtil;

public class FrequencyUtil {

	public static Map<Integer, String> getHoursOfDayMap() {
		Map<Integer,String> hoursOfDayMap = new TreeMap<Integer,String>();
		Calendar calendar = Calendar.getInstance(SessionUtil.getSelectedLocale());
		int min = calendar.getMinimum(Calendar.HOUR_OF_DAY);
		int max = calendar.getMaximum(Calendar.HOUR_OF_DAY);

		for( int x = min; x <= max; x++ ) {
			if(x == 0)
				hoursOfDayMap.put(x, "12AM");
			else if(x < 12)
				hoursOfDayMap.put(x, x + "AM");
			else if(x == 12)
				hoursOfDayMap.put(x, "12PM");
			else {
				hoursOfDayMap.put(x, x%12 + "PM");
			}
		}
		return hoursOfDayMap;
	}
}
