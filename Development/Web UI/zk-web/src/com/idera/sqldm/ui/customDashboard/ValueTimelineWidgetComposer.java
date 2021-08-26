package com.idera.sqldm.ui.customDashboard;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Div;
import org.zkoss.zul.Label;

import com.idera.sqldm.data.category.CategoryException;
import com.idera.sqldm.data.customdashboard.CustomDashboardWidgetData;
import com.idera.sqldm.data.customdashboard.MetricValues;
import com.idera.sqldm.ui.components.charts.line.IderaLineChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.ui.customDashboard.widgets.CustomDashboardWidget;
import com.idera.sqldm.ui.customDashboard.widgets.CustomDashboardWidgetSizeEnum;
import com.idera.sqldm.ui.customDashboard.widgets.composer.CustomdDashboardBaseWidgetComposer;

public class ValueTimelineWidgetComposer extends CustomdDashboardBaseWidgetComposer {

	private static final long serialVersionUID = 1L;
	@Wire private Div editVTWidgetDiv,displayVTWidgetDiv,vtDataDiv,vtErrorDiv;
	@Wire private IderaLineChart vtLineChart;
	@Wire private Label vtInstanceName,vtCurrentValue,vtAverageValue,vtPeakValue,vtWidgetNameLbl;
	@Wire private Button vtdetailsBtn,vtRemoveBtn,vtEditbtn;
	
	public void doAfterCompose(CustomDashboardWidget comp) throws CategoryException,Exception {
		super.doAfterCompose(comp);
		setWidgetConfig(comp);
		vtWidgetNameLbl.setValue((String) arg.get("widgetName"));
		vtWidgetNameLbl.setTooltiptext((String) arg.get("widgetName"));
		onDisplayCustomDashBoard();
		vtDataDiv.setVisible(false);
		vtErrorDiv.setVisible(false);
	}

	@Override
	protected void setWidgetData() {
		CustomDashboardWidgetData widgetData = null;
		if(metricValuesforInstance != null && metricValuesforInstance.size() > 0)
			widgetData = metricValuesforInstance.get(0);
		if(widgetData != null && widgetData.getmetricValuesforInstance() != null
				&& widgetData.getmetricValuesforInstance().size() > 0) {
//			vtInstanceName.setValue(widgetData.getInstanceName()); @author Saumyadeep
			vtInstanceName.setValue(widgetData.getDisplayName());
		} else {
			setEmptyMessage();
			return;
		}
		List<MetricValues> list = metricValuesforInstance.get(0).getmetricValuesforInstance();
		IderaLineChartModel model = new IderaLineChartModel();
		double current = widgetData.getmetricValuesforInstance().get(0).getMetricValue();
		double average = 0, peak = Integer.MIN_VALUE;
		for(int i=list.size()-1; i>=0; i--) {
			MetricValues values = list.get(i);
			long collectionTime = values.getCollectionTime().getTime();
			model.setValue("SQL Server", collectionTime, values.getMetricValue());
			
			if(values.getMetricValue() > peak)
				peak = values.getMetricValue();
			average += values.getMetricValue();
		}
		average = Math.round((average/list.size())*100.0)/100.0;
		vtCurrentValue.setValue(current+"");
		vtAverageValue.setValue(average+"");
		vtPeakValue.setValue(peak + "");
		vtDataDiv.setVisible(true);
		vtErrorDiv.setVisible(false);
		vtLineChart.setModel(model);
		setChartProperties(vtLineChart);
	}
	
	private void setChartProperties(IderaLineChart chart) {
		chart.getChart().setShowSeriesLabels(false);
		chart.getChart().setXAxisTickCount(0);
		chart.getChart().setYAxisTickCount(0);
		chart.getTopDiv().setVisible(false);
		chart.getInnerSpace().setVisible(false);
		chart.getContainerDiv().setStyle(
				"padding-left:0px;padding-right:0px");
		chart.getChart().setHeight("100%");
		chart.getChart().setWidth("100%");
		chart.getChart().setDrawXAxis(false);
		chart.getChart().setDrawYAxis(false);
		chart.getErrorLabel().setStyle("font-size:10px;");
	}
	
	@Listen("onClick = #vtdetailsBtn")
	public void displayDetails() {
		super.displayDetails();
	}
	
	@Listen("onClick = #vtRemoveBtn")
	public void removeWidget() {
		super.removeWidget();
	}
	
	@Listen("onClick = #vtEditbtn")
	public void editWidget() {
		super.editWidget();
	}

	@Override
	protected void onEditCustomDashBoard() {
		editVTWidgetDiv.setVisible(true);
		displayVTWidgetDiv.setVisible(false);
	}

	@Override
	protected void onDisplayCustomDashBoard() {
		editVTWidgetDiv.setVisible(false);
		displayVTWidgetDiv.setVisible(true);
	}

	@Override
	protected void setWidgetConfig(CustomDashboardWidget comp) {
		Map<String,String> widgetConfig = new HashMap<>();
		widgetConfig.put("title", (String) Executions.getCurrent().getArg().get("widgetName"));
		widgetConfig.put("width", CustomDashboardWidgetSizeEnum.medium.getWidth());
		widgetConfig.put("height", CustomDashboardWidgetSizeEnum.medium.getHeight());
		comp.setWidgetConfig(widgetConfig);
	}

	@Override
	protected void setEmptyMessage() {
		vtDataDiv.setVisible(false);
		vtErrorDiv.setVisible(true);
	}

}
