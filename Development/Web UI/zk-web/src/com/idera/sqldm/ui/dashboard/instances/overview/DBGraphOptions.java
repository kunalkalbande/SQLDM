package com.idera.sqldm.ui.dashboard.instances.overview;

import java.util.ArrayList;
import java.util.List;

public class DBGraphOptions{
	public enum DBGraphOptionEnum {
		TRANS_PER_SEC("Transactions/sec", 1, CustomizationConstants.TRANS_DB_MODEL),
		LOG_FLUSHES_PER_SEC("Log Flushes/sec",2, CustomizationConstants.LOG_FLUSHES_MODEL),
		READS_PER_SEC("Reads/sec", 3, CustomizationConstants.READS_DB_MODEL),
		WRITES_PER_SEC("Writes/sec", 4,CustomizationConstants.WRITES_DB_MODEL),
		IO_STALL_PER_SEC("I/O Stall ms/sec",5, CustomizationConstants.IO_STALL_DB_MODEL);

		private int defaultSequence;
		private String optionName;
		private String modelName;

		DBGraphOptionEnum(String optionName, int defaultSequence, String modelName) {
			this.setOptionName(optionName);
			this.setDefaultSequence(defaultSequence);
			this.setModelName(modelName);
		}

		public int getDefaultSequence() {
			return defaultSequence;
		}

		public void setDefaultSequence(int defaultSequence) {
			this.defaultSequence = defaultSequence;
		}

		public String getOptionName() {
			return optionName;
		}

		public void setOptionName(String optionName) {
			this.optionName = optionName;
		}

		public String getModelName() {
			return modelName;
			
		}

		public void setModelName(String modelName) {
			this.modelName = modelName;
		}
	}

	public static List<String> getDefaultOptionNameList() {
		// TODO Auto-generated method stub
		List<String> list = new ArrayList<String>();
		DBGraphOptionEnum[] values = DBGraphOptionEnum.values();
		for (DBGraphOptionEnum dbGraphOption : values) {
			list.add(dbGraphOption.getOptionName());
		}
		return list;
	}
	public static DBGraphOptionEnum getDBGraphOptionEnum(String categoryName){
		if(DBGraphOptionEnum.TRANS_PER_SEC.getOptionName().equalsIgnoreCase(categoryName)){
			return DBGraphOptionEnum.TRANS_PER_SEC;
		}
		else if(DBGraphOptionEnum.LOG_FLUSHES_PER_SEC.getOptionName().equalsIgnoreCase(categoryName)){
			return DBGraphOptionEnum.LOG_FLUSHES_PER_SEC;
		}
		else if(DBGraphOptionEnum.IO_STALL_PER_SEC.getOptionName().equalsIgnoreCase(categoryName)){
			return DBGraphOptionEnum.IO_STALL_PER_SEC;
		}
		else if(DBGraphOptionEnum.READS_PER_SEC.getOptionName().equalsIgnoreCase(categoryName)){
			return DBGraphOptionEnum.READS_PER_SEC;
		}
		else if(DBGraphOptionEnum.WRITES_PER_SEC.getOptionName().equalsIgnoreCase(categoryName)){
			return DBGraphOptionEnum.WRITES_PER_SEC;
		}
		else return null;
	}

}
