package com.idera.sqldm.data.instances;


public class GraphCategories {

		public enum GraphCategoriesEnum {
			
			CACHE("Cache", 1),
			CPU("Cpu",2),
			CUSTOM_COUNTERS("Custom Counters", 3),
			DATABASES("Databases", 4),
			DISK("Disk", 5),
			FILE_ACTIVITY("File Activity", 6),
			LOCK_WAITS("Lock Waits", 7),
			MEMORY("Memory", 8),
			NETWORK("Network", 9),
			SERVER_WAITS("Server Waits", 10),
			SESSIONS("Sessions", 11),
			TEMPDB("tempDB", 12),
			VIRTUALIZATION("Virtualization", 13);
			
			private String categoryName;
			private int defaultPosition;
			
			GraphCategoriesEnum(String categoryName, int defaultPosition) {
				this.categoryName = categoryName;
				this.defaultPosition = defaultPosition;
			}
			
			public String getCategoryName() {
				return categoryName;
			}

			public int getDefaultPosition() {
				return defaultPosition;
			}
			
			
		}
		
		GraphCategoriesEnum category;

		private int defaultPosition;
		private int position;
		boolean visible = false;
		int tempPosition;
		boolean tempVisible;
		
		public GraphCategories(GraphCategoriesEnum grapghCatEnum) {
			this.category = grapghCatEnum;
			this.position = grapghCatEnum.defaultPosition;
			this.tempPosition = category.defaultPosition;
			this.visible = false;
			this.tempVisible = false;
		}

		public int getDefaultPosition() {
			return defaultPosition;
		}

		public int getPosition() {
			return position;
		}
		
		public void setPosition(int position) {
			this.position = position;
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
		
		public GraphCategoriesEnum getCategory() {
			return category;
		}
		/*public static List<GraphCategories> getDefaultList() {
			
			List<GraphCategories> list = new ArrayList<>();
			for(GraphCategoriesEnum ace: GraphCategoriesEnum.values()) {
				list.add(ace.defaultPosition-1, new GraphCategories(ace));				
			}
			return list;
		}*/

		public boolean isVisible() {
			return visible;
		}

		public void setVisible(boolean visible) {
			this.visible = visible;
		}

	
		/*public static HashMap<String, GraphCategories> getDefaultNameVsModelMap() {
			HashMap<String, GraphCategories> map = new HashMap<String, GraphCategories>();
			for(GraphCategoriesEnum ace: GraphCategoriesEnum.values()) {
				map.put(ace.getCategoryName(),  new GraphCategories(ace));
			}
			return map;
		}

		*/

}
