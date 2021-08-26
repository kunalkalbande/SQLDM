package com.idera.sqldm.d3zk.chart;

import java.util.Map;

public class LineChart<T extends Number> extends XYChart<T> {

	private static final long serialVersionUID = 1L;
	
	protected Integer animationStepSize = 1;
	protected Boolean enableMouseOverText = false;
	protected Map<String,String> customColorMap = null;
	
	/**
	 * Gets how many points of the dataset to append in each step of the animation. 
	 * 
	 * For example, if there are 100 data points, and the step size is set to 5, then each step of
	 * the animation will display 5 additional points for each path. 
	 *  
	 * @return Returns the number of points that will be appended during each step of the animation.
	 */
	public Integer getAnimationStepSize() {
		return animationStepSize;
	}

	/**
	 * Gets how many points of the dataset to append in each step of the animation. 
	 * 
	 * For example, if there are 100 data points, and the step size is set to 5, then each step of
	 * the animation will display 5 additional points for each path.
	 * 
	 * @param animationStepSize The number of points that will be appended during each step of the animation.
	 */
	public void setAnimationStepSize(Integer animationStepSize) {
		if( this.animationStepSize != animationStepSize ) {
			this.animationStepSize = animationStepSize;
			smartUpdate("animationStepSize", this.animationStepSize);
		}
	}
	
	public Boolean getEnableMouseOverText() {
		return this.enableMouseOverText;
	}
	
	public void setEnableMouseOverText(Boolean enableMouseOverText) {
		if(this.enableMouseOverText != enableMouseOverText) {
			this.enableMouseOverText = enableMouseOverText;
			smartUpdate("enableMouseOverText", this.enableMouseOverText);
		}
	}
	
	public Map<String,String> getCustomColorMap() {
		return this.customColorMap;
	}
	
	public void setCustomColorMap(Map<String,String> customColorMap){
		if(customColorMap != null) {
			this.customColorMap = customColorMap;
			smartUpdate("customColorMap",this.customColorMap);
		}
	}
	
	@Override
	protected void renderProperties(org.zkoss.zk.ui.sys.ContentRenderer renderer) throws java.io.IOException {
		super.renderProperties(renderer);
		render(renderer, "animationStepSize", animationStepSize);
		render(renderer, "enableMouseOverText", enableMouseOverText);
		render(renderer, "customColorMap",customColorMap);
	}
	

}
