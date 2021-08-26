package com.idera.sqldm.ui.components.charts;

import org.zkoss.zk.ui.HtmlMacroComponent;
import org.zkoss.zk.ui.select.annotation.Wire;
import org.zkoss.zul.CategoryModel;
import org.zkoss.zul.Div;
import org.zkoss.zul.Label;
import org.zkoss.zul.Space;

import com.idera.i18n.I18NStrings;
import com.idera.server.web.ELFunctions;
import com.idera.sqldm.d3zk.chart.RectangularChart;

public abstract class IderaChart extends HtmlMacroComponent {

	private static final long serialVersionUID = 1L;
	
	private CategoryModel model = new IderaChartModel();
	private String valueFormat = "";
	
	protected String noDataMessage = ELFunctions.getLabel(I18NStrings.CHART_HAS_NO_DATA);
	
	public static final String _defaultLeftMargin = "0px";
	public static final String _defaultRightMargin = "0px";
	public static final String _defaultTopMargin = "0px";
	public static final String _defaultBottomMargin = "0px";
	
	
	@Wire private Label mc_titleLabel;
	@Wire private Label mc_errorLabel;
	@Wire private Div mc_containerDiv;
	@Wire private RectangularChart mc_chart;
	//added to control visibility of all tags @author: prakash
	@Wire private Div topDiv;
	@Wire private Space innerSpace;
	@Wire private Div errorDiv;
	
	public IderaChart(String chartZulPath) {
		setMacroURI(chartZulPath);
		compose();
	}
	
	public abstract void updateChart();
	
	public Label getTitleLabel() {
		return mc_titleLabel;
	}

	public Label getErrorLabel() {
		return mc_errorLabel;
	}

	public Div getContainerDiv() {
		return mc_containerDiv;
	}
	
	public RectangularChart getChart() {
		return mc_chart;
	}
	
	//added to control visibility of all tags @author: prakash
	public Div getTopDiv(){
		return topDiv;
	}
	
	public Space getInnerSpace(){
		return innerSpace;
	}
	
	public Div getErrorDiv(){
		return errorDiv;
	}

	@Override
	public String getWidth() {
		return getContainerDiv().getWidth();
	}

	@Override
	public void setWidth(String width) {
		getContainerDiv().setWidth(width);
	}

	@Override
	public String getHeight() {
		return getContainerDiv().getHeight();
	}

	@Override
	public void setHeight(String height) {
		getContainerDiv().setHeight(height);
	}

	public String getTitle() {
		return getTitleLabel().getValue();
	}

	public void setTitle(String title) {
		getTitleLabel().setValue(title);
		getTitleLabel().setStyle("font-size:12px;font-weight:bold;font-family:Montserrat;");
	}

	public CategoryModel getModel() {
		return model;
	}

	public void setModel(CategoryModel model) {
		this.model = model;
		updateChart();
	}

	public String getLeftMargin() {
		String left = getChart().getPlotMarginLeft();
		if (left.isEmpty()) {
			return _defaultLeftMargin;
		}
		return left;
	}

	public void setLeftMargin(String leftMargin) {
		getChart().setPlotMarginLeft(leftMargin);
	}
	
	public String getRightMargin() {
		String right = getChart().getPlotMarginRight();
		if (right.isEmpty()) {
			return _defaultRightMargin;
		}
		return right;
	}

	public void setRightMargin(String rightMargin) {
		getChart().setPlotMarginRight(rightMargin);
	}
	
	public String getTopMargin() {
		String top = getChart().getPlotMarginTop();
		if (top.isEmpty()) {
			return _defaultTopMargin;
		}
		return top;
	}

	public void setTopMargin(String topMargin) {
		getChart().setPlotMarginTop(topMargin);
	}
	
	public String getBottomMargin() {
		String bottom = getChart().getPlotMarginBottom();
		if (bottom.isEmpty()) {
			return _defaultBottomMargin;
		}
		return bottom;
	}

	public void setBottomMargin(String bottomMargin) {
		getChart().setPlotMarginBottom(bottomMargin);
	}

	public String getNoDataMessage() {
		return noDataMessage;
	}

	public void setNoDataMessage(String message) {
		this.noDataMessage = message;
	}
	
	public void setErrorMessage(String errorMessage) {
		if (errorMessage != null) {
			showError(errorMessage);
		}
	}

	public String getValueFormat() {
		return valueFormat;
	}
	

	/*
	 * The value format must be D3 format compatible.
	 * 
	 * The full documentation is found here:
	 * https://github.com/mbostock/d3/wiki/Formatting
	 * 
	 * Sample: "<" - left justifies the number ">" - right justifies the number
	 * "^" - centers the number "," - separates thousand values by comma ".2f" -
	 * shows two decimal values
	 * 
	 * The combined value format specifiers MUST be in correct order: Sample:
	 * "<,.2f"
	 */
	public void setValueFormat(String valueFormat) {
		this.valueFormat = valueFormat;
	}

	protected void showError(String message) {
		getErrorLabel().setValue(message);
		getChart().setVisible(false);
		getErrorLabel().setVisible(true);
	}
	
	public void refresh() {
		getErrorLabel().setValue("");
		getChart().setVisible(true);
		getErrorLabel().setVisible(false);
	}
	

}
