package com.idera.sqldm.d3zk.chart;

import java.io.IOException;
import java.util.HashMap;
import java.util.Map;
import java.util.regex.Matcher;

import org.zkoss.json.JSONObject;
import org.zkoss.json.JSONValue;
import org.zkoss.zk.ui.sys.ContentRenderer;
import org.zkoss.zul.PieModel;
import org.zkoss.zul.SimplePieModel;

public class PieChart extends AbstractChart {

	static final long serialVersionUID = 1L;

	protected PieModel model = new SimplePieModel();

	protected Boolean showSliceLabels = false;

	protected Boolean animateSlices = true;
	protected Integer animationLength = 500;
	protected Integer animationDelay = 50;

	protected Integer innerRadiusPercentage = 0;
	protected Integer outerRadiusPercentage = 85;

	protected String valueFormat = "";

	protected String sortDirection = "asc";

	protected float labelThreshold = 0.05F;
	protected boolean mouseoverEnabled = true;

	protected Map<String, String> colorCodes = new HashMap<String, String>();

	/**
	 * Gets the PieModel containing the data for the chart.
	 *
	 * The model has a Comparable<?> key that should be unique for the category and a
	 * Number for the value.
	 *
	 * The PieModel is a ZK built-in object and the most common implementation is SimplePieModel.
	 *
	 * @return Returns the map containing the data for the chart
	 */
	public PieModel getModel() {
		return model;
	}

	/**
	 * Set the PieModel containing the data for the chart.
	 *
	 * The model has a Comparable<?> key that should be unique for the category and a
	 * Number for the value.
	 *
	 * The PieModel is a ZK built-in object and the most common implementation is SimplePieModel.
	 *
	 * @param model The PieModel to use for the chart data.
	 */
	public void setModel(PieModel model) {
		this.model = model;
	}

	/**
	 * Returns whether or not a label will be displayed on each individual slice of the pie chart.
	 *
	 * If set to true, then a label with either the key or the value for the slice will be rendered.
	 *
	 * The setSliceLabelType(...) method controls whether the key or the value is displayed.
	 *
	 * @return Returns true if the slice labels are shown, false otherwise.
	 */
	public Boolean getShowSliceLabels() {
		return showSliceLabels;
	}

	/**
	 * Sets whether or not a label will be displayed on each individual slice of the pie chart.
	 *
	 * If set to true, then a label with either the key or the value for the slice will be rendered.
	 *
	 * The setSliceLabelType(...) method controls whether the key or the value is displayed.
	 *
	 * @param showSliceLabels True if a label for each slice should be shown, false if not.
	 */
	public void setShowSliceLabels(Boolean showSliceLabels) {
		this.showSliceLabels = showSliceLabels;
	}

	/**
	 * Gets whether or not the slice of the chart will be animated when the chart is
	 * displayed or resized.
	 *
	 * @return Returns true if the slices will be animated when the chart is displayed, false if not.
	 */
	public Boolean getAnimateSlices() {
		return animateSlices;
	}

	/**
	 * Sets whether or not the slices of the chart will be animated when the chart is
	 * displayed or resized.
	 *
	 * @param animateSlices True if the slices will be animated when the chart is displayed, false if not.
	 */
	public void setAnimateSlices(Boolean animateSlices) {
		this.animateSlices = animateSlices;
	}

	/**
	 * Gets the delay between when the chart is first displayed and when the bar animation begins
	 * in milliseconds.
	 *
	 * This field has no effect if animateSlices is set to false.
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
	 * This field has no effect if animateSlices is set to false.
	 *
	 * @param animationDelay The number of milliseconds to wait between when the chart is first displayed and when the animation begins
	 */
	public void setAnimationDelay(Integer animationDelay) {
		this.animationDelay = animationDelay;
	}

	/**
	 * Gets the length of the bar animation in milliseconds.
	 *
	 * This field has no effect if animateSlices is set to false.
	 *
	 * @return Returns the length of the bar animation in milliseconds.
	 */
	public Integer getAnimationLength() {
		return animationLength;
	}

	/**
	 * Sets the length of the bar animation in milliseconds.
	 *
	 * This field has no effect if animateSlices is set to false.
	 *
	 * @param animationLength The length of the bar animation in milliseconds.
	 */
	public void setAnimationLength(Integer animationLength) {
		this.animationLength = animationLength;
	}

	/**
	 * Gets the inner radius percentage as an integer between 0 and 100.
	 *
	 * This value indicates whether or not the chart will be displayed as a ring instead of a pie.
	 *
	 * It is calculated as a percentage of the outer radius of the graph.
	 *
	 * If the graph has a 1000px radius, and the innerRadiusPercentage is set to 85, then the ring
	 * will be formed by removing an inner circle with a radius of 850px from the pie.
	 *
	 * @return Returns the inner radius percentage as an integer between 0 and 100
	 */
	public Integer getInnerRadiusPercentage() {
		return innerRadiusPercentage;
	}

	/**
	 * Sets the inner radius percentage as an integer between 0 and 100.
	 *
	 * This value indicates whether or not the chart will be displayed as a ring instead of a pie.
	 *
	 * It is calculated as a percentage of the outer radius of the graph.
	 *
	 * If the graph has a 1000px radius, and the innerRadiusPercentage is set to 85, then the ring
	 * will be formed by removing an inner circle with a radius of 850px from the pie.
	 *
	 * Specified values larger than 100 will set the percentage to 100, values less than 0 will set the percentage to 0.
	 *
	 * @param innerRadiusPercentage The inner radius percentage as an integer between 0 and 100
	 */
	public void setInnerRadiusPercentage(Integer innerRadiusPercentage) {
		if( innerRadiusPercentage > 100 ) innerRadiusPercentage = 100;
		if( innerRadiusPercentage < 0 ) innerRadiusPercentage = 0;
		this.innerRadiusPercentage = innerRadiusPercentage;
	}

	/**
	 * Gets the outer radius percentage as an integer between 0 and 100.
	 *
	 * This value indicates how much of the available space of the component the chart itself will take up.
	 *
	 * It is calculated based on the width or height of the component, whichever is smaller.
	 *
	 * If the component width is 1000, and the outerRadiusPercentage is set to 85, then the graph will be 850px wide.
	 *
	 * @return Returns the outer radius percentage as an integer between 0 and 100
	 */
	public Integer getOuterRadiusPercentage() {
		return outerRadiusPercentage;
	}

	/**
	 * Sets the outer radius percentage as an integer between 0 and 100.
	 *
	 * This value indicates how much of the available space of the component the chart itself will take up.
	 *
	 * It is calculated based on the width or height of the component, whichever is smaller. If the component width is 1000,
	 * and the outerRadiusPercentage is set to 85, then the graph will be 850px wide.
	 *
	 * Specified values larger than 100 will set the percentage to 100, values less than 0 will set the percentage to 0.
	 *
	 * @param outerRadiusPercentage The outer radius percentage as an integer between 0 and 100
	 */
	public void setOuterRadiusPercentage(Integer outerRadiusPercentage) {
		if( outerRadiusPercentage > 100 ) outerRadiusPercentage = 100;
		if( outerRadiusPercentage < 0 ) outerRadiusPercentage = 0;
		this.outerRadiusPercentage = outerRadiusPercentage;
	}

	/**
	 * Gets the format applied to the values of the plot.
	 *
	 * The value is specified as a format string that the d3js library recognizes. This format
	 * is specific to the JS library that the graphs are based on.
	 *
	 * @see <a href="https://github.com/mbostock/d3/wiki/Formatting">https://github.com/mbostock/d3/wiki/Formatting</a>
	 *
	 * The default is an empty string, indicating that no format will be applied.
	 *
	 * This field has no effect if showSliceLabels is false.
	 *
	 * @return Returns the format applied to the values of the plot
	 */
	public String getValueFormat() {
		return valueFormat;
	}

	/**
	 * Sets the format applied to the values of the plot.
	 *
	 * The value is specified as a format string that the d3js library recognizes. This format
	 * is specific to the JS library that the graphs are based on.
	 *
	 * @see <a href="https://github.com/mbostock/d3/wiki/Formatting">https://github.com/mbostock/d3/wiki/Formatting</a>
	 *
	 * The default is an empty string, indicating that no format will be applied.
	 *
	 * This field has no effect if showSliceLabels is false.
	 *
	 * @param valueFormat The format to apply to the values of the plot
	 */
	public void setValueFormat(String valueFormat) {
		this.valueFormat = valueFormat;
	}

	/**
	 * Gets the sort order for the slices in the graph.
	 *
	 * The slices are ordered clockwise starting at the top of the graph.
	 *
	 * @return Returns "asc" if the slices are ordered in ascending order or "dsc" if the slices are ordered in descending order
	 */
	public String getSortDirection() {
		return sortDirection;
	}

	/**
	 * Sets the sort order for the slices in the graph.
	 *
	 * The slices are ordered clockwise starting at the top of the graph.
	 *
	 * @param sortDirection "asc" if the slices should be ordered in ascending order or "dsc" if the slices should be ordered in descending order
	 */
	public void setSortDirection(String sortDirection) {
		this.sortDirection = sortDirection;
	}

	/**
	 * Gets the minimum percentage of the pie that a slice must be to display the slice label.
	 *
	 * Defaults to 0.05 (5%)
	 *
	 * @return Returns the minimum percentage required to show the slice label
	 */
	public float getLabelThreshold() {
		return labelThreshold;
	}

	/**
	 * Gets the minimum percentage of the pie that a slice must be to display the slice label.
	 *
	 * Defaults to 0.05 (5%)
	 *
	 * @param labelThreshold The minimum percentage required to show the slice label
	 */
	public void setLabelThreshold(float labelThreshold) {
		this.labelThreshold = labelThreshold;
	}

	/**
	 * Gets whether or not mouseover events are enabled for the chart slices.
	 *
	 * @return True if mouseover events are enabled, false if not
	 */
	public boolean isMouseoverEnabled() {
		return mouseoverEnabled;
	}

	/**
	 * Sets whether or not mouseover events are enabled for the chart slices.
	 *
	 * @param mouseoverEnabled True if mouseover events should be enabled, false if not
	 */
	public void setMouseoverEnabled(boolean mouseoverEnabled) {
		this.mouseoverEnabled = mouseoverEnabled;
	}

	@Override
	protected void renderProperties(ContentRenderer renderer) throws IOException {
		super.renderProperties(renderer);
		renderer.render("model", toJSON(model));
		renderer.render("showSliceLabels", showSliceLabels);
		renderer.render("animateSlices", animateSlices);
		renderer.render("animationLength", animationLength);
		renderer.render("animationDelay", animationDelay);
		renderer.render("innerRadiusPercentage", innerRadiusPercentage);
		renderer.render("outerRadiusPercentage", outerRadiusPercentage);
		renderer.render("valueFormat", valueFormat);
		renderer.render("sortDirection", sortDirection);
		renderer.render("labelThreshold", labelThreshold);
		renderer.render("mouseoverEnabled", mouseoverEnabled);
		render(renderer, "colorCodes", colorCodes);
	}

	protected static JSONObject toJSON(PieModel model) {

		JSONObject json = new JSONObject();

		for( Comparable<?> category : model.getCategories() ) {

			String newKey = String.valueOf(category);

			newKey = JSONValue.toJSONString(newKey);
			newKey = newKey.substring(1, newKey.length()-1);
			newKey = newKey.replaceAll("[']", Matcher.quoteReplacement("\\'"));

			json.put(newKey, model.getValue(category));
		}

		return json;
	}

	/**
	 * Get the map containing color codes for all labels
	 * 
	 * @return Returns the map containing color codes for all labels
	 */
	public Map<String, String> getColorCodes() {
		return colorCodes;
	}

	/**
	 * Sets the value of map containing color codes
	 * 
	 * @param colorCodes Map of all color codes
	 */
	public void setColorCodes(Map<String, String> colorCodes) {
		this.colorCodes = colorCodes;
	}

}
