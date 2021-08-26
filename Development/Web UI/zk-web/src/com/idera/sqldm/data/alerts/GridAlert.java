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
public class GridAlert {

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
	protected Long alertId;
	@JsonProperty("Heading")
	protected String name;
	@JsonProperty("ServerName")
	protected String instanceName;

	@JsonProperty("DatabaseName")
	protected String databaseName;

	@JsonProperty("SQLServerId")
	private int instanceId;

	@JsonProperty("IsActive")
	protected Integer isActive;
	
	@JsonProperty("UTCOccurrenceDateTime")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	protected Date utcUpdated;

	@JsonProperty("Severity")
	protected String severity;
	
	@JsonProperty("PreviousAlertSeverity")
	protected Integer previousAlertSeverity;
	
	@JsonProperty("Message")
	protected String description;

	/* @JsonProperty("activeDuration") */
	protected long activeDuration;

	@JsonProperty("Metric")
	private Metrics Metric;

	@JsonProperty("Value")
	protected Double value;

	@JsonProperty("StringValue")
	protected String StringValue;
	@JsonProperty("StateEvent")
	private Integer StateEvent;
	
	//Used in Idera Dashboard widget
	private Product product;
	
	// @author Saumyadeep 
	// Friendly Begin
	
	@JsonProperty("FriendlyServerName")
	protected String friendlyServerName;

	public String getFriendlyServerName() {
		return friendlyServerName;
	}
	
	protected String displayName;
	
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

	public String getSeverity() {
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
		if (!(obj instanceof GridAlert)) {
			return false;
		}
		return this.alertId == ((GridAlert) obj).alertId;
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
