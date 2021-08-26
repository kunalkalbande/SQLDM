package com.idera.sqldm.ui.dashboard.instances.queries;

public enum QueryTimePeriodsEnum {

	THIRTY_MINUTES("Last 30 minutes", 0), ONE_HOUR("Last 1 Hour", 1), FOUR_HOURS(
			"Last 4 Hours", 2), EIGHT_HOURS("Last 8 Hours", 3), TWENTY_FOUR_HOURS(
			"Last 24 Hours", 4), SEVEN_DAYS("Last 7 Days", 5), THIRTY_DAYS(
			"Last 30 Days", 6), CUSTOM("Custom", 7);

	private String timePeriod;
	private int index;

	private QueryTimePeriodsEnum(String timePeriod, int index) {
		this.timePeriod = timePeriod;
		this.index = index;
	}

	public String getTimePeriod() {
		return timePeriod;
	}

	public void setTimePeriod(String timePeriod) {
		this.timePeriod = timePeriod;
	}

	public int getIndex() {
		return index;
	}

	public void setIndex(int index) {
		this.index = index;
	}

}
