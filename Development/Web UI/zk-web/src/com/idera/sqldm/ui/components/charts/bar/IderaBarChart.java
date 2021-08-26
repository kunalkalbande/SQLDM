package com.idera.sqldm.ui.components.charts.bar;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;
import java.util.TreeMap;

import org.apache.log4j.Logger;

import com.idera.i18n.I18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.ui.components.charts.IderaChart;
import com.idera.sqldm.ui.components.charts.IderaChartModel;
import com.idera.sqldm.d3zk.chart.BarChart;

public class IderaBarChart extends IderaChart {
	private static final long serialVersionUID = 1L;
	private static final Logger log = Logger.getLogger(IderaBarChart.class);
	
	//private static final Logger log = Logger.getLogger(IderaBarChart.class); 
	private static final int DEFAULT_BAR_HEIGHT = 25;
	private boolean fitToHeight = false;
	private boolean showZeroValues = true;
	private Map<String, Double> chartData;
	
	public IderaBarChart() {
		super("~./sqldm/com/idera/sqldm/ui/components/barChart.zul");
	}
	
	@Override
	@SuppressWarnings("unchecked")
	public BarChart<Number> getChart() {
		return (BarChart<Number>) super.getChart();
	}
	
	@Override
	public IderaChartModel getModel() {
		throw new UnsupportedOperationException("Method not supported by IderaBarChart");
	}
	
	public boolean isShowZeroValues() {
		return showZeroValues;
	}
	
	public void setShowZeroValues(boolean showZeroValues) {
		this.showZeroValues = showZeroValues;
	}
	
	public Map<String, Double> getChartData() {
		return this.chartData;
	}

	public boolean isFitToHeight() {
		return fitToHeight;
	}

	public void setFitToHeight(boolean fitToHeight) {
		this.fitToHeight = fitToHeight;
	}
	
	public void setChartData(Map<String, Double> chartData) {
		if (!(chartData instanceof TreeMap<?, ?>)) {
			this.chartData = chartData; 
		} else {
			this.chartData = chartData;
		}
		updateChart();
	}
		
	public void setOrient(String orient) {
		getChart().setOrient(orient);
	}
	
	public void setAnimateBars(boolean animateBars) {
		getChart().setAnimateBars(animateBars);
	}
	
	public void setAnimationDelay(int delay) {
		getChart().setAnimationDelay(delay);
	}
	
	public void setAnimationLength(int animationLength) {
		getChart().setAnimationLength(animationLength);
	}

	@Override
	public void updateChart() {
		try {
			Map<String, Number> filteredChartData = new HashMap<String, Number>();
			for (Entry<String, Double> entry : getChartData().entrySet()) {
				if (entry.getKey() != null) {
					Double value = entry.getValue() == null ? 0 : entry.getValue();
					if (isShowZeroValues() || value > 0) {
						filteredChartData.put(entry.getKey(), value);
					}
				}
			}
			if (!filteredChartData.isEmpty()) {
				getChart().setValueFormat(getValueFormat());
				if (isFitToHeight()) {
					getChart().setHeight(DEFAULT_BAR_HEIGHT * filteredChartData.size() + "px");
				}
				
				// feed the chart data to the D3 bar chart control
				sortMapByValue(filteredChartData);
				getChart().setSeries(filteredChartData);
				
				getErrorLabel().setVisible(false);
				getChart().setVisible(true);
			} else {
				showError(noDataMessage);
			}
		} catch (Exception x) {
			log.error(x.getMessage(), x);
			showError(ELFunctions.getMessage(I18NStrings.ERROR_OCCURRED_LOADING_CHART));
		}
	}
	
	private void sortMapByValue(Map<String, Number> map) {
		List<Map.Entry<String, Number>> entries = new ArrayList<>(map.entrySet());
		Collections.sort(entries, doubleComparator);
	}
	
	private static final Comparator<Entry<String, Number>> doubleComparator = new Comparator<Map.Entry<String, Number>>() {
		@Override
		public int compare(Map.Entry<String, Number> a, Map.Entry<String, Number> b) {
			return ((Double)b.getValue()).compareTo((Double)a.getValue());
		}
	};
}
