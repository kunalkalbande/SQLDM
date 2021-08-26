package com.idera.sqldm.ui.dashboard;

import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

import com.idera.sqldm.data.DashboardInstance;

public class InstanceStatusLabelConverter implements TypeConverter {

	@Override
	public Object coerceToBean(Object arg0, Component arg1) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component comp) {

		if( obj == null || !(obj instanceof DashboardInstance) ) return "";
		return obj;
		
//		DashboardInstance instance = (DashboardInstance) obj;
//		
//		return Utility.getMessage(instance.getStatus().getState());
	}

}
