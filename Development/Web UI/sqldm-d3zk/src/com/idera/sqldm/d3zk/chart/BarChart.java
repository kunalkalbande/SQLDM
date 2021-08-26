package com.idera.sqldm.d3zk.chart;

import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.Map;
import java.util.Map.Entry;
import java.util.regex.Matcher;

import org.zkoss.json.JSONValue;

public class BarChart<T extends Number> extends RectangularChart {

	static final long serialVersionUID = 1L;

	protected String orient = "vertical";

	protected Double innerPadding = 0D;
	protected Double outerPadding = 0D;

	protected Boolean showValueLabels = true;
	protected String valueFormat = "";

	protected Map<String, T> series = new HashMap<String, T>();

	protected Boolean animateBars = true;
	protected Integer animationDelay = 50;
	protected Integer animationLength = 500;

	/**
	 * Gets the orientation of the bar chart.
	 *
	 * "vertical" indicates a bar chart where the bars run from bottom to top
	 * "horizontal" indicates a bar chart where the bars run from left to right
	 *
	 * Default: "vertical"
	 *
	 * @return Returns a string indicating the orientation of the bar chart
	 */
	public String getOrient() {
		return orient;
	}

	/**
	 *
	 * Gets the orientation of the bar chart.
	 *
	 * "vertical" indicates a bar chart where the bars run from bottom to top
	 * "horizontal" indicates a bar chart where the bars run from left to right
	 *
	 * Default: "vertical"
	 *
	 * @param A string indicating the orientation of the bar chart
	 */
	public void setOrient(String orient) {
		if( !this.orient.equals(orient) ) {
			this.orient = orient;
			smartUpdate("orient", this.orient);
		}
	}

	/**
	 * Get the amount of padding between the bars of the chart, typically a value between
	 * 0 and 1.
	 *
	 * The value is expressed as a ratio of the space from the beginning of the first
	 * bar to the end of the last bar that will be used for padding.
	 *
	 * A value of 0.5 (50%) means that half the space will be given to padding, so the
	 * width of the bar will be equal to be width of the spacing between the bars.
	 *
	 * Increasing the inner padding will increase the space between the bars and decrease
	 * the width of the bars themselves. Decreasing the inner padding will decrease the
	 * space between the bars and increase the width of the bars.
	 *
	 * @see <a href="https://github.com/mbostock/d3/wiki/Ordinal-Scales#wiki-ordinal_rangeRoundBands">https://github.com/mbostock/d3/wiki/Ordinal-Scales#wiki-ordinal_rangeRoundBands</a>
	 *
	 * Default: 0
	 *
	 * @return Returns the ratio of space between the bars to the width of the bars themselves
	 */
	public Double getInnerPadding() {
		return innerPadding;
	}


	/**
	 * Sets the amount of padding between the bars of the graph to the width of the bars
	 * themselves.
	 *
	 * The value is expressed as a ratio of the space from the beginning of the first
	 * bar to the end of the last bar that will be used for padding.
	 *
	 * A value of 0.5 (50%) means that half the space will be given to padding, so the
	 * width of the bar will be equal to be width of the spacing between the bars.
	 *
	 * Increasing the inner padding will increase the space between the bars and decrease
	 * the width of the bars themselves. Decreasing the inner padding will decrease the
	 * space between the bars and increase the width of the bars.
	 *
	 * @see <a href="https://github.com/mbostock/d3/wiki/Ordinal-Scales#wiki-ordinal_rangeRoundBands">https://github.com/mbostock/d3/wiki/Ordinal-Scales#wiki-ordinal_rangeRoundBands</a>
	 *
	 * Default: 0
	 *
	 * @param innerPadding The ratio of space between the bars to the width of the bars themselves
	 */
	public void setInnerPadding(Double innerPadding) {
		if( this.innerPadding != innerPadding ) {
			this.innerPadding = innerPadding;
			smartUpdate("innerPadding", this.innerPadding);
		}
	}

	/**
	 * Gets the amount of padding between the first and last bars of the graph and the edges
	 * of the plot.
	 *
	 * The value is expressed as a ratio of the size of each bar in the graph.
	 *
	 * A value of 0.5 (50%) means that the space between the edges of the plot and the first
	 * and last bar will be half as wide as the bars of the graph.
	 *
	 * @see <a href="https://github.com/mbostock/d3/wiki/Ordinal-Scales#wiki-ordinal_rangeRoundBands">https://github.com/mbostock/d3/wiki/Ordinal-Scales#wiki-ordinal_rangeRoundBands</a>
	 *
	 * Default: 0
	 *
	 * @return Returns the ratio of space between the edges of the plot and the first and last bars
	 */
	public Double getOuterPadding() {
		return outerPadding;
	}

	/**
	 * Gets the amount of padding between the first and last bars of the graph and the edges
	 * of the plot.
	 *
	 * The value is expressed as a ratio of the size of each bar in the graph.
	 *
	 * A value of 0.5 (50%) means that the space between the edges of the plot and the first
	 * and last bar will be half as wide as the bars of the graph.
	 *
	 * @see <a href="https://github.com/mbostock/d3/wiki/Ordinal-Scales#wiki-ordinal_rangeRoundBands">https://github.com/mbostock/d3/wiki/Ordinal-Scales#wiki-ordinal_rangeRoundBands</a>
	 *
	 * Default: 0
	 *
	 * @param outerPadding The ratio of space between the edges of the plot and the first and last bars
	 */
	public void setOuterPadding(Double outerPadding) {
		if( this.outerPadding != outerPadding ) {
			this.outerPadding = outerPadding;
			smartUpdate("outerPadding", this.outerPadding);
		}
	}

	/**
	 * Gets the map containing the data for the chart.
	 *
	 * The Map key is a String that represents the category for the data, and the value is a
	 * number that specifies the value for the category.
	 *
	 * @return Returns the map containing the data for the chart
	 */
	public Map<String, T> getSeries() {
		return series;
	}

	/**
	 * Sets the map containing the data for the chart.
	 *
	 * The Map key is a String that represents the category for the data, and the value is a
	 * number that specifies the value for the category.
	 *
	 * @param series The map containing the data for the chart
	 */
	public void setSeries(Map<String, T> series) {
		this.series = series;
		smartUpdate("series", this.series);
	}


	/**
	 * Gets whether or not the bars of the chart will be animated when the chart is
	 * displayed or resized.
	 *
	 * @return Returns true if the bars will be animated when the chart is displayed, false if not.
	 */
	public Boolean getAnimateBars() {
		return animateBars;
	}

	/**
	 * Sets whether or not the bars of the chart will be animated when the chart is
	 * displayed or resized.
	 *
	 * @param animateBars True if the bars will be animated when the chart is displayed, false if not.
	 */
	public void setAnimateBars(Boolean animateBars) {
		if( this.animateBars != animateBars ) {
			this.animateBars = animateBars;
			smartUpdate("animateBars", this.animateBars);
		}
	}

	/**
	 * Gets the delay between when the chart is first displayed and when the bar animation begins
	 * in milliseconds.
	 *
	 * This field has no effect if animateBars is set to false.
	 *
	 * @return Returns the delay for when the animation begins in milliseconds
	 */
	public Integer getAnimationDelay() {
		return animationDelay;
	}

	/**
	 * Sets the delay between when the chart is first displayed and when the bar animation begins
	 * in milliseconds.
	 *
	 * This field has no effect if animateBars is set to false.
	 *
	 * @param animationDelay The number of milliseconds to wait between when the chart is first displayed and when the animation begins
	 */
	public void setAnimationDelay(Integer animationDelay) {
		if( this.animationDelay != animationDelay ) { 
			this.animationDelay = animationDelay;
			smartUpdate("animationDelay", animationDelay);
		}
	}

	/**
	 * Gets the length of the bar animation in milliseconds.
	 *
	 * This field has no effect if animateBars is set to false.
	 *
	 * @return Returns the length of the bar animation in milliseconds.
	 */
	public Integer getAnimationLength() {
		return animationLength;
	}

	/**
	 * Sets the length of the bar animation in milliseconds.
	 *
	 * This field has no effect if animateBars is set to false.
	 *
	 * @param animationLength The length of the bar animation in milliseconds.
	 */
	public void setAnimationLength(Integer animationLength) {
		if( this.animationLength != animationLength ) {
			this.animationLength = animationLength;
			smartUpdate("animationLength", this.animationLength);
		}
	}

	/**
	 * Gets whether a value label for each bar is displayed as part of the chart.
	 *
	 * If animateBars is set to true, the graph will also wait for animationDelay + animationLength milliseconds before this value is displayed.
	 *
	 * @return Returns true if a per-bar value label is displayed, false if not.
	 */
	public Boolean getShowValueLabels() {
		return showValueLabels;
	}

	/**
	 * Sets whether a value label for each bar is displayed as part of the chart.
	 *
	 * If animateBars is set to true, the graph will also wait for animationDelay + animationLength milliseconds before this value is displayed.
	 *
	 * @param showValueLabels True if a per-bar value label is displayed, false if not.
	 */
	public void setShowValueLabels(Boolean showValueLabels) {
		if( this.showValueLabels != showValueLabels ) {
			this.showValueLabels = showValueLabels;
			smartUpdate("showValueLabels", this.showValueLabels);
		}
	}

	/**
	 * Gets the format applied to the value labels of the plot.
	 *
	 * The value is specified as a format string that the d3js library recognizes. This format
	 * is specific to the JS library that the graphs are based on.
	 *
	 * @see <a href="https://github.com/mbostock/d3/wiki/Formatting">https://github.com/mbostock/d3/wiki/Formatting</a>
	 *
	 * The default is an empty string, indicating that no format will be applied.
	 *
	 * This field has no effect if showValueLabels is false.
	 *
	 * @return Returns the format applied to the values of the plot
	 */
	public String getValueFormat() {
		return valueFormat;
	}

	/**
	 * Sets the format applied to the value labels of the plot.
	 *
	 * The value is specified as a format string that the d3js library recognizes. This format
	 * is specific to the JS library that the graphs are based on.
	 *
	 * @see <a href="https://github.com/mbostock/d3/wiki/Formatting">https://github.com/mbostock/d3/wiki/Formatting</a>
	 *
	 * The default is an empty string, indicating that no format will be applied.
	 *
	 * This field has no effect if showValueLabels is false.
	 *
	 * @param valueLabelFormat The format applied to the values of the plot
	 */
	public void setValueFormat(String valueFormat) {
		if( !this.valueFormat.equals(valueFormat) ) {
			this.valueFormat = valueFormat;
			smartUpdate("valueFormat", this.valueFormat);
		}
	}

	@Override
	protected void renderProperties(org.zkoss.zk.ui.sys.ContentRenderer renderer) throws java.io.IOException {

		super.renderProperties(renderer);

		render(renderer, "orient", orient);

		render(renderer, "innerPadding", innerPadding);
		render(renderer, "outerPadding", outerPadding);

		// Temporarily manually sanitize single quotes due to a bug in ZK's JSONValue.escape(...) method
		LinkedHashMap<String, T> newEntries = new LinkedHashMap<>();

		for( Entry<String, T> entry : series.entrySet() ) {

			String newKey = entry.getKey();

			newKey = JSONValue.toJSONString(newKey);
			newKey = newKey.substring(1, newKey.length()-1);

			if( newKey.contains("'") ) {
				newKey = newKey.replaceAll("[']", Matcher.quoteReplacement("\\'"));
			}

			newEntries.put(newKey, entry.getValue());
		}

		render(renderer, "series", newEntries);

		render(renderer, "animateBars", animateBars);
		render(renderer, "animationLength", animationLength);
		render(renderer, "animationDelay", animationDelay);

		render(renderer, "showValueLabels", showValueLabels);
		render(renderer, "valueFormat", valueFormat);
	}
}
