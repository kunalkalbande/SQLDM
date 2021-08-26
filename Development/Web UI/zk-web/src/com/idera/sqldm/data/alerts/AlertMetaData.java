package com.idera.sqldm.data.alerts;

import java.util.ArrayList;
import java.util.List;

import org.apache.commons.lang.builder.EqualsBuilder;
import org.apache.commons.lang.builder.HashCodeBuilder;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class AlertMetaData {

	@JsonProperty("ID")
	protected int id;

	@JsonProperty("Name")
	protected String name;

	@JsonProperty("Category")
	protected String category;

	@JsonProperty("Description")
	protected String description;

	@JsonProperty("IsCriticalSupported")
	protected boolean criticalSupported;

	@JsonProperty("IsWarningSupported")
	protected boolean warningSupported;

	@JsonProperty("IsInformationalSupported")
	protected boolean informationalSupported;

	@JsonProperty("IsMoreBetter")
	protected boolean moreBetter;

	@JsonProperty("ValueType")
	protected String valueType;

	@JsonProperty("DisplayFormat")
	protected String displayFormat;

	@JsonProperty("Rank")
	protected int rank;

	@JsonProperty("Template")
	protected AlertTemplate alertTemplate;

	@JsonProperty("Alertable")
	protected boolean alertable;

	@JsonProperty("Enabled")
	protected boolean enabled;

	@JsonProperty("Exclusive")
	protected boolean exclusive;
	
	@JsonProperty("Links")
	protected List<Link> links;	

	public AlertMetaData() {
		super();
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public String getCategory() {
		return category;
	}

	public void setCategory(String category) {
		this.category = category;
	}

	public String getDescription() {
		return description;
	}

	public void setDescription(String description) {
		this.description = description;
	}

	public boolean isCriticalSupported() {
		return criticalSupported;
	}

	public void setCriticalSupported(boolean criticalSupported) {
		this.criticalSupported = criticalSupported;
	}

	public boolean isWarningSupported() {
		return warningSupported;
	}

	public void setWarningSupported(boolean warningSupported) {
		this.warningSupported = warningSupported;
	}

	public boolean isInformationalSupported() {
		return informationalSupported;
	}

	public void setInformationalSupported(boolean informationalSupported) {
		this.informationalSupported = informationalSupported;
	}

	public boolean isMoreBetter() {
		return moreBetter;
	}

	public void setMoreBetter(boolean moreBetter) {
		this.moreBetter = moreBetter;
	}

	public String getValueType() {
		return valueType;
	}

	public void setValueType(String valueType) {
		this.valueType = valueType;
	}

	public String getDisplayFormat() {
		return displayFormat;
	}

	public void setDisplayFormat(String displayFormat) {
		this.displayFormat = displayFormat;
	}

	public int getRank() {
		return rank;
	}

	public void setRank(int rank) {
		this.rank = rank;
	}

	public AlertTemplate getAlertTemplate() {
		return alertTemplate;
	}

	public void setAlertTemplate(AlertTemplate alertTemplate) {
		this.alertTemplate = alertTemplate;
	}

	public boolean isAlertable() {
		return alertable;
	}

	public void setAlertable(boolean alertable) {
		this.alertable = alertable;
	}

	public boolean isEnabled() {
		return enabled;
	}

	public void setEnabled(boolean enabled) {
		this.enabled = enabled;
	}

	public boolean isExclusive() {
		return exclusive;
	}

	public void setExclusive(boolean exclusive) {
		this.exclusive = exclusive;
	}
	
	public void setLinks(List<Link> links) {
		this.links = links;
	}
	
	public List<Link> getLinks() {
		if (links == null)
			links = new ArrayList<Link>();
		return links;
	}

	@Override
	public boolean equals(Object obj) {

		if( obj == null ) return false;
		if( obj == this ) return true;
		if( !(obj instanceof AlertMetaData) ) return false;

		AlertMetaData rhs = (AlertMetaData) obj;

		return new EqualsBuilder().append(this.getId(), rhs.getId()).isEquals();
	}

	@Override
	public int hashCode() {
		return new HashCodeBuilder(17, 31).append(getId()).toHashCode();
	}

}
