package com.idera.sqldm_10_3.ui.dashboard;

import com.fasterxml.jackson.annotation.JsonProperty;

import java.util.ArrayList;
import java.util.List;

public class AnalysisSummary {
	@JsonProperty("PreviousAnalysisInformation")
	private PreviousAnalysisInformation previousAnalysisInformation;

	@JsonProperty("RecommendationSummary")
	private List<RecommendationSummary> recommendationSummary = new ArrayList<RecommendationSummary>();
	
	public PreviousAnalysisInformation getPreviousAnalysisInformation() {
		return previousAnalysisInformation;
	}
	public void setPreviousAnalysisInformation(PreviousAnalysisInformation previousAnalysisInformation) {
		this.previousAnalysisInformation = previousAnalysisInformation;
	}
	public List<RecommendationSummary> getRecommendationSummaryList() {
		return recommendationSummary;
	}
	public void setRecommendationSummaryList(List<RecommendationSummary> recommendationSummaryList) {
		recommendationSummary = recommendationSummaryList;
	}
}
