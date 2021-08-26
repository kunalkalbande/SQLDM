package com.idera.sqldm.d3zk.chart;

import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.Map;
import java.util.regex.Matcher;

import org.zkoss.json.JSONArray;
import org.zkoss.json.JSONObject;
import org.zkoss.json.JSONValue;

public class StackedBarChart<T extends Number> extends RectangularChart {

	static final long serialVersionUID = 1L;

	protected String orient = "vertical";

	protected Double innerPadding = 0D;
	protected Double outerPadding = 0D;

	protected Boolean showValueLabels = true;
	protected String valueFormat = "";

	protected Map<String, Map<String,T>> series = new HashMap<String, Map<String,T>>();
	
	protected Map<String,String> legendLinks = new HashMap<String,String>();
	protected Map<String,String> axisLinks = new HashMap<String,String>();
	protected String barLinks;

	protected Boolean animateBars = true;
	protected Integer animationDelay = 50;
	protected Integer animationLength = 500;
	protected String xAxisTitle = null;
	protected Integer xAxisTicksCount = 0;
	protected String mouseOverText = null;
	protected int xAxisTickPadding = 3;
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
	
	public int getXAxisTickPadding() {
		return xAxisTickPadding;
	}

	public void setXAxisTickPadding(int xAxisTickPadding) {
		this.xAxisTickPadding = xAxisTickPadding;
	}
	
	/**
	 * @return Returns a string indicating the text to be shown on mouseover 
	 */
	public String getMouseOverText() {
		return mouseOverText;
	}
	
	/**
	 * Sets the text to be shown on mouseover event
	 * 
	 * @param mouseOverText
	 */
	public void setMouseOverText(String mouseOverText) {
		if(this.mouseOverText != mouseOverText ) {
			this.mouseOverText = mouseOverText;
			smartUpdate("mouseOverText", this.mouseOverText);
		}
	}

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
	 * @param orient string indicating the orientation of the bar chart
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
	public Map<String, Map<String,T>> getSeries() {
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
	public void setSeries(Map<String, Map<String,T>> series) {
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
	 * @param valueFormat The format applied to the values of the plot
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
//TODO
	/*	for( Entry<String, T> entry : series.entrySet() ) {

			String newKey = entry.getKey();

			newKey = JSONValue.toJSONString(newKey);
			newKey = newKey.substring(1, newKey.length()-1);

			if( newKey.contains("'") ) {
				newKey = newKey.replaceAll("[']", Matcher.quoteReplacement("\\'"));
			}

			newEntries.put(newKey, entry.getValue());
		}
*/
		render(renderer, "series", toJSON(series));

		render(renderer, "animateBars", animateBars);
		render(renderer, "animationLength", animationLength);
		render(renderer, "animationDelay", animationDelay);

		render(renderer, "showValueLabels", showValueLabels);
		render(renderer, "valueFormat", valueFormat);
		render(renderer, "xAxisTitle", xAxisTitle);
		render(renderer, "xAxisTicksCount", xAxisTicksCount);
		render(renderer,"mouseOverText",mouseOverText);
		render(renderer,"axisLinks",axisLinks);
		render(renderer,"legendLinks",legendLinks);
		render(renderer,"barLinks",barLinks);
		render(renderer,"xAxisTickPadding",xAxisTickPadding);
		render(renderer,"drillable",this.drillable);
	}

    protected JSONObject toJSON(Map<String, Map<String,T>> model) {

        JSONArray categoryJSON = new JSONArray();

        JSONObject json = new JSONObject();

        for(String key:model.keySet()) {
            Map<String,T> categories =model.get(key);

            JSONObject obj = new JSONObject();
            obj.put("series",key);

            for(String categoryKey:categories.keySet()){
                T categoryValue=categories.get(categoryKey);
                if( categoryValue == null ) continue;

                String sanitizedSeries = String.valueOf(categoryKey);

                sanitizedSeries = JSONValue.toJSONString(sanitizedSeries);
                sanitizedSeries = sanitizedSeries.substring(1, sanitizedSeries.length()-1);
                sanitizedSeries = sanitizedSeries.replaceAll("[']", Matcher.quoteReplacement("\\'"));
                sanitizedSeries = sanitizedSeries.replaceAll("[\\\\]", Matcher.quoteReplacement(""));
                obj.put(sanitizedSeries,categoryValue );
            }
            categoryJSON.add(obj);

        }
        json.put("data", categoryJSON);
        return json;
    }
    
    /**
	 * Gets the label used for the title of the X axis on the plot area.
	 *
	 * @return Returns the label used for the title of the X axis on the plot area.
	 */
	public String getXAxisTitle() {
		return xAxisTitle;
	}

	/**
	 * Sets the label used for the title of the X axis on the plot area.
	 *
	 * @param xAxisTitle The label to use for the title of the X axis on the plot area.
	 */
	public void setXAxisTitle(String xAxisTitle) {
		if( this.xAxisTitle != xAxisTitle ) {
			this.xAxisTitle = xAxisTitle;
			smartUpdate("xAxisTitle", this.xAxisTitle);
		}
	}
	
	/**
	 * Sets Legends links to be called on mouse click of legends. The value in the map has to be valid javascript function along with the scope.
	 * 
	 * @param legendLinks The map consisting of link to be called
	 */
	public void setLegendLinks(Map<String, String> legendLinks) {
		this.legendLinks = legendLinks;
	}
	
	/**
	 * Gets the map containing legend links
	 * 
	 * @return Returns the map containing legend links
	 */
	public Map<String, String> getLegendLinks() {
		return legendLinks;
	}
	
	/**
	 * Sets axis links to be called on mouse click of axis labels
	 * 
	 * @param axisLinks The map consisting of link to be called
	 */
	public void setAxisLinks(Map<String, String> axisLinks) {
		this.axisLinks = axisLinks;
	}
	
	/**
	 * Gets the map containing axis label links
	 * 
	 * @return Returns the map containing axis label links
	 */
	public Map<String, String> getAxisLinks() {
		return axisLinks;
	}
	
	/**
	 * Sets bar links to be called on mouse click of chart bars
	 * 
	 * @param barLinks
	 */
	public void setBarLinks(String barLinks) {
		this.barLinks = barLinks;
	}
	
	/**
	 * Gets the bar link
	 * 
	 * @return Returns the bar link 
	 */
	public String getBarLinks() {
		return barLinks;
	}

	/**
	 * Gets the count of visible labels for axis ticks
	 * 
	 * @return Returns the map containing axis label links
	 */
	public Integer getxAxisTicksCount() {
		return xAxisTicksCount;
	}

	/**
	 * Sets the count of visible labels for axis ticks
	 * 
	 * @param barLinks
	 */
	public void setxAxisTicksCount(Integer xAxisTicksCount) {
		this.xAxisTicksCount = xAxisTicksCount;
	}
	
	
}
