package com.idera.sqldm.data.databases;

import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.LinkedHashMap;

import org.apache.commons.lang.StringUtils;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.Utility;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;
import com.idera.i18n.I18NStrings;

@JsonIgnoreProperties(ignoreUnknown = true)
public class InstanceDetailsDatabase {

	@JsonProperty("DatabaseName")
	private String DatabaseName;
	@JsonProperty("LatestStatsCollectionDateTimeUtc")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date LatestStatsCollectionDateTimeUtc;
	@JsonProperty("LatestSizeCollectionDateTimeUtc")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date LatestSizeCollectionDateTimeUtc;
	@JsonProperty("InstanceId")
	private Integer InstanceId;
	@JsonProperty("CurrentLogFileSizeInMb")
	private Double CurrentLogFileSizeInMb;
	@JsonProperty("Transactions")
	private Integer Transactions;
	@JsonProperty("IsSystemDatabase")
	private Boolean IsSystemDatabase;
	@JsonProperty("CurrentDatabaseStatus")
	private String CurrentDatabaseStatus;
	@JsonProperty("CurrentDatabaseState")
	private String CurrentDatabaseState;
	@JsonProperty("CreationDateTimeUtc")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date CreationDateTimeUtc;
	@JsonProperty("CurrentDataSizeInMb")
	private Double CurrentDataSizeInMb;
	@JsonProperty("CurrentLogSizeInMb")
	private Double CurrentLogSizeInMb;
	@JsonProperty("CurrentDataFileSizeInMb")
	private Double CurrentDataFileSizeInMb;
	@JsonProperty("CurrentTotalFileSizeInMb")
	private Double CurrentTotalFileSizeInMb;
	@JsonProperty("DatabaseId")
	private Integer DatabaseId;
	@JsonProperty("CurrentTotalSizeInMb")
	private Double CurrentTotalSizeInMb;
	@JsonProperty("ActiveAlertCount")
	private Integer ActiveAlertCount;
	@JsonProperty("TotalAlertCount")
	private Integer TotalAlertCount;
	@JsonProperty("IsInstanceEnabled")
	private Boolean IsInstanceEnabled;
	@JsonProperty("CurrentTextSizeInMb")
	private Double CurrentTextSizeInMb;
	@JsonProperty("CurrentIndexSizeInMb")
	private Double CurrentIndexSizeInMb;
	
	public Double getCurrentTextSizeInMb() {
		return CurrentTextSizeInMb;
	}
	
	public Double getCurrentIndexSizeInMb() {
		return CurrentIndexSizeInMb;
	}
	public Integer getNoOfFiles() {
		return (noOfDataFiles == null ? 0 : noOfDataFiles)  + (noOfLogFiles == null ? 0 : noOfLogFiles);
	}

	public String getRecoveryModel() {
		return RecoveryModel;
	}

	public Double getUnusedDataSizeInMb() {
		//return UnusedDataSizeInMb == null ? 0d : UnusedDataSizeInMb;
		return CurrentDataFileSizeInMb - CurrentDataSizeInMb - CurrentIndexSizeInMb - CurrentTextSizeInMb;
	}

	public Double getUnusedLogSizeInMb() {
		return UnusedLogSizeInMb == null ? 0d : UnusedLogSizeInMb;
	}

	@JsonProperty("noOfDataFiles")
	private Integer noOfDataFiles;
	
	public Integer getNoOfDataFiles() {
		return noOfDataFiles;
	}

	@JsonProperty("noOfLogFiles")
	private Integer noOfLogFiles;
	
	public Integer getNoOfLogFiles() {
		return noOfLogFiles;
	}

	@JsonProperty("RecoveryModel")
	private String RecoveryModel;
	
	@JsonProperty("LastBackupDate")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date LastBackUpdate;
	public Date getLastBackUpdate() {
		return LastBackUpdate;
	}

	@JsonProperty("UnUsedDataSizeInMb")
	private Double UnusedDataSizeInMb;
	
	@JsonProperty("UnUsedLogSizeInMb")
	private Double UnusedLogSizeInMb;

	public String getDatabaseName() {
		return StringUtils.trimToEmpty(DatabaseName);
	}

	public Date getLatestStatsCollectionDateTimeUtc() {
		return LatestStatsCollectionDateTimeUtc;
	}

	public Date getLatestSizeCollectionDateTimeUtc() {
		return LatestSizeCollectionDateTimeUtc;
	}

	public Integer getInstanceId() {
		return InstanceId;
	}

	public Double getCurrentLogFileSizeInMb() {
		return CurrentLogFileSizeInMb == null ? 0d : CurrentLogFileSizeInMb;
	}

	public Integer getTransactions() {
		return Transactions;
	}

	public Boolean isSystemDatabase() {
		if (IsSystemDatabase != null) {
			return IsSystemDatabase;
		} else {
			return false;
		}
	}

	public Double getCurrentDataSizeInMb() {
		return CurrentDataSizeInMb;
	}

	public String getCurrentDatabaseStatus() {
		return CurrentDatabaseStatus;
	}

	public String getCurrentDatabaseState() {
		return CurrentDatabaseState;
	}

	public Date getCreationDateTimeUtc() {
		return CreationDateTimeUtc;
	}

	public Double getCurrentDataFileSizeInMb() {
		return CurrentDataFileSizeInMb == null ? 0d : CurrentDataFileSizeInMb;
	}

	public Double getCurrentTotalFileSizeInMb() {
		return CurrentTotalFileSizeInMb;
	}

	public Integer getDatabaseId() {
		return DatabaseId;
	}

	public Double getCurrentTotalSizeInMb() {
		return CurrentTotalSizeInMb;
	}

	public Integer getActiveAlertCount() {
		return ActiveAlertCount;
	}

	public Integer getTotalAlertCount() {
		return TotalAlertCount;
	}

	public Boolean getIsInstanceEnabled() {
		return IsInstanceEnabled;
	}

	public Double getCurrentLogSizeInMb() {
		return CurrentLogSizeInMb;
	}

	public String getDatabaseType() {
		// TODO: Correct values when there's null value. Remove null check for bean.
		return isSystemDatabase() ? Utility
				.getMessage(I18NStrings.DATABASE_TYPE_SYSTEM) : Utility
				.getMessage(I18NStrings.DATABASE_TYPE_USER);
	}

	public static HashMap<String, ArrayList<String>> getMap() {
		
		LinkedHashMap<String, ArrayList<String>> map = new LinkedHashMap<>();
		
		ArrayList<String> columnsList = new ArrayList<>();
		
		columnsList.add("getDatabaseName");		
		map.put(I18NStrings.DATABASE, columnsList);
		
		columnsList = new ArrayList<>();
		columnsList.add("getCreationDateTimeUtc");
		map.put(com.idera.sqldm.i18n.SQLdmI18NStrings.DATABASE_CREATION_DATE, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getCurrentDataSizeInMb");
		map.put(com.idera.sqldm.i18n.SQLdmI18NStrings.DATABASE_DATA_SIZE, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getCurrentLogFileSizeInMb");
		map.put(com.idera.sqldm.i18n.SQLdmI18NStrings.DATABASE_LOG_SIZE, columnsList);

		columnsList = new ArrayList<>();
		columnsList.add("getCurrentDatabaseStatus");
		map.put(I18NStrings.STATUS, columnsList);
		
		return map;
	}

}
