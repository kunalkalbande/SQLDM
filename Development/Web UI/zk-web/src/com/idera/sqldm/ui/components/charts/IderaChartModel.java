package com.idera.sqldm.ui.components.charts;

import org.zkoss.zul.SimpleCategoryModel;

public class IderaChartModel extends SimpleCategoryModel {
	
	private static final long serialVersionUID = 1L;
	
	
	/**
	 * @param series : Name of the series. Should be same for all values in series.
	 * @param category: It is x-axis
	 * @param value: y-axis 
	 */
	public void setValue(String series, String category, int value) {
		super.setValue(series, category, new Integer(value));
	}

}
