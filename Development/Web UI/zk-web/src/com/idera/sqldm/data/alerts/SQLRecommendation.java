package com.idera.sqldm.data.alerts;

import java.text.MessageFormat;

import javax.print.attribute.standard.Severity;

import org.apache.commons.lang.builder.EqualsBuilder;
import org.apache.commons.lang.builder.HashCodeBuilder;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class SQLRecommendation {

	@JsonProperty("ID")
	protected int id;

	@JsonProperty("Type")
	protected SQLActionItemType type;

	@JsonProperty("Metric")
	protected AlertMetrics metric;

	@JsonProperty("Severity")
	protected Severity severity;

	@JsonProperty("InstanceID")
	protected int instanceId;

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

	@JsonProperty("Value")
	protected Double value;

	@JsonProperty("ValueTranslated")
	protected String valueTranslated;

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
	
	public SQLRecommendation() {
		super();
	}

	public int getId() {
		return id;
	}

	public void setId(int id) {
		this.id = id;
	}

	public SQLActionItemType getType() {
		return type;
	}

	public void setCategory(SQLActionItemType type) {
		this.type = type;
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

	public int getInstanceId() {
		return instanceId;
	}

	public void setInstanceId(int instanceId) {
		this.instanceId = instanceId;
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

	public Double getValue() {
		return value;
	}

	public void setValue(Double value) {
		this.value = value;
	}

	public String getValueTranslated() {
		return valueTranslated;
	}

	public void setKnowledgeBase(String valueTranslated) {
		this.valueTranslated = valueTranslated;
	}

	public Object[] getSubstitutionValues() {
		return new Object[] {
				getValueTranslated(),
				instanceName != null ? instanceName : "",
				databaseName != null ? databaseName : "",
				resourceName != null ? resourceName : "",
				resourceType != null ? resourceType : ""
		};
	}

	public String formatMessage(String template) {
		if (template == null) return "";
		
		return MessageFormat.format(template, getSubstitutionValues());
	}
	
	 @Override
    public int hashCode() {
        return new HashCodeBuilder(17, 37)
            .append(type)
            .append(metric)
            .append(id)
            .toHashCode();
    }

    @Override
    public boolean equals(Object obj) {
        if( obj == null ) return false; // Check for null
        if( this == obj ) return true; // Check for identical references
        if( getClass() != obj.getClass() ) return false; // Check class
        SQLRecommendation rhs = (SQLRecommendation) obj;
       	return new EqualsBuilder()
       		.append(type, rhs.type)
           	.append(metric, rhs.metric)
           	.append(id, rhs.id)
           	.isEquals();
        
    }
}