package com.idera.sqldm.ui.topten;

public enum TopXTimeFrameEnum {
	LAST_24_HOURS ("Last 24 Hours", 1),
	LAST_7_DAYS ("Last 7 Days", 7);
	
	private String message;
	private int days;
	
	private TopXTimeFrameEnum(String message, int days) {
		this.message = message;
		this.days = days;
	}

	public String getMessage() {
		return message;
	}

	public int getDays() {
		return days;
	}
	
	public static TopXTimeFrameEnum getDaysFromEnum(String message) {
		if (message == null) {
			return null;
		}
		for (TopXTimeFrameEnum topXTimeFrameEnum : TopXTimeFrameEnum.values()) {
			if ( message.toUpperCase().equals(topXTimeFrameEnum.getMessage().toUpperCase()) ) {
				return topXTimeFrameEnum;
			}
		}
		
		return null;
	} 
}
