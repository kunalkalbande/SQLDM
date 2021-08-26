package com.idera.sqldm_10_3.ui.components.charts.line;

import org.apache.log4j.Logger;

import com.idera.i18n.I18NStrings;
import com.idera.sqldm_10_3.server.web.ELFunctions;
import com.idera.sqldm_10_3.ui.components.charts.IderaChart;
import com.idera.sqldm_10_3.utils.SQLdmConstants;
import com.idera.sqldm.d3zk.chart.LineChart;

public class IderaLineChart extends IderaChart {

	private static final long serialVersionUID = 1L;
	private static final Logger log = Logger.getLogger(IderaLineChart.class);

	public IderaLineChart() {
		super("~./sqldm/com/idera/sqldm/widgets/lineChart.zul");
	}

	@Override
	@SuppressWarnings("unchecked")
	public LineChart<Number> getChart() {
		return (LineChart<Number>) super.getChart();
	}

	@Override
	public void updateChart() {
		try {
			if (getModel() != null) {
				getChart().setModel(getModel());
				getChart().setDrawHorizontalGridLines(false);
				getChart().setDrawVerticalGridLines(false);
				getChart().setShowSeriesLabels(true);
				getChart().setTruncateSeriesLabels(false);
				getErrorLabel().setVisible(false);
				getChart().setVisible(true);
				
				this.setXAxisScaleType("time");
				getChart().setXAxisTickFormat("%m-%d %I:%M %p");
				getChart().setShowSeriesLabels(true);
				getChart().setXAxisTickCount(SQLdmConstants.CHART_X_AXIS_TICKS_COUNT);
				getChart().setXAxisLegendSpacing(new Integer(25));
			} else {
				showError(noDataMessage);
			}
		} catch (Exception x) {
			log.error(x.getMessage(), x);
			showError(ELFunctions.getMessage(I18NStrings.ERROR_OCCURRED_LOADING_CHART));
		}
	}
	
	public void setXAxisScaleType(String xAxisScaleType) {
		getChart().setXAxisScaleType(xAxisScaleType);
	}

}
