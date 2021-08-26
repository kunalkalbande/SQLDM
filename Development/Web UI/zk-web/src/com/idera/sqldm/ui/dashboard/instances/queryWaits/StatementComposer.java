package com.idera.sqldm.ui.dashboard.instances.queryWaits;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Div;
import org.zkoss.zul.Label;

import com.idera.sqldm.data.instances.QueryWaits;
import com.idera.sqldm.ui.components.charts.bar.IderaStackedBarChart;


public class StatementComposer extends QueryViewComposer{

	private static final long serialVersionUID = 1L;
	
	@Wire
	private IderaStackedBarChart statementVsDurationStackedBarChart; 
	
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

	private String labelPrefix = "Statement ";
	
	@Override
	public IderaStackedBarChart getLineChartInstance() {
/*		timeVsStatementsLineChart.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.STATEMENTS_BY_WAITTIME));*/
		return durationChart;
	}

	@Override
	public Label getLineChartLabel() {
		return lineChartLabel;
	}
	
	@Override
	public String getGroupBY(QueryWaits queryWaits) {
		return queryWaits.getSQLStatement().trim();
	}
	
	@Override
	public String getLabelPrefix() {
		return labelPrefix;
	}
	@Override
	public Label getStackedBarChartLabel() {
		return stackedBarChartLabel;
	}
	
	@Override
	public IderaStackedBarChart getStackedBarChartInstance() {
/*		statementVsDurationStackedBarChart.setTitle(ELFunctions.getMessage(SQLdmI18NStrings.STATEMENTS_BY_DURATION));*/
		return statementVsDurationStackedBarChart;
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
	public int getGroupById(QueryWaits waits) {
		return waits.getSQLStatementID();
	}

	@Override
	public QueryChartOptionsEnum getQueryChartOption() {
		return QueryChartOptionsEnum.STATEMENTS;
	}
}
