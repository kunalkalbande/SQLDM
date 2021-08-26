/////-----------Auth:Rishabh Mishra---------------
package com.idera.sqldm.ui.dashboard;

import com.fasterxml.jackson.annotation.JsonProperty;

public class TagsScaleFactorList {
	@JsonProperty("TagHealthScaleFactor")
	private Double TagHealthScaleFactor;
	@JsonProperty("TagId")
	private int TagId;
	@JsonProperty("TagName")
	private String TagName;
	@JsonProperty("IsTagHealthScaleFactorSet")
	private boolean IsTagHealthScaleFactorSet;
				
	public boolean isIsTagHealthScaleFactorSet() {
		return IsTagHealthScaleFactorSet;
	}

	public void setIsTagHealthScaleFactorSet(boolean isTagHealthScaleFactorSet) {
		IsTagHealthScaleFactorSet = isTagHealthScaleFactorSet;
	}

	public TagsScaleFactorList() {
	}

	public Double getTagHealthScaleFactor() {
		return TagHealthScaleFactor;
	}

	public void setTagHealthScaleFactor(Double tagHealthScaleFactor) {
		TagHealthScaleFactor = tagHealthScaleFactor;
	}

	public int getTagId() {
		return TagId;
	}

	public void setTagId(int tagId) {
		TagId = tagId;
	}

	public String getTagName() {
		return TagName;
	}

	public void setTagName(String tagName) {
		TagName = tagName;
	}
	/*@Override
	public boolean equals(Object obj) {
		if(obj instanceof TagsScaleFactorList)
		if(this.TagName.equals(((TagsScaleFactorList) obj).getTagName()))
				return true;
		return false;
	}*/
	@Override
	public boolean equals(Object obj) {
		if(obj instanceof TagsScaleFactorList)
		if(this.TagId==(((TagsScaleFactorList) obj).TagId))
				return true;
		return false;
	}
}
