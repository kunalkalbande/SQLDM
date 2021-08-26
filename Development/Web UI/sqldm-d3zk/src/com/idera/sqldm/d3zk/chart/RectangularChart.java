package com.idera.sqldm.d3zk.chart;


public abstract class RectangularChart extends AbstractChart {

	private static final long serialVersionUID = 1L;

	protected String plotMarginTop = "0";
	protected String plotMarginRight = "0";
	protected String plotMarginBottom = "0";
	protected String plotMarginLeft = "0";
	
	protected String xAxisTickFormat = null;
	protected String yAxisTickFormat = null;
	
	protected int yAxisInnerTickSize = 0;
	protected int yAxisOuterTickSize = 0;
	protected int yAxisTickPadding = 0;
	protected String yAxisTitle = null;
	protected String yAxisLabelOrient = "left";
	
	protected int yAxisMinDomainValue = 0;
	protected int yAxisMaxDomainValue = 100;
	protected boolean yAxisCustomMinDomainValue = false;
	protected boolean yAxisCustomMaxDomainValue = false;

	protected Object xAxisMinDomainValue = null;
	protected Object xAxisMaxDomainValue = null;
	protected boolean xAxisCustomMinDomainValue = false;
	protected boolean xAxisCustomMaxDomainValue = false;

	
	/**
	 * Gets the size of the margin between the top of the component and the top of the plot
	 * area.
	 *
	 * If this value is specified as a number (e.g. "10"), it is treated as a static number
	 * of pixels to apply to the margin.
	 *
	 * If this value is specified as a percentage (e.g. "10%"), it is treated as a percentage
	 * of the total height of the component and will expand and shrink if the component's
	 * vertical size changes.
	 *
	 * The value is applied by taking the specified height of the component, subtracting the
	 * values of the top and bottom margins, and then translating the plot area down by the
	 * value of the top margin.
	 *
	 * Default: 0
	 *
	 * @return Returns the size of the top margin expressed as either the number of pixels or the percentage of total vertical space
	 */
	public String getPlotMarginTop() {
		return plotMarginTop;
	}

	/**
	 * Sets the size of the margin between the top of the component and the top of the plot
	 * area.
	 *
	 * If this value is specified as a number (e.g. "10"), it is treated as a static number
	 * of pixels to apply to the margin.
	 *
	 * If this value is specified as a percentage (e.g. "10%"), it is treated as a percentage
	 * of the total height of the component and will expand and shrink if the component's
	 * vertical size changes.
	 *
	 * The value is applied by taking the specified height of the component, subtracting the
	 * values of the top and bottom margins, and then translating the plot area down by the
	 * value of the top margin.
	 *
	 * Default: 0
	 *
	 * @param plotMarginTop The size of the top margin expressed as either the number of pixels or the percentage of total vertical space.
	 */
	public void setPlotMarginTop(String plotMarginTop) {
		if( this.plotMarginTop != plotMarginTop ) {
			this.plotMarginTop = plotMarginTop;
			smartUpdate("plotMarginTop", this.plotMarginTop);
		}
	}

	/**
	 * Gets the size of the margin between the right of the component and the right edge of
	 * the plot area.
	 *
	 * If this value is specified as a number (e.g. "10"), it is treated as a static number
	 * of pixels to apply to the margin.
	 *
	 * If this value is specified as a percentage (e.g. "10%"), it is treated as a percentage
	 * of the total width of the component and will expand and shrink if the component's
	 * horizontal size changes.
	 *
	 * The value is applied by taking the specified width of the component, subtracting the
	 * values of the left and right margins, and then translating the plot area right by the
	 * value of the left margin.
	 *
	 * Default: 0
	 *
	 * @return The size of the right margin expressed as either the number of pixels or the percentage of total horizontal space
	 */
	public String getPlotMarginRight() {
		return plotMarginRight;
	}

	/**
	 * Sets the size of the margin between the right of the component and the right edge of
	 * the plot area in pixels.
	 *
	 * If this value is specified as a number (e.g. "10"), it is treated as a static number
	 * of pixels to apply to the margin.
	 *
	 * If this value is specified as a percentage (e.g. "10%"), it is treated as a percentage
	 * of the total width of the component and will expand and shrink if the component's
	 * horizontal size changes.
	 *
	 * The value is applied by taking the specified width of the component, subtracting the
	 * values of the left and right margins, and then translating the plot area right by the
	 * value of the left margin.
	 *
	 * Default: 0
	 *
	 * @param plotMarginRight The size of the right margin expressed as either the number of pixels or the percentage of total horizontal space
	 */
	public void setPlotMarginRight(String plotMarginRight) {
		if( this.plotMarginRight != plotMarginRight ) {
			this.plotMarginRight = plotMarginRight;
			smartUpdate("plotMarginRight", this.plotMarginRight);
		}
	}

	/**
	 * Gets the size of the margin between the bottom of the component and the bottom of the
	 * plot area in pixels.
	 *
	 * If this value is specified as a number (e.g. "10"), it is treated as a static number
	 * of pixels to apply to the margin.
	 *
	 * If this value is specified as a percentage (e.g. "10%"), it is treated as a percentage
	 * of the total height of the component and will expand and shrink if the component's
	 * vertical size changes.
	 *
	 * The value is applied by taking the specified height of the component, subtracting the
	 * values of the top and bottom margins, and then translating the plot area down by the
	 * value of the top margin.
	 *
	 * Default: 0
	 *
	 * @return Returns the size of the bottom margin expressed as either the number of pixels or the percentage of total vertical space
	 */
	public String getPlotMarginBottom() {
		return plotMarginBottom;
	}

	/**
	 * Sets the size of the margin between the bottom of the component and the bottom of the
	 * plot area in pixels.
	 *
	 * If this value is specified as a number (e.g. "10"), it is treated as a static number
	 * of pixels to apply to the margin.
	 *
	 * If this value is specified as a percentage (e.g. "10%"), it is treated as a percentage
	 * of the total height of the component and will expand and shrink if the component's
	 * vertical size changes.
	 *
	 * The value is applied by taking the specified height of the component, subtracting the
	 * values of the top and bottom margins, and then translating the plot area down by the
	 * value of the top margin.
	 *
	 * Default: 0
	 *
	 * @param plotMarginBottom The size of the bottom margin expressed as either the number of pixels or the percentage of total vertical space
	 */
	public void setPlotMarginBottom(String plotMarginBottom) {
		if( this.plotMarginBottom != plotMarginBottom ) {
			this.plotMarginBottom = plotMarginBottom;
			smartUpdate("plotMarginBottom", this.plotMarginBottom);
		}
	}

	/**
	 * Gets the size of the margin between the left of the component and the left edge of
	 * the plot area in pixels.
	 *
	 * If this value is specified as a number (e.g. "10"), it is treated as a static number
	 * of pixels to apply to the margin.
	 *
	 * If this value is specified as a percentage (e.g. "10%"), it is treated as a percentage
	 * of the total width of the component and will expand and shrink if the component's
	 * horizontal size changes.
	 *
	 * The value is applied by taking the specified width of the component, subtracting the
	 * values of the left and right margins, and then translating the plot area right by the
	 * value of the left margin.
	 *
	 * Default: 0
	 *
	 * @return Returns the size of the left margin expressed as either the number of pixels or the percentage of total horizontal space
	 */
	public String getPlotMarginLeft() {
		return plotMarginLeft;
	}

	/**
	 * Sets the size of the margin between the left of the component and the left edge of
	 * the plot area in pixels.
	 *
	 * If this value is specified as a number (e.g. "10"), it is treated as a static number
	 * of pixels to apply to the margin.
	 *
	 * If this value is specified as a percentage (e.g. "10%"), it is treated as a percentage
	 * of the total width of the component and will expand and shrink if the component's
	 * horizontal size changes.
	 *
	 * The value is applied by taking the specified width of the component, subtracting the
	 * values of the left and right margins, and then translating the plot area right by the
	 * value of the left margin.
	 *
	 * Default: 0
	 *
	 * @param plotMarginLeft The size of the left margin expressed as either the number of pixels or the percentage of total horizontal space
	 */
	public void setPlotMarginLeft(String plotMarginLeft) {
		if( this.plotMarginLeft != plotMarginLeft ) {
			this.plotMarginLeft = plotMarginLeft;
			smartUpdate("plotMarginLeft", this.plotMarginLeft);
		}
	}
	
	/**
	 * Gets the format applied to the ticks for the X-axis of the plot.
	 *
	 * The value is specified as a format string that the d3js library recognizes. This format
	 * is specific to the JS library that the graphs are based on.
	 *
	 * @see <a href="https://github.com/mbostock/d3/wiki/Formatting">https://github.com/mbostock/d3/wiki/Formatting</a>
	 *
	 * @return Returns the format applied to the ticks for the X-axis, or null if no format is specified
	 */
	public String getXAxisTickFormat() {
		return xAxisTickFormat;
	}

	/**
	 * Sets the format applied to the ticks for the X-axis of the plot.
	 *
	 * The value is specified as a format string that the d3js library recognizes. This format
	 * is specific to the JS library that the graphs are based on.
	 *
	 * @see <a href="https://github.com/mbostock/d3/wiki/Formatting">https://github.com/mbostock/d3/wiki/Formatting</a>
	 *
	 * @param xAxisTickFormat The format to apply to the ticks for the X-axis, or null for no format
	 */
	public void setXAxisTickFormat(String xAxisTickFormat) {
		if( this.xAxisTickFormat != xAxisTickFormat ) {
			this.xAxisTickFormat = xAxisTickFormat;
			smartUpdate("xAxisTickFormat", this.xAxisTickFormat);
		}
	}

	/**
	 * Gets the format applied to the ticks for the Y-axis of the plot.
	 *
	 * The value is specified as a format string that the d3js library recognizes. This format
	 * is specific to the JS library that the graphs are based on.
	 *
	 * @see <a href="https://github.com/mbostock/d3/wiki/Formatting">https://github.com/mbostock/d3/wiki/Formatting</a>
	 *
	 * @return Returns the format applied to the ticks for the Y-axis, or null if no format is specified
	 */
	public String getYAxisTickFormat() {
		return yAxisTickFormat;
	}

	/**
	 * Sets the format applied to the ticks for the Y-axis of the plot.
	 *
	 * The value is specified as a format string that the d3js library recognizes. This format
	 * is specific to the JS library that the graphs are based on.
	 *
	 * @see <a href="https://github.com/mbostock/d3/wiki/Formatting">https://github.com/mbostock/d3/wiki/Formatting</a>
	 *
	 * @param yAxisTickFormat The format to apply to the ticks for the Y-axis, or null for no format
	 */
	public void setYAxisTickFormat(String yAxisTickFormat) {
		if( this.yAxisTickFormat != yAxisTickFormat ) {
			this.yAxisTickFormat = yAxisTickFormat;
			smartUpdate("yAxisTickFormat", this.yAxisTickFormat);
		}
	}	

	/**
	 * Gets the inner tick size for the Y-axis. The inner tick size controls the length of the tick lines, offset from the native position of the axis.
	 *
	 * Default: 0
	 *
	 * @return Returns the inner tick size for the Y-axis in pixels
	 */
	public int getYAxisInnerTickSize() {
		return yAxisInnerTickSize;
	}

	/**
	 * Gets the inner tick size for the Y-axis. The inner tick size controls the length of the tick lines, offset from the native position of the axis.
	 *
	 * Default: 0
	 *
	 * @param yAxisInnerTickSize The inner tick size for the Y-axis in pixels
	 */
	public void setYAxisInnerTickSize(int yAxisInnerTickSize) {
		this.yAxisInnerTickSize = yAxisInnerTickSize;
	}

	/**
	 * Gets the outer tick size for the Y-axis.
	 *
	 * From: https://github.com/mbostock/d3/wiki/SVG-Axes#wiki-tickSize
	 *
	 * The outer tick size controls the length of the square ends of the domain path, offset from the native position of the axis. Thus, the “outer
	 * ticks” are not actually ticks but part of the domain path, and their position is determined by the associated scale's domain extent. Thus,
	 * outer ticks may overlap with the first or last inner tick. An outer tick size of 0 suppresses the square ends of the domain path, instead
	 * producing a straight line.
	 *
	 * @return Returns the outer tick size for the Y-axis in pixels
	 */
	public int getYAxisOuterTickSize() {
		return yAxisOuterTickSize;
	}

	/**
	 * Gets the outer tick size for the Y-axis.
	 *
	 * From: https://github.com/mbostock/d3/wiki/SVG-Axes#wiki-tickSize
	 *
	 * The outer tick size controls the length of the square ends of the domain path, offset from the native position of the axis. Thus, the “outer
	 * ticks” are not actually ticks but part of the domain path, and their position is determined by the associated scale's domain extent. Thus,
	 * outer ticks may overlap with the first or last inner tick. An outer tick size of 0 suppresses the square ends of the domain path, instead
	 * producing a straight line.
	 *
	 * @param yAxisOuterTickSize The outer tick size for the Y-axis in pixels
	 */
	public void setYAxisOuterTickSize(int yAxisOuterTickSize) {
		this.yAxisOuterTickSize = yAxisOuterTickSize;
	}

	public int getYAxisTickPadding() {
		return yAxisTickPadding;
	}

	public void setYAxisTickPadding(int yAxisTickPadding) {
		this.yAxisTickPadding = yAxisTickPadding;
	}

	/**
	 * Gets the label used for the title of the Y axis on the plot area.
	 *
	 * @return Returns the label used for the title of the Y axis on the plot area.
	 */
	public String getYAxisTitle() {
		return yAxisTitle;
	}

	/**
	 * Sets the label used for the title of the Y axis on the plot area.
	 *
	 * @param yAxisTitle The label to use for the title of the Y axis on the plot area.
	 */
	public void setYAxisTitle(String yAxisTitle) {
		if( this.yAxisTitle != yAxisTitle ) {
			this.yAxisTitle = yAxisTitle;
			smartUpdate("yAxisTitle", this.yAxisTitle);
		}
	}
	
	/**
	 * Gets the orientation of the Y axis labels in relation to the chart.
	 *
	 * "left" indicates that the labels will be aligned against the left side of the graph, with any additional space between the label and the Y axis
	 * "right" indicates that the labels will be aligned against the Y axis, with any addition space between the label and the left side of the graph
	 *
	 * Default: "left"
	 *
	 * @return Returns a string indicating the orientation of the Y axis labels
	 */
	public String getYAxisLabelOrient() {
		return yAxisLabelOrient;
	}

	/**
	 * Sets the orientation of the Y axis labels in relation to the chart.
	 *
	 * "left" indicates that the labels will be aligned against the left side of the graph, with any additional space between the label and the Y axis
	 * "right" indicates that the labels will be aligned against the Y axis, with any addition space between the label and the left side of the graph
	 *
	 * Default: "left"
	 *
	 * @param yAxisLabelOrient A string indicating the orientation of the Y axis labels
	 */
	public void setYAxisLabelOrient(String yAxisLabelOrient) {
		if( !this.yAxisLabelOrient.equals(yAxisLabelOrient) ) {
			this.yAxisLabelOrient = yAxisLabelOrient;
			smartUpdate("yAxisLabelOrient", this.yAxisLabelOrient);
		}
	}
	
	@Override
	protected void renderProperties(org.zkoss.zk.ui.sys.ContentRenderer renderer) throws java.io.IOException {

		super.renderProperties(renderer);

		render(renderer, "plotMarginTop", plotMarginTop);
		render(renderer, "plotMarginRight", plotMarginRight);
		render(renderer, "plotMarginBottom", plotMarginBottom);
		render(renderer, "plotMarginLeft", plotMarginLeft);

		render(renderer, "xAxisTickFormat", xAxisTickFormat);
		
		render(renderer, "yAxisTickFormat", yAxisTickFormat);
		render(renderer, "yAxisInnerTickSize", yAxisInnerTickSize);
		render(renderer, "yAxisOuterTickSize", yAxisOuterTickSize);
		render(renderer, "yAxisTickPadding", yAxisTickPadding);
		render(renderer, "yAxisTitle", yAxisTitle);
		render(renderer, "yAxisLabelOrient", yAxisLabelOrient);
		render(renderer, "yAxisCustomMinDomainValue", yAxisCustomMinDomainValue);
		render(renderer, "yAxisCustomMaxDomainValue", yAxisCustomMaxDomainValue);
		render(renderer, "yAxisMinDomainValue", yAxisMinDomainValue);
		render(renderer, "yAxisMaxDomainValue", yAxisMaxDomainValue);
		render(renderer, "xAxisCustomMinDomainValue", xAxisCustomMinDomainValue);
		render(renderer, "xAxisCustomMaxDomainValue", xAxisCustomMaxDomainValue);
		render(renderer, "xAxisMinDomainValue", xAxisMinDomainValue);
		render(renderer, "xAxisMaxDomainValue", xAxisMaxDomainValue);
	}

	/**
	 * Get y-axis minimum domain value
	 * 
	 * @return the yAxisMinDomainValue
	 */
	public int getyAxisMinDomainValue() {
		return yAxisMinDomainValue;
	}

	/**
	 * Set y-axis minimum domain value
	 * 
	 * @param yAxisMinDomainValue the yAxisMinDomainValue to set
	 */
	public void setyAxisMinDomainValue(int yAxisMinDomainValue) {
		if(this.yAxisMinDomainValue != yAxisMinDomainValue){
			this.yAxisMinDomainValue = yAxisMinDomainValue;
			smartUpdate("yAxisMinDomainValue", yAxisMinDomainValue);
		}
	}

	/**
	 * Get y-axis Maximum domain value
	 * 
	 * @return the yAxisMaxDomainValue
	 */
	public int getyAxisMaxDomainValue() {
		return yAxisMaxDomainValue;
	}

	/**
	 * Get y-axis maximum domain value
	 * 
	 * @param yAxisMaxDomainValue the yAxisMaxDomainValue to set
	 */
	public void setyAxisMaxDomainValue(int yAxisMaxDomainValue) {
		if(this.yAxisMaxDomainValue != yAxisMaxDomainValue){
			this.yAxisMaxDomainValue = yAxisMaxDomainValue;
			smartUpdate("yAxisMaxDomainValue", yAxisMaxDomainValue);
		}
	}

	/**
	 * Return whether y-axis custom domain value is set or not. If this value is true, than yAxisMaxDomainValue is used on graph
	 * 
	 * @return the yAxisCustomMaxDomainValue
	 */
	public boolean isyAxisCustomMaxDomainValue() {
		return yAxisCustomMaxDomainValue;
	}

	/**
	 * Set whether to enable y-axis custom domain maximum value
	 * 
	 * @param yAxisCustomMaxDomainValue the yAxisCustomMaxDomainValue to set
	 */
	public void setyAxisCustomMaxDomainValue(boolean yAxisCustomMaxDomainValue) {
		if(this.yAxisCustomMaxDomainValue != yAxisCustomMaxDomainValue){
			this.yAxisCustomMaxDomainValue = yAxisCustomMaxDomainValue;
			smartUpdate("yAxisCustomMaxDomainValue", yAxisCustomMaxDomainValue);
		}
	}

	/**
	 * Return whether y-axis custom domain value is set or not. If this value is true, than yAxisMinDomainValue is used on graph
	 * 
	 * @return the yAxisCustomMinDomainValue
	 */
	public boolean isyAxisCustomMinDomainValue() {
		return yAxisCustomMinDomainValue;
	}

	/**
	 * Set whether to enable y-axis custom domain minimum value
	 * 
	 * @param yAxisCustomMinDomainValue the yAxisCustomMinDomainValue to set
	 */
	public void setyAxisCustomMinDomainValue(boolean yAxisCustomMinDomainValue) {
		if(this.yAxisCustomMinDomainValue != yAxisCustomMinDomainValue){
			this.yAxisCustomMinDomainValue = yAxisCustomMinDomainValue;
			smartUpdate("yAxisCustomMinDomainValue", yAxisCustomMinDomainValue);
		}
	}
	
/*Changes to support X axis Domain configurable*/
	/**
	 * Get x-axis minimum domain Value
	 * 
	 * @return the xAxisMinDomainValue
	 */
	public Object getxAxisMinDomainValue() {
		return xAxisMinDomainValue;
	}

	/**
	 * Set x-axis minimum domain value
	 * 
	 * @param xAxisMinDomainValue the xAxisMinDomainValue to set
	 */
	public void setxAxisMinDomainValue(Object xAxisMinDomainValue ) {
		if(this.xAxisMinDomainValue != xAxisMinDomainValue){
			this.xAxisMinDomainValue = xAxisMinDomainValue;
			smartUpdate("xAxisMinDomainValue", xAxisMinDomainValue);
		}
	}

	/**
	 * Get x-axis Maximum domain value
	 * 
	 * @return the xAxisMaxDomainValue
	 */
	public Object getxAxisMaxDomainValue() {
		return xAxisMaxDomainValue;
	}

	/**
	 * Get x-axis maximum domain value
	 * 
	 * @param xAxisMaxDomainValue the xAxisMaxDomainValue to set
	 */
	public void setxAxisMaxDomainValue(Object xAxisMaxDomainValue) {
		if(this.xAxisMaxDomainValue != xAxisMaxDomainValue){
			this.xAxisMaxDomainValue = xAxisMaxDomainValue;
			smartUpdate("xAxisMaxDomainValue", xAxisMaxDomainValue);
		}
	}

	/**
	 * Return whether x-axis custom domain value is set or not. If this value is true, than xAxisCustomMaxDomainValue is used on graph
	 * 
	 * @return the xAxisCustomMaxDomainValue
	 */
	public boolean isxAxisCustomMaxDomainValue() {
		return xAxisCustomMaxDomainValue;
	}

	/**
	 * Set whether to enable y-axis custom domain maximum value
	 * 
	 * @param yAxisCustomMaxDomainValue the yAxisCustomMaxDomainValue to set
	 */
	public void setxAxisCustomMaxDomainValue(boolean xAxisCustomMaxDomainValue) {
		if(this.xAxisCustomMaxDomainValue != xAxisCustomMaxDomainValue){
			this.xAxisCustomMaxDomainValue = xAxisCustomMaxDomainValue;
			smartUpdate("xAxisCustomMaxDomainValue", xAxisCustomMaxDomainValue);
		}
	}

	/**
	 * Return whether x-axis custom domain value is set or not. If this value is true, than xAxisCustomMinDomainValue  is used on graph
	 * 
	 * @return the xAxisCustomMinDomainValue
	 */
	public boolean isxAxisCustomMinDomainValue() {
		return xAxisCustomMinDomainValue;
	}

	/**
	 * Set whether to enable x-axis custom domain minimum value
	 * 
	 * @param xAxisCustomMinDomainValue the xAxisCustomMinDomainValue to set
	 */
	public void setxAxisCustomMinDomainValue(boolean xAxisCustomMinDomainValue) {
		if(this.xAxisCustomMinDomainValue != xAxisCustomMinDomainValue){
			this.xAxisCustomMinDomainValue = xAxisCustomMinDomainValue;
			smartUpdate("xAxisCustomMinDomainValue", xAxisCustomMinDomainValue);
		}
	}

	
	
	
}
