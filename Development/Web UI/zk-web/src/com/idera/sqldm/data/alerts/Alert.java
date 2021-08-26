package com.idera.sqldm.data.alerts;

import java.util.ArrayList;
import java.util.Date;
import java.util.EnumSet;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.Map;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;
import com.idera.cwf.model.Product;
import com.idera.sqldm.i18n.SQLdmI18NStrings;

@JsonIgnoreProperties(ignoreUnknown = true)
public class Alert {

	public enum AlertSeverity {
		OK(1), INFORMATIONAL(2), WARNING(4), CRITICAL(8);
		private int id;

		AlertSeverity(int id) {
			this.id = id;
		}
		
		public int getId(){
			return id;
		}
		
		private static final Map<Integer, AlertSeverity> lookupById = new HashMap<>();

		static {
			for(AlertSeverity severity : EnumSet.allOf(AlertSeverity.class)) {
				lookupById.put(severity.getId(), severity);
			}
		}

		public static AlertSeverity findById(int input) {
			return lookupById.get(input);
		}
	}

	@JsonProperty("AlertId")
	public Long alertId;
	@JsonProperty("Heading")
	public String name;
	@JsonProperty("ServerName")
	public String instanceName;

	@JsonProperty("DatabaseName")
	public String databaseName;

	@JsonProperty("SQLServerId")
	public int instanceId;

	@JsonProperty("IsActive")
	public Integer isActive;
	
	@JsonProperty("UTCOccurrenceDateTime")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	public Date utcUpdated;

	@JsonProperty("Severity")
	public Integer severity;
	
	@JsonProperty("PreviousAlertSeverity")
	public Integer previousAlertSeverity;
	
	@JsonProperty("Message")
	public String description;

	/* @JsonProperty("activeDuration") */
	public long activeDuration;

	@JsonProperty("Metric")
	public Metrics Metric;

	@JsonProperty("Value")
	public Double value;

	@JsonProperty("StringValue")
	public String StringValue;
	@JsonProperty("StateEvent")
	public Integer StateEvent;
	
	//Used in Idera Dashboard widget
	public Product product;
	
	// @author Saumyadeep 
	// Friendly Begin
	
	@JsonProperty("FriendlyServerName")
	public String friendlyServerName;

	public String getFriendlyServerName() {
		return friendlyServerName;
	}
	
	public String displayName;
	
	public String getDisplayName() {
		if(this.getFriendlyServerName()!= null)	
			return this.getFriendlyServerName();
		else
			return this.getInstanceName();
	}

	// Friendly End
	
	
	public Long getAlertId() {
		return alertId;
	}

	public String getName() {
		return name;
	}

	public String StringValueStr() {
		if(StringValue==null)return "N/A";
		else if (StringValue.equals("-1")) {
			return "N/A";
		}
		return StringValue;
	}

	public String getInstanceName() {
		return instanceName;
	}

	public String getDatabaseName() {
		return databaseName;
	}

	public int getInstanceId() {
		return instanceId;
	}

	public Integer getIsActive() {
		return isActive;
	}

	public Date getUtcUpdated() {
		return utcUpdated;
	}

	public Integer getSeverity() {
		return severity;
	}

	public Integer getPreviousAlertSeverity() {
		return previousAlertSeverity;
	}

	public String getDescription() {
		return description;
	}

	public long getActiveDuration() {
		return activeDuration;
	}

	public Metrics getMetric() {
		return Metric;
	}

	public String getValueAsString() {
		return String.format("%,.2f", value);
	}

	public Double getValue() {
		return value;
	}

	public String getStringValue() {
		if(StringValue==null)return "N/A";
		return StringValue;
	}

	public Integer getStateEvent() {
		return StateEvent;
	}

	@Override
	public boolean equals(Object obj) {
		if (!(obj instanceof Alert)) {
			return false;
		}
		return this.alertId == ((Alert) obj).alertId;
	}

	public String getCriticalThresholdStr() {
		if(StringValue==null)return "N/A";
		else if (StringValue.equals("-1")) {
			return "N/A";
		}
		return Metric.getCriticalThreshold();
	}

	public String getWarningThresholdStr() {
		if(StringValue==null)return "N/A";
		else if (StringValue.equals("-1")) {
			return "N/A";
		}
		return Metric.getWarningThreshold();
	}

	public static HashMap<String, ArrayList<String>> getMap() {
		
		LinkedHashMap<String, ArrayList<String>> map = new LinkedHashMap<>();
		
		ArrayList<String> columnsList = new ArrayList<>();
		
		columnsList.add("getUtcUpdated");		
		map.put(SQLdmI18NStrings.ALERT_DATE, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getName");
		map.put(SQLdmI18NStrings.SUMMARY_CC, columnsList);

		columnsList = new ArrayList<>();
		//columnsList.add("getInstanceName"); @author Saumyadeep
		columnsList.add("getDisplayName");
		map.put(SQLdmI18NStrings.INSTANCE, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getDatabaseName");
		map.put(SQLdmI18NStrings.DATABASE, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getMetric");
		columnsList.add("getMetricCategory");
		map.put(SQLdmI18NStrings.SERVER_CATEGORY, columnsList);
		
		return map;
	}

	public static HashMap<String, ArrayList<String>> getMapDashboard() {
		
		LinkedHashMap<String, ArrayList<String>> map = new LinkedHashMap<>();
		
		ArrayList<String> columnsList = new ArrayList<>();
		
		columnsList.add("getUtcUpdated");		
		map.put(SQLdmI18NStrings.ALERT_DATE, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getName");
		map.put(SQLdmI18NStrings.SUMMARY_CC, columnsList);

		columnsList = new ArrayList<>();
		//columnsList.add("getInstanceName"); @author Saumyadeep
		columnsList.add("getDisplayName");
		map.put(SQLdmI18NStrings.INSTANCE, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getMetric");
		columnsList.add("getMetricCategory");
		map.put(SQLdmI18NStrings.SERVER_CATEGORY, columnsList);
		
		return map;
	}

	public Product getProduct() {
		return product;
	}

	public void setProduct(Product product) {
		this.product = product;
	}
}
