package com.idera.sqldm.d3zk.chart;

import org.zkoss.zul.Div;

public abstract class AbstractChart extends Div {

	private static final long serialVersionUID = 1L;

	protected String colorPalette = "category10";
	
	/** 
	 * Gets the color palette used to pick the colors for the series lines and labels. These color 
	 * palettes are defined by the d3js library. 
	 * 
	 * Possible palettes are: category10, category20, category20b, and category20c.
	 * 
	 * @see <a href="https://github.com/mbostock/d3/wiki/Ordinal-Scales#categorical-colors">https://github.com/mbostock/d3/wiki/Ordinal-Scales#categorical-colors</a>
	 * 
	 * In addition, the ColorBrewer scales are also supported:
	 * @see <a href="https://github.com/mbostock/d3/wiki/Ordinal-Scales#colorbrewer">https://github.com/mbostock/d3/wiki/Ordinal-Scales#colorbrewer</a>
	 * 
	 * The default palette is category10.
	 * 
	 * @return Returns a string indicating which color palette has been selected for the chart.
	 */
	public String getColorPalette() {
		return colorPalette;
	}

	/**
	 * Sets the color palette used to pick the colors for the series lines and labels. These color 
	 * palettes are defined by the d3js library. 
	 * 
	 * Possible palettes are: category10, category20, category20b, and category20c.
	 * 
	 * @see <a href="https://github.com/mbostock/d3/wiki/Ordinal-Scales#categorical-colors">https://github.com/mbostock/d3/wiki/Ordinal-Scales#categorical-colors</a>
	 * 
	 * The default palette is category10.
	 * 
	 * In addition, the ColorBrewer scales are also supported:
	 * @see <a href="https://github.com/mbostock/d3/wiki/Ordinal-Scales#colorbrewer">https://github.com/mbostock/d3/wiki/Ordinal-Scales#colorbrewer</a>
	 *
	 * @param colorPalette A string indicating which color palette has been selected for the chart.
	 */
	public void setColorPalette(String colorPalette) {
		if( !this.colorPalette.equals(colorPalette) ) { 
			this.colorPalette = colorPalette;
			smartUpdate("colorPalette", this.colorPalette);
		}
	}
	
	@Override
	protected void renderProperties(org.zkoss.zk.ui.sys.ContentRenderer renderer) throws java.io.IOException {
		super.renderProperties(renderer);
		render(renderer, "colorPalette", colorPalette);
	}
	
}
