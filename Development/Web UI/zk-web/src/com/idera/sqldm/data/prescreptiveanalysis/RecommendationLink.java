package com.idera.sqldm.data.prescreptiveanalysis;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

public class RecommendationLink
{
    @JsonProperty("Link")
    private String link;
    
    @JsonProperty("Title")
	private Object title;
	
    @JsonProperty("Condition")
    private String condition;
    
    @JsonProperty("CondensedLink")
	private Object condensedLink;
	
	public void setLink(String link) {
		this.link = link;
	}

	public String getLink() {
		return link;
	}

	public void setTitle(Object title) {
		this.title = title;
	}

	public Object getTitle() {
        return title;
	}
	
	public void setCondition(String condition) {
		this.condition = condition;
	}

	public String getCondition() {
		return condition;
	}

	public void setCondensedLink(Object condensedLink) {
		this.condensedLink = condensedLink;
	}

	public Object getCondensedLink() {
        return condensedLink;
	}

	@Override
	public String toString() {
		return "RecommendationLink => {link=" + link + ", title=" + title + "}";
	}
}