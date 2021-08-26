package com.idera.sqldm.ui.dashboard.instances.queryWaits;

import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Div;

import com.idera.sqldm.data.instances.QueryWaits;
import com.idera.sqldm.ui.components.charts.bar.IderaStackedBarChart;

public class WaitsComposer extends QueryViewComposer {

	private static final long serialVersionUID = 1L;
	@Wire
	private IderaStackedBarChart stackedBarChart;
	@Wire
	private IderaStackedBarChart durationChart;

	@Wire
	private Div timeGraphDiv;

	@Wire
	private Div durationGraphDiv;

	@Override
	public IderaStackedBarChart getLineChartInstance() {
		return durationChart;
	}

	@Override
	public String getGroupBY(QueryWaits waits) {
		// TODO Auto-generated method stub
		return waits.getWaitType().trim();
	}

	@Override
	public IderaStackedBarChart getStackedBarChartInstance() {
		return stackedBarChart;
	}

	@Override
	public Div getTimeGraphDiv() {
		return timeGraphDiv;
	}

	@Override
	public Div getDurationGraphDiv() {
		return durationGraphDiv;
	}

	@Override
	public QueryChartOptionsEnum getQueryChartOption() {
		return QueryChartOptionsEnum.WAITS;
	}

	@Override
	public int getGroupById(QueryWaits waits) {
		return waits.getWaitTypeID();
	}

}
