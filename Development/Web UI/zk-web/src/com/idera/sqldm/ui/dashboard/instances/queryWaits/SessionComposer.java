package com.idera.sqldm.ui.dashboard.instances.queryWaits;

import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Div;

import com.idera.sqldm.data.instances.QueryWaits;
import com.idera.sqldm.ui.components.charts.bar.IderaStackedBarChart;

public class SessionComposer extends QueryViewComposer {

	private static final long serialVersionUID = 1L;
	
	@Wire
	private IderaStackedBarChart durationChart;
	
	@Wire
	private IderaStackedBarChart stackedBarChart; 
	
	@Wire
	private Div timeGraphDiv;

	@Wire
	private Div durationGraphDiv;

	@Override
	public IderaStackedBarChart getLineChartInstance() {
/*		timeVsSessionsLineChart.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.SESSIONS_BY_WAITTIME));*/
		return durationChart;
	}

	@Override
	public String getGroupBY(QueryWaits waits) {
		// TODO Auto-generated method stub
		return waits.getSessionID().toString().trim();
	}

	@Override
	public IderaStackedBarChart getStackedBarChartInstance() {
/*		stackedBarChart.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.SESSIONS_BY_DURATION));*/
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
		return QueryChartOptionsEnum.SESSIONS;
	}

	@Override
	public int getGroupById(QueryWaits waits) {
		return waits.getSessionID();
	}
	
}
