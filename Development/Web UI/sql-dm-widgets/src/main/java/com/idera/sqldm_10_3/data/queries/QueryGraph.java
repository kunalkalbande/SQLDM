/*
 * {
 BucketStartDateTime:"09-11-2014 23:33:33",
 BucketEndDateTime: "10-11-2014 23:33:33",
 BucketType:"hours",
 YAxisValue:"450",
 GroupByName:"SQL Server Management Studio",
 GroupByID:"1"
 }

 * */

package com.idera.sqldm_10_3.data.queries;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm_10_3.data.instances.ServerStatus;

@JsonIgnoreProperties(ignoreUnknown = true)
public class QueryGraph {

	private static final ServerStatus DEFAULT_SERVER_STATUS = new ServerStatus();

	@JsonProperty("BucketStartDateTime")
	private String BucketStartDateTime;
	
	@JsonProperty("BucketEndDateTime")
	private String BucketEndDateTime;
	
	@JsonProperty("BucketType")
	private String BucketType;
	
	@JsonProperty("YAxisValue")
	private String YAxisValue;
	
	@JsonProperty("GroupByName")
	private String GroupByName;
	
	@JsonProperty("GroupByID")
	private String GroupByID;

	public String getBucketStartDateTime() {
		return BucketStartDateTime;
	}
	public void setBucketStartDateTime(String bucketStartDateTime) {
		BucketStartDateTime = bucketStartDateTime;
	}
	public String getBucketEndDateTime() {
		return BucketEndDateTime;
	}
	public void setBucketEndDateTime(String bucketEndDateTime) {
		BucketEndDateTime = bucketEndDateTime;
	}
	public String getBucketType() {
		return BucketType;
	}
	public void setBucketType(String bucketType) {
		BucketType = bucketType;
	}
	public String getYAxisValue() {
		return YAxisValue;
	}
	public void setYAxisValue(String yAxisValue) {
		YAxisValue = yAxisValue;
	}
	public String getGroupByName() {
		if(GroupByName == null || GroupByName.equals(""))
			GroupByName = "<blank>";
		return GroupByName;
	}
	public void setGroupByName(String groupByName) {
		GroupByName = groupByName;
	}
	public String getGroupByID() {
		return GroupByID;
	}
	public void setGroupByID(String groupByID) {
		GroupByID = groupByID;
	}
	public static ServerStatus getDefaultServerStatus() {
		return DEFAULT_SERVER_STATUS;
	}

}
