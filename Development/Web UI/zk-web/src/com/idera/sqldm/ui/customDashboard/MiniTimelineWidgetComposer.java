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

import com.idera.sqldm.data.customdashboard.CustomDashboardWidgetData;
import com.idera.sqldm.data.customdashboard.MetricValues;
import com.idera.sqldm.ui.components.charts.line.IderaLineChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.ui.customDashboard.widgets.CustomDashboardWidget;
import com.idera.sqldm.ui.customDashboard.widgets.CustomDashboardWidgetSizeEnum;
import com.idera.sqldm.ui.customDashboard.widgets.composer.CustomdDashboardBaseWidgetComposer;


public class MiniTimelineWidgetComposer extends CustomdDashboardBaseWidgetComposer {
	private static final long serialVersionUID = 1L;
	@Wire private Div editMTWidgetDiv,displayMTWidgetDiv,mtDataDiv,mtErrorDiv;
	@Wire private IderaLineChart mtLineChart;
	@Wire private Label mtInstanceName,mtInstanceValue,mtWidgetNameLbl;
	@Wire private Button mtdetailsBtn,mtRemoveBtn,mtEditbtn;
	
	public void doAfterCompose(CustomDashboardWidget comp) throws Exception {
		super.doAfterCompose(comp);
		setWidgetConfig(comp);
		mtWidgetNameLbl.setValue((String) Executions.getCurrent().getArg().get("widgetName"));
		mtWidgetNameLbl.setTooltiptext((String) Executions.getCurrent().getArg().get("widgetName"));
		onDisplayCustomDashBoard();
		mtDataDiv.setVisible(false);
		mtErrorDiv.setVisible(false);
	}

	@Override
	protected void setWidgetData() {
		CustomDashboardWidgetData widgetData = null;
		if(metricValuesforInstance != null && metricValuesforInstance.size() > 0)
			widgetData = metricValuesforInstance.get(0);
		if(widgetData != null && widgetData.getmetricValuesforInstance() != null
				&& widgetData.getmetricValuesforInstance().size() > 0) {
//			mtInstanceName.setValue(widgetData.getInstanceName()); @author Saumyadeep
			mtInstanceName.setValue(widgetData.getDisplayName());
			mtInstanceValue.setValue(
							widgetData.getmetricValuesforInstance().get(0).getMetricValue()+"");
		} else {
			setEmptyMessage();
			return;
		}
		List<MetricValues> list = metricValuesforInstance.get(0).getmetricValuesforInstance();
		IderaLineChartModel model = new IderaLineChartModel();
		
		for(int i=list.size()-1; i>=0; i--) {
			MetricValues values = list.get(i);
			long collectionTime = values.getCollectionTime().getTime();
			model.setValue("SQL Server", collectionTime, values.getMetricValue());
		}
		mtDataDiv.setVisible(true);
		mtErrorDiv.setVisible(false);
		mtLineChart.setModel(model);
		setChartProperties(mtLineChart);
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
	
	@Listen("onClick = #mtdetailsBtn")
	public void displayDetails() {
		super.displayDetails();
	}
	
	@Listen("onClick = #mtRemoveBtn")
	public void removeWidget() {
		super.removeWidget();
	}
	
	@Listen("onClick = #mtEditbtn")
	public void editWidget() {
		super.editWidget();
	}

	@Override
	protected void onEditCustomDashBoard() {
		editMTWidgetDiv.setVisible(true);
		displayMTWidgetDiv.setVisible(false);
	}

	@Override
	protected void onDisplayCustomDashBoard() {
		editMTWidgetDiv.setVisible(false);
		displayMTWidgetDiv.setVisible(true);
	}
	
	@Override
	protected void setWidgetConfig(CustomDashboardWidget comp) {
		Map<String,String> widgetConfig = new HashMap<>();
		widgetConfig.put("title", (String) Executions.getCurrent().getArg().get("widgetName"));
		widgetConfig.put("width", CustomDashboardWidgetSizeEnum.small.getWidth());
		widgetConfig.put("height", CustomDashboardWidgetSizeEnum.small.getHeight());
		comp.setWidgetConfig(widgetConfig);
	}

	@Override
	protected void setEmptyMessage() {
		mtDataDiv.setVisible(false);
		mtErrorDiv.setVisible(true);
	}
	
}
