package com.idera.sqldm.ui.customDashboard;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Div;
import org.zkoss.zul.Label;
import org.zkoss.zul.Row;
import org.zkoss.zul.Rows;

import com.idera.sqldm.data.customdashboard.CustomDashboardWidgetData;
import com.idera.sqldm.data.customdashboard.ListOfValueWidgetGridModel;
import com.idera.sqldm.data.customdashboard.MetricValues;
import com.idera.sqldm.i18n.SQLdmI18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.ui.components.charts.line.IderaLineChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.ui.customDashboard.widgets.CustomDashboardWidget;
import com.idera.sqldm.ui.customDashboard.widgets.CustomDashboardWidgetSizeEnum;
import com.idera.sqldm.ui.customDashboard.widgets.composer.CustomdDashboardBaseWidgetComposer;

public class ListOfValuesWidgetComposer extends CustomdDashboardBaseWidgetComposer {
	private static final long serialVersionUID = 1L;
	@Wire private Div editLVWidgetDiv,displayLVWidgetDiv,lvDataDiv,lvErrorDiv;
	@Wire private Rows chartGridRows;
	@Wire private Label lvWidgetNameLbl;
	@Wire private Button lvdetailsBtn,lvRemoveBtn,lvEditbtn;
	private List<ListOfValueWidgetGridModel> gridModel = new ArrayList<>();
	
	public void doAfterCompose(CustomDashboardWidget comp) throws Exception {
		super.doAfterCompose(comp);
		setWidgetConfig(comp);
		lvWidgetNameLbl.setValue((String) Executions.getCurrent().getArg().get("widgetName"));
		lvWidgetNameLbl.setTooltiptext((String) Executions.getCurrent().getArg().get("widgetName"));
		onDisplayCustomDashBoard();
		lvErrorDiv.setVisible(false);
		lvDataDiv.setVisible(false);
	}
	
	public void setWidgetData() {
		
		if(metricValuesforInstance == null || metricValuesforInstance.size() <= 0) {
			setEmptyMessage();
			return;
		}
		gridModel.clear();
		for(CustomDashboardWidgetData metricValues: metricValuesforInstance) {
			
			if(metricValues != null && metricValues.getmetricValuesforInstance() != null
				&& metricValues.getmetricValuesforInstance().size() > 0) {
				
				IderaLineChartModel model = new IderaLineChartModel();
				List<MetricValues> list = metricValues.getmetricValuesforInstance();
				
				for(int i=list.size()-1; i>=0; i--) {
					MetricValues values = list.get(i);
					long collectionTime = values.getCollectionTime().getTime();
//					model.setValue(metricValues.getInstanceName(), collectionTime, values.getMetricValue()); @author Saumyadeep 
					model.setValue(metricValues.getDisplayName(), collectionTime, values.getMetricValue());
				}
				gridModel.add(new ListOfValueWidgetGridModel(
//						metricValues.getInstanceName(), Author -  Saumyadeep
						metricValues.getDisplayName(),
						(metricValues.getmetricValuesforInstance().get(0).getMetricValue()+""),
						model,
						list.size() >= 2?true:false));
			}
		}
		lvErrorDiv.setVisible(false);
		lvDataDiv.setVisible(true);
		rowrender();
	}

	private void setChartProperties(IderaLineChart chart) {
		chart.setHeight("20px");
		chart.setWidth("100px");
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
		chart.getErrorDiv().setStyle("padding-left:0px;padding-top:0px;");
	}
	
	public void rowrender() {
		if(chartGridRows.getChildren() != null && chartGridRows.getChildren().size() > 0)
			chartGridRows.getChildren().clear();
		
		for(ListOfValueWidgetGridModel model: gridModel) {
			Row row = new Row();
//			Label identifier = new Label(model.getInstanceName()); @author Saumyadeep ???
			Label identifier = new Label(model.getInstanceName());
			identifier.setTooltiptext(model.getInstanceName());
			Label value = new Label(model.getInstanceValue());
			Div chartDiv = new Div();
			chartDiv.setSclass("list-of-value-chart-div");
			IderaLineChart chart  = new IderaLineChart();
			if(model.getIsDataAvailable())
				chart.setModel(model.getModel());
			else
				chart.setErrorMessage(ELFunctions.getMessage(SQLdmI18NStrings.NO_DATA_AVAILABLE));
			setChartProperties(chart);
			chartDiv.appendChild(chart);
			row.appendChild(identifier);
			row.appendChild(value);
			row.appendChild(chartDiv);
			chartGridRows.appendChild(row);
		}
	}
	
	@Listen("onClick = #lvdetailsBtn")
	public void displayDetails() {
		super.displayDetails();
	}
	
	@Listen("onClick = #lvRemoveBtn")
	public void removeWidget() {
		super.removeWidget();
	}
	
	@Listen("onClick = #lvEditbtn")
	public void editWidget() {
		super.editWidget();
	}

	@Override
	protected void onEditCustomDashBoard() {
		displayLVWidgetDiv.setVisible(false);
		editLVWidgetDiv.setVisible(true);
	}

	@Override
	protected void onDisplayCustomDashBoard() {
		displayLVWidgetDiv.setVisible(true);
		editLVWidgetDiv.setVisible(false);
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
		lvErrorDiv.setVisible(true);
		lvDataDiv.setVisible(false);
	}
}
