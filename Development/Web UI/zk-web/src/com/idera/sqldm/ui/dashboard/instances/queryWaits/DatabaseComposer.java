package com.idera.sqldm.ui.dashboard.instances.queryWaits;

import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Div;

import com.idera.sqldm.data.instances.QueryWaits;
import com.idera.sqldm.ui.components.charts.bar.IderaStackedBarChart;

public class DatabaseComposer extends QueryViewComposer {

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
/*		timeVsDatabasesLineChart.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.DATABASE_BY_WAITTIME));*/
		return durationChart;
	}

	@Override
	public String getGroupBY(QueryWaits waits) {
		return waits.getDatabaseName().trim();
	}

	@Override
	public IderaStackedBarChart getStackedBarChartInstance() {
/*		stackedBarChart.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.DATABASE_BY_DURATION));*/
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
		return QueryChartOptionsEnum.DATABASES;
	}

	@Override
	public int getGroupById(QueryWaits waits) {
		return waits.getDatabaseID();
	}
}
