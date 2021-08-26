package com.idera.sqldm.data;

public class TopTenFacade {
	
	public static class Top10EventDataBean {
		private int count;
		private int days;
		
		public Top10EventDataBean(int count, int days) {
			super();
			this.count = count;
			this.days = days;
		}
		
		public int getCount() {
			return count;
		}
		public int getDays() {
			return days;
		}
		
	}

}
