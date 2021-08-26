package com.idera.sqldm.data.customdashboard;


import java.util.List;

import com.fasterxml.jackson.annotation.JsonProperty;

public class CustomDashboardWidget {
		
		@JsonProperty("Match")
		private int match;
		
		@JsonProperty("MetricID")
		private int metricID;
		
		@JsonProperty("TagId")
		private List<Integer> tagId;
		
		@JsonProperty("WidgetID")
		private int widgetID;
		
		@JsonProperty("WidgetName")
		private String widgetName;
		
		@JsonProperty("WidgetTypeID")
		private int widgetTypeID;
		
		@JsonProperty("relatedCustomDashboardID")
		private int relatedCustomDashboardID;
		
		@JsonProperty("sqlServerId")
		private List<Integer> sqlServerId;

		public int getMatch() {
			return match;
		}

		public int getMetricID() {
			return metricID;
		}

		public List<Integer> getTagId() {
			return tagId;
		}

		public int getWidgetID() {
			return widgetID;
		}

		public String getWidgetName() {
			return widgetName;
		}

		public int getWidgetTypeID() {
			return widgetTypeID;
		}

		public int getRelatedCustomDashboardID() {
			return relatedCustomDashboardID;
		}

		public List<Integer> getSqlServerId() {
			return sqlServerId;
		}
		
		
}
