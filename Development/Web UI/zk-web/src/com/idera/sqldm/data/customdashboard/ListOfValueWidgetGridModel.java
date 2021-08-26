package com.idera.sqldm.data.customdashboard;

import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;

public class ListOfValueWidgetGridModel {
	private String instanceName;
	private String instanceValue;
	private IderaLineChartModel model;
	private boolean isDataAvailable;
	
	public ListOfValueWidgetGridModel(String instanceName,String instanceValue,
			IderaLineChartModel model,
			boolean isDataAvailable) {
		this.instanceName = instanceName;
		this.instanceValue = instanceValue;
		this.model = model;
		this.isDataAvailable = isDataAvailable;
	}
	
	public String getInstanceName() {
		return this.instanceName;
	}
	
	public String getInstanceValue() {
		return this.instanceValue;
	}
	
	public IderaLineChartModel getModel() {
		return this.model;
	}
	
	public boolean getIsDataAvailable() {
		return this.isDataAvailable;
	}
}
