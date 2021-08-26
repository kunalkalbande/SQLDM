package com.idera.sqldm.data;

import org.zkoss.bind.annotation.DependsOn;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class AlertCategoryWiseMaxSeverity {


	@JsonProperty("Cpu") private Integer Cpu;
	@JsonProperty("Memory") private Integer Memory;
	@JsonProperty("IO") private Integer IO;
	@JsonProperty("Logs") private Integer Logs;
	@JsonProperty("Databases") private Integer Databases;
	@JsonProperty("Queries") private Integer Queries;
	@JsonProperty("Services") private Integer Services;
	@JsonProperty("Sessions") private Integer Sessions;
	@JsonProperty("Virtualization") private Integer Virtualization;
	@JsonProperty("Operational") private Integer Operational;
	
	@DependsOn("Cpu")
	public Integer getCpu() {
		return getSeverity(Cpu);
	}

	@DependsOn("Memory")
	public Integer getMemory() {
		return getSeverity(Memory);
	}

	@DependsOn("IO")
	public Integer getIO() {
		return getSeverity(IO);
	}

	@DependsOn("Logs")
	public Integer getLogs() {
		return getSeverity(Logs);
	}

	@DependsOn("Databases")
	public Integer getDatabases() {
		return getSeverity(Databases);
	}

	@DependsOn("Queries")
	public Integer getQueries() {
		return getSeverity(Queries);
	}
	
	public void setCpu(Integer cpu) {
		Cpu = cpu;
	}
	public void setMemory(Integer memory) {
		Memory = memory;
	}
	public void setIO(Integer iO) {
		IO = iO;
	}
	public void setLogs(Integer logs) {
		Logs = logs;
	}
	public void setDatabases(Integer databases) {
		Databases = databases;
	}
	public void setQueries(Integer query) {
		Queries = query;
	}
	
	public Integer getSeverity(Integer categorySeverity) {
		if(categorySeverity == null)
			return SeverityCodeToStringEnum.OK.getId();
		return categorySeverity;
	}

	@DependsOn("Services")
	public Integer getServices() {
		return getSeverity(Services);
	}

	public void setServices(Integer services) {
		Services = services;
	}

	@DependsOn("Sessions")
	public Integer getSessions() {
		return getSeverity(Sessions);
	}

	public void setSessions(Integer sessions) {
		Sessions = sessions;
	}

	@DependsOn("Virtualization")
	public Integer getVirtualization() {
		return getSeverity(Virtualization);
	}

	public void setVirtualization(Integer virtualization) {
		Virtualization = virtualization;
	}

	@DependsOn("Operational")
	public Integer getOperational() {
		return getSeverity(Operational);
	}

	public void setOperational(Integer operational) {
		Operational = operational;
	}
	
	Integer computeMaxSeverity() {
		int sevId = SeverityCodeToStringEnum.OK.getId();
		if(Cpu !=null)
			sevId =  Math.max(sevId,getCpu());
		if(Databases !=null)
			sevId =  Math.max(sevId,getDatabases());
		if(IO !=null)
			sevId =  Math.max(sevId,getIO());
		if(Logs !=null)
			sevId =  Math.max(sevId,getLogs());
		if(Memory !=null)
			sevId =  Math.max(sevId,getMemory());
		if(Operational !=null)
			sevId =  Math.max(sevId,getOperational());
		if(Queries !=null)
			sevId =  Math.max(sevId,getQueries());
		if(Services !=null)
			sevId =  Math.max(sevId,getServices());
		if(Sessions !=null)
			sevId =  Math.max(sevId,getSessions());
		if(Virtualization !=null)
			sevId =  Math.max(sevId,getVirtualization());
		
		return sevId;
	}

}
