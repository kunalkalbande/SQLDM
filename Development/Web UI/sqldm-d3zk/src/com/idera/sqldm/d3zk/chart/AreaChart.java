package com.idera.sqldm.d3zk.chart;

public class AreaChart<T extends Number> extends XYChart<T> {

	private static final long serialVersionUID = 1L;
	
	protected Integer animationLength = 500;
	
	/**
	 * Gets the length of the area animation in milliseconds. 
	 * 
	 * This property has no effect if animateChart is false.
	 * 
	 * @return Returns the length of the area animation in milliseconds.
	 */
	public Integer getAnimationLength() {
		return animationLength;
	}
	
	/**
	 * Set the length of the area animation in milliseconds. 
	 * 
	 * This property has no effect if animateChart is false.
	 * 
	 * @param animationLength The length of the area animation in milliseconds.
	 */
	public void setAnimationLength(Integer animationLength) {
		if( this.animationLength != animationLength ) {
			this.animationLength = animationLength;
			smartUpdate("animationLength", this.animationLength);
		}
	}
	
	@Override
	protected void renderProperties(org.zkoss.zk.ui.sys.ContentRenderer renderer) throws java.io.IOException {
		super.renderProperties(renderer);
		render(renderer, "animationLength", animationLength);
	}
	
}
