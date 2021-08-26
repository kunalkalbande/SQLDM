package com.idera.sqldm.ui.customDashboard;

import java.awt.Color;
import java.util.Calendar;
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
import com.idera.sqldm.utils.Utility;

public class LargeTimelineWidgetComposer extends CustomdDashboardBaseWidgetComposer {
	private static final long serialVersionUID = 1L;
	@Wire private Div editLTWidgetDiv,displayLTWidgetDiv,seriesDiv,ltDataDiv,ltErrorDiv;
	@Wire private IderaLineChart ltLineChart;
	@Wire private Label ltTimeStampLbl,ltWidgetMetricLbl;
	@Wire private Button ltdetailsBtn,ltRemoveBtn,ltEditbtn;
	private Integer OFFSET_IN_MILLISECOND = 0;
	private Map<String,String> seriesColorMap = new HashMap<>();
	private int idx = 0;
	private int[] colour;


	public void doAfterCompose(CustomDashboardWidget comp) throws Exception {
		super.doAfterCompose(comp);
		setWidgetConfig(comp);
		setOffSet();
		ltWidgetMetricLbl.setValue((String) Executions.getCurrent().getArg().get("widgetName"));
		ltWidgetMetricLbl.setTooltiptext((String) Executions.getCurrent().getArg().get("widgetName"));
		onDisplayCustomDashBoard();
		ltErrorDiv.setVisible(false);
		ltDataDiv.setVisible(false);
	}

	@Override
	protected void setWidgetData() {
	
		if(metricValuesforInstance == null || metricValuesforInstance.size() <= 0) {
			setEmptyMessage();
			return;
		}
		
		if(seriesDiv.getChildren() != null) {
			seriesDiv.getChildren().clear();
		}
		
		
		seriesColorMap.clear();
		idx = 0;
		if(metricValuesforInstance.size() <= 10)
			colour = ss;
		else if(metricValuesforInstance.size() <= 20)
			colour = hs;
		else{
			// appending same colors again in case of large number of instances
			colour = new int[metricValuesforInstance.size()];
			int idxHs = 0;
			for (int i = 0; i < colour.length; i++) {
				if(idxHs == 20){
					idxHs = 0;
				}
				colour[i] = hs[idxHs++];
			}
		}

		//Calendar c = Calendar.getInstance();
		//c.setTime(metricValuesforInstance.get(0).getmetricValuesforInstance().get(0).getCollectionTime());
		ltTimeStampLbl.setValue(metricValuesforInstance.get(0)
				.getmetricValuesforInstance().get(0).getCollectionTime()
				.toString().replace("GMT", ""));
		
		IderaLineChartModel model = new IderaLineChartModel();
		for(CustomDashboardWidgetData metricValues: metricValuesforInstance) {
			if(metricValues != null && metricValues.getmetricValuesforInstance() != null
				&& metricValues.getmetricValuesforInstance().size() > 0) {
				setSeries(metricValues);
				List<MetricValues> list = metricValues.getmetricValuesforInstance();
				for(int i=list.size()-1; i>=0; i--) {
					MetricValues values = list.get(i);
					//long collectionTime = values.getCollectionTime().getTime();
					Calendar cal = Calendar.getInstance();
					cal.setTime(values.getCollectionTime());
					cal.add(Calendar.MILLISECOND, OFFSET_IN_MILLISECOND);
//					model.setValue(metricValues.getInstanceName(), cal.getTime().getTime(), values.getMetricValue()); @author Saumyadeep
					model.setValue(metricValues.getDisplayName(), cal.getTime().getTime(), values.getMetricValue()); 
					
				}
			}
		}
		ltErrorDiv.setVisible(false);
		ltDataDiv.setVisible(true);
		ltLineChart.setModel(model);
		setChartProperties(ltLineChart);
	}
	
	private void setSeries(CustomDashboardWidgetData metricValues) {
		Div div = new Div();
		div.setSclass("largetimeline-items");
//		Label nameLbl = new Label(metricValues.getInstanceName()); @author Saumyadeep
		Label nameLbl = new Label(metricValues.getDisplayName());
		nameLbl.setSclass("largetimeline-items-series");
		//hue += 0.05f;
		//float saturation = 0.1f;//1.0 for brilliant, 0.0 for dull
		//float luminance = 1.0f; //1.0 for brighter, 0.0 for black
		//Color color = Color.getHSBColor(hue, saturation, luminance);
		Color color = new Color(colour[idx++]);
		String rgb = "rgb("+color.getRed() + "," + color.getGreen() + "," + color.getBlue() + ")";
		nameLbl.setStyle("color:"+rgb);
		String key = metricValues.getInstanceName().replaceAll("[\\\\]", "");
		seriesColorMap.put(key, rgb);
		Label valueLbl = new Label(
				metricValues.getmetricValuesforInstance().get(0).getMetricValue()+"");
		valueLbl.setSclass("largetimeline-items-value");
		div.appendChild(valueLbl);
		div.appendChild(nameLbl);
		seriesDiv.appendChild(div);
	}
	
	private void setChartProperties(IderaLineChart chart) {
		chart.getChart().setShowSeriesLabels(false);
		chart.getChart().setXAxisTickCount(5);
		chart.getChart().setYAxisTickCount(5);
		chart.getTopDiv().setVisible(false);
		chart.getInnerSpace().setVisible(false);
		chart.getChart().setHeight("100%");
		chart.getChart().setWidth("100%");
		chart.getErrorLabel().setStyle("font-size:10px;");
		//chart.getChart().setyAxisCustomMaxDomainValue(true);
		//chart.getChart().setyAxisCustomMinDomainValue(true);
		chart.getChart().setDrawHorizontalGridLines(true);
		chart.getChart().setEnableMouseOverText(true);
		chart.getChart().setCustomColorMap(seriesColorMap);
	}
	
	@Listen("onClick = #ltdetailsBtn")
	public void displayDetails() {
		super.displayDetails();
	}
	
	@Listen("onClick = #ltRemoveBtn")
	public void removeWidget() {
		super.removeWidget();
	}
	
	@Listen("onClick = #ltEditbtn")
	public void editWidget() {
		super.editWidget();
	}

	@Override
	protected void onEditCustomDashBoard() {
		editLTWidgetDiv.setVisible(true);
		displayLTWidgetDiv.setVisible(false);
	}

	@Override
	protected void onDisplayCustomDashBoard() {
		editLTWidgetDiv.setVisible(false);
		displayLTWidgetDiv.setVisible(true);
	}
	
	@Override
	protected void setWidgetConfig(CustomDashboardWidget comp) {
		Map<String,String> widgetConfig = new HashMap<>();
		widgetConfig.put("title", (String) Executions.getCurrent().getArg().get("widgetName"));
		widgetConfig.put("width", CustomDashboardWidgetSizeEnum.large.getWidth());
		widgetConfig.put("height", CustomDashboardWidgetSizeEnum.large.getHeight());
		comp.setWidgetConfig(widgetConfig);
	}

	@Override
	protected void setEmptyMessage() {
		ltErrorDiv.setVisible(true);
		ltDataDiv.setVisible(false);
	}
	
	private void setOffSet() {
		OFFSET_IN_MILLISECOND = (int)Utility.cancelOffSetInMillis();
	}
}
