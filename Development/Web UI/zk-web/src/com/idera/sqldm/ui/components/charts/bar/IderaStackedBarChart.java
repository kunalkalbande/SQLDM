package com.idera.sqldm.ui.components.charts.bar;

import com.idera.i18n.I18NStrings;
import com.idera.server.web.ELFunctions;
import com.idera.sqldm.ui.components.charts.IderaChart;
import com.idera.sqldm.ui.components.charts.IderaChartModel;
import com.idera.sqldm.d3zk.chart.BarChart;
import com.idera.sqldm.d3zk.chart.StackedBarChart;
import org.apache.log4j.Logger;

import java.util.*;
import java.util.Map.Entry;

public class IderaStackedBarChart extends IderaChart {
	private static final long serialVersionUID = 1L;
	private static final Logger log = Logger.getLogger(IderaStackedBarChart.class);

	private static final int DEFAULT_BAR_HEIGHT = 25;
	private Map<String, Map<String,Number>> chartData;

	public IderaStackedBarChart() {
		super("~./sqldm/com/idera/sqldm/ui/components/stackedBarChart.zul");
	}

	@Override
	@SuppressWarnings("unchecked")
	public StackedBarChart<Number> getChart() {
		return (StackedBarChart<Number>) super.getChart();
	}

	@Override
	public IderaChartModel getModel() {
		throw new UnsupportedOperationException("Method not supported by IderaStackedBarChart");
	}

	public Map<String, Map<String,Number>> getChartData() {
		return this.chartData;
	}

    public void setOrient(String orient) {
        getChart().setOrient(orient);
    }

	public void setChartData(Map<String, Map<String,Number>> chartData) {
		if (!(chartData instanceof TreeMap<?, ?>)) {
			this.chartData = chartData;
		} else {
			this.chartData = chartData;
		}
		updateChart();
	}

	@Override
	public void updateChart() {
		try {
			Map<String, Map<String,Number>> filteredChartData = getChartData();
			/*for (Entry<String, Double> entry : getChartData().entrySet()) {
				if (entry.getKey() != null) {
					Double value = entry.getValue() == null ? 0 : entry.getValue();
					if (isShowZeroValues() || value > 0) {
						filteredChartData.put(entry.getKey(), value);
					}
				}
			}*/

			if (!filteredChartData.isEmpty()) {
				getChart().setValueFormat(getValueFormat());
				// feed the chart data to the D3 bar chart control
				getChart().setSeries(filteredChartData);
                //Default y-axis tick padding
                getChart().setYAxisTickPadding(20);
                getChart().setXAxisTickPadding(5);
               /* getChart().setXAxisTitle(xAxisTitle);
                getChart().setXAxisTickPadding(10);
*/
				getErrorLabel().setVisible(false);
				getChart().setVisible(true);
				super.invalidate();
			} else {
				showError(noDataMessage);
			}
		} catch (Exception x) {
			log.error(x.getMessage(), x);
			showError(ELFunctions.getMessage(I18NStrings.ERROR_OCCURRED_LOADING_CHART));
		}
	}

	private static final Comparator<Entry<String, Number>> doubleComparator = new Comparator<Entry<String, Number>>() {
		@Override
		public int compare(Entry<String, Number> a, Entry<String, Number> b) {
			return ((Double)b.getValue()).compareTo((Double)a.getValue());
		}
	};
}
