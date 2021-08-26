package com.idera.sqldm.d3zk.chart;

import java.io.IOException;
import java.util.HashMap;
import java.util.Map;

import org.zkoss.json.JSONArray;

public class HeatmapChart<T extends Number> extends RectangularChart {
	static final long serialVersionUID = 1L;
	  protected JSONArray data = new JSONArray();
	  protected String orient = "vertical";
	  protected Boolean animateBars = true;
	  protected Integer animationDelay = 50;
	  protected Integer animationLength = 500;
	  protected Boolean hide = false;
	  
	  public JSONArray getData()
	  {
	    return this.data;
	  }
	  
	  public void setData(JSONArray value)
	  {
	      this.data = value;
	      smartUpdate("data", this.data);
	  }
	  
	  public void setHide(){
		  smartUpdate("hide", this.hide);
	  }
	  
	  
	  public String getOrient() {
		  return this.orient;
	  }
	  
	  public void setOrient(String value) {
		  if(this.orient != value){
			  this.orient = value;
			  smartUpdate("orient",this.orient);
		  }
	  }
	  public Boolean getAnimateBars() {
		  return this.animateBars;
	  }
	  
	  public void setAnimateBars(Boolean value) {
		  if(this.animateBars != value){
			  this.animateBars = value;
			  smartUpdate("animateBars",this.animateBars);
		  }
	  }
	  public Integer getAnimationDelay() {
		  return this.animationDelay;
	  }
	  
	  public void setAnimationDelay(Integer value) {
		  if(this.animationDelay != value){
			  this.animationDelay = value;
			  smartUpdate("animationDelay",this.animationDelay);
		  }
	  }
	  
	  public Integer getAnimationLength() {
		  return this.animationLength;
	  }
	  
	  public void setAnimationLength(Integer value) {
		  if(this.animationLength != value){
			  this.animationLength = value;
			  smartUpdate("animationLength",this.animationLength);
		  }
	  }
	  @Override
	  protected void renderProperties(org.zkoss.zk.ui.sys.ContentRenderer renderer) throws IOException
	  {
	    super.renderProperties(renderer);
	    render(renderer, "data", this.data);
	    render(renderer, "orient", this.orient);
	    render(renderer, "animateBars", this.animateBars);
	    render(renderer, "animationDelay", this.animationDelay);
	    render(renderer, "animationLength", this.animationLength);
	    render(renderer, "hide", this.hide);
	  }
}
