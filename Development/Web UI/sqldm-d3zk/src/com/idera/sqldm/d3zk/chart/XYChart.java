package com.idera.sqldm.d3zk.chart;

import java.util.regex.Matcher;

import org.zkoss.json.JSONArray;
import org.zkoss.json.JSONObject;
import org.zkoss.json.JSONValue;
import org.zkoss.zk.ui.event.Event;
import org.zkoss.zk.ui.event.EventListener;
import org.zkoss.zk.ui.event.Events;
import org.zkoss.zk.ui.event.SerializableEventListener;
import org.zkoss.zul.CategoryModel;
import org.zkoss.zul.SimpleCategoryModel;
import org.zkoss.zul.event.ChartDataEvent;
import org.zkoss.zul.event.ChartDataListener;

public class XYChart<T extends Number> extends RectangularChart {

	private static final long serialVersionUID = 1L;
	
	protected Boolean animateChart = true;
	protected Integer animationDelay = 50;
	
	protected Boolean drawVerticalGridLines = true;
	protected Integer verticalGridLineCount = 10;
	
	protected Boolean drawHorizontalGridLines = true;
	protected Integer horizontalGridLineCount = 10;
	
	protected String interpolationMode = "linear";
	
	protected CategoryModel model = new SimpleCategoryModel();
//	protected ChartDataListener dataListener;
//	protected EventListener<Event> updateListener;
	
	protected Boolean showSeriesLabels = true;
	protected Boolean truncateSeriesLabels = false;
	
	protected String xAxisScaleType = "time";
	protected Integer xAxisTickCount = 10;
	
	protected String yAxisScaleType = "linear";
	protected Integer yAxisTickCount = 10;
	protected int xAxisLegendSpacing = 25;
	
	protected Boolean drawYAxis = true;
	protected Boolean drawXAxis = true;
	
	protected Boolean drillable = false;
	  
	  public Boolean getDrillable()
	  {
	    return this.drillable;
	  }
	  
	  public void setDrillable(Boolean paramBoolean)
	  {
	    if (this.drillable != paramBoolean)
	    {
	      this.drillable = paramBoolean;
	      smartUpdate("drillable", this.drillable);
	    }
	  }
	public int getXAxisLegendSpacing() {
		return this.xAxisLegendSpacing;
	}
	
	public void setXAxisLegendSpacing(Integer xAxisLegendSpacing) {
		if( this.xAxisLegendSpacing != xAxisLegendSpacing ) {
			this.xAxisLegendSpacing = xAxisLegendSpacing;
			smartUpdate("xAxisLegendSpacing", this.xAxisLegendSpacing);
		}
	}
	
	public XYChart() { 
//		init();
	}
	
	private void init() {
//		this.updateListener = new SmartUpdateListener();
//		addEventListener("onSmartUpdateModel", this.updateListener);
	}
	
	public Boolean getDrawXAxis(){
		return this.drawXAxis;
	}
	
	public Boolean getDrawYAxis(){
		return this.drawYAxis;
	}
	
	public void setDrawXAxis(Boolean drawXAxis){
		if(this.drawXAxis != drawXAxis){
			this.drawXAxis = drawXAxis;
			smartUpdate("drawXAxis",this.drawXAxis);
		}
	}
	
	public void setDrawYAxis(Boolean drawYAxis){
		if(this.drawYAxis != drawYAxis){
			this.drawYAxis = drawYAxis;
			smartUpdate("drawYAxis",this.drawYAxis);
		}
	}
	/**
	 * Gets whether or not the lines of the chart will be animated when the chart is
	 * displayed or resized.
	 * 
	 * Note: Use this setting with care. Animation of large datasets is usually impractical, as the 
	 * browser will grind to a halt attempting to animate the paths.
	 *
	 * @return Returns true if the lines will be animated when the chart is displayed, false if not.
	 */
	public Boolean getAnimateChart() {
		return animateChart;
	}

	/**
	 * Sets whether or not the lines of the chart will be animated when the chart is
	 * displayed or resized.
	 *
	 * Note: Use this setting with care. Animation of large datasets is usually impractical, as the 
	 * browser will grind to a halt attempting to animate the paths.
	 *
	 * @param animateBars True if the lines will be animated when the chart is displayed, false if not.
	 */
	public void setAnimateChart(Boolean animateChart) {
		if( this.animateChart != animateChart ) {
			this.animateChart = animateChart;
			smartUpdate("animateChart", this.animateChart);
		}
	}

	/**
	 * Gets the delay between when the chart is first displayed and when the line animation begins
	 * in milliseconds.
	 *
	 * This field has no effect if animateChart is set to false.
	 *
	 * @return Returns the delay for when the animation begins in milliseconds
	 */
	public Integer getAnimationDelay() {
		return animationDelay;
	}

	/**
	 * Sets the delay between when the chart is first displayed and when the line animation begins
	 * in milliseconds.
	 *
	 * This field has no effect if animateChart is set to false.
	 *
	 * @param animationDelay The number of milliseconds to wait between when the chart is first displayed and when the animation begins
	 */
	public void setAnimationDelay(Integer animationDelay) {
		if( this.animationDelay != animationDelay ) {
			this.animationDelay = animationDelay;
			smartUpdate("animationDelay", this.animationDelay);
		}
	}
	
	/**
	 * Gets whether or not horizontal grid lines will be displayed on the chart for each of the Y-axis ticks.
	 * 
	 * @return Returns true if horizontal grid lines will be drawn, false otherwise.
	 */
	public Boolean getDrawHorizontalGridLines() {
		return drawHorizontalGridLines;
	}

	/**
	 * Sets whether or not horizontal grid lines will be displayed on the chart for each of the Y-axis ticks.
	 * 
	 * @param drawHorizontalGridLines True if horizontal grid lines will be drawn, false otherwise.
	 */
	public void setDrawHorizontalGridLines(Boolean drawHorizontalGridLines) {
		if( this.drawHorizontalGridLines != drawHorizontalGridLines ) {
			this.drawHorizontalGridLines = drawHorizontalGridLines;
			smartUpdate("drawHorizontalGridLines", this.drawHorizontalGridLines);
		}
	}
	
	public Integer getHorizontalGridLineCount() {
		return horizontalGridLineCount;
	}
	
	public void setHorizontalGridLineCount(Integer horizontalGridLineCount) {
		if( this.horizontalGridLineCount != horizontalGridLineCount ) {
			this.horizontalGridLineCount = horizontalGridLineCount;
			smartUpdate("horizontalGridLineCount", this.horizontalGridLineCount);
		}
	}
	
	/**
	 * Gets whether or not vertical grid lines will be displayed on the chart for each of the X-axis ticks.
	 * 
	 * @return Returns true if vertical grid lines will be drawn, false otherwise.
	 */
	public Boolean getDrawVerticalGridLines() {
		return drawVerticalGridLines;
	}
	
	/**
	 * Sets whether or not vertical grid lines will be displayed on the chart for each of the X-axis ticks.
	 * 
	 * @param drawVerticalGridLines True if vertical grid lines will be drawn, false otherwise.
	 */
	public void setDrawVerticalGridLines(Boolean drawVerticalGridLines) {
		if( this.drawVerticalGridLines != drawVerticalGridLines ) {
			this.drawVerticalGridLines = drawVerticalGridLines;
			smartUpdate("drawVerticalGridLines", this.drawVerticalGridLines);
		}
	}
	
	public Integer getVerticalGridLineCount() {
		return verticalGridLineCount;
	}
	
	public void setVerticalGridLineCount(Integer verticalGridLineCount) {
		if( this.verticalGridLineCount != verticalGridLineCount ) {
			this.verticalGridLineCount = verticalGridLineCount;
			smartUpdate("verticalGridLineCount", this.verticalGridLineCount);
		}
	}
	
	/**
	 * Gets the interpolation mode of the line or area shape.
	 * 
	 * @see <a href="https://github.com/mbostock/d3/wiki/SVG-Shapes#wiki-line_interpolate">https://github.com/mbostock/d3/wiki/SVG-Shapes#wiki-line_interpolate</a>
	 * 
	 * The default interpolationMode is "linear."
	 * 
	 * @return Returns a string indicating which interpolation mode will be used for the line or area shapes.
	 */
	public String getInterpolationMode() {
		return interpolationMode;
	}

	/**
	 * Sets the interpolation mode of the line or area shape.
	 * 
	 * @see <a href="https://github.com/mbostock/d3/wiki/SVG-Shapes#wiki-line_interpolate">https://github.com/mbostock/d3/wiki/SVG-Shapes#wiki-line_interpolate</a>
	 * 
	 * The default interpolationMode is "linear."
	 * 
	 * @param interpolationMode A string indicating which interpolation mode will be used for the line or area shapes.
	 */
	public void setInterpolationMode(String interpolationMode) {
		if( !this.interpolationMode.equals(interpolationMode) ) {
			this.interpolationMode = interpolationMode;
			smartUpdate("interpolationMode", this.interpolationMode);
		}
	}
	
	/**
	 * Gets the CategoryModel containing the data for the chart.
	 *
	 * The model has a Comparable<?> key that should be unique for the category and a
	 * Number for the value.
	 *
	 * The CategoryModel is a ZK built-in object and the most common implementation is SimpleCategoryModel.
	 *
	 * @return Returns the map containing the data for the chart
	 */
	public CategoryModel getModel() {
		return model;
	}

	/**
	 * Set the CategoryModel containing the data for the chart.
	 *
	 * The model has a Comparable<?> key that should be unique for the category and a
	 * Number for the value.
	 *
	 * The CategoryModel is a ZK built-in object and the most common implementation is SimpleCategoryModel.
	 *
	 * @param model The CategoryModel to use for the chart data.
	 */
	public void setModel(CategoryModel model) {
		
		if( this.model != model ) { 
		
//			if( this.model != null ) { 
//				this.model.removeChartDataListener(dataListener);
//			}
			
			this.model = model;
			
//			dataListener = new MyChartDataListener();
//			this.model.addChartDataListener(dataListener);
			smartUpdate("model", toJSON(model));
		}
	}
	
//	private class MyChartDataListener implements ChartDataListener {
//
//		@Override
//		public void onChange(ChartDataEvent arg0) {
//			Events.postEvent(new Event("onSmartUpdateModel", XYChart.this, null));
//		} 
//		
//	}
//	
//	private class SmartUpdateListener implements SerializableEventListener<Event> {
//
//		@Override
//		public void onEvent(Event arg0) throws Exception {
//			smartUpdate("model", toJSON(model));
//		} 
//		
//	}
	
	/**
	 * Gets whether or not the series name is displayed as a label in the right margin of the chart.
	 * 
	 * @return Returns true if the series name should be displayed, false if not.
	 */
	public Boolean getShowSeriesLabels() {
		return showSeriesLabels;
	}
	
	/**
	 * Sets whether or not the series name is displayed as a label in the right margin of the chart.
	 * 
	 * @param showSeriesLabels True if the series name should be displayed, false if not.
	 */
	public void setShowSeriesLabels(Boolean showSeriesLabels) {
		if( this.showSeriesLabels != showSeriesLabels ) {
			this.showSeriesLabels = showSeriesLabels;
			smartUpdate("showSeriesLabels", this.showSeriesLabels);
		}
	}
	
	/**
	 * Gets whether or not the series name labels will be truncated to fit into the right margin of 
	 * the chart. 
	 * 
	 * If truncation is true, then the labels will be truncated and '...' added to the end of them
	 * if they would overrun the right margin.
	 * 
	 * If truncation is false, then the size of the right margin will be adjusted so that the
	 * longest label fits and then will have the amount of space specified by plotMarginRight 
	 * added to the right side. 
	 * 
	 * This field has no effect if showSeriesLabels is set to false.
	 * 
	 * @return Returns true if the series names will be truncated, false if not.
	 */
	public Boolean getTruncateSeriesLabels() {
		return truncateSeriesLabels;
	}

	/**
	 * Sets whether or not the series name labels will be truncated to fit into the right margin of 
	 * the chart. 
	 * 
	 * If truncation is true, then the labels will be truncated and '...' added to the end of them
	 * if they would overrun the right margin.
	 * 
	 * If truncation is false, then the size of the right margin will be adjusted so that the
	 * longest label fits and then will have the amount of space specified by plotMarginRight 
	 * added to the right side. 
	 * 
	 * This field has no effect if showSeriesLabels is set to false.
	 * 
	 * @param truncateSeriesLabels True if the series names will be truncated, false if not.
	 */
	public void setTruncateSeriesLabels(Boolean truncateSeriesLabels) {
		if( this.truncateSeriesLabels != truncateSeriesLabels ) {
			this.truncateSeriesLabels = truncateSeriesLabels;
			smartUpdate("truncateSeriesLabels", this.truncateSeriesLabels);
		}
	}	
	
	/**
	 * Gets the type of scale used to create the X-axis.
	 * 
	 * Supported scale types are: "linear," "pow," "log," and "time." 
	 * 
	 * @see <a href="https://github.com/mbostock/d3/wiki/Quantitative-Scales">https://github.com/mbostock/d3/wiki/Quantitative-Scales</a>
	 *
	 * The default X-axis scale type is "time."
	 * 
	 * @return Returns a string indicating the type of scale used to create the X-axis.
	 */
	public String getXAxisScaleType() {
		return xAxisScaleType;
	}

	/**
	 * Sets the type of scale used to create the X-axis.
	 * 
	 * Supported scale types are: "linear," "pow," "log," and "time." 
	 * 
	 * @see <a href="https://github.com/mbostock/d3/wiki/Quantitative-Scales">https://github.com/mbostock/d3/wiki/Quantitative-Scales</a>
	 *
	 * The default X-axis scale type is "time."
	 * 
	 * @param xAxisScaleType A string indicating the type of scale used to create the X-axis.
	 */
	public void setXAxisScaleType(String xAxisScaleType) {
		if( !this.xAxisScaleType.equals(xAxisScaleType) ) {
			this.xAxisScaleType = xAxisScaleType;
			smartUpdate("xAxisScaleType", this.xAxisScaleType);
		}
	}
	
	/** 
	 * Gets the approximate count of ticks for the X-axis scale's domain. 
	 * 
	 * @see <a href="https://github.com/mbostock/d3/wiki/Quantitative-Scales#wiki-linear_ticks">https://github.com/mbostock/d3/wiki/Quantitative-Scales#wiki-linear_ticks</a>
	 * 
	 * @return Returns the approximate count of ticks for the X-axis domain 
	 */
	public Integer getXAxisTickCount() {
		return xAxisTickCount;
	}

	/**
	 * Sets the approximate count of ticks for the X-axis scale's domain. 
	 * 
	 * @see <a href="https://github.com/mbostock/d3/wiki/Quantitative-Scales#wiki-linear_ticks">https://github.com/mbostock/d3/wiki/Quantitative-Scales#wiki-linear_ticks</a>
	 * 
	 * @param xAxisTickCount The approximate count of ticks for the X-axis domain
	 */
	public void setXAxisTickCount(Integer xAxisTickCount) { 
		if( this.xAxisTickCount != xAxisTickCount ) {
			this.xAxisTickCount = xAxisTickCount;
			smartUpdate("xAxisTickCount", this.xAxisTickCount);
		}
	}
	
	/**
	 * Gets the type of scale used to create the Y-axis.
	 * 
	 * Supported scale types are: "linear," "pow," "log," and "time." 
	 * 
	 * @see <a href="https://github.com/mbostock/d3/wiki/Quantitative-Scales">https://github.com/mbostock/d3/wiki/Quantitative-Scales</a>
	 *
	 * The default Y-axis scale type is "linear."
	 * 
	 * @return Returns a string indicating the type of scale used to create the Y-axis.
	 */
	public String getYAxisScaleType() {
		return yAxisScaleType;
	}

	/**
	 * Sets the type of scale used to create the Y-axis.
	 * 
	 * Supported scale types are: "linear," "pow," "log," and "time." 
	 * 
	 * @see <a href="https://github.com/mbostock/d3/wiki/Quantitative-Scales">https://github.com/mbostock/d3/wiki/Quantitative-Scales</a>
	 *
	 * The default Y-axis scale type is "linear."
	 * 
	 * @param yAxisScaleType A string indicating the type of scale used to create the Y-axis.
	 */
	public void setYAxisScaleType(String yAxisScaleType) {
		if( !this.yAxisScaleType.equals(yAxisScaleType) ) {
			this.yAxisScaleType = yAxisScaleType;
			smartUpdate("yAxisScaleType", this.yAxisScaleType);
		}
	}

	/** 
	 * Gets the approximate count of ticks for the Y-axis scale's domain. 
	 * 
	 * @see <a href="https://github.com/mbostock/d3/wiki/Quantitative-Scales#wiki-linear_ticks">https://github.com/mbostock/d3/wiki/Quantitative-Scales#wiki-linear_ticks</a>
	 * 
	 * @return Returns the approximate count of ticks for the Y-axis domain 
	 */
	public Integer getYAxisTickCount() {
		return yAxisTickCount;
	}

	/**
	 * Sets the approximate count of ticks for the Y-axis scale's domain. 
	 * 
	 * @see <a href="https://github.com/mbostock/d3/wiki/Quantitative-Scales#wiki-linear_ticks">https://github.com/mbostock/d3/wiki/Quantitative-Scales#wiki-linear_ticks</a>
	 * 
	 * @param yAxisScaleType The approximate count of ticks for the Y-axis domain
	 */
	public void setYAxisTickCount(Integer yAxisTickCount) { 
		if( this.yAxisTickCount != yAxisTickCount ) {
			this.yAxisTickCount = yAxisTickCount;
			smartUpdate("yAxisTickCount", this.yAxisTickCount);
		}
	}
	
	@Override
	protected void renderProperties(org.zkoss.zk.ui.sys.ContentRenderer renderer) throws java.io.IOException {
		super.renderProperties(renderer);
		render(renderer, "animateChart", animateChart);
		render(renderer, "animationDelay", animationDelay);		
		render(renderer, "drawVerticalGridLines", drawVerticalGridLines);
		render(renderer, "verticalGridLineCount", verticalGridLineCount);
		render(renderer, "drawHorizontalGridLines", drawHorizontalGridLines);
		render(renderer, "horizontalGridLineCount", horizontalGridLineCount);
		render(renderer, "interpolationMode", interpolationMode);
		render(renderer, "model", toJSON(model));
		render(renderer, "showSeriesLabels", showSeriesLabels);
		render(renderer, "truncateSeriesLabels", truncateSeriesLabels);		
		render(renderer, "xAxisScaleType", xAxisScaleType);
		render(renderer, "xAxisTickCount", xAxisTickCount);
		render(renderer, "yAxisScaleType", yAxisScaleType);
		render(renderer, "yAxisTickCount", yAxisTickCount);
		render(renderer,"xAxisLegendSpacing",xAxisLegendSpacing);
		render(renderer,"drawXAxis",drawXAxis);
		render(renderer,"drawYAxis",drawYAxis);
		render(renderer, "drillable", this.drillable);
	}
	
	protected JSONObject toJSON(CategoryModel model) {

		JSONObject json = new JSONObject();

		for(Comparable<?> series : model.getSeries() ) {

			String sanitizedSeries = String.valueOf(series);

			sanitizedSeries = JSONValue.toJSONString(sanitizedSeries);
			sanitizedSeries = sanitizedSeries.substring(1, sanitizedSeries.length()-1);
			sanitizedSeries = sanitizedSeries.replaceAll("[']", Matcher.quoteReplacement("\\'"));
            sanitizedSeries = sanitizedSeries.replaceAll("[\\\\]", Matcher.quoteReplacement(""));
			
			JSONArray categoryJSON = new JSONArray();
			
			for( Comparable<?> category : model.getCategories() ) {

				T value = (T) model.getValue(series, category);
				if( value == null ) continue;
				
				String sanitizedCategory = String.valueOf(category);

				sanitizedCategory = JSONValue.toJSONString(sanitizedCategory);
				sanitizedCategory = sanitizedCategory.substring(1, sanitizedCategory.length()-1);
				sanitizedCategory = sanitizedCategory.replaceAll("[']", Matcher.quoteReplacement("\\'"));
                //Fixed issue DE42841
                sanitizedCategory = sanitizedCategory.replaceAll("[\\\\]", Matcher.quoteReplacement(""));

				JSONObject obj = new JSONObject();
				obj.put("category", sanitizedCategory);
				obj.put("value", value);
				
				categoryJSON.add(obj);
			}
			
			json.put(sanitizedSeries, categoryJSON);
		}

		return json;
	}
	
}
