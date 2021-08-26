/////-----------Auth:Rishabh Mishra---------------
package com.idera.sqldm_10_3.ui.dashboard;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.idera.sqldm_10_3.ui.dashboard.HealthIndexCoefficients;
import com.idera.sqldm_10_3.ui.dashboard.InstanceScaleFactorList;
import com.idera.sqldm_10_3.ui.dashboard.TagsScaleFactorList;

import java.util.ArrayList;
import java.util.List;

@JsonIgnoreProperties(ignoreUnknown = true)
public class GearViewBean {
	@JsonProperty("HealthIndexCoefficients")
	private HealthIndexCoefficients HealthIndexCoefficients;
	@JsonProperty("InstanceScaleFactorList")
	private List<InstanceScaleFactorList> instanceList = new ArrayList<InstanceScaleFactorList>();
	@JsonProperty("TagScaleFactorList")
	private List<TagsScaleFactorList> tagsList = new ArrayList<TagsScaleFactorList>();

	public HealthIndexCoefficients getHealthIndexCoefficients() {
		return HealthIndexCoefficients;
	}

	public void setHealthIndexCoefficients(HealthIndexCoefficients healthIndexCoefficients) {

		this.HealthIndexCoefficients = healthIndexCoefficients;
	}

	public List<InstanceScaleFactorList> getInstanceList() {
		return instanceList;
	}

	public void setInstanceList(List<InstanceScaleFactorList> instanceList) {
		this.instanceList = instanceList;
	}

	public List<TagsScaleFactorList> getTagsList() {
		return tagsList;
	}

	public void setTagsList(List<TagsScaleFactorList> tagsList) {
		this.tagsList = tagsList;
	}

}
