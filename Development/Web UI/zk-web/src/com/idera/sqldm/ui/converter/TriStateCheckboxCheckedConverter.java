package com.idera.sqldm.ui.converter;

import org.zkoss.zk.ui.Component;
import org.zkoss.zkplus.databind.TypeConverter;

import com.idera.sqldm.ui.components.instancetagspopup.SelectedState;

public class TriStateCheckboxCheckedConverter implements TypeConverter {

	@Override
	public Object coerceToBean(Object obj, Component comp) {
		return null;
	}

	@Override
	public Object coerceToUi(Object obj, Component comp) {

		if (obj == null || !(obj instanceof SelectedState))
			return false;

		if (SelectedState.SELECTED.equals((SelectedState) obj)) {
			return true;
		}
		return false;
	}

}
