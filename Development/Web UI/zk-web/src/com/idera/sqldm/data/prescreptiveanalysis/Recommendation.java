package com.idera.sqldm.data.prescreptiveanalysis;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class Recommendation {
	
	@JsonProperty("AnalysisRecommendationID")
	private String analysisRecommendationID;
	
	@JsonProperty("Category")
	private String category;
	
	@JsonProperty("ComputedRankFactor")
	private Integer computedRankFactor;
	
	@JsonProperty("Database")
	private String database;

	@JsonProperty("DaysSinceLastCheck")
	private String daysSinceLastCheck;
	
	@JsonProperty("DescriptionText")
	private String descriptionText;
	
	@JsonProperty("FindingText")
	private String findingText;
	
	@JsonProperty("ID")
	private String id;
	
	@JsonProperty("ImpactExplanationText")
	private String impactExplanationText;
	
	@JsonProperty("ImpactFactor")
	private Integer impactFactor;
	
	@JsonProperty("IsFlagged")
	private boolean isFlagged;
	
	@JsonProperty("Links")
	private Object link;
	
	@JsonProperty("OptimizationErrorMessage")
	private String optimizationErrorMessage;

	@JsonProperty("OptimizationStatus")
	private Integer optimizationStatus;
	
	@JsonProperty("ProblemExplanationText")
	private String problemExplanationText;
	
	@JsonProperty("RecommendationText")
	private String recommendationText;

	@JsonProperty("RecommendationType")
	private Integer recommendationType;
	
	@JsonProperty("Relevance")
	private Integer relevance;
	
	@JsonProperty("Table")
	private String table;
	
	@JsonProperty("Properties")
	private List<Property> properties;
	
	@JsonProperty("RecommendationLinks")
	private List<RecommendationLink> recommendationLinks;

	public String getAnalysisRecommendationID() {
		return analysisRecommendationID;
	}

	public void setAnalysisRecommendationID(String analysisRecommendationID) {
		this.analysisRecommendationID = analysisRecommendationID;
	}

	public String getCategory() {
		return category;
	}

	public void setCategory(String category) {
		this.category = category;
	}

	public Integer getComputedRankFactor() {
		return computedRankFactor;
	}

	public void setComputedRankFactor(Integer computedRankFactor) {
		this.computedRankFactor = computedRankFactor;
	}

	public String getDatabase() {
		return database;
	}

	public void setDatabase(String database) {
		this.database = database;
	}

	public String getDaysSinceLastCheck() {
		return daysSinceLastCheck;
	}

	public void setDaysSinceLastCheck(String daysSinceLastCheck) {
		this.daysSinceLastCheck = daysSinceLastCheck;
	}

	public String getDescriptionText() {
		return descriptionText;
	}

	public void setDescriptionText(String descriptionText) {
		this.descriptionText = descriptionText;
	}

	public String getFindingText() {
		return findingText;
	}

	public void setFindingText(String findingText) {
		this.findingText = findingText;
	}

	public String getId() {
		return id;
	}

	public void setId(String id) {
		this.id = id;
	}

	public String getImpactExplanationText() {
		return impactExplanationText;
	}

	public void setImpactExplanationText(String impactExplanationText) {
		this.impactExplanationText = impactExplanationText;
	}

	public Integer getImpactFactor() {
		return impactFactor;
	}

	public void setImpactFactor(Integer impactFactor) {
		this.impactFactor = impactFactor;
	}

	public boolean isFlagged() {
		return isFlagged;
	}

	public void setFlagged(boolean isFlagged) {
		this.isFlagged = isFlagged;
	}

	public Object getLink() {
		return link;
	}

	public void setLink(Object link) {
		this.link = link;
	}

	public String getOptimizationErrorMessage() {
		return optimizationErrorMessage;
	}

	public void setOptimizationErrorMessage(String optimizationErrorMessage) {
		this.optimizationErrorMessage = optimizationErrorMessage;
	}

	public Integer getOptimizationStatus() {
		return optimizationStatus;
	}

	public void setOptimizationStatus(Integer optimizationStatus) {
		this.optimizationStatus = optimizationStatus;
	}

	public String getProblemExplanationText() {
		return problemExplanationText;
	}

	public void setProblemExplanationText(String problemExplanationText) {
		this.problemExplanationText = problemExplanationText;
	}

	public String getRecommendationText() {
		return recommendationText;
	}

	public void setRecommendationText(String recommendationText) {
		this.recommendationText = recommendationText;
	}

	public Integer getRecommendationType() {
		return recommendationType;
	}

	public void setRecommendationType(Integer recommendationType) {
		this.recommendationType = recommendationType;
	}

	public Integer getRelevance() {
		return relevance;
	}

	public void setRelevance(Integer relevance) {
		this.relevance = relevance;
	}

	public String getTable() {
		return table;
	}

	public void setTable(String table) {
		this.table = table;
	}

	public List<Property> getProperties() {
		return properties;
	}

	public void setProperties(List<Property> properties) {
		this.properties = properties;
	}

	public List<RecommendationLink> getRecommendationLinks() {
		return recommendationLinks;
	}

	public void setRecommendationLinks(List<RecommendationLink> recommendationLinks) {
		this.recommendationLinks = recommendationLinks;
	}

	@Override
	public String toString() {
		return "PADetails [analysisRecommendationID=" + analysisRecommendationID + ", category=" + category
			+ ", computedRankFactor=" + computedRankFactor + ", database=" 
			+ database + ", daysSinceLastCheck="
			+ daysSinceLastCheck + ", descriptionText=" + descriptionText + ", findingText=" + findingText + ", id="
			+ id + ", impactExplanationText=" + impactExplanationText + ", impactFactor=" + impactFactor
			+ ", isFlagged=" + isFlagged + ", link=" + link + ", optimizationErrorMessage="
			+ optimizationErrorMessage + ", optimizationStatus=" + optimizationStatus + ", problemExplanationText="
			+ problemExplanationText + ", recommendationText=" + recommendationText + ", recommendationType="
			+ recommendationType + ", relevance=" + relevance + ", table=" 
			+ table + ", properties=" + properties + ", recommendationLinks=" + recommendationLinks + "]";
	}
}
