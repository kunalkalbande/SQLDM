package com.idera.sqldm.data.alerts;

import java.util.ArrayList;
import java.util.List;

import javax.print.attribute.standard.Severity;

import org.apache.commons.lang.builder.EqualsBuilder;
import org.apache.commons.lang.builder.HashCodeBuilder;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class SQLInstanceRecommendation {

	@JsonProperty("ID")
	protected int id;

	@JsonProperty("Category")
	protected String category;

	@JsonProperty("Metric")
	protected AlertMetrics metric;

	@JsonProperty("Severity")
	protected Severity severity;

	@JsonProperty("UnifiedInstanceID")
	protected int unifiedInstanceId;

	@JsonProperty("InstanceName")
	protected String instanceName;

	@JsonProperty("DatabaseId")
	protected int databaseId;

	@JsonProperty("DatabaseName")
	protected String databaseName;

	@JsonProperty("ResourceId")
	protected int resourceId;

	@JsonProperty("ResourceType")
	protected String resourceType;

	@JsonProperty("ResourceName")
	protected String resourceName;

	@JsonProperty("Title")
	protected String title;

	@JsonProperty("Preview")
	protected String preview;

	@JsonProperty("KB")
	protected String knowledgeBase;
	
	// @author Saumyadeep 
	// Friendly Begin
	
	@JsonProperty("FriendlyServerName")
	protected String friendlyServerName;

	public void setFriendlyServerName(String friendlyServerName) {
		this.friendlyServerName = friendlyServerName;
	}
	
	public String getFriendlyServerName() {
		return this.friendlyServerName;
	}

	protected String displayName;
	
	public String getDisplayName() {
		if(this.getFriendlyServerName()!= null)	
			return this.getFriendlyServerName();
		else
			return this.getInstanceName();
	}

	public void setDisplayName(String displayName) {
		this.displayName = displayName;
	}
	// Friendly End
	

	public SQLInstanceRecommendation() {
		super();
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public String getCategory() {
		return category;
	}

	public void setCategory(String category) {
		this.category = category;
	}

	public AlertMetrics getMetric() {
		return metric;
	}

	public void setMetric(AlertMetrics metric) {
		this.metric = metric;
	}

	public Severity getSeverity() {
		return severity;
	}

	public void setSeverity(Severity severity) {
		this.severity = severity;
	}

	public int getUnifiedInstanceId() {
		return unifiedInstanceId;
	}

	public void setUnifiedInstanceId(int unifiedInstanceId) {
		this.unifiedInstanceId = unifiedInstanceId;
	}

	public String getInstanceName() {
		return instanceName;
	}

	public void setInstanceName(String instanceName) {
		this.instanceName = instanceName;
	}

	public int getDatabaseId() {
		return databaseId;
	}

	public void setDatabaseId(int databaseId) {
		this.databaseId = databaseId;
	}

	public String getDatabaseName() {
		return databaseName;
	}

	public void setDatabaseName(String databaseName) {
		this.databaseName = databaseName;
	}

	public int getResourceId() {
		return resourceId;
	}

	public void setResourceId(int resourceId) {
		this.resourceId = resourceId;
	}

	public String getResourceType() {
		return resourceType;
	}

	public void setResourceType(String resourceType) {
		this.resourceType = resourceType;
	}

	public String getResourceName() {
		return resourceName;
	}

	public void setResourceName(String resourceName) {
		this.resourceName = resourceName;
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

	public List<Object> getSubstitutionValues() {
		List<Object> values = new ArrayList<Object>();
		values.add(0D);
		values.add(instanceName != null ? instanceName : "");
		values.add(databaseName != null ? databaseName : "");
		values.add(resourceName != null ? resourceName : "");
		values.add(resourceType != null ? resourceType : "");
		return values;
	}

	 @Override
    public int hashCode() {
        return new HashCodeBuilder(17, 37)
            .append(id)
            .toHashCode();
    }

    @Override
    public boolean equals(Object obj) {
        if( obj == null ) return false; // Check for null
        if( this == obj ) return true; // Check for identical references
        if( getClass() != obj.getClass() ) return false; // Check class
        SQLInstanceRecommendation rhs = (SQLInstanceRecommendation) obj;
        return new EqualsBuilder()
            .append(this.id, rhs.id)
            .isEquals();
    }
}
