package com.idera.sqldm.ui.dashboard.instances.queryWaits;

import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Div;
import org.zkoss.zul.Label;

import com.idera.sqldm.data.instances.QueryWaits;
import com.idera.sqldm.ui.components.charts.bar.IderaStackedBarChart;

public class ApplicationComposer extends QueryViewComposer {
	
	private static final long serialVersionUID = 1L;
	
	@Wire
	private IderaStackedBarChart stackedBarChart; 
	
	@Wire
	private IderaStackedBarChart durationChart;
	
	@Wire
	private Label lineChartLabel;
	
	@Wire
	private Label stackedBarChartLabel;
	
	@Wire
	private Div timeGraphDiv;

	@Wire
	private Div durationGraphDiv;

	private String labelPrefix ="Application ";

	@Override
	public IderaStackedBarChart getLineChartInstance() {
//		timeVsApplicationLineChart.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.APPLICATION_BY_WAITTIME));
		return durationChart;
	}

	@Override
	public Label getLineChartLabel() {
		return lineChartLabel;
	}
	
	@Override
	public String getGroupBY(QueryWaits waits) {
		return waits.getApplicationName().trim();
	}

/*	@Override
	public String getLabelPrefix() {
		return labelPrefix;
	}

	@Override
	public Label getStackedBarChartLabel() {
		return stackedBarChartLabel;
	}
*/	
	@Override
	public IderaStackedBarChart getStackedBarChartInstance() {
//		stackedBarChart.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.APPLICATION_BY_DURATION));
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
		return QueryChartOptionsEnum.APPLICATION;
	}

	@Override
	public int getGroupById(QueryWaits waits) {
		return waits.getApplicationID();
	}

}
