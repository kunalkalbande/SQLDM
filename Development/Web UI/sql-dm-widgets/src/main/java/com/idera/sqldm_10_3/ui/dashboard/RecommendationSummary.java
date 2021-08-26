package com.idera.sqldm_10_3.ui.dashboard;

import com.fasterxml.jackson.annotation.JsonProperty;

public class RecommendationSummary {
	@JsonProperty("Category")
	private String category;
	@JsonProperty("Priority")
	private String priority;
	@JsonProperty("Recommendations")
	private String recommendations;

	public String getCategory() {
		return category;
	}

	public void setCategory(String category) {

		this.category = category;
	}

	public String getRecommendations() {
		return recommendations;
	}

	public void setRecommendation(String recommendations) {
		this.recommendations = recommendations;
	}

	public String getPriority() {
		/*String s[] = priority.split(".");
		if (s.length > 0)
			this.priority */
		Double d = Double.parseDouble(priority);
		this.priority = ""+d.intValue();
		return this.priority;
	}

	public void setPriority(String priority) {
		this.priority = priority;
	}
}
