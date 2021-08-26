package com.idera.sqldm.ui.customDashboard;

import java.awt.Color;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.TimeZone;

import org.apache.log4j.Logger;
import org.zkoss.zk.ui.Component;
import org.zkoss.zk.ui.Executions;
import org.zkoss.zk.ui.Sessions;
import org.zkoss.zk.ui.select.SelectorComposer;
import org.zkoss.zk.ui.select.annotation.Listen;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Button;
import org.zkoss.zul.Div;
import org.zkoss.zul.Label;

import com.idera.server.web.WebConstants;
import com.idera.sqldm.data.customdashboard.CustomDashboardFacade;
import com.idera.sqldm.data.customdashboard.CustomDashboardWidgetData;
import com.idera.sqldm.data.customdashboard.MetricValues;
import com.idera.sqldm.ui.components.charts.line.IderaLineChart;
import com.idera.sqldm.ui.components.charts.line.IderaLineChartModel;
import com.idera.sqldm.utils.Utility;


public class WidgetDetailsComposer extends SelectorComposer<Component> {

	private static final long serialVersionUID = 1L;
	private final Logger log = Logger.getLogger(WidgetDetailsComposer.class);
	@Wire private Div wdErrorDiv,wdDataDiv,detailsseriesDiv;
	@Wire private Label wdTimeStampLbl,wdWidgetNameLbl;
	@Wire private IderaLineChart detailsLineChart;
	@Wire private Button wdCloseBtn;
	private String productInstanceName,widgetName,OFFSET_IN_HOURS = "0.0";
	private int customDashboardId,widgetId;
	private Date startTime,endTime;
	private Integer OFFSET_IN_MILLISECOND = 0;
	private Map<String,String> seriesColorMap = new HashMap<>();
	private int[] ss = new int[]{2062260, 16744206, 2924588, 14034728, 9725885, 9197131, 14907330, 8355711, 12369186, 1556175},
			hs = new int[]{3244733, 7057110, 10406625, 13032431, 15095053, 16616764, 16625259, 16634018, 3253076, 7652470, 10607003, 13101504, 7695281, 10394312, 12369372, 14342891, 6513507, 9868950, 12434877, 14277081};
	private int idx = 0;
	private int[] colour;
	
	public void doAfterCompose(Component comp) throws Exception {
		super.doAfterCompose(comp);
		wdDataDiv.setVisible(false);
		wdErrorDiv.setVisible(false);
		setOffSet();
		productInstanceName = (String) Executions.getCurrent().getArg().get("productInstanceName");
		customDashboardId = (Integer) Executions.getCurrent().getArg().get("customDashboardId");
		widgetId = (Integer) Executions.getCurrent().getArg().get("widgetId");
		widgetName = (String)Executions.getCurrent().getArg().get("widgetName");
		SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
		sdf.setTimeZone(TimeZone.getTimeZone("UTC"));
		startTime = sdf.parse((String) Executions.getCurrent().getArg().get("startTime"));
		endTime = sdf.parse((String) Executions.getCurrent().getArg().get("endTime"));
		loadWidgetData();
	}
	
	@Listen("onClick = #wdCloseBtn")
	public void close() {
		getSelf().detach();
	}
	
	private void loadWidgetData() {
		List<CustomDashboardWidgetData> metricValuesforInstance = new LinkedList<>();
		try {
			metricValuesforInstance = CustomDashboardFacade.getWidgetData(productInstanceName, customDashboardId
					, widgetId, startTime, endTime,OFFSET_IN_HOURS);
		} catch (Exception e) {
			log.error(e.getMessage(), e);
		}
		setWidgetData(metricValuesforInstance);
	}
	
	private void setWidgetData(List<CustomDashboardWidgetData> metricValuesforInstance) {
		if(metricValuesforInstance == null || metricValuesforInstance.size() <= 0) {
			setEmptyMessage();
			return;
		}
		
		if(detailsseriesDiv.getChildren() != null) {
			detailsseriesDiv.getChildren().clear();
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
		
		wdTimeStampLbl.setValue(metricValuesforInstance.get(0)
				.getmetricValuesforInstance().get(0).getCollectionTime()
				.toString().replace("GMT", ""));
		wdWidgetNameLbl.setValue(widgetName);
		
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
		wdErrorDiv.setVisible(false);
		wdDataDiv.setVisible(true);
		detailsLineChart.setModel(model);
		setChartProperties(detailsLineChart);
	}
	
	private void setSeries(CustomDashboardWidgetData metricValues) {
		Div div = new Div();
		div.setSclass("largetimeline-items");
//		Label nameLbl = new Label(metricValues.getInstanceName()); @author Saumyadeep
		Label nameLbl = new Label(metricValues.getDisplayName());
		nameLbl.setSclass("largetimeline-items-series");
		//hue += 0.1f;
		//float saturation = 0.9f;//1.0 for brilliant, 0.0 for dull
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
		valueLbl.setStyle("width: 150px;");
		div.appendChild(valueLbl);
		div.appendChild(nameLbl);
		detailsseriesDiv.appendChild(div);
	}
	
	private void setChartProperties(IderaLineChart chart) {
		chart.getChart().setShowSeriesLabels(false);
		chart.getTopDiv().setVisible(false);
		chart.getInnerSpace().setVisible(false);
		chart.getChart().setHeight("100%");
		chart.getChart().setWidth("100%");
		//chart.getChart().setyAxisCustomMaxDomainValue(true);
		//chart.getChart().setyAxisCustomMinDomainValue(true);
		chart.getChart().setDrawHorizontalGridLines(true);
		chart.getChart().setEnableMouseOverText(true);
		chart.getChart().setCustomColorMap(seriesColorMap);
	}
	
	private void setEmptyMessage() {
		wdErrorDiv.setVisible(true);
		wdDataDiv.setVisible(false);
	}
	
	private void setOffSet() {
	OFFSET_IN_MILLISECOND = (int)Utility.cancelOffSetInMillis();
	OFFSET_IN_HOURS = Utility.setOffSet();
	}

}
