package com.idera.sqldm.ui.dashboard;

import java.text.ParseException;
import java.util.Date;

import org.zkoss.zk.ui.Sessions;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.idera.common.rest.DataContractDateDeserializer;
import com.idera.common.rest.DataContractUtcDateSerializer;
import com.idera.sqldm.utils.Utility;

public class PreviousAnalysisInformation {
	@JsonProperty("AnalyisType")
	private String AnalysisType;
	@JsonProperty("Duration")
	private String Duration;
	@JsonProperty("StartedOn")
	@JsonDeserialize(using = DataContractDateDeserializer.class)
	@JsonSerialize(using = DataContractUtcDateSerializer.class)
	private Date StartedOn;

	public String getAnalysisType() {
		return AnalysisType;
	}

	public void setAnalysisType(String analysisType) {
		AnalysisType = analysisType;
	}

	public Date getStartedOn() throws ParseException {
		
		return this.StartedOn;
	}

	public void setStartedOn(Date startedOn) {
		StartedOn = startedOn;
	}

	/*public String getDuration() {
		String s[] = Duration.split(":");
		if (s.length > 1)
			Duration = s[0] + "h " + s[1] + "m " + s[2] + "s";
		return "" + Duration;
	}*/
	public String getDuration() {
		if (Duration != null) {
			String s[] = Duration.split(":");
			if (s.length > 1)
				Duration = s[0] + "h " + s[1] + "m " + s[2] + "s";
		}
		return Duration;
	}

	public void setDuration(String duration) {
		Duration = duration;
	}
}
