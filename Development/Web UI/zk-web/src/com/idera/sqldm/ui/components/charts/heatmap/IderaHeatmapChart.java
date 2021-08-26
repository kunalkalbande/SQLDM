package com.idera.sqldm.ui.components.charts.heatmap;

import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.Div;

import com.idera.sqldm.d3zk.chart.HeatmapChart;
import com.idera.sqldm.ui.components.charts.IderaChart;
import com.idera.sqldm.ui.components.charts.IderaChartModel;

public class IderaHeatmapChart extends IderaChart {
	private static final long serialVersionUID = 1L;
	
	@Wire private Div titleDiv,errorDiv;

	
	public IderaHeatmapChart() {
		super("~./sqldm/com/idera/sqldm/ui/components/heatmapChart.zul");
	}
	
	@Override
	@SuppressWarnings("unchecked")
	public HeatmapChart<Number> getChart() {
		return (HeatmapChart<Number>) super.getChart();
	}
	
	@Override
	public IderaChartModel getModel() {
		throw new UnsupportedOperationException("Method not supported by IderaBarChart");
	}
	
	public Div getErrorDiv(){
		return this.errorDiv;
	}
	
	public Div getTitleDiv(){
		return this.titleDiv;
	}
	@Override
	public void updateChart() {
		getChart().setVisible(true);
		//showError(noDataMessage);
		
	}
	
}
