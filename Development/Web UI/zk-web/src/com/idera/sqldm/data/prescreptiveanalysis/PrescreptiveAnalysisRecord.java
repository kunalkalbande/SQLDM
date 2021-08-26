package com.idera.sqldm.data.prescreptiveanalysis;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * 
 * @author akshay.ghaisas
 *
 */
@JsonIgnoreProperties(ignoreUnknown = true)
public class PrescreptiveAnalysisRecord {
	
	@JsonProperty("AnalysisCompleteTime")
	private String analysisCompleteTime;
	
	@JsonProperty("AnalysisDuration")
	private String analysisDuration;
	
	@JsonProperty("AnalysisID")
	private Integer analysisID;
	
	@JsonProperty("AnalysisStartTime")
	private String analysisStartTime;

	@JsonProperty("ComputedRankFactor")
	private Float computedRankFactor;
	
	@JsonProperty("Error")
	private String error;
	
	@JsonProperty("SQLServerID")
	private Integer sQLServerID;
	
	@JsonProperty("TotalRecommendationCount")
	private Integer totalRecommendationCount;
	
	@JsonProperty("Type")
	private String type;

	
	public String getAnalysisCompleteTime() {
		return analysisCompleteTime;
	}

	public void setAnalysisCompleteTime(String analysisCompleteTime) {
		this.analysisCompleteTime = analysisCompleteTime;
	}

	public String getAnalysisDuration() {
		return analysisDuration;
	}

	public void setAnalysisDuration(String analysisDuration) {
		this.analysisDuration = analysisDuration;
	}

	public Integer getAnalysisID() {
		return analysisID;
	}

	public void setAnalysisID(Integer analysisID) {
		this.analysisID = analysisID;
	}

	public String getAnalysisStartTime() {
		return analysisStartTime;
	}

	public void setAnalysisStartTime(String analysisStartTime) {
		this.analysisStartTime = analysisStartTime;
	}

	public Float getComputedRankFactor() {
		return computedRankFactor;
	}

	public void setComputedRankFactor(Float computedRankFactor) {
		this.computedRankFactor = computedRankFactor;
	}

	public String getError() {
		return error;
	}

	public void setError(String error) {
		this.error = error;
	}

	public Integer getsQLServerID() {
		return sQLServerID;
	}

	public void setsQLServerID(Integer sQLServerID) {
		this.sQLServerID = sQLServerID;
	}

	public Integer getTotalRecommendationCount() {
		return totalRecommendationCount;
	}

	public void setTotalRecommendationCount(Integer totalRecommendationCount) {
		this.totalRecommendationCount = totalRecommendationCount;
	}

	public String getType() {
		return type;
	}

	public void setType(String type) {
		this.type = type;
	}

	@Override
	public String toString() {
		return "PrescreptiveAnalysisRecord [analysisCompleteTime=" + analysisCompleteTime + ", analysisDuration="
				+ analysisDuration + ", analysisID=" + analysisID + ", analysisStartTime=" + analysisStartTime
				+ ", computedRankFactor=" + computedRankFactor + ", error=" + error + ", sQLServerID=" + sQLServerID
				+ ", totalRecommendationCount=" + totalRecommendationCount + ", type=" + type + "]";
	}
	
	
	
}
