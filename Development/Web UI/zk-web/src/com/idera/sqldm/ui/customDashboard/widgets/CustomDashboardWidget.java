package com.idera.sqldm.ui.customDashboard.widgets;

import java.awt.Font;
import java.awt.font.FontRenderContext;
import java.awt.geom.AffineTransform;
import java.util.Map;

import org.zkoss.zk.ui.IdSpace;
import org.zkoss.zul.Panel;

public class CustomDashboardWidget extends Panel 
implements IdSpace {

	private static final long serialVersionUID = 111111L;
	private Map<String,String> widgetConfig;
	
	public void setWidgetConfig(Map<String,String> widgetConfig) {
		this.widgetConfig = widgetConfig;
		applyConfiguration();
	}
	
	public void applyConfiguration() {
		setBorder();
		setWidth();
		setHeight();
		setTitle();
	}
	
	public void setTitle(){
		String title = "";
		if(this.widgetConfig.containsKey("title")) {
			title = this.widgetConfig.get("title");
		}
		getCaption().setTooltiptext(title);
		super.setTitle(title);
	}
	
	public void setBorder() {
		String border = "normal";
		if(this.widgetConfig.containsKey("border")) {
			 border = this.widgetConfig.get("border");
		}
		super.setBorder(border);
	}
	
	public void setWidth() {
		String width = CustomDashboardWidgetSizeEnum.medium.getWidth();
		if(this.widgetConfig.containsKey("width")) {
			width = this.widgetConfig.get("width");
		}
		super.setWidth(width);
	}
	
	public void setHeight() {
		String height = CustomDashboardWidgetSizeEnum.medium.getHeight();
		if(this.widgetConfig.containsKey("height")){
			height = this.widgetConfig.get("height");
		}
		
		super.setHeight(height);
	}

}