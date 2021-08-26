package com.idera.sqldm.data.alerts;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class AlertCategoriesDetails {

	public enum AlertCategoriesEnum {
		
		CPU("Cpu", 1, new Integer[]{0, 26, 27, 28, 29}),
		MEMORY("Memory", 2, new Integer[]{13, 24}),
		IO("I/O", 3, new Integer[] {25, 30, 31, 62, 63, 64, 76, 81, 82, 83, 84, 85, 86, 87, 111}),
		DATABASES("Databases", 4, new Integer[]{7, 8, 9, 11, 14, 20, 68, 69, 70, 71, 72, 73, 74, 75, 89, 90, 91, 92, 93, 94, 95, 109, 110, 116, 117, 118, 119, 120, 121, 122, 123, 124, 126, 127}),
		LOGS("Logs", 5, new Integer[] {66, 67}),
		QUERIES("Queries", 6, new Integer[] {51}),
		SERVICES("Services", 7, new Integer[] {4, 5, 10, 12, 17, 18, 19, 21, 34, 35, 65, 77, 78, 79, 88, 128, 129}),
		SESSIONS("Sessions", 8, new Integer[] {1, 6, 22, 32, 33, 57, 58, 80}),
		VIRTUALIZATION("Virtualization", 9, new Integer[] {96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108}),
		OPERATIONAL("Operational", 10, new Integer[] {23, 48, 49, 50, 52, 53, 54, 56, 59, 60, 125}),
		CUSTOM("Custom", 11, new Integer[] {});
		
		private String categoryName;
		private int defaultPosition;
		private List<Integer> metricIds = null;
		
		AlertCategoriesEnum(String categoryName, int defaultPosition, Integer metricIds[]) {
			this.categoryName = categoryName;
			this.defaultPosition = defaultPosition;
			if(metricIds != null)
				this.metricIds = Arrays.asList(metricIds);
		}
		
		public static String returnCategoryString(int metricId) {
			
			if(AlertCategoriesEnum.CPU.metricIds.contains(metricId)) {
				return AlertCategoriesEnum.CPU.categoryName;
			}
			
			if(AlertCategoriesEnum.MEMORY.metricIds.contains(metricId)) {
				return AlertCategoriesEnum.MEMORY.categoryName;
			}
			
			return AlertCategoriesEnum.IO.categoryName;
			
		}
		
		public static AlertCategoriesEnum returnCategoryEnum(String category) {
			
			for(AlertCategoriesEnum ace: AlertCategoriesEnum.values()) {
				if(ace.getCategoryName().equals(category))
					return ace;
			}
			
			return null;
			
		}
		
		public static List<AlertCategoriesDetails> getDefaultList() {
			
			List<AlertCategoriesDetails> list = new ArrayList<>();
			for(AlertCategoriesEnum ace: AlertCategoriesEnum.values()) {
				list.add(ace.defaultPosition - 1, new AlertCategoriesDetails(ace));				
			}
			
			return list;
		
		}

		public String getCategoryName() {
			return categoryName;
		}

		public int getDefaultPosition() {
			return defaultPosition;
		}

		public List<Integer> getMetricIds() {
			return metricIds;
		}
	}
	
	AlertCategoriesEnum category;
	int position;
	int tempPosition;
	boolean visible = true;
	boolean tempVisible;
	
	public AlertCategoriesDetails() {
	}
	
	public AlertCategoriesDetails(AlertCategoriesEnum category) {
		
		this.category = category;
		this.position = category.defaultPosition;
		this.tempPosition = category.defaultPosition;
		this.visible = true;
		this.tempVisible = true;
		
	}
	
	public AlertCategoriesEnum getCategory() {
		return category;
	}
	
	public void setCategoryName(AlertCategoriesEnum category) {
		this.category = category;
	}
	
	public int getPosition() {
		return position;
	}
	
	public void setPosition(int position) {
		this.position = position;
	}
	
	public boolean isVisible() {
		return visible;
	}
	
	public void setVisible(boolean visibility) {
		this.visible = visibility;
	}

	public int getTempPosition() {
		return tempPosition;
	}

	public void setTempPosition(int tempPosition) {
		this.tempPosition = tempPosition;
	}

	public boolean isTempVisible() {
		return tempVisible;
	}

	public void setTempVisible(boolean tempVisible) {
		this.tempVisible = tempVisible;
	}

	public void setCategory(AlertCategoriesEnum category) {
		this.category = category;
	}
	
}
