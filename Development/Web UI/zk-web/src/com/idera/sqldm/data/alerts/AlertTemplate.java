package com.idera.sqldm.data.alerts;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class AlertTemplate {

	@JsonProperty("ID")
	protected int id;
	
	@JsonProperty("Plural")
	protected String plural;

	@JsonProperty("PluralPreview")
	protected String pluralPreview;
	
	@JsonProperty("Title")
	protected String title;
	
	@JsonProperty("Preview")
	protected String preview;
	
	@JsonProperty("KnowledgeBase")
	protected String knowledgeBase;

	public AlertTemplate() { 
		super();
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public String getPlural() {
		return plural;
	}

	public void setPlural(String plural) {
		this.plural = plural;
	}

	public String getPluralPreview() {
		return pluralPreview;
	}

	public void setPluralPreview(String pluralPreview) {
		this.pluralPreview = pluralPreview;
	}
	
	public String getTitle() {
		return title;
	}

	public void setTitle(String title) {
		this.title = title;
	}

	public String getPreview() {
		return preview;
	}

	public void setPreview(String preview) {
		this.preview = preview;
	}

	public String getKnowledgeBase() {
		return knowledgeBase;
	}

	public void setKnowledgeBase(String knowledgeBase) {
		this.knowledgeBase = knowledgeBase;
	}
	
}
