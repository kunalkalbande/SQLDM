package com.idera.sqldm.data.customdashboard;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class CustomDashboard {
	
		@JsonProperty("CustomDashboardId")
		private int customDashboardId;
		
		@JsonProperty("CustomDashboardName")
		private String customDashboardName;
		
		@JsonProperty("IsDefaultOnUI")
		private boolean isDefaultOnUI;
		
		@JsonProperty("SID")
		private String sid;
		
		@JsonProperty("TagsDashboard")
		private List<String> tags;
		
		public String getCustomDashboardName() {
			return this.customDashboardName;
		}
		
		public boolean getIsDefaultOnUI() {
			return this.isDefaultOnUI;
		}
		
		public String getSID() {
			return this.sid;
		}
		
		public int getCustomDashboardId() {
			return this.customDashboardId;
		}
		
		public List<String> getTags() {
			return this.tags;
		}
		
		public void setCustomDashboardName(String customDashboardName) {
			this.customDashboardName = customDashboardName;
		}
		
		public void setIsDefaultOnUI(boolean isDefaultOnUI) {
			this.isDefaultOnUI = isDefaultOnUI;
		}
		
		public void setTags(List<String> tags) {
			this.tags = tags;
		}
		
		public boolean equals(Object o) {
			if(o instanceof CustomDashboard) {
				return this.customDashboardId == ((CustomDashboard)o).customDashboardId;
			}
			return false;
		}
}
