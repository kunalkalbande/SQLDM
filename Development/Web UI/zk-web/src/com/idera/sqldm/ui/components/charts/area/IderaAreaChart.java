package com.idera.sqldm.ui.components.charts.area;

import org.apache.log4j.Logger;

import com.idera.i18n.I18NStrings;
import com.idera.sqldm.server.web.ELFunctions;
import com.idera.sqldm.ui.components.charts.IderaChart;
import com.idera.sqldm.utils.SQLdmConstants;
import com.idera.sqldm.d3zk.chart.AreaChart;

public class IderaAreaChart extends IderaChart {

	private static final long serialVersionUID = 1L;
	private static final Logger log = Logger.getLogger(IderaAreaChart.class);

	public IderaAreaChart() {
		super("~./sqldm/com/idera/sqldm/ui/components/areaChart.zul");
	}

	@Override
	@SuppressWarnings("unchecked")
	public AreaChart<Double> getChart() {
		return (AreaChart<Double>) super.getChart();
	}

	@Override
	public void updateChart() {
		try {
			if (getModel() != null) {
				getChart().setModel(getModel());
				getChart().setDrawHorizontalGridLines(false);
				getChart().setDrawVerticalGridLines(false);
				getChart().setShowSeriesLabels(true);
				getChart().setXAxisTickCount(SQLdmConstants.CHART_X_AXIS_TICKS_COUNT);
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

}
